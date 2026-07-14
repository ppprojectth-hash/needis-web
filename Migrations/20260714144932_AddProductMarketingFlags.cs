using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddProductMarketingFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPromotion",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PromotionLabelEN",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromotionLabelTH",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPromotion",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PromotionLabelEN",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PromotionLabelTH",
                table: "Products");
        }
    }
}
