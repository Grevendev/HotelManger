using System.Text.Json;
using Hotel;

namespace Hotel.Services;

public class FileService : IRoomRepository
{
  private readonly string roomsFile = "Data/rooms.json";
  private readonly string usersFile = "Data/users.txt";

  public List<Room> LoadRooms()
  {
    try
    {
      if (!File.Exists(roomsFile) || new FileInfo(roomsFile).Length == 0)
        return CreateDefaultRooms();

      var json = File.ReadAllText(roomsFile);

      if (string.IsNullOrWhiteSpace(json))
        return CreateDefaultRooms();

      var rooms = JsonSerializer.Deserialize<List<Room>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
      return rooms ?? CreateDefaultRooms();
    }
    catch
    {
      // Om filen är skadad → skriv över med default
      return CreateDefaultRooms();
    }
  }

  public void SaveRooms(List<Room> rooms)
  {
    var json = JsonSerializer.Serialize(rooms, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(roomsFile, json);
  }

  private List<Room> CreateDefaultRooms()
  {
    var defaultRooms = new List<Room>();
    for (int i = 1; i <= 10; i++)
      defaultRooms.Add(new Room(i));

    SaveRooms(defaultRooms);
    return defaultRooms;
  }

  public List<User> LoadUsers()
  {
    var users = new List<User>();
    if (!File.Exists(usersFile)) return users;

    foreach (var line in File.ReadAllLines(usersFile))
    {
      var parts = line.Split(',');
      if (parts.Length == 3)
      {
        if (Enum.TryParse(parts[2].Trim(), out UserRole role))
          users.Add(new User(parts[0].Trim(), parts[1].Trim(), role));
      }
    }
    return users;
  }

  public void SaveUsers(List<User> users)
  {
    var lines = users.Select(u => $"{u.Username},{u.Password},{u.Role}");
    File.WriteAllLines(usersFile, lines);
  }
}
