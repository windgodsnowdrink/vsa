#load "Fatek.cs"

using System.Reflection;

public class FatekTest
{
    [Fact]
    public void FatekDriver_TypeExists()
    {
        var type = typeof(FatekDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void FatekNode_TypeExists()
    {
        var type = typeof(FatekNode);
        Assert.NotNull(type);
    }

    [Fact]
    public void FatekParameter_TypeExists()
    {
        var type = typeof(FatekParameter);
        Assert.NotNull(type);
    }
}