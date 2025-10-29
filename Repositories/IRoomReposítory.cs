using System.Collections.Generic;
using Hotel.Models;

namespace Hotel.Services;

public interface IRoomRepository
{
  List<Room> LoadRooms();
  void SaveRooms(List<Room> rooms);
}
