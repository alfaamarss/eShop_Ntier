using eShop.NTier.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace eShop.NTier.API.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogMetadataController(CatalogService service)
    : ControllerBase
{
    [HttpGet("catalogbrands")]
    public async Task<IActionResult> GetBrands()
    {
        var brands = await service.GetBrandsAsync();
        return Ok(brands);
    }

    [HttpGet("catalogtypes")]
    public async Task<IActionResult> GetTypes()
    {
        var types = await service.GetTypesAsync();
        return Ok(types);
    }
}