namespace Hotel;

public class HistoryService
{
  private readonly string historyFile = "Data/history.txt";
  private readonly List<string> history = new();

  public HistoryService()
  {
    if (!Directory.Exists("Data"))
      Directory.CreateDirectory("Data");

    if (File.Exists(historyFile))
    {
      history.AddRange(File.ReadAllLines(historyFile));
    }
  }

  public void Log(string message)
  {
    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    var entry = $"[{timestamp}] {message}";
    history.Add(entry);
    File.AppendAllText(historyFile, entry + Environment.NewLine);
  }

  public void ShowHistory()
  {
    Console.WriteLine("=== Booking History ===");
    if (history.Count == 0)
    {
      Console.WriteLine("No history found.");
      return;
    }

    foreach (var item in history)
    {
      Console.WriteLine(item);
    }
  }
}
