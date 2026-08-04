[MemoryPackable]
public partial record DeviceModel
{
    public required string ModelId { get; init; }
    // 新增物模型标准字段
    public DeviceModelType ModelType { get; init; } = DeviceModelType.Thing;
    public Dictionary<string, PropertyMetadata> Properties { get; init; } = new();
    public Dictionary<string, TelemetryMetadata> Telemetry { get; init; } = new();
    // 新增物模型关系定义
    public Dictionary<string, Relationship> Relationships { get; init; } = new();
}

public enum DeviceModelType 
{
    Thing = 0,
    Space = 1,
    Asset = 2
}

[MemoryPackable]
public partial record PropertyMetadata
{
    public required string Identifier { get; init; }
    public string DataType { get; init; } = "string";
    public string AccessMode { get; init; } = "rw";
    public object? DefaultValue { get; init; }
}

[SkipLocalsInit]
public interface IDeviceModelProtocol
{
    string ProtocolName { get; }
    // 增强解析方法支持物模型版本
    ValueTask<DeviceModel> ParseModelAsync(ReadOnlyMemory<byte> payload, string modelVersion = "1.0");
    // 新增物模型校验方法
    ValueTask<bool> ValidateModelAsync(DeviceModel model);
    // 新增物模型转换方法
    ValueTask<byte[]> ConvertModelAsync(DeviceModel source, string targetFormat);
}
