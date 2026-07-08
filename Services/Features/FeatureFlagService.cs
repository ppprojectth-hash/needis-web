using Microsoft.Extensions.Options;
using Needis.Web.Options;

namespace Needis.Web.Services.Features;

public class FeatureFlagService : IFeatureFlagService
{
    private readonly FeatureFlagsOptions _opts;

    public FeatureFlagService(IOptions<FeatureFlagsOptions> opts) => _opts = opts.Value;

    public bool ProductYoutubeVideoEnabled => _opts.ProductYoutubeVideo;
    public bool AboutGoogleMapEnabled      => _opts.AboutGoogleMap;
    public bool StaffProfileDetailEnabled  => _opts.StaffProfileDetail;
    public bool UploadedFileSizeEnabled    => _opts.UploadedFileSize;
    public bool HotProductPromotionEnabled => _opts.HotProductPromotion;
    public bool HomePopupEnabled           => _opts.HomePopup;
}
