using System.ComponentModel.DataAnnotations;
using Imobiliaria.Web.Models;
using Imobiliaria.Web.ViewModels;

namespace Imobiliaria.Tests;

public sealed class ViewModelTests
{
    [Fact]
    public void ClientFormApplyToNormalisesUserInput()
    {
        var model = new ClientFormViewModel
        {
            Name = "  Ana Martins  ",
            Email = "  ANA@EXAMPLE.TEST  ",
            Phone = "  +351 910 000 000  ",
            Address = "  Lisboa  ",
        };
        var client = new Client();

        model.ApplyTo(client);

        Assert.Equal("Ana Martins", client.Name);
        Assert.Equal("ana@example.test", client.Email);
        Assert.Equal("+351 910 000 000", client.Phone);
        Assert.Equal("Lisboa", client.Address);
    }

    [Fact]
    public void PropertyFormRejectsInvalidCommercialValues()
    {
        var model = new PropertyFormViewModel
        {
            Title = "Imóvel de teste",
            Address = "Rua de Teste",
            Zone = "Lisboa",
            Year = 1200,
            Rooms = -1,
            AreaSquareMeters = 0,
            Price = 0,
            Description = "Curta",
        };
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, new ValidationContext(model), results, true);

        Assert.False(isValid);
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(model.Year)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(model.Price)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(model.Description)));
    }

    [Fact]
    public void PagedResultComputesNavigationState()
    {
        var result = new PagedResult<int>([11, 12], 2, 10, 22);

        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }
}
