namespace Needis.Web.Services.Content;

public interface IMarkdownService
{
    // Converts Markdown to safe HTML. Raw HTML/script input is escaped, never executed.
    string ToSafeHtml(string? markdown);
}
