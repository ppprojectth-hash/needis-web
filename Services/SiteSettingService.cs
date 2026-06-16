using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;

namespace Needis.Web.Services;

public class SiteSettingService : ISiteSettingService
{
    private readonly AppDbContext _db;

    public SiteSettingService(AppDbContext db) => _db = db;

    public Task<SiteSetting?> GetActiveAsync() =>
        _db.SiteSettings.AsNoTracking().Where(s => s.IsActive).FirstOrDefaultAsync();
}
