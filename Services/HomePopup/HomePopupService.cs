using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;

namespace Needis.Web.Services.HomePopup;

public class HomePopupService : IHomePopupService
{
    private readonly AppDbContext _db;

    public HomePopupService(AppDbContext db) => _db = db;

    public async Task<Models.HomePopup?> GetActivePopupAsync()
    {
        var now = DateTime.UtcNow;
        return await _db.HomePopups
            .AsNoTracking()
            .Where(p => p.IsActive && !p.IsDelete
                && (p.StartDateUtc == null || p.StartDateUtc <= now)
                && (p.EndDateUtc   == null || p.EndDateUtc   >= now))
            .OrderBy(p => p.DisplayOrder)
            .ThenByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
