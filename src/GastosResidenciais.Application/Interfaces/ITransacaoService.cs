using GastosResidenciais.Application.DTOs.Transacoes;

namespace GastosResidenciais.Application.Interfaces;

public interface ITransacaoService
{
    Task<TransacaoResponse> CriarAsync(
        CreateTransacaoRequest request,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TransacaoResponse>> ListarAsync(
        CancellationToken cancellationToken = default);
}

