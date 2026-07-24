#load "ollamsSharp_integration.cs"

using System.Reflection;

public class ollamsSharp_integrationTest
{
    [Fact]
    public void ProductAIService_TypeExists()
    {
        var type = typeof(App.ProductAIService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("SupportAsync"));
        Assert.NotNull(type.GetMethod("SearchProductsAsync"));
    }

    [Fact]
    public void ProductService_TypeExists()
    {
        var type = typeof(App.ProductService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("GetProductsAsync"));
        Assert.NotNull(type.GetMethod("GetProductByIdAsync"));
        Assert.NotNull(type.GetMethod("CreateProductAsync"));
        Assert.NotNull(type.GetMethod("UpdateProductAsync"));
        Assert.NotNull(type.GetMethod("DeleteProductAsync"));
        Assert.NotNull(type.GetMethod("SearchProductsAsync"));
    }

    [Fact]
    public void Product_TypeExists()
    {
        var type = typeof(App.Product);
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
        var type = typeof(App.ProductVector);
        Assert.NotNull(type);
    }
}