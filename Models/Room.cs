using System.Text.Json.Serialization;

namespace Hotel.Models;

public enum RoomType
{
  SingleBed,
  DoubleBed,
  Suite
}

public enum RoomStatus
{
  Available,
  Occupied,
  Unavailable
}

public class Room
{
  [JsonInclude]
  public int roomNumber;
  [JsonInclude]
  public string? guestName;
  [JsonInclude]
  public DateTime? checkInDate;
  [JsonInclude]
  public RoomStatus status;
  [JsonInclude]
  public RoomType type;
  [JsonInclude]
  public int capacity;
  [JsonInclude]
  public decimal pricePerNight;
  [JsonInclude]
  public string description;
  [JsonInclude]
  public string bedType;
  [JsonInclude]
  public int bedCount;
  [JsonInclude]
  public List<string> amenities;

  public Room() { }

  public Room(int number, RoomType type = RoomType.SingleBed, int capacity = 1, decimal pricePerNight = 500, string description = null, string bedType = "Single", int bedCount = 1, List<string>? amenities = null)
  {
    if (number <= 0) throw new ArgumentException("Room number must be greater than 0");

    roomNumber = number;
    guestName = null;
    status = RoomStatus.Available;
    this.type = type;
    this.capacity = capacity;
    this.pricePerNight = pricePerNight;
    this.description = description ?? "No description provided.";
    this.bedType = bedType;
    this.bedCount = bedCount;
    this.amenities = amenities ?? new List<string>();
  }

  public void SetGuest(string guest)
  {
    guestName = guest;
    status = RoomStatus.Occupied;
  }

  public void ClearGuest()
  {
    guestName = null;
    checkInDate = null;
    status = RoomStatus.Available;
  }

  public void MakeUnavailable()
  {
    guestName = null;
    status = RoomStatus.Unavailable;
  }


  public override string ToString()
  {
    string amenityList = amenities.Count > 0 ? string.Join(", ", amenities) : "None";
    return $"Room {roomNumber}: {status}, {type}, Capacity: {capacity}, Beds: {bedCount} ({bedType}), Price: {pricePerNight:C}, Amenities: {amenityList}" +
           (guestName != null ? $" ({guestName})" : "");
  }



}
