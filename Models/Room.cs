namespace Hotel.Models;

public enum RoomStatus
{
  Available,
  Occupied,
  Unavailable
}

public class Room
{
  public int roomNumber;
  public string? guestName;
  public RoomStatus status;

  public Room(int number)
  {
    roomNumber = number;
    guestName = null;
    status = RoomStatus.Available;
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
    return $"Room {roomNumber}: {status}" + (guestName != null ? $" ({guestName})" : "");
  }
}
