using eShop.NTier.BusinessLogic.DTOs.Basket;
using eShop.NTier.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eShop.NTier.API.Controllers;

[ApiController]
[Route("api/basket")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _service;

    public BasketController(IBasketService service)
    {
        _service = service;
    }

    [HttpGet("{buyerId}")]
    public async Task<IActionResult> GetBasket(string buyerId)
    {
        var basket = await _service.GetBasketAsync(buyerId);

        return Ok(basket ?? new UpdateBasketDto
        {
            BuyerId = buyerId,
            Items = []
        });
    }

    [HttpPut("{buyerId}")]
    public async Task<IActionResult> UpdateBasket(
        string buyerId,
        [FromBody] UpdateBasketDto dto)
    {
        try
        {
            dto.BuyerId = buyerId;

            var basket = await _service.UpdateBasketAsync(dto);

            return Ok(basket);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{buyerId}")]
    public async Task<IActionResult> DeleteBasket(string buyerId)
    {
        await _service.DeleteBasketAsync(buyerId);

        return NoContent();
    }
}
