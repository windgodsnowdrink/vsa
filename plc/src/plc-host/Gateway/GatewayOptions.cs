namespace Plc.Host.Gateway;

/// <summary>
/// 宿主 ↔ 边缘网关（plc-gateway）通信配置。默认值与网关 NamedPipe 监听名一致。
/// </summary>
public sealed class GatewayOptions
{
    /// <summary>NamedPipe 名称，须与 plc-gateway 的监听名一致。</summary>
    public string PipeName { get; set; } = "plc-gateway";

    /// <summary>管道服务器名，"." 表示本机。</summary>
    public string ServerName { get; set; } = ".";

    /// <summary>连接超时（毫秒），网关未启动时快速失败而非长阻塞。</summary>
    public int ConnectTimeoutMs { get; set; } = 2000;
}
