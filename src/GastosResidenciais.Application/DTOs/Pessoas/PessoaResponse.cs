using GastosResidenciais.Domain.Entities;

namespace GastosResidenciais.Application.DTOs.Pessoas;

public sealed record PessoaResponse(Guid Id, string Nome, int Idade)
{
    public static PessoaResponse FromEntity(Pessoa pessoa) =>
        new(pessoa.Id, pessoa.Nome, pessoa.Idade);
}

