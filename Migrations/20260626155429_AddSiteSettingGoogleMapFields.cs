using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteSettingGoogleMapFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleMapEmbedUrl",
                table: "SiteSettings",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleMapUrl",
                table: "SiteSettings",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapDescriptionEN",
                table: "SiteSettings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapDescriptionTH",
                table: "SiteSettings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapTitleEN",
                table: "SiteSettings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapTitleTH",
                table: "SiteSettings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowMapOnAboutPage",
                table: "SiteSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleMapEmbedUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "GoogleMapUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "MapDescriptionEN",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "MapDescriptionTH",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "MapTitleEN",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "MapTitleTH",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "ShowMapOnAboutPage",
                table: "SiteSettings");
        }
    }
}
