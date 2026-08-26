using System.ComponentModel.DataAnnotations;

namespace Imobiliaria.Web.Models;

public sealed class PropertyListing
{
    public int Id { get; set; }

    [Required, StringLength(140)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(180)]
    public string Address { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Zone { get; set; } = string.Empty;

    public PropertyType Type { get; set; }

    public int Year { get; set; }

    public int Rooms { get; set; }

    public decimal AreaSquareMeters { get; set; }

    public decimal Price { get; set; }

    [Required, StringLength(1200)]
    public string Description { get; set; } = string.Empty;

    public PropertyStatus Status { get; set; } = PropertyStatus.Available;

    public int? OwnerId { get; set; }

    public Client? Owner { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Visit> Visits { get; } = [];
}

public enum PropertyType
{
    [Display(Name = "Apartamento")]
    Apartment,

    [Display(Name = "Moradia")]
    House,

    [Display(Name = "Terreno")]
    Land,

    [Display(Name = "Espaço comercial")]
    Commercial,
}

public enum PropertyStatus
{
    [Display(Name = "Disponível")]
    Available,

    [Display(Name = "Reservado")]
    Reserved,

    [Display(Name = "Vendido")]
    Sold,

    [Display(Name = "Arrendado")]
    Rented,
}
