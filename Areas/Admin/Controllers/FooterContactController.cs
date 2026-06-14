using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("SiteSetting.View")]
public class FooterContactController : Controller
{
    private readonly AppDbContext _db;

    public FooterContactController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var fc = await _db.FooterContacts.AsNoTracking().FirstOrDefaultAsync();

        if (fc is null)
        {
            fc = new FooterContact { IsActive = true, UpdatedAt = DateTime.UtcNow };
            _db.FooterContacts.Add(fc);
            await _db.SaveChangesAsync();
        }

        return View(MapToViewModel(fc));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(FooterContactViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var fc = await _db.FooterContacts.FirstOrDefaultAsync();
        if (fc is null)
        {
            fc = new FooterContact();
            _db.FooterContacts.Add(fc);
        }

        fc.DescriptionTH = vm.CompanyDescriptionTH;
        fc.DescriptionEN = vm.CompanyDescriptionEN;
        fc.AddressTH     = vm.AddressTH;
        fc.AddressEN     = vm.AddressEN;
        fc.Phone         = vm.Phone;
        fc.Email         = vm.Email;
        fc.FacebookUrl   = vm.FacebookUrl;
        fc.LineUrl       = vm.LineUrl;
        fc.LinkedInUrl   = vm.LinkedInUrl;
        fc.IsActive      = vm.IsActive;
        fc.UpdatedAt     = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Footer contact updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    private static FooterContactViewModel MapToViewModel(FooterContact fc) => new()
    {
        FooterContactId      = fc.Id,
        CompanyDescriptionTH = fc.DescriptionTH,
        CompanyDescriptionEN = fc.DescriptionEN,
        AddressTH            = fc.AddressTH,
        AddressEN            = fc.AddressEN,
        Phone                = fc.Phone,
        Email                = fc.Email,
        FacebookUrl          = fc.FacebookUrl,
        LineUrl              = fc.LineUrl,
        LinkedInUrl          = fc.LinkedInUrl,
        IsActive             = fc.IsActive,
    };
}
