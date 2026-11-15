
using eShop.NTier.Domain.Entities;

namespace eShop.NTier.BusinessLogic.Interfaces;

public interface ICatalogRepository
{
    Task<(IReadOnlyList<CatalogItem> Items, long TotalCount)> GetItemsAsync(
        int pageIndex,
        int pageSize,
        string? name,
        int[]? brand,
        int[]? type);

    Task<CatalogItem?> GetByIdAsync(int id);
    Task AddAsync(CatalogItem item);
    Task UpdateAsync(CatalogItem item);
    Task DeleteAsync(CatalogItem item);
    Task<IReadOnlyList<CatalogBrand>> GetBrandsAsync();
    Task<IReadOnlyList<CatalogType>> GetTypesAsync();
    Task<bool> BrandExistsAsync(int id);
    Task<bool> TypeExistsAsync(int id);
}
