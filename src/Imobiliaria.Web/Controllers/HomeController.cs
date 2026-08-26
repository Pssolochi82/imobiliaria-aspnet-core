using System.Diagnostics;
using Imobiliaria.Web.Models;
using Imobiliaria.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Imobiliaria.Web.Controllers;

public sealed class HomeController(IDashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await dashboardService.GetAsync(cancellationToken));
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
