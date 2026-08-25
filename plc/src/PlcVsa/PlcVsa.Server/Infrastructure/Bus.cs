// ────────────────────────────────────────────────────────────────────────────
// §6 Disruptor-net RingBuffer（BusProducer + BusEvent + 三消费者）
// ────────────────────────────────────────────────────────────────────────────
using System.Runtime.CompilerServices;
using System.Text;
using Disruptor;
using Microsoft.Extensions.Logging;
using PlcVsa.Server.Infrastructure;
using Seq64 = System.Int64;

namespace PlcVsa.Server.Infrastructure;

public enum BusTopic : byte
{
    ProtocolFrame = 1, LogEntry = 2, PluginEvent = 3, UserCommand = 4,
}

public sealed class BusEvent
{
    public BusTopic Topic;
    public Seq64 Sequence;
    public long EpochMs;

    public ProtocolFrame? Frame;
    public LogLevel Level;
    public string? LogCategory;
    public string? LogMessage;
    public Exception? LogException;
    public string? PluginId;
    public string? PluginAction;
    public string? DeviceId;
    public string? Command;
    public string? PayloadJson;

    public void Reset()
    {
        Topic = 0; Sequence = 0; EpochMs = 0;
        Frame = null;
        Level = 0; LogCategory = null; LogMessage = null; LogException = null;
        PluginId = null; PluginAction = null;
        DeviceId = null; Command = null; PayloadJson = null;
    }
}

public sealed class BusProducer
{
    private readonly Disruptor<BusEvent> _disruptor;
    public BusProducer(Disruptor<BusEvent> d) => _disruptor = d;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PublishFrame(ProtocolFrame f)
    {
        using var scope = _disruptor.PublishEvent();
        var e = scope.Event();
        e.Reset();
        e.Topic = BusTopic.ProtocolFrame;
        e.EpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        e.Frame = f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PublishLog(LogLevel l, string cat, string msg, Exception? ex = null)
    {
        using var scope = _disruptor.PublishEvent();
        var e = scope.Event();
        e.Reset();
        e.Topic = BusTopic.LogEntry;
        e.EpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        e.Level = l;
        e.LogCategory = cat;
        e.LogMessage = msg;
        e.LogException = ex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PublishPluginEvent(string id, string action)
    {
        using var scope = _disruptor.PublishEvent();
        var e = scope.Event();
        e.Reset();
        e.Topic = BusTopic.PluginEvent;
        e.EpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        e.PluginId = id;
        e.PluginAction = action;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PublishUserCommand(string deviceId, string cmd, string json)
    {
        using var scope = _disruptor.PublishEvent();
        var e = scope.Event();
        e.Reset();
        e.Topic = BusTopic.UserCommand;
        e.EpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        e.DeviceId = deviceId;
        e.Command = cmd;
        e.PayloadJson = json;
    }
}

/// <summary>日志消费者：写入 32MB 滚动文件。</summary>
public sealed class LogFileHandler : IEventHandler<BusEvent>
{
    private readonly string _logDir;
    private const long _maxBytes = 32 * 1024 * 1024;
    private Stream? _stream;
    private long _written;
    private int _fileSeq;
    public LogFileHandler()
    {
        _logDir = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(_logDir);
    }

    public void OnEvent(BusEvent e, long sequence, bool endOfBatch)
    {
        if (e.Topic != BusTopic.LogEntry) return;
        EnsureStream();
        var line = $"[{UnixMsToDate(e.EpochMs)}] [{e.Level}] [{e.LogCategory}] {e.LogMessage}{e.LogException}{Environment.NewLine}";
        var buf = Encoding.UTF8.GetBytes(line);
        _stream!.Write(buf);
        _written += buf.Length;
        if (endOfBatch) _stream.Flush();
        if (_written >= _maxBytes) Rotate();
    }
    private void EnsureStream() => _stream ??= File.OpenWrite(Path.Combine(_logDir, $"bus-{_fileSeq:0000}.log"));
    private void Rotate() { _stream?.Dispose(); _stream = null; _written = 0; _fileSeq++; }
    private static string UnixMsToDate(long ms) =>
        DateTimeOffset.FromUnixTimeMilliseconds(ms).LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
}

/// <summary>协议帧 → 消息记忆（持久化）。</summary>
public sealed class ProtocolMemoryHandler : IEventHandler<BusEvent>
{
    private readonly ProtocolMemoryStore _store;
    public ProtocolMemoryHandler(ProtocolMemoryStore s) => _store = s;

    public void OnEvent(BusEvent e, long sequence, bool endOfBatch)
    {
        if (e.Topic != BusTopic.ProtocolFrame || e.Frame is null) return;
        e.Frame.Seq = sequence;
        e.Frame.ComputeChecksum();
        _store.Append(e.Frame);
    }
}

/// <summary>插件事件 → 结构化日志输出。</summary>
public sealed class PluginEventHandler : IEventHandler<BusEvent>
{
    private readonly ILogger _logger;
    public PluginEventHandler(ILoggerFactory lf) => _logger = lf.CreateLogger<PluginEventHandler>();
    public void OnEvent(BusEvent e, long sequence, bool endOfBatch)
    {
        if (e.Topic != BusTopic.PluginEvent) return;
        _logger.LogInformation("Plugin {Id} {Action} @ {Seq}", e.PluginId, e.PluginAction, sequence);
    }
}
