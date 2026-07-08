using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffProfileDetailFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AchievementEN",
                table: "StaffProfiles",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AchievementTH",
                table: "StaffProfiles",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BiographyEN",
                table: "StaffProfiles",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BiographyTH",
                table: "StaffProfiles",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "StaffProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "StaffProfiles",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobilePhone",
                table: "StaffProfiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfFileName",
                table: "StaffProfiles",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfFileUrl",
                table: "StaffProfiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowContactInfo",
                table: "StaffProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowDetailPage",
                table: "StaffProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "StaffProfiles",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaffProfiles_DisplayOrder",
                table: "StaffProfiles",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_StaffProfiles_ShowDetailPage",
                table: "StaffProfiles",
                column: "ShowDetailPage");

            migrationBuilder.CreateIndex(
                name: "IX_StaffProfiles_Slug",
                table: "StaffProfiles",
                column: "Slug",
                unique: true,
                filter: "\"Slug\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StaffProfiles_DisplayOrder",
                table: "StaffProfiles");

            migrationBuilder.DropIndex(
                name: "IX_StaffProfiles_ShowDetailPage",
                table: "StaffProfiles");

            migrationBuilder.DropIndex(
                name: "IX_StaffProfiles_Slug",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "AchievementEN",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "AchievementTH",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "BiographyEN",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "BiographyTH",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "MobilePhone",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "PdfFileName",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "PdfFileUrl",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "ShowContactInfo",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "ShowDetailPage",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "StaffProfiles");
        }
    }
}
