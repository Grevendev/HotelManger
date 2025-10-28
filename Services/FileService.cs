using System.Text.Json;

namespace Hotel;

public class FileService : IRoomRepository, IUserRepository
{
  private string roomsFile = "Data/rooms.json";
  private string usersFile = "Data/users.txt";

  //IRoomRepository
  public List<Room> LoadRooms()
  {
    if (!File.Exists(roomsFile))
    {
      Directory.CreateDirectory("Data");
      var defaultRooms = new List<Room>();
      for (int i = 1; i <= 10; i++)
        defaultRooms.Add(new Room(i));

      SaveRooms(defaultRooms);
      return defaultRooms;
    }
    var json = File.ReadAllText(roomsFile);
    var options = new JsonSerializerOptions { IncludeFields = true };
    var data = JsonSerializer.Deserialize<List<Room>>(json, options);
    return data ?? new List<Room>();
  }
  public void SaveRooms(List<Room> rooms)
  {
    Directory.CreateDirectory("Data");
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      IncludeFields = true
    };
    var json = JsonSerializer.Serialize(rooms, options);
    File.WriteAllText(roomsFile, json);
  }
  //IUserRepository
  public List<User> LoadUsers()
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