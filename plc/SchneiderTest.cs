#load "Schneider.cs"

using System.Reflection;

public class SchneiderTest
{
    [Fact]
    public void SchneiderDriver_TypeExists()
    {
        var type = typeof(SchneiderDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void SchneiderNode_TypeExists()
    {
        var type = typeof(SchneiderNode);
        Assert.NotNull(type);
    }

    [Fact]
    public void SchneiderParameter_TypeExists()
    {
        var type = typeof(SchneiderParameter);
        Assert.NotNull(type);
    }
}