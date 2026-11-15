
using eShop.NTier.BusinessLogic.DTOs.Catalog;
using eShop.NTier.BusinessLogic.Interfaces;
using eShop.NTier.Domain.Entities;

namespace eShop.NTier.BusinessLogic.Services;

public class CatalogService(ICatalogRepository repository)
{
    public async Task<object> GetItemsAsync(
        int pageIndex,
        int pageSize,
        string? name,
        int[]? brand,
        int[]? type)
    {
        if (pageIndex < 0 || pageSize < 1 || pageSize > 100)
            throw new ArgumentException("Invalid pagination parameters.");

        var result = await repository.GetItemsAsync(
            pageIndex, pageSize, name, brand, type);

        return new
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Data = result.Items.Select(MapToDto)
        };
    }

    public async Task<CatalogItemDto?> GetByIdAsync(int id)
    {
        var item = await repository.GetByIdAsync(id);
        return item is null ? null : MapToDto(item);
    }

    public async Task<CatalogItemDto> CreateAsync(CreateCatalogItemDto dto)
    {
        Validate(dto.Name, dto.Price);

        if (dto.AvailableStock < 0)
            throw new ArgumentException("Stock cannot be negative.");

        if (!await repository.BrandExistsAsync(dto.CatalogBrandId))
            throw new ArgumentException("Brand not found.");

        if (!await repository.TypeExistsAsync(dto.CatalogTypeId))
            throw new ArgumentException("Type not found.");

        var item = new CatalogItem
        {
            Name = dto.Name.Trim(),
            Description = dto.Description,
            Price = dto.Price,
            PictureFileName = dto.PictureFileName,
            CatalogBrandId = dto.CatalogBrandId,
            CatalogTypeId = dto.CatalogTypeId,
            RestockThreshold = dto.RestockThreshold,
            MaxStockThreshold = dto.MaxStockThreshold,
            OnReorder = dto.OnReorder
        };

        item.SetInitialStock(dto.AvailableStock);

        await repository.AddAsync(item);

        var savedItem = await repository.GetByIdAsync(item.Id);
        return MapToDto(savedItem!);
    }

    public async Task<CatalogItemDto?> UpdateAsync(
        int id,
        UpdateCatalogItemDto dto)
    {
        Validate(dto.Name, dto.Price);

        if (dto.AvailableStock < 0)
            throw new ArgumentException("Stock cannot be negative.");

        var item = await repository.GetByIdAsync(id);

        if (item is null)
            return null;

        if (!await repository.BrandExistsAsync(dto.CatalogBrandId))
            throw new ArgumentException("Brand not found.");

        if (!await repository.TypeExistsAsync(dto.CatalogTypeId))
            throw new ArgumentException("Type not found.");

        item.Name = dto.Name.Trim();
        item.Description = dto.Description;
        item.Price = dto.Price;
        item.PictureFileName = dto.PictureFileName;
        item.CatalogBrandId = dto.CatalogBrandId;
        item.CatalogTypeId = dto.CatalogTypeId;
        item.RestockThreshold = dto.RestockThreshold;
        item.MaxStockThreshold = dto.MaxStockThreshold;
        item.OnReorder = dto.OnReorder;

        item.SetInitialStock(dto.AvailableStock);

        await repository.UpdateAsync(item);

        var updatedItem = await repository.GetByIdAsync(id);
        return MapToDto(updatedItem!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await repository.GetByIdAsync(id);

        if (item is null)
            return false;

        await repository.DeleteAsync(item);
        return true;
    }

    private static void Validate(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");

        if (name.Length > 50)
            throw new ArgumentException(
                "Product name maximum is 50 characters.");

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");
    }

    public Task<IReadOnlyList<CatalogBrand>> GetBrandsAsync()
    {
        return repository.GetBrandsAsync();
    }

    public Task<IReadOnlyList<CatalogType>> GetTypesAsync()
    {
        return repository.GetTypesAsync();
    }

    private static CatalogItemDto MapToDto(CatalogItem item)
    {
        return new CatalogItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            PictureFileName = item.PictureFileName,
            CatalogTypeId = item.CatalogTypeId,
            CatalogType = item.CatalogType.Type,
            CatalogBrandId = item.CatalogBrandId,
            CatalogBrand = item.CatalogBrand.Brand,
            AvailableStock = item.AvailableStock,
            RestockThreshold = item.RestockThreshold,
            MaxStockThreshold = item.MaxStockThreshold,
            OnReorder = item.OnReorder
        };
    }
}
