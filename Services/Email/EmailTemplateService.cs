using System.Text;
using Needis.Web.Models;

namespace Needis.Web.Services.Email;

public class EmailTemplateService : IEmailTemplateService
{
    // ── HTML escape ──────────────────────────────────────────────────────────

    private static string Esc(string? s) =>
        (s ?? string.Empty)
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;");

    // ── Shared layout ────────────────────────────────────────────────────────

    private static string Wrap(string headerLabel, string bodyHtml)
    {
        return
            "<!DOCTYPE html><html lang=\"en\"><head>" +
            "<meta charset=\"utf-8\">" +
            "<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">" +
            "</head>" +
            "<body style=\"margin:0;padding:0;background:#f3f5f9;font-family:Arial,Helvetica,sans-serif\">" +
            "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"background:#f3f5f9;padding:32px 16px\">" +
            "<tr><td align=\"center\">" +
            "<table width=\"600\" cellpadding=\"0\" cellspacing=\"0\" " +
            "style=\"background:#ffffff;border-radius:10px;overflow:hidden;box-shadow:0 2px 12px rgba(0,0,0,.08)\">" +
            // Header
            "<tr><td style=\"background:#2d4199;padding:24px 32px\">" +
            $"<span style=\"color:#fff;font-size:20px;font-weight:700\">{Esc(headerLabel)}</span>" +
            "</td></tr>" +
            // Body
            $"<tr><td style=\"padding:32px\">{bodyHtml}</td></tr>" +
            // Footer
            "<tr><td style=\"background:#f8f9fc;padding:16px 32px;border-top:1px solid #e5e7eb;" +
            "text-align:center;color:#9ca3af;font-size:12px\">" +
            "Neediss &mdash; Precision Instruments &amp; Scientific Equipment<br>" +
            "This is an automated message. Please do not reply directly to this email." +
            "</td></tr>" +
            "</table></td></tr></table></body></html>";
    }

    private static string Row(string label, string? value)
    {
        const string tdLabelStyle =
            "padding:8px 12px;font-size:13px;font-weight:600;color:#6b7280;" +
            "background:#f9fafb;border:1px solid #e5e7eb;width:38%;vertical-align:top";
        const string tdValueStyle =
            "padding:8px 12px;font-size:14px;color:#111827;" +
            "border:1px solid #e5e7eb;vertical-align:top";
        return
            $"<tr><td style=\"{tdLabelStyle}\">{Esc(label)}</td>" +
            $"<td style=\"{tdValueStyle}\">{Esc(value ?? "—")}</td></tr>";
    }

    private static string InfoTable(params (string Label, string? Value)[] rows)
    {
        var sb = new StringBuilder();
        sb.Append("<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" " +
                  "style=\"border-collapse:collapse;margin-bottom:24px\">");
        foreach (var (l, v) in rows) sb.Append(Row(l, v));
        sb.Append("</table>");
        return sb.ToString();
    }

    private static string Button(string href, string label)
    {
        const string style =
            "display:inline-block;background:#2d4199;color:#ffffff;text-decoration:none;" +
            "padding:12px 28px;border-radius:6px;font-size:14px;font-weight:700";
        return $"<div style=\"margin:24px 0 0\">" +
               $"<a href=\"{Esc(href)}\" style=\"{style}\">{Esc(label)}</a>" +
               "</div>";
    }

    private static string ItemsTable(List<QuotationRequestItem> items, bool useTh)
    {
        if (items.Count == 0) return string.Empty;

        var sb = new StringBuilder();
        sb.Append("<p style=\"font-weight:700;color:#1a2b6b;font-size:15px;margin:0 0 8px\">Requested Items</p>");
        sb.Append("<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" " +
                  "style=\"border-collapse:collapse;margin-bottom:24px;font-size:13px\">");
        sb.Append("<thead><tr style=\"background:#2d4199;color:#fff\">");
        foreach (var h in new[] { "#", "Type", "Name", "Details", "Qty", "Note" })
            sb.Append($"<th style=\"padding:8px 10px;text-align:left\">{h}</th>");
        sb.Append("</tr></thead><tbody>");

        for (int i = 0; i < items.Count; i++)
        {
            var item   = items[i];
            var bg     = i % 2 == 0 ? "#ffffff" : "#f9fafb";
            const string td = "padding:7px 10px;border-bottom:1px solid #e5e7eb;";

            bool isService = item.ItemType == "Service";

            var name = isService
                ? (useTh
                    ? (item.ServiceNameSnapshotTH ?? item.ServiceNameSnapshotEN ?? "—")
                    : (item.ServiceNameSnapshotEN ?? item.ServiceNameSnapshotTH ?? "—"))
                : (useTh
                    ? (item.ProductNameSnapshotTH ?? item.ProductNameSnapshotEN ?? "—")
                    : (item.ProductNameSnapshotEN ?? item.ProductNameSnapshotTH ?? "—"));

            var details = isService
                ? (item.ServiceCodeSnapshot ?? "—")
                : string.Join(" · ", new[] { item.BrandSnapshot, item.ModelSnapshot }
                    .Where(x => !string.IsNullOrEmpty(x)));

            var typeBadge = isService
                ? "<span style=\"background:#6f42c1;color:#fff;border-radius:4px;padding:2px 6px;font-size:11px\">Service</span>"
                : "<span style=\"background:#2d4199;color:#fff;border-radius:4px;padding:2px 6px;font-size:11px\">Product</span>";

            sb.Append($"<tr style=\"background:{bg}\">");
            sb.Append($"<td style=\"{td}color:#6b7280\">{i + 1}</td>");
            sb.Append($"<td style=\"{td}\">{typeBadge}</td>");
            sb.Append($"<td style=\"{td}font-weight:600;color:#111827\">{Esc(name)}</td>");
            sb.Append($"<td style=\"{td}font-family:monospace;color:#374151;font-size:12px\">{Esc(details.Length > 0 ? details : "—")}</td>");
            sb.Append($"<td style=\"{td}text-align:center;font-weight:700;color:#2d4199\">{item.Quantity}</td>");
            sb.Append($"<td style=\"{td}color:#6b7280\">{Esc(item.ItemNote ?? "—")}</td>");
            sb.Append("</tr>");
        }

        sb.Append("</tbody></table>");
        return sb.ToString();
    }

    // ── Template 1: Admin notification ──────────────────────────────────────

    public string BuildQuotationAdminNotificationEmail(
        QuotationRequest request,
        List<QuotationRequestItem> items,
        string adminUrl)
    {
        var sb = new StringBuilder();

        sb.Append("<p style=\"color:#374151;font-size:15px;margin:0 0 20px\">" +
                  "A new quotation request has been received and is awaiting your response.</p>");

        sb.Append("<div style=\"background:#eef2ff;border-left:4px solid #2d4199;padding:12px 16px;" +
                  "border-radius:0 6px 6px 0;margin-bottom:24px\">" +
                  "<span style=\"font-size:13px;color:#6b7280\">Request No</span><br>" +
                  $"<span style=\"font-size:22px;font-weight:700;color:#2d4199;letter-spacing:.04em\">" +
                  $"{Esc(request.RequestNo)}</span></div>");

        sb.Append(InfoTable(
            ("Submitted At",  request.CreatedAt.ToLocalTime().ToString("dd MMM yyyy HH:mm")),
            ("Request Type",  request.RequestType ?? "General"),
            ("Language",      request.Language?.ToUpperInvariant() ?? "—")));

        sb.Append("<p style=\"font-weight:700;color:#1a2b6b;font-size:15px;margin:0 0 8px\">Customer Information</p>");
        sb.Append(InfoTable(
            ("Full Name",         request.CustomerName),
            ("Company",           request.CompanyName),
            ("Email",             request.Email),
            ("Phone",             request.Phone),
            ("Preferred Contact", request.PreferredContactMethod)));

        sb.Append("<p style=\"font-weight:700;color:#1a2b6b;font-size:15px;margin:0 0 8px\">Request Details</p>");
        sb.Append(InfoTable(("Subject", request.Subject)));

        if (!string.IsNullOrWhiteSpace(request.Message))
        {
            sb.Append("<p style=\"font-weight:600;color:#6b7280;font-size:13px;margin:0 0 6px\">Message</p>");
            sb.Append("<div style=\"background:#f9fafb;border:1px solid #e5e7eb;border-radius:6px;" +
                      "padding:12px 16px;font-size:14px;color:#374151;" +
                      $"white-space:pre-wrap;margin-bottom:24px\">{Esc(request.Message)}</div>");
        }

        sb.Append(ItemsTable(items, useTh: false));
        sb.Append(Button(adminUrl, "View in Admin Panel →"));

        return Wrap("Neediss — New Quotation Request", sb.ToString());
    }

    // ── Template 2: Customer auto-reply ─────────────────────────────────────

    public string BuildQuotationCustomerAutoReplyEmail(
        QuotationRequest request,
        List<QuotationRequestItem> items,
        string currentLanguage)
    {
        bool isTh = currentLanguage == "th";

        string greeting = isTh
            ? $"เรียน คุณ{Esc(request.CustomerName)}"
            : $"Dear {Esc(request.CustomerName)},";

        string intro = isTh
            ? "ขอบคุณสำหรับคำขอใบเสนอราคา ระบบได้รับข้อมูลเรียบร้อยแล้ว " +
              "ทีมงานของเราจะตรวจสอบและติดต่อกลับโดยเร็วที่สุด"
            : "Thank you for your quotation request. We have successfully received your inquiry " +
              "and our team will review it and contact you as soon as possible.";

        string requestLabel  = isTh ? "หมายเลขคำขอ" : "Request Number";
        string subjectLabel  = isTh ? "หัวข้อ" : "Subject";
        string whatNextTitle = isTh ? "ขั้นตอนต่อไป" : "What Happens Next";
        string whatNextText  = isTh
            ? "ทีมผู้เชี่ยวชาญของเราจะตรวจสอบคำขอและเตรียมใบเสนอราคา " +
              "ปกติแล้วเราจะติดต่อกลับภายใน 1 วันทำการ"
            : "Our expert team will review your request and prepare a competitive quotation. " +
              "We typically respond within 1 business day.";
        string keepRef = isTh
            ? "กรุณาจดหมายเลขคำขอนี้ไว้สำหรับอ้างอิงในการติดต่อครั้งต่อไป"
            : "Please keep this request number for your reference in any future correspondence.";

        var sb = new StringBuilder();

        sb.Append($"<p style=\"color:#374151;font-size:15px;margin:0 0 16px\">{greeting}</p>");
        sb.Append($"<p style=\"color:#374151;font-size:14px;line-height:1.7;margin:0 0 24px\">{intro}</p>");

        sb.Append("<div style=\"background:#eef2ff;border-left:4px solid #2d4199;padding:12px 16px;" +
                  "border-radius:0 6px 6px 0;margin-bottom:24px\">" +
                  $"<span style=\"font-size:13px;color:#6b7280\">{requestLabel}</span><br>" +
                  $"<span style=\"font-size:22px;font-weight:700;color:#2d4199;letter-spacing:.04em\">" +
                  $"{Esc(request.RequestNo)}</span></div>");

        sb.Append(InfoTable((subjectLabel, request.Subject)));
        sb.Append(ItemsTable(items, useTh: isTh));

        sb.Append("<div style=\"background:#f0fdf4;border:1px solid #bbf7d0;border-radius:6px;" +
                  "padding:16px;margin-bottom:20px\">" +
                  $"<p style=\"font-weight:700;color:#15803d;font-size:14px;margin:0 0 8px\">" +
                  $"✓ {whatNextTitle}</p>" +
                  $"<p style=\"color:#374151;font-size:13px;line-height:1.7;margin:0\">{whatNextText}</p>" +
                  "</div>");

        sb.Append($"<p style=\"color:#6b7280;font-size:12px;margin:0\">{keepRef}</p>");

        string headerLabel = isTh
            ? "Neediss — ยืนยันการรับคำขอใบเสนอราคา"
            : "Neediss — Quotation Request Received";

        return Wrap(headerLabel, sb.ToString());
    }

    // ── Template 3: Contact message notification ─────────────────────────────

    public string BuildContactMessageNotificationEmail(ContactMessage message, string? adminDetailUrl = null)
    {
        var sb = new StringBuilder();

        sb.Append("<p style=\"color:#374151;font-size:15px;margin:0 0 20px\">" +
                  "A new contact message has been received through the website contact form.</p>");

        sb.Append("<p style=\"font-weight:700;color:#1a2b6b;font-size:15px;margin:0 0 8px\">Sender Information</p>");
        sb.Append(InfoTable(
            ("Full Name", message.FullName),
            ("Email",     message.Email),
            ("Phone",     message.Phone)));

        sb.Append(InfoTable(("Subject", message.Subject)));

        sb.Append("<p style=\"font-weight:600;color:#6b7280;font-size:13px;margin:0 0 6px\">Message</p>");
        sb.Append("<div style=\"background:#f9fafb;border:1px solid #e5e7eb;border-radius:6px;" +
                  "padding:12px 16px;font-size:14px;color:#374151;" +
                  $"white-space:pre-wrap;margin-bottom:16px\">{Esc(message.Message)}</div>");

        sb.Append($"<p style=\"color:#9ca3af;font-size:12px;margin:0 0 12px\">" +
                  $"Received: {message.CreatedAt.ToLocalTime():dd MMM yyyy HH:mm}</p>");

        if (!string.IsNullOrWhiteSpace(adminDetailUrl))
        {
            sb.Append($"<p style=\"margin:0\"><a href=\"{Esc(adminDetailUrl)}\" " +
                      "style=\"display:inline-block;background:#2d4199;color:#fff;padding:8px 20px;" +
                      "border-radius:6px;text-decoration:none;font-size:14px;font-weight:600\">" +
                      "View in Admin Panel</a></p>");
        }

        return Wrap("Neediss — New Contact Message", sb.ToString());
    }
}
