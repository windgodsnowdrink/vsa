#load "mcpserver_integration.cs"

using System.Reflection;

public class mcpserver_integrationTest
{
    [Fact]
    public void ITransientService_TypeExists()
    {
        var type = typeof(App.ITransientService);
        Assert.NotNull(type);
    }

    [Fact]
    public void IScopedService_TypeExists()
    {
        var type = typeof(App.IScopedService);
        Assert.NotNull(type);
    }

    [Fact]
    public void ISingletonService_TypeExists()
    {
        var type = typeof(App.ISingletonService);
        Assert.NotNull(type);
    }

    [Fact]
    public void IOpenGeneric_TypeExists()
    {
        var type = typeof(App.IOpenGeneric<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void IReadableRepository_TypeExists()
    {
        var type = typeof(App.IReadableRepository);
        Assert.NotNull(type);
    }

    [Fact]
    public void IWritableRepository_TypeExists()
    {
        var type = typeof(App.IWritableRepository);
        Assert.NotNull(type);
    }

    [Fact]
    public void IMyService_TypeExists()
    {
        var type = typeof(App.IMyService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("GetData"));
    }

    [Fact]
    public void MyService_TypeExists()
    {
        var type = typeof(App.MyService);
        Assert.NotNull(type);
    }

    [Fact]
    public void LoggingDecorator_TypeExists()
    {
        var type = typeof(App.LoggingDecorator);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductDto_TypeExists()
    {
        var type = typeof(App.ProductDto);
        Assert.NotNull(type);
    }

    [Fact]
    public void GetProductByIdQuery_TypeExists()
    {
        var type = typeof(App.GetProductByIdQuery);
        Assert.NotNull(type);
    }

    [Fact]
    public void GetProductByIdResult_TypeExists()
    {
        var type = typeof(App.GetProductByIdResult);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductCreatedEventHandler_TypeExists()
    {
        var type = typeof(App.ProductCreatedEventHandler);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductPriceChangedEventHandler_TypeExists()
    {
        var type = typeof(App.ProductPriceChangedEventHandler);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductCreatedEvent_TypeExists()
    {
        var type = typeof(App.ProductCreatedEvent);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductPriceChangedEvent_TypeExists()
    {
        var type = typeof(App.ProductPriceChangedEvent);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductNotFoundException_TypeExists()
    {
        var type = typeof(App.ProductNotFoundException);
        Assert.NotNull(type);
    }

    [Fact]
    public void CreateProductEndpoint_TypeExists()
    {
        var type = typeof(App.CreateProductEndpoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void CreateProductCommand_TypeExists()
    {
        var type = typeof(App.CreateProductCommand);
        Assert.NotNull(type);
    }

    [Fact]
    public void CreateProductCommandValidator_TypeExists()
    {
        var type = typeof(App.CreateProductCommandValidator);
        Assert.NotNull(type);
    }

    [Fact]
    public void DeleteProductEndpoint_TypeExists()
    {
        var type = typeof(App.DeleteProductEndpoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void UpdateProductEndpoint_TypeExists()
    {
        var type = typeof(App.UpdateProductEndpoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void GetProductByCategoryEndpoint_TypeExists()
    {
        var type = typeof(App.GetProductByCategoryEndpoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void GetProductByIdEndpoint_TypeExists()
    {
        var type = typeof(App.GetProductByIdEndpoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void GetProductsEndpoint_TypeExists()
    {
        var type = typeof(App.GetProductsEndpoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void Product_TypeExists()
    {
        var type = typeof(App.Product);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductConfiguration_TypeExists()
    {
        var type = typeof(App.ProductConfiguration);
        Assert.NotNull(type);
    }

    [Fact]
    public void CatalogDataSeeder_TypeExists()
    {
        var type = typeof(App.CatalogDataSeeder);
        Assert.NotNull(type);
    }

    [Fact]
    public void CatalogDbContext_TypeExists()
    {
        var type = typeof(App.CatalogDbContext);
        Assert.NotNull(type);
    }

    [Fact]
    public void BasketCheckoutDto_TypeExists()
    {
        var type = typeof(App.BasketCheckoutDto);
        Assert.NotNull(type);
    }

    [Fact]
    public void ShoppingCartDto_TypeExists()
    {
        var type = typeof(App.ShoppingCartDto);
        Assert.NotNull(type);
    }

    [Fact]
    public void ShoppingCartItemDto_TypeExists()
    {
        var type = typeof(App.ShoppingCartItemDto);
        Assert.NotNull(type);
    }

    [Fact]
    public void BasketNotFoundException_TypeExists()
    {
        var type = typeof(App.BasketNotFoundException);
        Assert.NotNull(type);
    }

    [Fact]
    public void CreateBasketEndpoint_TypeExists()
    {
        var type = typeof(App.CreateBasketEndpoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void AddItemIntoBasketEndpoint_TypeExists()
    {
        var type = typeof(App.AddItemIntoBasketEndpoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void DeleteBasketEndpoint_TypeExists()
    {
        var type = typeof(App.DeleteBasketEndpoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void RemoveItemFromBasketEndpoint_TypeExists()
    {
        var type = typeof(App.RemoveItemFromBasketEndpoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void GetBasketEndpoint_TypeExists()
    {
        var type = typeof(App.GetBasketEndpoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void CheckoutBasketEndpoint_TypeExists()
    {
        var type = typeof(App.CheckoutBasketEndpoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void OutboxMessage_TypeExists()
    {
        var type = typeof(App.OutboxMessage);
        Assert.NotNull(type);
    }

    [Fact]
    public void ShoppingCart_TypeExists()
    {
        var type = typeof(App.ShoppingCart);
        Assert.NotNull(type);
    }

    [Fact]
    public void ShoppingCartItem_TypeExists()
    {
        var type = typeof(App.ShoppingCartItem);
        Assert.NotNull(type);
    }

    [Fact]
    public void ShoppingCartConfiguration_TypeExists()
    {
        var type = typeof(App.ShoppingCartConfiguration);
        Assert.NotNull(type);
    }

    [Fact]
    public void ShoppingCartItemConfiguration_TypeExists()
    {
        var type = typeof(App.ShoppingCartItemConfiguration);
        Assert.NotNull(type);
    }

    [Fact]
    public void BasketDbContext_TypeExists()
    {
        var type = typeof(App.BasketDbContext);
        Assert.NotNull(type);
    }

    [Fact]
    public void ShoppingCartConverter_TypeExists()
    {
        var type = typeof(App.ShoppingCartConverter);
        Assert.NotNull(type);
    }
}