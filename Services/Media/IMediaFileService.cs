using Needis.Web.Models;

namespace Needis.Web.Services.Media;

public interface IMediaFileService
{
    Task<MediaFile> UploadAsync(
        IFormFile file,
        string usageType,
        string? relatedModule = null,
        int? relatedEntityId = null,
        string? folder = null,
        string? uploadedBy = null,
        CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteAsync(int id, string? updatedBy, CancellationToken cancellationToken = default);

    Task<bool> UpdateMetadataAsync(
        int id,
        string? titleTH, string? titleEN,
        string? altTextTH, string? altTextEN,
        string? captionTH, string? captionEN,
        string? descriptionTH, string? descriptionEN,
        bool isPublic, bool isActive,
        string? updatedBy,
        CancellationToken cancellationToken = default);

    bool IsAllowedFile(IFormFile file, out string errorMessage);

    string GetFileType(string extension, string contentType);
}
