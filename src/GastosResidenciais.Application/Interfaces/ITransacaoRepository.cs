using GastosResidenciais.Domain.Entities;

namespace GastosResidenciais.Application.Interfaces;

public interface ITransacaoRepository
{
    Task AddAsync(Transacao transacao, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Transacao>> GetAllWithPeopleAsync(
        CancellationToken cancellationToken = default);
}

