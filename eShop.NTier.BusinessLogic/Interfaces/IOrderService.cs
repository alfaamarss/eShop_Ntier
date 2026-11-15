using eShop.NTier.BusinessLogic.DTOs.Ordering;

namespace eShop.NTier.BusinessLogic.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CheckoutAsync(string buyerId, CheckoutRequestDto request);
    Task<IReadOnlyList<OrderDto>> GetByBuyerIdAsync(string buyerId);
    Task<OrderDto?> GetDetailAsync(string buyerId, int orderId);
    Task<OrderDto?> CancelAsync(string buyerId, int orderId);
}
