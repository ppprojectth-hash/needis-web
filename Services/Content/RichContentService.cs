using System.Text.RegularExpressions;
using Ganss.Xss;

namespace Needis.Web.Services.Content;

// Bridges the old Markdown/plain-text admin fields and the new WYSIWYG editor:
// content saved by the WYSIWYG editor is already HTML, content saved before this
// change is Markdown or plain text. Either way, ToSafeHtml() is the single place
// that decides how to render it and guarantees the output is sanitized.
public class RichContentService : IRichContentService
{
    private readonly IMarkdownService _markdown;

    // Cheap heuristic: WYSIWYG output always contains a real tag (<p>, <strong>, <ul>...);
    // legacy Markdown/plain text essentially never does (a stray "<" in plain text is rare
    // and, worst case, just gets routed through the Markdown path where it's HTML-escaped).
    private static readonly Regex LooksLikeHtml = new(
        @"<(p|br|strong|b|em|i|ul|ol|li|h[234]|a|div|span)\b[^>]*>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly HtmlSanitizer Sanitizer = BuildSanitizer();

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

        return Sanitizer.Sanitize(html);
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
