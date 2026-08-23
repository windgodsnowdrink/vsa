// 集成草图（非编译产物，仅示意如何接入 plc-host 的 ICapabilityModule 契约）。
// 重要前提：CsGo 因 control_strand/send_control 耦合 System.Windows.Forms，
// 只能以 net11.0-windows 目标框架引入，仅限 Windows 部署。
using System.Threading;
using System.Threading.Tasks;
using Go;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plc.Host.Foundation;

namespace Plc.Host.Modules;

/// <summary>
/// Tier2 能力模块：把 CsGo 的协作式 CSP 调度器作为宿主级并发基础设施暴露。
/// 新增能力 = 新增一个实现类，Scrutor 自动发现（与现有 cache.hybrid / sys.info 一致）。
/// </summary>
public sealed class CsGoCapability : ICapabilityModule
{
    public string Id => "concurrency.csgo";
    public string Name => "CsGo CSP 并发调度";
    public int Order => 50; // 多数能力在其后再注册

    public void RegisterServices(IServiceCollection services)
    {
        // 单例封装：work_service 事件循环 + 共享 strand（同 strand 内任务线程安全）
        services.AddSingleton<CsGoScheduler>();
        services.AddHostedService(sp => sp.GetRequiredService<CsGoScheduler>());
    }
}

/// <summary>
/// 承载 CsGo work_service.run() 事件循环的后台服务；
/// AIOT 的 MQTT 订阅可经 strand 派发，业务逻辑用 generator.go / chan 串联。
/// </summary>
public sealed class CsGoScheduler : IHostedService
{
    private readonly work_service _work = new();
    private readonly shared_strand _strand = new work_strand(new work_service());
    private Thread? _loop;

    /// <summary>供其它能力模块派发 goroutine / 创建 chan 的 strand。</summary>
    public shared_strand Strand => _strand;

    public Task StartAsync(CancellationToken ct)
    {
        _loop = new Thread(() => _work.run()) { IsBackground = true, Name = "CsGoScheduler" };
        _loop.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct)
    {
        _work.stop();          // 置 _runSign=false，run() 在队列排空后退出
        return Task.CompletedTask;
    }
}

/* 典型用法（示意）：
   generator.go(_scheduler.Strand, async () =>
   {
       chan<MqttEnvelope> q = chan<MqttEnvelope>.make(_scheduler.Strand, 64);
       var children = new generator.children();
       children.go(_scheduler.Strand, () => MqttConsumer(q));   // 消费侧
       children.go(_scheduler.Strand, () => MqttProducer(q));   // 订阅侧
       await children.wait_all();
   });
*/
