using System.Net;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using Ganss.Xss;

namespace Needis.Web.Services.Content;

// Bridges the old Markdown/plain-text admin fields and the new WYSIWYG editor:
// content saved by the WYSIWYG editor is already HTML, content saved before this
// change is Markdown or plain text. Either way, ToSafeHtml() is the single place
// that decides how to render it and guarantees the output is sanitized and free of
// the empty <li>/<p> junk the WYSIWYG editor's bullet button can leave behind.
public class RichContentService : IRichContentService
{
    private readonly IMarkdownService _markdown;

    // Cheap heuristic: WYSIWYG output always contains a real tag (<p>, <strong>, <ul>...);
    // legacy Markdown/plain text essentially never does (a stray "<" in plain text is rare
    // and, worst case, just gets routed through the Markdown path where it's HTML-escaped).
    private static readonly Regex LooksLikeHtml = new(
        @"<(p|br|strong|b|em|i|ul|ol|li|h[234]|a|div|span)\b[^>]*>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Block-level boundaries in the sanitized/allowed tag set: when flattening to plain
    // text, these must become a space so adjacent words/list items don't run together.
    private static readonly Regex BlockBreakPattern = new(
        @"</(p|li|h2|h3|h4)>|<br\s*/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex TagPattern = new("<[^>]+>", RegexOptions.Compiled);
    private static readonly Regex WhitespacePattern = new(@"\s+", RegexOptions.Compiled);

    private const char NonBreakingSpace = ' ';

    private static readonly HtmlSanitizer Sanitizer = BuildSanitizer();
    private static readonly HtmlParser Parser = new();

    public RichContentService(IMarkdownService markdown)
    {
        _markdown = markdown;
    }

    public string ToSafeHtml(string? content)
    {
        if (string.IsNullOrWhiteSpace(content)) return string.Empty;

        var html = LooksLikeHtml.IsMatch(content)
            ? content
            : _markdown.ToSafeHtml(content); // legacy Markdown/plain text -> HTML first

        var sanitized = Sanitizer.Sanitize(html);
        return RemoveEmptyElements(sanitized);
    }

    public string ToPlainText(string? content, int? maxLength = null)
    {
        if (string.IsNullOrWhiteSpace(content)) return string.Empty;

        var safeHtml = ToSafeHtml(content);
        if (safeHtml.Length == 0) return string.Empty;

        var withBreaks = BlockBreakPattern.Replace(safeHtml, " ");
        var stripped = TagPattern.Replace(withBreaks, string.Empty);
        var decoded = WebUtility.HtmlDecode(stripped);
        var collapsed = WhitespacePattern.Replace(decoded, " ").Trim();

        if (maxLength.HasValue && collapsed.Length > maxLength.Value)
            collapsed = collapsed[..maxLength.Value].TrimEnd() + "…";

        return collapsed;
    }

    // Strips leftovers the WYSIWYG editor's bullet/numbered-list buttons can produce
    // (<li><br></li>, <li>&nbsp;</li>, empty <ul>/<ol>/<p>) so old and newly-saved bad
    // data never renders as visible empty bullet rows or blank paragraphs.
    private static string RemoveEmptyElements(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;

        var document = Parser.ParseDocument($"<body>{html}</body>");
        var body = document.Body;
        if (body is null) return html.Trim();

        bool changed;
        do
        {
            changed = false;

            foreach (var li in body.QuerySelectorAll("li").ToArray())
            {
                var text = li.TextContent.Replace(NonBreakingSpace, ' ').Trim();
                if (text.Length == 0 && li.QuerySelector("a,strong,b,em,i") is null)
                {
                    li.Remove();
                    changed = true;
                }
            }

            foreach (var list in body.QuerySelectorAll("ul,ol").ToArray())
            {
                if (list.QuerySelector("li") is null)
                {
                    list.Remove();
                    changed = true;
                }
            }

            foreach (var p in body.QuerySelectorAll("p").ToArray())
            {
                var text = p.TextContent.Replace(NonBreakingSpace, ' ').Trim();
                if (text.Length == 0 && p.QuerySelector("a,strong,b,em,i,ul,ol") is null)
                {
                    p.Remove();
                    changed = true;
                }
            }
        } while (changed);

        return body.InnerHtml.Trim();
    }

    private static HtmlSanitizer BuildSanitizer()
    {
        var sanitizer = new HtmlSanitizer();

        sanitizer.AllowedTags.Clear();
        foreach (var tag in new[] { "p", "br", "strong", "b", "em", "i", "ul", "ol", "li", "h2", "h3", "h4", "a" })
            sanitizer.AllowedTags.Add(tag);

        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedAttributes.Add("href");

        sanitizer.AllowedCssProperties.Clear();
        sanitizer.AllowedAtRules.Clear();

        sanitizer.AllowedSchemes.Clear();
        foreach (var scheme in new[] { "http", "https", "mailto", "tel" })
            sanitizer.AllowedSchemes.Add(scheme);

        sanitizer.AllowDataAttributes = false;

        // Anchors always get a safe rel/target so sanitized links can't be used for
        // tabnabbing even though we don't allow target= to be set by admin input.
        sanitizer.PostProcessNode += (_, args) =>
        {
            if (args.Node is AngleSharp.Dom.IElement el && el.NodeName.Equals("A", StringComparison.OrdinalIgnoreCase))
            {
                el.SetAttribute("rel", "noopener noreferrer");
            }
        };

        return sanitizer;
    }
}
