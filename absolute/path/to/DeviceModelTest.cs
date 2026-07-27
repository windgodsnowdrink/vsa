#load "DeviceModel.cs"

using System.Reflection;

public class DeviceModelTest
{
    [Fact]
    public void DeviceModelType_EnumExists()
    {
        var type = typeof(DeviceModelType);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void IDeviceModelProtocol_TypeExists()
    {
        var type = typeof(IDeviceModelProtocol);
        Assert.NotNull(type);
    }
}