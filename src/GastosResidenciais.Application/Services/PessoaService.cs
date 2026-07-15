using FluentValidation;
using GastosResidenciais.Application.DTOs.Pessoas;
using GastosResidenciais.Application.Exceptions;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Entities;

namespace GastosResidenciais.Application.Services;

public sealed class PessoaService(
    IPessoaRepository repository,
    IValidator<CreatePessoaRequest> validator) : IPessoaService
{
    public async Task<PessoaResponse> CriarAsync(
        CreatePessoaRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAppAsync(request, cancellationToken);

        var pessoa = Pessoa.Criar(request.Nome!, request.Idade!.Value);
        await repository.AddAsync(pessoa, cancellationToken);

        return PessoaResponse.FromEntity(pessoa);
    }

    public async Task<IReadOnlyCollection<PessoaResponse>> ListarAsync(
        CancellationToken cancellationToken = default)
    {
        var pessoas = await repository.GetAllAsync(cancellationToken);
        return pessoas.Select(PessoaResponse.FromEntity).ToArray();
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pessoa = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Pessoa não encontrada.");

        await repository.RemoveAsync(pessoa, cancellationToken);
    }
}

