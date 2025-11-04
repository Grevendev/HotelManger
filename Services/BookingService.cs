using Hotel.Models;

namespace Hotel.Services;

public class BookingService
{
  private readonly List<Room> rooms;
  private readonly IRoomRepository repository;
  private readonly HistoryService history;

  public BookingService(IRoomRepository repo, HistoryService historyService)
  {
    repository = repo ?? throw new ArgumentNullException(nameof(repo));
    history = historyService ?? throw new ArgumentNullException(nameof(historyService));
    rooms = repository.LoadRooms() ?? new List<Room>();
  }

  // --- SHOW ALL ROOMS ---
  public void ShowAllRooms()
  {
    Console.WriteLine("\n=== All Rooms ===");
    foreach (var room in rooms)
      DisplayRoomStatus(room);
    Console.ResetColor();
  }


  // --- SHOW AVAILABLE ROOMS ---
  public void ShowAvailableRooms()
  {
    Console.WriteLine("=== Available Rooms ===");
    foreach (var room in rooms)
      if (room.status == RoomStatus.Available)
        DisplayRoomStatus(room);
    Console.ResetColor();
  }

  // --- SHOW UNAVAILABLE ROOMS ---
  public void ShowUnavailableRooms()
  {
    Console.WriteLine("=== Unavailable Rooms ===");
    foreach (var room in rooms)
      if (room.status == RoomStatus.Unavailable)
        DisplayRoomStatus(room);
    Console.ResetColor();
  }

  // --- DISPLAY ROOM STATUS ---
  private void DisplayRoomStatus(Room room)
  {
    switch (room.status)
    {
      case RoomStatus.Available:
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Room {room.roomNumber}: Available");
        break;
      case RoomStatus.Occupied:
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Room {room.roomNumber}: Occupied by {room.guestName}");
        break;
      case RoomStatus.Unavailable:
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Room {room.roomNumber}: Unavailable");
        break;
    }
    Console.WriteLine($"Room {room.roomNumber}: {room.status}, {room.type}, Capacity: {room.capacity}" +
    (room.guestName != null ? $" ({room.guestName})" : ""));

    Console.ResetColor();
  }

  // --- BOOK ROOM ---
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
    room.checkInDate = DateTime.Now;
    repository.SaveRooms(rooms);
    history.Log($"BOOKED | Guest '{guest}' booked Room {number}");
    Console.WriteLine($"Guest {guest} successfully booked Room {number}.");
  }

  // --- CHECKOUT ROOM ---
  public void CheckoutRoom(int number)
  {
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
      Console.WriteLine("Room not found!");
      return;
    }

    if (room.status != RoomStatus.Occupied)
    {
      Console.WriteLine("Room is not currently occupied.");
      return;
    }

    var guest = room.guestName ?? "Unknown";

    // Calculate total amount of nights
    var checkIn = room.checkInDate ?? DateTime.Now;
    var nights = (DateTime.Now - checkIn).Days;
    if (nights < 1) nights = 1;

    var totalCost = nights * room.pricePerNight;

    room.ClearGuest();
    repository.SaveRooms(rooms);

    history.Log($"CHECKOUT | Guest '{guest}' checked out from Room {number} after {nights} night(s). Total: {totalCost:C}");
    Console.WriteLine($"Guest {guest} checked out from Room {number}. Total cost: {totalCost:C}");
  }

  // --- Markera otillgänglig ---
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
    history.Log($"UNAVAILABLE | Room {number} marked as temporarily unavailable.");
    Console.WriteLine($"Room {number} marked as unavailable.");
  }

  // --- Add room ---
  public void AddRoom(int number, RoomType type, int capacity)
  {
    if (rooms.Exists(r => r.roomNumber == number))
    {
      Console.WriteLine($"Room {number} already exists!");
      return;
    }

    var newRoom = new Room(number, type, capacity);
    rooms.Add(newRoom);
    repository.SaveRooms(rooms);
    history.Log($"SYSTEM | Room {number} added: {type}, capacity {capacity} added to the hotel.");
    Console.WriteLine($"Room {number} successfully added.");
  }

  // --- Remove room ---
  public void RemoveRoom(int number)
  {
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
      Console.WriteLine($"Room {number} does not exist!");
      return;
    }

    if (room.status == RoomStatus.Occupied)
    {
      Console.WriteLine($"Cannot remove Room {number} because it is currently occupied.");
      return;
    }

    rooms.Remove(room);
    repository.SaveRooms(rooms);
    history.Log($"SYSTEM | Room {number} removed from the hotel.");
    Console.WriteLine($"Room {number} successfully removed.");
  }
  //Update room price
  public void UpdateRoomPrice(int roomNumber, decimal newPrice)
  {
    var room = rooms.Find(r => r.roomNumber == roomNumber);
    if (room == null)
    {
      Console.WriteLine($"Room {roomNumber} not found!");
      return;
    }
    if (newPrice <= 0)
    {
      Console.WriteLine("Invalid price. Must be greater than zero.");
      return;
    }
    room.pricePerNight = newPrice;
    repository.SaveRooms(rooms);
    history.Log($"SYSTEM | Room {roomNumber} price updated to {newPrice:C}");
    Console.WriteLine($"Room {roomNumber} price updated to {newPrice:C}");
  }
}
