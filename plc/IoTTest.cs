#load "IoT.cs"

using System.Reflection;

public class IoTTest
{
    [Fact]
    public void DatabaseParameter_TypeExists()
    {
        var type = typeof(DatabaseParameter);
        Assert.NotNull(type);
    }

    [Fact]
    public void DatabseNode_TypeExists()
    {
        var type = typeof(DatabseNode);
        Assert.NotNull(type);
    }

    [Fact]
    public void IoTDatabaseDriver_TypeExists()
    {
        var type = typeof(IoTDatabaseDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void SerialParameter_TypeExists()
    {
        var type = typeof(SerialParameter);
        Assert.NotNull(type);
    }

    [Fact]
    public void IoTSerialParameter_TypeExists()
    {
        var type = typeof(IoTSerialParameter);
        Assert.NotNull(type);
    }

    [Fact]
    public void SerialNode_TypeExists()
    {
        var type = typeof(SerialNode);
        Assert.NotNull(type);
    }

    [Fact]
    public void IoTSerialDriver_TypeExists()
    {
        var type = typeof(IoTSerialDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void HttpParameter_TypeExists()
    {
        var type = typeof(HttpParameter);
        Assert.NotNull(type);
    }

    [Fact]
    public void IoTHttpDriver_TypeExists()
    {
        var type = typeof(IoTHttpDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void IoTTcpDriver_TypeExists()
    {
        var type = typeof(IoTTcpDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void IoTUdpDriver_TypeExists()
    {
        var type = typeof(IoTUdpDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void SocketNode_TypeExists()
    {
        var type = typeof(SocketNode);
        Assert.NotNull(type);
    }

    [Fact]
    public void SocketParameter_TypeExists()
    {
        var type = typeof(SocketParameter);
        Assert.NotNull(type);
    }

    [Fact]
    public void IDevice_TypeExists()
    {
        var type = typeof(IDevice);
        Assert.NotNull(type);
    }

    [Fact]
    public void DriverAttribute_TypeExists()
    {
        var type = typeof(DriverAttribute);
        Assert.NotNull(type);
    }

    [Fact]
    public void DriverFactory_TypeExists()
    {
        var type = typeof(DriverFactory);
        Assert.NotNull(type);
    }

    [Fact]
    public void DriverInfo_TypeExists()
    {
        var type = typeof(DriverInfo);
        Assert.NotNull(type);
    }

    [Fact]
    public void IAsyncDriver_TypeExists()
    {
        var type = typeof(IAsyncDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void IDiscoverableDriver_TypeExists()
    {
        var type = typeof(IDiscoverableDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void IDriver_TypeExists()
    {
        var type = typeof(IDriver);
        Assert.NotNull(type);
    }

    [Fact]
    public void IDriverParameter_TypeExists()
    {
        var type = typeof(IDriverParameter);
        Assert.NotNull(type);
    }

    [Fact]
    public void IDriverParameterKey_TypeExists()
    {
        var type = typeof(IDriverParameterKey);
        Assert.NotNull(type);
    }

    [Fact]
    public void INode_TypeExists()
    {
        var type = typeof(INode);
        Assert.NotNull(type);
    }

    [Fact]
    public void Node_TypeExists()
    {
        var type = typeof(Node);
        Assert.NotNull(type);
    }

    [Fact]
    public void IExpressionEngine_TypeExists()
    {
        var type = typeof(IExpressionEngine);
        Assert.NotNull(type);
    }

    [Fact]
    public void IDeviceInfo_TypeExists()
    {
        var type = typeof(IDeviceInfo);
        Assert.NotNull(type);
    }

    [Fact]
    public void DeviceInfo_TypeExists()
    {
        var type = typeof(DeviceInfo);
        Assert.NotNull(type);
    }

    [Fact]
    public void DeviceModel_TypeExists()
    {
        var type = typeof(DeviceModel);
        Assert.NotNull(type);
    }

    [Fact]
    public void PostKinds_EnumExists()
    {
        var type = typeof(PostKinds);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void ServiceRequest_TypeExists()
    {
        var type = typeof(ServiceRequest);
        Assert.NotNull(type);
    }

    [Fact]
    public void DataModel_TypeExists()
    {
        var type = typeof(DataModel);
        Assert.NotNull(type);
    }

    [Fact]
    public void DataModels_TypeExists()
    {
        var type = typeof(DataModels);
        Assert.NotNull(type);
    }

    [Fact]
    public void DevicePropertyModel_TypeExists()
    {
        var type = typeof(DevicePropertyModel);
        Assert.NotNull(type);
    }

    [Fact]
    public void EndianType_EnumExists()
    {
        var type = typeof(EndianType);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void ByteOrder_EnumExists()
    {
        var type = typeof(ByteOrder);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void EventModel_TypeExists()
    {
        var type = typeof(EventModel);
        Assert.NotNull(type);
    }

    [Fact]
    public void EventModels_TypeExists()
    {
        var type = typeof(EventModels);
        Assert.NotNull(type);
    }

    [Fact]
    public void EventModes_EnumExists()
    {
        var type = typeof(EventModes);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void IPoint_TypeExists()
    {
        var type = typeof(IPoint);
        Assert.NotNull(type);
    }

    [Fact]
    public void PointModel_TypeExists()
    {
        var type = typeof(PointModel);
        Assert.NotNull(type);
    }

    [Fact]
    public void PropertyModel_TypeExists()
    {
        var type = typeof(PropertyModel);
        Assert.NotNull(type);
    }

    [Fact]
    public void PropertyModels_TypeExists()
    {
        var type = typeof(PropertyModels);
        Assert.NotNull(type);
    }

    [Fact]
    public void ServiceEventArgs_TypeExists()
    {
        var type = typeof(ServiceEventArgs);
        Assert.NotNull(type);
    }

    [Fact]
    public void ServiceModel_TypeExists()
    {
        var type = typeof(ServiceModel);
        Assert.NotNull(type);
    }

    [Fact]
    public void ServiceReplyModel_TypeExists()
    {
        var type = typeof(ServiceReplyModel);
        Assert.NotNull(type);
    }

    [Fact]
    public void ServiceStatus_EnumExists()
    {
        var type = typeof(ServiceStatus);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void ShadowModel_TypeExists()
    {
        var type = typeof(ShadowModel);
        Assert.NotNull(type);
    }

    [Fact]
    public void ThingSpec_TypeExists()
    {
        var type = typeof(ThingSpec);
        Assert.NotNull(type);
    }

    [Fact]
    public void DataSpecs_TypeExists()
    {
        var type = typeof(DataSpecs);
        Assert.NotNull(type);
    }

    [Fact]
    public void EventSpec_TypeExists()
    {
        var type = typeof(EventSpec);
        Assert.NotNull(type);
    }

    [Fact]
    public void Profile_TypeExists()
    {
        var type = typeof(Profile);
        Assert.NotNull(type);
    }

    [Fact]
    public void PropertySpec_TypeExists()
    {
        var type = typeof(PropertySpec);
        Assert.NotNull(type);
    }

    [Fact]
    public void ServiceSpec_TypeExists()
    {
        var type = typeof(ServiceSpec);
        Assert.NotNull(type);
    }

    [Fact]
    public void TypeSpec_TypeExists()
    {
        var type = typeof(TypeSpec);
        Assert.NotNull(type);
    }

    [Fact]
    public void IServiceHandler_TypeExists()
    {
        var type = typeof(IServiceHandler);
        Assert.NotNull(type);
    }

    [Fact]
    public void DefaultSerialPort_TypeExists()
    {
        var type = typeof(DefaultSerialPort);
        Assert.NotNull(type);
    }

    [Fact]
    public void IBoard_TypeExists()
    {
        var type = typeof(IBoard);
        Assert.NotNull(type);
    }

    [Fact]
    public void Board_TypeExists()
    {
        var type = typeof(Board);
        Assert.NotNull(type);
    }

    [Fact]
    public void IInputPort_TypeExists()
    {
        var type = typeof(IInputPort);
        Assert.NotNull(type);
    }

    [Fact]
    public void KeyEventArgs_TypeExists()
    {
        var type = typeof(KeyEventArgs);
        Assert.NotNull(type);
    }

    [Fact]
    public void FileInputPort_TypeExists()
    {
        var type = typeof(FileInputPort);
        Assert.NotNull(type);
    }

    [Fact]
    public void IModbus_TypeExists()
    {
        var type = typeof(IModbus);
        Assert.NotNull(type);
    }
}