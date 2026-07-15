namespace GastosResidenciais.Application.DTOs.Totais;

public sealed record TotaisResponse(
    IReadOnlyCollection<TotalPorPessoaResponse> Pessoas,
    decimal TotalGeralReceitas,
    decimal TotalGeralDespesas,
    decimal SaldoLiquidoGeral);

