namespace eShop.NTier.Domain.Entities;

public class CatalogBrand
{
    public int Id { get; set; }
    public string Brand { get; set; } = string.Empty;

    public ICollection<CatalogItem> Items { get; set; }
        = new List<CatalogItem>();
}
