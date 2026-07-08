using Microsoft.EntityFrameworkCore;
using Needis.Web.Models;

namespace Needis.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AdminUser>           AdminUsers           => Set<AdminUser>();
    public DbSet<AdminRole>           AdminRoles           => Set<AdminRole>();
    public DbSet<AdminPermission>     AdminPermissions     => Set<AdminPermission>();
    public DbSet<AdminRolePermission> AdminRolePermissions => Set<AdminRolePermission>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<HomeBanner> HomeBanners => Set<HomeBanner>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductSpecification> ProductSpecifications => Set<ProductSpecification>();
    public DbSet<FooterContact>   FooterContacts  => Set<FooterContact>();
    public DbSet<ContactMessage>  ContactMessages => Set<ContactMessage>();
    public DbSet<UsageLog>           UsageLogs            => Set<UsageLog>();
    public DbSet<QuotationRequest>     QuotationRequests     => Set<QuotationRequest>();
    public DbSet<QuotationRequestItem> QuotationRequestItems => Set<QuotationRequestItem>();
    public DbSet<EmailSendLog>           EmailSendLogs          => Set<EmailSendLog>();
    public DbSet<AboutSection>           AboutSections          => Set<AboutSection>();
    public DbSet<AboutSectionItem>       AboutSectionItems      => Set<AboutSectionItem>();
    public DbSet<AboutCompanyHistory>    AboutCompanyHistories  => Set<AboutCompanyHistory>();
    public DbSet<AboutStatCard>          AboutStatCards         => Set<AboutStatCard>();
    public DbSet<BrandPartner>           BrandPartners          => Set<BrandPartner>();
    public DbSet<StaffProfile>           StaffProfiles          => Set<StaffProfile>();
    public DbSet<ProductSale>            ProductSales           => Set<ProductSale>();
    public DbSet<ServicePage>            ServicePages           => Set<ServicePage>();
    public DbSet<Models.Service>         Services               => Set<Models.Service>();
    public DbSet<ServiceDetailSection>   ServiceDetailSections  => Set<ServiceDetailSection>();
    public DbSet<ServiceScopeItem>       ServiceScopeItems      => Set<ServiceScopeItem>();
    public DbSet<ServiceContactCard>     ServiceContactCards    => Set<ServiceContactCard>();
    public DbSet<ActivityPage>           ActivityPages          => Set<ActivityPage>();
    public DbSet<Activity>               Activities             => Set<Activity>();
    public DbSet<ActivityTag>            ActivityTags           => Set<ActivityTag>();
    public DbSet<ActivityTagMap>         ActivityTagMaps        => Set<ActivityTagMap>();
    public DbSet<ActivityDetailBlock>    ActivityDetailBlocks   => Set<ActivityDetailBlock>();
    public DbSet<ActivityImage>          ActivityImages         => Set<ActivityImage>();
    public DbSet<ActivityRelatedItem>    ActivityRelatedItems   => Set<ActivityRelatedItem>();
    public DbSet<SeoSetting>             SeoSettings            => Set<SeoSetting>();
    public DbSet<SeoRedirect>            SeoRedirects           => Set<SeoRedirect>();
    public DbSet<QuotationCart>          QuotationCarts         => Set<QuotationCart>();
    public DbSet<QuotationCartItem>      QuotationCartItems     => Set<QuotationCartItem>();
    public DbSet<MediaFile>              MediaFiles             => Set<MediaFile>();
    public DbSet<HomePopup>              HomePopups             => Set<HomePopup>();
    public DbSet<SiteText>               SiteTexts              => Set<SiteText>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Store all DateTime as timestamptz (UTC) in PostgreSQL
        configurationBuilder.Properties<DateTime>().HaveColumnType("timestamptz");
        configurationBuilder.Properties<DateTime?>().HaveColumnType("timestamptz");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AdminUser>(e =>
        {
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
            e.HasOne(x => x.AdminRole)
             .WithMany(x => x.AdminUsers)
             .HasForeignKey(x => x.AdminRoleId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AdminRole>(e =>
        {
            e.HasIndex(x => x.RoleKey).IsUnique();
        });

        modelBuilder.Entity<AdminPermission>(e =>
        {
            e.HasIndex(x => x.PermissionKey).IsUnique();
        });

        modelBuilder.Entity<AdminRolePermission>(e =>
        {
            e.HasIndex(x => new { x.AdminRoleId, x.AdminPermissionId }).IsUnique();
            e.HasOne(x => x.AdminRole)
             .WithMany(x => x.RolePermissions)
             .HasForeignKey(x => x.AdminRoleId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.AdminPermission)
             .WithMany(x => x.RolePermissions)
             .HasForeignKey(x => x.AdminPermissionId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SiteSetting>(e =>
        {
            e.Property(x => x.GoogleMapUrl).HasMaxLength(1000);
            e.Property(x => x.GoogleMapEmbedUrl).HasMaxLength(2000);
            e.Property(x => x.MapTitleTH).HasMaxLength(200);
            e.Property(x => x.MapTitleEN).HasMaxLength(200);
            e.Property(x => x.MapDescriptionTH).HasMaxLength(500);
            e.Property(x => x.MapDescriptionEN).HasMaxLength(500);
        });

        modelBuilder.Entity<HomeBanner>(e =>
        {
            e.Property(x => x.MediaType).HasMaxLength(50);
            e.Property(x => x.VideoUrl).HasMaxLength(500);
            e.Property(x => x.VideoFileUrl).HasMaxLength(500);
            e.Property(x => x.MobileImageUrl).HasMaxLength(500);
            e.Property(x => x.MobileVideoUrl).HasMaxLength(500);
            e.Property(x => x.OverlayStyle).HasMaxLength(50);
            e.Property(x => x.TextPosition).HasMaxLength(50);
            e.HasIndex(x => x.MediaType);
            e.HasIndex(x => x.IsActive);
        });

        modelBuilder.Entity<ProductCategory>(e =>
        {
            e.HasIndex(x => x.Slug).IsUnique();
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.HasIndex(x => x.Slug).IsUnique();
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.Property(x => x.YoutubeVideoUrl).HasMaxLength(500);
            e.Property(x => x.YoutubeVideoTitleTH).HasMaxLength(200);
            e.Property(x => x.YoutubeVideoTitleEN).HasMaxLength(200);
            e.Property(x => x.YoutubeVideoDescriptionTH).HasMaxLength(1000);
            e.Property(x => x.YoutubeVideoDescriptionEN).HasMaxLength(1000);
            e.HasIndex(x => x.ShowYoutubeVideo);
            e.HasOne(x => x.Category)
             .WithMany(x => x.Products)
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductImage>(e =>
        {
            e.HasOne(x => x.Product)
             .WithMany(x => x.Images)
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductSpecification>(e =>
        {
            e.Property(x => x.SpecGroupTH).HasMaxLength(150);
            e.Property(x => x.SpecGroupEN).HasMaxLength(150);
            e.Property(x => x.SpecNameTH).HasMaxLength(200);
            e.Property(x => x.SpecNameEN).HasMaxLength(200);
            e.Property(x => x.SpecValueTH).HasMaxLength(500);
            e.Property(x => x.SpecValueEN).HasMaxLength(500);
            e.Property(x => x.UnitTH).HasMaxLength(100);
            e.Property(x => x.UnitEN).HasMaxLength(100);
            e.Property(x => x.RemarkTH).HasMaxLength(500);
            e.Property(x => x.RemarkEN).HasMaxLength(500);
            e.Property(x => x.CreatedBy).HasMaxLength(150);
            e.Property(x => x.UpdatedBy).HasMaxLength(150);
            e.HasIndex(x => x.ProductId);
            e.HasIndex(x => x.SpecGroupEN);
            e.HasIndex(x => x.DisplayOrder);
            e.HasIndex(x => x.IsActive);
            e.HasIndex(x => x.IsDelete);
            e.HasOne(x => x.Product)
             .WithMany(x => x.Specifications)
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuotationRequest>(e =>
        {
            e.HasIndex(x => x.RequestNo).IsUnique();
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.RequestType);
            e.HasIndex(x => x.CreatedAt);
            e.HasIndex(x => x.Email);
        });

        modelBuilder.Entity<QuotationRequestItem>(e =>
        {
            e.HasIndex(x => x.QuotationRequestId);
            e.HasIndex(x => x.ItemType);
            e.HasIndex(x => x.ServiceId);
            e.HasOne(x => x.QuotationRequest)
             .WithMany(x => x.Items)
             .HasForeignKey(x => x.QuotationRequestId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Product)
             .WithMany()
             .HasForeignKey(x => x.ProductId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.ServiceItem)
             .WithMany()
             .HasForeignKey(x => x.ServiceId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AboutSection>(e =>
        {
            e.HasIndex(x => x.SectionKey).IsUnique();
            e.HasIndex(x => x.IsDelete);
        });

        modelBuilder.Entity<AboutSectionItem>(e =>
        {
            e.HasIndex(x => x.AboutSectionId);
            e.HasOne(x => x.AboutSection)
             .WithMany(x => x.Items)
             .HasForeignKey(x => x.AboutSectionId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AboutCompanyHistory>(e =>
        {
            e.HasIndex(x => x.Year);
            e.HasIndex(x => x.IsDelete);
        });

        modelBuilder.Entity<AboutStatCard>(e =>
        {
            e.HasIndex(x => x.StatKey).IsUnique();
            e.HasIndex(x => x.IsDelete);
        });

        modelBuilder.Entity<BrandPartner>(e =>
        {
            e.HasIndex(x => x.BrandName);
            e.HasIndex(x => x.IsGlobalBrand);
            e.HasIndex(x => x.IsDelete);
        });

        modelBuilder.Entity<StaffProfile>(e =>
        {
            e.HasIndex(x => x.EmployeeCode);
            e.HasIndex(x => x.IsExpert);
            e.HasIndex(x => x.IsDelete);
            e.HasIndex(x => x.Slug).IsUnique().HasFilter("\"Slug\" IS NOT NULL");
            e.Property(x => x.Slug).HasMaxLength(250);
            e.Property(x => x.MobilePhone).HasMaxLength(100);
            e.Property(x => x.Email).HasMaxLength(250);
            e.Property(x => x.BiographyTH).HasMaxLength(4000);
            e.Property(x => x.BiographyEN).HasMaxLength(4000);
            e.Property(x => x.AchievementTH).HasMaxLength(4000);
            e.Property(x => x.AchievementEN).HasMaxLength(4000);
            e.Property(x => x.PdfFileUrl).HasMaxLength(500);
            e.Property(x => x.PdfFileName).HasMaxLength(250);
            e.HasIndex(x => x.DisplayOrder);
            e.HasIndex(x => x.ShowDetailPage);
        });

        modelBuilder.Entity<ProductSale>(e =>
        {
            e.HasIndex(x => x.ProductId);
            e.HasIndex(x => x.SaleDate);
            e.HasIndex(x => x.CountInAboutStats);
            e.HasIndex(x => x.IsDelete);
            e.HasOne(x => x.Product)
             .WithMany(x => x.ProductSales)
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ServicePage>(e =>
        {
            e.HasIndex(x => x.PageKey).IsUnique();
            e.HasIndex(x => x.IsDelete);
        });

        modelBuilder.Entity<Models.Service>(e =>
        {
            e.HasIndex(x => x.ServiceCode).IsUnique();
            e.HasIndex(x => x.ServiceSlug).IsUnique();
            e.HasIndex(x => x.IsDelete);
        });

        modelBuilder.Entity<ServiceDetailSection>(e =>
        {
            e.HasIndex(x => x.ServiceId);
            e.HasIndex(x => x.SectionKey);
            e.HasIndex(x => x.IsDelete);
            e.HasOne(x => x.Service)
             .WithMany(x => x.DetailSections)
             .HasForeignKey(x => x.ServiceId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ServiceScopeItem>(e =>
        {
            e.HasIndex(x => x.ServiceDetailSectionId);
            e.HasIndex(x => x.IsDelete);
            e.HasOne(x => x.ServiceDetailSection)
             .WithMany(x => x.ScopeItems)
             .HasForeignKey(x => x.ServiceDetailSectionId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ServiceContactCard>(e =>
        {
            e.HasIndex(x => x.ServiceId);
            e.HasIndex(x => x.IsDelete);
            e.HasOne(x => x.Service)
             .WithMany(x => x.ContactCards)
             .HasForeignKey(x => x.ServiceId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ActivityPage>(e =>
        {
            e.HasIndex(x => x.PageKey).IsUnique();
            e.HasIndex(x => x.IsDelete);
        });

        modelBuilder.Entity<Activity>(e =>
        {
            e.HasIndex(x => x.ActivitySlug).IsUnique();
            e.HasIndex(x => x.IsPublished);
            e.HasIndex(x => x.PublishedDate);
            e.HasIndex(x => x.IsFeatured);
            e.HasIndex(x => x.IsDelete);
        });

        modelBuilder.Entity<ActivityTag>(e =>
        {
            e.HasIndex(x => x.TagKey).IsUnique();
            e.HasIndex(x => x.IsDelete);
        });

        modelBuilder.Entity<ActivityTagMap>(e =>
        {
            e.HasIndex(x => new { x.ActivityId, x.ActivityTagId }).IsUnique();
            e.HasOne(x => x.Activity)
             .WithMany(x => x.ActivityTagMaps)
             .HasForeignKey(x => x.ActivityId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.ActivityTag)
             .WithMany(x => x.ActivityTagMaps)
             .HasForeignKey(x => x.ActivityTagId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ActivityDetailBlock>(e =>
        {
            e.HasIndex(x => x.ActivityId);
            e.HasIndex(x => x.IsDelete);
            e.HasOne(x => x.Activity)
             .WithMany(x => x.DetailBlocks)
             .HasForeignKey(x => x.ActivityId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ActivityImage>(e =>
        {
            e.HasIndex(x => x.ActivityId);
            e.HasIndex(x => x.ActivityDetailBlockId);
            e.HasIndex(x => x.IsDelete);
            e.HasOne(x => x.Activity)
             .WithMany(x => x.Images)
             .HasForeignKey(x => x.ActivityId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.ActivityDetailBlock)
             .WithMany(x => x.Images)
             .HasForeignKey(x => x.ActivityDetailBlockId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ActivityRelatedItem>(e =>
        {
            e.HasIndex(x => new { x.ActivityId, x.RelatedActivityId }).IsUnique();
            e.HasOne(x => x.Activity)
             .WithMany(x => x.RelatedItems)
             .HasForeignKey(x => x.ActivityId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.RelatedActivity)
             .WithMany()
             .HasForeignKey(x => x.RelatedActivityId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ContactMessage>(e =>
        {
            e.Property(x => x.Status).HasMaxLength(50);
            e.Property(x => x.AdminRemark).HasColumnType("text");
            e.Property(x => x.AssignedTo).HasMaxLength(150);
            e.Property(x => x.IpAddress).HasMaxLength(100);
            e.Property(x => x.UserAgent).HasMaxLength(500);
            e.Property(x => x.Language).HasMaxLength(10);
            e.Property(x => x.UpdatedBy).HasMaxLength(150);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.CreatedAt);
            e.HasIndex(x => x.Email);
            e.HasIndex(x => x.Subject);
            e.HasIndex(x => x.AssignedTo);
        });

        modelBuilder.Entity<SeoSetting>(e =>
        {
            e.Property(x => x.PageKey).HasMaxLength(150);
            e.Property(x => x.EntityType).HasMaxLength(100);
            e.Property(x => x.RoutePath).HasMaxLength(500);
            e.Property(x => x.CanonicalUrl).HasMaxLength(500);
            e.Property(x => x.OgImageUrl).HasMaxLength(500);
            e.Property(x => x.Robots).HasMaxLength(100);
            e.Property(x => x.ChangeFrequency).HasMaxLength(50);
            e.Property(x => x.Priority).HasPrecision(3, 1);
            e.HasIndex(x => x.PageKey);
            e.HasIndex(x => new { x.EntityType, x.EntityId });
            e.HasIndex(x => x.RoutePath);
            e.HasIndex(x => x.IncludeInSitemap);
            e.HasIndex(x => x.IsActive);
        });

        modelBuilder.Entity<SeoRedirect>(e =>
        {
            e.Property(x => x.SourcePath).HasMaxLength(500);
            e.Property(x => x.TargetPath).HasMaxLength(500);
            e.HasIndex(x => x.SourcePath).IsUnique();
            e.HasIndex(x => x.IsActive);
        });

        modelBuilder.Entity<QuotationCart>(e =>
        {
            e.Property(x => x.CartToken).HasMaxLength(100);
            e.Property(x => x.Language).HasMaxLength(10);
            e.Property(x => x.IpAddress).HasMaxLength(100);
            e.Property(x => x.UserAgent).HasMaxLength(500);
            e.HasIndex(x => x.CartToken).IsUnique();
            e.HasIndex(x => x.IsSubmitted);
            e.HasIndex(x => x.ExpiresAt);
        });

        modelBuilder.Entity<QuotationCartItem>(e =>
        {
            e.Property(x => x.ItemType).HasMaxLength(50);
            e.Property(x => x.ProductNameSnapshotTH).HasMaxLength(200);
            e.Property(x => x.ProductNameSnapshotEN).HasMaxLength(200);
            e.Property(x => x.ProductSlugSnapshot).HasMaxLength(150);
            e.Property(x => x.BrandSnapshot).HasMaxLength(100);
            e.Property(x => x.ModelSnapshot).HasMaxLength(100);
            e.Property(x => x.PartNumberSnapshot).HasMaxLength(100);
            e.Property(x => x.ServiceCodeSnapshot).HasMaxLength(100);
            e.Property(x => x.ServiceNameSnapshotTH).HasMaxLength(200);
            e.Property(x => x.ServiceNameSnapshotEN).HasMaxLength(200);
            e.Property(x => x.ServiceSlugSnapshot).HasMaxLength(150);
            e.HasIndex(x => x.QuotationCartId);
            e.HasIndex(x => x.ItemType);
            e.HasIndex(x => x.ProductId);
            e.HasIndex(x => x.ServiceId);
            e.HasOne(x => x.QuotationCart)
             .WithMany(x => x.Items)
             .HasForeignKey(x => x.QuotationCartId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Product)
             .WithMany()
             .HasForeignKey(x => x.ProductId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Service)
             .WithMany()
             .HasForeignKey(x => x.ServiceId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmailSendLog>(e =>
        {
            e.HasIndex(x => x.CreatedAt);
            e.HasIndex(x => x.EmailType);
            e.HasIndex(x => x.ReferenceType);
            e.HasIndex(x => x.ReferenceId);
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<MediaFile>(e =>
        {
            e.Property(x => x.FileName).HasMaxLength(255);
            e.Property(x => x.OriginalFileName).HasMaxLength(255);
            e.Property(x => x.FileUrl).HasMaxLength(500);
            e.Property(x => x.Folder).HasMaxLength(200);
            e.Property(x => x.FileType).HasMaxLength(50);
            e.Property(x => x.ContentType).HasMaxLength(150);
            e.Property(x => x.FileExtension).HasMaxLength(20);
            e.Property(x => x.UsageType).HasMaxLength(100);
            e.Property(x => x.RelatedModule).HasMaxLength(100);
            e.Property(x => x.CreatedBy).HasMaxLength(150);
            e.Property(x => x.UpdatedBy).HasMaxLength(150);
            e.Property(x => x.TitleTH).HasMaxLength(200);
            e.Property(x => x.TitleEN).HasMaxLength(200);
            e.Property(x => x.AltTextTH).HasMaxLength(300);
            e.Property(x => x.AltTextEN).HasMaxLength(300);
            e.Property(x => x.CaptionTH).HasMaxLength(500);
            e.Property(x => x.CaptionEN).HasMaxLength(500);
            e.HasIndex(x => x.FileType);
            e.HasIndex(x => x.UsageType);
            e.HasIndex(x => x.RelatedModule);
            e.HasIndex(x => x.RelatedEntityId);
            e.HasIndex(x => x.CreatedAt);
            e.HasIndex(x => x.IsActive);
            e.HasIndex(x => x.IsDelete);
            e.HasIndex(x => x.FileExtension);
        });

        modelBuilder.Entity<HomePopup>(e =>
        {
            e.Property(x => x.TitleTH).HasMaxLength(200);
            e.Property(x => x.TitleEN).HasMaxLength(200);
            e.Property(x => x.ImageUrl).HasMaxLength(500);
            e.Property(x => x.MobileImageUrl).HasMaxLength(500);
            e.Property(x => x.LinkUrl).HasMaxLength(500);
            e.Property(x => x.ButtonTextTH).HasMaxLength(100);
            e.Property(x => x.ButtonTextEN).HasMaxLength(100);
            e.Property(x => x.PopupType).HasMaxLength(100);
            e.Property(x => x.CreatedBy).HasMaxLength(150);
            e.Property(x => x.UpdatedBy).HasMaxLength(150);
            e.HasIndex(x => x.IsActive);
            e.HasIndex(x => x.IsDelete);
            e.HasIndex(x => x.StartDateUtc);
            e.HasIndex(x => x.EndDateUtc);
            e.HasIndex(x => x.DisplayOrder);
            e.HasIndex(x => x.PopupType);
        });

        modelBuilder.Entity<SiteText>(e =>
        {
            e.HasIndex(x => x.Key).IsUnique();
            e.HasIndex(x => x.Page);
            e.HasIndex(x => x.IsActive);
            e.HasIndex(x => x.IsDelete);
        });

        modelBuilder.Entity<UsageLog>(e =>
        {
            e.HasIndex(x => x.AccessedAt);
            e.HasIndex(x => x.PageName);
            e.HasIndex(x => x.FunctionName);
            e.HasIndex(x => x.StatusCode);
            e.HasIndex(x => x.Username);
            e.HasOne(x => x.AdminUser)
             .WithMany()
             .HasForeignKey(x => x.AdminUserId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
