using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddUsageLogExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "UsageLogs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "UsageLogs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Controller",
                table: "UsageLogs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DurationMs",
                table: "UsageLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "UsageLogs",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "UsageLogs",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QueryString",
                table: "UsageLogs",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                table: "UsageLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UsageLogs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "UsageLogs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsageLogs_FunctionName",
                table: "UsageLogs",
                column: "FunctionName");

            migrationBuilder.CreateIndex(
                name: "IX_UsageLogs_PageName",
                table: "UsageLogs",
                column: "PageName");

            migrationBuilder.CreateIndex(
                name: "IX_UsageLogs_StatusCode",
                table: "UsageLogs",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_UsageLogs_Username",
                table: "UsageLogs",
                column: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsageLogs_FunctionName",
                table: "UsageLogs");

            migrationBuilder.DropIndex(
                name: "IX_UsageLogs_PageName",
                table: "UsageLogs");

            migrationBuilder.DropIndex(
                name: "IX_UsageLogs_StatusCode",
                table: "UsageLogs");

            migrationBuilder.DropIndex(
                name: "IX_UsageLogs_Username",
                table: "UsageLogs");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "UsageLogs");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "UsageLogs");

            migrationBuilder.DropColumn(
                name: "Controller",
                table: "UsageLogs");

            migrationBuilder.DropColumn(
                name: "DurationMs",
                table: "UsageLogs");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "UsageLogs");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "UsageLogs");

            migrationBuilder.DropColumn(
                name: "QueryString",
                table: "UsageLogs");

            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "UsageLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UsageLogs");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "UsageLogs");
        }
    }
}
