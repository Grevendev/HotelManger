using Hotel;

List<User> users = FileService.LoadUser();
List<Room> rooms = FileService.LoadRooms();

User activeUser = null;
HotelManager hotel = new HotelManager(rooms);
bool running = true;

while (running)
{
  Console.Clear();

  if (activeUser == null)
  {
    Console.WriteLine("=== Hotel State Manager ===");
    Console.Write("Username: ");
    string userInput = Console.ReadLine();
    Console.WriteLine("Password: ");
    string passInput = Console.ReadLine();

    foreach (User u in users)
    {
      if (u.CheckLogin(userInput, passInput))
      {
        activeUser = u;
        break;
      }
    }
    if (activeUser == null)
    {
      Console.WriteLine("Wrong login!!");
      Console.ReadKey();
      continue;
    }
    Console.WriteLine($"Welcome {activeUser.GetName()}!");
    Console.ReadKey();
  }
  Console.Clear();
  Console.WriteLine("=== Main menu ===");
  Console.WriteLine("1. See all rooms");
  Console.WriteLine("2. See available rooms");
  Console.WriteLine("3. See occupied rooms");
  Console.WriteLine("4. Book a room");
  Console.WriteLine("5. Check-out an guest");
  Console.WriteLine("6. Mark an room as unavailable");
  Console.WriteLine("7. Logout");
  Console.WriteLine("Choice: ");
  string choice = Console.ReadLine();

  switch (choice)
  {
    case "1":
      hotel.ShowAllRooms();
      break;
    case "2":
      hotel.ShowAvailableRooms();
      break;
    case "3":
      hotel.ShowOccupiedRomms();
      break;
    case "4":
      Console.Write("Guestname: ");
      string guest = Console.ReadLine();
      Console.WriteLine("Room-number: ");
      int bookNum = int.Parse(Console.ReadLine());
      hotel.BookRoom(guest, bookNum);
      break;
    case "5":
      Console.Write("Room-mumber: ");
      int outNum = int.Parse(Console.ReadLine());
      hotel.Checkout(outNum);
      break;
    case "6":
      Console.Write("Room-number: ");
      int blockNum = int.Parse(Console.ReadLine());
      hotel.MarkUnavailable(blockNum);
      break;
    case "7":
      activeUser = null;
      break;
    default:
      Console.WriteLine("Unvalid choice!");
      break;
  }
  Console.WriteLine("Press any key to continue...");
  Console.ReadKey();
}