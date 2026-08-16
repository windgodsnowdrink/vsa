namespace PlcAiot.Abstractions.Devices;

/// <summary>设备连接端点（TCP / RTU 共用）。</summary>
public sealed record DeviceEndpoint
{
    public string Host { get; init; } = "127.0.0.1";
    public int Port { get; init; } = 502;
    public byte SlaveId { get; init; } = 1;
    public string? SerialPort { get; init; }
    public int BaudRate { get; init; } = 9600;
}
