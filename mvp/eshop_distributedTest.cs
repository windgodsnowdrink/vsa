#load "eshop_distributed.cs"

using System.Reflection;

public class eshop_distributedTest
{
    [Fact]
    public void IntegrationEvent_TypeExists()
    {
        var type = typeof(ServiceDefaults.IntegrationEvent);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductPriceChangedIntegrationEvent_TypeExists()
    {
        var type = typeof(ServiceDefaults.ProductPriceChangedIntegrationEvent);
        Assert.NotNull(type);
    }

    [Fact]
    public void MassTransitExtentions_TypeExists()
    {
        var type = typeof(ServiceDefaults.MassTransitExtentions);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("AddMassTransitWithAssemblies"));
    }

    [Fact]
    public void Extensions_TypeExists()
    {
        var type = typeof(ServiceDefaults.Extensions);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("AddServiceDefaults"));
        Assert.NotNull(type.GetMethod("ConfigureOpenTelemetry"));
        Assert.NotNull(type.GetMethod("AddDefaultHealthChecks"));
        Assert.NotNull(type.GetMethod("MapDefaultEndpoints"));
    }

    [Fact]
    public void ProductEndpoints_TypeExists()
    {
        var type = typeof(Catalog.ProductEndpoints);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("MapProductEndpoints"));
    }

    [Fact]
    public void DataSeeder_TypeExists()
    {
        var type = typeof(Catalog.DataSeeder);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("Seed"));
    }

    [Fact]
    public void ProductDbContext_TypeExists()
    {
        var type = typeof(Catalog.ProductDbContext);
        Assert.NotNull(type);
    }

    [Fact]
    public void Product_TypeExists()
    {
        var type = typeof(Catalog.Product);
        Assert.NotNull(type);
        Assert.NotNull(type.GetProperty("Id"));
        Assert.NotNull(type.GetProperty("Name"));
        Assert.NotNull(type.GetProperty("Description"));
        Assert.NotNull(type.GetProperty("Price"));
        Assert.NotNull(type.GetProperty("ImageUrl"));
    }

    [Fact]
    public void ProductVector_TypeExists()
    {
        var type = typeof(Catalog.ProductVector);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductAIService_TypeExists()
    {
        var type = typeof(Catalog.ProductAIService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("SupportAsync"));
        Assert.NotNull(type.GetMethod("SearchProductsAsync"));
    }

    [Fact]
    public void ProductService_TypeExists()
    {
        var type = typeof(Catalog.ProductService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("GetProductsAsync"));
        Assert.NotNull(type.GetMethod("GetProductByIdAsync"));
        Assert.NotNull(type.GetMethod("CreateProductAsync"));
        Assert.NotNull(type.GetMethod("UpdateProductAsync"));
        Assert.NotNull(type.GetMethod("DeleteProductAsync"));
        Assert.NotNull(type.GetMethod("SearchProductsAsync"));
    }

    [Fact]
    public void BasketEndpoints_TypeExists()
    {
        var type = typeof(Basket.BasketEndpoints);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("MapBasketEndpoints"));
    }

    [Fact]
    public void ShoppingCart_TypeExists()
    {
        var type = typeof(Basket.ShoppingCart);
        Assert.NotNull(type);
        Assert.NotNull(type.GetProperty("UserName"));
        Assert.NotNull(type.GetProperty("Items"));
        Assert.NotNull(type.GetProperty("TotalPrice"));
    }

    [Fact]
    public void ShoppingCartItem_TypeExists()
    {
        var type = typeof(Basket.ShoppingCartItem);
        Assert.NotNull(type);
    }

    [Fact]
    public void BasketService_TypeExists()
    {
        var type = typeof(Basket.BasketService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("GetBasket"));
        Assert.NotNull(type.GetMethod("UpdateBasket"));
        Assert.NotNull(type.GetMethod("DeleteBasket"));
    }

    [Fact]
    public void ProductPriceChangedIntegrationEventHandler_TypeExists()
    {
        var type = typeof(Basket.ProductPriceChangedIntegrationEventHandler);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("Consume"));
    }

    [Fact]
    public void CatalogApiClient_TypeExists()
    {
        var type = typeof(Basket.CatalogApiClient);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("GetProductById"));
    }
}