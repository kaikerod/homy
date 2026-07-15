using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.Infrastructure.Repositories;

public sealed class PessoaRepository(AppDbContext dbContext) : IPessoaRepository
{
    public async Task AddAsync(Pessoa pessoa, CancellationToken cancellationToken = default)
    {
        dbContext.Pessoas.Add(pessoa);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Pessoa>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Pessoas
            .AsNoTracking()
            .OrderBy(pessoa => pessoa.CreatedAt)
            .ToArrayAsync(cancellationToken);

    public Task<Pessoa?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Pessoas.SingleOrDefaultAsync(pessoa => pessoa.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Pessoa>> GetAllWithTransactionsAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Pessoas
            .AsNoTracking()
            .Include(pessoa => pessoa.Transacoes)
            .OrderBy(pessoa => pessoa.CreatedAt)
            .ToArrayAsync(cancellationToken);

    public async Task RemoveAsync(Pessoa pessoa, CancellationToken cancellationToken = default)
    {
        dbContext.Pessoas.Remove(pessoa);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

