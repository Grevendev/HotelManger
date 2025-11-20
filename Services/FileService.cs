using System.Text.Json;
using Hotel.Models;

namespace Hotel.Services
{
  public class FileService : IRoomRepository, IUserRepository
  {
    private readonly string dataDir = "Data";
    private readonly string roomsFile;
    private readonly string usersFile;

    public FileService()
    {
      roomsFile = Path.Combine(dataDir, "rooms.json");
      usersFile = Path.Combine(dataDir, "users.txt");

      EnsureDataIntegrity();
    }

    // --- Ensure data directory and default files exist ---
    private void EnsureDataIntegrity()
    {
      if (!Directory.Exists(dataDir))
        Directory.CreateDirectory(dataDir);

      // --- Default rooms ---
      if (!File.Exists(roomsFile) || new FileInfo(roomsFile).Length == 0)
      {
        var defaultRooms = new List<Room>();
        for (int i = 1; i <= 10; i++)
          defaultRooms.Add(new Room(i));
        SaveRooms(defaultRooms);
      }

      // --- Default users ---
      if (!File.Exists(usersFile) || new FileInfo(usersFile).Length == 0)
      {
        var defaultUsers = new List<string>
                {
                    "admin,admin123,Admin",
                    "reception,reception123,Receptionist"
                };
        File.WriteAllLines(usersFile, defaultUsers);
      }
    }

    // --- Room Handling ---
    public List<Room> LoadRooms()
    {
      try
      {
        var json = File.ReadAllText(roomsFile);
        if (string.IsNullOrWhiteSpace(json))
          return new List<Room>();

        return JsonSerializer.Deserialize<List<Room>>(json) ?? new List<Room>();
      }
      catch
      {
        return new List<Room>();
      }
    }

    public void SaveRooms(List<Room> rooms)
    {
      var json = JsonSerializer.Serialize(rooms, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(roomsFile, json);
    }

    // --- User Handling ---
    public List<User> LoadUsers()
    {
      var users = new List<User>();
      if (!File.Exists(usersFile)) return users;

      foreach (var line in File.ReadAllLines(usersFile))
      {
        var parts = line.Split(',');
        if (parts.Length == 3 && Enum.TryParse(parts[2].Trim(), true, out UserRole role))
          users.Add(new User(parts[0].Trim(), parts[1].Trim(), role));
      }
      return users;
    }

    public void SaveUsers(List<User> users)
    {
      var lines = new List<string>();
      foreach (var user in users)
        lines.Add($"{user.Username},{user.Password},{user.Role}");
      File.WriteAllLines(usersFile, lines);
    }

    public List<Reservation> LoadReservations()
    {
      string path = "data/reservations.json";

      if (!File.Exists(path))
        return new List<Reservation>();

      string json = File.ReadAllText(path);
      return JsonSerializer.Deserialize<List<Reservation>>(json) ?? new List<Reservation>();
    }

    public void SaveReservations(List<Reservation> reservations)
    {
      File.WriteAllText("data/reservations.json",
          JsonSerializer.Serialize(reservations, new JsonSerializerOptions { WriteIndented = true }));
    }

  }
}
