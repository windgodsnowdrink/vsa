#load "SmartA4.cs"

using System.Reflection;

public class SmartA4Test
{
    [Fact]
    public void SmartA4_TypeExists()
    {
        var type = typeof(SmartA4);
        Assert.NotNull(type);
    }
}