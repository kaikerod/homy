using GastosResidenciais.Application.DTOs.Totais;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Enums;

namespace GastosResidenciais.Application.Services;

public sealed class TotaisService(IPessoaRepository pessoaRepository) : ITotaisService
{
    public async Task<TotaisResponse> ObterAsync(CancellationToken cancellationToken = default)
    {
        var pessoas = await pessoaRepository.GetAllWithTransactionsAsync(cancellationToken);

        var totaisPorPessoa = pessoas.Select(pessoa =>
        {
            var receitas = pessoa.Transacoes
                .Where(transacao => transacao.Tipo == TipoTransacao.Receita)
                .Sum(transacao => transacao.Valor);
            var despesas = pessoa.Transacoes
                .Where(transacao => transacao.Tipo == TipoTransacao.Despesa)
                .Sum(transacao => transacao.Valor);

            return new TotalPorPessoaResponse(
                pessoa.Id,
                pessoa.Nome,
                receitas,
                despesas,
                receitas - despesas);
        }).ToArray();

        var totalReceitas = totaisPorPessoa.Sum(total => total.TotalReceitas);
        var totalDespesas = totaisPorPessoa.Sum(total => total.TotalDespesas);

        return new TotaisResponse(
            totaisPorPessoa,
            totalReceitas,
            totalDespesas,
            totalReceitas - totalDespesas);
    }
}
