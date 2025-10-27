using Hotel.Models;

namespace Hotel;

public class Room
{
  public int roomNumber;
  public string guestName;
  public RoomStatus status;

  public Room(int number)
  {
    roomNumber = number;
    status = RoomStatus.Available;
    guestName = null;
  }
  public void SetGuest(string name)
  {
    guestName = name;
    status = RoomStatus.Occupied;
  }
  public void ClearGuest()
  {
    guestName = null;
    status = RoomStatus.Available;
  }
  public void MakeUnavailable()
  {
    status = RoomStatus.Unavailable;
  }
  public override string ToString()
  {
    string info = $"Room {roomNumber} - {status}";
    if (guestName != null)
      info += $" (Guest: {guestName})";
    return info;
  }
}