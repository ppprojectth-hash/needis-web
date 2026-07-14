namespace Needis.Web.Services.Content;

public interface IRichContentService
{
    // Converts admin-entered content (WYSIWYG-produced HTML, legacy Markdown, or
    // plain text) into sanitized, safe-to-render HTML. Never returns raw script/style
    // markup or unsafe link schemes.
    string ToSafeHtml(string? content);
}
