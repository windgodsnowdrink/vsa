#load "functional.cs"

using System.Reflection;

public class functionalTest
{
    [Fact]
    public void void_type_TypeExists()
    {
        var type = typeof(void_type);
        Assert.NotNull(type);
    }

    [Fact]
    public void tuple_TypeExists()
    {
        var type = typeof(tuple<>);
        Assert.NotNull(type);
    }
}