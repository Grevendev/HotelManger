using System.Collections.Generic;
using Hotel.Models;

namespace Hotel.Services;

public interface IUserRepository
{
  List<User> LoadUsers();
}
