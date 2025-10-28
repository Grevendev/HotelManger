using Hotel;


// Skapa tjänsterna i rätt ordning
var fileService = new FileService();       // ✅ litet f
var history = new HistoryService();
var bookingService = new BookingService(fileService, history); // ✅ rättstavat 'fileService'

// Om du har HotelManager kvar (men den används ej i mainloopen ännu)
var hotel = new HotelManager(fileService);



bool running = true;

while (running)
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
  Console.WriteLine("8. Exit");
  Console.Write("Choice: ");
  var choice = Console.ReadLine();

  switch (choice)
  {
    case "1":
      hotel.ShowAllRooms(); break;
    case "2":
      hotel.ShowAvailableRooms(); break;
    case "3":
      bookingService.ShowUnavailableRooms(); break;
    case "4":
      Console.WriteLine("Guest name: ");
      var guest = Console.ReadLine();
      Console.Write("Room number: ");
      if (int.TryParse(Console.ReadLine(), out int roomNum1))
        bookingService.BookRoom(guest, roomNum1);
      else
        Console.WriteLine("Invaild room number!"); break;

    case "5":
      Console.Write("Room number to checkout: ");
      if (int.TryParse(Console.ReadLine(), out int roomNum2))
        bookingService.CheckoutRoom(roomNum2);
      else
        Console.WriteLine("Invalid room number!");
      break;
    case "6":
      Console.Write("Room number to mark unavailable: ");
      if (int.TryParse(Console.ReadLine(), out int roomNum3))
        bookingService.MakeRoomUnavailable(roomNum3);
      else
        Console.WriteLine("Invalid room number!");
      break;
    case "7":
      history.ShowHistory();
      break;
    case "8":
      running = false; break;
    default:
      Console.WriteLine("Invalid choice!");
      break;
  }
  Console.WriteLine("Press any key...");
  Console.ReadKey();
}
