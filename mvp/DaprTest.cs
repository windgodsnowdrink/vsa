#load "Dapr.cs"

using System.Reflection;

public class DaprTest
{
    [Fact]
    public void Test_TypeExists()
    {
        var type = typeof(App.Test);
        Assert.NotNull(type);
    }
}