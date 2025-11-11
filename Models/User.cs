namespace Hotel.Models
{
  public class User
  {
    public string Username;
    public string Password;
    public UserRole Role;
    public List<string> TempPermissions { get; set; } = new();

    public User(string username, string password, UserRole role = UserRole.Receptionist)
    {
      Username = username;
      Password = password;
      Role = role;
    }

    // Kontrollera om användaren har permission
    public bool HasPermission(string permission)
    {
      return Role == UserRole.Admin || TempPermissions.Contains(permission);
    }

    public void GrantTempPermission(string permission)
    {
      if (!TempPermissions.Contains(permission))
        TempPermissions.Add(permission);
    }

    public void RevokeTempPermission(string permission)
    {
      if (TempPermissions.Contains(permission))
        TempPermissions.Remove(permission);
    }
  }

  public enum UserRole
  {
    Receptionist,
    Admin
  }
}
