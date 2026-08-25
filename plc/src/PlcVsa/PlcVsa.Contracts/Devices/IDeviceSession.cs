namespace PlcVsa.Contracts.Devices;

/// <summary>一次设备连接的生命周期会话：按寄存器类型读 / 写原始字节。</summary>
public interface IDeviceSession : IAsyncDisposable
{
    /// <summary>读取原始字节（协议相关：如 Modbus 为寄存器值大端排列）。</summary>
    Task<byte[]> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default);

    /// <summary>写入值（values 含义按协议约定）。</summary>
    Task WriteAsync(WriteRequest request, CancellationToken cancellationToken = default);
}
