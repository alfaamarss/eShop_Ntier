using System.Data;
using eShop.NTier.BusinessLogic.Interfaces;
using eShop.NTier.DataAccess.Context;
using eShop.NTier.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eShop.NTier.DataAccess.Repositories;

public class OrderRepository(CatalogDbContext context) : IOrderRepository
{
    public async Task<Order> CheckoutAsync(Order order)
    {
        await using var transaction =
            await context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        try
        {
            var basket = await context.CustomerBaskets
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.BuyerId == order.BuyerId);

            if (basket is null || basket.Items.Count == 0)
                throw new InvalidOperationException(
                    "Basket is empty or does not exist.");

            var productIds = basket.Items
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();

            var products = await context.CatalogItems
                .Where(x => productIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);

            if (products.Count != productIds.Count)
                throw new InvalidOperationException(
                    "One or more products no longer exist.");

            decimal total = 0;

            foreach (var basketItem in basket.Items)
            {
                var product = products[basketItem.ProductId];

                if (basketItem.Quantity <= 0)
                    throw new InvalidOperationException(
                        "Invalid product quantity.");

                if (product.AvailableStock < basketItem.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for product: {product.Name}.");

                product.RemoveStock(basketItem.Quantity);

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = basketItem.Quantity,
                    PictureFileName = product.PictureFileName
                };

                order.OrderItems.Add(orderItem);
                total += orderItem.Subtotal;
            }

            order.Total = total;

            context.Orders.Add(order);
            context.CustomerBaskets.Remove(basket);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return order;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IReadOnlyList<Order>> GetByBuyerIdAsync(
        string buyerId)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(x => x.OrderItems)
            .Where(x => x.BuyerId == buyerId)
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync();
    }

    public Task<Order?> GetDetailAsync(string buyerId, int orderId)
    {
        return context.Orders
            .AsNoTracking()
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(
                x => x.Id == orderId && x.BuyerId == buyerId);
    }

    public async Task<Order?> CancelAsync(string buyerId, int orderId)
    {
        await using var transaction =
            await context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        try
        {
            var order = await context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(
                    x => x.Id == orderId && x.BuyerId == buyerId);

            if (order is null)
                return null;

            if (order.Status != OrderStatus.Submitted)
                throw new InvalidOperationException(
                    "Only submitted orders can be cancelled.");

            var productIds = order.OrderItems
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();

            var products = await context.CatalogItems
                .Where(x => productIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);

            foreach (var item in order.OrderItems)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    throw new InvalidOperationException(
                        $"Product {item.ProductId} no longer exists.");

                product.AddStock(item.Quantity);
            }

            order.Status = OrderStatus.Cancelled;

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return order;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
