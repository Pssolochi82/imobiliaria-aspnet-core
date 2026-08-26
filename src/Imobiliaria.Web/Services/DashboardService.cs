using Imobiliaria.Web.Data;
using Imobiliaria.Web.Models;
using Imobiliaria.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Imobiliaria.Web.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetAsync(CancellationToken cancellationToken = default);
}

public sealed class DashboardService(AppDbContext context) : IDashboardService
{
    public async Task<DashboardViewModel> GetAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var clientCount = await context.Clients.CountAsync(cancellationToken);
        var propertyCount = await context.Properties
            .CountAsync(property => property.Status == PropertyStatus.Available, cancellationToken);
        var visitCount = await context.Visits
            .CountAsync(
                visit => visit.ScheduledAt >= today && visit.Status == VisitStatus.Scheduled,
                cancellationToken);
        var interestCount = await context.Interests.CountAsync(cancellationToken);
        var recentProperties = await context.Properties
            .AsNoTracking()
            .OrderByDescending(property => property.CreatedAtUtc)
            .Take(3)
            .ToListAsync(cancellationToken);
        var upcomingVisits = await context.Visits
            .AsNoTracking()
            .Include(visit => visit.Client)
            .Include(visit => visit.Property)
            .Where(visit => visit.ScheduledAt >= today && visit.Status == VisitStatus.Scheduled)
            .OrderBy(visit => visit.ScheduledAt)
            .Take(4)
            .ToListAsync(cancellationToken);

        return new DashboardViewModel(
            clientCount,
            propertyCount,
            visitCount,
            interestCount,
            recentProperties,
            upcomingVisits);
    }
}
