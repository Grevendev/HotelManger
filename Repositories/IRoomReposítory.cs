using System.Collections.Generic;


namespace Hotel.Services;

public interface IRoomRepository
{
  List<Room> LoadRooms();
  void SaveRooms(List<Room> rooms);
}
