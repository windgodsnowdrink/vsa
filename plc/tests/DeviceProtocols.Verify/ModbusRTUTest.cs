#load "ModbusRTU.cs"

using System.Reflection;

public class ModbusRTUTest
{
    [Fact]
    public void ModbusRTU_TypeExists()
    {
        var type = typeof(ModbusRTU);
        Assert.NotNull(type);
    }
}