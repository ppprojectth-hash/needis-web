using Microsoft.EntityFrameworkCore;
using ActivityModel = Needis.Web.Models.Activity;
using Needis.Web.Models;

namespace Needis.Web.Data;

public static class ActivitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await SeedPageAsync(db);
        await SeedTagsAsync(db);
        await SeedActivitiesAsync(db);
    }

    private static async Task SeedPageAsync(AppDbContext db)
    {
        if (await db.ActivityPages.AnyAsync()) return;
        db.ActivityPages.Add(new ActivityPage
        {
            PageKey          = "activity_main",
            TitleEN          = "Activity",
            TitleTH          = "กิจกรรมและข่าวสาร",
            SubtitleEN       = "Latest news, events, articles, and promotions.",
            SubtitleTH       = "ข่าวสาร กิจกรรม บทความ และโปรโมชั่นล่าสุด",
            BreadcrumbTextEN = "Activity",
            BreadcrumbTextTH = "กิจกรรม",
            DisplayOrder     = 1,
            IsActive         = true,
            CreatedAt        = DateTime.UtcNow,
            CreatedBy        = "seeder",
        });
        await db.SaveChangesAsync();
    }

    private static async Task SeedTagsAsync(AppDbContext db)
    {
        if (await db.ActivityTags.AnyAsync()) return;
        db.ActivityTags.AddRange(new[]
        {
            new ActivityTag { TagKey="news",      TagNameEN="News",      TagNameTH="ข่าวสาร",   TagColor="#2d4199", IsFilterable=true, DisplayOrder=1, IsActive=true, CreatedAt=DateTime.UtcNow, CreatedBy="seeder" },
            new ActivityTag { TagKey="article",   TagNameEN="Article",   TagNameTH="บทความ",    TagColor="#198754", IsFilterable=true, DisplayOrder=2, IsActive=true, CreatedAt=DateTime.UtcNow, CreatedBy="seeder" },
            new ActivityTag { TagKey="promotion", TagNameEN="Promotion", TagNameTH="โปรโมชั่น", TagColor="#ffc107", IsFilterable=true, DisplayOrder=3, IsActive=true, CreatedAt=DateTime.UtcNow, CreatedBy="seeder" },
            new ActivityTag { TagKey="event",     TagNameEN="Event",     TagNameTH="กิจกรรม",  TagColor="#0dcaf0", IsFilterable=true, DisplayOrder=4, IsActive=true, CreatedAt=DateTime.UtcNow, CreatedBy="seeder" },
        });
        await db.SaveChangesAsync();
    }

    private static async Task SeedActivitiesAsync(AppDbContext db)
    {
        if (await db.Activities.AnyAsync()) return;

        var now = DateTime.UtcNow;

        // Load tags
        var tags = await db.ActivityTags.ToDictionaryAsync(t => t.TagKey, t => t.Id);

        // Activity 1: ASEAN Lab Expo
        var a1 = new ActivityModel
        {
            ActivitySlug       = "hplc-technology-asean-lab-expo-2026",
            ActivityTitleEN    = "SciInstruments Showcases Latest HPLC Technology at ASEAN Lab Expo 2026",
            ActivityTitleTH    = "Needis จัดแสดงเทคโนโลยี HPLC ล่าสุดในงาน ASEAN Lab Expo 2026",
            ShortDescriptionEN = "We were proud to showcase our latest HPLC solutions and measurement instruments at ASEAN Lab Expo 2026.",
            ShortDescriptionTH = "เรายินดีนำเสนอโซลูชัน HPLC ล่าสุดและเครื่องมือวัดในงาน ASEAN Lab Expo 2026",
            IsFeatured         = true,
            IsPublished        = true,
            DisplayOrder       = 1,
            IsActive           = true,
            ActivityDate       = now.AddDays(-30),
            PublishedDate      = now.AddDays(-30),
            AuthorName         = "Needis Team",
            CreatedAt          = now,
            CreatedBy          = "seeder",
        };
        db.Activities.Add(a1);
        await db.SaveChangesAsync();

        db.ActivityTagMaps.AddRange(new[]
        {
            new ActivityTagMap { ActivityId=a1.Id, ActivityTagId=tags["event"], IsPrimary=true,  DisplayOrder=1, CreatedAt=now, CreatedBy="seeder" },
            new ActivityTagMap { ActivityId=a1.Id, ActivityTagId=tags["news"],  IsPrimary=false, DisplayOrder=2, CreatedAt=now, CreatedBy="seeder" },
        });
        db.ActivityDetailBlocks.AddRange(new[]
        {
            new ActivityDetailBlock { ActivityId=a1.Id, BlockType="heading", BlockTitleEN="Event Highlights",       BlockTitleTH="ไฮไลต์ของงาน",                                                                                                                                                                     DisplayOrder=1, IsActive=true, CreatedAt=now, CreatedBy="seeder" },
            new ActivityDetailBlock { ActivityId=a1.Id, BlockType="text",    BlockContentEN="The event featured live demonstrations of our latest HPLC systems, drawing interest from laboratory professionals across the ASEAN region.", BlockContentTH="งานนี้มีการสาธิตระบบ HPLC ล่าสุดของเราแบบสด ซึ่งดึงดูดความสนใจจากผู้เชี่ยวชาญในห้องปฏิบัติการทั่วภูมิภาคอาเซียน", DisplayOrder=2, IsActive=true, CreatedAt=now, CreatedBy="seeder" },
            new ActivityDetailBlock { ActivityId=a1.Id, BlockType="button",  ButtonTextEN="Contact Us for Demo",   ButtonTextTH="ติดต่อสอบถามการสาธิต",                                                                                   ButtonUrl="/Contact",                                                         DisplayOrder=3, IsActive=true, CreatedAt=now, CreatedBy="seeder" },
        });
        await db.SaveChangesAsync();

        // Activity 2: Shimadzu Partnership
        var a2 = new ActivityModel
        {
            ActivitySlug       = "shimadzu-advanced-spectroscopy-partnership",
            ActivityTitleEN    = "New Partnership with Shimadzu for Advanced Spectroscopy Solutions",
            ActivityTitleTH    = "ความร่วมมือใหม่กับ Shimadzu สำหรับโซลูชันสเปกโตรสโคปีขั้นสูง",
            ShortDescriptionEN = "We are excited to announce our new authorized partnership with Shimadzu Corporation for advanced spectroscopy instruments.",
            ShortDescriptionTH = "เรายินดีประกาศความร่วมมือใหม่กับ Shimadzu Corporation สำหรับเครื่องมือสเปกโตรสโคปีขั้นสูง",
            IsFeatured         = true,
            IsPublished        = true,
            DisplayOrder       = 2,
            IsActive           = true,
            ActivityDate       = now.AddDays(-15),
            PublishedDate      = now.AddDays(-15),
            AuthorName         = "Needis Team",
            CreatedAt          = now,
            CreatedBy          = "seeder",
        };
        db.Activities.Add(a2);
        await db.SaveChangesAsync();

        db.ActivityTagMaps.Add(new ActivityTagMap { ActivityId=a2.Id, ActivityTagId=tags["news"], IsPrimary=true, DisplayOrder=1, CreatedAt=now, CreatedBy="seeder" });
        db.ActivityDetailBlocks.AddRange(new[]
        {
            new ActivityDetailBlock { ActivityId=a2.Id, BlockType="heading", BlockTitleEN="Partnership Details",   BlockTitleTH="รายละเอียดความร่วมมือ", DisplayOrder=1, IsActive=true, CreatedAt=now, CreatedBy="seeder" },
            new ActivityDetailBlock { ActivityId=a2.Id, BlockType="text",    BlockContentEN="As an authorized Shimadzu partner, we can now provide full support for their spectroscopy product line including UV-Vis, FTIR, and fluorescence spectrometers.", BlockContentTH="ในฐานะพันธมิตรที่ได้รับอนุญาตจาก Shimadzu เราสามารถให้การสนับสนุนอย่างเต็มรูปแบบสำหรับสายผลิตภัณฑ์สเปกโตรสโคปี", DisplayOrder=2, IsActive=true, CreatedAt=now, CreatedBy="seeder" },
        });
        await db.SaveChangesAsync();

        // Activity 3: GC-MS Guide
        var a3 = new ActivityModel
        {
            ActivitySlug       = "gc-ms-guide-laboratory-professionals",
            ActivityTitleEN    = "Understanding GC-MS: A Comprehensive Guide for Laboratory Professionals",
            ActivityTitleTH    = "ทำความเข้าใจ GC-MS: คู่มือครบถ้วนสำหรับผู้เชี่ยวชาญห้องปฏิบัติการ",
            ShortDescriptionEN = "Gas Chromatography-Mass Spectrometry (GC-MS) is one of the most powerful analytical techniques available. This guide covers the fundamentals.",
            ShortDescriptionTH = "แก๊สโครมาโทกราฟี-แมสสเปกโตรเมทรี (GC-MS) เป็นหนึ่งในเทคนิควิเคราะห์ที่ทรงพลังที่สุด คู่มือนี้ครอบคลุมพื้นฐาน",
            IsFeatured         = false,
            IsPublished        = true,
            DisplayOrder       = 3,
            IsActive           = true,
            PublishedDate      = now.AddDays(-7),
            AuthorName         = "Technical Team",
            CreatedAt          = now,
            CreatedBy          = "seeder",
        };
        db.Activities.Add(a3);
        await db.SaveChangesAsync();

        db.ActivityTagMaps.Add(new ActivityTagMap { ActivityId=a3.Id, ActivityTagId=tags["article"], IsPrimary=true, DisplayOrder=1, CreatedAt=now, CreatedBy="seeder" });
        db.ActivityDetailBlocks.AddRange(new[]
        {
            new ActivityDetailBlock { ActivityId=a3.Id, BlockType="heading", BlockTitleEN="What is GC-MS?", BlockTitleTH="GC-MS คืออะไร?", DisplayOrder=1, IsActive=true, CreatedAt=now, CreatedBy="seeder" },
            new ActivityDetailBlock { ActivityId=a3.Id, BlockType="text",    BlockContentEN="GC-MS combines gas chromatography with mass spectrometry to identify different substances within a test sample. The technique is widely used in environmental monitoring, food safety, and pharmaceutical analysis.", BlockContentTH="GC-MS รวมแก๊สโครมาโทกราฟีกับแมสสเปกโตรเมทรีเพื่อระบุสารต่างๆ ในตัวอย่างทดสอบ เทคนิคนี้ใช้กันอย่างแพร่หลายในการตรวจสอบสิ่งแวดล้อม ความปลอดภัยอาหาร และการวิเคราะห์เภสัชกรรม", DisplayOrder=2, IsActive=true, CreatedAt=now, CreatedBy="seeder" },
        });
        await db.SaveChangesAsync();

        // Activity 4: Free Calibration Promotion
        var a4 = new ActivityModel
        {
            ActivitySlug       = "free-calibration-new-instrument-purchase",
            ActivityTitleEN    = "Special Promotion: Free Calibration with New Instrument Purchase",
            ActivityTitleTH    = "โปรโมชั่นพิเศษ: รับบริการสอบเทียบฟรีเมื่อซื้อเครื่องมือใหม่",
            ShortDescriptionEN = "For a limited time, receive complimentary ISO-traceable calibration service with every new instrument purchase. Valid through end of quarter.",
            ShortDescriptionTH = "สำหรับช่วงเวลาจำกัด รับบริการสอบเทียบ ISO ฟรีเมื่อซื้อเครื่องมือใหม่ทุกชิ้น ใช้ได้ถึงสิ้นไตรมาส",
            IsFeatured         = false,
            IsPublished        = true,
            DisplayOrder       = 4,
            IsActive           = true,
            PublishedDate      = now.AddDays(-3),
            AuthorName         = "Sales Team",
            CreatedAt          = now,
            CreatedBy          = "seeder",
        };
        db.Activities.Add(a4);
        await db.SaveChangesAsync();

        db.ActivityTagMaps.Add(new ActivityTagMap { ActivityId=a4.Id, ActivityTagId=tags["promotion"], IsPrimary=true, DisplayOrder=1, CreatedAt=now, CreatedBy="seeder" });
        db.ActivityDetailBlocks.AddRange(new[]
        {
            new ActivityDetailBlock { ActivityId=a4.Id, BlockType="heading", BlockTitleEN="Promotion Details",   BlockTitleTH="รายละเอียดโปรโมชั่น",   DisplayOrder=1, IsActive=true, CreatedAt=now, CreatedBy="seeder" },
            new ActivityDetailBlock { ActivityId=a4.Id, BlockType="text",    BlockContentEN="Valid for all instrument purchases made before the end of this quarter. Calibration will be performed at our ISO/IEC 17025 accredited facility.", BlockContentTH="ใช้ได้กับการซื้อเครื่องมือทุกชิ้นก่อนสิ้นไตรมาสนี้ การสอบเทียบจะดำเนินการที่สถานที่ที่ได้รับการรับรอง ISO/IEC 17025 ของเรา", DisplayOrder=2, IsActive=true, CreatedAt=now, CreatedBy="seeder" },
            new ActivityDetailBlock { ActivityId=a4.Id, BlockType="button",  ButtonTextEN="Claim This Offer",   ButtonTextTH="รับข้อเสนอนี้",            ButtonUrl="/Quotation/Create",                                                  DisplayOrder=3, IsActive=true, CreatedAt=now, CreatedBy="seeder" },
        });
        await db.SaveChangesAsync();
    }
}
