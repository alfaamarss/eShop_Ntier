
using eShop.NTier.BusinessLogic.Interfaces;
using eShop.NTier.DataAccess.Context;
using eShop.NTier.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eShop.NTier.DataAccess.Repositories;

public class CatalogRepository : ICatalogRepository
{
    private readonly CatalogDbContext _context;

    public CatalogRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyList<CatalogItem> Items, long TotalCount)>
        GetItemsAsync(
            int pageIndex,
            int pageSize,
            string? name,
            int[]? brand,
            int[]? type)
    {
        IQueryable<CatalogItem> query = _context.CatalogItems
            .AsNoTracking()
            .Include(x => x.CatalogBrand)
            .Include(x => x.CatalogType);

        // Filter berdasarkan awalan nama produk
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(x => x.Name.StartsWith(name));
        }

        // Filter berdasarkan satu atau beberapa brand
        if (brand is { Length: > 0 })
        {
            query = query.Where(x => brand.Contains(x.CatalogBrandId));
        }

        // Filter berdasarkan satu atau beberapa type
        if (type is { Length: > 0 })
        {
            query = query.Where(x => type.Contains(x.CatalogTypeId));
        }

        var totalCount = await query.LongCountAsync();

        var items = await query
            .OrderBy(x => x.Name)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public Task<CatalogItem?> GetByIdAsync(int id)
    {
        return _context.CatalogItems
            .Include(x => x.CatalogBrand)
            .Include(x => x.CatalogType)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(CatalogItem item)
    {
        _context.CatalogItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public Task UpdateAsync(CatalogItem item)
    {
        return _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(CatalogItem item)
    {
        _context.CatalogItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    public Task<bool> BrandExistsAsync(int id)
    {
        return _context.CatalogBrands.AnyAsync(x => x.Id == id);
    }

    public Task<bool> TypeExistsAsync(int id)
    {
        return _context.CatalogTypes.AnyAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<CatalogBrand>> GetBrandsAsync()
    {
        return await _context.CatalogBrands
            .AsNoTracking()
            .OrderBy(x => x.Brand)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<CatalogType>> GetTypesAsync()
    {
        return await _context.CatalogTypes
            .AsNoTracking()
            .OrderBy(x => x.Type)
            .ToListAsync();
    }
}
