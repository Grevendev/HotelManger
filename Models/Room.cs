using System.Text.Json.Serialization;

namespace Hotel.Models;

public enum RoomType
{
  SingleBed,
  DoubleBed,
  Suite
}

public enum RoomStatus
{
  Available,
  Occupied,
  Unavailable
}

public class Room
{
  [JsonInclude]
  public int roomNumber;
  [JsonInclude]
  public string? guestName;
  [JsonInclude]
  public RoomStatus status;
  [JsonInclude]
  public RoomType type;
  [JsonInclude]
  public int capacity;

  public Room() { }

  public Room(int number, RoomType type = RoomType.SingleBed, int capacity = 1)
  {
    if (number <= 0) throw new ArgumentException("Room number must be greater than 0");
    roomNumber = number;
    guestName = null;
    status = RoomStatus.Available;
    this.type = type;
    this.capacity = capacity;
  }

  public void SetGuest(string guest)
  {
    guestName = guest;
    status = RoomStatus.Occupied;
  }

  public void ClearGuest()
  {
    guestName = null;
    status = RoomStatus.Available;
  }

  public void MakeUnavailable()
  {
    guestName = null;
    status = RoomStatus.Unavailable;
  }

  public override string ToString()
  {
    return $"Room {roomNumber}: {status}, {type}, Capacity: {capacity}" + (guestName != null ? $" ({guestName})" : "");
  }
}
