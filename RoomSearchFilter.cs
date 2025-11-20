using Hotel.Models;

namespace Hotel.Services
{
  public class RoomSearchFilter
  {
    public decimal? MaxPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public RoomType? Type { get; set; }
    public int? MinCapacity { get; set; }
    public RoomStatus? Status { get; set; }
    public List<string>? RequiredAmenities { get; set; }
  }
}

