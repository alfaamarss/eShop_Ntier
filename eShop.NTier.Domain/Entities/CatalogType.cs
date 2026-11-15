namespace eShop.NTier.Domain.Entities;

public class CatalogType
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;

    public ICollection<CatalogItem> Items { get; set; }
        = new List<CatalogItem>();
}
