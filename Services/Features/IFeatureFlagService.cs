namespace Needis.Web.Services.Features;

public interface IFeatureFlagService
{
    bool ProductYoutubeVideoEnabled { get; }
    bool AboutGoogleMapEnabled      { get; }
    bool StaffProfileDetailEnabled  { get; }
    bool UploadedFileSizeEnabled    { get; }
    bool HotProductPromotionEnabled { get; }
    bool HomePopupEnabled           { get; }
}
