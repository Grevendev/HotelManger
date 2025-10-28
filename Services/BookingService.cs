namespace Hotel;

public class BookingService
{
  private List<Room> rooms;
  private IRoomRepository repository;
  private HistoryService history;

  public BookingService(IRoomRepository repo, HistoryService historyService)
  {
    repository = repo;
    rooms = repository.LoadRooms();
    history = historyService;
  }
  public void ShowAllRooms()
  {
    foreach (var room in rooms) Console.WriteLine(room.ToString());
  }
  public void ShowAvailableRooms()
  {
    foreach (var room in rooms)
    {
      if (room.status == RoomStatus.Available) Console.WriteLine(room.ToString());
    }
  }
  public void ShowUnavailableRooms()
  {
    foreach (var room in rooms)
    {
      if (room.status == RoomStatus.Unavailable) Console.WriteLine(room.ToString());
    }
  }
  public void BookRoom(string guest, int number)
  {
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
      Console.WriteLine("Room not found!");
      return;
    }
    if (room.status != RoomStatus.Available)
    {
      Console.WriteLine("Room not available!");
      return;
    }
    room.SetGuest(guest);
    repository.SaveRooms(rooms);
    history.LogBooking(room, guest);
    Console.WriteLine($"Guest {guest} booked in room {number}.");
  }
  public void CheckoutRoom(int number)
  {
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null || room.status != RoomStatus.Occupied)
    {
      Console.WriteLine("Room is not occupied!");
      return;
    }
    string guest = room.guestName;
    room.ClearGuest();
    repository.SaveRooms(rooms);
    history.LogCheckout(room, guest);
    Console.WriteLine($"Guest {guest} checked out from room {number}.");
  }
  public void MakeRoomUnavailable(int number)
  {
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
      Console.WriteLine("Room not found!");
      return;
    }
    room.MakeUnavailable();
    repository.SaveRooms(rooms);
    Console.WriteLine($"Room {number} marked as unavailable.");
  }
}