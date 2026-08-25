using System.IO.Pipes;
using StreamJsonRpc;

namespace Plc.Host.Gateway;

/// <summary>
/// 宿主侧 StreamJsonRpc 客户端：经 NamedPipe 连接 plc-gateway，把边缘网关的
/// 设备协议 / 消息记忆（Disruptor 流水线）能力透传到 WebApi 层。
/// <para>
/// 单例复用一条连接（StreamJsonRpc 支持单连接上的并发调用）；首次调用懒连接，
/// 连接断开时自动重连并重试一次。网关未启动 / 管道不可达时抛出
/// <see cref="GatewayUnavailableException"/>，不应重试网关 RPC 的业务异常（RemoteInvocationException）。
/// </para>
/// </summary>
public sealed class GatewayRpcClient : IAsyncDisposable
{
    private readonly GatewayOptions _options;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private NamedPipeClientStream? _pipe;
    private JsonRpc? _rpc;
    private bool _disposed;

    public GatewayRpcClient(GatewayOptions options)
    {
        _options = options;
    }

    /// <summary>当前是否持有可用连接。</summary>
    public bool IsConnected => _rpc is { IsDisposed: false } && _pipe is { IsConnected: true };

    private async Task EnsureConnectedAsync(CancellationToken ct)
    {
        if (IsConnected)
        {
            return;
        }

        await _gate.WaitAsync(ct);
        try
        {
            if (IsConnected)
            {
                return;
            }

            await DisposeRpcAsync();

            var pipe = new NamedPipeClientStream(
                _options.ServerName, _options.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

            using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct);
            linked.CancelAfter(_options.ConnectTimeoutMs);

            try
            {
                await pipe.ConnectAsync(linked.Token);
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                pipe.Dispose();
                throw new GatewayUnavailableException(
                    $"未在 {_options.ConnectTimeoutMs}ms 内连接到网关管道 '{_options.PipeName}'");
            }

            _pipe = pipe;
            _rpc = JsonRpc.Attach(pipe);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<T> InvokeAsync<T>(string method, CancellationToken ct, params object?[] args)
    {
        Exception? last = null;
        for (var attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                await EnsureConnectedAsync(ct);
                return await _rpc!.InvokeWithCancellationAsync<T>(method, args, ct);
            }
            catch (Exception ex) when (_rpc is { IsDisposed: true }
                                        || ex is IOException
                                        or ObjectDisposedException)
            {
                last = ex;
                await DisposeRpcAsync();
                if (attempt == 1)
                {
                    break;
                }
            }
        }

        throw new GatewayUnavailableException($"网关调用 {method} 失败: {last?.Message}", last);
    }

    public Task<string[]> ListProtocolsAsync(CancellationToken ct = default)
        => InvokeAsync<string[]>("gateway.listProtocols", ct);

    public Task<GatewayDevice[]> ListDevicesAsync(CancellationToken ct = default)
        => InvokeAsync<GatewayDevice[]>("gateway.listDevices", ct);

    public Task<byte[]> ReadAsync(string deviceId, ushort start, ushort count, int type, CancellationToken ct = default)
        => InvokeAsync<byte[]>("gateway.read", ct, deviceId, start, count, type);

    public Task WriteAsync(string deviceId, ushort start, int type, ushort[] values, CancellationToken ct = default)
        => InvokeAsync<object?>("gateway.write", ct, deviceId, start, type, values);

    private async Task DisposeRpcAsync()
    {
        var rpc = _rpc;
        _rpc = null;
        var pipe = _pipe;
        _pipe = null;
        if (rpc is not null)
        {
            try
            {
                rpc.Dispose();
            }
            catch
            {
                // 忽略断开时的清理异常
            }
        }

        pipe?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        await DisposeRpcAsync();
        _gate.Dispose();
    }
}
