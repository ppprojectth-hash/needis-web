using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Needis.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivitySlug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ActivityTitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ActivityTitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ShortDescriptionTH = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ShortDescriptionEN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SummaryTH = table.Column<string>(type: "text", nullable: true),
                    SummaryEN = table.Column<string>(type: "text", nullable: true),
                    ContentPreviewTH = table.Column<string>(type: "text", nullable: true),
                    ContentPreviewEN = table.Column<string>(type: "text", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BannerImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ActivityDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    PublishedDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    LocationTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LocationEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AuthorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    MetaTitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MetaTitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MetaDescriptionTH = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MetaDescriptionEN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityPages",
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
                    BreadcrumbTextTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BreadcrumbTextEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_ActivityPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TagKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TagNameTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TagNameEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TagDescriptionTH = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TagDescriptionEN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TagColor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsFilterable = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityDetailBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivityId = table.Column<int>(type: "integer", nullable: false),
                    BlockType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BlockTitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BlockTitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BlockSubtitleTH = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    BlockSubtitleEN = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    BlockContentTH = table.Column<string>(type: "text", nullable: true),
                    BlockContentEN = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    VideoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ButtonTextTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ButtonTextEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ButtonUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_ActivityDetailBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityDetailBlocks_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityRelatedItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivityId = table.Column<int>(type: "integer", nullable: false),
                    RelatedActivityId = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityRelatedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityRelatedItems_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityRelatedItems_Activities_RelatedActivityId",
                        column: x => x.RelatedActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActivityTagMaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivityId = table.Column<int>(type: "integer", nullable: false),
                    ActivityTagId = table.Column<int>(type: "integer", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTagMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityTagMaps_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityTagMaps_ActivityTags_ActivityTagId",
                        column: x => x.ActivityTagId,
                        principalTable: "ActivityTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivityId = table.Column<int>(type: "integer", nullable: false),
                    ActivityDetailBlockId = table.Column<int>(type: "integer", nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageTitleTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ImageTitleEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CaptionTH = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CaptionEN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AltTextTH = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AltTextEN = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ImageType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsCover = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityImages_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityImages_ActivityDetailBlocks_ActivityDetailBlockId",
                        column: x => x.ActivityDetailBlockId,
                        principalTable: "ActivityDetailBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ActivitySlug",
                table: "Activities",
                column: "ActivitySlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_IsDelete",
                table: "Activities",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_IsFeatured",
                table: "Activities",
                column: "IsFeatured");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_IsPublished",
                table: "Activities",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_PublishedDate",
                table: "Activities",
                column: "PublishedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDetailBlocks_ActivityId",
                table: "ActivityDetailBlocks",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDetailBlocks_IsDelete",
                table: "ActivityDetailBlocks",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityImages_ActivityDetailBlockId",
                table: "ActivityImages",
                column: "ActivityDetailBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityImages_ActivityId",
                table: "ActivityImages",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityImages_IsDelete",
                table: "ActivityImages",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPages_IsDelete",
                table: "ActivityPages",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPages_PageKey",
                table: "ActivityPages",
                column: "PageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRelatedItems_ActivityId_RelatedActivityId",
                table: "ActivityRelatedItems",
                columns: new[] { "ActivityId", "RelatedActivityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRelatedItems_RelatedActivityId",
                table: "ActivityRelatedItems",
                column: "RelatedActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTagMaps_ActivityId_ActivityTagId",
                table: "ActivityTagMaps",
                columns: new[] { "ActivityId", "ActivityTagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTagMaps_ActivityTagId",
                table: "ActivityTagMaps",
                column: "ActivityTagId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTags_IsDelete",
                table: "ActivityTags",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTags_TagKey",
                table: "ActivityTags",
                column: "TagKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityImages");

            migrationBuilder.DropTable(
                name: "ActivityPages");

            migrationBuilder.DropTable(
                name: "ActivityRelatedItems");

            migrationBuilder.DropTable(
                name: "ActivityTagMaps");

            migrationBuilder.DropTable(
                name: "ActivityDetailBlocks");

            migrationBuilder.DropTable(
                name: "ActivityTags");

            migrationBuilder.DropTable(
                name: "Activities");
        }
    }
}
