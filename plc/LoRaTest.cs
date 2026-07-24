#load "LoRa.cs"

using System.Reflection;

public class LoRaTest
{
    [Fact]
    public void Setting_TypeExists()
    {
        var type = typeof(Setting);
        Assert.NotNull(type);
    }

    [Fact]
    public void DataMessage_TypeExists()
    {
        var type = typeof(DataMessage);
        Assert.NotNull(type);
    }

    [Fact]
    public void JoinRequest_TypeExists()
    {
        var type = typeof(JoinRequest);
        Assert.NotNull(type);
    }

    [Fact]
    public void JoinAccept_TypeExists()
    {
        var type = typeof(JoinAccept);
        Assert.NotNull(type);
    }

    [Fact]
    public void LoRaMessage_TypeExists()
    {
        var type = typeof(LoRaMessage);
        Assert.NotNull(type);
    }

    [Fact]
    public void LoRaType_EnumExists()
    {
        var type = typeof(LoRaType);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void MessageTypes_EnumExists()
    {
        var type = typeof(MessageTypes);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void PHYMessage_TypeExists()
    {
        var type = typeof(PHYMessage);
        Assert.NotNull(type);
    }
}