#load "generator.cs"

using System.Reflection;

public class generatorTest
{
    [Fact]
    public void generator_TypeExists()
    {
        var type = typeof(generator);
        Assert.NotNull(type);
    }

    [Fact]
    public void async_result_wrap_TypeExists()
    {
        var type = typeof(async_result_wrap<>);
        Assert.NotNull(type);
    }
}