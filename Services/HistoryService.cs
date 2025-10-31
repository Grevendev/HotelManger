using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hotel.Models;

namespace Hotel.Services;

public class HistoryService
{
  private readonly string historyFile;

  public HistoryService(string historyPath = "Data/history.txt")
  {
    historyFile = historyPath;
    if (!File.Exists(historyFile))
      File.WriteAllText(historyFile, "");
  }

  // --- Grundläggande loggfunktion ---
  public void Log(string message, string type = "INFO")
  {
    var entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {type.ToUpper()} | {message}";
    File.AppendAllText(historyFile, entry + Environment.NewLine);
  }

  // --- Loggar användarhändelser ---
  public void LogLogin(User user)
  {
    Log($"User '{user.Username}' logged in ({user.Role})", "LOGIN");
  }

  public void LogLogout(User user)
  {
    Log($"User '{user.Username}' logged out", "LOGOUT");
  }

  // --- Läser hela historiken ---
  public List<string> LoadHistory()
  {
    if (!File.Exists(historyFile)) return new List<string>();
    return new List<string>(File.ReadAllLines(historyFile));
  }

  // --- Filtrerar historiken efter datum/nyckelord ---
  public List<string> FilterHistory(DateTime? from = null, DateTime? to = null, string? keyword = null)
  {
    var history = LoadHistory();

    var filtered = history
        .Where(line =>
        {
          if (!DateTime.TryParse(line.Split('|')[0].Trim(), out var date))
            return false;

          bool dateMatch = (!from.HasValue || date >= from) && (!to.HasValue || date <= to);
          bool keywordMatch = string.IsNullOrWhiteSpace(keyword) || line.Contains(keyword, StringComparison.OrdinalIgnoreCase);
          return dateMatch && keywordMatch;
        })
        .ToList();

    return filtered;
  }

  // --- Visar historik i färg ---
  public void DisplayHistory(List<string>? entries = null)
  {
    entries ??= LoadHistory();
    Console.WriteLine("=== Hotel History Log ===");

    foreach (var line in entries)
    {
      if (line.Contains("BOOKED", StringComparison.OrdinalIgnoreCase))
        Console.ForegroundColor = ConsoleColor.Green;
      else if (line.Contains("CHECKOUT", StringComparison.OrdinalIgnoreCase))
        Console.ForegroundColor = ConsoleColor.Yellow;
      else if (line.Contains("UNAVAILABLE", StringComparison.OrdinalIgnoreCase))
        Console.ForegroundColor = ConsoleColor.Gray;
      else if (line.Contains("LOGIN", StringComparison.OrdinalIgnoreCase))
        Console.ForegroundColor = ConsoleColor.Cyan;
      else if (line.Contains("LOGOUT", StringComparison.OrdinalIgnoreCase))
        Console.ForegroundColor = ConsoleColor.Blue;
      else if (line.Contains("ERROR", StringComparison.OrdinalIgnoreCase))
        Console.ForegroundColor = ConsoleColor.Red;
      else
        Console.ForegroundColor = ConsoleColor.White;

      Console.WriteLine(line);
    }

    Console.ResetColor();
  }
}
