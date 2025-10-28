namespace Hotel;

public class HistoryService
{
  private List<string> history = new();

  public void LogBooking(Room room, string guest)
  {
    string entry = $"{DateTime.Now}: Guest {guest} checked in to room {room.roomNumber}";
    history.Add(entry);
  }
  public void LogCheckout(Room room, string guest)
  {
    string entry = $"{DateTime.Now}: Guest {guest} checked out from room {room.roomNumber}";
    history.Add(entry);
  }
  public void ShowHistory()
  {
    Console.WriteLine("=== Booking History ===");
    foreach (var entry in history) Console.WriteLine(entry);
  }
}