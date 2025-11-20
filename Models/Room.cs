// File: Models/Room.cs
using System;
using System.Collections.Generic;
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
    Unavailable,
    BookedInAdvance
  }

  public class Room
  {
    [JsonInclude] public int Number { get; set; }
    [JsonInclude] public string? GuestName { get; set; }
    [JsonInclude] public DateTime? CheckInDate { get; set; }
    [JsonInclude] public DateTime? CheckOutDate { get; set; }   // <<--- NY
    [JsonInclude] public RoomStatus Status { get; set; } = RoomStatus.Available;
    [JsonInclude] public RoomType Type { get; set; } = RoomType.SingleBed;
    [JsonInclude] public int Capacity { get; set; } = 1;
    [JsonInclude] public decimal PricePerNight { get; set; } = 500m;
    [JsonInclude] public string Description { get; set; } = "No description provided.";
    [JsonInclude] public string BedType { get; set; } = "Single";
    [JsonInclude] public int BedCount { get; set; } = 1;
    [JsonInclude] public List<string> Amenities { get; set; } = new();

    public Room() { } // parameterless for serializer

    public Room(int number,
                RoomType type = RoomType.SingleBed,
                int capacity = 1,
                decimal pricePerNight = 500m,
                string? description = null,
                string bedType = "Single",
                int bedCount = 1,
                List<string>? amenities = null)
    {
      if (number <= 0) throw new ArgumentException("Room number must be greater than 0", nameof(number));

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
      CheckOutDate = null;
    }

    // Sätt gäst och räknar fram ett checkout-datum utifrån stayDays
    public void SetGuest(string guest, int stayDays = 1)
    {
      if (string.IsNullOrWhiteSpace(guest)) throw new ArgumentException("Guest name cannot be empty.", nameof(guest));
      if (stayDays <= 0) stayDays = 1;

      GuestName = guest;
      CheckInDate = DateTime.Now;
      CheckOutDate = DateTime.Now.AddDays(stayDays);
      Status = RoomStatus.Occupied;
    }

    public void ClearGuest()
    {
      GuestName = null;
      CheckInDate = null;
      CheckOutDate = null;
      Status = RoomStatus.Available;
    }

    public void MakeUnavailable()
    {
      GuestName = null;
      CheckInDate = null;
      CheckOutDate = null;
      Status = RoomStatus.Unavailable;
    }

    public override string ToString()
    {
      string amenityList = Amenities != null && Amenities.Count > 0 ? string.Join(", ", Amenities) : "None";
      string guest = string.IsNullOrWhiteSpace(GuestName) ? "" : $" Guest: {GuestName}";
      string checkout = CheckOutDate.HasValue ? $" Check-out: {CheckOutDate.Value:yyyy-MM-dd}" : "";
      return $"Room {Number}: {Status}, {Type}, Capacity: {Capacity}, Beds: {BedCount} ({BedType}), Price: {PricePerNight:C}, Amenities: {amenityList}{guest}{checkout}";
    }
  }
}
