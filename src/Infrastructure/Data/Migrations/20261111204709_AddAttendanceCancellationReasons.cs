using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FonoChampionHub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceCancellationReasons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceCancellationReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceCancellationReasons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCancellationReasons_NormalizedName",
                table: "AttendanceCancellationReasons",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.InsertData(
                table: "AttendanceCancellationReasons",
                columns: new[] { "Id", "Name", "NormalizedName", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "Não comparecimento", "NÃO COMPARECIMENTO", null, true },
                    { 2, "Indisponibilidade do profissional", "INDISPONIBILIDADE DO PROFISSIONAL", null, true },
                    { 3, "Solicitação do paciente/responsável", "SOLICITAÇÃO DO PACIENTE/RESPONSÁVEL", null, true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceCancellationReasons");
        }
    }
}
