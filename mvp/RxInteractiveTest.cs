#load "RxInteractive.cs"

using System.Reflection;

public class RxInteractiveTest
{
    [Fact]
    public void AsyncDataProcessingService_TypeExists()
    {
        var type = typeof(AsyncDataProcessingService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("ProcessProductsAsync"));
        Assert.NotNull(type.GetMethod("ProcessOrdersRealTimeAsync"));
        Assert.NotNull(type.GetMethod("ProcessCombinedDataStreamsAsync"));
        Assert.NotNull(type.GetMethod("ProcessWithConditionalLogicAsync"));
    }

    [Fact]
    public void AsyncDataFilteringService_TypeExists()
    {
        var type = typeof(AsyncDataFilteringService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("FilterProductsAdvancedAsync"));
        Assert.NotNull(type.GetMethod("AggregateByTimeWindowAsync"));
        Assert.NotNull(type.GetMethod("GetCategoryStatisticsAsync"));
    }

    [Fact]
    public void AsyncPagingService_TypeExists()
    {
        var type = typeof(AsyncPagingService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("GetPagedResultsAsync"));
        Assert.NotNull(type.GetMethod("StreamPagesAsync"));
    }

    [Fact]
    public void Product_TypeExists()
    {
        var type = typeof(Product);
        Assert.NotNull(type);
    }

    [Fact]
    public void Order_TypeExists()
    {
        var type = typeof(Order);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProcessingResult_TypeExists()
    {
        var type = typeof(ProcessingResult<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void RealTimeProcessingResult_TypeExists()
    {
        var type = typeof(RealTimeProcessingResult<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void CombinedProcessingResult_TypeExists()
    {
        var type = typeof(CombinedProcessingResult);
        Assert.NotNull(type);
    }

    [Fact]
    public void ConditionalProcessingResult_TypeExists()
    {
        var type = typeof(ConditionalProcessingResult);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductFilterCriteria_TypeExists()
    {
        var type = typeof(ProductFilterCriteria);
        Assert.NotNull(type);
    }

    [Fact]
    public void TimeWindowAggregationResult_TypeExists()
    {
        var type = typeof(TimeWindowAggregationResult);
        Assert.NotNull(type);
    }

    [Fact]
    public void CategoryStats_TypeExists()
    {
        var type = typeof(CategoryStats);
        Assert.NotNull(type);
    }

    [Fact]
    public void PagedResult_TypeExists()
    {
        var type = typeof(PagedResult<>);
        Assert.NotNull(type);
    }
}