#load "EDD.cs"

using System.Reflection;

public class EDDTest
{
    [Fact]
    public void IEvent_TypeExists()
    {
        var type = typeof(App.IEvent);
        Assert.NotNull(type);
    }

    [Fact]
    public void IEventBus_TypeExists()
    {
        var type = typeof(App.IEventBus);
        Assert.NotNull(type);
    }

    [Fact]
    public void EventBus_TypeExists()
    {
        var type = typeof(App.EventBus);
        Assert.NotNull(type);
    }

    [Fact]
    public void IDevice_TypeExists()
    {
        var type = typeof(App.IDevice);
        Assert.NotNull(type);
    }

    [Fact]
    public void TemperatureSensor_TypeExists()
    {
        var type = typeof(App.TemperatureSensor);
        Assert.NotNull(type);
    }

    [Fact]
    public void DeviceDataUpdatedEvent_TypeExists()
    {
        var type = typeof(App.DeviceDataUpdatedEvent);
        Assert.NotNull(type);
    }

    [Fact]
    public void DeviceAlarmEvent_TypeExists()
    {
        var type = typeof(App.DeviceAlarmEvent);
        Assert.NotNull(type);
    }

    [Fact]
    public void AlarmLevel_EnumExists()
    {
        var type = typeof(App.AlarmLevel);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }
}