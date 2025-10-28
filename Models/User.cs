namespace Hotel;

public class User
{
  public string Username;
  public string Password;
  public UserRole Role;

  public User(string username, string password, UserRole role = UserRole.Receptionist)
  {
    Username = username;
    Password = password;
    Role = role;
  }
}

public enum UserRole
{
  Receptionist,
  Admin
}
