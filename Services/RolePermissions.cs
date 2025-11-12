// File: Services/RolePermissions.cs
using Hotel.Models;

namespace Hotel.Services
{
  public static class RolePermissions
  {
    // Define which permissions each role has permanently
    public static readonly Dictionary<UserRole, List<Permission>> RolePermissionMap = new()
        {
            { UserRole.Admin, new List<Permission>
                {
                    Permission.AddRoom,
                    Permission.UpdateRoom,
                    Permission.RemoveRoom,
                    Permission.UpdateRoomPrice,
                    Permission.BookRoom,
                    Permission.CheckoutRoom,
                    Permission.MakeRoomUnavailable,
                    Permission.ViewHistory,
                    Permission.FilterHistory,
                    Permission.AddUser,
                    Permission.ManagePermissions
                }
            },
            { UserRole.Receptionist, new List<Permission>
                {
                    Permission.BookRoom,
                    Permission.CheckoutRoom,
                    Permission.MakeRoomUnavailable,
                    Permission.ViewHistory,
                    Permission.FilterHistory
                }
            }
        };
  }
}
