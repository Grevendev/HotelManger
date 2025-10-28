using Hotel;
using Hotel.Models;
using Hotel.Services;

var fileService = new FileService();
var history = new HistoryService();
var bookingService = new BookingService(fileService, history);
var userService = new UserService();

User? activeUser = null;

// --- Login-loop ---
while (activeUser == null)
{
  Console.Clear();
  Console.WriteLine("=== Hotel Management Login ===");
  Console.Write("Username: ");
  var username = Console.ReadLine();
  Console.Write("Password: ");
  var password = Console.ReadLine();

  activeUser = userService.Login(username!, password!);

  if (activeUser == null)
  {
    Console.WriteLine("Invalid credentials. Press any key to try again...");
    Console.ReadKey();
  }
}

// --- Menu Methods ---
void ShowMenu(User activeUser)
{
  Console.Clear();
  Console.WriteLine("=== Hotel Management ===");
  Console.WriteLine("1. Show all rooms");
  Console.WriteLine("2. Show available rooms");
  Console.WriteLine("3. Show unavailable rooms");
  Console.WriteLine("4. Book room");
  Console.WriteLine("5. Checkout room");
  Console.WriteLine("6. Make room unavailable");
  Console.WriteLine("7. Show history");

  if (activeUser.Role == UserRole.Admin)
  {
    Console.WriteLine("8. Add new user");
    Console.WriteLine("9. Add new room");
    Console.WriteLine("10. Remove room");
    Console.WriteLine("11. Logout");
    Console.WriteLine("12. Exit");
  }
  else
  {
    Console.WriteLine("8. Logout");
    Console.WriteLine("9. Exit");
  }
  Console.WriteLine("----------------------");
  Console.Write("Enter your choice: ");
}

int GetMenuChoice(User activeUser)
{
  int maxChoice = activeUser.Role == UserRole.Admin ? 12 : 9;
  while (true)
  {
    var input = Console.ReadLine();
    if (int.TryParse(input, out int choice) && choice >= 1 && choice <= 9)
      return choice;
    Console.Write("Invalid choice, try again: ");
  }
}

// --- Main loop ---
bool running = true;

while (running)
{
  ShowMenu(activeUser);
  int choice = GetMenuChoice(activeUser);

  switch (choice)
  {
    case 1: bookingService.ShowAllRooms(); break;
    case 2: bookingService.ShowAvailableRooms(); break;
    case 3: bookingService.ShowUnavailableRooms(); break;
    case 4:
      Console.Write("Guest name: ");
      var guest = Console.ReadLine();
      Console.Write("Room number: ");
      if (int.TryParse(Console.ReadLine(), out int roomNum1))
        bookingService.BookRoom(guest!, roomNum1);
      else
        Console.WriteLine("Invalid room number!");
      break;
    case 5:
      Console.Write("Room number to checkout: ");
      if (int.TryParse(Console.ReadLine(), out int roomNum2))
        bookingService.CheckoutRoom(roomNum2);
      else
        Console.WriteLine("Invalid room number!");
      break;
    case 6:
      Console.Write("Room number to mark unavailable: ");
      if (int.TryParse(Console.ReadLine(), out int roomNum3))
        bookingService.MakeRoomUnavailable(roomNum3);
      else
        Console.WriteLine("Invalid room number!");
      break;
    case 7:
      history.ShowHistory();
      break;

    // Admin-specifika val
    case 8 when activeUser.Role == UserRole.Admin:
      Console.Write("New username: ");
      var newUser = Console.ReadLine();
      Console.Write("Password: ");
      var newPass = Console.ReadLine();
      Console.Write("Role (Admin/Receptionist): ");
      var roleInput = Console.ReadLine();
      var role = Enum.TryParse<UserRole>(roleInput, true, out var parsedRole) ? parsedRole : UserRole.Receptionist;

      var allUsers = new FileService().LoadUsers();
      allUsers.Add(new User(newUser!, newPass!, role));
      new FileService().SaveUsers(allUsers);
      Console.WriteLine($"User '{newUser}' added with role {role}.");
      break;

    case 9 when activeUser.Role == UserRole.Admin:
      Console.Write("Room number to add: ");
      if (int.TryParse(Console.ReadLine(), out int newRoomNum))
        bookingService.AddRoom(newRoomNum);
      else
        Console.WriteLine("Invalid room number!");
      break; // 

    case 10 when activeUser.Role == UserRole.Admin:
      Console.Write("Room number to remove: ");
      if (int.TryParse(Console.ReadLine(), out int removeRoomNum))
        bookingService.RemoveRoom(removeRoomNum);
      else
        Console.WriteLine("Invalid room number!");
      break; // 


    case 8: // Receptionist logout
    case 9 when activeUser.Role == UserRole.Admin: // Admin logout
      activeUser = null;
      Console.WriteLine("Logged out. Press any key to login again...");
      Console.ReadKey();
      while (activeUser == null)
      {
        Console.Clear();
        Console.WriteLine("=== Hotel Management Login ===");
        Console.Write("Username: ");
        var u2 = Console.ReadLine();
        Console.Write("Password: ");
        var p2 = Console.ReadLine();
        activeUser = userService.Login(u2!, p2!);
        if (activeUser == null)
        {
          Console.WriteLine("Invalid credentials. Press any key to try again...");
          Console.ReadKey();
        }
      }
      break;

    case 9: // Receptionist exit
    case 10: // Admin exit
      running = false;
      break;
  }

  if (running)
  {
    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
  }
}
