using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Authorization;
using Needis.Web.Data;
using Needis.Web.Middleware;
using Needis.Web.Models;
using Needis.Web.Options;
using Needis.Web.Services;
using Needis.Web.Services.Email;
using Needis.Web.Services.Permissions;
using Needis.Web.Services.Media;
using Needis.Web.Services.Quotation;
using Needis.Web.Services.Manual;
using Needis.Web.Services.Seo;
using Needis.Web.Services.Features;
using Needis.Web.Services.HomePopup;
using Needis.Web.Services.Content;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPasswordHasher<AdminUser>, PasswordHasher<AdminUser>>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();

// Permission system
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// SEO service
builder.Services.AddScoped<ISeoService, SeoService>();

// Quotation cart service
builder.Services.AddScoped<IQuotationCartService, QuotationCartService>();

// Media library service
builder.Services.AddScoped<IMediaFileService, MediaFileService>();

// Manual service
builder.Services.AddScoped<IManualService, ManualService>();

// Site setting service
builder.Services.AddScoped<ISiteSettingService, SiteSettingService>();

// Home popup service
builder.Services.AddScoped<IHomePopupService, HomePopupService>();

// Site text (editable public wording) service
builder.Services.AddScoped<ISiteTextService, SiteTextService>();

// Markdown rendering for rich description/content fields
builder.Services.AddScoped<IMarkdownService, MarkdownService>();

// Feature flags
builder.Services.Configure<FeatureFlagsOptions>(builder.Configuration.GetSection("FeatureFlags"));
builder.Services.AddScoped<IFeatureFlagService, FeatureFlagService>();

// File info (size display for admin uploads)
builder.Services.AddScoped<Needis.Web.Services.Files.IFileInfoService, Needis.Web.Services.Files.FileInfoService>();

// YouTube URL parsing (home banner YouTube media type)
builder.Services.AddScoped<IYoutubeUrlService, YoutubeUrlService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath         = "/Admin/Account/Login";
        options.AccessDeniedPath  = "/Admin/Account/AccessDenied";
        options.Cookie.Name       = "Needis.Admin.Auth";
        options.ExpireTimeSpan    = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly   = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// Register authorization policies for every permission key
string[] permissionKeys =
[
    "Dashboard.View",
    "AdminUser.View", "AdminUser.Create", "AdminUser.Edit", "AdminUser.Delete",
    "Role.View", "Role.Edit",
    "SiteSetting.View", "SiteSetting.Edit",
    "Banner.View", "Banner.Create", "Banner.Edit", "Banner.Delete",
    "Category.View", "Category.Create", "Category.Edit", "Category.Delete",
    "Product.View", "Product.Create", "Product.Edit", "Product.Delete",
    "About.View", "About.Edit",
    "Service.View", "Service.Edit",
    "Activity.View", "Activity.Edit",
    "Quotation.View", "Quotation.Edit",
    "ContactMessage.View", "ContactMessage.Edit",
    "UsageStatistic.View",
    "EmailLog.View",
    "Media.View", "Media.Upload", "Media.Edit", "Media.Delete",
    "Seo.View", "Seo.Edit",
    "Manual.View",
    "HomePopup.View", "HomePopup.Create", "HomePopup.Edit", "HomePopup.Delete",
    "SiteText.View", "SiteText.Edit",
];

builder.Services.AddAuthorization(options =>
{
    foreach (var key in permissionKeys)
    {
        var capturedKey = key;
        options.AddPolicy($"Permission:{capturedKey}",
            policy => policy.Requirements.Add(new PermissionRequirement(capturedKey)));
    }
});

var app = builder.Build();

await AdminSeeder.SeedAsync(app.Services);
await RolePermissionSeeder.SeedAsync(app.Services);
await AboutSeeder.SeedAsync(app.Services);
await ServiceSeeder.SeedAsync(app.Services);
await ActivitySeeder.SeedAsync(app.Services);
await SeoSeeder.SeedAsync(app.Services);
await SiteTextSeeder.SeedAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<SeoRedirectMiddleware>();
app.UseAuthentication();
app.UseMiddleware<UsageStatisticsMiddleware>();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "product-detail",
    pattern: "Product/Detail/{slug}",
    defaults: new { controller = "Product", action = "Detail" });

app.MapControllerRoute(
    name: "service-detail",
    pattern: "Services/Detail/{slug}",
    defaults: new { controller = "Services", action = "Detail" });

app.MapControllerRoute(
    name: "activity-detail",
    pattern: "Activity/Detail/{slug}",
    defaults: new { controller = "Activity", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
