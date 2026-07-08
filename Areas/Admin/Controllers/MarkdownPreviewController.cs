using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Needis.Web.Services.Content;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class MarkdownPreviewController : Controller
{
    private readonly IMarkdownService _markdown;

    public MarkdownPreviewController(IMarkdownService markdown)
    {
        _markdown = markdown;
    }

    // POST /Admin/MarkdownPreview/Preview
    // Renders admin-submitted Markdown through the same safe pipeline used on public
    // pages, so the preview always matches what visitors will actually see.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Preview([FromForm] string? markdown)
    {
        var safeHtml = _markdown.ToSafeHtml(markdown);
        return Json(new { safeHtml });
    }
}
