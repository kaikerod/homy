using GastosResidenciais.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GastosResidenciais.Infrastructure.Persistence.Configurations;

public sealed class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
{
    public void Configure(EntityTypeBuilder<Transacao> builder)
    {
        builder.ToTable("Transacoes", table =>
        {
            table.HasCheckConstraint("CK_Transacao_Valor", "Valor > 0");
            table.HasCheckConstraint("CK_Transacao_Tipo", "Tipo IN (1, 2)");
        });

        builder.HasKey(transacao => transacao.Id);

        builder.Property(transacao => transacao.Id)
            .HasConversion<string>()
            .HasMaxLength(36);
        builder.Property(transacao => transacao.Descricao)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(transacao => transacao.Valor)
            .IsRequired()
            .HasColumnType("NUMERIC(18,2)");
        builder.Property(transacao => transacao.Tipo)
            .IsRequired()
            .HasConversion<int>();
        builder.Property(transacao => transacao.PessoaId)
            .HasConversion<string>()
            .HasMaxLength(36);
        builder.Property(transacao => transacao.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(transacao => transacao.PessoaId);
    }
}

