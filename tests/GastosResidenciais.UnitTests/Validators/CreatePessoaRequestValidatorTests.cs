using GastosResidenciais.Application.DTOs.Pessoas;
using GastosResidenciais.Application.Validators;

namespace GastosResidenciais.UnitTests.Validators;

public sealed class CreatePessoaRequestValidatorTests
{
    private readonly CreatePessoaRequestValidator _validator = new();

    [Theory]
    [InlineData(null, null)]
    [InlineData("", -1)]
    [InlineData("   ", 131)]
    public async Task Validate_DadosInvalidos_RetornaErros(string? nome, int? idade)
    {
        var result = await _validator.ValidateAsync(new CreatePessoaRequest(nome, idade));

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task Validate_DadosValidos_NaoRetornaErros()
    {
        var result = await _validator.ValidateAsync(new CreatePessoaRequest("Maria", 34));

        Assert.True(result.IsValid);
    }
}

