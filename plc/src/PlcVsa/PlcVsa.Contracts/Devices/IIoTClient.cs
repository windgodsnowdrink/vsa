namespace PlcVsa.Contracts.Devices;

/// <summary>统一步进结果（避免在 Contracts 引入额外外部 Result<> 包）。</summary>
/// <remarks>来自 vsa-demo Result<T>。</remarks>
public sealed class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Message { get; init; }

    public static Result<T> Ok(T v) => new() { IsSuccess = true, Value = v };
    public static Result<T> Fail(string msg) => new() { IsSuccess = false, Message = msg };
}

public static class Result
{
    public static Result<object> Ok() => Result<object>.Ok(new());
    public static Result<object> Fail(string msg) => Result<object>.Fail(msg);
}

/// <summary>
/// IoTClient 风格高层协议客户端：强类型地址读 / 写，不关心底层连接与会话。
/// </summary>
public interface IIoTClient : IAsyncDisposable
{
    string Protocol { get; }
    Result<T> Read<T>(string address, string? dataType = null);
    Result Write<T>(string address, T value);
}

/// <summary>协议工厂：按 ProtocolId + 连接选项创建 IIoTClient 实例。</summary>
public interface IIoTClientFactory
{
    IIoTClient Create(string protocolId, DeviceConnectionOptions options);
}
