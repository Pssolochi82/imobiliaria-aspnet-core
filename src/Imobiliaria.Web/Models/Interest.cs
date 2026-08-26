using System.ComponentModel.DataAnnotations;

namespace Imobiliaria.Web.Models;

public sealed class Interest
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string PreferredZone { get; set; } = string.Empty;

    public int MinimumRooms { get; set; }

    public decimal? MaximumPrice { get; set; }

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
