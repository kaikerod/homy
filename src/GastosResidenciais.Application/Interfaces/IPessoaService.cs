using GastosResidenciais.Application.DTOs.Pessoas;

namespace GastosResidenciais.Application.Interfaces;

public interface IPessoaService
{
    Task<PessoaResponse> CriarAsync(
        CreatePessoaRequest request,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PessoaResponse>> ListarAsync(
        CancellationToken cancellationToken = default);
    Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
}

