using Microsoft.EntityFrameworkCore;
using Needis.Web.Models;

namespace Needis.Web.Data;

public static class SiteTextSeeder
{
    // (Key, Page, Section, Label, TextTH, TextEN, Description, DisplayOrder)
    private static readonly (string Key, string Page, string? Section, string Label, string? TextTH, string? TextEN, string? Description, int Order)[] Defaults =
    [
        // ── Home ──────────────────────────────────────────────────────────────
        ("home.hero.title", "home", "hero", "Hero Title",
            "เครื่องมือวัดและวิทยาศาสตร์เพื่อความแม่นยำระดับมืออาชีพ", "Precision Instruments for Scientific Excellence",
            "Main headline in the home page hero section.", 1),
        ("home.hero.subtitle", "home", "hero", "Hero Subtitle",
            "สนับสนุนงานวิจัยและอุตสาหกรรมด้วยเครื่องมือวิเคราะห์ ทดสอบ และตรวจวัดที่มีประสิทธิภาพสูง",
            "Empowering research and industry with cutting-edge analytical, testing, and measurement instruments.",
            "Subtitle text below the hero headline.", 2),
        ("home.hero.primary_button", "home", "hero", "Hero Primary Button",
            "สำรวจสินค้า", "Explore Products", "Primary call-to-action button in the hero section.", 3),
        ("home.hero.secondary_button", "home", "hero", "Hero Secondary Button",
            "ติดต่อเรา", "Contact Us", "Secondary button in the hero section.", 4),

        ("home.product_lines.eyebrow", "home", "product_lines", "Product Lines Eyebrow",
            "หมวดหมู่สินค้า", "CATEGORIES", "Small label above the product categories heading.", 5),
        ("home.product_lines.title", "home", "product_lines", "Product Lines Title",
            "สำรวจกลุ่มผลิตภัณฑ์ของเรา", "Explore Our Product Lines", "Heading for the product categories section.", 6),
        ("home.product_lines.subtitle", "home", "product_lines", "Product Lines Subtitle",
            "เครื่องมือและอุปกรณ์จากแบรนด์ชั้นนำสำหรับทุกอุตสาหกรรมและการวิจัย",
            "Precision instruments and equipment from leading brands, serving every industry and research need.",
            "Subtitle for the product categories section.", 7),

        ("home.hot_products.eyebrow", "home", "hot_products", "Hot Products Eyebrow",
            "สินค้าแนะนำ", "HOT PRODUCTS", "Small label above the hot products heading.", 8),
        ("home.hot_products.title", "home", "hot_products", "Hot Products Title",
            "สินค้าแนะนำ / โปรโมชั่น", "Hot Product / Promotion", "Heading for the hot products section.", 9),
        ("home.hot_products.subtitle", "home", "hot_products", "Hot Products Subtitle",
            "เลือกชมสินค้าแนะนำและโปรโมชั่นที่เราคัดสรรไว้ให้คุณ",
            "Discover highlighted products and current promotions selected for you.",
            "Subtitle for the hot products section.", 10),

        ("home.services.eyebrow", "home", "services", "Services Eyebrow",
            "บริการของเรา", "SERVICES", "Small label above the services heading.", 11),
        ("home.services.title", "home", "services", "Services Title",
            "บริการและการสนับสนุนอย่างครบวงจร", "Comprehensive Support & Services", "Heading for the home page services section.", 12),
        ("home.services.subtitle", "home", "services", "Services Subtitle",
            "เราให้บริการครบวงจรตั้งแต่การจำหน่ายจนถึงการสอบเทียบและบำรุงรักษา",
            "From procurement and installation to calibration and maintenance — we support you end-to-end.",
            "Subtitle for the home page services section.", 13),

        ("home.activities.eyebrow", "home", "activities", "Activities Eyebrow",
            "กิจกรรม", "ACTIVITY", "Small label above the activities heading.", 14),
        ("home.activities.title", "home", "activities", "Activities Title",
            "ข่าวสารและอัปเดต", "News & Updates", "Heading for the home page activities section.", 15),
        ("home.activities.subtitle", "home", "activities", "Activities Subtitle",
            "ติดตามข่าวสาร กิจกรรม และนวัตกรรมล่าสุดจากทีมงานของเรา",
            "Stay updated with our latest news, events, and innovations.",
            "Subtitle for the home page activities section.", 16),

        ("home.cta.title", "home", "cta", "CTA Title",
            "พร้อมค้นหาเครื่องมือที่เหมาะกับงานของคุณหรือยัง?", "Ready to Find the Right Instrument?",
            "Headline for the bottom call-to-action section.", 17),
        ("home.cta.subtitle", "home", "cta", "CTA Subtitle",
            "ทีมผู้เชี่ยวชาญของเราพร้อมให้คำปรึกษาและจัดทำใบเสนอราคาให้ฟรีภายใน 24 ชั่วโมง",
            "Our specialists will help you select the ideal instruments and provide a free quotation within 24 hours.",
            "Subtitle for the bottom call-to-action section.", 18),
        ("home.cta.button", "home", "cta", "CTA Button",
            "ขอใบเสนอราคา", "Request Quotation", "Primary button in the bottom call-to-action section.", 19),

        // ── About ─────────────────────────────────────────────────────────────
        ("about.page.title", "about", "hero", "Page Title",
            "เกี่ยวกับ Neediss", "About Neediss", "Main heading of the About page.", 1),
        ("about.page.subtitle", "about", "hero", "Page Subtitle",
            "พันธมิตรด้านเครื่องมือวัด เครื่องมือวิทยาศาสตร์ และบริการสนับสนุนทางเทคนิค",
            "Trusted partner for measuring tools, scientific instruments, and technical support.",
            "Subtitle under the About page heading.", 2),
        ("about.company.title", "about", "company", "Company Section Title",
            "เกี่ยวกับบริษัทของเรา", "About Our Company", "Generic company introduction heading (optional use).", 3),
        ("about.company.description", "about", "company", "Company Section Description",
            "Neediss เป็นผู้นำด้านเครื่องมือวัด เครื่องมือวิทยาศาสตร์ และอุปกรณ์ตรวจสอบสิ่งแวดล้อม",
            "Neediss is a leading provider of measuring instruments, scientific equipment, and environmental monitoring devices.",
            "Generic company introduction paragraph (optional use).", 4),
        ("about.team.eyebrow", "about", "team", "Team Eyebrow",
            "ทีมงานของเรา", "Meet the Team", "Small label above the team heading.", 5),
        ("about.team.title", "about", "team", "Team Title",
            "ทีมผู้เชี่ยวชาญของเรา", "Our Expert Team", "Heading for the staff/team section.", 6),
        ("about.team.subtitle", "about", "team", "Team Subtitle",
            "ผู้เชี่ยวชาญด้านเทคนิคที่พร้อมให้คำปรึกษาและสนับสนุนคุณ",
            "Technical specialists ready to advise and support your instrumentation needs.",
            "Subtitle for the staff/team section.", 7),
        ("about.location.title", "about", "location", "Location Title",
            "ที่ตั้งของเรา", "Our Location", "Heading for the map/location section (used when no custom map title is set).", 8),
        ("about.location.subtitle", "about", "location", "Location Subtitle",
            "ดูที่ตั้งบริษัทหรือเปิดตำแหน่งใน Google Maps", "Visit us or open the location in Google Maps.",
            "Subtitle for the map/location section (used when no custom map description is set).", 9),
        ("about.location.button", "about", "location", "Location Button",
            "เปิดใน Google Maps", "Open in Google Maps", "Button label to open the location in Google Maps.", 10),

        // ── Product ───────────────────────────────────────────────────────────
        ("product.page.title", "product", "hero", "Page Title",
            "สินค้าทั้งหมด", "Products", "Default title on the product listing page.", 1),
        ("product.page.subtitle", "product", "hero", "Page Subtitle",
            "เครื่องมือวัดและอุปกรณ์วิทยาศาสตร์คุณภาพสูง", "Precision measuring instruments and scientific equipment",
            "Default subtitle on the product listing page.", 2),
        ("product.search.placeholder", "product", "filter", "Search Placeholder",
            "ชื่อ, รุ่น, รหัส...", "Name, model, SKU...", "Placeholder text for the product search box.", 3),
        ("product.categories.title", "product", "filter", "Categories Title",
            "หมวดหมู่", "Categories", "Heading above the category filter list.", 4),
        ("product.all_products", "product", "filter", "All Products Label",
            "สินค้าทั้งหมด", "All Products", "Label for the 'all products' filter option.", 5),
        ("product.view_detail", "product", "card", "View Detail Button",
            "รายละเอียด", "Details", "Button label to view a product's detail page.", 6),
        ("product.add_to_quote", "product", "detail", "Add to Quote Button",
            "เพิ่มในรายการขอราคา", "Add to Quote", "Button label to add a product to the quotation cart.", 7),
        ("product.empty.title", "product", "empty", "Empty State Title",
            "ไม่พบสินค้า", "No products found", "Heading shown when no products match the current filter.", 8),
        ("product.empty.description", "product", "empty", "Empty State Description",
            "ลองค้นหาด้วยคำอื่น หรือเลือกหมวดหมู่อื่น", "Try a different search term or browse another category.",
            "Description shown when no products match the current filter.", 9),

        // ── Services ──────────────────────────────────────────────────────────
        ("services.page.title", "services", "hero", "Page Title",
            "บริการของเรา", "Our Services", "Default title on the services page (used when no Service Page setting is configured).", 1),
        ("services.page.subtitle", "services", "hero", "Page Subtitle",
            "บริการสนับสนุน สอบเทียบ ติดตั้ง และบำรุงรักษาเครื่องมืออย่างครบวงจร",
            "Comprehensive support, calibration, and maintenance for all your instruments.",
            "Default subtitle on the services page.", 2),
        ("services.rental.title", "services", "rental", "Rental Section Title",
            "บริการครบวงจรของเรา", "Our Complete Service Range", "Heading for the full services list section.", 3),
        ("services.rental.subtitle", "services", "rental", "Rental Section Subtitle",
            "เราพร้อมดูแลเครื่องมือวัดของคุณทุกขั้นตอน ตั้งแต่การติดตั้งจนถึงการสอบเทียบ",
            "We support your instruments at every stage — from installation to calibration and beyond.",
            "Subtitle for the full services list section.", 4),
        ("services.view_detail", "services", "card", "View Detail Button",
            "ดูรายละเอียด", "View Details", "Button label to view a service's detail page.", 5),

        // ── Activity ──────────────────────────────────────────────────────────
        ("activity.page.title", "activity", "hero", "Page Title",
            "กิจกรรมและข่าวสาร", "Activity", "Default title on the activity/news page.", 1),
        ("activity.page.subtitle", "activity", "hero", "Page Subtitle",
            "ข่าวสาร กิจกรรม บทความ และโปรโมชั่นล่าสุด", "Latest news, events, articles, and promotions.",
            "Default subtitle on the activity/news page.", 2),
        ("activity.read_more", "activity", "card", "Read More Button",
            "อ่านเพิ่มเติม", "Read More", "Button label to read a full activity/news article.", 3),
        ("activity.empty.title", "activity", "empty", "Empty State Title",
            "ไม่พบข้อมูลกิจกรรมหรือข่าวสาร", "No activity found.", "Message shown when no activities match the current filter.", 4),

        // ── Contact ───────────────────────────────────────────────────────────
        ("contact.page.title", "contact", "hero", "Page Title",
            "ติดต่อเรา", "Contact Us", "Title on the contact page.", 1),
        ("contact.page.subtitle", "contact", "hero", "Page Subtitle",
            "ติดต่อทีมผู้เชี่ยวชาญของเรา เราพร้อมให้คำปรึกษาและช่วยเหลือคุณ",
            "Get in touch with our team of experts. We're here to help and advise you.",
            "Subtitle on the contact page.", 2),
        ("contact.form.title", "contact", "form", "Form Title",
            "ส่งข้อความถึงเรา", "Send Us a Message", "Heading above the contact form.", 3),
        ("contact.form.subtitle", "contact", "form", "Form Subtitle",
            "กรอกแบบฟอร์มด้านล่าง ทีมงานของเราจะติดต่อกลับโดยเร็ว",
            "Fill in the form below and our team will get back to you promptly.",
            "Subtitle above the contact form.", 4),
        ("contact.submit_button", "contact", "form", "Submit Button",
            "ส่งข้อความ", "Send Message", "Submit button label on the contact form.", 5),
        ("contact.success_message", "contact", "form", "Success Message",
            "ส่งข้อความเรียบร้อยแล้ว", "Your message has been sent successfully.",
            "Message shown after a contact form is submitted successfully.", 6),

        // ── Quotation ─────────────────────────────────────────────────────────
        ("quote.page.title", "quotation", "hero", "Page Title",
            "ขอใบเสนอราคา", "Request Quotation", "Title on the quotation request page.", 1),
        ("quote.page.subtitle", "quotation", "hero", "Page Subtitle",
            "กรอกข้อมูลด้านล่าง ทีมงานของเราจะติดต่อกลับโดยเร็ว",
            "Fill in the form below and our team will get back to you promptly.",
            "Subtitle on the quotation request page.", 2),
        ("quote.submit_button", "quotation", "form", "Submit Button",
            "ส่งคำขอใบเสนอราคา", "Submit Quotation Request", "Submit button label on the quotation form.", 3),
        ("quote.empty_cart", "quotation", "cart", "Empty Cart Message",
            "ยังไม่มีรายการขอใบเสนอราคา", "Your quotation cart is empty", "Message shown when the quotation cart has no items.", 4),

        // ── Footer ────────────────────────────────────────────────────────────
        ("footer.company.description", "footer", null, "Company Description",
            "ผู้นำด้านเครื่องมือวัด เครื่องมือวิทยาศาสตร์ และอุปกรณ์ตรวจสอบสิ่งแวดล้อม",
            "Leading supplier of measuring instruments, scientific equipment, and environmental monitoring solutions.",
            "Fallback company description shown in the footer (used when Footer Contact description is not set).", 1),
        ("footer.quick_links", "footer", null, "Quick Links Heading",
            "ลิงก์ด่วน", "Quick Links", "Heading for the footer quick links column.", 2),
        ("footer.contact", "footer", null, "Contact Heading",
            "ติดต่อเรา", "Contact Us", "Heading for the footer contact column.", 3),
        ("footer.copyright", "footer", null, "Copyright Text",
            "สงวนลิขสิทธิ์", "All rights reserved.", "Text shown after the copyright year and company name.", 4),

        // ── Common ────────────────────────────────────────────────────────────
        ("common.learn_more", "common", null, "Learn More",
            "เรียนรู้เพิ่มเติม", "Learn More", "Generic 'learn more' link/button label.", 1),
        ("common.view_all", "common", null, "View All",
            "ดูทั้งหมด", "View All", "Generic 'view all' link/button label.", 2),
        ("common.back", "common", null, "Back",
            "ย้อนกลับ", "Back", "Generic 'back' link/button label.", 3),
        ("common.search", "common", null, "Search",
            "ค้นหา", "Search", "Generic 'search' label/placeholder.", 4),
        ("common.no_data", "common", null, "No Data",
            "ไม่มีข้อมูล", "No data", "Generic empty-state label.", 5),
    ];

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existingKeys = (await db.SiteTexts.AsNoTracking()
            .Select(t => t.Key)
            .ToListAsync())
            .ToHashSet();

        var toAdd = Defaults
            .Where(d => !existingKeys.Contains(d.Key))
            .Select(d => new SiteText
            {
                Key          = d.Key,
                Page         = d.Page,
                Section      = d.Section,
                Label        = d.Label,
                TextTH       = d.TextTH,
                TextEN       = d.TextEN,
                Description  = d.Description,
                TextType     = "text",
                DisplayOrder = d.Order,
                IsActive     = true,
                IsDelete     = false,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy    = "Seeder",
            })
            .ToList();

        if (toAdd.Count > 0)
        {
            db.SiteTexts.AddRange(toAdd);
            await db.SaveChangesAsync();
        }
    }
}
