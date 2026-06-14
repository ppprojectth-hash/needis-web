using Microsoft.EntityFrameworkCore;
using Needis.Web.Models;

namespace Needis.Web.Data;

public static class AboutSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await SeedSectionsAsync(db);
        await SeedStatCardsAsync(db);
        await SeedHistoryAsync(db);
    }

    private static async Task SeedSectionsAsync(AppDbContext db)
    {
        if (await db.AboutSections.AnyAsync()) return;

        var sections = new[]
        {
            new AboutSection
            {
                SectionKey   = "our_story",
                TitleTH      = "เรื่องราวของเรา",
                TitleEN      = "Our Story",
                SubtitleTH   = "จากจุดเริ่มต้นสู่ผู้นำด้านเครื่องมือวัด",
                SubtitleEN   = "From humble beginnings to a leading instrument provider",
                DescriptionTH = "Needis ก่อตั้งขึ้นด้วยความมุ่งมั่นในการจัดหาเครื่องมือวัดคุณภาพสูงสำหรับอุตสาหกรรมไทย",
                DescriptionEN = "Needis was founded with a commitment to providing high-quality measuring instruments for Thai industry.",
                LayoutType   = "text_left",
                DisplayOrder = 1,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
                CreatedBy    = "seeder",
            },
            new AboutSection
            {
                SectionKey   = "our_mission",
                TitleTH      = "พันธกิจของเรา",
                TitleEN      = "Our Mission",
                SubtitleTH   = "มุ่งมั่นสู่ความเป็นเลิศ",
                SubtitleEN   = "Committed to excellence",
                DescriptionTH = "ส่งมอบเครื่องมือวัดและบริการที่มีคุณภาพสูงสุด",
                DescriptionEN = "To deliver the highest quality measuring instruments and services.",
                LayoutType   = "cards",
                DisplayOrder = 2,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
                CreatedBy    = "seeder",
            },
            new AboutSection
            {
                SectionKey   = "our_vision",
                TitleTH      = "วิสัยทัศน์ของเรา",
                TitleEN      = "Our Vision",
                SubtitleTH   = "เป็นผู้นำด้านเครื่องมือวัดในอาเซียน",
                SubtitleEN   = "To be the leading instrument provider in ASEAN",
                LayoutType   = "cards",
                DisplayOrder = 3,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
                CreatedBy    = "seeder",
            },
        };

        db.AboutSections.AddRange(sections);
        await db.SaveChangesAsync();
    }

    private static async Task SeedStatCardsAsync(AppDbContext db)
    {
        if (await db.AboutStatCards.AnyAsync()) return;

        var cards = new[]
        {
            new AboutStatCard
            {
                StatKey      = "global_brands",
                LabelTH      = "แบรนด์ระดับโลก",
                LabelEN      = "Global Brands",
                DescriptionTH = "พันธมิตรแบรนด์ชั้นนำระดับสากล",
                DescriptionEN = "World-class brand partners",
                SourceType   = "GlobalBrandCount",
                Suffix       = "+",
                IconUrl      = "bi bi-globe",
                DisplayOrder = 1,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
                CreatedBy    = "seeder",
            },
            new AboutStatCard
            {
                StatKey      = "expert_staff",
                LabelTH      = "บุคลากรผู้เชี่ยวชาญ",
                LabelEN      = "Expert Staff",
                DescriptionTH = "ทีมผู้เชี่ยวชาญด้านเทคนิค",
                DescriptionEN = "Certified technical specialists",
                SourceType   = "ExpertStaffCount",
                Suffix       = "+",
                IconUrl      = "bi bi-people-fill",
                DisplayOrder = 2,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
                CreatedBy    = "seeder",
            },
            new AboutStatCard
            {
                StatKey      = "product_sold",
                LabelTH      = "เครื่องมือที่จำหน่ายแล้ว",
                LabelEN      = "Instruments Delivered",
                DescriptionTH = "เครื่องมือที่ส่งมอบให้ลูกค้าทั่วประเทศ",
                DescriptionEN = "Instruments delivered to customers nationwide",
                SourceType   = "ProductSoldCount",
                Suffix       = "+",
                IconUrl      = "bi bi-box-seam-fill",
                DisplayOrder = 3,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
                CreatedBy    = "seeder",
            },
            new AboutStatCard
            {
                StatKey      = "technical_support",
                LabelTH      = "บริการสนับสนุนทางเทคนิค",
                LabelEN      = "Technical Support",
                DescriptionTH = "พร้อมให้บริการตลอดเวลา",
                DescriptionEN = "Always available for your needs",
                SourceType   = "Manual",
                ManualValue  = 24,
                Suffix       = "/7",
                IconUrl      = "bi bi-headset",
                DisplayOrder = 4,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
                CreatedBy    = "seeder",
            },
        };

        db.AboutStatCards.AddRange(cards);
        await db.SaveChangesAsync();
    }

    private static async Task SeedHistoryAsync(AppDbContext db)
    {
        if (await db.AboutCompanyHistories.AnyAsync()) return;

        var history = new[]
        {
            new AboutCompanyHistory
            {
                Year         = 2005,
                TitleTH      = "ก่อตั้งบริษัท",
                TitleEN      = "Company Founded",
                DescriptionTH = "Needis ก่อตั้งขึ้นด้วยทีมผู้เชี่ยวชาญด้านเครื่องมือวัดอุตสาหกรรม",
                DescriptionEN = "Needis was established by a team of industrial measurement specialists.",
                Position     = "left",
                DisplayOrder = 1,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
                CreatedBy    = "seeder",
            },
            new AboutCompanyHistory
            {
                Year         = 2010,
                TitleTH      = "รับรองมาตรฐาน ISO 17025",
                TitleEN      = "ISO 17025 Accreditation",
                DescriptionTH = "ได้รับการรับรองห้องปฏิบัติการสอบเทียบตามมาตรฐาน ISO/IEC 17025",
                DescriptionEN = "Received calibration laboratory accreditation to ISO/IEC 17025 standard.",
                Position     = "right",
                DisplayOrder = 2,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
                CreatedBy    = "seeder",
            },
            new AboutCompanyHistory
            {
                Year         = 2015,
                TitleTH      = "ขยายสู่ตลาดสิ่งแวดล้อม",
                TitleEN      = "Expanded to Environmental Monitoring",
                DescriptionTH = "ขยายธุรกิจสู่เครื่องมือตรวจวัดสิ่งแวดล้อมและห้องปฏิบัติการ",
                DescriptionEN = "Expanded into environmental monitoring and laboratory instruments.",
                Position     = "left",
                DisplayOrder = 3,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
                CreatedBy    = "seeder",
            },
            new AboutCompanyHistory
            {
                Year         = 2020,
                TitleTH      = "สำนักงานใหม่",
                TitleEN      = "New Headquarters",
                DescriptionTH = "ย้ายสู่สำนักงานใหม่เพื่อรองรับการเติบโตของธุรกิจ",
                DescriptionEN = "Moved to a new headquarters to support continued business growth.",
                Position     = "right",
                DisplayOrder = 4,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
                CreatedBy    = "seeder",
            },
        };

        db.AboutCompanyHistories.AddRange(history);
        await db.SaveChangesAsync();
    }
}
