using GastosResidenciais.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GastosResidenciais.Infrastructure.Persistence.Configurations;

public sealed class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
{
    public void Configure(EntityTypeBuilder<Pessoa> builder)
    {
        builder.ToTable("Pessoas", table =>
            table.HasCheckConstraint("CK_Pessoa_Idade", "Idade >= 0 AND Idade <= 130"));

        builder.HasKey(pessoa => pessoa.Id);

        builder.Property(pessoa => pessoa.Id)
            .HasConversion<string>()
            .HasMaxLength(36);
        builder.Property(pessoa => pessoa.Nome)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(pessoa => pessoa.Idade)
            .IsRequired();
        builder.Property(pessoa => pessoa.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasMany(pessoa => pessoa.Transacoes)
            .WithOne(transacao => transacao.Pessoa)
            .HasForeignKey(transacao => transacao.PessoaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(pessoa => pessoa.Transacoes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

