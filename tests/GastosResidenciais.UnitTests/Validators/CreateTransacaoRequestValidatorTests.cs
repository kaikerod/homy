using GastosResidenciais.Application.DTOs.Transacoes;
using GastosResidenciais.Application.Validators;
using GastosResidenciais.Domain.Enums;

namespace GastosResidenciais.UnitTests.Validators;

public sealed class CreateTransacaoRequestValidatorTests
{
    private readonly CreateTransacaoRequestValidator _validator = new();

    [Fact]
    public async Task Validate_CamposAusentes_RetornaErrosDeTodosOsCampos()
    {
        var result = await _validator.ValidateAsync(new CreateTransacaoRequest(null, null, null, null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Descricao");
        Assert.Contains(result.Errors, error => error.PropertyName == "Valor");
        Assert.Contains(result.Errors, error => error.PropertyName == "Tipo");
        Assert.Contains(result.Errors, error => error.PropertyName == "PessoaId");
    }

    [Fact]
    public async Task Validate_DadosValidos_NaoRetornaErros()
    {
        var request = new CreateTransacaoRequest(
            "Salário",
            3500m,
            TipoTransacao.Receita,
            Guid.CreateVersion7());

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }
}

