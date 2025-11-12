using Hotel.Models;

namespace Hotel.Services
{
  public class PermissionService
  {
    private readonly List<User> _users;

    public PermissionService(List<User> users)
    {
      _users = users;
    }

    // Handles the entire menu for temporary permissions

    public void ManagePermissionsMenu(User admin)
    {
      if (admin.Role != UserRole.Admin)
      {
        Console.WriteLine("Only admins can manage permissions.");
        return;
      }

      bool back = false;

      while (!back)
      {
        Console.Clear();
        Console.WriteLine("=== TEMPORARY PERMISSIONS MENU ===");
        Console.WriteLine("1. Grant temporary permission");
        Console.WriteLine("2. Revoke temporary permission");
        Console.WriteLine("3. List user permissions");
        Console.WriteLine("4. Back to main menu");

        string choice = InputHelper.GetString("Select an option: ");

        switch (choice)
        {
          case "1":
            GrantTemporaryPermission(admin);
            break;
          case "2":
            RevokeTemporaryPermission(admin);
            break;
          case "3":
            ListAllUserPermissions();
            break;
          case "4":
            back = true;
            break;
          default:
            Console.WriteLine("Invalid option, try again.");
            break;
        }

        if (!back)
        {
          Console.WriteLine("\nPress any key to continue...");
          Console.ReadKey();
        }
      }
    }

    private void GrantTemporaryPermission(User admin)
    {
      string targetUsername = InputHelper.GetString("Enter target username: ");
      var targetUser = _users.FirstOrDefault(u => u.Username.Equals(targetUsername, StringComparison.OrdinalIgnoreCase));

      if (targetUser == null)
      {
        Console.WriteLine("User not found.");
        return;
      }

      Console.WriteLine("\nAvailable permissions:");
      foreach (var perm in Enum.GetValues(typeof(Permission)))
        Console.WriteLine($"- {perm}");

      string permInput = InputHelper.GetString("Enter permission name to grant: ");

      if (!Enum.TryParse<Permission>(permInput, true, out var permission))
      {
        Console.WriteLine("Invalid permission name.");
        return;
      }

      int hours = InputHelper.GetInt("Enter duration in hours (1-72): ", 1, 72);
      targetUser.GrantTemporaryPermission(permission, TimeSpan.FromHours(hours));

      Console.WriteLine($"Granted {permission} to {targetUser.Username} for {hours} hours.");
    }

    private void RevokeTemporaryPermission(User admin)
    {
      string targetUsername = InputHelper.GetString("Enter target username: ");
      var targetUser = _users.FirstOrDefault(u => u.Username.Equals(targetUsername, StringComparison.OrdinalIgnoreCase));

      if (targetUser == null)
      {
        Console.WriteLine("User not found.");
        return;
      }

      if (!targetUser.TemporaryPermissions.Any())
      {
        Console.WriteLine("No temporary permissions to revoke.");
        return;
      }

      Console.WriteLine($"Temporary permissions for {targetUser.Username}:");
      foreach (var tp in targetUser.TemporaryPermissions)
        Console.WriteLine($"- {tp.Permission} (expires {tp.Expiry})");

      string permInput = InputHelper.GetString("Enter permission name to revoke: ");

      if (!Enum.TryParse<Permission>(permInput, true, out var permission))
      {
        Console.WriteLine("Invalid permission name.");
        return;
      }

      targetUser.RevokeTemporaryPermission(permission);
      Console.WriteLine($"Revoked {permission} from {targetUser.Username}.");
    }

    private void ListAllUserPermissions()
    {
      foreach (var user in _users)
      {
        Console.WriteLine($"\nUser: {user.Username} ({user.Role})");

        if (RolePermissions.RolePermissionMap.TryGetValue(user.Role, out var perms))
        {
          Console.WriteLine("Permanent permissions:");
          foreach (var p in perms)
            Console.WriteLine($"- {p}");
        }

        if (user.TemporaryPermissions.Any())
        {
          Console.WriteLine("Temporary permissions:");
          foreach (var tp in user.TemporaryPermissions)
            Console.WriteLine($"- {tp.Permission} (expires {tp.Expiry})");
        }
      }
    }
  }
}
