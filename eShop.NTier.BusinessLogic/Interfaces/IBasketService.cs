using eShop.NTier.BusinessLogic.DTOs.Basket;

namespace eShop.NTier.BusinessLogic.Interfaces;

public interface IBasketService
{
    Task<UpdateBasketDto?> GetBasketAsync(string buyerId);

    Task<UpdateBasketDto> UpdateBasketAsync(UpdateBasketDto basket);

    Task<bool> DeleteBasketAsync(string buyerId);
}
