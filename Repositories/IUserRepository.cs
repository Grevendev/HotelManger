namespace Hotel;

public interface IUserRepository
{
  List<User> LoadUsers();
}