using eShop.NTier.BusinessLogic.DTOs.Ordering;
using eShop.NTier.BusinessLogic.Interfaces;
using eShop.NTier.Domain.Entities;

namespace eShop.NTier.BusinessLogic.Services;

public class OrderService(IOrderRepository repository) : IOrderService
{
    public async Task<OrderDto> CheckoutAsync(
        string buyerId,
        CheckoutRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(buyerId))
            throw new ArgumentException("BuyerId is required.");

        ValidateAddress(request);

        var order = new Order
        {
            BuyerId = buyerId.Trim(),
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Submitted,
            RecipientName = request.RecipientName.Trim(),
            Street = request.Street.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim(),
            Country = request.Country.Trim(),
            ZipCode = request.ZipCode.Trim()
        };

        var savedOrder = await repository.CheckoutAsync(order);
        return MapToDto(savedOrder);
    }

    public async Task<IReadOnlyList<OrderDto>> GetByBuyerIdAsync(
        string buyerId)
    {
        if (string.IsNullOrWhiteSpace(buyerId))
            throw new ArgumentException("BuyerId is required.");

        var orders = await repository.GetByBuyerIdAsync(buyerId);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDto?> GetDetailAsync(
        string buyerId, int orderId)
    {
        if (string.IsNullOrWhiteSpace(buyerId))
            throw new ArgumentException("BuyerId is required.");

        if (orderId <= 0)
            throw new ArgumentException("Invalid OrderId.");

        var order = await repository.GetDetailAsync(buyerId, orderId);
        return order is null ? null : MapToDto(order);
    }

    public async Task<OrderDto?> CancelAsync(
        string buyerId, int orderId)
    {
        if (string.IsNullOrWhiteSpace(buyerId))
            throw new ArgumentException("BuyerId is required.");

        if (orderId <= 0)
            throw new ArgumentException("Invalid OrderId.");

        var order = await repository.CancelAsync(buyerId, orderId);
        return order is null ? null : MapToDto(order);
    }

    private static void ValidateAddress(CheckoutRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateField(request.RecipientName, 100, "RecipientName");
        ValidateField(request.Street, 250, "Street");
        ValidateField(request.City, 100, "City");
        ValidateField(request.State, 100, "State");
        ValidateField(request.Country, 100, "Country");
        ValidateField(request.ZipCode, 20, "ZipCode");
    }

    private static void ValidateField(
        string? value, int maxLength, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{field} is required.");

        if (value.Trim().Length > maxLength)
            throw new ArgumentException(
                $"{field} maximum length is {maxLength} characters.");
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            BuyerId = order.BuyerId,
            OrderDate = order.OrderDate,
            Status = order.Status.ToString(),
            RecipientName = order.RecipientName,
            Street = order.Street,
            City = order.City,
            State = order.State,
            Country = order.Country,
            ZipCode = order.ZipCode,
            Total = order.Total,
            Items = order.OrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                Subtotal = item.Subtotal,
                PictureFileName = item.PictureFileName
            }).ToList()
        };
    }
}
