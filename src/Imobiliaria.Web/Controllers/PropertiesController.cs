using System.Globalization;
using Imobiliaria.Web.Data;
using Imobiliaria.Web.Models;
using Imobiliaria.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Imobiliaria.Web.Controllers;

public sealed class PropertiesController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index(
        string? query,
        PropertyStatus? status,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var properties = context.Properties.AsNoTracking().Include(property => property.Owner).AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            properties = properties.Where(property =>
                property.Title.Contains(term) ||
                property.Zone.Contains(term) ||
                property.Address.Contains(term));
        }

        if (status.HasValue)
        {
            properties = properties.Where(property => property.Status == status.Value);
        }

        var result = await properties
            .OrderByDescending(property => property.CreatedAtUtc)
            .ToPagedResultAsync(page, 9, cancellationToken);

        return View(new PropertyListViewModel(query, status, result));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var property = await context.Properties
            .AsNoTracking()
            .Include(item => item.Owner)
            .Include(item => item.Visits)
            .ThenInclude(visit => visit.Client)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return property is null ? NotFound() : View(property);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new PropertyFormViewModel
        {
            Owners = await BuildOwnerOptionsAsync(null, cancellationToken),
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        PropertyFormViewModel model,
        CancellationToken cancellationToken)
    {
        await ValidateOwnerAsync(model.OwnerId, cancellationToken);
        if (!ModelState.IsValid)
        {
            model.Owners = await BuildOwnerOptionsAsync(model.OwnerId, cancellationToken);
            return View(model);
        }

        var property = new PropertyListing();
        model.ApplyTo(property);
        context.Properties.Add(property);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Imóvel criado com sucesso.";
        return RedirectToAction(nameof(Details), new { id = property.Id });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var property = await context.Properties.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (property is null)
        {
            return NotFound();
        }

        var model = PropertyFormViewModel.FromEntity(property);
        model.Owners = await BuildOwnerOptionsAsync(model.OwnerId, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        PropertyFormViewModel model,
        CancellationToken cancellationToken)
    {
        var property = await context.Properties.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (property is null)
        {
            return NotFound();
        }

        await ValidateOwnerAsync(model.OwnerId, cancellationToken);
        if (!ModelState.IsValid)
        {
            model.Owners = await BuildOwnerOptionsAsync(model.OwnerId, cancellationToken);
            return View(model);
        }

        model.ApplyTo(property);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Imóvel atualizado com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var property = await context.Properties.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return property is null ? NotFound() : View(property);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var property = await context.Properties.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (property is null)
        {
            return NotFound();
        }

        if (await context.Visits.AnyAsync(visit => visit.PropertyId == id, cancellationToken))
        {
            TempData["Error"] = "Não é possível eliminar um imóvel com visitas associadas.";
            return RedirectToAction(nameof(Details), new { id });
        }

        context.Properties.Remove(property);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Imóvel eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IReadOnlyList<SelectListItem>> BuildOwnerOptionsAsync(
        int? selectedId,
        CancellationToken cancellationToken) =>
        await context.Clients.AsNoTracking()
            .OrderBy(client => client.Name)
            .Select(client => new SelectListItem(
                client.Name,
                client.Id.ToString(CultureInfo.InvariantCulture),
                client.Id == selectedId))
            .ToListAsync(cancellationToken);

    private async Task ValidateOwnerAsync(int? ownerId, CancellationToken cancellationToken)
    {
        if (ownerId.HasValue &&
            !await context.Clients.AnyAsync(client => client.Id == ownerId.Value, cancellationToken))
        {
            ModelState.AddModelError(nameof(PropertyFormViewModel.OwnerId), "O proprietário selecionado não existe.");
        }
    }
}
