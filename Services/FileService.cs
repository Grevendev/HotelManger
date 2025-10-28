using System.Text.Json;

namespace Hotel;

public static class FileService
{
  private static string roomsFile = "Data/rooms.json";
  private static string usersFile = "Data/users.txt";
  public static List<Room> LoadRooms()
  {
    if (!File.Exists(roomsFile))
    {
      var defaultRooms = new List<Room>();
      for (int i = 1; i <= 10; i++)
        defaultRooms.Add(new Room(i));

      SaveRooms(defaultRooms);
      return defaultRooms;
    }
    var json = File.ReadAllText(roomsFile);
    var data = JsonSerializer.Deserialize<List<Room>>(json);
    return data ?? new List<Room>();
  }
  public static void SaveRooms(List<Room> rooms)
  {
    var json = JsonSerializer.Serialize(rooms, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(roomsFile, json);
  }
  public static List<User> LoadUser()
  {
    var users = new List<User>();
    if (!File.Exists(usersFile)) return users;

    foreach (var line in File.ReadAllLines(usersFile))
    {
      var parts = line.Split(',');
      if (parts.Length == 2)
      {
        users.Add(new User(parts[0].Trim(), parts[1].Trim()));
      }
    }
    return users;
  }
}