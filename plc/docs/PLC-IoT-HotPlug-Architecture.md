# 可热插拔 .NET PLC IoT / AIOT 平台框架架构设计

> 面向对象：大型软件系统的整体架构设计与长期演进规划  
> 技术基线：.NET 11 / C# 14+（高级特性优先）  
> 目标形态：PLC 物联网 AIOT 平台 + 上位机（HMI），base-field apps 优先做 MVP  
> 设计基调：Modular Monolith 底座 → 微服务演进；Vertical Slice + 插件 AssemblyLoadContext（ALC）

---

## 0. 设计总纲（目标 / 约束 / 原则）

### 0.1 一句话定位

构建一个「**底座 + 能力 + 插件**」三位一体的可热插拔框架：底座提供生命周期、隔离、DI、配置、可观测；能力以插件形式挂载；开发者用**统一入口**接入、**统一出口**消费，全程践行**三高一低**与**零分配**。

### 0.2 必须兑现的非功能目标

| 维度    | 目标                | 落地抓手                                                                          |
| ----- | ----------------- | ----------------------------------------------------------------------------- |
| 高可用   | 单插件故障不拖垮宿主；支持优雅降级 | 插件独立 ALC 隔离、故障域隔离、Polly 弹性                                                    |
| 高可靠   | 协议通信不丢、不重、不乱序     | 幂等、确认重传、环形缓冲 + 事务日志刷盘                                                         |
| 高性能   | 高吞吐、低尾延迟          | Span/Memory 零拷贝、Channel通道、Dataflow、Pipe、对象池、RingBuffer+Disruptor              |
| 低部署成本 | 小镜像、快启动、易迁移       | 模块化单体 + Docker、AOT 可选、按需加载                                                    |
| 零分配   | 热路径零 GC 压力        | `Span<T>`/`Memory<T>`、`ArrayPool`、`RecyclableMemoryStream`、Pipeline、Channel背压 |

### 0.3 架构风格选择（为什么是 Modular Monolith 起步）

- **MVP 阶段**用模块化单体：一个进程内，按垂直切片（Vertical Slice）组织，协议驱动（Modbus/BACnet/Fatek/Fuji/Melsec/Keyence/Omron…）作为**插件**加载。部署简单、调试容易、成本最低。
- **演进阶段**把高负载/独立演进的切片抽成微服务（用 Dapr 边车或 YARP 网关切分），底座契约不变，迁移零代码改动。
- 关键：底座的 `Abstractions`（契约）永不依赖实现，保证「单体 ⇄ 微服务」双向可逆。

---

## 1. 框架底座部分

### 1.1 系统架构（分层 + 模块化 + 垂直切片）

```
┌──────────────────────────────────────────────────────────────────────┐
│                          宿主 Host 进程 (ASP.NET Core)                  │
│  ┌────────────────────────────────────────────────────────────────┐  │
│  │  契约层 PlcPlatform.Abstractions (仅接口/DTO/枚举, 零实现)        │  │
│  └────────────────────────────────────────────────────────────────┘  │
│  ┌─────────────── 底座核心 (Base) ──────────────────────────────────┐ │
│  │ PluginManager │ ServiceRegistry │ ConfigCenter │ Telemetry │ DI   │ │
│  │ CapabilityRegistry │ EventBus │ Scheduler │ Security                │ │
│  └────────────────────────────────────────────────────────────────┘  │
│  ┌── 插件隔离区（每个插件一个 collectible ALC + 独立子容器）──────────┐ │
│  │  [ModbusPlugin ALC] [BACnetPlugin ALC] [AIServicePlugin ALC] ...  │ │
│  └────────────────────────────────────────────────────────────────┘  │
│  ┌── 适配层 Adapters（协议/序列化/缓存/消息 的统一外观）─────────────┐ │
│  └────────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────────┘
                         │ 容器化 (Docker) │ 编排 (K8s / Tye / Aspire)
```

- **接口层（契约）**：`PlcPlatform.Abstractions`，被宿主和所有插件共同引用，是类型共享的唯一边界。
- **实现层（底座核心）**：宿主内置，负责插件生命周期、隔离、DI、配置、监控。
- **组件层（插件/适配器）**：协议、AI、媒体、报表等能力，运行时热插拔。

### 1.2 核心设计理念

1. **热插拔（核心）**：基于 .NET 的 `AssemblyLoadContext`（collectible）实现「加载—激活—卸载—重载」四态机。每个插件独立 ALC，依赖隔离；宿主共享契约类型（`PreferSharedTypes` / `sharedTypes`）解决跨上下文类型转换。
2. **三高一低**：见 0.2。
3. **零分配热路径**：所有 I/O 与协议解析走 `System.IO.Pipelines` + `ReadOnlySequence<byte>` + `Span<T>`，避免 `byte[]` 拷贝；对象用 `ArrayPool<T>`/`ObjectPool<T>`/`RecyclableMemoryStream` 复用。
4. **容器化与可观测**：宿主本身容器友好（最小 API + 健康检查 `/health`）；指标/追踪走 OpenTelemetry，日志走 Serilog→Seq。

### 1.3 底座架构（关键选型）

| 关注点   | 推荐实现                                                                                    | 说明                                      |
| ----- | --------------------------------------------------------------------------------------- | --------------------------------------- |
| 服务发现  | Consul / etcd（或 `Microsoft.Extensions.ServiceDiscovery优先`）                              | 进程内能力注册表为主，跨节点用 Consul                  |
| 配置中心  | AgileConfig / Consul KV                                                                 | 支持热更新、灰度                                |
| 高性能消息 | Disruptor .NET（RingBuffer 多生产者多消费者）、SpanNetty / DotNetty                                | 进程内高频事件用 Disruptor；网络传输用 SpanNetty（零拷贝） |
| 宿主    | ASP.NET Core（模块化 + 自动加载）                                                                | 用 `IHostedService` 承载插件生命周期             |
| 动态加载  | `AssemblyLoadContext` + 反射 + Castle.Core（动态代理）                                          | Castle 用于 AOP 拦截（日志/熔断）                 |
| 内存优化  | `Memory<T>`/`Span<T>` + Dataflow(`System.Threading.Tasks.Dataflow`) + Pipelines+Channel | 流式处理、背压、零拷贝                             |

### 1.4 底座组件

- **插件管理系统（PluginManager）**：负责扫描目录 → 校验清单（manifest/签名）→ 创建 ALC → 反射发现 `IPlugin` → 构建子容器 → 启停 → 卸载（含 GC 回收观测）。
- **插件接口库（Abstractions）**：定义 `IPlugin`、`ICapability`、`IEventBus`、`ICapabilityRegistry`、DTO、枚举。
- **依赖注入**：默认 MS.DI（`Microsoft.Extensions.DependencyInjection`）+Scrutor注入+Cater模块化，重场景用 Autofac（#280）做模块化注册。
- **附加底座**：能力注册表、配置中心客户端、统一遥测出口、调度器（Hangfire/Coravel）、安全（OpenIddict/Casbin）。

---

## 2. 能力部分设计

### 2.1 核心服务

- **服务注册与发现**：进程内 `CapabilityRegistry` 为主，跨进程/跨节点走 Consul + `Microsoft.Extensions.ServiceDiscovery优先`（#277）。
- **统一能力与生命周期管理接口**：所有能力声明为 `ICapability`，由 PluginManager 编排。

### 2.2 能力组件（按领域，对应你清单中的选型）

- **协议/通信**：Modbus/OPC UA/BACnet/Fatek/Fuji/Melsec/Keyence/Omron（你仓库已有 `*.cs` 驱动，直接包成协议插件）；上位机通讯 LOIC/llcom（#139/#140）；网络 SpanNetty/DotNetty/SuperSocket/TouchSocket（#200–#204）。
- **消息**：MQTTnet + MQTTnet.EventBus（#15）、ZeroMQ（#17）、MassTransit + RabbitMQ（CQRS/Saga）、csharp-nats（#184）、CAP（#186）。
- **数据**：SQLite（#49）、LiteDB 日志库（#50）、EF Core + Dapper 读写分离 + DapperAOT（#52）、Ardalis.Specification 规约（#51）、ZoneTree（#170）。
- **缓存**：Hybrid Cache混合缓存,Garnet（Redis 协议，#7）、FusionCache 混合缓存（#10）、EasyCaching（#9）、StackExchange.Redis（#8）。
- **序列化**：进程内 MemoryPack（#18），跨进程 MessagePack（#24）/System.Text.Json（#23）/SpanJson（#25）。
- **AI/AIGC**：MS.AI、Semantic Kernel（#223）、kernel-memory、OllamaSharp（#218）、AntSK 知识库（#220）、LongChain（#219）。
- **媒体/SIP/流媒体**：SIPSorcery（#57）、GB28181（#158）、JT808Gateway（#159）、LibVLCSharp/Xabe.FFmpeg（#63/#64）、ZLMRTC/SRSManager（#69/#70）。
- **视觉/OCR**：OpenCvSharp/EmguCV/ImageSharp（#146/#150/#149）、PaddleSharp OCR（#71）、YOLOv16（#154）。
- **调度/任务**：Hangfire+Redis+NCrontab（#72）、Coravel（#77）、Quartz 思路（FluentScheduler #73）。
- **安全/认证**：OpenIddict/Keycloak（#95/#97）、Casbin.Net 授权（#99）、BouncyCastle 加解密（#113）、AspNetCoreDataProtection（#134）。
- **可观测**：OpenTelemetry（#32/#279）、AppMetrics（#27）、MiniProfiler（#28）、Seq/Serilog（#110/#112）、WatchDog（#30）。
- **客户端/桌面**：Avalonia/Photino/Blazor Hybrid（#208/#206/#209）、MAUI（Silky #142）。
- **文档/报表**：QuestPDF/FastReport（#225/#181）、MiniExcel/ClosedXML（#125/#126）。

### 2.3 插件化

每个能力=一个插件工程，输出独立目录、独立 ALC。遵循统一接口规范，由 PluginManager 管理生命周期，支持热插拔与版本共存。

### 2.4 扩展服务（基础件）

日志（Serilog/Exceptionless）、配置（AgileConfig/Consul）、缓存（FusionCache）、数据库访问（EF Core/Dapper）、HTTP 弹性客户端（`Microsoft.Extensions.Http.Resilience` #278 + Polly #229）。

### 2.5 适配器设计模式

用 Adapter 把不同第三方实现收敛成统一外观：例如 `IProtocolAdapter` 屏蔽 Modbus/BACnet 差异；`ICacheAdapter` 屏蔽 Garnet/Redis/本地；`IMessageAdapter` 屏蔽 MQTT/RabbitMQ/NATS。插件只依赖抽象，底座负责注入具体实现。

---

## 3. 插件标准入口

### 3.1 契约（Abstractions，零实现）

```csharp
// PlcPlatform.Abstractions
namespace PlcPlatform.Abstractions;

public interface IPlugin
{
    PluginManifest Manifest { get; }
    // 标准入口：向子容器注册能力，仅能拿到宿主提供的共享服务
    ValueTask ConfigureServicesAsync(IPluginContext context, CancellationToken ct);
    ValueTask StartAsync(CancellationToken ct);
    ValueTask StopAsync(CancellationToken ct);
}

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = false)]
public sealed class PluginAttribute : Attribute
{
    public string Id { get; }
    public string Version { get; }
    public string? EntryType { get; set; }   // 可选：指定 IPlugin 实现类
    public PluginAttribute(string id, string version) { Id = id; Version = version; }
}

public sealed class PluginManifest
{
    public string Id { get; init; } = "";
    public string Version { get; init; } = "1.0.0";
    public string[] Capabilities { get; init; } = [];
    public string[] Dependencies { get; init; } = [];
    public string? Description { get; init; }
}
```

### 3.2 标准入口流程

1. **发现**：PluginManager 扫描插件目录 → `AssemblyLoadContext.LoadFromAssemblyPath` → 反射查找 `[Plugin]` 与 `IPlugin` 实现。
2. **隔离**：每个插件一个 collectible ALC，`PreferSharedTypes` 共享 `Abstractions` 类型，依赖冲突自动隔离。
3. **注册（入口）**：调用 `ConfigureServicesAsync`，插件只能向**自己的子容器**注册，且只能通过 `IPluginContext.SharedServices` 获取宿主能力（不能直接 new 宿主内部类型）。
4. **激活**：`StartAsync` 启动后台通道/监听器。
5. **元数据校验**：`PluginAttribute` + manifest 保证 Id/版本/依赖准确；签名校验防不可信插件。

### 3.3 宿主加载器（生产级骨架）

```csharp
public sealed class PluginSlot : IDisposable
{
    public IPlugin Plugin { get; }
    public PluginLoadContext Alc { get; }
    public IServiceProvider Provider { get; }
    public PluginSlot(IPlugin plugin, PluginLoadContext alc, IServiceProvider provider)
        => (Plugin, Alc, Provider) = (plugin, alc, provider);
    public void Dispose()
    {
        (Provider as IDisposable)?.Dispose();
        Alc.Unload();
        // 关键：强制回收以释放 ALC（否则内存泄漏）
        for (int i = 0; i < 3; i++) { GC.Collect(); GC.WaitForPendingFinalizers(); }
    }
}

public sealed class PluginManager
{
    private readonly ConcurrentDictionary<string, PluginSlot> _slots = new();
    private readonly IServiceProvider _hostServices;

    public async Task LoadAsync(string pluginDir, CancellationToken ct)
    {
        var alc = new PluginLoadContext(pluginDir);          // collectible
        var asm = alc.LoadFromAssemblyPath(Path.Combine(pluginDir, FindDll(pluginDir)));
        var pluginType = asm.GetTypes().First(t => typeof(IPlugin).IsAssignableFrom(t));
        var plugin = (IPlugin)Activator.CreateInstance(pluginType)!;

        var services = new ServiceCollection();
        var ctx = new PluginContext(services, _hostServices); // 仅暴露共享服务
        await plugin.ConfigureServicesAsync(ctx, ct);
        var provider = services.BuildServiceProvider();
        await plugin.StartAsync(ct);

        _slots[plugin.Manifest.Id] = new PluginSlot(plugin, alc, provider);
    }

    public async Task UnloadAsync(string id)
    {
        if (_slots.TryRemove(id, out var slot))
        {
            await slot.Plugin.StopAsync(CancellationToken.None);
            slot.Dispose();   // 卸载 ALC
        }
    }
}
```

> 关键坑：**类型标识**。跨 ALC 的类型无法互转，必须把契约程序集设为共享类型（放在宿主 `wwwroot`/共享目录并用 `PreferSharedTypes`）。不可信插件建议升级为 sidecar 进程（进程级隔离）。

---

## 4. 插件标准出口

出口 = 插件**不直接调用**其它插件/宿主内部，而是统一通过底座抽象「流出」结果，保证可替换与零耦合。

### 4.1 三类标准出口

```csharp
// 1) 能力注册出口：插件把自己能提供的 ICancellable 能力挂到全局注册表
public interface ICapabilityRegistry
{
    void Register(string capabilityId, object implementation);
    T? Resolve<T>(string capabilityId) where T : class;
    IReadOnlyCollection<string> Listing();
}

// 2) 事件出口：进程内 MemoryPack 零分配，跨进程自动桥接 MQ
public interface IEventBus
{
    ValueTask PublishAsync<T>(T @event, CancellationToken ct) where T : class;
    IDisposable Subscribe<T>(Func<T, CancellationToken, ValueTask> handler);
}

// 3) 统一服务出口：插件通过 DI 取宿主能力（反向）
public interface IHostServices
{
    T GetRequiredService<T>() where T : notnull;
}
```

### 4.2 出口约定

- 插件**产出**统一走 `IEventBus` / `ICapabilityRegistry`；**消费**统一走 `IHostServices`（DI 注入）。
- 遥测出口统一到 OpenTelemetry（计数/追踪/指标 #279），不允许插件私自接第三方 APM。
- 日志出口统一 Serilog，插件只 `ILogger<T>`，不直连 Seq/文件。

### 4.3 零分配数据出口示例（Pipe + Span）

```csharp
// 协议报文解析：从 PipeReader 读，用 Span 解析，全程不分配 byte[]
async ValueTask ProcessAsync(PipeReader reader, CancellationToken ct)
{
    while (!ct.IsCancellationRequested)
    {
        ReadResult result = await reader.ReadAsync(ct);
        ReadOnlySequence<byte> buffer = result.Buffer;
        while (TryParseFrame(ref buffer, out var frame))   // frame 是 Span<byte>
        {
            Handle(frame);                                  // 零拷贝处理
        }
        reader.AdvanceTo(buffer.Start, buffer.End);
        if (result.IsCompleted) break;
    }
}
```

---

## 5. 推荐集成框架（含 GitHub / 优缺点 / 推荐理由）

### 5.1 插件 / 热插拔「核心框架」重点对比（你清单 #81–#85）

| 框架                                     | GitHub                                              | 定位                       | 优点                                                              | 缺点                                               | 推荐理由                                               |
| -------------------------------------- | --------------------------------------------------- | ------------------------ | --------------------------------------------------------------- | ------------------------------------------------ | -------------------------------------------------- |
| **DotNetCorePlugins** (natemcmaster)   | <https://github.com/natemcmaster/DotNetCorePlugins> | 基于 ALC 的官方级插件加载器         | 类型共享/隔离精细、支持热重载（FileSystemWatcher）、MVC/Razor 插件、v2.x 支持 .NET 8+ | 卸载依赖 collectible ALC（需谨慎 GC）、netstandard2.0 不受支持 | **首选底座插件引擎**：与本文 ALC 方案完全契合，省去底层加载代码，自带 hot reload |
| **Oqtane** (oqtane/oqtane.framework)   | <https://github.com/oqtane/oqtane.framework>        | 模块化 Blazor 应用框架（.NET 10） | 动态页面合成、多租户、模块市场、.NET Foundation 背书                              | 偏 CMS/Web，非 IoT 协议场景；体积大                         | 若上位机走 Blazor Hybrid/Web，可直接复用其模块化与模块市场机制           |
| **NetPro** (LeonKou/NetPro)            | <https://github.com/LeonKou/NetPro>                 | 低侵入、可插拔中间件集合             | 引用即初始化、按需引用、不强依赖；易转微服务                                          | 维护节奏偏 6.0；生态较个人化                                 | 适合做「能力组件按需引用」的封装范式参考                               |
| **Foundatio** (FoundatioNet/Foundatio) | <https://github.com/FoundatioNet/Foundatio>         | 可插拔基础块（缓存/队列/日志/Job）     | 抽象统一、可换底层实现、轻量                                                  | 偏通用基建，不含 ALC 热插拔                                 | 做「能力插件内部的可替换基础件」很合适（#81）                           |
| **MEF** (System.Composition，内置)        | dotnet/runtime                                      | 组合式部件发现                  | 零额外依赖、声明式 `[Export]/[Import]`                                   | 不支持 collectible 卸载、热插拔弱                          | 仅用于进程内静态模块组合，不作为主热插拔方案                             |

**结论**：底座热插拔引擎 = **DotNetCorePlugins（ALC 隔离 + 热重载）** + 自研 `PluginManager` 编排；模块化 UI 参考 **Oqtane**；基础件可替换性参考 **Foundatio**；中间件封装范式参考 **NetPro**。

### 5.2 能力框架「分类索引」（对应你清单 282 项，每类给出首选 + GitHub）

> 282 项逐一展开会成书，下面按关注点归类，标注该类**首选**与对应条目号，便于你按 MVP 优先级取用。

| 分类        | 首选                                       | GitHub                                              | 覆盖条目                          |
| --------- | ---------------------------------------- | --------------------------------------------------- | ----------------------------- |
| 插件/热插拔    | DotNetCorePlugins                        | natemcmaster/DotNetCorePlugins                      | #81–#85, #280                 |
| API/端点    | FastEndpoints + Carter                   | FastEndpoints/FastEndpoints, CarterCommunity/Carter | #1,#2,#103,#46                |
| 对象映射      | Mapster + Mapperly                       | MapsterMapper/Mapster, Riokapp/Mapperly             | #3,#4                         |
| gRPC/实时   | MagicOnion + protobuf-net + SignalR      | Cysharp/MagicOnion, protobuf-net                    | #42,#43,#44                   |
| 消息/事件总线   | MassTransit + MQTTnet.EventBus           | MassTransit/MassTransit, dotnet/MQTTnet             | #15,#186,#79,#80              |
| 序列化       | MemoryPack + MessagePack                 | Cysharp/MemoryPack, MessagePack-CSharp              | #18,#24,#25                   |
| 缓存        | Garnet + FusionCache                     | microsoft/garnet, ZiggyCreatures/FusionCache        | #7,#10,#8,#9                  |
| 数据/ORM    | EF Core + Dapper(AOT) + Specification    | ardalis/Specification                               | #49–#56,#52                   |
| ID/时间     | Ulid + Snowflake + NodaTime              | ulid-net, Snowflake, NodaTime                       | #86–#91,#101                  |
| 认证/授权     | OpenIddict + Keycloak + Casbin.Net       | openiddict, keycloak, CasbinNet                     | #93–#100,#134                 |
| 调度/Job    | Hangfire + Coravel                       | HangfireIO/Hangfire, coravel                        | #72–#78                       |
| SIP/流媒体   | SIPSorcery + GB28181 + ZLMRTC            | sipsorcery-org, GB28181/GB28181.Solution            | #57–#70,#158–#160             |
| 视觉/OCR    | OpenCvSharp + PaddleSharp + ImageSharp   | shimat/opencvsharp, PaddleSharp                     | #71,#146–#157,#154            |
| AI/AIGC   | MEAI + OllamaSharp + AntSK               | microsoft/semantic-kernel, ollama-sharp             | #217–#224                     |
| 网络/Socket | SpanNetty + TouchSocket + MsQuic         | SpanNetty, TouchSocket                              | #119–#123,#200–#204           |
| 服务治理      | Dapr + Aspire + Consul + Polly           | dapr, dotnet/aspire, Polly-Contrib                  | #227–#242,#229,#278           |
| 文档/报表     | QuestPDF + MiniExcel + FastReport        | QuestPDF/QuestPDF, mini-software                    | #125–#127,#181,#225,#226      |
| 可观测       | OpenTelemetry + Seq + MiniProfiler       | open-telemetry/opentelemetry-dotnet                 | #27–#34,#279,#110–#112        |
| 桌面/客户端    | Avalonia + Photino + Blazor Hybrid       | AvaloniaUI/Avalonia, tryphotino/photino             | #206–#215                     |
| 安全/加解密    | BouncyCastle + SecurityHeaders + Captcha | bc-csharp, rjmurillo/security-headers               | #113–#117,#131–#135,#266–#271 |
| 规则/ETL/爬虫 | NRules + ChoETL + DotnetSpider           | NRules/NRules, DotNetSpider                         | #257–#260,#248–#250           |
| 动态编译/脚本   | Natasha + Fody + Rougamo + CS-Script     | dotnetcore/Natasha, Fody, CS-Script                 | #115–#117,#262–#264           |
| 图表/词云     | OxyPlot + jieba.NET                      | oxyplot, jieba.NET                                  | #251–#256                     |

### 5.3 与你现有 PLC 驱动的结合点

你仓库已有 `Modbus.cs / BACnet.cs / Fatek.cs / Fuji.cs / Melsec.cs / Keyence.cs / Omron.cs / MQTT.cs` 等——这些**天然就是协议插件**。把它们各自抽成实现 `IProtocolAdapter` 的插件工程，统一入口注册到 `CapabilityRegistry`，上位机/AIOT 平台通过 `IHostServices` 解析调用，即可实现「新增一种 PLC 协议 = 丢一个 DLL 进插件目录」的热插拔体验。

---

## 6. 优缺点分析与落地路线

### 6.1 优点

- **模块化 / 可扩展**：协议、AI、媒体能力即插即用，新增 PLC 协议零改动宿主。
- **快速交付**：Vertical Slice + 现成能力插件，MVP 周期短。
- **高可用 / 高可靠**：ALC 故障域隔离、事件溯源 + 幂等、Disruptor 抗背压。
- **高性能 / 零分配**：Span/Memory、Pipe、对象池、RingBuffer，热路径低 GC。
- **低部署成本**：模块化单体起步，Docker 一键部署，按需加载。

### 6.2 缺点（与缓解）

- **部署复杂**：微服务/容器化需 K8s 知识 → MVP 先用模块化单体，演进期再切。
- **维护成本**：服务间依赖与通信复杂 → 契约先行（`Abstractions`），用 Dapr 边车降耦。
- **ALC 卸载风险**：卸载不干净会内存泄漏 → 强制 GC 回收观测 + sidecar 兜底。
- **跨 ALC 类型转换坑** → 共享契约程序集 + `PreferSharedTypes`。

### 6.3 落地路线

1. **MVP（0–3 月）**：模块化单体 + DotNetCorePlugins + 现有 PLC 驱动包成协议插件 + SQLite + MQTTnet + OpenTelemetry。
2. **增强（3–6 月）**：FusionCache/Garnet、MassTransit CQRS、Hangfire 调度、MEAI质检。
3. **演进（6 月+）**：Dapr 边车 + YARP 网关切微服务；不可信插件转 sidecar 进程；AOT 关键路径。

---

## 附录 A：零分配「协议帧解析」完整骨架（Pipe + Dataflow + ArrayPool）

```csharp
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks.Dataflow;

// 生产者：串口/网口读入 → Pipe
// 消费者：TransformBlock 用 Span 解析，零分配
var parseBlock = new TransformBlock<ReadOnlyMemory<byte>, Frame>(
    mem => ParseFrame(mem.Span),  // Span<byte> 解析，不拷贝
    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4, BoundedCapacity = 1024 });

static Frame ParseFrame(ReadOnlySpan<byte> span)
{
    // 直接在 span 上做 CRC/长度校验与字段提取，零分配
    var pool = ArrayPool<byte>.Shared;
    // 仅在必须落盘时才 Rent，用完 Return
    return new Frame(span[0], span.Slice(1, 4).ToArray()); // 示意
}
```

## 附录 B：关键依赖版本建议（.NET 10 基线）

- `McMaster.NETCore.Plugins` ≥ 2.0（.NET 8+，建议 .NET 10 重新编译）
- `Microsoft.Extensions.*` 与 SDK 同版本
- 序列化优先 `MemoryPack/MessagePack`；网络优先 `SpanNetty`；缓存优先 `Garnet`/`FusionCache`

---

*本设计以「契约隔离 + ALC 热插拔 + 零分配热路径」为骨架，兼顾你清单中 282 项能力的可插拔集成路径。落地时优先把现有 PLC 驱动转为协议插件，即可在最短时间内验证底座价值。*
