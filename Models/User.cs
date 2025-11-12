// File: Models/User.cs
using Hotel.Services;

namespace Hotel.Models
{
  public class User
  {
    public string Username { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }

    // List of temporary rights
    public List<TemporaryPermission> TemporaryPermissions { get; set; } = new();

    public User(string username, string password, UserRole role = UserRole.Receptionist)
    {
      Username = username;
      Password = password;
      Role = role;
    }

    // Check if the user has a permission (permanent or temporary)
    public bool HasPermission(Permission permission)
    {
      // Admin always has full access
      if (Role == UserRole.Admin)
        return true;

      // Check the role's permanent permissions via RolePermissions
      if (RolePermissions.RolePermissionMap.TryGetValue(Role, out var perms) && perms.Contains(permission))
        return true;

      // Check temporary permissions and clear expired ones
      TemporaryPermissions.RemoveAll(tp => tp.Expiry <= DateTime.Now);
      return TemporaryPermissions.Any(tp => tp.Permission == permission && tp.IsActive);
    }

    // Grant temporary permission
    public void GrantTemporaryPermission(Permission permission, TimeSpan duration)
    {
      var expiry = DateTime.Now.Add(duration);
      TemporaryPermissions.Add(new TemporaryPermission(permission, expiry));
    }

    // Remove temporary permission
    public void RevokeTemporaryPermission(Permission permission)
    {
      TemporaryPermissions.RemoveAll(tp => tp.Permission == permission);
    }
  }

  public enum UserRole
  {
    Receptionist,
    Admin
  }
}
