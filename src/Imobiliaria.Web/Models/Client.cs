using System.ComponentModel.DataAnnotations;

namespace Imobiliaria.Web.Models;

public sealed class Client
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(180)]
    public string? Address { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<PropertyListing> Properties { get; } = [];

    public ICollection<Interest> Interests { get; } = [];

    public ICollection<Visit> Visits { get; } = [];
}
