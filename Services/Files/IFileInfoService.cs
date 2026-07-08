namespace Needis.Web.Services.Files;

public interface IFileInfoService
{
    long?  GetFileSizeBytes(string? fileUrl);
    string GetFileSizeDisplay(string? fileUrl);
    bool   FileExists(string? fileUrl);
}
