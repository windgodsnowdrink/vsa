#load "Panasonic.cs"

using System.Reflection;

public class PanasonicTest
{
    [Fact]
    public void PanasonicDriver_TypeExists()
    {
        var type = typeof(PanasonicDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void PanasonicNode_TypeExists()
    {
        var type = typeof(PanasonicNode);
        Assert.NotNull(type);
    }

    [Fact]
    public void PanasonicParameter_TypeExists()
    {
        var type = typeof(PanasonicParameter);
        Assert.NotNull(type);
    }
}