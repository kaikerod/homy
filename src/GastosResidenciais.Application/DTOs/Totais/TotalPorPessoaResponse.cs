namespace GastosResidenciais.Application.DTOs.Totais;

public sealed record TotalPorPessoaResponse(
    Guid PessoaId,
    string Nome,
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal Saldo);

