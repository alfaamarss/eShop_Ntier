
using eShop.NTier.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eShop.NTier.DataAccess.Context;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<CatalogBrand> CatalogBrands => Set<CatalogBrand>();
    public DbSet<CatalogType> CatalogTypes => Set<CatalogType>();
    public DbSet<CustomerBasket> CustomerBaskets => Set<CustomerBasket>();
    public DbSet<BasketItem> BasketItems => Set<BasketItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CustomerBasket>(entity =>
        {
            entity.ToTable("CustomerBasket");
            entity.HasKey(x => x.BuyerId);

            entity.Property(x => x.BuyerId)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasMany(x => x.Items)
                .WithOne(x => x.CustomerBasket)
                .HasForeignKey(x => x.BuyerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BasketItem>(entity =>
        {
            entity.ToTable("BasketItem");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ProductId).IsRequired();
            entity.Property(x => x.Quantity).IsRequired();

            entity.HasIndex(x => new { x.BuyerId, x.ProductId }).IsUnique();
        });
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.BuyerId)
                .HasMaxLength(100).IsRequired();

            entity.Property(x => x.RecipientName)
                .HasMaxLength(100).IsRequired();

            entity.Property(x => x.Street)
                .HasMaxLength(250).IsRequired();

            entity.Property(x => x.City)
                .HasMaxLength(100).IsRequired();

            entity.Property(x => x.State)
                .HasMaxLength(100).IsRequired();

            entity.Property(x => x.Country)
                .HasMaxLength(100).IsRequired();

            entity.Property(x => x.ZipCode)
                .HasMaxLength(20).IsRequired();

            entity.Property(x => x.Total)
                .HasPrecision(18, 2);

            entity.Property(x => x.Status)
                .HasConversion<int>();

            entity.HasMany(x => x.OrderItems)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.BuyerId);
            entity.HasIndex(x => x.OrderDate);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItem");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ProductName)
                .HasMaxLength(100).IsRequired();

            entity.Property(x => x.UnitPrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.Quantity).IsRequired();
        });

        modelBuilder.Entity<CatalogItem>(entity =>
        {
            entity.ToTable("Catalog");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.Price)
                .HasPrecision(18, 2);

            entity.Property(x => x.AvailableStock)
                .IsRequired();

            entity.Property(x => x.OnReorder)
                .IsRequired();

            entity.HasIndex(x => x.Name);

            entity.HasOne(x => x.CatalogBrand)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.CatalogBrandId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CatalogType)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.CatalogTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CatalogBrand>(entity =>
        {
            entity.ToTable("CatalogBrand");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Brand)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<CatalogType>(entity =>
        {
            entity.ToTable("CatalogType");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Type)
                .HasMaxLength(100)
                .IsRequired();
        });
    }
}


