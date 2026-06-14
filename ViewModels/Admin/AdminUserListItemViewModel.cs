namespace Needis.Web.ViewModels.Admin;

public class AdminUserListItemViewModel
{
    public int       Id          { get; init; }
    public string    Username    { get; init; } = string.Empty;
    public string    Email       { get; init; } = string.Empty;
    public string?   DisplayName { get; init; }
    public string    Role        { get; init; } = string.Empty;
    public bool      IsActive    { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public DateTime  CreatedAt   { get; init; }
}
