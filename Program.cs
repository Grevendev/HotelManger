using Hotel.Models;
using Hotel.Services;

// --- Load data ---
var fileService = new FileService(); // Hanterar filbaserad lagring av users och rooms
var users = fileService.LoadUsers();
var rooms = fileService.LoadRooms();

var historyService = new HistoryService();
var bookingService = new BookingService(rooms, historyService);
var permService = new PermissionService(users);

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
    Console.WriteLine("8. Logout");

    int menuIndex = 9;

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
        int roomNum = InputHelper.GetInt("Room number: ", minValue: 1);
        bookingService.BookRoom(guest, roomNum);
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
        historyService.Log($"{activeUser.Username} logged out.", "LOGOUT");
        Console.WriteLine("Logging out...");
        Thread.Sleep(1000);
        loggedIn = false;
        break;
      case "9" when activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.AddUser):
        string newUser = InputHelper.GetString("New username: ");
        string newPass = InputHelper.GetString("Password: ");
        UserRole role = InputHelper.GetEnum("Role (Admin/Receptionist): ", UserRole.Receptionist);
        users.Add(new User(newUser, newPass, role));
        fileService.SaveUsers(users);
        Console.WriteLine($"User {newUser} added with role {role}.");
        break;
      case "10" when activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.AddRoom):
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
      case "11" when activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.RemoveRoom):
        int removeRoomNum = InputHelper.GetInt("Room number to remove: ", minValue: 1);
        bookingService.RemoveRoom(removeRoomNum, activeUser);
        break;
      case "12" when activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.UpdateRoomPrice):
        int priceRoomNum = InputHelper.GetInt("Enter room number to update price: ", minValue: 1);
        decimal newPrice = InputHelper.GetDecimal("Enter new price: ", minValue: 1);
        bookingService.UpdateRoomPrice(priceRoomNum, newPrice, activeUser);
        break;
      case "13" when activeUser.Role == UserRole.Admin || activeUser.HasPermission(Permission.UpdateRoom):
        int updateNum = InputHelper.GetInt("Enter room number to update details: ", minValue: 1);
        bookingService.UpdateRoom(updateNum, activeUser);
        break;
      case "14" when activeUser.HasPermission(Permission.ViewHistory):
        historyService.DisplayHistory();
        break;
      case "15" when activeUser.HasPermission(Permission.FilterHistory):
        string keyword = InputHelper.GetString("Keyword (optional): ", allowEmpty: true);
        DateTime? from = InputHelper.GetOptionalDate("From date (yyyy-MM-dd) or blank: ");
        DateTime? to = InputHelper.GetOptionalDate("To date (yyyy-MM-dd) or blank: ");
        var filtered = historyService.FilterHistory(from, to, keyword);
        historyService.DisplayHistory(filtered);
        break;
      case "16" when activeUser.HasPermission(Permission.ManagePermissions):
        permService.ManagePermissionsMenu(activeUser);
        break;
      case "17":
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
}
