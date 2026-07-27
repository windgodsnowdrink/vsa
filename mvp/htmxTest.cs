#load "htmx.cs"

using System.Reflection;

public class htmxTest
{
    [Fact]
    public void Input_TypeExists()
    {
        var type = typeof(Input);
        Assert.NotNull(type);
        Assert.NotNull(type.GetProperty("Name"));
        Assert.NotNull(type.GetProperty("Bio"));
        Assert.NotNull(type.GetProperty("Gender"));
        Assert.NotNull(type.GetProperty("IsEmployed"));
        Assert.NotNull(type.GetProperty("Transportation"));
    }
}