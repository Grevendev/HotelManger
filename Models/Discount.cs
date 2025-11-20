namespace Hotel.Models;

public enum DiscountType
{
  Percentage, //Example 10%
  FixedAmount //Example 100 kr
}

public class Discount
{
  public string Name { get; set; }
  public DiscountType Type { get; set; }
  public decimal Value { get; set; } //Procent or fixed value
  public RoomType? RoomType { get; set; } //Can be for all rooms or an specific room.
  public DateTime? StartDate { get; set; } //Campaign start
  public DateTime? EndDate { get; set; } //Campaign end
  public int? MinStayDays { get; set; } //Only if the booking is min X days

  public Discount(string name, DiscountType type, decimal value)
  {
    Name = name;
    Type = type;
    Value = value;
  }
  public bool IsActive(RoomType roomType, DateTime checkIn, int stayDays)
  {
    bool active = true;
    if (RoomType.HasValue && RoomType != roomType) active = false;
    if (StartDate.HasValue && checkIn < StartDate.Value) active = false;
    if (EndDate.HasValue && checkIn > EndDate.Value) active = false;
    if (MinStayDays.HasValue && stayDays < MinStayDays.Value) active = false;
    return active;
  }
  public decimal Apply(decimal price)
  {
    return Type == DiscountType.Percentage ? price * (1 - Value / 100m) : price - Value;
  }
}