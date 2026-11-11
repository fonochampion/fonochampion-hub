using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FonoChampionHub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalInactivation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Professionals",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Professionals");
        }
    }
}
