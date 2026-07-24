#load "OpcDa.cs"

using System.Reflection;

public class OpcDaTest
{
    [Fact]
    public void OpcDa_TypeExists()
    {
        var type = typeof(OpcDa);
        Assert.NotNull(type);
    }
}