using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DesafioBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entregadores",
                columns: table => new
                {
                    Identificador = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    CNPJ = table.Column<string>(type: "text", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NumeroCNH = table.Column<string>(type: "text", nullable: false),
                    TipoCNH = table.Column<string>(type: "text", nullable: false),
                    ImagemCNHPath = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entregadores", x => x.Identificador);
                });

            migrationBuilder.CreateTable(
                name: "EventoMotos",
                columns: table => new
                {
                    EventoMotoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MotoId = table.Column<int>(type: "integer", nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Modelo = table.Column<string>(type: "text", nullable: false),
                    Placa = table.Column<string>(type: "text", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoMotos", x => x.EventoMotoId);
                });

            migrationBuilder.CreateTable(
                name: "Motos",
                columns: table => new
                {
                    Identificador = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Modelo = table.Column<string>(type: "text", nullable: false),
                    Placa = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motos", x => x.Identificador);
                });

            migrationBuilder.CreateTable(
                name: "Locacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MotoId = table.Column<int>(type: "integer", nullable: false),
                    EntregadorId = table.Column<int>(type: "integer", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFimPrevisto = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFimReal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    ValorDiaria = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    PlanoDias = table.Column<int>(type: "integer", nullable: false),
                    Multa = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locacoes_Entregadores_EntregadorId",
                        column: x => x.EntregadorId,
                        principalTable: "Entregadores",
                        principalColumn: "Identificador",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Locacoes_Motos_MotoId",
                        column: x => x.MotoId,
                        principalTable: "Motos",
                        principalColumn: "Identificador",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entregadores_CNPJ",
                table: "Entregadores",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entregadores_NumeroCNH",
                table: "Entregadores",
                column: "NumeroCNH",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_EntregadorId",
                table: "Locacoes",
                column: "EntregadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_MotoId",
                table: "Locacoes",
                column: "MotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Motos_Placa",
                table: "Motos",
                column: "Placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoMotos");

            migrationBuilder.DropTable(
                name: "Locacoes");

            migrationBuilder.DropTable(
                name: "Entregadores");

            migrationBuilder.DropTable(
                name: "Motos");
        }
    }
}
