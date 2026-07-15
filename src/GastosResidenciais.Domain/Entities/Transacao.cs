using GastosResidenciais.Domain.Enums;
using GastosResidenciais.Domain.Exceptions;

namespace GastosResidenciais.Domain.Entities;

public sealed class Transacao
{
    private Transacao()
    {
    }

    public Guid Id { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public TipoTransacao Tipo { get; private set; }
    public Guid PessoaId { get; private set; }
    public Pessoa Pessoa { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    public static Transacao Criar(
        string descricao,
        decimal valor,
        TipoTransacao tipo,
        Pessoa pessoa)
    {
        ArgumentNullException.ThrowIfNull(pessoa);
        ArgumentException.ThrowIfNullOrWhiteSpace(descricao);

        if (valor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valor), "Valor deve ser maior que zero.");
        }

        if (!Enum.IsDefined(tipo))
        {
            throw new ArgumentOutOfRangeException(nameof(tipo), "Tipo de transação inválido.");
        }

        if (pessoa.Idade < 18 && tipo == TipoTransacao.Receita)
        {
            throw new BusinessRuleException(
                "Pessoas menores de 18 anos só podem ter transações do tipo Despesa.");
        }

        var transacao = new Transacao
        {
            Id = Guid.CreateVersion7(),
            Descricao = descricao.Trim(),
            Valor = valor,
            Tipo = tipo,
            PessoaId = pessoa.Id,
            Pessoa = pessoa,
            CreatedAt = DateTime.UtcNow
        };

        pessoa.AdicionarTransacao(transacao);
        return transacao;
    }
}
