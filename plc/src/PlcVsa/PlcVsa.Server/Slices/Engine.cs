// ────────────────────────────────────────────────────────────────────────────
// Slice: Engine（PlcVsaEngine：Disruptor 启动/停止 + 采样循环）
// 对应 8 项需求 §4 Disruptor-net RingBuffer 生产/消费 三管道
// ────────────────────────────────────────────────────────────────────────────
using Disruptor;
using Disruptor.Dsl;
using Microsoft.Extensions.Logging;
using MVT = Microsoft.VisualStudio.Threading;
using PlcVsa.Contracts.Devices;
using PlcVsa.Server.Infrastructure;
using U16 = System.UInt16;

namespace PlcVsa.Server.Slices;

public sealed class PlcVsaEngine : global::System.IAsyncDisposable
{
    private readonly ILogger<PlcVsaEngine> _log;
    private readonly ILoggerFactory _lf;
    private readonly IDeviceCatalog _catalog;
    private readonly ProtocolMemoryStore _memory;
    private readonly Disruptor<BusEvent> _disruptor;
    private readonly BusProducer _producer;
    private readonly MVT.JoinableTaskFactory _jtf;
    private Task? _samplerLoop;
    private readonly CancellationTokenSource _cts = new();

    public BusProducer Bus => _producer;
    public ProtocolMemoryStore MessageMemory => _memory;

    public object RingStats => new
    {
        RingSize = PlcVsaConfig.RING_SIZE,
        Cursor = _disruptor.RingBuffer?.Cursor ?? -1,
        Protocols = _catalog.All.Count,
        MemoryDir = Path.GetFullPath(PlcVsaConfig.PROTOCOL_MEMORY_DIR),
    };

    public PlcVsaEngine(
        ILogger<PlcVsaEngine> log,
        ILoggerFactory lf,
        IDeviceCatalog catalog,
        ProtocolMemoryStore memory,
        MVT.JoinableTaskContext jtc)
    {
        _log = log; _lf = lf; _catalog = catalog; _jtf = jtc.Factory;
        _memory = memory;

        _disruptor = new Disruptor<BusEvent>(
            () => new BusEvent(),
            PlcVsaConfig.RING_SIZE,
            TaskScheduler.Default,
            ProducerType.Multi,
            new BlockingWaitStrategy());

        _producer = new BusProducer(_disruptor);

        _disruptor.HandleEventsWith(
            new LogFileHandler(),
            new ProtocolMemoryHandler(_memory),
            new PluginEventHandler(lf));
    }

    public async Task StartAsync()
    {
        _disruptor.Start();
        _log.LogInformation("Disruptor RingBuffer started (size={N})", PlcVsaConfig.RING_SIZE);
        _samplerLoop = SamplerLoop(_cts.Token);
        await Task.CompletedTask;
    }

    private async Task SamplerLoop(CancellationToken ct)
    {
        var devices = new[] { "line-A-cnc-01", "line-B-robot-07", "utility-boiler-3" };
        var protos = new[] { "siemens-s7-1200", "modbus-tcp", "melsec-mc", "omron-fins" };
        var rand = new Random();
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(1000, ct);
                var d = devices[rand.Next(devices.Length)];
                var p = protos[rand.Next(protos.Length)];
                var payload = new byte[8];
                rand.NextBytes(payload);
                _producer.PublishFrame(new ProtocolFrame
                {
                    Protocol = p,
                    DeviceId = d,
                    StartAddr = (U16)rand.Next(0, 5000),
                    Count = (U16)rand.Next(1, 10),
                    Rt = RegisterType.HoldingRegister,
                    Payload = payload,
                    Ts = DateTimeOffset.UtcNow,
                });
                _producer.PublishLog(LogLevel.Debug, "Sampler", $"{d}@{p} 采样完成");
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _producer.PublishLog(LogLevel.Error, "Sampler", "采样异常", ex);
            }
        }
    }

    public async Task StopAsync()
    {
        _cts.Cancel();
        if (_samplerLoop is not null) await _samplerLoop;
        _disruptor?.Shutdown(TimeSpan.FromSeconds(5));
        _log.LogInformation("PlcVsaEngine stopped");
    }

    public async System.Threading.Tasks.ValueTask DisposeAsync()
    {
        await StopAsync();
        _cts.Dispose();
    }
}
