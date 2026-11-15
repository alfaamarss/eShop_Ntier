
using eShop.NTier.BusinessLogic.DTOs.Catalog;
using eShop.NTier.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace eShop.NTier.API.Controllers;

[ApiController]
[Route("api/catalog/items")]
public class CatalogController : ControllerBase
{
    private readonly CatalogService _service;

    public CatalogController(CatalogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetItems(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? name = null,
        [FromQuery] int[]? brand = null,
        [FromQuery] int[]? type = null)
    {
        try
        {
            var result = await _service.GetItemsAsync(
                pageIndex, pageSize, name, brand, type);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}", Name = "GetCatalogItemById")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);

        if (item is null)
            return NotFound();

        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCatalogItemDto dto)
    {
        try
        {
            var item = await _service.CreateAsync(dto);

            return CreatedAtRoute(
                "GetCatalogItemById",
                new { id = item.Id },
                item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateCatalogItemDto dto)
    {
        try
        {
            var item = await _service.UpdateAsync(id, dto);

            if (item is null)
                return NotFound();

            return CreatedAtRoute(
                "GetCatalogItemById",
                new { id = item.Id },
                item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
