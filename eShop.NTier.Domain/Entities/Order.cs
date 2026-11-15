namespace eShop.NTier.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string BuyerId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Submitted;

    public string RecipientName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public List<OrderItem> OrderItems { get; set; } = [];
}
