using System.ComponentModel.DataAnnotations;
using Imobiliaria.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Imobiliaria.Web.ViewModels;

public sealed class InterestFormViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um cliente.")]
    [Display(Name = "Cliente")]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "Indique a zona preferida."), StringLength(80)]
    [Display(Name = "Zona preferida")]
    public string PreferredZone { get; set; } = string.Empty;

    [Range(0, 50, ErrorMessage = "Introduza um número entre 0 e 50.")]
    [Display(Name = "Mínimo de quartos")]
    public int MinimumRooms { get; set; }

    [Range(typeof(decimal), "1", "999999999", ErrorMessage = "O orçamento deve ser superior a 0.")]
    [Display(Name = "Orçamento máximo (€)")]
    public decimal? MaximumPrice { get; set; }

    public IReadOnlyList<SelectListItem> Clients { get; set; } = [];

    public static InterestFormViewModel FromEntity(Interest interest) => new()
    {
        ClientId = interest.ClientId,
        PreferredZone = interest.PreferredZone,
        MinimumRooms = interest.MinimumRooms,
        MaximumPrice = interest.MaximumPrice,
    };

    public void ApplyTo(Interest interest)
    {
        interest.ClientId = ClientId;
        interest.PreferredZone = PreferredZone.Trim();
        interest.MinimumRooms = MinimumRooms;
        interest.MaximumPrice = MaximumPrice;
    }
}
