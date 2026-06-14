using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSeoManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeoRedirects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourcePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TargetPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeoRedirects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeoSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PageKey = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EntityId = table.Column<int>(type: "integer", nullable: true),
                    RoutePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MetaTitleTH = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    MetaTitleEN = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    MetaDescriptionTH = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MetaDescriptionEN = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MetaKeywordsTH = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MetaKeywordsEN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OgTitleTH = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    OgTitleEN = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    OgDescriptionTH = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OgDescriptionEN = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OgImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CanonicalUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Robots = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Priority = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: false),
                    ChangeFrequency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IncludeInSitemap = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeoSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeoRedirects_IsActive",
                table: "SeoRedirects",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SeoRedirects_SourcePath",
                table: "SeoRedirects",
                column: "SourcePath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeoSettings_EntityType_EntityId",
                table: "SeoSettings",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_SeoSettings_IncludeInSitemap",
                table: "SeoSettings",
                column: "IncludeInSitemap");

            migrationBuilder.CreateIndex(
                name: "IX_SeoSettings_IsActive",
                table: "SeoSettings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SeoSettings_PageKey",
                table: "SeoSettings",
                column: "PageKey");

            migrationBuilder.CreateIndex(
                name: "IX_SeoSettings_RoutePath",
                table: "SeoSettings",
                column: "RoutePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeoRedirects");

            migrationBuilder.DropTable(
                name: "SeoSettings");
        }
    }
}
