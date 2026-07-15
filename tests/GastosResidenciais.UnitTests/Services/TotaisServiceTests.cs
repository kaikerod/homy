using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Application.Services;
using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Domain.Enums;

namespace GastosResidenciais.UnitTests.Services;

public sealed class TotaisServiceTests
{
    [Fact]
    public async Task ObterAsync_CalculaTotaisIndividuaisEGeraisIncluindoPessoaSemTransacao()
    {
        var maria = Pessoa.Criar("Maria", 34);
        var lucas = Pessoa.Criar("Lucas", 15);
        var ana = Pessoa.Criar("Ana", 22);

        Transacao.Criar("Salário", 3500m, TipoTransacao.Receita, maria);
        Transacao.Criar("Aluguel", 1200m, TipoTransacao.Despesa, maria);
        Transacao.Criar("Lanche", 100m, TipoTransacao.Despesa, lucas);

        var service = new TotaisService(new PessoaRepositoryStub([maria, lucas, ana]));

        var response = await service.ObterAsync();

        Assert.Equal(3500m, response.TotalGeralReceitas);
        Assert.Equal(1300m, response.TotalGeralDespesas);
        Assert.Equal(2200m, response.SaldoLiquidoGeral);

        var totalMaria = Assert.Single(response.Pessoas, total => total.PessoaId == maria.Id);
        Assert.Equal(3500m, totalMaria.TotalReceitas);
        Assert.Equal(1200m, totalMaria.TotalDespesas);
        Assert.Equal(2300m, totalMaria.Saldo);

        var totalAna = Assert.Single(response.Pessoas, total => total.PessoaId == ana.Id);
        Assert.Equal(0m, totalAna.TotalReceitas);
        Assert.Equal(0m, totalAna.TotalDespesas);
        Assert.Equal(0m, totalAna.Saldo);
    }

    private sealed class PessoaRepositoryStub(IReadOnlyCollection<Pessoa> pessoas)
        : IPessoaRepository
    {
        public Task<IReadOnlyCollection<Pessoa>> GetAllWithTransactionsAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(pessoas);

        public Task AddAsync(Pessoa pessoa, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyCollection<Pessoa>> GetAllAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Pessoa?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task RemoveAsync(Pessoa pessoa, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
