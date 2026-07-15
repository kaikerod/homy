using GastosResidenciais.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GastosResidenciais.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260715000000_InitialCreate")]
public sealed class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Pessoas",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                Idade = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Pessoas", pessoa => pessoa.Id);
                table.CheckConstraint("CK_Pessoa_Idade", "Idade >= 0 AND Idade <= 130");
            });

        migrationBuilder.CreateTable(
            name: "Transacoes",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                Descricao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Valor = table.Column<decimal>(type: "NUMERIC(18,2)", nullable: false),
                Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                PessoaId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Transacoes", transacao => transacao.Id);
                table.CheckConstraint("CK_Transacao_Tipo", "Tipo IN (1, 2)");
                table.CheckConstraint("CK_Transacao_Valor", "Valor > 0");
                table.ForeignKey(
                    name: "FK_Transacoes_Pessoas_PessoaId",
                    column: transacao => transacao.PessoaId,
                    principalTable: "Pessoas",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Transacoes_PessoaId",
            table: "Transacoes",
            column: "PessoaId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Transacoes");
        migrationBuilder.DropTable(name: "Pessoas");
    }
}
