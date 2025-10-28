namespace Hotel;

public class Room
{
  public int roomNumber;
  public RoomStatus status;
  public string? guestName;

  public Room(int number)
  {
    roomNumber = number;
    status = RoomStatus.Available;
    guestName = null;
  }

  public bool IsAvailable() => status == RoomStatus.Available;

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
    return $"Room {roomNumber} - {status}" + (guestName != null ? $" (Guest: {guestName})" : "");
  }
}

public enum RoomStatus
{
  Available,
  Occupied,
  Unavailable
}
