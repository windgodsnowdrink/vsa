using System.Diagnostics;
using System.IO;
using PlcAiot.Abstractions.Devices;
using PlcAiot.Abstractions.Faults;
using PlcAiot.Abstractions.Logging;
using PlcAiot.Devices;
using PlcAiot.Faults;
using PlcAiot.Faults.Recovery;
using PlcAiot.Supervisor;

var logger = new ConsoleLogger("host");
var once = args.Contains("--once");
var runSeconds = once ? 12 : int.MaxValue;

// 1) 加载 DeviceProfile（换型 = 换此 JSON，零代码改动，§1.12.1）
var profilePath = Path.Combine(AppContext.BaseDirectory, "profiles", "plc-a1.modbus.json");
var profile = await ProfileLoader.LoadFromJsonAsync(profilePath);
logger.Log(LogLevel.Information,
    $"loaded profile {profile.DeviceId} ({profile.Protocol}/{profile.Transport}, {profile.Points.Count} points)");

// 2) 设备注册表 + 热替换能力（§1.12.4）
var catalog = new DeviceCatalog();
catalog.RegisterProfile(profile);

// 3) 故障存储：热(内存) + 冷(文件 JSONL) 双写。
//    接真实 PostgreSQL/InfluxDB 双写，只需把下面一行换成（见 PlcAiot.Faults.Persistent）：
//    var store = new DualFaultStore(new PostgreSqlFaultStore(connStr), new InfluxDbFaultStore(url, token, org, bucket));
var hot = new InMemoryFaultStore();
var cold = new FileFaultStore(Path.Combine(AppContext.BaseDirectory, "faults.jsonl"));
var store = new DualFaultStore(hot, cold, new ConsoleLogger("store"));

// 4) 恢复策略注册（§1.13.3）
var recovery = new RecoveryRegistry();
recovery.Register(new ReconnectStrategy(catalog));
recovery.Register(new ReinitSessionStrategy(catalog));
recovery.Register(new FailoverStrategy());
recovery.Register(new SafeStateStrategy());
var faultSvc = new FaultService(store, recovery, new ConsoleLogger("fault"));

// 5) 长期运行 GC 基线（§1.11.4）
GcStability.ApplyLongRunningBaseline();

// 6) 监督者托管每设备会话（§1.11.2）
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var protocol = catalog.Resolve(profile.DeviceId);
await protocol.ConfigureAsync(profile, cts.Token);
var opts = new DeviceOptions { DeviceId = profile.DeviceId, WatchdogTimeout = TimeSpan.FromSeconds(2) };
var sup = new DeviceSessionSupervisor(opts, protocol, faultSvc, new ConsoleLogger($"sup:{profile.DeviceId}"));
sup.Start(cts.Token);

// 7) 演示：4s 后注入一次 COMM_TIMEOUT，验证自动恢复闭环
_ = Task.Run(async () =>
{
    await Task.Delay(4000, CancellationToken.None);
    logger.Log(LogLevel.Information, "demo: 注入 COMM_TIMEOUT 演示自动恢复");
    await faultSvc.RaiseAsync(new FaultEvent
    {
        DeviceId = profile.DeviceId,
        Code = "COMM_TIMEOUT",
        Severity = FaultSeverity.Error,
        Category = FaultCategory.Comm,
        Source = "demo",
        CorrelationId = Guid.NewGuid().ToString("N"),
        Fingerprint = $"{profile.DeviceId}|COMM_TIMEOUT"
    });
});

logger.Log(LogLevel.Information, $"running ({(once ? $"{runSeconds}s 后自动退出" : "Ctrl+C 停止")})");
var sw = Stopwatch.StartNew();
while (!cts.IsCancellationRequested && sw.Elapsed.TotalSeconds < runSeconds)
{
    await Task.Delay(1000, cts.Token);
    logger.Log(LogLevel.Information, $"alive={sup.IsAlive(TimeSpan.FromSeconds(3))} uptime={sw.Elapsed.TotalSeconds:F0}s restarts={sup.Restarts}");
}
cts.Cancel();
await Task.Delay(500, CancellationToken.None);
await sup.DisposeAsync();
if (protocol is IAsyncDisposable ad) await ad.DisposeAsync();

// 8) 查询演示：回看本设备全部故障（§1.13.5）
var recent = await store.QueryAsync(new FaultQuery { DeviceId = profile.DeviceId, MinSeverity = FaultSeverity.Info });
logger.Log(LogLevel.Information, $"recorded faults: {recent.Count}");
foreach (var f in recent)
    logger.Log(LogLevel.Information, $"  - {f.Code} [{f.Status}] {f.Severity} @ {f.OccurredAt:HH:mm:ss}");

// 9) 点表导入/导出演示（互换性，§1.12.5）
await using var ms = new MemoryStream();
var exchange = new CsvPointTableExchange(catalog);
await exchange.ExportAsync(profile.DeviceId, ms);
ms.Position = 0;
var imported = await exchange.ImportAsync(ms);
logger.Log(LogLevel.Information, $"point-table export/import round-trip OK: {imported.Points.Count} points");

logger.Log(LogLevel.Information, "demo finished");
