namespace Hotel;

public interface IRoomRepository
{
  List<Room> LoadRooms();
  void SaveRooms(List<Room> rooms);
}