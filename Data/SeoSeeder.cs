using Microsoft.EntityFrameworkCore;
using Needis.Web.Models;

namespace Needis.Web.Data;

public static class SeoSeeder
{
    private static readonly (string PageKey, string? RoutePath, string TitleEN, string TitleTH,
        string DescEN, string DescTH, bool InSitemap, decimal Priority, string Freq, string Robots)[] Seeds =
    [
        ("home", "/",
            "Needis | Measuring Tools and Scientific Instruments",
            "Needis | เครื่องมือวัดและเครื่องมือวิทยาศาสตร์",
            "Needis provides measuring tools, scientific instruments, calibration, installation, and technical support.",
            "Needis ให้บริการเครื่องมือวัด เครื่องมือวิทยาศาสตร์ งานสอบเทียบ ติดตั้ง และสนับสนุนทางเทคนิค",
            true, 1.0m, "daily", "index, follow"),

        ("about", "/About",
            "About Us | Needis",
            "เกี่ยวกับเรา | Needis",
            "Learn about Needis – our history, team, and expertise in measuring and scientific instruments.",
            "เรียนรู้เกี่ยวกับ Needis ประวัติ ทีมงาน และความเชี่ยวชาญด้านเครื่องมือวัดและวิทยาศาสตร์",
            true, 0.7m, "monthly", "index, follow"),

        ("products", "/Product",
            "Products | Measuring & Scientific Instruments | Needis",
            "สินค้า | เครื่องมือวัดและวิทยาศาสตร์ | Needis",
            "Browse our range of measuring tools, laboratory equipment, and scientific instruments.",
            "เลือกชมเครื่องมือวัด อุปกรณ์ห้องแล็บ และเครื่องมือวิทยาศาสตร์ของเรา",
            true, 0.8m, "weekly", "index, follow"),

        ("services", "/Services",
            "Services | Calibration, Installation & Support | Needis",
            "บริการ | สอบเทียบ ติดตั้ง และซัพพอร์ต | Needis",
            "Needis offers calibration, installation, maintenance, and technical consulting services.",
            "Needis ให้บริการสอบเทียบ ติดตั้ง ซ่อมบำรุง และที่ปรึกษาทางเทคนิค",
            true, 0.8m, "weekly", "index, follow"),

        ("activity", "/Activity",
            "News & Activities | Needis",
            "ข่าวและกิจกรรม | Needis",
            "Stay updated with the latest news, activities, and events from Needis.",
            "ติดตามข่าวสาร กิจกรรม และเหตุการณ์ล่าสุดจาก Needis",
            true, 0.7m, "weekly", "index, follow"),

        ("contact", "/Contact",
            "Contact Us | Needis",
            "ติดต่อเรา | Needis",
            "Get in touch with Needis for product inquiries, technical support, or partnership.",
            "ติดต่อ Needis สำหรับสอบถามสินค้า ซัพพอร์ทเทคนิค หรือความร่วมมือ",
            true, 0.5m, "monthly", "index, follow"),

        ("quotation", "/Quotation/Create",
            "Request Quotation | Needis",
            "ขอใบเสนอราคา | Needis",
            "Submit a quotation request for our instruments and services.",
            "ส่งคำขอใบเสนอราคาสำหรับเครื่องมือและบริการของเรา",
            false, 0.3m, "monthly", "noindex, nofollow"),
    ];

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existingKeys = (await db.SeoSettings.AsNoTracking()
            .Where(s => s.EntityId == null)
            .Select(s => s.PageKey)
            .ToListAsync())
            .ToHashSet();

        foreach (var (pageKey, routePath, titleEN, titleTH, descEN, descTH, inSitemap, priority, freq, robots) in Seeds)
        {
            if (existingKeys.Contains(pageKey)) continue;

            db.SeoSettings.Add(new SeoSetting
            {
                PageKey             = pageKey,
                RoutePath           = routePath,
                MetaTitleEN         = titleEN,
                MetaTitleTH         = titleTH,
                MetaDescriptionEN   = descEN,
                MetaDescriptionTH   = descTH,
                IncludeInSitemap    = inSitemap,
                Priority            = priority,
                ChangeFrequency     = freq,
                Robots              = robots,
                IsActive            = true,
                IsDelete            = false,
                CreatedAt           = DateTime.UtcNow,
                CreatedBy           = "Seeder",
            });
        }

        await db.SaveChangesAsync();
    }
}
