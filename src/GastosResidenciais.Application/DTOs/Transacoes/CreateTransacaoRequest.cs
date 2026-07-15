using GastosResidenciais.Domain.Enums;

namespace GastosResidenciais.Application.DTOs.Transacoes;

public sealed record CreateTransacaoRequest(
    string? Descricao,
    decimal? Valor,
    TipoTransacao? Tipo,
    Guid? PessoaId);

