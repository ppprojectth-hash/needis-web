using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailSendLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailSendLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmailType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ToEmail = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ReferenceType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReferenceId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSendLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailSendLogs_CreatedAt",
                table: "EmailSendLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSendLogs_EmailType",
                table: "EmailSendLogs",
                column: "EmailType");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSendLogs_ReferenceId",
                table: "EmailSendLogs",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSendLogs_ReferenceType",
                table: "EmailSendLogs",
                column: "ReferenceType");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSendLogs_Status",
                table: "EmailSendLogs",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailSendLogs");
        }
    }
}
