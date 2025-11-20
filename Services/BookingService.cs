// File: Services/BookingService.cs
using Hotel.Models;

namespace Hotel.Services
{
  public class BookingService
  {
    private FileService _fileService;
    private readonly List<Room> _rooms;
    private readonly HistoryService _history;
    private readonly List<Reservation> _reservations = new();
    private readonly List<Discount> _discounts = new();

    public BookingService(List<Room> rooms, HistoryService history, FileService fileService)
    {
      _rooms = rooms;
      _history = history;
      _fileService = fileService;
      _reservations = _fileService.LoadReservations();
    }

    public void ShowAllRooms()
    {
      Console.WriteLine("--- All Rooms ---");
      foreach (var room in _rooms)
        DisplayRoom(room);
    }

    public void ShowAvailableRooms()
    {
      Console.WriteLine("--- Available Rooms ---");
      foreach (var room in _rooms.Where(r => r.Status == RoomStatus.Available))
        DisplayRoom(room);
    }

    public void BookRoomInAdvance(string guest, int roomNumber, DateTime checkIn, int stayDays)
    {
      var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
      if (room == null)
      {
        Console.WriteLine($"Room {roomNumber} not found!");
        return;
      }

      if (checkIn.Date <= DateTime.Now.Date)
      {
        Console.WriteLine("Check-in date must be in the future.");
        return;
      }

      // Kontrollera överlappande bokningar
      var checkOut = checkIn.AddDays(stayDays);
      bool overlaps = _reservations.Any(r =>
          r.RoomNumber == roomNumber && checkIn < r.CheckOutDate && checkOut > r.CheckInDate);

      if (overlaps)
      {
        Console.WriteLine("Room is already booked during that period.");
        return;
      }

      // Beräkna totalpris med rabatter
      decimal totalPrice = ApplyDiscounts(room, stayDays) * stayDays;

      // Lägg till reservation
      _reservations.Add(new Reservation(roomNumber, guest, checkIn, checkOut));

      _history.Log($"RESERVATION | Room {roomNumber} reserved by {guest} from {checkIn:yyyy-MM-dd} to {checkOut:yyyy-MM-dd}, Total: {totalPrice:C}");

      Console.WriteLine($"Room {roomNumber} reserved successfully for {guest} from {checkIn:yyyy-MM-dd} to {checkOut:yyyy-MM-dd}. Total price: {totalPrice:C}");
    }


    public void ActivateTodaysReservations()
    {
      var today = DateTime.Now.Date;

      var dueReservations = _reservations
      .Where(r => r.CheckInDate.Date <= today)
      .ToList();

      foreach (var res in dueReservations)
      {
        var room = _rooms.FirstOrDefault(r => r.Number == res.RoomNumber);
        if (room == null) continue;

        room.Status = RoomStatus.Occupied;
        room.GuestName = res.Guest;
        room.CheckInDate = res.CheckInDate;
        room.CheckOutDate = res.CheckOutDate;

        _history.Log($"ACTIVATE | Room {room.Number} auto-activated for {res.Guest} (reservation started)");

        Console.WriteLine($"Room {room.Number} is now occupied for {res.Guest} (reservation began).");

        _reservations.Remove(res);
      }
    }

    public void CancelFutureBooking(int roomNumber)
    {
      var reservation = _reservations
          .FirstOrDefault(r => r.RoomNumber == roomNumber);

      if (reservation == null)
      {
        Console.WriteLine("This room has no future booking to cancel.");
        return;
      }

      // Check if the reservation already started
      if (reservation.CheckInDate.Date <= DateTime.Today)
      {
        Console.WriteLine("This reservation has already started and cannot be cancelled.");
        return;
      }

      // Remove reservation
      _reservations.Remove(reservation);

      // Save reservations list
      _fileService.SaveReservations(_reservations);

      _history.Log(
          $"CANCEL FUTURE | Future reservation for room {roomNumber} cancelled (Guest: {reservation.Guest})."
      );

      Console.WriteLine($"Future reservation for room {roomNumber} has been cancelled.");
    }

    public void ConfirmFutureBooking(int roomNumber)
    {
      var reservation = _reservations.FirstOrDefault(r => r.RoomNumber == roomNumber);
      if (reservation == null)
      {
        Console.WriteLine("No future booking found for this room.");
        return;
      }
      if (reservation.IsConfirmed)
      {
        Console.WriteLine("This booking is already confirmed.");
        return;
      }
      reservation.IsConfirmed = true;
      _history.Log($"CONFIRM | Future booking for room {roomNumber} confirmed for {reservation.Guest}");
      Console.WriteLine($"Future booking for Room {roomNumber} confirmed for {reservation.Guest}.");

      _fileService.SaveReservations(_reservations);
    }

    public void PrintBookingReceipt(int roomNumber)
    {
      var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
      if (room == null)
      {
        Console.WriteLine($"Room {roomNumber} not found!");
        return;
      }
      if (room.Status != RoomStatus.Occupied && room.Status != RoomStatus.BookedInAdvance)
      {
        Console.WriteLine($"Room {roomNumber} is not currently booked.");
        return;
      }
      var checkIn = room.CheckInDate ?? DateTime.Now;
      var checkOut = room.CheckOutDate ?? checkIn.AddDays(1);
      var totalDays = (checkOut - checkIn).Days;
      totalDays = totalDays == 0 ? 1 : totalDays;
      var totalPrice = totalDays * room.PricePerNight;

      string receipt = $@" 
      --- Booking Receipt ---
      Guest: {room.GuestName}
      Room Number: {room.Number}
      Room Type: {room.Type}
      Check-in: {checkIn:yyyy-MM-dd}
      Check-out: {checkOut:yyyy-MM-dd}
      Nights: {totalDays}
      Price per night: {room.PricePerNight:C}
      Total Price {totalPrice:C}
      -----------------------
      ";

      Console.WriteLine(receipt);

      string fileName = $"receipts/Receipt_Room{room.Number}_{DateTime.Now:yyyyMMddHHmmss}.txt";
      Directory.CreateDirectory("receipts");
      File.WriteAllText(fileName, receipt);

      _history.Log($"RECEIPT | Printed receipt for Room {room.Number} ({room.GuestName})");

      Console.WriteLine($"Receipt saved to {fileName}");
    }

    public void AddDiscount(Discount discount)
    {
      _discounts.Add(discount);
      _history.Log($"SYSTEM | Discount '{discount.Name}' added.");
      Console.WriteLine($"Discount '{discount.Name}' added successfully.");
    }
    public decimal ApplyDiscounts(Room room, int stayDays)
    {
      decimal finalPrice = room.PricePerNight;
      foreach (var discount in _discounts)
      {
        if (discount.IsActive(room.Type, room.CheckInDate ?? DateTime.Now, stayDays))
        {
          finalPrice = discount.Apply(finalPrice);
          Console.WriteLine($"Applied discount '{discount.Name}': new price {finalPrice:C}");
        }
      }
      return finalPrice;
    }

    public void ShowUnavailableRooms()
    {
      Console.WriteLine("--- Unavailable Rooms ---");
      foreach (var room in _rooms.Where(r => r.Status == RoomStatus.Unavailable))
        DisplayRoom(room);
    }

    public void FilterRooms(RoomType? type = null, RoomStatus? status = null, decimal? minPrice = null, decimal? maxPrice = null, int? minCap = null)
    {
      var filtered = _rooms.AsEnumerable();

      if (type.HasValue) filtered = filtered.Where(r => r.Type == type.Value);
      if (status.HasValue) filtered = filtered.Where(r => r.Status == status.Value);
      if (minPrice.HasValue) filtered = filtered.Where(r => r.PricePerNight >= minPrice.Value);
      if (maxPrice.HasValue) filtered = filtered.Where(r => r.PricePerNight <= maxPrice.Value);
      if (minCap.HasValue) filtered = filtered.Where(r => r.Capacity >= minCap.Value);

      Console.WriteLine("--- Filtered Rooms ---");
      foreach (var room in filtered)
        DisplayRoom(room);
    }


    public void BookRoom(string guest, int roomNumber, int stayDays = 1)
    {
      var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
      if (room == null)
      {
        Console.WriteLine($"Room {roomNumber} not found!");
        return;
      }

      if (room.Status != RoomStatus.Available)
      {
        Console.WriteLine($"Room {roomNumber} is not available.");
        return;
      }

      room.Status = RoomStatus.Occupied;
      room.GuestName = guest;
      room.CheckInDate = DateTime.Now;
      room.CheckOutDate = DateTime.Now.AddDays(stayDays); // Here we set the check-out date
      decimal totalPrice = ApplyDiscounts(room, stayDays) * stayDays;
      _history.Log($"BOOKED | Room {roomNumber} booked for {guest}, total {totalPrice:C}");
      Console.WriteLine($"Room {roomNumber} booked successfully for {guest}. " + $"Check-out date: {room.CheckOutDate:yyyy-MM-dd}, Total price: {totalPrice:C}");
    }


    public void CheckoutRoom(int roomNumber)
    {
      var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
      if (room == null)
      {
        Console.WriteLine($"Room {roomNumber} not found!");
        return;
      }

      if (room.Status != RoomStatus.Occupied)
      {
        Console.WriteLine($"Room {roomNumber} is not occupied.");
        return;
      }

      int totalDays = (DateTime.Now - room.CheckInDate!.Value).Days;
      totalDays = totalDays == 0 ? 1 : totalDays;
      decimal totalPrice = totalDays * room.PricePerNight;

      _history.Log($"CHECKOUT | Room {roomNumber} checked out. Guest: {room.GuestName}, Total: {totalPrice:C}");
      Console.WriteLine($"Room {roomNumber} checked out successfully. Total price: {totalPrice:C}");

      room.ClearGuest();

      room.Status = RoomStatus.NeedsCleaning;

      _history.Log($"CLEANING_REQUIRED | Room {room.Number} now needs cleaning.");
      Console.WriteLine($"Room {room.Number} is now marked as NEEDS CLEANING.");

      _fileService.SaveRooms(_rooms);
    }

    public void MakeRoomUnavailable(int roomNumber)
    {
      var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
      if (room == null)
      {
        Console.WriteLine($"Room {roomNumber} not found!");
        return;
      }

      room.MakeUnavailable();
      _history.Log($"SYSTEM | Room {roomNumber} marked as unavailable.");
      Console.WriteLine($"Room {roomNumber} is now unavailable.");
    }

    public void AddRoom(int number, RoomType type, int capacity, decimal price, string description, string bedType, int bedCount, List<string>? amenities = null)
    {
      if (_rooms.Any(r => r.Number == number))
      {
        Console.WriteLine($"Room {number} already exists!");
        return;
      }

      var room = new Room(number, type, capacity, price, description, bedType, bedCount, amenities);
      _rooms.Add(room);
      _history.Log($"SYSTEM | Room {number} added.");
      Console.WriteLine($"Room {number} added successfully.");
    }

    public void SetCleaningInProgress(int roomNumber)
    {
      var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
      if (room == null)
      {
        Console.WriteLine("Room not found.");
        return;
      }
      if (room.Status != RoomStatus.NeedsCleaning)
      {
        Console.WriteLine("Room does not require cleaning.");
        return;
      }
      room.Status = RoomStatus.CleaningInProgress;
      _history.Log($"CLEANING_STARTED | Room {room.Number} is being cleaned.");
      Console.WriteLine($"Room {room.Number} is now markedas Cleaning In Progress.");

      _fileService.SaveRooms(_rooms);
    }

    public void MakeRoomAsCleaned(int roomNumber)
    {
      var room = _rooms.FirstOrDefault(r => r.Number == roomNumber);
      if (room == null)
      {
        Console.WriteLine("Room not found.");
        return;
      }
      if (room.Status != RoomStatus.CleaningInProgress && room.Status != RoomStatus.NeedsCleaning)
      {
        Console.WriteLine("Room is not in a cleanable state.");
        return;
      }
      room.Status = RoomStatus.Available;
      _history.Log($"CLEANED | Room {room.Number} has been cleaned and is available.");
      Console.WriteLine($"Room {room.Number} is now clean and available.");

      _fileService.SaveRooms(_rooms);
    }

    public void RemoveRoom(int number, User user)
    {
      var room = _rooms.FirstOrDefault(r => r.Number == number);
      if (room == null)
      {
        Console.WriteLine($"Room {number} not found!");
        return;
      }

      if (room.Status == RoomStatus.Occupied)
      {
        Console.WriteLine("Cannot remove a room that is currently occupied.");
        return;
      }

      _rooms.Remove(room);
      _history.Log($"SYSTEM | Room {number} removed by {user.Username}.");
      Console.WriteLine($"Room {number} removed successfully.");
    }

    public void UpdateRoomPrice(int number, decimal newPrice, User user)
    {
      var room = _rooms.FirstOrDefault(r => r.Number == number);
      if (room == null)
      {
        Console.WriteLine($"Room {number} not found!");
        return;
      }

      room.PricePerNight = newPrice;
      _history.Log($"SYSTEM | Room {number} price updated to {newPrice:C} by {user.Username}");
      Console.WriteLine($"Room {number} price updated to {newPrice:C}");
    }

    public void UpdateRoom(int number, User user)
    {
      var room = _rooms.FirstOrDefault(r => r.Number == number);
      if (room == null)
      {
        Console.WriteLine($"Room {number} not found!");
        return;
      }

      Console.WriteLine($"--- Update Room {number} ---");
      Console.WriteLine($"Current Type: {room.Type}");
      Console.WriteLine($"Current Capacity: {room.Capacity}");
      Console.WriteLine($"Current Price: {room.PricePerNight:C}");
      Console.WriteLine($"Current Description: {room.Description}");
      Console.WriteLine($"Current Bed Type: {room.BedType}");
      Console.WriteLine($"Current Bed Count: {room.BedCount}");
      Console.WriteLine();

      room.Type = InputHelper.GetEnum("Type (SingleBed/DoubleBed/Suite): ", room.Type);
      room.Capacity = InputHelper.GetInt("Capacity: ", 1);
      room.PricePerNight = InputHelper.GetDecimal("Price per night: ", 1);
      room.Description = InputHelper.GetString("Description: ", allowEmpty: true);
      room.BedType = InputHelper.GetString("Bed type: ", allowEmpty: true);
      room.BedCount = InputHelper.GetInt("Number of beds: ", 1);

      _history.Log($"SYSTEM | Room {number} updated by {user.Username}");
      Console.WriteLine($"Room {number} updated successfully.");
    }

    private void DisplayRoom(Room room)
    {
      Console.WriteLine($"Room {room.Number} | Type: {room.Type} | Status: {room.Status} | Capacity: {room.Capacity} | Price: {room.PricePerNight:C} | Beds: {room.BedCount} {room.BedType}");
    }
    public void AutoCheckoutExpiredRooms()
    {
      foreach (var room in _rooms)
      {
        if (room.Status == RoomStatus.Occupied && room.CheckOutDate.HasValue)
        {
          if (DateTime.Now >= room.CheckOutDate.Value)
          {
            Console.WriteLine($"Room {room.Number} auto-checked out (Guest: {room.GuestName})");
            _history.Log($"AUTO-CHECKOUT | Room {room.Number} auto-checked out. Guest: {room.GuestName}");
            room.ClearGuest();
          }
        }
      }
    }
  }
}
