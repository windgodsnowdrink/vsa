#load "time_heap.cs"

using System.Reflection;

public class time_heapTest
{
    [Fact]
    public void Map_TypeExists()
    {
        var type = typeof(Map<,>);
        Assert.NotNull(type);
    }

    [Fact]
    public void MsgQueue_TypeExists()
    {
        var type = typeof(MsgQueue<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void AsyncMsgQueue_TypeExists()
    {
        var type = typeof(AsyncMsgQueue<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void BlockingMsgQueue_TypeExists()
    {
        var type = typeof(BlockingMsgQueue<>);
        Assert.NotNull(type);
    }
}