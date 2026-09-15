namespace Identity.Api.Data.Entities;

public class UserEntity
{
    public int Id
    {
        get; set;
    }
    public string UserName { get; set; } = string.Empty;
    public string? DisplayName
    {
        get; set;
    }
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive
    {
        get; set;
    }
    public bool MustChangePassword
    {
        get; set;
    }
    public DateTime? LastLoginAt
    {
        get; set;
    }
    public DateTime CreatedAt
    {
        get; set;
    }
    public DateTime? UpdatedAt
    {
        get; set;
    }
    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    public ICollection<UserSessionEntity> Sessions { get; set; } = new List<UserSessionEntity>();
}
