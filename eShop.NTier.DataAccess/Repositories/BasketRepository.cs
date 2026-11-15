using eShop.NTier.BusinessLogic.Interfaces;
using eShop.NTier.DataAccess.Context;
using eShop.NTier.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eShop.NTier.DataAccess.Repositories;

public class BasketRepository(CatalogDbContext context) : IBasketRepository
{
    public async Task<CustomerBasket?> GetBasketAsync(string buyerId)
    {
        return await context.CustomerBaskets
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.BuyerId == buyerId);
    }

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        var existingBasket = await context.CustomerBaskets
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.BuyerId == basket.BuyerId);

        if (existingBasket is null)
        {
            context.CustomerBaskets.Add(basket);
        }
        else
        {
            context.BasketItems.RemoveRange(existingBasket.Items);

            existingBasket.Items = basket.Items.Select(item => new BasketItem
            {
                BuyerId = basket.BuyerId,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList();
        }

        await context.SaveChangesAsync();

        return await GetBasketAsync(basket.BuyerId)
            ?? throw new InvalidOperationException("Basket could not be retrieved after update.");
    }

    public async Task<bool> DeleteBasketAsync(string buyerId)
    {
        var basket = await context.CustomerBaskets
            .FirstOrDefaultAsync(x => x.BuyerId == buyerId);

        if (basket is null)
        {
            return false;
        }

        context.CustomerBaskets.Remove(basket);
        await context.SaveChangesAsync();

        return true;
    }
}
