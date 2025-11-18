namespace Hotel.Models
{
  public class Reservation
  {
    public int RoomNumber { get; set; }
    public string Guest { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }

    public Reservation(int roomNumber, string guest, DateTime checkIn, DateTime checkOut)
    {
      RoomNumber = roomNumber;
      Guest = guest;
      CheckInDate = checkIn;
      CheckOutDate = checkOut;
    }
  }
}