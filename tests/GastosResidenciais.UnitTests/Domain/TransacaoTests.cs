using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Domain.Enums;
using GastosResidenciais.Domain.Exceptions;

namespace GastosResidenciais.UnitTests.Domain;

public sealed class TransacaoTests
{
    [Fact]
    public void Criar_ReceitaParaMenor_LancaRegraDeNegocio()
    {
        var menor = Pessoa.Criar("Lucas", 17);

        var exception = Assert.Throws<BusinessRuleException>(() =>
            Transacao.Criar("Mesada", 100m, TipoTransacao.Receita, menor));

        Assert.Equal(
            "Pessoas menores de 18 anos só podem ter transações do tipo Despesa.",
            exception.Message);
    }

    [Fact]
    public void Criar_DespesaParaMenor_CriaTransacaoValida()
    {
        var menor = Pessoa.Criar("Lucas", 15);

        var transacao = Transacao.Criar("Lanche", 25.50m, TipoTransacao.Despesa, menor);

        Assert.NotEqual(Guid.Empty, transacao.Id);
        Assert.Equal(menor.Id, transacao.PessoaId);
        Assert.Equal("Lanche", transacao.Descricao);
        Assert.Equal(25.50m, transacao.Valor);
    }
}

