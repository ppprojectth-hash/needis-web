using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddServicesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServicePages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PageKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SubtitleTH = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    SubtitleEN = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DescriptionTH = table.Column<string>(type: "text", nullable: true),
                    DescriptionEN = table.Column<string>(type: "text", nullable: true),
                    BackgroundImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_ServicePages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ServiceSlug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ServiceNameTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ServiceNameEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ShortDescriptionTH = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ShortDescriptionEN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FullDescriptionTH = table.Column<string>(type: "text", nullable: true),
                    FullDescriptionEN = table.Column<string>(type: "text", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BannerImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IconUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DetailTitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DetailTitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DetailSubtitleTH = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DetailSubtitleEN = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceContactCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    TitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DescriptionTH = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DescriptionEN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PrimaryButtonTextTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PrimaryButtonTextEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PrimaryButtonUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SecondaryButtonTextTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SecondaryButtonTextEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SecondaryButtonUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ContactLabelTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactLabelEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactValue = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactIconUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_ServiceContactCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceContactCards_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDetailSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    SectionKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SectionTitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SectionTitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SectionSubtitleTH = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    SectionSubtitleEN = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    SectionDescriptionTH = table.Column<string>(type: "text", nullable: true),
                    SectionDescriptionEN = table.Column<string>(type: "text", nullable: true),
                    SectionImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_ServiceDetailSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceDetailSections_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceScopeItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceDetailSectionId = table.Column<int>(type: "integer", nullable: false),
                    ItemTitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ItemTitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ItemDescriptionTH = table.Column<string>(type: "text", nullable: true),
                    ItemDescriptionEN = table.Column<string>(type: "text", nullable: true),
                    IconUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_ServiceScopeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceScopeItems_ServiceDetailSections_ServiceDetailSectio~",
                        column: x => x.ServiceDetailSectionId,
                        principalTable: "ServiceDetailSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceContactCards_IsDelete",
                table: "ServiceContactCards",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceContactCards_ServiceId",
                table: "ServiceContactCards",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetailSections_IsDelete",
                table: "ServiceDetailSections",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetailSections_SectionKey",
                table: "ServiceDetailSections",
                column: "SectionKey");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetailSections_ServiceId",
                table: "ServiceDetailSections",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePages_IsDelete",
                table: "ServicePages",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePages_PageKey",
                table: "ServicePages",
                column: "PageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_IsDelete",
                table: "Services",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceCode",
                table: "Services",
                column: "ServiceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceSlug",
                table: "Services",
                column: "ServiceSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScopeItems_IsDelete",
                table: "ServiceScopeItems",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScopeItems_ServiceDetailSectionId",
                table: "ServiceScopeItems",
                column: "ServiceDetailSectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceContactCards");

            migrationBuilder.DropTable(
                name: "ServicePages");

            migrationBuilder.DropTable(
                name: "ServiceScopeItems");

            migrationBuilder.DropTable(
                name: "ServiceDetailSections");

            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
