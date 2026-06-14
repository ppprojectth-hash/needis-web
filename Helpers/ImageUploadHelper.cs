namespace Needis.Web.Helpers;

public static class ImageUploadHelper
{
    private static readonly string[] AllowedExtensions =
        [".jpg", ".jpeg", ".png", ".webp", ".svg"];

    private const long MaxSizeBytes = 3 * 1024 * 1024; // 3 MB

    public static async Task<(bool Ok, string Error, string Path)> SaveAsync(
        IFormFile file,
        string webRootPath,
        string relativeFolder)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(ext))
            return (false,
                $"File type '{ext}' not allowed. Accepted: jpg, jpeg, png, webp, svg.",
                string.Empty);

        if (file.Length > MaxSizeBytes)
            return (false, "Image size must not exceed 3 MB.", string.Empty);

        var folder = relativeFolder.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var dir    = Path.Combine(webRootPath, folder);
        Directory.CreateDirectory(dir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(dir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var urlPath = "/" + relativeFolder.Trim('/') + "/" + fileName;
        return (true, string.Empty, urlPath);
    }
}
