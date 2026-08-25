namespace Plc.Host.Gateway;

/// <summary>
/// 边缘网关不可用（未启动 / 管道断开 / 连接超时）时抛出，由 WebApi 端点映射为 HTTP 503。
/// 与网关 RPC 的业务异常（如设备未找到）区分，后者不应触发重连。
/// </summary>
public sealed class GatewayUnavailableException : Exception
{
    public GatewayUnavailableException(string message) : base(message)
    {
    }

    public GatewayUnavailableException(string message, Exception? inner) : base(message, inner)
    {
    }
}
