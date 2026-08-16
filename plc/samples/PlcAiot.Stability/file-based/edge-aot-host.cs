// edge-aot-host.cs — .NET 10 File-based App（瘦边缘 AOT 宿主样例）
// 运行：  dotnet run edge-aot-host.cs
// 发布 AOT： dotnet publish edge-aot-host.cs -c Release -r linux-x64 /p:PublishAot=true
//
// 场景：边缘网关上的轻量采集宿主，要求「小镜像、快启动、低 GC 停顿、7×24 不假死」。
// 解决方案：NativeAOT 单文件 + 常驻采集循环 + 关键批次 NoGCRegion 零停顿 + CancellationToken 优雅停机。
// 技术要点：System.Threading.Lock（AOT 安全轻量锁）、GC.TryStartNoGCRegion（临界区零停顿）、
//          顶层语句即程序入口、顶层 await 即 async Main。

using System;
using System.Threading;
using System.Threading.Tasks;

// Ctrl+C / 容器 SIGTERM → 优雅停机（不停留在中途状态）
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var host = new EdgeCollector("PLC-A1", cts.Token);
var runTask = host.RunAsync();

Console.WriteLine("边缘 AOT 宿主已启动（Ctrl+C 退出）…");
await runTask;

// 瘦边缘采集宿主：单设备一个常驻任务；崩溃由外层容器编排重启。
sealed class EdgeCollector
{
    private readonly string _deviceId;
    private readonly CancellationToken _stop;
    private readonly Lock _gate = new();      // .NET 9+ 轻量锁，AOT/trimming 安全
    private int _batch;

    public EdgeCollector(string deviceId, CancellationToken stop) => (_deviceId, _stop) = (deviceId, stop);

    public async Task RunAsync()
    {
        try
        {
            while (!_stop.IsCancellationRequested)
            {
                await Task.Delay(1000, _stop);  // 模拟轮询周期（真实为协议 Poll）
                CollectBatch();
            }
        }
        catch (OperationCanceledException) { /* 优雅停机，不抛 */ }
        Console.WriteLine($"[{_deviceId}] 已优雅停机");
    }

    // 采集→组批→转发的临界区用 NoGCRegion 包住，避免 GC 停顿影响实时性
    private void CollectBatch()
    {
        if (GC.TryStartNoGCRegion(64 * 1024))           // 申请 64KB 内不发生 GC
        {
            try
            {
                var n = Interlocked.Increment(ref _batch);
                lock (_gate) { /* 合并写共享缓冲（AOT 安全的 Lock） */ }
                Console.WriteLine($"[{_deviceId}] 批次 #{n} 采集完成（NoGCRegion 内）");
            }
            finally { GC.EndNoGCRegion(); }              // 必须配对释放
        }
        else
        {
            Console.WriteLine($"[{_deviceId}] 内存压力下跳过 NoGCRegion，走常规采集");
        }
    }
}
