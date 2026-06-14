using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeBannerVideoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAutoplay",
                table: "HomeBanners",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLoop",
                table: "HomeBanners",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMuted",
                table: "HomeBanners",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MediaType",
                table: "HomeBanners",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobileImageUrl",
                table: "HomeBanners",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileVideoUrl",
                table: "HomeBanners",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OverlayStyle",
                table: "HomeBanners",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ShowControls",
                table: "HomeBanners",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SlideDurationSeconds",
                table: "HomeBanners",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TextPosition",
                table: "HomeBanners",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VideoFileUrl",
                table: "HomeBanners",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "HomeBanners",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HomeBanners_IsActive",
                table: "HomeBanners",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_HomeBanners_MediaType",
                table: "HomeBanners",
                column: "MediaType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HomeBanners_IsActive",
                table: "HomeBanners");

            migrationBuilder.DropIndex(
                name: "IX_HomeBanners_MediaType",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "IsAutoplay",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "IsLoop",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "IsMuted",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "MobileImageUrl",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "MobileVideoUrl",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "OverlayStyle",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "ShowControls",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "SlideDurationSeconds",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "TextPosition",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "VideoFileUrl",
                table: "HomeBanners");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "HomeBanners");
        }
    }
}
