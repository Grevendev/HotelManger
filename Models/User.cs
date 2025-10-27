namespace Hotel;

public class User
{
  public string username;
  public string password;

  public User(string name, string pass)
  {
    username = name;
    password = pass;
  }
  public string GetName()
  {
    return username;
  }
  public bool CheckLogin(string name, string pass)
  {
    return username == name && password == pass;
  }
}