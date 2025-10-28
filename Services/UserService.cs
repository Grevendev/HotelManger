namespace Hotel.Services;

public class UserService
{
  private readonly List<User> users;

  public UserService()
  {
    var fileService = new FileService();
    users = fileService.LoadUsers();

    if (users.Count == 0)
    {
      users.Add(new User("admin", "password", UserRole.Admin));
      fileService.SaveUsers(users);
    }
  }

  public User? Login(string username, string password)
  {
    foreach (var user in users)
    {
      if (user.Username == username && user.Password == password)
        return user;
    }
    return null;
  }
}
