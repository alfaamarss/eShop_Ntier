namespace eShop.NTier.Domain.Entities;

public class BasketItem
{
    public int Id { get; set; }

    public string BuyerId { get; set; } = string.Empty;

    public CustomerBasket CustomerBasket { get; set; } = null!;

    public int ProductId { get; set; }

    public int Quantity { get; set; }
}
