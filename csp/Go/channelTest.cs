#load "channel.cs"

using System.Reflection;

public class channelTest
{
    [Fact]
    public void chan_state_EnumExists()
    {
        var type = typeof(chan_state);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void chan_type_EnumExists()
    {
        var type = typeof(chan_type);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void chan_notify_sign_TypeExists()
    {
        var type = typeof(chan_notify_sign);
        Assert.NotNull(type);
    }

    [Fact]
    public void unlimit_chan_TypeExists()
    {
        var type = typeof(unlimit_chan<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void limit_chan_TypeExists()
    {
        var type = typeof(limit_chan<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void nil_chan_TypeExists()
    {
        var type = typeof(nil_chan<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void broadcast_chan_TypeExists()
    {
        var type = typeof(broadcast_chan<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void csp_chan_TypeExists()
    {
        var type = typeof(csp_chan<,>);
        Assert.NotNull(type);
    }
}