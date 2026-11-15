
namespace eShop.NTier.BusinessLogic.DTOs.Catalog;

public class CreateCatalogItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? PictureFileName { get; set; }

    public int CatalogTypeId { get; set; }
    public int CatalogBrandId { get; set; }

    public int AvailableStock { get; set; }
    public int RestockThreshold { get; set; }
    public int MaxStockThreshold { get; set; }
    public bool OnReorder { get; set; }
}
