using GastosResidenciais.Application.DTOs.Totais;

namespace GastosResidenciais.Application.Interfaces;

public interface ITotaisService
{
    Task<TotaisResponse> ObterAsync(CancellationToken cancellationToken = default);
}

