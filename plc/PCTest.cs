#load "PC.cs"

using System.Reflection;

public class PCTest
{
    [Fact]
    public void PCDriver_TypeExists()
    {
        var type = typeof(PCDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void PCParameter_TypeExists()
    {
        var type = typeof(PCParameter);
        Assert.NotNull(type);
    }
}