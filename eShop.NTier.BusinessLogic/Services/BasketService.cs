using eShop.NTier.BusinessLogic.DTOs.Basket;
using eShop.NTier.BusinessLogic.Interfaces;
using eShop.NTier.Domain.Entities;

namespace eShop.NTier.BusinessLogic.Services;

public class BasketService(IBasketRepository repository) : IBasketService
{
    public async Task<UpdateBasketDto?> GetBasketAsync(string buyerId)
    {
        var basket = await repository.GetBasketAsync(buyerId);

        if (basket is null)
        {
            return null;
        }

        return MapToDto(basket);
    }

    public async Task<UpdateBasketDto> UpdateBasketAsync(UpdateBasketDto basketDto)
    {
        if (string.IsNullOrWhiteSpace(basketDto.BuyerId))
        {
            throw new ArgumentException("BuyerId is required.");
        }

        if (basketDto.Items.Any(item => item.ProductId <= 0))
        {
            throw new ArgumentException("ProductId must be greater than zero.");
        }

        if (basketDto.Items.Any(item => item.Quantity < 1))
        {
            throw new ArgumentException("Invalid number of units.");
        }

        if (basketDto.Items.GroupBy(item => item.ProductId).Any(group => group.Count() > 1))
        {
            throw new ArgumentException("Duplicate ProductId is not allowed.");
        }

        var basket = new CustomerBasket
        {
            BuyerId = basketDto.BuyerId,
            Items = basketDto.Items.Select(item => new BasketItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };

        var updatedBasket = await repository.UpdateBasketAsync(basket);

        return MapToDto(updatedBasket);
    }

    public Task<bool> DeleteBasketAsync(string buyerId)
    {
        if (string.IsNullOrWhiteSpace(buyerId))
        {
            throw new ArgumentException("BuyerId is required.");
        }

        return repository.DeleteBasketAsync(buyerId);
    }

    private static UpdateBasketDto MapToDto(CustomerBasket basket)
    {
        return new UpdateBasketDto
        {
            BuyerId = basket.BuyerId,
            Items = basket.Items.Select(item => new BasketItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}
