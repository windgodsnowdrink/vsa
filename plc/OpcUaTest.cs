#load "OpcUa.cs"

using System.Reflection;

public class OpcUaTest
{
    [Fact]
    public void OpcUa_TypeExists()
    {
        var type = typeof(OpcUa);
        Assert.NotNull(type);
    }
}