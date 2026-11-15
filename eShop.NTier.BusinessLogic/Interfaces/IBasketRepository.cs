using eShop.NTier.Domain.Entities;

namespace eShop.NTier.BusinessLogic.Interfaces;

public interface IBasketRepository
{
    Task<CustomerBasket?> GetBasketAsync(string buyerId);

    Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);

    Task<bool> DeleteBasketAsync(string buyerId);
}
