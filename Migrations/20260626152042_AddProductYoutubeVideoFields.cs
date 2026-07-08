using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddProductYoutubeVideoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowYoutubeVideo",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeVideoDescriptionEN",
                table: "Products",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeVideoDescriptionTH",
                table: "Products",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeVideoTitleEN",
                table: "Products",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeVideoTitleTH",
                table: "Products",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeVideoUrl",
                table: "Products",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ShowYoutubeVideo",
                table: "Products",
                column: "ShowYoutubeVideo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_ShowYoutubeVideo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ShowYoutubeVideo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "YoutubeVideoDescriptionEN",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "YoutubeVideoDescriptionTH",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "YoutubeVideoTitleEN",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "YoutubeVideoTitleTH",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "YoutubeVideoUrl",
                table: "Products");
        }
    }
}
