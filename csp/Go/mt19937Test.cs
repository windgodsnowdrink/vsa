#load "mt19937.cs"

using System.Reflection;

public class mt19937Test
{
    [Fact]
    public void mt19937_TypeExists()
    {
        var type = typeof(mt19937);
        Assert.NotNull(type);
    }
}