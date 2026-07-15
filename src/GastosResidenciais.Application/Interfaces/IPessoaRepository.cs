using GastosResidenciais.Domain.Entities;

namespace GastosResidenciais.Application.Interfaces;

public interface IPessoaRepository
{
    Task AddAsync(Pessoa pessoa, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Pessoa>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Pessoa?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Pessoa>> GetAllWithTransactionsAsync(
        CancellationToken cancellationToken = default);
    Task RemoveAsync(Pessoa pessoa, CancellationToken cancellationToken = default);
}

