using PlcAiot.Abstractions.Devices;
using PlcAiot.Abstractions.Faults;
using PlcAiot.Abstractions.Logging;
using PlcAiot.Faults;
using System.Threading.Channels;

namespace PlcAiot.Supervisor;

/// <summary>
/// 设备会话监督者（§1.11）：
/// - 每设备独立 Task，崩溃即指数退避重启（带抖动防惊群）；
/// - 重启预算防 flapping 雪崩；
/// - 看门狗心跳超时 → 失联判定 → COMM_TIMEOUT 故障（可触发重连/故障转移）；
/// - critical/telemetry 双通道背压：过载先丢遥测保指令（§1.11.5）。
/// </summary>
public sealed class DeviceSessionSupervisor : IAsyncDisposable
{
    private readonly Lock _gate = new();                 // .NET 9+ 轻量锁
    private Task? _loop;
    private int _restarts;
    private DateTime _lastHeartbeat = DateTime.UtcNow;
    private readonly DeviceOptions _opts;
    private readonly IDeviceProtocol _protocol;
    private readonly FaultService _faults;
    private readonly ILogger _log;

    // 优先级背压：指令/心跳走 critical（容量高于遥测），遥测可丢
    private readonly Channel<PointSample> _telemetry = Channel.CreateBounded<PointSample>(
        new BoundedChannelOptions(1024) { SingleReader = true, SingleWriter = false, FullMode = BoundedChannelFullMode.DropOldest });
    private readonly Channel<WriteCommand> _critical = Channel.CreateBounded<WriteCommand>(
        new BoundedChannelOptions(256) { SingleReader = true, SingleWriter = false, FullMode = BoundedChannelFullMode.DropOldest });

    public DeviceSessionSupervisor(DeviceOptions opts, IDeviceProtocol protocol, FaultService faults, ILogger? log = null)
    {
        _opts = opts;
        _protocol = protocol;
        _faults = faults;
        _log = log ?? new ConsoleLogger($"sup:{opts.DeviceId}");
    }

    public int Restarts
    {
        get { lock (_gate) return _restarts; }
    }

    public void Start(CancellationToken outer)
    {
        _loop = Task.Run(async () =>
        {
            while (!outer.IsCancellationRequested)
            {
                try
                {
                    await RunSessionAsync(outer);          // 连接→轮询→处理
                    return;                                // 正常退出（被停机取消）
                }
                catch (OperationCanceledException) when (outer.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex) when (!outer.IsCancellationRequested)
                {
                    if (++_restarts > _opts.MaxRestarts)  // 重启预算（防 flapping）
                    {
                        _log.Log(LogLevel.Critical, $"{_opts.DeviceId} 重启超预算，升级安全态");
                        await RaiseAsync("RESTART_BUDGET", FaultSeverity.Critical, "supervisor");
                        await EnterSafeStateAsync();
                        break;
                    }
                    var backoff = Backoff(_restarts);
                    _log.Log(LogLevel.Warning,
                        $"{_opts.DeviceId} 会话崩溃，{backoff.TotalSeconds:F1}s 后第 {_restarts} 次重启 ({ex.GetType().Name})");
                    await RaiseAsync("SESSION_CRASH", FaultSeverity.Error, "supervisor", ex.Message);
                    await Task.Delay(backoff, outer);
                }
            }
        }, outer);
    }

    private async Task RunSessionAsync(CancellationToken ct)
    {
        await _protocol.ConnectAsync(ct);
        _log.Log(LogLevel.Information, "session started");
        var pollTask = ConsumeAsync(_protocol.PollAsync(ct), ct);   // 数据平面
        var cmdTask = ConsumeCommandsAsync(ct);                     // 控制平面
        var watchdog = WatchdogAsync(ct);                           // 看门狗
        try { await Task.WhenAny(pollTask, cmdTask, watchdog); }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { }
        await _protocol.DisconnectAsync(CancellationToken.None);
    }

    private async Task ConsumeAsync(IAsyncEnumerable<PointSample> samples, CancellationToken ct)
    {
        await foreach (var s in samples.WithCancellation(ct))
        {
            Heartbeat();                                       // 数据平面周期心跳
            _telemetry.Writer.TryWrite(s);                    // 过载由 DropOldest 自动丢旧
        }
    }

    private async Task ConsumeCommandsAsync(CancellationToken ct)
    {
        await foreach (var cmd in _critical.Reader.ReadAllAsync(ct))
        {
            try { await _protocol.WriteAsync(cmd, ct); }
            catch (Exception ex)
            {
                _log.Log(LogLevel.Error, $"write failed: {ex.Message}");
                await RaiseAsync("WRITE_FAIL", FaultSeverity.Error, "command", cmd.Tag);
            }
        }
    }

    private async Task WatchdogAsync(CancellationToken ct)
    {
        var misses = 0;
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(_opts.WatchdogTimeout, ct);
            if (DateTime.UtcNow - _lastHeartbeat > _opts.WatchdogTimeout)
            {
                if (++misses >= _opts.WatchdogMisses)
                {
                    _log.Log(LogLevel.Error, "watchdog: 设备无响应 -> COMM_TIMEOUT");
                    await RaiseAsync("COMM_TIMEOUT", FaultSeverity.Error, "watchdog");
                    throw new TimeoutException($"{_opts.DeviceId} heartbeat lost");
                }
            }
            else
            {
                misses = 0;
            }
        }
    }

    public void Heartbeat() => _lastHeartbeat = DateTime.UtcNow;
    public bool IsAlive(TimeSpan timeout) => DateTime.UtcNow - _lastHeartbeat < timeout;
    public bool TryEnqueueCommand(WriteCommand cmd) => _critical.Writer.TryWrite(cmd);

    private Task EnterSafeStateAsync() => RaiseAsync("SAFE_STATE", FaultSeverity.Critical, "supervisor");

    private Task RaiseAsync(string code, FaultSeverity severity, string source, string? payload = null)
        => _faults.RaiseAsync(new FaultEvent
        {
            DeviceId = _opts.DeviceId,
            Code = code,
            Severity = severity,
            Category = code.StartsWith("ESTOP", StringComparison.Ordinal) ? FaultCategory.Safety : FaultCategory.Comm,
            Source = source,
            CorrelationId = Guid.NewGuid().ToString("N"),
            Fingerprint = $"{_opts.DeviceId}|{code}",
            Payload = payload
        });

    private static TimeSpan Backoff(int n) =>
        TimeSpan.FromSeconds(Math.Min(300, Math.Pow(2, n)))
            .Add(TimeSpan.FromMilliseconds(Random.Shared.Next(0, 500)));   // 抖动防惊群

    public async ValueTask DisposeAsync()
    {
        if (_protocol is IAsyncDisposable ad) await ad.DisposeAsync();
        _telemetry.Writer.TryComplete();
        _critical.Writer.TryComplete();
    }
}
