using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAboutUsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutCompanyHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    TitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DescriptionTH = table.Column<string>(type: "text", nullable: true),
                    DescriptionEN = table.Column<string>(type: "text", nullable: true),
                    Position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutCompanyHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AboutSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SectionKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SubtitleTH = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    SubtitleEN = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DescriptionTH = table.Column<string>(type: "text", nullable: true),
                    DescriptionEN = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LayoutType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutSections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AboutStatCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StatKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LabelTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LabelEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DescriptionTH = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DescriptionEN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SourceType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ManualValue = table.Column<int>(type: "integer", nullable: true),
                    Prefix = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Suffix = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IconUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CardStyle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutStatCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrandPartners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BrandName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BrandType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsGlobalBrand = table.Column<bool>(type: "boolean", nullable: false),
                    ShowOnPartnerSection = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandPartners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductSales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    SaleDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReferenceNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CountInAboutStats = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSales_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FullNameTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FullNameEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PositionTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PositionEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Department = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StaffType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsExpert = table.Column<bool>(type: "boolean", nullable: false),
                    ShowPublic = table.Column<bool>(type: "boolean", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AboutSectionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AboutSectionId = table.Column<int>(type: "integer", nullable: false),
                    TitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DescriptionTH = table.Column<string>(type: "text", nullable: true),
                    DescriptionEN = table.Column<string>(type: "text", nullable: true),
                    IconUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutSectionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AboutSectionItems_AboutSections_AboutSectionId",
                        column: x => x.AboutSectionId,
                        principalTable: "AboutSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AboutCompanyHistories_IsDelete",
                table: "AboutCompanyHistories",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_AboutCompanyHistories_Year",
                table: "AboutCompanyHistories",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_AboutSectionItems_AboutSectionId",
                table: "AboutSectionItems",
                column: "AboutSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_AboutSections_IsDelete",
                table: "AboutSections",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_AboutSections_SectionKey",
                table: "AboutSections",
                column: "SectionKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AboutStatCards_IsDelete",
                table: "AboutStatCards",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_AboutStatCards_StatKey",
                table: "AboutStatCards",
                column: "StatKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandPartners_BrandName",
                table: "BrandPartners",
                column: "BrandName");

            migrationBuilder.CreateIndex(
                name: "IX_BrandPartners_IsDelete",
                table: "BrandPartners",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_BrandPartners_IsGlobalBrand",
                table: "BrandPartners",
                column: "IsGlobalBrand");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSales_CountInAboutStats",
                table: "ProductSales",
                column: "CountInAboutStats");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSales_IsDelete",
                table: "ProductSales",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSales_ProductId",
                table: "ProductSales",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSales_SaleDate",
                table: "ProductSales",
                column: "SaleDate");

            migrationBuilder.CreateIndex(
                name: "IX_StaffProfiles_EmployeeCode",
                table: "StaffProfiles",
                column: "EmployeeCode");

            migrationBuilder.CreateIndex(
                name: "IX_StaffProfiles_IsDelete",
                table: "StaffProfiles",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_StaffProfiles_IsExpert",
                table: "StaffProfiles",
                column: "IsExpert");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutCompanyHistories");

            migrationBuilder.DropTable(
                name: "AboutSectionItems");

            migrationBuilder.DropTable(
                name: "AboutStatCards");

            migrationBuilder.DropTable(
                name: "BrandPartners");

            migrationBuilder.DropTable(
                name: "ProductSales");

            migrationBuilder.DropTable(
                name: "StaffProfiles");

            migrationBuilder.DropTable(
                name: "AboutSections");
        }
    }
}
