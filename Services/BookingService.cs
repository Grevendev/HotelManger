using System;
using System.Collections.Generic;

namespace Hotel.Services;

public class BookingService
{
  private readonly List<Room> rooms;
  private readonly IRoomRepository repository;
  private readonly HistoryService history;

  public BookingService(IRoomRepository repo, HistoryService historyService)
  {
    repository = repo ?? throw new ArgumentNullException(nameof(repo));
    history = historyService ?? throw new ArgumentNullException(nameof(historyService));
    rooms = repository.LoadRooms() ?? new List<Room>();
  }

  // --- Display Methods ---
  public void ShowAllRooms()
  {
    Console.WriteLine("=== All Rooms ===");
    foreach (var room in rooms)
      Console.WriteLine(room);
  }

  public void ShowAvailableRooms()
  {
    Console.WriteLine("=== Available Rooms ===");
    foreach (var room in rooms)
    {
      if (room.status == RoomStatus.Available)
        Console.WriteLine(room);
    }
  }

  public void ShowUnavailableRooms()
  {
    Console.WriteLine("=== Unavailable Rooms ===");
    foreach (var room in rooms)
    {
      if (room.status == RoomStatus.Unavailable)
        Console.WriteLine(room);
    }
  }

  // --- Booking ---
  public void BookRoom(string guest, int number)
  {
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
      Console.WriteLine("Room not found!");
      return;
    }

    if (room.status != RoomStatus.Available)
    {
      Console.WriteLine("Room not available!");
      return;
    }

    room.SetGuest(guest);
    repository.SaveRooms(rooms);
    history.Log($"Guest '{guest}' booked Room {number}.");
    Console.WriteLine($"Guest {guest} successfully booked Room {number}.");
  }

  // --- Checkout ---
  public void CheckoutRoom(int number)
  {
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
      Console.WriteLine("Room not found!");
      return;
    }

    if (room.status != RoomStatus.Occupied)
    {
      Console.WriteLine("Room is not currently occupied.");
      return;
    }

    var guest = room.guestName ?? "Unknown";
    room.ClearGuest();
    repository.SaveRooms(rooms);
    history.Log($"Guest '{guest}' checked out from Room {number}.");
    Console.WriteLine($"Guest {guest} checked out from Room {number}.");
  }

  // --- Unavailable ---
  public void MakeRoomUnavailable(int number)
  {
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
      Console.WriteLine("Room not found!");
      return;
    }

    room.MakeUnavailable();
    repository.SaveRooms(rooms);
    history.Log($"Room {number} marked as temporarily unavailable.");
    Console.WriteLine($"Room {number} marked as unavailable.");
  }
}
