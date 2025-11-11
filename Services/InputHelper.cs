namespace Hotel.Services;

public static class InputHelper
{
  // --- Hämta positivt heltal ---
  public static int GetInt(string prompt, int minValue = 1, int maxValue = int.MaxValue, bool allowEmpty = false)
  {
    while (true)
    {
      Console.Write(prompt);
      string? input = Console.ReadLine()?.Trim();

      if (allowEmpty && string.IsNullOrWhiteSpace(input))
        return 0; // fallback för optional utan default

      if (int.TryParse(input, out int value) && value >= minValue && value <= maxValue)
        return value;

      Console.WriteLine($"Invalid input. Must be a number between {minValue} and {maxValue}.");
    }
  }

  public static int? GetOptionalInt(string prompt, int minValue = 0)
  {
    Console.Write(prompt);
    string? input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) return null;
    return int.TryParse(input, out int value) && value >= minValue ? value : GetOptionalInt(prompt, minValue);
  }

  // --- Hämta positiv decimal ---
  public static decimal GetDecimal(string prompt, decimal minValue = 0, decimal maxValue = decimal.MaxValue, bool allowEmpty = false)
  {
    while (true)
    {
      Console.Write(prompt);
      string? input = Console.ReadLine()?.Trim();

      if (allowEmpty && string.IsNullOrWhiteSpace(input))
        return 0;

      if (decimal.TryParse(input, out decimal value) && value >= minValue && value <= maxValue)
        return value;

      Console.WriteLine($"Invalid input. Must be a number between {minValue} and {maxValue}.");
    }
  }

  public static decimal? GetOptionalDecimal(string prompt, decimal minValue = 0)
  {
    Console.Write(prompt);
    string? input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) return null;
    return decimal.TryParse(input, out decimal value) && value >= minValue ? value : GetOptionalDecimal(prompt, minValue);
  }

  // --- Hämta sträng ---
  public static string GetString(string prompt, bool allowEmpty = false)
  {
    while (true)
    {
      Console.Write(prompt);
      string? input = Console.ReadLine()?.Trim();

      if (!string.IsNullOrWhiteSpace(input) || allowEmpty)
        return input ?? "";

      Console.WriteLine("Input cannot be empty.");
    }
  }

  public static string GetText(string prompt, string fallback)
  {
    Console.Write(prompt);
    string? input = Console.ReadLine()?.Trim();
    return !string.IsNullOrWhiteSpace(input) ? input : fallback;
  }

  // --- Hämta enum ---
  public static T GetEnum<T>(string prompt, T fallback) where T : struct, Enum
  {
    Console.Write(prompt);
    string? input = Console.ReadLine()?.Trim();

    if (!string.IsNullOrWhiteSpace(input) && Enum.TryParse(input, true, out T value))
      return value;

    return fallback;
  }

  public static T? GetOptionalEnum<T>(string prompt) where T : struct, Enum
  {
    Console.Write(prompt);
    string? input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) return null;
    return Enum.TryParse<T>(input, true, out T val) ? val : GetOptionalEnum<T>(prompt);
  }

  // --- Hämta datum ---
  public static DateTime GetDate(string prompt)
  {
    while (true)
    {
      Console.Write(prompt);
      string? input = Console.ReadLine()?.Trim();

      if (DateTime.TryParse(input, out DateTime date))
        return date;

      Console.WriteLine("Invalid date. Please enter a valid date (yyyy-MM-dd).");
    }
  }

  public static DateTime? GetOptionalDate(string prompt)
  {
    Console.Write(prompt);
    string? input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) return null;
    return DateTime.TryParse(input, out DateTime date) ? date : GetOptionalDate(prompt);
  }

  // --- Ja/Nej val ---
  public static bool GetYesNo(string prompt)
  {
    while (true)
    {
      Console.Write(prompt + " (y/n): ");
      string? input = Console.ReadLine()?.Trim().ToLower();

      if (input == "y" || input == "yes") return true;
      if (input == "n" || input == "no") return false;

      Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
    }
  }
}
