#load "Fuji.cs"

using System.Reflection;

public class FujiTest
{
    [Fact]
    public void FujiDriver_TypeExists()
    {
        var type = typeof(FujiDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void FujiNode_TypeExists()
    {
        var type = typeof(FujiNode);
        Assert.NotNull(type);
    }

    [Fact]
    public void FujiParameter_TypeExists()
    {
        var type = typeof(FujiParameter);
        Assert.NotNull(type);
    }
}