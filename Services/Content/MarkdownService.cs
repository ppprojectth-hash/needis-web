using System.Text.RegularExpressions;
using Markdig;

namespace Needis.Web.Services.Content;

public class MarkdownService : IMarkdownService
{
    // DisableHtml() escapes raw HTML/script tags instead of passing them through,
    // so admin-entered content can never inject executable markup.
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseEmphasisExtras()
        .UseAutoLinks()
        .UseSoftlineBreakAsHardlineBreak()
        .DisableHtml()
        .Build();

    // Defense in depth: strip javascript:/data:/vbscript: link targets even though
    // DisableHtml already blocks raw <a onclick=...> style markup.
    private static readonly Regex UnsafeHrefPattern = new(
        "href\\s*=\\s*\"(javascript:|data:|vbscript:)[^\"]*\"",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Legacy plain-text descriptions sometimes use ">>" as a pseudo-bullet convention.
    // Escape leading ">" so it renders as literal text instead of a Markdown blockquote.
    private static readonly Regex LeadingBlockquotePattern = new(
        @"^(\s*)>", RegexOptions.Multiline | RegexOptions.Compiled);

    public string ToSafeHtml(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown)) return string.Empty;

        var escaped = LeadingBlockquotePattern.Replace(markdown, "$1\\>");
        var html = Markdown.ToHtml(escaped, Pipeline);
        return UnsafeHrefPattern.Replace(html, "href=\"#\"");
    }
}
