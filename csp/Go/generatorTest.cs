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

    [Fact]
    public void chan_lost_msg_TypeExists()
    {
        var type = typeof(chan_lost_msg<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void chan_exception_TypeExists()
    {
        var type = typeof(chan_exception);
        Assert.NotNull(type);
    }

    [Fact]
    public void csp_fail_exception_TypeExists()
    {
        var type = typeof(csp_fail_exception);
        Assert.NotNull(type);
    }

    [Fact]
    public void wait_group_TypeExists()
    {
        var type = typeof(wait_group);
        Assert.NotNull(type);
    }

    [Fact]
    public void wait_gate_TypeExists()
    {
        var type = typeof(wait_gate<>);
        Assert.NotNull(type);
    }
}