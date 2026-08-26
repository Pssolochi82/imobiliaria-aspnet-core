using System.Globalization;
using Imobiliaria.Web.Data;
using Imobiliaria.Web.Models;
using Imobiliaria.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Imobiliaria.Web.Controllers;

public sealed class InterestsController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var interests = await context.Interests
            .AsNoTracking()
            .Include(interest => interest.Client)
            .OrderBy(interest => interest.PreferredZone)
            .ThenBy(interest => interest.Client.Name)
            .ToListAsync(cancellationToken);
        return View(interests);
    }

    public async Task<IActionResult> Create(
        int? clientId,
        CancellationToken cancellationToken)
    {
        var model = new InterestFormViewModel
        {
            ClientId = clientId ?? 0,
            Clients = await BuildClientOptionsAsync(clientId, cancellationToken),
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        InterestFormViewModel model,
        CancellationToken cancellationToken)
    {
        await ValidateClientAsync(model.ClientId, cancellationToken);
        if (!ModelState.IsValid)
        {
            model.Clients = await BuildClientOptionsAsync(model.ClientId, cancellationToken);
            return View(model);
        }

        var interest = new Interest();
        model.ApplyTo(interest);
        context.Interests.Add(interest);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Interesse registado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var interest = await context.Interests.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (interest is null)
        {
            return NotFound();
        }

        var model = InterestFormViewModel.FromEntity(interest);
        model.Clients = await BuildClientOptionsAsync(model.ClientId, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        InterestFormViewModel model,
        CancellationToken cancellationToken)
    {
        var interest = await context.Interests.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (interest is null)
        {
            return NotFound();
        }

        await ValidateClientAsync(model.ClientId, cancellationToken);
        if (!ModelState.IsValid)
        {
            model.Clients = await BuildClientOptionsAsync(model.ClientId, cancellationToken);
            return View(model);
        }

        model.ApplyTo(interest);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Interesse atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var interest = await context.Interests
            .AsNoTracking()
            .Include(item => item.Client)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return interest is null ? NotFound() : View(interest);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var interest = await context.Interests.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (interest is null)
        {
            return NotFound();
        }

        context.Interests.Remove(interest);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Interesse eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IReadOnlyList<SelectListItem>> BuildClientOptionsAsync(
        int? selectedId,
        CancellationToken cancellationToken) =>
        await context.Clients.AsNoTracking()
            .OrderBy(client => client.Name)
            .Select(client => new SelectListItem(
                client.Name,
                client.Id.ToString(CultureInfo.InvariantCulture),
                client.Id == selectedId))
            .ToListAsync(cancellationToken);

    private async Task ValidateClientAsync(int clientId, CancellationToken cancellationToken)
    {
        if (!await context.Clients.AnyAsync(client => client.Id == clientId, cancellationToken))
        {
            ModelState.AddModelError(nameof(InterestFormViewModel.ClientId), "O cliente selecionado não existe.");
        }
    }
}
