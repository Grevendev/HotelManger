using Hotel.Models;
using Hotel.Services;

// --- Load data ---
var fileService = new FileService(); // Handles file-based storage of users and rooms
var users = fileService.LoadUsers();
var rooms = fileService.LoadRooms();

var historyService = new HistoryService();
var bookingService = new BookingService(rooms, historyService, fileService);
var permService = new PermissionService(users);
bookingService.ActivateTodaysReservations();
bookingService.AutoReleaseMaintenance();
List<RoomSearchFilter> filterHistory = new List<RoomSearchFilter>();


foreach (var room in rooms)
{
  if (room.Status == RoomStatus.Occupied && room.CheckOutDate.HasValue && room.CheckOutDate.Value <= DateTime.Now)
  {
    bookingService.CheckoutRoom(room.Number);
    Console.WriteLine($"Room {room.Number} automatically checked out (past CheckOutDate).");
  }
}
bool exitProgram = false;

while (!exitProgram)
{
  User? activeUser = null;

  // --- LOGIN LOOP ---
  while (activeUser == null)
  {
    Console.Clear();
    Console.WriteLine("=== Welcome to the Hotel Manager System ===");
    Console.Write("Username: ");
    var username = Console.ReadLine();
    Console.Write("Password: ");
    var password = Console.ReadLine();

    activeUser = users.Find(u => u.Username == username && u.Password == password);

    if (activeUser == null)
    {
      Console.WriteLine("Invalid credentials. Try again...");
      Thread.Sleep(1500);
    }
    else
    {
      historyService.Log($"{activeUser.Username} logged in.", "LOGIN");
    }
  }

  bool loggedIn = true;

  // --- LOGGED IN MENU ---
  while (loggedIn)
  {
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n╔════════════════════════════════╗");
    Console.WriteLine("║         HOTEL MAIN MENU        ║");
    Console.WriteLine("╚════════════════════════════════╝");
    Console.ResetColor();
    Console.WriteLine($"Logged in as {activeUser.Username} ({activeUser.Role})\n");

    Console.WriteLine("=== MENU ===");
    Console.WriteLine("1. Show all rooms");
    Console.WriteLine("2. Show available rooms");
    Console.WriteLine("3. Show unavailable rooms");
    Console.WriteLine("4. Filter rooms");
    Console.WriteLine("5. Book a room");
    Console.WriteLine("6. Checkout a room");
    Console.WriteLine("7. Mark room as unavailable");
    Console.WriteLine("8. Book room in advance");
    Console.WriteLine("9. Cancel future booking");
    Console.WriteLine("10. Confirm future booking");
    Console.WriteLine("11. Print booking receipt");
    Console.WriteLine("12. Mark room as cleaned");
    Console.WriteLine("13. Mark room cleaning in progress");
    Console.WriteLine("14. Run advance search");
    Console.WriteLine("15. Logout");


    int menuIndex = 15;

    if (activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.AddUser))
      Console.WriteLine($"{menuIndex++}. Add new user");

    if (activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.AddRoom))
      Console.WriteLine($"{menuIndex++}. Add new room");

    if (activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.RemoveRoom))
      Console.WriteLine($"{menuIndex++}. Remove room");

    if (activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.UpdateRoomPrice))
      Console.WriteLine($"{menuIndex++}. Change room price");

    if (activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.UpdateRoom))
      Console.WriteLine($"{menuIndex++}. Update room details");

    if (activeUser.HasPermission(Permission.ViewHistory))
      Console.WriteLine($"{menuIndex++}. Show full history");

    if (activeUser.HasPermission(Permission.FilterHistory))
      Console.WriteLine($"{menuIndex++}. Filter history");

    if (activeUser.HasPermission(Permission.ManagePermissions))
      Console.WriteLine($"{menuIndex++}. Manage temporary permissions");

    if (activeUser.Role == UserRole.Admin)
      Console.WriteLine($"{menuIndex++}. Add discount/campaign");

    if (activeUser.Role == UserRole.Admin)
    {
      Console.WriteLine($"{menuIndex++}. Mark room as under maintenance");
      Console.WriteLine($"{menuIndex++}. End maintenance for a room");
      Console.WriteLine($"{menuIndex++}. Extend maintenance period");
    }
    if (activeUser.Role == UserRole.Admin)
      Console.WriteLine($"{menuIndex++}. Show occupancy & revenue statistics");


    Console.WriteLine($"{menuIndex}. Exit program");

    string choice = InputHelper.GetString("Select an option: ");

    switch (choice)
    {
      case "1":
        bookingService.ShowAllRooms();
        break;

      case "2":
        bookingService.ShowAvailableRooms();
        break;

      case "3":
        bookingService.ShowUnavailableRooms();
        break;

      case "4":
        RoomType? fType = InputHelper.GetOptionalEnum<RoomType>("Type (SingleBed/DoubleBed/Suite or leave empty): ");
        RoomStatus? fStatus = InputHelper.GetOptionalEnum<RoomStatus>("Status (Available/Occupied/Unavailable or leave empty): ");
        decimal? minPrice = InputHelper.GetOptionalDecimal("Minimum price (or leave empty): ");
        decimal? maxPrice = InputHelper.GetOptionalDecimal("Maximum price (or leave empty): ");
        int? minCap = InputHelper.GetOptionalInt("Minimum capacity (or leave empty): ");
        bookingService.FilterRooms(fType, fStatus, minPrice, maxPrice, minCap);
        break;

      case "5":
        string guest = InputHelper.GetString("Guest name: ");
        int bookRoomNum = InputHelper.GetInt("Room number: ", minValue: 1);
        bookingService.BookRoom(guest, bookRoomNum);
        break;

      case "6":
        int checkoutNum = InputHelper.GetInt("Room number to checkout: ", minValue: 1);
        bookingService.CheckoutRoom(checkoutNum);
        break;

      case "7":
        int unavailNum = InputHelper.GetInt("Room number to mark unavailable: ", minValue: 1);
        bookingService.MakeRoomUnavailable(unavailNum);
        break;


      case "8":
        Console.Write("Guest name: ");
        var futureGuest = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(futureGuest))
        {
          Console.WriteLine("Guest name cannot be empty. Booking cancelled.");
          break;
        }

        int futureRoom = InputHelper.GetInt("Room number: ");
        DateTime futureCheckIn = InputHelper.GetDate("Check-in date (YYYY-MM-DD): ");
        int futureDays = InputHelper.GetInt("Number of nights: ", 1);

        bookingService.BookRoomInAdvance(futureGuest!, futureRoom, futureCheckIn, futureDays);
        break;


      case "9":
        int cancel = InputHelper.GetInt("Room number to cancel future booking: ", 1);
        bookingService.CancelFutureBooking(cancel);
        break;

      case "10":
        int confirmRoom = InputHelper.GetInt("Room number to confirm for future booking: ", 1);
        bookingService.ConfirmFutureBooking(confirmRoom);
        break;

      case "11":
        int receiptRoom = InputHelper.GetInt("Room number to print receipt: ", 1);
        bookingService.PrintBookingReceipt(receiptRoom);
        break;

      case "12":
        int cleanProgRoom = InputHelper.GetInt("Room number: ");
        bookingService.SetCleaningInProgress(cleanProgRoom);
        break;

      case "13":
        int cleanedRoom = InputHelper.GetInt("Room number: ");
        bookingService.MakeRoomAsCleaned(cleanedRoom);
        break;

      case "14":
        RunAdvancedSearch();
        break;



      case "15":
        historyService.Log($"{activeUser.Username} logged out.", "LOGOUT");
        Console.WriteLine("Logging out...");
        Thread.Sleep(1000);
        loggedIn = false;
        break;


      case "16" when activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.AddUser):
        string newUser = InputHelper.GetString("New username: ");
        string newPass = InputHelper.GetString("Password: ");
        UserRole role = InputHelper.GetEnum("Role (Admin/Receptionist): ", UserRole.Receptionist);
        users.Add(new User(newUser, newPass, role));
        fileService.SaveUsers(users);
        Console.WriteLine($"User {newUser} added with role {role}.");
        break;

      case "17" when activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.AddRoom):
        int newRoomNum = InputHelper.GetInt("Room number to add: ", minValue: 1);
        RoomType type = InputHelper.GetEnum("Room type (SingleBed/DoubleBed/Suite): ", RoomType.SingleBed);
        int capacity = InputHelper.GetInt("Capacity: ", minValue: 1);
        decimal price = InputHelper.GetDecimal("Price per night: ", minValue: 1);
        string desc = InputHelper.GetString("Room description: ", allowEmpty: true);
        if (string.IsNullOrWhiteSpace(desc)) desc = "Standard room";
        string bedType = InputHelper.GetString("Bed type (Single/Queen/King): ", allowEmpty: true);
        if (string.IsNullOrWhiteSpace(bedType)) bedType = "Single";
        int bedCount = InputHelper.GetInt("Number of beds: ", minValue: 1);
        bookingService.AddRoom(newRoomNum, type, capacity, price, desc, bedType, bedCount);
        break;

      case "18" when activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.RemoveRoom):
        int removeRoomNum = InputHelper.GetInt("Room number to remove: ", minValue: 1);
        bookingService.RemoveRoom(removeRoomNum, activeUser);
        break;

      case "19" when activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.UpdateRoomPrice):
        int priceRoomNum = InputHelper.GetInt("Enter room number to update price: ", minValue: 1);
        decimal newPrice = InputHelper.GetDecimal("Enter new price: ", minValue: 1);
        bookingService.UpdateRoomPrice(priceRoomNum, newPrice, activeUser);
        break;

      case "20" when activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.UpdateRoom):
        int updateNum = InputHelper.GetInt("Enter room number to update details: ", minValue: 1);
        bookingService.UpdateRoom(updateNum, activeUser);
        break;

      case "21" when activeUser.HasPermission(Permission.ViewHistory):
        historyService.DisplayHistory();
        break;

      case "22" when activeUser.HasPermission(Permission.FilterHistory):
        string keyword = InputHelper.GetString("Keyword (optional): ", allowEmpty: true);
        DateTime? from = InputHelper.GetOptionalDate("From date (yyyy-MM-dd) or blank: ");
        DateTime? to = InputHelper.GetOptionalDate("To date (yyyy-MM-dd) or blank: ");
        var filtered = historyService.FilterHistory(from, to, keyword);
        historyService.DisplayHistory(filtered);
        break;

      case "23" when activeUser.HasPermission(Permission.ManagePermissions):
        permService.ManagePermissionsMenu(activeUser);
        break;

      case "24" when activeUser.Role == UserRole.Admin:
        string dName = InputHelper.GetString("Discount name: ");
        DiscountType dType = InputHelper.GetEnum("Type (Percentage/FixedAmount): ", DiscountType.Percentage);
        decimal dValue = InputHelper.GetDecimal("Value: ", minValue: 0);
        RoomType? rType = InputHelper.GetOptionalEnum<RoomType>("Optional room type or leave empty: ");
        DateTime? start = InputHelper.GetOptionalDate("Start date (yyyy-MM-dd) or leave empty: ");
        DateTime? end = InputHelper.GetOptionalDate("End date (yyyy-MM-dd) or leave empty: ");
        int? minStay = InputHelper.GetOptionalInt("Minimum stay days or leave empty: ");
        var discount = new Discount(dName, dType, dValue) { RoomType = rType, StartDate = start, EndDate = end, MinStayDays = minStay };
        bookingService.AddDiscount(discount);
        break;

      case "25" when activeUser.Role == UserRole.Admin:
        int m1 = InputHelper.GetInt("Room number: ");
        DateTime ms = InputHelper.GetDate("Start date: ");
        DateTime me = InputHelper.GetDate("End date: ");
        bookingService.MarkRoomUnderMaintenance(m1, ms, me, activeUser);
        break;

      case "26" when activeUser.Role == UserRole.Admin:
        int m2 = InputHelper.GetInt("Room number: ");
        bookingService.EndMaintenance(m2, activeUser);
        break;

      case "27" when activeUser.Role == UserRole.Admin:
        int m3 = InputHelper.GetInt("Room number: ");
        DateTime newEnd = InputHelper.GetDate("New end date: ");
        bookingService.ExtendMaintenance(m3, newEnd, activeUser);
        break;

      case "28" when activeUser.Role == UserRole.Admin:
        bookingService.ShowAdminStatistics();
        break;



      case "29":
        Console.WriteLine("Exiting program...");
        Thread.Sleep(1000);
        loggedIn = false;
        exitProgram = true;
        break;

      default:
        Console.WriteLine("Invalid option, try again.");
        break;
    }

    Console.WriteLine();
    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
  }

  void DisplayRooms(List<Room> rooms)
  {
    if (rooms.Count == 0)
    {
      Console.WriteLine("No rooms match the filter criteria.");
      return;
    }

    Console.WriteLine("\n--- Matching Rooms ---");
    Console.WriteLine("Number | Type   | Status      | Capacity | Price/Night | Beds | Amenities");
    Console.WriteLine("--------------------------------------------------------------------------");

    foreach (var room in rooms)
    {
      string amenities = room.Amenities != null && room.Amenities.Count > 0
          ? string.Join(", ", room.Amenities)
          : "None";
      Console.WriteLine($"{room.Number,6} | {room.Type,-6} | {room.Status,-11} | {room.Capacity,8} | {room.PricePerNight,11:C} | {room.BedCount,4} | {amenities}");
    }
  }
  List<Room> SortRooms(List<Room> rooms)
  {
    Console.WriteLine("\nSort by: 1 = Price, 2 = Capacity, 3 = Type, 4 = No sorting");
    string sortChoice = Console.ReadLine();

    return sortChoice switch
    {
      "1" => rooms.OrderBy(r => r.PricePerNight).ToList(),
      "2" => rooms.OrderByDescending(r => r.Capacity).ToList(),
      "3" => rooms.OrderBy(r => r.Type).ToList(),
      _ => rooms
    };
  }

  void SaveFilterHistory(RoomSearchFilter filter)
  {
    filterHistory.Add(filter);
    Console.WriteLine("Filter saved to history.");
  }
  void ShowFilterHistory()
  {
    if (!filterHistory.Any())
    {
      Console.WriteLine("No saved filters.");
      return;
    }

    Console.WriteLine("\n--- Saved Filters ---");
    for (int i = 0; i < filterHistory.Count; i++)
    {
      var f = filterHistory[i];
      Console.WriteLine(
    $"{i + 1}: " +
    $"Type={f.Type?.ToString() ?? "Any"}, " +
    $"MinCap={f.MinCapacity?.ToString() ?? "Any"}, " +
    $"Status={f.Status?.ToString() ?? "Any"}, " +
    $"MinPrice={f.MinPrice?.ToString() ?? "Any"}, " +
    $"MaxPrice={f.MaxPrice?.ToString() ?? "Any"}, " +
    $"Amenities={f.RequiredAmenities?.Count ?? 0}"
);

    }
  }
  bool ValidateFilter(RoomSearchFilter filter)
  {
    if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue && filter.MinPrice > filter.MaxPrice)
    {
      Console.WriteLine("Minimum price cannot be greater than maximum price.");
      return false;
    }

    if (filter.MinCapacity.HasValue && filter.MinCapacity <= 0)
    {
      Console.WriteLine("Minimum capacity must be at least 1.");
      return false;
    }

    return true;
  }
  void RunAdvancedSearch()
  {
    Console.WriteLine("\n=== ADVANCED ROOM SEARCH ===");

    var filter = new RoomSearchFilter();

    Console.Write("Room type (Single, Double, Suite) or leave empty: ");
    var typeInput = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(typeInput) &&
        Enum.TryParse<RoomType>(typeInput, true, out var parsedType))
    {
      filter.Type = parsedType;
    }
    else
    {
      filter.Type = null;
    }
    filter.MinCapacity = InputHelper.GetOptionalInt("Minimum capacity or leave empty: ");
    filter.MinPrice = InputHelper.GetOptionalDecimal("Minimum price or leave empty: ");
    filter.MaxPrice = InputHelper.GetOptionalDecimal("Maximum price or leave empty: ");
    filter.Status = InputHelper.GetOptionalEnum<RoomStatus>("Status (Available/Occupied/Unavailable) or leave empty: ");

    Console.Write("Required amenities (comma-separated) or leave empty: ");
    var amInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(amInput))
      filter.RequiredAmenities = amInput.Split(',').Select(a => a.Trim()).ToList();

    if (!ValidateFilter(filter)) return;

    var matchingRooms = bookingService.SearchRooms(filter);
    matchingRooms = SortRooms(matchingRooms);

    DisplayRooms(matchingRooms);
    SaveFilterHistory(filter);
  }
}
