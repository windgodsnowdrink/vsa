#load "Channels.cs"

using System.Reflection;

public class ChannelsTest
{
    [Fact]
    public void MessageData_TypeExists()
    {
        var type = typeof(App.MessageData);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProcessingResult_TypeExists()
    {
        var type = typeof(App.ProcessingResult<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void ChannelProcessingException_TypeExists()
    {
        var type = typeof(App.ChannelProcessingException);
        Assert.NotNull(type);
    }

    [Fact]
    public void ChannelConfigurationException_TypeExists()
    {
        var type = typeof(App.ChannelConfigurationException);
        Assert.NotNull(type);
    }

    [Fact]
    public void ChannelConfiguration_TypeExists()
    {
        var type = typeof(App.ChannelConfiguration);
        Assert.NotNull(type);
    }

    [Fact]
    public void ChannelTelemetryService_TypeExists()
    {
        var type = typeof(App.ChannelTelemetryService);
        Assert.NotNull(type);
    }

    [Fact]
    public void ChannelMetrics_TypeExists()
    {
        var type = typeof(App.ChannelMetrics);
        Assert.NotNull(type);
    }

    [Fact]
    public void IChannelFactory_TypeExists()
    {
        var type = typeof(App.IChannelFactory);
        Assert.NotNull(type);
    }

    [Fact]
    public void ChannelFactory_TypeExists()
    {
        var type = typeof(App.ChannelFactory);
        Assert.NotNull(type);
    }

    [Fact]
    public void ChannelMessageProcessor_TypeExists()
    {
        var type = typeof(App.ChannelMessageProcessor);
        Assert.NotNull(type);
    }

    [Fact]
    public void ChannelProducerService_TypeExists()
    {
        var type = typeof(App.ChannelProducerService);
        Assert.NotNull(type);
    }

    [Fact]
    public void ChannelConsumerService_TypeExists()
    {
        var type = typeof(App.ChannelConsumerService);
        Assert.NotNull(type);
    }

    [Fact]
    public void ChannelManagementService_TypeExists()
    {
        var type = typeof(App.ChannelManagementService);
        Assert.NotNull(type);
    }

    [Fact]
    public void ChannelStatus_TypeExists()
    {
        var type = typeof(App.ChannelStatus);
        Assert.NotNull(type);
    }

    [Fact]
    public void StreamingChannelProcessor_TypeExists()
    {
        var type = typeof(App.StreamingChannelProcessor);
        Assert.NotNull(type);
    }

    [Fact]
    public void PriorityChannelService_TypeExists()
    {
        var type = typeof(App.PriorityChannelService);
        Assert.NotNull(type);
    }

    [Fact]
    public void PriorityChannels_TypeExists()
    {
        var type = typeof(App.PriorityChannels<>);
        Assert.NotNull(type);
    }
}