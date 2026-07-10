using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class BannerController : Controller
{
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp", ".svg"];
    private static readonly string[] AllowedVideoExtensions = [".mp4", ".webm", ".mov"];
    private const long MaxImageSize = 5  * 1024 * 1024;  // 5 MB
    private const long MaxVideoSize = 50 * 1024 * 1024;  // 50 MB

    private readonly AppDbContext        _db;
    private readonly IWebHostEnvironment _env;

    public BannerController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    // ── Index ───────────────────────────────────────────────────────────────

    [RequirePermission("Banner.View")]
    public async Task<IActionResult> Index()
    {
        var banners = await _db.HomeBanners
            .AsNoTracking()
            .OrderBy(b => b.DisplayOrder)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync();

        return View(banners);
    }

    // ── Create ──────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Banner.Create")]
    public IActionResult Create() => View(new HomeBannerViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Banner.Create")]
    public async Task<IActionResult> Create(HomeBannerViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var mediaType = string.IsNullOrWhiteSpace(vm.MediaType) ? "Image" : vm.MediaType.Trim();

        var banner = new HomeBanner
        {
            TitleTH             = vm.TitleTH      ?? string.Empty,
            TitleEN             = vm.TitleEN       ?? string.Empty,
            SubtitleTH          = vm.SubtitleTH,
            SubtitleEN          = vm.SubtitleEN,
            ButtonTextTH        = vm.ButtonTextTH,
            ButtonTextEN        = vm.ButtonTextEN,
            ButtonUrl           = vm.ButtonUrl,
            MediaType           = mediaType,
            // The DB has a single VideoUrl column shared by the Video and YouTube
            // media types; which ViewModel field feeds it depends on MediaType.
            VideoUrl            = mediaType == "YouTube" ? vm.YoutubeUrl?.Trim() : vm.VideoUrl?.Trim(),
            IsAutoplay          = vm.IsAutoplay,
            IsMuted             = vm.IsMuted,
            IsLoop              = vm.IsLoop,
            ShowControls        = vm.ShowControls,
            SlideDurationSeconds = vm.SlideDurationSeconds,
            OverlayStyle        = vm.OverlayStyle,
            TextPosition        = vm.TextPosition,
            StartDate           = ToUtc(vm.StartDate),
            EndDate             = ToUtc(vm.EndDate),
            DisplayOrder        = vm.DisplayOrder,
            IsActive            = vm.IsActive,
            CreatedAt           = DateTime.UtcNow,
            UpdatedAt           = DateTime.UtcNow,
        };

        // A stale file selection left in a hidden Image/Video tab must never attach
        // itself to a YouTube banner, so file uploads are skipped entirely for that type.
        if (mediaType != "YouTube")
        {
            // Image upload
            if (vm.ImageFile is { Length: > 0 })
            {
                var (ok, err, path) = await SaveImageAsync(vm.ImageFile);
                if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
                banner.ImagePath = path;
            }

            // Mobile image upload
            if (vm.MobileImageFile is { Length: > 0 })
            {
                var (ok, err, path) = await SaveMobileImageAsync(vm.MobileImageFile);
                if (!ok) { ModelState.AddModelError(nameof(vm.MobileImageFile), err); return View(vm); }
                banner.MobileImageUrl = path;
            }

            // Video file upload
            if (vm.VideoFile is { Length: > 0 })
            {
                var (ok, err, path) = await SaveVideoAsync(vm.VideoFile);
                if (!ok) { ModelState.AddModelError(nameof(vm.VideoFile), err); return View(vm); }
                banner.VideoFileUrl = path;
            }

            // Mobile video file upload
            if (vm.MobileVideoFile is { Length: > 0 })
            {
                var (ok, err, path) = await SaveVideoAsync(vm.MobileVideoFile, "mobile");
                if (!ok) { ModelState.AddModelError(nameof(vm.MobileVideoFile), err); return View(vm); }
                banner.MobileVideoUrl = path;
            }
        }

        _db.HomeBanners.Add(banner);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Banner created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit ────────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Banner.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var banner = await _db.HomeBanners.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        if (banner is null) return NotFound();
        return View(MapToViewModel(banner));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Banner.Edit")]
    public async Task<IActionResult> Edit(int id, HomeBannerViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var banner = await _db.HomeBanners.FindAsync(id);
        if (banner is null) return NotFound();

        var mediaType = string.IsNullOrWhiteSpace(vm.MediaType) ? "Image" : vm.MediaType.Trim();

        banner.TitleTH              = vm.TitleTH      ?? string.Empty;
        banner.TitleEN              = vm.TitleEN       ?? string.Empty;
        banner.SubtitleTH           = vm.SubtitleTH;
        banner.SubtitleEN           = vm.SubtitleEN;
        banner.ButtonTextTH         = vm.ButtonTextTH;
        banner.ButtonTextEN         = vm.ButtonTextEN;
        banner.ButtonUrl            = vm.ButtonUrl;
        banner.MediaType            = mediaType;
        banner.VideoUrl             = mediaType == "YouTube" ? vm.YoutubeUrl?.Trim() : vm.VideoUrl?.Trim();
        banner.IsAutoplay           = vm.IsAutoplay;
        banner.IsMuted              = vm.IsMuted;
        banner.IsLoop               = vm.IsLoop;
        banner.ShowControls         = vm.ShowControls;
        banner.SlideDurationSeconds = vm.SlideDurationSeconds;
        banner.OverlayStyle         = vm.OverlayStyle;
        banner.TextPosition         = vm.TextPosition;
        banner.StartDate            = ToUtc(vm.StartDate);
        banner.EndDate              = ToUtc(vm.EndDate);
        banner.DisplayOrder         = vm.DisplayOrder;
        banner.IsActive             = vm.IsActive;
        banner.UpdatedAt            = DateTime.UtcNow;

        // A stale file selection left in a hidden Image/Video tab must never attach
        // itself to a YouTube banner, so file uploads are skipped entirely for that type.
        if (mediaType != "YouTube")
        {
            // Image upload
            if (vm.ImageFile is { Length: > 0 })
            {
                var (ok, err, path) = await SaveImageAsync(vm.ImageFile);
                if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
                banner.ImagePath = path;
            }

            // Mobile image upload
            if (vm.MobileImageFile is { Length: > 0 })
            {
                var (ok, err, path) = await SaveMobileImageAsync(vm.MobileImageFile);
                if (!ok) { ModelState.AddModelError(nameof(vm.MobileImageFile), err); return View(vm); }
                banner.MobileImageUrl = path;
            }

            // Video file upload
            if (vm.VideoFile is { Length: > 0 })
            {
                var (ok, err, path) = await SaveVideoAsync(vm.VideoFile);
                if (!ok) { ModelState.AddModelError(nameof(vm.VideoFile), err); return View(vm); }
                banner.VideoFileUrl = path;
            }

            // Mobile video file upload
            if (vm.MobileVideoFile is { Length: > 0 })
            {
                var (ok, err, path) = await SaveVideoAsync(vm.MobileVideoFile, "mobile");
                if (!ok) { ModelState.AddModelError(nameof(vm.MobileVideoFile), err); return View(vm); }
                banner.MobileVideoUrl = path;
            }
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = "Banner updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete ──────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Banner.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var banner = await _db.HomeBanners.FindAsync(id);
        if (banner is not null)
        {
            _db.HomeBanners.Remove(banner);
            await _db.SaveChangesAsync();
        }

        TempData["Success"] = "Banner deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private async Task<(bool ok, string error, string path)> SaveImageAsync(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(ext))
            return (false, $"Image type '{ext}' not allowed. Accepted: jpg, jpeg, png, webp, svg.", "");
        if (file.Length > MaxImageSize)
            return (false, "Image must not exceed 5 MB.", "");

        var dir = Path.Combine(_env.WebRootPath, "uploads", "banners");
        Directory.CreateDirectory(dir);
        var fileName = $"{Guid.NewGuid()}{ext}";
        await using var stream = new FileStream(Path.Combine(dir, fileName), FileMode.Create);
        await file.CopyToAsync(stream);
        return (true, "", $"/uploads/banners/{fileName}");
    }

    private async Task<(bool ok, string error, string path)> SaveMobileImageAsync(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(ext))
            return (false, $"Mobile image type '{ext}' not allowed. Accepted: jpg, jpeg, png, webp.", "");
        if (file.Length > MaxImageSize)
            return (false, "Mobile image must not exceed 5 MB.", "");

        var dir = Path.Combine(_env.WebRootPath, "uploads", "banners", "mobile");
        Directory.CreateDirectory(dir);
        var fileName = $"{Guid.NewGuid()}{ext}";
        await using var stream = new FileStream(Path.Combine(dir, fileName), FileMode.Create);
        await file.CopyToAsync(stream);
        return (true, "", $"/uploads/banners/mobile/{fileName}");
    }

    private async Task<(bool ok, string error, string path)> SaveVideoAsync(IFormFile file, string subFolder = "videos")
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedVideoExtensions.Contains(ext))
            return (false, $"Video type '{ext}' not allowed. Accepted: mp4, webm, mov.", "");
        if (file.Length > MaxVideoSize)
            return (false, "Video must not exceed 50 MB.", "");

        var dir = Path.Combine(_env.WebRootPath, "uploads", "banners", subFolder);
        Directory.CreateDirectory(dir);
        var fileName = $"{Guid.NewGuid()}{ext}";
        await using var stream = new FileStream(Path.Combine(dir, fileName), FileMode.Create);
        await file.CopyToAsync(stream);
        return (true, "", $"/uploads/banners/{subFolder}/{fileName}");
    }

    private static HomeBannerViewModel MapToViewModel(HomeBanner b) => new()
    {
        HomeBannerId        = b.Id,
        TitleTH             = b.TitleTH,
        TitleEN             = b.TitleEN,
        SubtitleTH          = b.SubtitleTH,
        SubtitleEN          = b.SubtitleEN,
        ButtonTextTH        = b.ButtonTextTH,
        ButtonTextEN        = b.ButtonTextEN,
        ButtonUrl           = b.ButtonUrl,
        ImageUrl            = b.ImagePath,
        MediaType           = b.MediaType,
        // The DB stores one VideoUrl column for both the Video and YouTube media
        // types; split it back into the two form fields based on MediaType.
        VideoUrl            = b.MediaType == "Video"   ? b.VideoUrl : null,
        YoutubeUrl          = b.MediaType == "YouTube" ? b.VideoUrl : null,
        VideoFileUrl        = b.VideoFileUrl,
        MobileImageUrl      = b.MobileImageUrl,
        MobileVideoUrl      = b.MobileVideoUrl,
        IsAutoplay          = b.IsAutoplay,
        IsMuted             = b.IsMuted,
        IsLoop              = b.IsLoop,
        ShowControls        = b.ShowControls,
        SlideDurationSeconds = b.SlideDurationSeconds,
        OverlayStyle        = b.OverlayStyle,
        TextPosition        = b.TextPosition,
        StartDate           = b.StartDate,
        EndDate             = b.EndDate,
        DisplayOrder        = b.DisplayOrder,
        IsActive            = b.IsActive,
    };

    private static DateTime? ToUtc(DateTime? dt) =>
        dt.HasValue ? DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc) : null;
}
