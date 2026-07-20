namespace Needis.Web.Services.Content;

public interface IRichContentService
{
    // Converts admin-entered content (WYSIWYG-produced HTML, legacy Markdown, or
    // plain text) into sanitized, safe-to-render HTML. Never returns raw script/style
    // markup or unsafe link schemes. Empty list items/paragraphs left behind by the
    // WYSIWYG editor or legacy data are stripped.
    string ToSafeHtml(string? content);

    // Converts the same admin-entered content into flat, tag-free text for cards and
    // summaries. Never returns HTML markup, even if the source content is HTML.
    string ToPlainText(string? content, int? maxLength = null);
}
