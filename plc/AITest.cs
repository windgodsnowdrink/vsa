#load "AI.cs"

using System.Reflection;

public class AITest
{
    [Fact]
    public void Class1_TypeExists()
    {
        var type = typeof(Class1);
        Assert.NotNull(type);
    }
}