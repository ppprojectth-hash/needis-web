using Microsoft.EntityFrameworkCore;
using Needis.Web.Models;

namespace Needis.Web.Data;

public static class ServiceSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await SeedPageAsync(db);
        await SeedServicesAsync(db);
    }

    private static async Task SeedPageAsync(AppDbContext db)
    {
        if (await db.ServicePages.AnyAsync()) return;

        db.ServicePages.Add(new ServicePage
        {
            PageKey      = "services_main",
            TitleEN      = "Our Services",
            TitleTH      = "บริการของเรา",
            SubtitleEN   = "Comprehensive support, calibration, and maintenance for all your instruments.",
            SubtitleTH   = "บริการสนับสนุน สอบเทียบ ติดตั้ง และบำรุงรักษาเครื่องมืออย่างครบวงจร",
            DisplayOrder = 1,
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow,
            CreatedBy    = "seeder",
        });
        await db.SaveChangesAsync();
    }

    private static async Task SeedServicesAsync(AppDbContext db)
    {
        if (await db.Services.AnyAsync()) return;

        var phone = await db.SiteSettings.AsNoTracking()
            .Where(s => s.IsActive)
            .Select(s => s.ContactPhone)
            .FirstOrDefaultAsync();

        // Service 1: Calibration
        var cal = new Models.Service
        {
            ServiceCode        = "CALIBRATION",
            ServiceSlug        = "calibration-certification",
            ServiceNameEN      = "Calibration & Certification",
            ServiceNameTH      = "บริการสอบเทียบและออกใบรับรอง",
            ShortDescriptionEN = "ISO/IEC 17025 traceable calibration with official certification for industrial and laboratory instruments.",
            ShortDescriptionTH = "บริการสอบเทียบที่สืบสาวได้ตาม ISO/IEC 17025 พร้อมออกใบรับรองอย่างเป็นทางการ",
            DetailTitleEN      = "Calibration & Certification",
            DetailTitleTH      = "บริการสอบเทียบและออกใบรับรอง",
            DetailSubtitleEN   = "Traceable to ISO/IEC 17025",
            DetailSubtitleTH   = "สืบสาวได้ตามมาตรฐาน ISO/IEC 17025",
            IsFeatured         = true,
            DisplayOrder       = 1,
            IsActive           = true,
            CreatedAt          = DateTime.UtcNow,
            CreatedBy          = "seeder",
        };
        db.Services.Add(cal);
        await db.SaveChangesAsync();

        var calOverview = new ServiceDetailSection
        {
            ServiceId            = cal.Id,
            SectionKey           = "overview",
            SectionTitleEN       = "Service Overview",
            SectionTitleTH       = "ภาพรวมบริการ",
            SectionDescriptionEN = "Our calibration service covers all types of industrial and laboratory measuring instruments including temperature, pressure, flow, and electrical devices.",
            SectionDescriptionTH = "บริการสอบเทียบครอบคลุมเครื่องมือวัดทุกประเภท ทั้งอุตสาหกรรมและห้องปฏิบัติการ",
            LayoutType           = "text",
            DisplayOrder         = 1,
            IsActive             = true,
            CreatedAt            = DateTime.UtcNow,
            CreatedBy            = "seeder",
        };
        db.ServiceDetailSections.Add(calOverview);
        await db.SaveChangesAsync();

        var calScope = new ServiceDetailSection
        {
            ServiceId      = cal.Id,
            SectionKey     = "scope_of_work",
            SectionTitleEN = "Scope of Work",
            SectionTitleTH = "ขอบเขตการให้บริการ",
            LayoutType     = "bullet",
            DisplayOrder   = 2,
            IsActive       = true,
            CreatedAt      = DateTime.UtcNow,
            CreatedBy      = "seeder",
        };
        db.ServiceDetailSections.Add(calScope);
        await db.SaveChangesAsync();

        var scopeItems = new[]
        {
            new ServiceScopeItem { ServiceDetailSectionId = calScope.Id, ItemTitleEN = "Field assessment and diagnostics",               ItemTitleTH = "ตรวจสอบภาคสนามและวินิจฉัย",                      DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "seeder" },
            new ServiceScopeItem { ServiceDetailSectionId = calScope.Id, ItemTitleEN = "Cleaning and preventive maintenance",            ItemTitleTH = "ทำความสะอาดและบำรุงรักษาเชิงป้องกัน",           DisplayOrder = 2, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "seeder" },
            new ServiceScopeItem { ServiceDetailSectionId = calScope.Id, ItemTitleEN = "Calibration against ISO/IEC 17025 standards",   ItemTitleTH = "สอบเทียบตามมาตรฐาน ISO/IEC 17025",            DisplayOrder = 3, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "seeder" },
            new ServiceScopeItem { ServiceDetailSectionId = calScope.Id, ItemTitleEN = "Issuance of official certification documents",   ItemTitleTH = "ออกใบรับรองอย่างเป็นทางการ",                    DisplayOrder = 4, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "seeder" },
            new ServiceScopeItem { ServiceDetailSectionId = calScope.Id, ItemTitleEN = "Staff training on equipment usage",              ItemTitleTH = "ฝึกอบรมเจ้าหน้าที่การใช้อุปกรณ์",              DisplayOrder = 5, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "seeder" },
        };
        db.ServiceScopeItems.AddRange(scopeItems);
        await db.SaveChangesAsync();

        db.ServiceContactCards.Add(new ServiceContactCard
        {
            ServiceId             = cal.Id,
            TitleEN               = "Need this service?",
            TitleTH               = "ต้องการบริการนี้?",
            PrimaryButtonTextEN   = "Contact Support",
            PrimaryButtonTextTH   = "ติดต่อทีมสนับสนุน",
            PrimaryButtonUrl      = "/Contact",
            SecondaryButtonTextEN = "Request Quotation",
            SecondaryButtonTextTH = "ขอใบเสนอราคา",
            SecondaryButtonUrl    = "/Quotation/Create",
            ContactLabelEN        = "Direct Service Line",
            ContactLabelTH        = "ช่องทางติดต่อบริการ",
            ContactValue          = phone ?? "Please contact us",
            DisplayOrder          = 1,
            IsActive              = true,
            CreatedAt             = DateTime.UtcNow,
            CreatedBy             = "seeder",
        });
        await db.SaveChangesAsync();

        // Service 2: Installation
        var inst = new Models.Service
        {
            ServiceCode        = "INSTALLATION",
            ServiceSlug        = "installation-system-setup",
            ServiceNameEN      = "Installation & System Setup",
            ServiceNameTH      = "บริการติดตั้งและตั้งค่าระบบ",
            ShortDescriptionEN = "Professional installation, commissioning, and system integration for all instrument types.",
            ShortDescriptionTH = "บริการติดตั้ง คอมมิชชันนิ่ง และบูรณาการระบบสำหรับเครื่องมือทุกประเภท",
            DetailTitleEN      = "Installation & System Setup",
            DetailTitleTH      = "บริการติดตั้งและตั้งค่าระบบ",
            IsFeatured         = true,
            DisplayOrder       = 2,
            IsActive           = true,
            CreatedAt          = DateTime.UtcNow,
            CreatedBy          = "seeder",
        };
        db.Services.Add(inst);
        await db.SaveChangesAsync();

        db.ServiceDetailSections.AddRange(new[]
        {
            new ServiceDetailSection
            {
                ServiceId            = inst.Id,
                SectionKey           = "overview",
                SectionTitleEN       = "Service Overview",
                SectionTitleTH       = "ภาพรวมบริการ",
                SectionDescriptionEN = "Our installation team handles site preparation, instrument mounting, wiring, and full commissioning.",
                SectionDescriptionTH = "ทีมติดตั้งของเราดูแลการเตรียมสถานที่ การติดตั้งเครื่องมือ การเดินสาย และการ commissioning อย่างครบวงจร",
                LayoutType           = "text",
                DisplayOrder         = 1,
                IsActive             = true,
                CreatedAt            = DateTime.UtcNow,
                CreatedBy            = "seeder",
            },
            new ServiceDetailSection
            {
                ServiceId      = inst.Id,
                SectionKey     = "scope_of_work",
                SectionTitleEN = "Scope of Work",
                SectionTitleTH = "ขอบเขตการให้บริการ",
                LayoutType     = "bullet",
                DisplayOrder   = 2,
                IsActive       = true,
                CreatedAt      = DateTime.UtcNow,
                CreatedBy      = "seeder",
            },
        });
        await db.SaveChangesAsync();
    }
}
