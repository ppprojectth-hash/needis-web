using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Needis.Web.Controllers;

public class RobotsController : Controller
{
    [HttpGet("/robots.txt")]
    public IActionResult Index()
    {
        var baseUrl  = $"{Request.Scheme}://{Request.Host}";
        var content  = new StringBuilder();

        content.AppendLine("User-agent: *");
        content.AppendLine("Disallow: /Admin");
        content.AppendLine("Disallow: /Admin/");
        content.AppendLine("Disallow: /Account");
        content.AppendLine("Disallow: /Quotation/ThankYou");
        content.AppendLine("Allow: /");
        content.AppendLine();
        content.AppendLine($"Sitemap: {baseUrl}/sitemap.xml");

        return Content(content.ToString(), "text/plain", Encoding.UTF8);
    }
}
