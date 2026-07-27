#load "mutex.cs"

using System.Reflection;

public class mutexTest
{
    [Fact]
    public void go_mutex_TypeExists()
    {
        var type = typeof(go_mutex);
        Assert.NotNull(type);
    }

    [Fact]
    public void go_shared_mutex_TypeExists()
    {
        var type = typeof(go_shared_mutex);
        Assert.NotNull(type);
    }

    [Fact]
    public void go_condition_variable_TypeExists()
    {
        var type = typeof(go_condition_variable);
        Assert.NotNull(type);
    }
}