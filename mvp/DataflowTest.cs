#load "Dataflow.cs"

using System.Reflection;

public class DataflowTest
{
    [Fact]
    public void DataflowBuilder_TypeExists()
    {
        var type = typeof(DataflowBuilder);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("CreateValidationBlock"));
        Assert.NotNull(type.GetMethod("CreateProcessingBlock"));
        Assert.NotNull(type.GetMethod("CreateAggregationBlock"));
        Assert.NotNull(type.GetMethod("CreateBroadcastBlock"));
        Assert.NotNull(type.GetMethod("CreateBufferBlock"));
    }

    [Fact]
    public void DataflowConfiguration_TypeExists()
    {
        var type = typeof(DataflowConfiguration);
        Assert.NotNull(type);
    }

    [Fact]
    public void DataflowOrchestrationService_TypeExists()
    {
        var type = typeof(DataflowOrchestrationService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("CreateLinearPipeline"));
        Assert.NotNull(type.GetMethod("CreateBranchingFlow"));
        Assert.NotNull(type.GetMethod("CompleteFlowAsync"));
    }

    [Fact]
    public void DataflowService_TypeExists()
    {
        var type = typeof(DataflowService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("CreateImageProcessingPipeline"));
        Assert.NotNull(type.GetMethod("CreateLoggingDataflow"));
        Assert.NotNull(type.GetMethod("ProcessBatchAsync"));
    }

    [Fact]
    public void ProcessingRequest_TypeExists()
    {
        var type = typeof(ProcessingRequest);
        Assert.NotNull(type);
    }

    [Fact]
    public void DataflowPipeline_TypeExists()
    {
        var type = typeof(DataflowPipeline<,>);
        Assert.NotNull(type);
    }
}