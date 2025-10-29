using Hotel.Services;
using Hotel.Models;

var fileService = new FileService();
var historyService = new HistoryService();
var bookingService = new BookingService(fileService, historyService);

User? activeUser = null;

while (activeUser == null)
{
  Console.Clear();
  Console.WriteLine("Welcome to the Hotel Manager System");
  Console.Write("Username: ");
  var username = Console.ReadLine();
  Console.Write("Password: ");
  var password = Console.ReadLine();

  var users = fileService.LoadUsers();
  activeUser = users.FirstOrDefault(u => u.Username == username && u.Password == password);

  if (activeUser == null)
  {
    Console.WriteLine("Invalid credentials. Try again.");
    Thread.Sleep(1500);
  }
}

bool running = true;

while (running)
{
  Console.Clear();
  Console.WriteLine($"Logged in as {activeUser.Username} ({activeUser.Role})");
  Console.WriteLine("Menu Options:");
  Console.WriteLine("1. Show all rooms");
  Console.WriteLine("2. Show available rooms");
  Console.WriteLine("3. Show unavailable rooms");
  Console.WriteLine("4. Book a room");
  Console.WriteLine("5. Checkout a room");
  Console.WriteLine("6. Mark room as unavailable");
  Console.WriteLine("7. Exit");

  if (activeUser.Role == UserRole.Admin)
  {
    Console.WriteLine("8. Add new user");
    Console.WriteLine("9. Add new room");
    Console.WriteLine("10. Remove room");
  }

  Console.WriteLine("11. Show full history");
  Console.WriteLine("12. Filter history");

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
        bookingService.BookRoom(guest!, roomNum);
      else
        Console.WriteLine("Invalid room number");
      break;

    case "5":
      Console.Write("Room number to checkout: ");
      if (int.TryParse(Console.ReadLine(), out int checkoutNum))
        bookingService.CheckoutRoom(checkoutNum);
      else
        Console.WriteLine("Invalid room number");
      break;

    case "6":
      Console.Write("Room number to mark as unavailable: ");
      if (int.TryParse(Console.ReadLine(), out int unavailNum))
        bookingService.MakeRoomUnavailable(unavailNum);
      else
        Console.WriteLine("Invalid room number");
      break;

    case "7":
      running = false;
      Console.WriteLine("Goodbye!");
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
      allUsers.Add(new User(newUser!, newPass!, role));
      fileService.SaveUsers(allUsers);
      Console.WriteLine($"User {newUser} added with role {role}.");
      break;

    case "9" when activeUser.Role == UserRole.Admin:
      Console.Write("Room number to add: ");
      if (int.TryParse(Console.ReadLine(), out int newRoomNum))
        bookingService.AddRoom(newRoomNum);
      else
        Console.WriteLine("Invalid room number");
      break;

    case "10" when activeUser.Role == UserRole.Admin:
      Console.Write("Room number to remove: ");
      if (int.TryParse(Console.ReadLine(), out int removeRoomNum))
        bookingService.RemoveRoom(removeRoomNum);
      else
        Console.WriteLine("Invalid room number");
      break;

    case "11":
      Console.WriteLine("Hotel History:");
      historyService.DisplayHistory();
      break;

    case "12":
      Console.Write("Keyword to filter by (optional): ");
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

    default:
      Console.WriteLine("Invalid option, try again.");
      break;
  }

  Console.WriteLine();
  Console.WriteLine("Press any key to continue...");
  Console.ReadKey();
}
