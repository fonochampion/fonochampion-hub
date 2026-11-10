using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FonoChampionHub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCid10Catalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cid10CatalogVersions",
                columns: table => new
                {
                    Fingerprint = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ImportedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cid10CatalogVersions", x => x.Fingerprint);
                });

            migrationBuilder.CreateTable(
                name: "Cid10CatalogEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VersionFingerprint = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    ChapterNumber = table.Column<int>(type: "integer", nullable: true),
                    Code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    CodeSearchKey = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    CodeRangeStart = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    CodeRangeEnd = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SearchText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cid10CatalogEntries", x => x.Id);
                    table.UniqueConstraint("AK_Cid10CatalogEntries_VersionFingerprint_Id", x => new { x.VersionFingerprint, x.Id });
                    table.ForeignKey(
                        name: "FK_Cid10CatalogEntries_Cid10CatalogEntries_VersionFingerprint_~",
                        columns: x => new { x.VersionFingerprint, x.ParentId },
                        principalTable: "Cid10CatalogEntries",
                        principalColumns: new[] { "VersionFingerprint", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cid10CatalogEntries_Cid10CatalogVersions_VersionFingerprint",
                        column: x => x.VersionFingerprint,
                        principalTable: "Cid10CatalogVersions",
                        principalColumn: "Fingerprint",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cid10CatalogEntries_VersionFingerprint_CodeSearchKey",
                table: "Cid10CatalogEntries",
                columns: new[] { "VersionFingerprint", "CodeSearchKey" },
                unique: true,
                filter: "\"CodeSearchKey\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cid10CatalogEntries_VersionFingerprint_ParentId",
                table: "Cid10CatalogEntries",
                columns: new[] { "VersionFingerprint", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Cid10CatalogVersions_IsCurrent",
                table: "Cid10CatalogVersions",
                column: "IsCurrent",
                unique: true,
                filter: "\"IsCurrent\" = TRUE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cid10CatalogEntries");

            migrationBuilder.DropTable(
                name: "Cid10CatalogVersions");
        }
    }
}
