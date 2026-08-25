using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using Plc.Plugins.Contracts;
using StreamJsonRpc;
using System.IO.Pipes;

namespace PlcGateway;

public record DeviceCfg(
    string Id,
    string Protocol,
    string Host,
    int Port,
    int? UnitId,
    int? Rack,
    int? Slot,
    int PollIntervalMs,
    bool Enabled);

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // 1) File-based app：从 app.manifest.json 装配（需求 2）
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("app.manifest.json", optional: false)
            .Build();

        var gateway = config.GetSection("gateway");
        var pluginsDir = gateway["plugins:directory"] ?? "plugins";
        var pipeName = gateway["rpc:pipeName"] ?? "plc-gateway";
        var ringSize = int.Parse(gateway["pipeline:ringBufferSize"] ?? "16384");
        var consumers = int.Parse(gateway["pipeline:consumerThreads"] ?? "2");

        // 2) 服务容器 + 日志；宿主自身即 IDeviceCatalog，注册进容器以供插件解析
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddConsole());
        services.AddSingleton<GatewayHost>();
        services.AddSingleton<IDeviceCatalog>(sp => sp.GetRequiredService<GatewayHost>());
        var sp = services.BuildServiceProvider();
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Gateway");

        // 3) 宿主 + 插件（需求 3/5：DotNetCorePlugins）
        var host = sp.GetRequiredService<GatewayHost>();
        host.Register(new PlcGateway.Protocols.SimProtocol()); // 内置保底协议
        host.LoadPlugins(Path.Combine(AppContext.BaseDirectory, pluginsDir));

        // 4) Disruptor 流水线（需求 4）
        await using var pipeline = new FramePipeline(ringSize, consumers, logger);

        // 5) vs-threading：背压信号量 + 可连接任务工厂（需求 6）
        var semaphore = new AsyncSemaphore(consumers);
        var jtf = new JoinableTaskContext().Factory;
        using var cts = new CancellationTokenSource();

        // 6) 按 manifest 注册设备并周期性采集
        // 直接读取 app.manifest.json 文件并用 System.Text.Json 反序列化，完全规避 ConfigurationBinder
        var devices = new List<DeviceCfg>();
        var manifestPath = Path.Combine(AppContext.BaseDirectory, "app.manifest.json");
        if (File.Exists(manifestPath))
        {
            using var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(manifestPath));
            if (doc.RootElement.TryGetProperty("gateway", out var gw) &&
                gw.TryGetProperty("devices", out var devsEl))
            {
                var jsonOpts = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                devices = System.Text.Json.JsonSerializer.Deserialize<List<DeviceCfg>>(devsEl.GetRawText(), jsonOpts) ?? new();
            }
        }
        logger.LogInformation("manifest 解析到 {Count} 个设备，启用 {Enabled}",
            devices.Count, devices.Count(x => x.Enabled));
        // 所有设备注册进宿主设备目录，供 RPC 按设备 id 路由（启用与否均可按需读写）
        foreach (var d in devices) host.RegisterDevice(d);
        foreach (var d in devices.Where(x => x.Enabled))
        {
            _ = jtf.RunAsync(async () =>
            {
                var protocol = host.Get(d.Protocol);
                if (protocol is null)
                {
                    logger.LogWarning("设备 {Id} 的协议 {Protocol} 未注册，跳过", d.Id, d.Protocol);
                    return;
                }
                while (!cts.IsCancellationRequested)
                {
                    using var releaser = await semaphore.EnterAsync(cts.Token);
                    try
                    {
                        await using var session = protocol.CreateSession(
                            new DeviceConnectionOptions(d.Host, d.Port, (byte)(d.UnitId ?? 1)));
                        var data = await session.ReadAsync(new ReadRequest(0, 10, RegisterType.HoldingRegister), cts.Token);
                        pipeline.Publish(new RawFrame
                        {
                            DeviceId = d.Id,
                            Timestamp = DateTime.Now,
                            Payload = data,
                            Note = d.Protocol
                        });
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "采集失败: {Id}", d.Id);
                    }

                    try { await Task.Delay(d.PollIntervalMs, cts.Token); }
                    catch (OperationCanceledException) { break; }
                }
            });
        }

        // 7) StreamJsonRpc 暴露 RPC（需求 7）：循环接受 NamedPipe 客户端连接
        logger.LogInformation("PLC 网关已启动，RPC 监听 NamedPipe: {Name}", pipeName);
        logger.LogInformation("已注册协议: {Protocols}", string.Join(", ", host.All.Select(p => p.ProtocolId)));

        while (!cts.IsCancellationRequested)
        {
            var pipe = new NamedPipeServerStream(
                pipeName, PipeDirection.InOut, 8, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            try
            {
                await pipe.WaitForConnectionAsync(cts.Token);
            }
            catch (OperationCanceledException) { break; }

            var rpc = JsonRpc.Attach(pipe, new GatewayRpc(host, pipeline));
            logger.LogInformation("RPC 客户端已连接");
            try
            {
                await rpc.Completion;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RPC 会话结束");
            }
            pipe.Dispose();
        }
    }
}
