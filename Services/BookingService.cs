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
    ConsoleColor color = room.status switch
    {
      RoomStatus.Available => ConsoleColor.Green,
      RoomStatus.Occupied => ConsoleColor.Red,
      RoomStatus.Unavailable => ConsoleColor.Yellow,
      _ => ConsoleColor.White
    };

    Console.ForegroundColor = color;

    string guestInfo = room.guestName != null ? $" (Guest: {room.guestName})" : "";
    string priceInfo = $" | Price: {room.pricePerNight:C} | Capacity: {room.capacity}";

    Console.WriteLine($"Room {room.roomNumber}: {room.status}, {room.type}{guestInfo}{priceInfo}");

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

    // --- CALCULATE TOTAL AMOUNT OF NIGHTS ---
    var checkIn = room.checkInDate ?? DateTime.Now;
    var nights = (DateTime.Now - checkIn).Days;
    if (nights < 1) nights = 1;

    var totalCost = nights * room.pricePerNight;

    room.ClearGuest();
    repository.SaveRooms(rooms);

    history.Log($"CHECKOUT | Guest '{guest}' checked out from Room {number} after {nights} night(s). Total: {totalCost:C}");
    Console.WriteLine($"Guest {guest} checked out from Room {number}. Total cost: {totalCost:C}");
  }

  // --- MAKE UNAVAILABLE ---
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

  // --- ADD ROOM ---
  public void AddRoom(int number, RoomType type, int capacity, decimal pricePerNight = 500)
  {
    if (number <= 0)
    {
      Console.WriteLine("Room number must be greater than 0.");
      return;
    }
    if (capacity <= 0)
    {
      Console.WriteLine("Capactity must be greater than 0.");
      return;
    }
    if (pricePerNight <= 0)
    {
      Console.WriteLine("Price must be greater than 0.");
      return;
    }
    if (rooms.Exists(r => r.roomNumber == number))
    {
      Console.WriteLine($"Room {number} already exists!");
      return;
    }

    var newRoom = new Room(number, type, capacity, pricePerNight);
    rooms.Add(newRoom);
    repository.SaveRooms(rooms);
    history.Log($"SYSTEM | Room {number} added: {type}, capacity {capacity}, price {pricePerNight:C} added to the hotel.");

    Console.WriteLine($"Room {number} successfully added.");
  }

  // --- REMOVE ROOM ---
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
  //UPDATE ROOM PRICE
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
  // --- UPDATE ROOM INFO (PRICE, CAPACITY, TYPE) ---
  public void UpdateRoom(int number)
{
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
        Console.WriteLine($"Room {number} not found!");
        return;
    }

    Console.WriteLine($"\nUpdating Room {number}");
    Console.WriteLine($"Current Type: {room.type} (SingleBed / DoubleBed / Suite, leave empty to keep current): ");
    string? typeInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(typeInput) && Enum.TryParse(typeInput, true, out RoomType newType))
        room.type = newType;

    Console.Write($"Current Capacity: {room.capacity}. Enter new capacity (leave empty to keep current): ");
    string? capInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(capInput))
    {
        if (int.TryParse(capInput, out int newCap) && newCap > 0)
            room.capacity = newCap;
        else
            Console.WriteLine("Invalid capacity. Must be a positive number. Keeping current value.");
    }

    Console.Write($"Current Price: {room.pricePerNight:C}. Enter new price (leave empty to keep current): ");
    string? priceInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(priceInput))
    {
        if (decimal.TryParse(priceInput, out decimal newPrice) && newPrice > 0)
            room.pricePerNight = newPrice;
        else
            Console.WriteLine("Invalid price. Must be a positive number. Keeping current value.");
    }

    repository.SaveRooms(rooms);
    history.Log($"SYSTEM | Room {number} updated: Type={room.type}, Capacity={room.capacity}, Price={room.pricePerNight:C}");
    Console.WriteLine($"Room {number} updated successfully!");
}

}
