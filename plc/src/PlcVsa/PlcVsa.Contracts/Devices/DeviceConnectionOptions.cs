namespace PlcVsa.Contracts.Devices;

/// <summary>设备连接参数。</summary>
public sealed record DeviceConnectionOptions(
    string Host,
    int Port,
    byte UnitId = 1,
    int TimeoutMs = 3000);
