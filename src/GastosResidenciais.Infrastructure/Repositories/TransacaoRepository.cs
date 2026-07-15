using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Infrastructure.Repositories;

public sealed class TransacaoRepository(AppDbContext dbContext) : ITransacaoRepository
{
    public async Task AddAsync(Transacao transacao, CancellationToken cancellationToken = default)
    {
        dbContext.Transacoes.Add(transacao);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Transacao>> GetAllWithPeopleAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Transacoes
            .AsNoTracking()
            .Include(transacao => transacao.Pessoa)
            .OrderBy(transacao => transacao.CreatedAt)
            .ToArrayAsync(cancellationToken);
}

