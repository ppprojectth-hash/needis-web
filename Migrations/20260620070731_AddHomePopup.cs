using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddHomePopup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomePopups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DescriptionTH = table.Column<string>(type: "text", nullable: true),
                    DescriptionEN = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MobileImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LinkUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ButtonTextTH = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ButtonTextEN = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PopupType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ShowOnlyHomePage = table.Column<bool>(type: "boolean", nullable: false),
                    ShowOncePerSession = table.Column<bool>(type: "boolean", nullable: false),
                    ShowOncePerDay = table.Column<bool>(type: "boolean", nullable: false),
                    OpenLinkInNewTab = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayDelaySeconds = table.Column<int>(type: "integer", nullable: false),
                    StartDateUtc = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    EndDateUtc = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomePopups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomePopups_DisplayOrder",
                table: "HomePopups",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_HomePopups_EndDateUtc",
                table: "HomePopups",
                column: "EndDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_HomePopups_IsActive",
                table: "HomePopups",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_HomePopups_IsDelete",
                table: "HomePopups",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_HomePopups_PopupType",
                table: "HomePopups",
                column: "PopupType");

            migrationBuilder.CreateIndex(
                name: "IX_HomePopups_StartDateUtc",
                table: "HomePopups",
                column: "StartDateUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomePopups");
        }
    }
}
