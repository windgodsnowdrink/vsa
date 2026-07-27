#load "SmartA2.cs"

using System.Reflection;

public class SmartA2Test
{
    [Fact]
    public void Class1_TypeExists()
    {
        var type = typeof(Class1);
        Assert.NotNull(type);
    }
}