using System;
using System.Collections.Generic;
using Hotel.Models;

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

  // --- Visa rum ---
  public void ShowAllRooms()
  {
    Console.WriteLine("\n=== All Rooms ===");
    foreach (var room in rooms)
      DisplayRoomStatus(room);
    Console.ResetColor();
  }

  public void ShowAvailableRooms()
  {
    Console.WriteLine("=== Available Rooms ===");
    foreach (var room in rooms)
      if (room.status == RoomStatus.Available)
        DisplayRoomStatus(room);
    Console.ResetColor();
  }

  public void ShowUnavailableRooms()
  {
    Console.WriteLine("=== Unavailable Rooms ===");
    foreach (var room in rooms)
      if (room.status == RoomStatus.Unavailable)
        DisplayRoomStatus(room);
    Console.ResetColor();
  }

  private void DisplayRoomStatus(Room room)
  {
    switch (room.status)
    {
      case RoomStatus.Available:
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Room {room.roomNumber}: Available");
        break;
      case RoomStatus.Occupied:
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Room {room.roomNumber}: Occupied by {room.guestName}");
        break;
      case RoomStatus.Unavailable:
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Room {room.roomNumber}: Unavailable");
        break;
    }
  }

  // --- Bokning ---
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
    history.Log($"BOOKED | Guest '{guest}' booked Room {number}");
    Console.WriteLine($"Guest {guest} successfully booked Room {number}.");
  }

  // --- Utcheckning ---
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
    history.Log($"CHECKOUT | Guest '{guest}' checked out from Room {number}");
    Console.WriteLine($"Guest {guest} checked out from Room {number}.");
  }

  // --- Markera otillgänglig ---
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
    history.Log($"UNAVAILABLE | Room {number} marked as temporarily unavailable.");
    Console.WriteLine($"Room {number} marked as unavailable.");
  }

  // --- Lägg till rum ---
  public void AddRoom(int number)
  {
    if (rooms.Exists(r => r.roomNumber == number))
    {
      Console.WriteLine($"Room {number} already exists!");
      return;
    }

    var newRoom = new Room(number);
    rooms.Add(newRoom);
    repository.SaveRooms(rooms);
    history.Log($"SYSTEM | Room {number} added to the hotel.");
    Console.WriteLine($"Room {number} successfully added.");
  }

  // --- Ta bort rum ---
  public void RemoveRoom(int number)
  {
    var room = rooms.Find(r => r.roomNumber == number);
    if (room == null)
    {
      Console.WriteLine($"Room {number} does not exist!");
      return;
    }

    if (room.status == RoomStatus.Occupied)
    {
      Console.WriteLine($"Cannot remove Room {number} because it is currently occupied.");
      return;
    }

    rooms.Remove(room);
    repository.SaveRooms(rooms);
    history.Log($"SYSTEM | Room {number} removed from the hotel.");
    Console.WriteLine($"Room {number} successfully removed.");
  }
}
