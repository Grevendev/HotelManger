using Hotel.Models;

namespace Hotel;

public class HotelManager
{
  private List<Room> rooms;
  public HotelManager(List<Room> loadedRooms)
  {
    rooms = loadedRooms;
  }
  public void ShowAllRooms()
  {
    foreach (Room r in rooms)
      Console.WriteLine(r.ToString());
  }
  public void ShowAvailableRooms()
  {
    foreach (Room r in rooms)
      if (r.status == RoomStatus.Available)
        Console.WriteLine(r.ToString());
  }
  public void ShowOccupiedRomms()
  {
    foreach (Room r in rooms)
      if (r.status == RoomStatus.Occupied)
        Console.WriteLine(r.ToString());
  }
  public void BookRoom(string guest, int number)
  {
    Room room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
      Console.WriteLine("Room does not exist!");
      return;
    }
    room.SetGuest(guest);
    FileService.SaveRooms(rooms);
    Console.WriteLine($"Guest {guest} have been booked in this room {number}.");
  }
  public void Checkout(int number)
  {
    Room room = rooms.Find(r => r.roomNumber == number);
    if (room == null || room.status != RoomStatus.Occupied)
    {
      Console.WriteLine("Unvalid room or not occupied.");
      return;
    }
    Console.WriteLine($"Guest {room.guestName} have checked out from this room {room.roomNumber}");
    room.ClearGuest();
    FileService.SaveRooms(rooms);
  }
  public void MarkUnavailable(int number)
  {
    Room room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
      Console.WriteLine("Room does not exist!");
      return;
    }
    room.MakeUnavailable();
    FileService.SaveRooms(rooms);
    Console.WriteLine($"Room {number} are now unavailble.");
  }
}