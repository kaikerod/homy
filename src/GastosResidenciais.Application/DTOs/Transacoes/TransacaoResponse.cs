using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Domain.Enums;

namespace GastosResidenciais.Application.DTOs.Transacoes;

public sealed record TransacaoResponse(
    Guid Id,
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    Guid PessoaId,
    string PessoaNome)
{
    public static TransacaoResponse FromEntity(Transacao transacao, string pessoaNome) =>
        new(
            transacao.Id,
            transacao.Descricao,
            transacao.Valor,
            transacao.Tipo,
            transacao.PessoaId,
            pessoaNome);
}

