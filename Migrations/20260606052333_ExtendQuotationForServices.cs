using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class ExtendQuotationForServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestType",
                table: "QuotationRequests",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ItemType",
                table: "QuotationRequestItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceCodeSnapshot",
                table: "QuotationRequestItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "QuotationRequestItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceNameSnapshotEN",
                table: "QuotationRequestItems",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceNameSnapshotTH",
                table: "QuotationRequestItems",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceSlugSnapshot",
                table: "QuotationRequestItems",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationRequests_RequestType",
                table: "QuotationRequests",
                column: "RequestType");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationRequestItems_ItemType",
                table: "QuotationRequestItems",
                column: "ItemType");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationRequestItems_ServiceId",
                table: "QuotationRequestItems",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationRequestItems_Services_ServiceId",
                table: "QuotationRequestItems",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationRequestItems_Services_ServiceId",
                table: "QuotationRequestItems");

            migrationBuilder.DropIndex(
                name: "IX_QuotationRequests_RequestType",
                table: "QuotationRequests");

            migrationBuilder.DropIndex(
                name: "IX_QuotationRequestItems_ItemType",
                table: "QuotationRequestItems");

            migrationBuilder.DropIndex(
                name: "IX_QuotationRequestItems_ServiceId",
                table: "QuotationRequestItems");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "QuotationRequests");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "QuotationRequestItems");

            migrationBuilder.DropColumn(
                name: "ServiceCodeSnapshot",
                table: "QuotationRequestItems");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "QuotationRequestItems");

            migrationBuilder.DropColumn(
                name: "ServiceNameSnapshotEN",
                table: "QuotationRequestItems");

            migrationBuilder.DropColumn(
                name: "ServiceNameSnapshotTH",
                table: "QuotationRequestItems");

            migrationBuilder.DropColumn(
                name: "ServiceSlugSnapshot",
                table: "QuotationRequestItems");
        }
    }
}
