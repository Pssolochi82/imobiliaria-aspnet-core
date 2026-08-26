using System.ComponentModel.DataAnnotations;
using Imobiliaria.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Imobiliaria.Web.ViewModels;

public sealed class PropertyFormViewModel
{
    [Required(ErrorMessage = "Indique o título."), StringLength(140)]
    [Display(Name = "Título")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Indique a morada."), StringLength(180)]
    [Display(Name = "Morada")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Indique a zona."), StringLength(80)]
    [Display(Name = "Zona")]
    public string Zone { get; set; } = string.Empty;

    [Display(Name = "Tipo de imóvel")]
    public PropertyType Type { get; set; }

    [Range(1800, 2100, ErrorMessage = "Introduza um ano entre 1800 e 2100.")]
    [Display(Name = "Ano")]
    public int Year { get; set; } = DateTime.Today.Year;

    [Range(0, 50, ErrorMessage = "Introduza um número entre 0 e 50.")]
    [Display(Name = "Quartos")]
    public int Rooms { get; set; }

    [Range(typeof(decimal), "1", "100000", ErrorMessage = "A área deve ser superior a 0.")]
    [Display(Name = "Área (m²)")]
    public decimal AreaSquareMeters { get; set; }

    [Range(typeof(decimal), "1", "999999999", ErrorMessage = "O preço deve ser superior a 0.")]
    [Display(Name = "Preço (€)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Adicione uma descrição."), StringLength(1200, MinimumLength = 20)]
    [Display(Name = "Descrição")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Estado")]
    public PropertyStatus Status { get; set; } = PropertyStatus.Available;

    [Display(Name = "Proprietário")]
    public int? OwnerId { get; set; }

    public IReadOnlyList<SelectListItem> Owners { get; set; } = [];

    public static PropertyFormViewModel FromEntity(PropertyListing property) => new()
    {
        Title = property.Title,
        Address = property.Address,
        Zone = property.Zone,
        Type = property.Type,
        Year = property.Year,
        Rooms = property.Rooms,
        AreaSquareMeters = property.AreaSquareMeters,
        Price = property.Price,
        Description = property.Description,
        Status = property.Status,
        OwnerId = property.OwnerId,
    };

    public void ApplyTo(PropertyListing property)
    {
        property.Title = Title.Trim();
        property.Address = Address.Trim();
        property.Zone = Zone.Trim();
        property.Type = Type;
        property.Year = Year;
        property.Rooms = Rooms;
        property.AreaSquareMeters = AreaSquareMeters;
        property.Price = Price;
        property.Description = Description.Trim();
        property.Status = Status;
        property.OwnerId = OwnerId;
    }
}

public sealed record PropertyListViewModel(
    string? Query,
    PropertyStatus? Status,
    PagedResult<PropertyListing> Properties);
