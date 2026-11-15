using eShop.NTier.Domain.Entities;

namespace eShop.NTier.BusinessLogic.Interfaces;

public interface IOrderRepository
{
    Task<Order> CheckoutAsync(Order order);
    Task<IReadOnlyList<Order>> GetByBuyerIdAsync(string buyerId);
    Task<Order?> GetDetailAsync(string buyerId, int orderId);
    Task<Order?> CancelAsync(string buyerId, int orderId);
}
