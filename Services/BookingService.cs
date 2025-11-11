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
  // --- FILTER ROOMS BY TYPE ---
  public void FilterRoomsByType()
  {
    Console.Write("Enter room type to filter (SingleBed / DoubleBed / Suite): ");
    string? input = Console.ReadLine();
    if (!Enum.TryParse(input, true, out RoomType type))
    {
      Console.WriteLine("Invalid type!");
      return;
    }

    var filtered = rooms.Where(r => r.type == type).ToList();
    Console.WriteLine($"\n=== Rooms of type: {type} ===");
    foreach (var room in filtered)
      DisplayRoomStatus(room);
  }

  // --- SORT ROOMS BY PRICE ---
  public void SortRoomsByPrice()
  {
    var sorted = rooms.OrderBy(r => r.pricePerNight).ToList();
    Console.WriteLine("\n=== Rooms sorted by price (ascending) ===");
    foreach (var room in sorted)
      DisplayRoomStatus(room);
  }

  // --- SORT ROOMS BY CAPACITY ---
  public void SortRoomsByCapacity()
  {
    var sorted = rooms.OrderByDescending(r => r.capacity).ToList();
    Console.WriteLine("\n=== Rooms sorted by capacity (descending) ===");
    foreach (var room in sorted)
      DisplayRoomStatus(room);
  }


  // --- DISPLAY ROOM STATUS ---
  // --- DISPLAY ROOM STATUS (DETAILED) ---
  // --- DISPLAY ROOM STATUS (DETAILED) ---
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
    Console.WriteLine($"Room {room.roomNumber} ({room.type}) - {room.status}");
    Console.ResetColor();

    Console.WriteLine($"  Capacity: {room.capacity}");
    Console.WriteLine($"  Price per night: {room.pricePerNight:C}");
    Console.WriteLine($"  Bed Type: {room.bedType} x{room.bedCount}");
    Console.WriteLine($"  Description: {room.description}");

    if (room.amenities != null && room.amenities.Count > 0)
      Console.WriteLine($"  Amenities: {string.Join(", ", room.amenities)}");

    if (!string.IsNullOrWhiteSpace(room.guestName))
      Console.WriteLine($"  Current guest: {room.guestName}");

    if (room.checkInDate.HasValue)
      Console.WriteLine($"  Checked in: {room.checkInDate.Value:g}");

    Console.WriteLine("------------------------------------");
  }

  // --- FILTER ROOMS ---
  public void FilterRooms(RoomType? type = null, RoomStatus? status = null, decimal? minPrice = null, decimal? maxPrice = null, int? minCapacity = null)
  {
    Console.WriteLine("\n=== Filtered Rooms ===");
    var filtered = rooms.Where(r =>
    (!type.HasValue || r.type == type) &&
    (!status.HasValue || r.status == status) &&
    (!minPrice.HasValue || r.pricePerNight >= minPrice) &&
    (!maxPrice.HasValue || r.pricePerNight <= maxPrice) &&
    (!minCapacity.HasValue || r.capacity >= minCapacity)
    ).ToList();

    if (filtered.Count == 0)
    {
      Console.WriteLine("No rooms match your criteria.");
      return;
    }
    foreach (var room in filtered)
      DisplayRoomStatus(room);
    Console.ResetColor();
  }


  // --- BOOK ROOM ---
  // --- BOOK ROOM WITH DATES ---

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

    // --- Input check-in date ---
    Console.Write("Enter check-in date (yyyy-MM-dd): ");
    if (!DateTime.TryParse(Console.ReadLine(), out DateTime checkIn))
    {
      Console.WriteLine("Invalid check-in date format!");
      return;
    }

    // --- Input check-out date ---
    Console.Write("Enter check-out date (yyyy-MM-dd): ");
    if (!DateTime.TryParse(Console.ReadLine(), out DateTime checkOut))
    {
      Console.WriteLine("Invalid check-out date format!");
      return;
    }

    if (checkOut <= checkIn)
    {
      Console.WriteLine("Check-out date must be after check-in date!");
      return;
    }

    int nights = (checkOut - checkIn).Days;
    decimal total = nights * room.pricePerNight;

    // --- Assign guest and check-in date ---
    room.SetGuest(guest);
    room.checkInDate = checkIn;

    // --- Save and log ---
    repository.SaveRooms(rooms);
    history.Log($"BOOKED | Guest '{guest}' booked Room {number} from {checkIn:yyyy-MM-dd} to {checkOut:yyyy-MM-dd} | Nights: {nights}, Total: {total:C}");

    Console.WriteLine($"Booking successful! {guest} will stay {nights} night(s). Total cost: {total:C}");
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
  public void AddRoom(
    int number,
    RoomType type,
    int capacity,
    decimal pricePerNight = 500,
    string description = "Standard room",
    string bedType = "Single",
    int bedCount = 1
  )
  {
    if (number <= 0)
    {
      Console.WriteLine("Room number must be greater than 0.");
      return;
    }
    if (capacity <= 0)
    {
      Console.WriteLine("Capacity must be greater than 0.");
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

    var newRoom = new Room(number, type, capacity, pricePerNight, description, bedType, bedCount);
    rooms.Add(newRoom);
    repository.SaveRooms(rooms);
    history.Log(
      $"SYSTEM | Room {number} added: {type}, capacity {capacity}, " +
      $"beds {bedCount}x{bedType}, price {pricePerNight:C}, desc '{description}'."
    );
    Console.WriteLine($"Room {number} successfully added.");
  }


  // --- REMOVE ROOM ---
  public void RemoveRoom(int number, User user)
  {
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
      Console.WriteLine($"Room {number} does not exist!");
      return;
    }

    // Kontrollera permission
    if (!user.HasPermission("RemoveRoom"))
    {
      Console.WriteLine("You do not have permission to remove this room.");
      return;
    }

    if (room.status == RoomStatus.Occupied)
    {
      Console.WriteLine($"Cannot remove Room {number} because it is currently occupied.");
      return;
    }

    rooms.Remove(room);
    repository.SaveRooms(rooms);
    history.Log($"SYSTEM | Room {number} removed by {user.Username}");
    Console.WriteLine($"Room {number} successfully removed.");
  }


  //UPDATE ROOM PRICE
  public void UpdateRoomPrice(int roomNumber, decimal newPrice, User user)
  {
    var room = rooms.Find(r => r.roomNumber == roomNumber);
    if (room == null)
    {
      Console.WriteLine($"Room {roomNumber} not found!");
      return;
    }

    // Kolla permissions
    if (!user.HasPermission("UpdatePrice"))
    {
      Console.WriteLine("You do not have permission to update room prices.");
      return;
    }

    if (newPrice <= 0)
    {
      Console.WriteLine("Invalid price. Must be greater than zero.");
      return;
    }

    room.pricePerNight = newPrice;
    repository.SaveRooms(rooms);
    history.Log($"SYSTEM | Room {roomNumber} price updated to {newPrice:C} by {user.Username}");
    Console.WriteLine($"Room {roomNumber} price updated to {newPrice:C}");
  }


  // --- UPDATE ROOM INFO (TYPE, CAPACITY, PRICE, BED INFO, DESCRIPTION, AMENITIES) ---
  public void UpdateRoom(int number, User user)
  {
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
      Console.WriteLine($"Room {number} not found!");
      return;
    }

    // Kontrollera permission
    if (!user.HasPermission("UpdateRoom"))
    {
      Console.WriteLine("You do not have permission to update this room.");
      return;
    }

    Console.WriteLine($"\nUpdating Room {number}");

    // --- TYPE ---
    Console.Write($"Current Type: {room.type} (SingleBed / DoubleBed / Suite, or leave empty): ");
    string? typeInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(typeInput) && Enum.TryParse(typeInput, out RoomType newType))
      room.type = newType;

    // --- CAPACITY ---
    Console.Write($"Current Capacity: {room.capacity} | New (or leave empty): ");
    string? capInput = Console.ReadLine();
    if (int.TryParse(capInput, out int newCap) && newCap > 0)
      room.capacity = newCap;

    // --- PRICE ---
    Console.Write($"Current Price: {room.pricePerNight:C} | New (or leave empty): ");
    string? priceInput = Console.ReadLine();
    if (decimal.TryParse(priceInput, out decimal newPrice) && newPrice > 0)
      room.pricePerNight = newPrice;

    // --- BED INFO ---
    Console.Write($"Current Bed Type: {room.bedType} | New (or leave empty): ");
    string? bedTypeInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(bedTypeInput))
      room.bedType = bedTypeInput;

    Console.Write($"Current Bed Count: {room.bedCount} | New (or leave empty): ");
    string? bedCountInput = Console.ReadLine();
    if (int.TryParse(bedCountInput, out int newBedCount) && newBedCount > 0)
      room.bedCount = newBedCount;

    // --- DESCRIPTION ---
    Console.Write($"Current Description: {room.description} | New (or leave empty): ");
    string? descInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(descInput))
      room.description = descInput;

    // --- AMENITIES ---
    Console.WriteLine("Update amenities (comma-separated, leave empty to skip): ");
    string? amenitiesInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(amenitiesInput))
      room.amenities = amenitiesInput.Split(',').Select(a => a.Trim()).ToList();

    repository.SaveRooms(rooms);
    history.Log($"SYSTEM | Room {number} updated by {user.Username}");
    Console.WriteLine($"Room {number} updated successfully!");
  }


}
