using Hotel.Models;
using Hotel.Services;

var fileService = new FileService();
var historyService = new HistoryService();
var userService = new UserService();
var bookingService = new BookingService(fileService, historyService);

bool exitProgram = false;

while (!exitProgram)
{
  User? activeUser = null;

  // --- LOGIN-LOOP ---
  while (activeUser == null)
  {
    Console.Clear();
    Console.WriteLine("=== Welcome to the Hotel Manager System ===");
    Console.Write("Username: ");
    var username = Console.ReadLine();
    Console.Write("Password: ");
    var password = Console.ReadLine();

    activeUser = userService.Login(username ?? "", password ?? "");

    if (activeUser == null)
    {
      Console.WriteLine("Invalid credentials. Try again...");
      Thread.Sleep(1500);
    }
    historyService.Log($"{activeUser.Username} logged in.", "LOGIN");
  }

  bool loggedIn = true;

  // --- LOGGED IN MENU ---
  while (loggedIn)
    while (loggedIn)
    {
      Console.Clear();
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine("\n╔════════════════════════════════╗");
      Console.WriteLine("║         HOTEL MAIN MENU        ║");
      Console.WriteLine("╚════════════════════════════════╝");
      Console.ResetColor();
      Console.WriteLine($"Logged in as {activeUser.Username} ({activeUser.Role})");
      Console.WriteLine();
      Console.WriteLine("=== MENU ===");
      Console.WriteLine("1. Show all rooms");
      Console.WriteLine("------------------------------------");
      Console.WriteLine("2. Show available rooms");
      Console.WriteLine("------------------------------------");
      Console.WriteLine("3. Show unavailable rooms");
      Console.WriteLine("------------------------------------");
      Console.WriteLine("4. Filter rooms");
      Console.WriteLine("------------------------------------");
      Console.WriteLine("5. Book a room");
      Console.WriteLine("------------------------------------");
      Console.WriteLine("6. Checkout a room");
      Console.WriteLine("------------------------------------");
      Console.WriteLine("7. Mark room as unavailable");
      Console.WriteLine("------------------------------------");
      Console.WriteLine("8. Logout");
      Console.WriteLine("------------------------------------");

      if (activeUser.Role == UserRole.Admin)
      {
        Console.WriteLine("9. Add new user");
        Console.WriteLine("------------------------------------");
        Console.WriteLine("10. Add new room");
        Console.WriteLine("------------------------------------");
        Console.WriteLine("11. Remove room");
        Console.WriteLine("------------------------------------");
        Console.WriteLine("12. Change room price");
        Console.WriteLine("------------------------------------");
        Console.WriteLine("13. Update room details (all properties)");
      }

      Console.WriteLine("14. Show full history");
      Console.WriteLine("------------------------------------");
      Console.WriteLine("15. Filter history");
      Console.WriteLine("------------------------------------");
      Console.WriteLine("16. Exit program");
      Console.WriteLine("------------------------------------");
      Console.WriteLine();

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
          Console.WriteLine("Filter rooms:");
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

        case "9" when activeUser.Role == UserRole.Admin:
          string newUser = InputHelper.GetString("New username: ");
          string newPass = InputHelper.GetString("Password: ");
          UserRole role = InputHelper.GetEnum("Role (Admin/Receptionist): ", UserRole.Receptionist);
          var allUsers = fileService.LoadUsers();
          allUsers.Add(new User(newUser, newPass, role));
          fileService.SaveUsers(allUsers);
          Console.WriteLine($"User {newUser} added with role {role}.");
          break;

        case "10" when activeUser.Role == UserRole.Admin:
        case "10" when activeUser.Role == UserRole.Receptionist && activeUser.HasPermission("AddRoom"):
          int newRoomNum = InputHelper.GetInt("Room number to add: ", minValue: 1);
          RoomType type = InputHelper.GetEnum("Room type (SingleBed/DoubleBed/Suite): ", RoomType.SingleBed);
          int capacity = InputHelper.GetInt("Capacity: ", minValue: 1);
          decimal price = InputHelper.GetDecimal("Price per night (default 500 if empty): ", minValue: 1, allowEmpty: true);
          string desc = InputHelper.GetString("Room description: ", allowEmpty: true);
          if (string.IsNullOrWhiteSpace(desc)) desc = "Standard room";
          string bedType = InputHelper.GetString("Bed type (Single/Queen/King): ", allowEmpty: true);
          if (string.IsNullOrWhiteSpace(bedType)) bedType = "Single";
          int bedCount = InputHelper.GetInt("Number of beds: ", minValue: 1);
          bookingService.AddRoom(newRoomNum, type, capacity, price > 0 ? price : 500, desc, bedType, bedCount);
          break;

        case "11" when activeUser.Role == UserRole.Admin:
        case "11" when activeUser.Role == UserRole.Receptionist && activeUser.HasPermission("RemoveRoom"):
          int removeRoomNum = InputHelper.GetInt("Room number to remove: ", minValue: 1);
          bookingService.RemoveRoom(removeRoomNum, activeUser);
          break;

        case "12" when activeUser.Role == UserRole.Admin:
        case "12" when activeUser.Role == UserRole.Receptionist && activeUser.HasPermission("ChangePrice"):
          int priceRoomNum = InputHelper.GetInt("Enter room number to update: ", minValue: 1);
          decimal newPrice = InputHelper.GetDecimal("Enter new price per night: ", minValue: 1);
          bookingService.UpdateRoomPrice(priceRoomNum, newPrice, activeUser);
          break;

        case "13" when activeUser.Role == UserRole.Admin:
        case "13" when activeUser.Role == UserRole.Receptionist && activeUser.HasPermission("UpdateRoom"):
          int updateNum = InputHelper.GetInt("Enter room number to update: ", minValue: 1);
          bookingService.UpdateRoom(updateNum, activeUser);
          break;

        case "14":
          Console.WriteLine("Hotel History:");
          historyService.DisplayHistory();
          break;

        case "15":
          string keyword = InputHelper.GetString("Keyword (optional): ", allowEmpty: true);
          DateTime? from = InputHelper.GetOptionalDate("From date (yyyy-MM-dd) or blank: ");
          DateTime? to = InputHelper.GetOptionalDate("To date (yyyy-MM-dd) or blank: ");
          var filtered = historyService.FilterHistory(from, to, keyword);
          historyService.DisplayHistory(filtered);
          break;

        case "16":
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
