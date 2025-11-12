// File: Models/Permissions.cs
namespace Hotel.Models
{
  public enum Permission
  {
    AddRoom,
    UpdateRoom,
    RemoveRoom,
    UpdateRoomPrice,
    BookRoom,
    CheckoutRoom,
    MakeRoomUnavailable,
    ViewHistory,
    FilterHistory,
    AddUser,
    ManagePermissions
  }
}
