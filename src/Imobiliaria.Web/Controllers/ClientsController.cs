using Imobiliaria.Web.Data;
using Imobiliaria.Web.Models;
using Imobiliaria.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Imobiliaria.Web.Controllers;

public sealed class ClientsController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index(
        string? query,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var clients = context.Clients.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            clients = clients.Where(client =>
                client.Name.Contains(term) ||
                client.Email.Contains(term) ||
                client.Phone.Contains(term));
        }

        var result = await clients
            .OrderBy(client => client.Name)
            .ToPagedResultAsync(page, 10, cancellationToken);

        return View(new ClientListViewModel(query, result));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var client = await context.Clients
            .AsNoTracking()
            .Include(item => item.Properties)
            .Include(item => item.Interests)
            .Include(item => item.Visits)
            .ThenInclude(visit => visit.Property)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return client is null ? NotFound() : View(client);
    }

    public IActionResult Create() => View(new ClientFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ClientFormViewModel model,
        CancellationToken cancellationToken)
    {
        await ValidateUniqueEmailAsync(model.Email, null, cancellationToken);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = new Client();
        model.ApplyTo(client);
        context.Clients.Add(client);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Cliente criado com sucesso.";
        return RedirectToAction(nameof(Details), new { id = client.Id });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var client = await context.Clients.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return client is null ? NotFound() : View(ClientFormViewModel.FromEntity(client));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ClientFormViewModel model,
        CancellationToken cancellationToken)
    {
        var client = await context.Clients.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (client is null)
        {
            return NotFound();
        }

        await ValidateUniqueEmailAsync(model.Email, id, cancellationToken);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.ApplyTo(client);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Cliente atualizado com sucesso.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var client = await context.Clients.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return client is null ? NotFound() : View(client);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var client = await context.Clients.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (client is null)
        {
            return NotFound();
        }

        if (await context.Visits.AnyAsync(visit => visit.ClientId == id, cancellationToken))
        {
            TempData["Error"] = "Não é possível eliminar um cliente com visitas associadas.";
            return RedirectToAction(nameof(Details), new { id });
        }

        context.Clients.Remove(client);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Cliente eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateUniqueEmailAsync(
        string email,
        int? currentId,
        CancellationToken cancellationToken)
    {
        var normalisedEmail = email.Trim().ToLowerInvariant();
        var exists = await context.Clients.AnyAsync(
            client => client.Email == normalisedEmail && client.Id != currentId,
            cancellationToken);
        if (exists)
        {
            ModelState.AddModelError(nameof(ClientFormViewModel.Email), "Já existe um cliente com este email.");
        }
    }
}
