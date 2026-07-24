#load "NetPing.cs"

using System.Reflection;

public class NetPingTest
{
    [Fact]
    public void NetPingDriver_TypeExists()
    {
        var type = typeof(NetPingDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void NetPingParameter_TypeExists()
    {
        var type = typeof(NetPingParameter);
        Assert.NotNull(type);
    }
}