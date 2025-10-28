using Hotel;

var FileService = new FileService();
var hotel = new HotelManager(FileService);

bool running = true;

while (running)
{
  Console.Clear();
  Console.WriteLine("=== Hotel Management ===");
  Console.WriteLine("1. Show all rooms");
  Console.WriteLine("2. Show available rooms");
  Console.WriteLine("3. Book room");
  Console.WriteLine("4. Exit");
  Console.Write("Choice: ");
  var choice = Console.ReadLine();

  switch (choice)
  {
    case "1":
      hotel.ShowAllRooms(); break;
    case "2":
      hotel.ShowAvailableRooms(); break;
    case "3":
      Console.WriteLine("Guest name: ");
      var guest = Console.ReadLine();
      Console.Write("Room number: ");
      if (int.TryParse(Console.ReadLine(), out int number))
      {
        hotel.BookRoom(guest, number);
      }
      else
      {
        Console.WriteLine("Invalid room number!");
      }
      break;
    case "4":
      running = false; break;
    default:
      Console.WriteLine("Invalid choice!"); break;
  }
  Console.WriteLine("Press any key...");
  Console.ReadKey();
}
