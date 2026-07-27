#load "Keyence.cs"

using System.Reflection;

public class KeyenceTest
{
    [Fact]
    public void KeyenceDriver_TypeExists()
    {
        var type = typeof(KeyenceDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void KeyenceNode_TypeExists()
    {
        var type = typeof(KeyenceNode);
        Assert.NotNull(type);
    }

    [Fact]
    public void KeyenceParameter_TypeExists()
    {
        var type = typeof(KeyenceParameter);
        Assert.NotNull(type);
    }
}