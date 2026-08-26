using System.Globalization;
using Imobiliaria.Web.Data;
using Imobiliaria.Web.Models;
using Imobiliaria.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Imobiliaria.Web.Controllers;

public sealed class VisitsController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index(
        string? query,
        bool upcomingOnly = true,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var visits = context.Visits
            .AsNoTracking()
            .Include(visit => visit.Client)
            .Include(visit => visit.Property)
            .AsQueryable();

        if (upcomingOnly)
        {
            visits = visits.Where(visit => visit.ScheduledAt >= DateTime.Today);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            visits = visits.Where(visit =>
                visit.Client.Name.Contains(term) ||
                visit.Property.Title.Contains(term) ||
                visit.Property.Zone.Contains(term));
        }

        var result = await visits
            .OrderBy(visit => visit.ScheduledAt)
            .ToPagedResultAsync(page, 10, cancellationToken);
        return View(new VisitListViewModel(query, upcomingOnly, result));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var visit = await context.Visits
            .AsNoTracking()
            .Include(item => item.Client)
            .Include(item => item.Property)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return visit is null ? NotFound() : View(visit);
    }

    public async Task<IActionResult> Create(
        int? propertyId,
        CancellationToken cancellationToken)
    {
        var model = new VisitFormViewModel
        {
            PropertyId = propertyId ?? 0,
        };
        await PopulateOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        VisitFormViewModel model,
        CancellationToken cancellationToken)
    {
        await ValidateReferencesAsync(model, cancellationToken);
        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model, cancellationToken);
            return View(model);
        }

        var visit = new Visit();
        model.ApplyTo(visit);
        context.Visits.Add(visit);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Visita agendada com sucesso.";
        return RedirectToAction(nameof(Details), new { id = visit.Id });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var visit = await context.Visits.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (visit is null)
        {
            return NotFound();
        }

        var model = VisitFormViewModel.FromEntity(visit);
        await PopulateOptionsAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        VisitFormViewModel model,
        CancellationToken cancellationToken)
    {
        var visit = await context.Visits.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (visit is null)
        {
            return NotFound();
        }

        await ValidateReferencesAsync(model, cancellationToken);
        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model, cancellationToken);
            return View(model);
        }

        model.ApplyTo(visit);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Visita atualizada com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var visit = await context.Visits
            .AsNoTracking()
            .Include(item => item.Client)
            .Include(item => item.Property)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return visit is null ? NotFound() : View(visit);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var visit = await context.Visits.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (visit is null)
        {
            return NotFound();
        }

        context.Visits.Remove(visit);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Visita eliminada.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateOptionsAsync(
        VisitFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Clients = await context.Clients.AsNoTracking()
            .OrderBy(client => client.Name)
            .Select(client => new SelectListItem(
                client.Name,
                client.Id.ToString(CultureInfo.InvariantCulture),
                client.Id == model.ClientId))
            .ToListAsync(cancellationToken);
        model.Properties = await context.Properties.AsNoTracking()
            .OrderBy(property => property.Title)
            .Select(property => new SelectListItem(
                $"{property.Title} · {property.Zone}",
                property.Id.ToString(CultureInfo.InvariantCulture),
                property.Id == model.PropertyId))
            .ToListAsync(cancellationToken);
    }

    private async Task ValidateReferencesAsync(
        VisitFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (!await context.Clients.AnyAsync(client => client.Id == model.ClientId, cancellationToken))
        {
            ModelState.AddModelError(nameof(VisitFormViewModel.ClientId), "O cliente selecionado não existe.");
        }

        if (!await context.Properties.AnyAsync(property => property.Id == model.PropertyId, cancellationToken))
        {
            ModelState.AddModelError(nameof(VisitFormViewModel.PropertyId), "O imóvel selecionado não existe.");
        }
    }
}
