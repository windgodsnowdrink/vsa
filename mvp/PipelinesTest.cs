#load "Pipelines.cs"

using System.Reflection;

public class PipelinesTest
{
    [Fact]
    public void MessageType_EnumExists()
    {
        var type = typeof(App.MessageType);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void MessageHeader_TypeExists()
    {
        var type = typeof(App.MessageHeader);
        Assert.NotNull(type);
        Assert.NotNull(type.GetProperty("Type"));
        Assert.NotNull(type.GetProperty("Length"));
        Assert.NotNull(type.GetProperty("CorrelationId"));
        Assert.NotNull(type.GetProperty("Timestamp"));
        Assert.NotNull(type.GetProperty("Metadata"));
    }

    [Fact]
    public void TextMessage_TypeExists()
    {
        var type = typeof(App.TextMessage);
        Assert.NotNull(type);
    }

    [Fact]
    public void BinaryDataMessage_TypeExists()
    {
        var type = typeof(App.BinaryDataMessage);
        Assert.NotNull(type);
    }

    [Fact]
    public void PipelineException_TypeExists()
    {
        var type = typeof(App.PipelineException);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProtocolValidationException_TypeExists()
    {
        var type = typeof(App.ProtocolValidationException);
        Assert.NotNull(type);
    }

    [Fact]
    public void MessageProtocolHandler_TypeExists()
    {
        var type = typeof(App.MessageProtocolHandler);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("ReadMessageAsync"));
        Assert.NotNull(type.GetMethod("WriteMessageAsync"));
    }

    [Fact]
    public void PipelineReadResult_TypeExists()
    {
        var type = typeof(App.PipelineReadResult);
        Assert.NotNull(type);
        Assert.NotNull(type.GetProperty("Header"));
        Assert.NotNull(type.GetProperty("Data"));
        Assert.NotNull(type.GetProperty("MessageType"));
    }

    [Fact]
    public void PipelineProcessor_TypeExists()
    {
        var type = typeof(App.PipelineProcessor);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("ProcessPipelineAsync"));
    }

    [Fact]
    public void PipelineProcessingResult_TypeExists()
    {
        var type = typeof(App.PipelineProcessingResult);
        Assert.NotNull(type);
    }

    [Fact]
    public void NetworkPipelineService_TypeExists()
    {
        var type = typeof(App.NetworkPipelineService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("CreatePipeReader"));
        Assert.NotNull(type.GetMethod("CreatePipeWriter"));
        Assert.NotNull(type.GetMethod("HandleClientConnectionAsync"));
    }

    [Fact]
    public void PipelineBufferOptimizationService_TypeExists()
    {
        var type = typeof(App.PipelineBufferOptimizationService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("CreateOptimizedPipe"));
        Assert.NotNull(type.GetMethod("GetPoolUsageStats"));
    }

    [Fact]
    public void BufferPoolStats_TypeExists()
    {
        var type = typeof(App.BufferPoolStats);
        Assert.NotNull(type);
    }

    [Fact]
    public void MessageProcessingService_TypeExists()
    {
        var type = typeof(App.MessageProcessingService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("HandleMessageAsync"));
    }

    [Fact]
    public void StreamMultiplexingService_TypeExists()
    {
        var type = typeof(App.StreamMultiplexingService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("MultiplexStreamsAsync"));
        Assert.NotNull(type.GetMethod("DemultiplexStreamAsync"));
    }

    [Fact]
    public void PipelineMiddleware_TypeExists()
    {
        var type = typeof(App.PipelineMiddleware);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("ProcessWithRateLimitingAsync"));
        Assert.NotNull(type.GetMethod("ProcessWithSecurityAsync"));
    }

    [Fact]
    public void ProcessingStatistics_TypeExists()
    {
        var type = typeof(App.ProcessingStatistics);
        Assert.NotNull(type);
    }

    [Fact]
    public void SecurityException_TypeExists()
    {
        var type = typeof(App.SecurityException);
        Assert.NotNull(type);
    }

    [Fact]
    public void IPipelineTelemetryService_TypeExists()
    {
        var type = typeof(App.IPipelineTelemetryService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("TrackMessageProcessed"));
        Assert.NotNull(type.GetMethod("TrackPipelineError"));
        Assert.NotNull(type.GetMethod("GetPerformanceMetrics"));
    }

    [Fact]
    public void PipelineTelemetryService_TypeExists()
    {
        var type = typeof(App.PipelineTelemetryService);
        Assert.NotNull(type);
    }

    [Fact]
    public void MessageMetrics_TypeExists()
    {
        var type = typeof(App.MessageMetrics);
        Assert.NotNull(type);
    }

    [Fact]
    public void PipelinePerformanceMetrics_TypeExists()
    {
        var type = typeof(App.PipelinePerformanceMetrics);
        Assert.NotNull(type);
    }

    [Fact]
    public void MessageProcessor_TypeExists()
    {
        var type = typeof(App.MessageProcessor);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("ProcessMessage"));
    }
}