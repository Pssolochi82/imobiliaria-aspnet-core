using System.ComponentModel.DataAnnotations;

namespace Imobiliaria.Web.Models;

public sealed class Visit
{
    public int Id { get; set; }

    public DateTime ScheduledAt { get; set; }

    public VisitStatus Status { get; set; } = VisitStatus.Scheduled;

    [StringLength(500)]
    public string? Notes { get; set; }

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    public int PropertyId { get; set; }

    public PropertyListing Property { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public enum VisitStatus
{
    [Display(Name = "Agendada")]
    Scheduled,

    [Display(Name = "Concluída")]
    Completed,

    [Display(Name = "Cancelada")]
    Cancelled,
}
