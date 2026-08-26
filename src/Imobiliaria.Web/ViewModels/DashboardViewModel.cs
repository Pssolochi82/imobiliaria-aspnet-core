using Imobiliaria.Web.Models;

namespace Imobiliaria.Web.ViewModels;

public sealed record DashboardViewModel(
    int ClientCount,
    int AvailablePropertyCount,
    int UpcomingVisitCount,
    int InterestCount,
    IReadOnlyList<PropertyListing> RecentProperties,
    IReadOnlyList<Visit> UpcomingVisits);
