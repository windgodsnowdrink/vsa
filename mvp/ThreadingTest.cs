#load "Threading.cs"

using System.Reflection;

public class ThreadingTest
{
    [Fact]
    public void ThreadSafeDataService_TypeExists()
    {
        var type = typeof(App.ThreadSafeDataService);
        Assert.NotNull(type);
    }

    [Fact]
    public void User_TypeExists()
    {
        var type = typeof(App.User);
        Assert.NotNull(type);
    }

    [Fact]
    public void AsyncConcurrencyProcessor_TypeExists()
    {
        var type = typeof(App.AsyncConcurrencyProcessor);
        Assert.NotNull(type);
    }

    [Fact]
    public void BusinessRequest_TypeExists()
    {
        var type = typeof(App.BusinessRequest);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProcessingResult_TypeExists()
    {
        var type = typeof(App.ProcessingResult);
        Assert.NotNull(type);
    }

    [Fact]
    public void ThreadPoolOptimizationDemo_TypeExists()
    {
        var type = typeof(App.ThreadPoolOptimizationDemo);
        Assert.NotNull(type);
    }

    [Fact]
    public void AsyncCollectionService_TypeExists()
    {
        var type = typeof(App.AsyncCollectionService);
        Assert.NotNull(type);
    }

    [Fact]
    public void DeadlockPreventionService_TypeExists()
    {
        var type = typeof(App.DeadlockPreventionService);
        Assert.NotNull(type);
    }

    [Fact]
    public void ThreadingPerformanceMonitor_TypeExists()
    {
        var type = typeof(App.ThreadingPerformanceMonitor);
        Assert.NotNull(type);
    }

    [Fact]
    public void ThreadingDemoProgram_TypeExists()
    {
        var type = typeof(App.ThreadingDemoProgram);
        Assert.NotNull(type);
    }

    [Fact]
    public void ThreadingConfiguration_TypeExists()
    {
        var type = typeof(App.ThreadingConfiguration);
        Assert.NotNull(type);
    }

    [Fact]
    public void AdvancedThreadingDemo_TypeExists()
    {
        var type = typeof(App.AdvancedThreadingDemo);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductionThreadConfiguration_TypeExists()
    {
        var type = typeof(App.ProductionThreadConfiguration);
        Assert.NotNull(type);
    }
}