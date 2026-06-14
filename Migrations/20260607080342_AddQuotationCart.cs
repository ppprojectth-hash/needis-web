using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddQuotationCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuotationCarts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CartToken = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsSubmitted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationCarts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuotationCartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuotationCartId = table.Column<int>(type: "integer", nullable: false),
                    ItemType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    ProductNameSnapshotTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProductNameSnapshotEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProductSlugSnapshot = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    BrandSnapshot = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ModelSnapshot = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PartNumberSnapshot = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ServiceId = table.Column<int>(type: "integer", nullable: true),
                    ServiceCodeSnapshot = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ServiceNameSnapshotTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ServiceNameSnapshotEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ServiceSlugSnapshot = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ItemNote = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationCartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationCartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationCartItems_QuotationCarts_QuotationCartId",
                        column: x => x.QuotationCartId,
                        principalTable: "QuotationCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationCartItems_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuotationCartItems_ItemType",
                table: "QuotationCartItems",
                column: "ItemType");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationCartItems_ProductId",
                table: "QuotationCartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationCartItems_QuotationCartId",
                table: "QuotationCartItems",
                column: "QuotationCartId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationCartItems_ServiceId",
                table: "QuotationCartItems",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationCarts_CartToken",
                table: "QuotationCarts",
                column: "CartToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationCarts_ExpiresAt",
                table: "QuotationCarts",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationCarts_IsSubmitted",
                table: "QuotationCarts",
                column: "IsSubmitted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuotationCartItems");

            migrationBuilder.DropTable(
                name: "QuotationCarts");
        }
    }
}
