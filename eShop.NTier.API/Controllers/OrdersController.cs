using eShop.NTier.BusinessLogic.DTOs.Ordering;
using eShop.NTier.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eShop.NTier.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost("checkout/{buyerId}")]
    public async Task<IActionResult> Checkout(
        string buyerId,
        [FromBody] CheckoutRequestDto request)
    {
        try
        {
            var order = await orderService.CheckoutAsync(buyerId, request);
            return CreatedAtAction(
                nameof(GetDetail),
                new { buyerId, orderId = order.Id },
                order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{buyerId}")]
    public async Task<IActionResult> GetOrders(string buyerId)
    {
        try
        {
            var orders = await orderService.GetByBuyerIdAsync(buyerId);
            return Ok(orders);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{buyerId}/detail/{orderId:int}")]
    public async Task<IActionResult> GetDetail(
        string buyerId, int orderId)
    {
        try
        {
            var order = await orderService.GetDetailAsync(buyerId, orderId);

            if (order is null)
                return NotFound(new { message = "Order not found." });

            return Ok(order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{buyerId}/{orderId:int}/cancel")]
    public async Task<IActionResult> Cancel(
        string buyerId, int orderId)
    {
        try
        {
            var order = await orderService.CancelAsync(buyerId, orderId);

            if (order is null)
                return NotFound(new { message = "Order not found." });

            return Ok(order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
