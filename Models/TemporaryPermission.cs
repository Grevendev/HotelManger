// File: Models/TemporaryPermission.cs
namespace Hotel.Models
{
  public class TemporaryPermission
  {
    public Permission Permission { get; set; }
    public DateTime Expiry { get; set; }

    public TemporaryPermission(Permission permission, DateTime expiry)
    {
      Permission = permission;
      Expiry = expiry;
    }

    public bool IsActive => Expiry > DateTime.Now;
  }
}
