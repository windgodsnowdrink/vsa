#load "time_heap.cs"

using System.Reflection;

public class time_heapTest
{
    [Fact]
    public void MapNode_TypeExists()
    {
        var type = typeof(MapNode<,>);
        Assert.NotNull(type);
    }

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
}