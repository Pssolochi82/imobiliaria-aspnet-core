using System.ComponentModel.DataAnnotations;
using Imobiliaria.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Imobiliaria.Web.ViewModels;

public sealed class VisitFormViewModel : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um cliente.")]
    [Display(Name = "Cliente")]
    public int ClientId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um imóvel.")]
    [Display(Name = "Imóvel")]
    public int PropertyId { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Data e hora")]
    public DateTime ScheduledAt { get; set; } = DateTime.Today.AddDays(1).AddHours(10);

    [Display(Name = "Estado")]
    public VisitStatus Status { get; set; } = VisitStatus.Scheduled;

    [StringLength(500)]
    [Display(Name = "Notas")]
    public string? Notes { get; set; }

    public IReadOnlyList<SelectListItem> Clients { get; set; } = [];

    public IReadOnlyList<SelectListItem> Properties { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ScheduledAt < DateTime.Today.AddYears(-1))
        {
            yield return new ValidationResult(
                "A data da visita não é válida.",
                [nameof(ScheduledAt)]);
        }
    }

    public static VisitFormViewModel FromEntity(Visit visit) => new()
    {
        ClientId = visit.ClientId,
        PropertyId = visit.PropertyId,
        ScheduledAt = visit.ScheduledAt,
        Status = visit.Status,
        Notes = visit.Notes,
    };

    public void ApplyTo(Visit visit)
    {
        visit.ClientId = ClientId;
        visit.PropertyId = PropertyId;
        visit.ScheduledAt = ScheduledAt;
        visit.Status = Status;
        visit.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();
    }
}

public sealed record VisitListViewModel(
    string? Query,
    bool UpcomingOnly,
    PagedResult<Visit> Visits);
