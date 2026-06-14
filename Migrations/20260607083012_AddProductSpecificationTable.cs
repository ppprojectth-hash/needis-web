using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSpecificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductSpecifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    SpecGroupTH = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    SpecGroupEN = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    SpecNameTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SpecNameEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SpecValueTH = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SpecValueEN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UnitTH = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UnitEN = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RemarkTH = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RemarkEN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsHighlight = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSpecifications_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_DisplayOrder",
                table: "ProductSpecifications",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_IsActive",
                table: "ProductSpecifications",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_IsDelete",
                table: "ProductSpecifications",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_ProductId",
                table: "ProductSpecifications",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_SpecGroupEN",
                table: "ProductSpecifications",
                column: "SpecGroupEN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSpecifications");
        }
    }
}
