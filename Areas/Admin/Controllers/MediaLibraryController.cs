using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Services.Media;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("Media.View")]
public class MediaLibraryController : Controller
{
    private readonly AppDbContext _db;
    private readonly IMediaFileService _mediaService;
    private readonly IWebHostEnvironment _env;

    public MediaLibraryController(AppDbContext db, IMediaFileService mediaService, IWebHostEnvironment env)
    {
        _db           = db;
        _mediaService = mediaService;
        _env          = env;
    }

    // ── GET Index ────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(
        string? keyword,
        string? fileType,
        string? usageType,
        string? relatedModule)
    {
        ViewData["Title"] = "Media Library";

        var query = _db.MediaFiles
            .AsNoTracking()
            .Where(m => !m.IsDelete)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(m =>
                m.OriginalFileName.Contains(kw) ||
                m.FileName.Contains(kw) ||
                (m.TitleTH    != null && m.TitleTH.Contains(kw)) ||
                (m.TitleEN    != null && m.TitleEN.Contains(kw)) ||
                (m.AltTextTH  != null && m.AltTextTH.Contains(kw)) ||
                (m.AltTextEN  != null && m.AltTextEN.Contains(kw)) ||
                (m.CaptionTH  != null && m.CaptionTH.Contains(kw)) ||
                (m.CaptionEN  != null && m.CaptionEN.Contains(kw)));
        }

        if (!string.IsNullOrWhiteSpace(fileType))
            query = query.Where(m => m.FileType == fileType);

        if (!string.IsNullOrWhiteSpace(usageType))
            query = query.Where(m => m.UsageType == usageType);

        if (!string.IsNullOrWhiteSpace(relatedModule))
            query = query.Where(m => m.RelatedModule == relatedModule);

        var files = await query
            .OrderByDescending(m => m.CreatedAt)
            .Take(300)
            .ToListAsync();

        // Summary counts (unfiltered)
        var totalCount   = await _db.MediaFiles.AsNoTracking().CountAsync(m => !m.IsDelete);
        var imageCount   = await _db.MediaFiles.AsNoTracking().CountAsync(m => !m.IsDelete && m.FileType == "Image");
        var docCount     = await _db.MediaFiles.AsNoTracking().CountAsync(m => !m.IsDelete && m.FileType == "Document");
        var pdfCount     = await _db.MediaFiles.AsNoTracking().CountAsync(m => !m.IsDelete && m.FileType == "Pdf");
        var totalStorage = await _db.MediaFiles.AsNoTracking().Where(m => !m.IsDelete).SumAsync(m => m.FileSize);

        var vm = new MediaLibraryIndexViewModel
        {
            Keyword       = keyword,
            FileType      = fileType,
            UsageType     = usageType,
            RelatedModule = relatedModule,
            MediaFiles    = files,
            TotalCount    = totalCount,
            ImageCount    = imageCount,
            DocumentCount = docCount,
            PdfCount      = pdfCount,
            TotalStorageBytes = totalStorage,
        };

        return View(vm);
    }

    // ── GET Upload ───────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Media.Upload")]
    public IActionResult Upload()
    {
        ViewData["Title"] = "Upload File";
        return View(new MediaUploadViewModel { IsPublic = true, IsActive = true });
    }

    // ── POST Upload ──────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Media.Upload")]
    public async Task<IActionResult> Upload(MediaUploadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Upload File";
            return View(model);
        }

        if (model.File is null || model.File.Length == 0)
        {
            ModelState.AddModelError(nameof(model.File), "Please select a file.");
            ViewData["Title"] = "Upload File";
            return View(model);
        }

        if (!_mediaService.IsAllowedFile(model.File, out var fileError))
        {
            ModelState.AddModelError(nameof(model.File), fileError);
            ViewData["Title"] = "Upload File";
            return View(model);
        }

        try
        {
            var uploader = User.FindFirst(ClaimTypes.Name)?.Value ?? "admin";
            var media    = await _mediaService.UploadAsync(
                model.File,
                usageType:       model.UsageType     ?? "General",
                relatedModule:   model.RelatedModule,
                relatedEntityId: model.RelatedEntityId,
                folder:          model.Folder,
                uploadedBy:      uploader);

            // Apply metadata immediately after upload
            await _mediaService.UpdateMetadataAsync(
                media.Id,
                model.TitleTH, model.TitleEN,
                model.AltTextTH, model.AltTextEN,
                model.CaptionTH, model.CaptionEN,
                model.DescriptionTH, model.DescriptionEN,
                model.IsPublic, model.IsActive,
                uploader);

            TempData["Success"] = $"File '{media.OriginalFileName}' uploaded successfully.";
            return RedirectToAction(nameof(Details), new { id = media.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Upload failed: {ex.Message}");
            ViewData["Title"] = "Upload File";
            return View(model);
        }
    }

    // ── GET Details ──────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var media = await _db.MediaFiles.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDelete);

        if (media is null) return NotFound();

        ViewData["Title"] = media.OriginalFileName;
        return View(media);
    }

    // ── GET Edit ─────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Media.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var media = await _db.MediaFiles.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDelete);

        if (media is null) return NotFound();

        ViewData["Title"] = $"Edit — {media.OriginalFileName}";
        return View(MapToEditViewModel(media));
    }

    // ── POST Edit ────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Media.Edit")]
    public async Task<IActionResult> Edit(int id, MediaEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Edit Media";
            return View(model);
        }

        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "admin";
        var ok = await _mediaService.UpdateMetadataAsync(
            id,
            model.TitleTH, model.TitleEN,
            model.AltTextTH, model.AltTextEN,
            model.CaptionTH, model.CaptionEN,
            model.DescriptionTH, model.DescriptionEN,
            model.IsPublic, model.IsActive,
            updatedBy);

        if (!ok) return NotFound();

        TempData["Success"] = "Metadata updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ── POST Delete ──────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Media.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "admin";
        var ok = await _mediaService.SoftDeleteAsync(id, updatedBy);

        TempData[ok ? "Success" : "Error"] = ok
            ? "File deleted from Media Library."
            : "File not found.";

        return RedirectToAction(nameof(Index));
    }

    // ── GET Picker ───────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Picker(string? fileType, string? usageType)
    {
        ViewData["Title"] = "Media Picker";

        var query = _db.MediaFiles
            .AsNoTracking()
            .Where(m => !m.IsDelete && m.IsActive);

        if (!string.IsNullOrWhiteSpace(fileType))
            query = query.Where(m => m.FileType == fileType);

        if (!string.IsNullOrWhiteSpace(usageType))
            query = query.Where(m => m.UsageType == usageType);

        var files = await query
            .OrderByDescending(m => m.CreatedAt)
            .Take(200)
            .ToListAsync();

        ViewData["PickerFileType"]  = fileType;
        ViewData["PickerUsageType"] = usageType;
        return View(files);
    }

    // ── GET Download ─────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Download(int id)
    {
        var media = await _db.MediaFiles.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDelete);

        if (media is null) return NotFound();

        // Build safe physical path from the relative URL — never from user input
        var relPath  = media.FileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var physPath = Path.Combine(_env.WebRootPath, relPath);

        if (!physPath.StartsWith(_env.WebRootPath, StringComparison.OrdinalIgnoreCase))
            return BadRequest("Invalid path.");

        if (!System.IO.File.Exists(physPath))
            return NotFound("Physical file not found.");

        return PhysicalFile(physPath, media.ContentType, media.OriginalFileName);
    }

    // ── GET Find (JSON picker API) ───────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Find(string? keyword, string? fileType)
    {
        var query = _db.MediaFiles
            .AsNoTracking()
            .Where(m => !m.IsDelete && m.IsActive);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(m =>
                m.OriginalFileName.Contains(kw) ||
                (m.TitleEN != null && m.TitleEN.Contains(kw)) ||
                (m.TitleTH != null && m.TitleTH.Contains(kw)));
        }

        if (!string.IsNullOrWhiteSpace(fileType))
            query = query.Where(m => m.FileType == fileType);

        var results = await query
            .OrderByDescending(m => m.CreatedAt)
            .Take(50)
            .Select(m => new
            {
                m.Id,
                m.OriginalFileName,
                m.FileUrl,
                m.FileType,
                m.FileExtension,
                FileSizeKb = m.FileSize / 1024,
            })
            .ToListAsync();

        return Json(results);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static MediaEditViewModel MapToEditViewModel(Models.MediaFile m) => new()
    {
        Id               = m.Id,
        FileName         = m.FileName,
        OriginalFileName = m.OriginalFileName,
        FileUrl          = m.FileUrl,
        FileType         = m.FileType,
        ContentType      = m.ContentType,
        FileExtension    = m.FileExtension,
        FileSize         = m.FileSize,
        UsageType        = m.UsageType,
        RelatedModule    = m.RelatedModule,
        RelatedEntityId  = m.RelatedEntityId,
        TitleTH          = m.TitleTH,
        TitleEN          = m.TitleEN,
        AltTextTH        = m.AltTextTH,
        AltTextEN        = m.AltTextEN,
        CaptionTH        = m.CaptionTH,
        CaptionEN        = m.CaptionEN,
        DescriptionTH    = m.DescriptionTH,
        DescriptionEN    = m.DescriptionEN,
        IsPublic         = m.IsPublic,
        IsActive         = m.IsActive,
    };
}
