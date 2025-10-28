

namespace Hotel.Services;

public class HistoryService
{
  private readonly string historyFile = "Data/history.txt";
  private readonly List<string> history = new();

  public HistoryService()
  {
    if (!Directory.Exists("Data"))
      Directory.CreateDirectory("Data");

    if (File.Exists(historyFile))
      history.AddRange(File.ReadAllLines(historyFile));
  }

  public void Log(string message)
  {
    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    var entry = $"[{timestamp}] {message}";
    history.Add(entry);
    File.AppendAllText(historyFile, entry + Environment.NewLine);
  }

  public void LogBooking(Room room, string guest) => Log($"Guest '{guest}' booked Room {room.roomNumber}.");
  public void LogCheckout(Room room, string guest) => Log($"Guest '{guest}' checked out from Room {room.roomNumber}.");
  public void LogRoomUnavailable(Room room) => Log($"Room {room.roomNumber} marked as temporarily unavailable.");

  public void ShowHistory()
  {
    Console.WriteLine("=== Booking History ===");
    if (history.Count == 0)
    {
      Console.WriteLine("No history found.");
      return;
    }

    foreach (var item in history)
      Console.WriteLine(item);
  }
}
