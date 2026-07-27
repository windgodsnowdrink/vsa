#load "async_timer.cs"

using System.Reflection;

public class async_timerTest
{
    [Fact]
    public void system_tick_TypeExists()
    {
        var type = typeof(system_tick);
        Assert.NotNull(type);
    }

    [Fact]
    public void async_timer_TypeExists()
    {
        var type = typeof(async_timer);
        Assert.NotNull(type);
    }
}