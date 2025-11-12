using System.Text.Json.Serialization;

namespace Hotel.Models
{
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
    [JsonInclude] public int Number { get; set; }
    [JsonInclude] public string? GuestName { get; set; }
    [JsonInclude] public DateTime? CheckInDate { get; set; }
    [JsonInclude] public RoomStatus Status { get; set; } = RoomStatus.Available;
    [JsonInclude] public RoomType Type { get; set; } = RoomType.SingleBed;
    [JsonInclude] public int Capacity { get; set; } = 1;
    [JsonInclude] public decimal PricePerNight { get; set; } = 500;
    [JsonInclude] public string Description { get; set; } = "No description provided.";
    [JsonInclude] public string BedType { get; set; } = "Single";
    [JsonInclude] public int BedCount { get; set; } = 1;
    [JsonInclude] public List<string> Amenities { get; set; } = new();

    public Room() { }

    public Room(int number, RoomType type = RoomType.SingleBed, int capacity = 1, decimal pricePerNight = 500,
        string? description = null, string bedType = "Single", int bedCount = 1, List<string>? amenities = null)
    {
      if (number <= 0) throw new ArgumentException("Room number must be greater than 0");
      Number = number;
      Type = type;
      Capacity = capacity;
      PricePerNight = pricePerNight;
      Description = description ?? "No description provided.";
      BedType = bedType;
      BedCount = bedCount;
      Amenities = amenities ?? new List<string>();
      Status = RoomStatus.Available;
      GuestName = null;
      CheckInDate = null;
    }

    public void SetGuest(string guest)
    {
      GuestName = guest;
      Status = RoomStatus.Occupied;
      CheckInDate = DateTime.Now;
    }

    public void ClearGuest()
    {
      GuestName = null;
      CheckInDate = null;
      Status = RoomStatus.Available;
    }

    public void MakeUnavailable()
    {
      GuestName = null;
      CheckInDate = null;
      Status = RoomStatus.Unavailable;
    }

    public override string ToString()
    {
      string amenityList = Amenities.Count > 0 ? string.Join(", ", Amenities) : "None";
      return $"Room {Number}: {Status}, {Type}, Capacity: {Capacity}, Beds: {BedCount} ({BedType}), Price: {PricePerNight:C}, Amenities: {amenityList}" +
             (GuestName != null ? $" (Guest: {GuestName})" : "");
    }
  }
}
