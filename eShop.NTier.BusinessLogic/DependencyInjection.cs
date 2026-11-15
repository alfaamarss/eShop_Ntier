using eShop.NTier.BusinessLogic.Services;
using eShop.NTier.BusinessLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.NTier.BusinessLogic;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(
        this IServiceCollection services)
    {
        services.AddScoped<CatalogService>();
        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}


