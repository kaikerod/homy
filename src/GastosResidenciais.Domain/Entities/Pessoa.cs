namespace GastosResidenciais.Domain.Entities;

public sealed class Pessoa
{
    private Pessoa()
    {
    }

    private Pessoa(string nome, int idade)
    {
        Id = Guid.CreateVersion7();
        Nome = nome.Trim();
        Idade = idade;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public int Idade { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<Transacao> Transacoes => _transacoes;

    private readonly List<Transacao> _transacoes = [];

    public static Pessoa Criar(string nome, int idade)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);

        if (idade is < 0 or > 130)
        {
            throw new ArgumentOutOfRangeException(nameof(idade), "Idade deve estar entre 0 e 130.");
        }

        return new Pessoa(nome, idade);
    }

    internal void AdicionarTransacao(Transacao transacao)
    {
        ArgumentNullException.ThrowIfNull(transacao);
        _transacoes.Add(transacao);
    }
}
