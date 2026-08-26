using Imobiliaria.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Imobiliaria.Web.Data;

public static class DatabaseInitializer
{
    public static async Task InitialiseAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        if (await context.Clients.AnyAsync())
        {
            return;
        }

        var ana = new Client
        {
            Name = "Ana Martins",
            Email = "ana.martins@example.test",
            Phone = "+351 910 000 101",
            Address = "Lisboa",
        };
        var miguel = new Client
        {
            Name = "Miguel Ferreira",
            Email = "miguel.ferreira@example.test",
            Phone = "+351 920 000 202",
            Address = "Oeiras",
        };
        var sofia = new Client
        {
            Name = "Sofia Almeida",
            Email = "sofia.almeida@example.test",
            Phone = "+351 930 000 303",
            Address = "Cascais",
        };

        var alfama = new PropertyListing
        {
            Title = "Apartamento luminoso em Alfama",
            Address = "Rua da Regueira, Lisboa",
            Zone = "Alfama",
            Type = PropertyType.Apartment,
            Year = 2018,
            Rooms = 2,
            AreaSquareMeters = 86,
            Price = 395_000,
            Description = "Apartamento renovado, com luz natural e varanda sobre o bairro histórico.",
            Owner = ana,
        };
        var oeiras = new PropertyListing
        {
            Title = "Moradia familiar perto do mar",
            Address = "Rua das Amendoeiras, Oeiras",
            Zone = "Santo Amaro",
            Type = PropertyType.House,
            Year = 2021,
            Rooms = 4,
            AreaSquareMeters = 218,
            Price = 875_000,
            Description = "Moradia com jardim, garagem e áreas generosas a poucos minutos da praia.",
            Owner = miguel,
            Status = PropertyStatus.Reserved,
        };
        var setubal = new PropertyListing
        {
            Title = "Terreno com vista para a serra",
            Address = "Estrada do Vale, Setúbal",
            Zone = "Azeitão",
            Type = PropertyType.Land,
            Year = 2024,
            Rooms = 0,
            AreaSquareMeters = 1_240,
            Price = 210_000,
            Description = "Terreno urbano com viabilidade de construção numa zona tranquila de Azeitão.",
            Owner = sofia,
        };

        context.AddRange(ana, miguel, sofia, alfama, oeiras, setubal);
        context.Interests.AddRange(
            new Interest { Client = ana, PreferredZone = "Cascais", MinimumRooms = 3, MaximumPrice = 650_000 },
            new Interest { Client = sofia, PreferredZone = "Lisboa", MinimumRooms = 2, MaximumPrice = 450_000 });
        context.Visits.AddRange(
            new Visit
            {
                Client = sofia,
                Property = alfama,
                ScheduledAt = DateTime.Today.AddDays(2).AddHours(10),
                Notes = "Confirmar acesso ao elevador.",
            },
            new Visit
            {
                Client = ana,
                Property = oeiras,
                ScheduledAt = DateTime.Today.AddDays(4).AddHours(15).AddMinutes(30),
            });

        await context.SaveChangesAsync();
    }
}
