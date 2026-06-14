using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddContactMessageManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminRemark",
                table: "ContactMessages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "ContactMessages",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "ContactMessages",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "ContactMessages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "ContactMessages",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "ContactMessages",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RepliedAt",
                table: "ContactMessages",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ContactMessages",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ContactMessages",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ContactMessages",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "ContactMessages",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_AssignedTo",
                table: "ContactMessages",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_CreatedAt",
                table: "ContactMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_Email",
                table: "ContactMessages",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_Status",
                table: "ContactMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_Subject",
                table: "ContactMessages",
                column: "Subject");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_AssignedTo",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_CreatedAt",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_Email",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_Status",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_Subject",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "AdminRemark",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "RepliedAt",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "ContactMessages");
        }
    }
}
