namespace Hotel.Models
{
  public static class Permissions
  {
    // Room management
    public const string AddRoom = "AddRoom";
    public const string UpdateRoom = "UpdateRoom";
    public const string RemoveRoom = "RemoveRoom";
    public const string UpdateRoomPrice = "UpdateRoomPrice";

    // Booking management
    public const string BookRoom = "BookRoom";
    public const string CheckoutRoom = "CheckoutRoom";
    public const string MakeRoomUnavailable = "MakeRoomUnavailable";

    // History and users
    public const string ViewHistory = "ViewHistory";
    public const string FilterHistory = "FilterHistory";
    public const string AddUser = "AddUser";
  }
}
