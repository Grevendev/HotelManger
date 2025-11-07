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
  {
    Console.Clear();
    Console.WriteLine($"Logged in as {activeUser.Username} ({activeUser.Role})");
    Console.WriteLine();
    Console.WriteLine("=== MENU ===");
    Console.WriteLine("1. Show all rooms");
    Console.WriteLine("2. Show available rooms");
    Console.WriteLine("3. Show unavailable rooms");
    Console.WriteLine("4. Book a room");
    Console.WriteLine("5. Checkout a room");
    Console.WriteLine("6. Mark room as unavailable");
    Console.WriteLine("7. Logout");

    if (activeUser.Role == UserRole.Admin)
    {
      Console.WriteLine("8. Add new user");
      Console.WriteLine("9. Add new room");
      Console.WriteLine("10. Remove room");
      Console.WriteLine("11. Change room price");
      Console.WriteLine("12. Update room details (price/capacity/type)");
    }

    Console.WriteLine("13. Show full history");
    Console.WriteLine("14. Filter history");
    Console.WriteLine("15. Exit program");
    Console.WriteLine();
    Console.Write("Select an option: ");

    var choice = Console.ReadLine();

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
        Console.Write("Guest name: ");
        var guest = Console.ReadLine();
        Console.Write("Room number: ");
        if (int.TryParse(Console.ReadLine(), out int roomNum))
          bookingService.BookRoom(guest ?? "Unknown", roomNum);
        else
          Console.WriteLine("Invalid room number.");
        break;

      case "5":
        Console.Write("Room number to checkout: ");
        if (int.TryParse(Console.ReadLine(), out int checkoutNum))
          bookingService.CheckoutRoom(checkoutNum);
        else
          Console.WriteLine("Invalid room number.");
        break;

      case "6":
        Console.Write("Room number to mark unavailable: ");
        if (int.TryParse(Console.ReadLine(), out int unavailNum))
          bookingService.MakeRoomUnavailable(unavailNum);
        else
          Console.WriteLine("Invalid room number.");
        break;

      case "7":
        historyService.Log($"{activeUser.Username} logged out.", "LOGOUT");
        Console.WriteLine("Logging out...");
        Thread.Sleep(1000);
        loggedIn = false;
        break;

      case "8" when activeUser.Role == UserRole.Admin:
        Console.Write("New username: ");
        var newUser = Console.ReadLine();
        Console.Write("Password: ");
        var newPass = Console.ReadLine();
        Console.Write("Role (Admin/Receptionist): ");
        var roleInput = Console.ReadLine();

        var role = Enum.TryParse<UserRole>(roleInput, true, out var parsedRole)
            ? parsedRole
            : UserRole.Receptionist;

        var allUsers = fileService.LoadUsers();
        allUsers.Add(new User(newUser ?? "Unnamed", newPass ?? "1234", role));
        fileService.SaveUsers(allUsers);

        Console.WriteLine($"User {newUser} added with role {role}.");
        break;

      case "9" when activeUser.Role == UserRole.Admin:
  Console.Write("Room number to add: ");
  string? roomInput = Console.ReadLine();

  if (!int.TryParse(roomInput, out int newRoomNum) || newRoomNum <= 0)
  {
    Console.WriteLine("Invalid room number. Must be a positive number.");
    break;
  }

  Console.Write("Room type (SingleBed/DoubleBed/Suite): ");
  string? inputType = Console.ReadLine();
  if (!Enum.TryParse(inputType, true, out RoomType type))
  {
    Console.WriteLine("Invalid room type. Defaulting to SingleBed.");
    type = RoomType.SingleBed;
  }

  Console.Write("Capacity: ");
  if (!int.TryParse(Console.ReadLine(), out int capacity) || capacity <= 0)
  {
    Console.WriteLine("Invalid capacity. Must be a positive number.");
    break;
  }

  Console.Write("Price per night (default 500 if left empty): ");
  string? priceInput = Console.ReadLine();
  decimal price = 500;
  if (!string.IsNullOrWhiteSpace(priceInput))
  {
    if (!decimal.TryParse(priceInput, out price) || price <= 0)
    {
      Console.WriteLine("Invalid price. Must be a positive number.");
      break;
    }
  }

  bookingService.AddRoom(newRoomNum, type, capacity, price);
  break;



      case "10" when activeUser.Role == UserRole.Admin:
        Console.Write("Room number to remove: ");
        if (int.TryParse(Console.ReadLine(), out int removeRoomNum))
          bookingService.RemoveRoom(removeRoomNum);
        else
          Console.WriteLine("Invalid room number.");
        break;

      case "11" when activeUser.Role == UserRole.Admin:
        Console.Write("Enter room number to update: ");
        if (!int.TryParse(Console.ReadLine(), out int priceRoomNum))
        {
          Console.WriteLine("Invalid room number.");
          break;
        }
        Console.Write("Enter new price per night: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal newPrice))
        {
          Console.WriteLine("Invalid price.");
          break;
        }
        bookingService.UpdateRoomPrice(priceRoomNum, newPrice);
        break;

      case "12" when activeUser.Role == UserRole.Admin:
        Console.Write("Enter room number to update: ");
        if (int.TryParse(Console.ReadLine(), out int updateNum))
          bookingService.UpdateRoom(updateNum);
        else
          Console.WriteLine("Invalid room number.");
        break;

      case "13":
        Console.WriteLine("Hotel History:");
        historyService.DisplayHistory();
        break;

      case "14":
        Console.Write("Keyword (optional): ");
        var keyword = Console.ReadLine();
        Console.Write("From date (yyyy-MM-dd) or blank: ");
        var fromInput = Console.ReadLine();
        Console.Write("To date (yyyy-MM-dd) or blank: ");
        var toInput = Console.ReadLine();

        DateTime? from = DateTime.TryParse(fromInput, out var f) ? f : null;
        DateTime? to = DateTime.TryParse(toInput, out var t) ? t : null;

        var filtered = historyService.FilterHistory(from, to, keyword);
        historyService.DisplayHistory(filtered);
        break;

      case "15":
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
