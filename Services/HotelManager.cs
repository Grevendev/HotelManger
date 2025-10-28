namespace Hotel;

public class HotelManager
{
  private List<Room> rooms;
  private IRoomRepository repository;

  public HotelManager(IRoomRepository repo)
  {
    repository = repo;
    rooms = repository.LoadRooms();
  }
  public void ShowAllRooms()
  {
    foreach (Room r in rooms) Console.WriteLine(r.ToString());
  }
  public void ShowAvailableRooms()
  {
    foreach (Room r in rooms)
      if (r.status == RoomStatus.Available) Console.WriteLine(r.ToString());
  }
  public void BookRoom(string guest, int number)
  {
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null || room.status != RoomStatus.Available)
    {
      Console.WriteLine("Room not available!");
      return;
    }
    room.SetGuest(guest);
    repository.SaveRooms(rooms);
    Console.WriteLine($"Guest {guest} booked in room {number}.");
  }
}