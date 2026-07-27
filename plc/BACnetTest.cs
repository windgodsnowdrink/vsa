#load "BACnet.cs"

using System.Reflection;

public class BACnetTest
{
    [Fact]
    public void BACnet_TypeExists()
    {
        var type = typeof(BACnet);
        Assert.NotNull(type);
    }
}