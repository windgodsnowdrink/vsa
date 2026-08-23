#load "Omron.cs"

using System.Reflection;

public class OmronTest
{
    [Fact]
    public void Omron_TypeExists()
    {
        var type = typeof(Omron);
        Assert.NotNull(type);
    }
}