using Imobiliaria.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Imobiliaria.Web.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients => Set<Client>();

    public DbSet<PropertyListing> Properties => Set<PropertyListing>();

    public DbSet<Interest> Interests => Set<Interest>();

    public DbSet<Visit> Visits => Set<Visit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasIndex(client => client.Email).IsUnique();
            entity.Property(client => client.Name).UseCollation("NOCASE");
        });

        modelBuilder.Entity<PropertyListing>(entity =>
        {
            entity.Property(property => property.Price).HasPrecision(12, 2);
            entity.Property(property => property.AreaSquareMeters).HasPrecision(8, 2);
            entity.Property(property => property.Type).HasConversion<string>();
            entity.Property(property => property.Status).HasConversion<string>();
            entity.HasIndex(property => new { property.Status, property.Zone });
            entity.HasOne(property => property.Owner)
                .WithMany(client => client.Properties)
                .HasForeignKey(property => property.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Interest>(entity =>
        {
            entity.Property(interest => interest.MaximumPrice).HasPrecision(12, 2);
            entity.HasOne(interest => interest.Client)
                .WithMany(client => client.Interests)
                .HasForeignKey(interest => interest.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Visit>(entity =>
        {
            entity.Property(visit => visit.Status).HasConversion<string>();
            entity.HasIndex(visit => visit.ScheduledAt);
            entity.HasOne(visit => visit.Client)
                .WithMany(client => client.Visits)
                .HasForeignKey(visit => visit.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(visit => visit.Property)
                .WithMany(property => property.Visits)
                .HasForeignKey(visit => visit.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
