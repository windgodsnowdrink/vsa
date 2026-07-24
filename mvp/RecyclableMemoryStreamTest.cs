#load "RecyclableMemoryStream.cs"

using System.Reflection;

public class RecyclableMemoryStreamTest
{
    [Fact]
    public void MemoryStreamManagerConfiguration_TypeExists()
    {
        var type = typeof(App.MemoryStreamManagerConfiguration);
        Assert.NotNull(type);
    }

    [Fact]
    public void FileProcessingService_TypeExists()
    {
        var type = typeof(App.FileProcessingService);
        Assert.NotNull(type);
    }

    [Fact]
    public void NetworkDataService_TypeExists()
    {
        var type = typeof(App.NetworkDataService);
        Assert.NotNull(type);
    }

    [Fact]
    public void SerializationService_TypeExists()
    {
        var type = typeof(App.SerializationService);
        Assert.NotNull(type);
    }

    [Fact]
    public void MemoryStreamMonitoringService_TypeExists()
    {
        var type = typeof(App.MemoryStreamMonitoringService);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductionUsageDemo_TypeExists()
    {
        var type = typeof(App.ProductionUsageDemo);
        Assert.NotNull(type);
    }

    [Fact]
    public void StartupConfiguration_TypeExists()
    {
        var type = typeof(App.StartupConfiguration);
        Assert.NotNull(type);
    }

    [Fact]
    public void AdvancedMemoryStreamManager_TypeExists()
    {
        var type = typeof(App.AdvancedMemoryStreamManager);
        Assert.NotNull(type);
    }
}