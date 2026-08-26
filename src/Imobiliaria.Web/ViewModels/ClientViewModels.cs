using System.ComponentModel.DataAnnotations;
using Imobiliaria.Web.Models;

namespace Imobiliaria.Web.ViewModels;

public sealed class ClientFormViewModel
{
    [Required(ErrorMessage = "Indique o nome."), StringLength(120)]
    [Display(Name = "Nome")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Indique o email."), EmailAddress(ErrorMessage = "Introduza um email válido."), StringLength(180)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Indique o telefone."), Phone(ErrorMessage = "Introduza um telefone válido."), StringLength(30)]
    [Display(Name = "Telefone")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(180)]
    [Display(Name = "Morada")]
    public string? Address { get; set; }

    public static ClientFormViewModel FromEntity(Client client) => new()
    {
        Name = client.Name,
        Email = client.Email,
        Phone = client.Phone,
        Address = client.Address,
    };

    public void ApplyTo(Client client)
    {
        client.Name = Name.Trim();
        client.Email = Email.Trim().ToLowerInvariant();
        client.Phone = Phone.Trim();
        client.Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim();
    }
}

public sealed record ClientListViewModel(string? Query, PagedResult<Client> Clients);
