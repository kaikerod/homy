using FluentValidation;
using GastosResidenciais.Application.DTOs.Transacoes;
using GastosResidenciais.Application.Exceptions;
using GastosResidenciais.Application.Interfaces;
using GastosResidenciais.Domain.Entities;
using GastosResidenciais.Domain.Enums;
using GastosResidenciais.Domain.Exceptions;

namespace GastosResidenciais.Application.Services;

public sealed class TransacaoService(
    ITransacaoRepository transacaoRepository,
    IPessoaRepository pessoaRepository,
    IValidator<CreateTransacaoRequest> validator) : ITransacaoService
{
    public async Task<TransacaoResponse> CriarAsync(
        CreateTransacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAppAsync(request, cancellationToken);

        var pessoa = await pessoaRepository.GetByIdAsync(request.PessoaId!.Value, cancellationToken)
            ?? throw new NotFoundException("Pessoa não encontrada.");

        if (pessoa.Idade < 18 && request.Tipo == TipoTransacao.Receita)
        {
            throw new BusinessRuleException(
                "Pessoas menores de 18 anos só podem ter transações do tipo Despesa.");
        }

        var transacao = Transacao.Criar(
            request.Descricao!,
            request.Valor!.Value,
            request.Tipo!.Value,
            pessoa);

        await transacaoRepository.AddAsync(transacao, cancellationToken);

        return TransacaoResponse.FromEntity(transacao, pessoa.Nome);
    }

    public async Task<IReadOnlyCollection<TransacaoResponse>> ListarAsync(
        CancellationToken cancellationToken = default)
    {
        var transacoes = await transacaoRepository.GetAllWithPeopleAsync(cancellationToken);
        return transacoes
            .Select(transacao => TransacaoResponse.FromEntity(transacao, transacao.Pessoa.Nome))
            .ToArray();
    }
}

