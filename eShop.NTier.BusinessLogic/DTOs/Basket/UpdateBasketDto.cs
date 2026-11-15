namespace eShop.NTier.BusinessLogic.DTOs.Basket;

public class UpdateBasketDto
{
    public string BuyerId { get; set; } = string.Empty;

    public List<BasketItemDto> Items { get; set; } = [];
}
