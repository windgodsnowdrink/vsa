#load "SSE.cs"

using System.Reflection;

public class SSETest
{
    [Fact]
    public void StockService_TypeExists()
    {
        var type = typeof(StockService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("GenerateStockPrices"));
    }

    [Fact]
    public void StockPriceEvent_TypeExists()
    {
        var type = typeof(StockPriceEvent);
        Assert.NotNull(type);
        Assert.NotNull(type.GetProperty("Id"));
        Assert.NotNull(type.GetProperty("Symbol"));
        Assert.NotNull(type.GetProperty("Price"));
        Assert.NotNull(type.GetProperty("Timestamp"));
    }
}