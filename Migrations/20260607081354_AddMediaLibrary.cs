using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Folder = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FileType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    FileExtension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    TitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AltTextTH = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    AltTextEN = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CaptionTH = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CaptionEN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DescriptionTH = table.Column<string>(type: "text", nullable: true),
                    DescriptionEN = table.Column<string>(type: "text", nullable: true),
                    UsageType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RelatedModule = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RelatedEntityId = table.Column<int>(type: "integer", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_CreatedAt",
                table: "MediaFiles",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_FileExtension",
                table: "MediaFiles",
                column: "FileExtension");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_FileType",
                table: "MediaFiles",
                column: "FileType");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_IsActive",
                table: "MediaFiles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_IsDelete",
                table: "MediaFiles",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_RelatedEntityId",
                table: "MediaFiles",
                column: "RelatedEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_RelatedModule",
                table: "MediaFiles",
                column: "RelatedModule");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_UsageType",
                table: "MediaFiles",
                column: "UsageType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaFiles");
        }
    }
}
