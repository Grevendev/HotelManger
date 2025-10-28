using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using Hotel.Models;

namespace Hotel.Services;

public class FileService : IRoomRepository
{
  private readonly string roomsFile = "Data/rooms.json";
  private readonly string usersFile = "Data/users.txt";

  public FileService()
  {
    if (!Directory.Exists("Data"))
      Directory.CreateDirectory("Data");
  }

  // ---------------------
  // Rooms
  // ---------------------
  public List<Room> LoadRooms()
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

  public void SaveRooms(List<Room> rooms)
  {
    var json = JsonSerializer.Serialize(rooms, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(roomsFile, json);
  }

  // ---------------------
  // Users
  // ---------------------
  public List<User> LoadUsers()
  {
    var users = new List<User>();
    if (!File.Exists(usersFile))
      return users;

    foreach (var line in File.ReadAllLines(usersFile))
    {
      var parts = line.Split(',');
      if (parts.Length >= 2)
      {
        var role = parts.Length == 3 && Enum.TryParse<UserRole>(parts[2].Trim(), out var parsedRole)
            ? parsedRole
            : UserRole.Receptionist;

        users.Add(new User(parts[0].Trim(), parts[1].Trim(), role));
      }
    }
    return users;
  }

  public void SaveUsers(List<User> users)
  {
    var lines = new List<string>();
    foreach (var user in users)
    {
      lines.Add($"{user.Username},{user.Password},{user.Role}");
    }
    File.WriteAllLines(usersFile, lines);
  }
}
