using Imobiliaria.Web.Data;
using Imobiliaria.Web.Models;
using Imobiliaria.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace Imobiliaria.Tests;

public sealed class DashboardServiceTests
{
    [Fact]
    public async Task GetAsyncReturnsOnlyAvailablePropertiesAndUpcomingScheduledVisits()
    {
        await using var context = CreateContext();
        var client = new Client
        {
            Name = "Cliente Teste",
            Email = "cliente@example.test",
            Phone = "+351 910 000 000",
        };
        var available = CreateProperty("Disponível", PropertyStatus.Available);
        var sold = CreateProperty("Vendido", PropertyStatus.Sold);
        context.AddRange(client, available, sold);
        context.Interests.Add(new Interest
        {
            Client = client,
            PreferredZone = "Lisboa",
            MinimumRooms = 2,
        });
        context.Visits.AddRange(
            new Visit
            {
                Client = client,
                Property = available,
                ScheduledAt = DateTime.Today.AddDays(1),
                Status = VisitStatus.Scheduled,
            },
            new Visit
            {
                Client = client,
                Property = sold,
                ScheduledAt = DateTime.Today.AddDays(-1),
                Status = VisitStatus.Completed,
            });
        await context.SaveChangesAsync();

        var result = await new DashboardService(context).GetAsync();

        Assert.Equal(1, result.ClientCount);
        Assert.Equal(1, result.AvailablePropertyCount);
        Assert.Equal(1, result.UpcomingVisitCount);
        Assert.Equal(1, result.InterestCount);
        Assert.Single(result.UpcomingVisits);
        Assert.Equal("Disponível", result.UpcomingVisits[0].Property.Title);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static PropertyListing CreateProperty(string title, PropertyStatus status) => new()
    {
        Title = title,
        Address = "Rua de Teste",
        Zone = "Lisboa",
        Type = PropertyType.Apartment,
        Year = 2020,
        Rooms = 2,
        AreaSquareMeters = 80,
        Price = 300_000,
        Description = "Descrição suficientemente longa para o teste.",
        Status = status,
    };
}
