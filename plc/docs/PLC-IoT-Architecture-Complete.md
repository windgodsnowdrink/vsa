# 可热插拔 .NET PLC IoT / AIOT 平台 — 总体架构设计（V9 长期运行稳定性版 · .NET 11 Preview.7 基线）

> 面向对象：大型软件系统的整体架构设计与长期演进规划  
> 技术基线：.NET 11 Preview.7 / C# 14（原生 AOT · 零分配 · 可观测优先）  
> 目标形态：PLC 物联网 AIOT 平台 + 上位机（HMI），base-field apps 优先做 MVP  
> 设计基调：Modular Monolith 底座 → 微服务演进；Vertical Slice + 插件 AssemblyLoadContext（ALC）  
> 本文档合并自 `PLC-IoT-HotPlug-Architecture.md`（底座/能力/入口/出口/框架推荐）与 `PLC-IoT-Design-Decisions.md`（具体选型决策与代码骨架），并迭代补充：工业级生产流水线、YARP 网关（Aneiang.Yarp 的 WAL/WAF）、实时图表、状态机解析流程、Modbus 详细实现、UI 产品体系、miniapi 实例、.NET 内置组件约束、PLC 生产级踩坑清单、管理/控制/数据三平面与南北向流量划分。  
> **V7（评审加固）**：依据 `PLC-IoT-Architecture-Review.md` 的架构评审，落实三条铁律——**① 先画域，再选技**（§1.6 领域上下文地图 + DDD 模型）；**② 热插拔的敌人是卸载**（§3.5 插件生命周期清理契约）；**③ OT 平台南向即红线**（§1.8 南向安全基线）；并补齐 **G2 一致性模型（域内/集成事件）**、**G11 测试架构（xUnit/Aspire/Testcontainers）**、**G10 急停覆盖**、**G6 背压单闸门**，修正 F1–F5 事实错误。  
> **V8（后端加固）**：以「可靠后端架构」为核心，补齐四支柱——**① 可扩展**（垂直切片 + 契约版本演进 + 微服务抽取缝，§1.10.2）；**② 高性能**（.NET 11 Preview.7 NativeAOT + 零分配热路径 + `System.Threading.Lock` + `Tensor<T>`，§1.10.3）；**③ 插件式**（ALC 隔离 + 清理契约 `IPlugin:IAsyncDisposable` + sidecar 兜底，§1.10.4）；**④ 可靠**（liveness/readiness/startup 三探针 + 优雅停机 + 弹性 + 幂等 + 混沌，§1.10.5）。新增 OpenTelemetry 三信号可观测（§1.10.6 / §23.3）、API 版本化与限流（§23.4 / §23.5）、横向扩展（无状态 + 会话外置 + 负载均衡，§1.10.7），技术基线统一到 **.NET 11 Preview.7 + C# 14**（§23）。  
> **V9（长期运行稳定性）**：聚焦 PLC AIOT 平台最看重的「**7×24 长时间运行稳定 + 高可用**」「**协议/PLC 设备的可扩展与更换通用性**」「**故障可追溯·恢复·解决·查询**」三大主题（ADR-009/010/011）。**① 长时间运行稳定性与高可用（§1.11）**：监督者+看门狗自愈重启、长期运行 GC（DATAS/低延迟）、连接租约防泄漏、过载保护与降级、HA（多副本 + PostgreSQL 咨询锁领导者选举 + 边缘主备 + 降级服务）、崩溃恢复与状态重建；**② 协议与 PLC 设备可扩展与互换（§1.12）**：`DeviceProfile` 点表描述文件 + `IDeviceProtocol` 统一契约 + 能力协商 + `DeviceCatalog` 热替换 + 通用点表导入导出，新增 PLC=新增 Profile 不改代码；**③ 故障全生命周期管理（§1.13）**：`FaultEvent` + 全链路 `correlationId` 可追溯、按故障码映射的 `IRecoveryStrategy` 自动化恢复、解决状态机（Open→Ack→Resolved→Closed）、`IFaultStore` 多维查询 + 冷热分层存储 + MTTR 看板。**④ 代码落地（§24）**：新增 §24「关键技术点 .NET 11 Preview.7 实现样例（File-based Apps）」——把上一轮拟拆建的两个独立 csproj 宿主工程（瘦边缘 AOT 宿主 / 标准 JIT 插件宿主）改为两个 **.NET 11 Preview.7 File-based App 单文件示例**（`dotnet run x.cs` 直跑，已验证），并按「场景 → 解决方案 → 技术要点 → 代码」补齐长稳/互换/故障三大支柱的关键 .NET 代码。**⑤ 前端产品体系扩充（§17）**：重组 §17 前端体系——新增 §17.0 设计系统（科技天蓝/玻璃态/统一布局规范）、§17.5 社媒矩阵追踪（单 HTMX 文件）、§17.6 Notion 风格团队仪表板（Live Artifact 自包含离线）、§17.7 多端适配（PC B/S Blazor Auto / 桌面 Blazor Hybrid+WebView2 / 移动 MAUI Hybrid + MQTT 客户端）、§17.8 服务端三段匹配与统一布局规范（操作≤3层/路径统一）；并产出 4 个可运行自包含 HTML 样板（`samples/PlcAiot.Web/`：落地页/驾驶舱/Notion 仪表板）。**⑥ 边缘数据链路与驾驶舱增强（§17.2 / §17.9）**：明确边缘数据架构——**边缘库采用 SQLite**（EF Core + DapperAOT，单文件 ACID、ARM 网关零运维）；**业务数据采用 Eclipse Mosquitto** 做边缘 MQTT 订阅/发布并桥接转发至中心 EMQX 集群；**数据同步采用 CommunityToolkit.Datasync** 做离线优先增量同步与冲突解决（设备注册表/配置/告警）。同时将 `cockpit.html` 运营驾驶舱从单页扩展为 **8 页 SPA**（驾驶舱/设备/告警/统计/实时看板/地图/系统/后端管理）：**地图页用 Three.js 渲染全球 3D 地球 + 园区设备点 + 枢纽弧线**，**实时看板页用 Three.js 程序化 PLC 机架 3D 结构透视（模块着色 + 拾取交互）**，并预留 Blender MCP 生成高保真模型 → 导出 glTF → GLTFLoader 加载的生产管线。

---

## 0. 设计总纲（目标 / 约束 / 原则）

### 0.1 一句话定位

构建一个「**底座 + 能力 + 插件**」三位一体的可热插拔框架：底座提供生命周期、隔离、DI、配置、可观测；能力以插件形式挂载；开发者用**统一入口**接入、**统一出口**消费，全程践行**三高一低**与**零分配**。

### 0.2 必须兑现的非功能目标

| 维度    | 目标                | 落地抓手                                                                           |
| ----- | ----------------- | ------------------------------------------------------------------------------ |
| 高可用   | 单插件故障不拖垮宿主；支持优雅降级 | 插件独立 ALC 隔离、故障域隔离、Polly 弹性                                                     |
| 高可靠   | 协议通信不丢、不重、不乱序     | 幂等、确认重传、环形缓冲 + 事务日志刷盘                                                          |
| 高性能   | 高吞吐、低尾延迟          | Span/Memory 零拷贝、Channel 通道、Dataflow、Pipe、对象池、RingBuffer + Disruptor            |
| 低部署成本 | 小镜像、快启动、易迁移       | 模块化单体 + Docker、AOT 可选、按需加载                                                     |
| 零分配   | 热路径零 GC 压力        | `Span<T>`/`Memory<T>`、`ArrayPool`、`RecyclableMemoryStream`、Pipeline、Channel 背压 |

### 0.3 架构风格选择（为什么是 Modular Monolith 起步）

- **MVP 阶段**用模块化单体：一个进程内，按垂直切片（Vertical Slice）组织，协议驱动（Modbus/BACnet/Fatek/Fuji/Melsec/Keyence/Omron…）作为**插件**加载。部署简单、调试容易、成本最低。
- **演进阶段**把高负载/独立演进的切片抽成微服务（用 Dapr 边车或 YARP 网关切分），底座契约不变，迁移零代码改动。
- 关键：底座的 `Abstractions`（契约）永不依赖实现，保证「单体 ⇄ 微服务」双向可逆。

### 0.4 全局技术选型速查

| 关注点        | 决策                                                         | 备注 / GitHub                                                |
| ------------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| 插件/热插拔   | **DotNetCorePlugins**（ALC 隔离 + 热重载）+ 自研 PluginManager | natemcmaster/DotNetCorePlugins                               |
| 模块化 UI     | Oqtane（Blazor 模块化）                                      | oqtane/oqtane.framework                                      |
| 基础件可替换  | Foundatio                                                    | FoundatioNet/Foundatio                                       |
| AI            | **Microsoft.Extensions.AI (MEAI)** + **MCP** + **Qdrant**    | dotnet/extensions · modelcontextprotocol/csharp-sdk · qdrant/qdrant-dotnet |
| PLC 状态机    | **Stateless**（Configure/Permit/Fire）                       | dotnet-state-machine/stateless                               |
| AIOT 传输     | **MQTT + EMQX**                                              | emqx/emqx                                                    |
| 消息队列      | `System.Threading.Channels` 生产者/消费者 + **单闸门 BoundedChannel 背压**（跨节点才用 Redis 深度计数） | 订阅/发布队列                                                |
| 弹性          | **Polly v8**：超时/熔断/舱壁/优先级/死信+重试                | App-vNext/Polly                                              |
| CQRS          | **Mediator(martinothamar, MIT)**（进程内, 源码生成, NativeAOT）+ **Outbox/Inbox**（Wolverine 事务发件箱） | martinothamar/Mediator · WolverineFx/wolverine               |
| IOC           | **Scrutor** 程序集扫描 + 外观 AOP                            | khellang/Scrutor                                             |
| 模块化端点    | **Carter**                                                   | CarterCommunity/Carter                                       |
| API 网关      | **YARP**                                                     | microsoft/reverse-proxy                                      |
| 实时图表      | **ScottPlot**（优先）/ **LiveCharts2**                       | ScottPlot/ScottPlot · beto-rodriguez/LiveCharts2             |
| 协议帧        | `\| 帧头\|长度\|命令字\|Payload\|校验\|帧尾\|`；解析引擎/帧分发器抽象隔离 | —                                                            |
| 抽象层级      | driver 驱动 / device 设备 / pipeline 通道 / registry 注册中心 | —                                                            |
| Modbus        | **NModbus4**（上游 `NModbus/NModbus`）+ 遵守 `IProtocolAdapter` | NModbus/NModbus                                              |
| OPC           | **OPC Foundation.NetStandard.Opc.Ua** + **OpcUaHelper** + 遵守 `IProtocolAdapter` | OPCFoundation/UA-.NETStandard                                |
| S7            | **s7netplus** + 遵守 `IProtocolAdapter`                      | S7NetPlus/s7netplus                                          |
| 设备配置      | json + Options 运行时                                        | —                                                            |
| 心跳/健康     | `HealthCheck`（设备心跳 + 存活探针）                         | —                                                            |
| 流式输出      | **IAsyncEnumerable** + **EnumeratorCancellation**            | —                                                            |
| 生命周期      | **BackgroundService** + **IHostedService** 编排 → Dataflow   | —                                                            |
| 数据库        | SQLite+EFCore+DapperAOT（边缘）/ InfluxDB（时序）/ PostgreSQL（业务）/ Seq+ClickHouse（日志） | —                                                            |
| 前端          | HTMX + Tailwind CSS + Blazor Hybrid + WebApi；MVVM + Rx.NET 发布 Mqtt；Mqtt.js 订阅渲染 | —                                                            |
| 可观测        | **OpenTelemetry**（Logs/Metrics/Traces）+ Aspire Dashboard 可观测一体化 | dotnet/opentelemetry-dotnet · dotnet/aspire                  |
| API 设计      | Minimal API + `TypedResults` + 版本化(`/v1`)+ OpenAPI 自动生成(`Microsoft.AspNetCore.OpenApi`) | dotnet/aspnetcore · 见 §23.4                                 |
| 限流          | **System.Threading.RateLimiting**（SlidingWindow/TokenBucket 内置）+ AspNetCoreRateLimit | dotnet/runtime · 见 §23.5                                    |
| 缓存          | **HybridCache**（L1 内存 + L2 分布式·踩踏保护）+ Garnet/Redis | dotnet/extensions · microsoft/Garnet                         |
| 健康检查      | liveness / readiness / startup 三探针 + 优雅停机（`IHostApplicationLifetime`） | —                                                            |
| 横向扩展      | 无状态后端 + 会话外置(Redis/PostgreSQL) + 负载均衡 + 容器/Aspire 编排 | —                                                            |
| 设备协议互换  | **`DeviceProfile` 点表描述文件**(JSON/YAML) + **`IDeviceProtocol`** 统一契约 + 能力协商 `DeviceCapabilities` + `DeviceCatalog` 设备注册表热替换 | 换型=更新 Profile 指针，上层零改动（见 §1.12）               |
| 长期运行 GC   | **DATAS**（`DOTNET_gcDynamicAdaptation`=1）+ `GCSettings.LatencyMode=SustainedLowLatency` + `GC.TryStartNoGCRegion` | .NET 11 Preview.7 长时间运行低停顿基线（见 §1.11.4）         |
| 看门狗/自愈   | **`DeviceSessionSupervisor`** 每设备独立 Task + 指数退避重启 + 熔断防 flapping + 心跳看门狗 | 单设备会话崩溃不拖垮宿主（见 §1.11.2 / §1.11.3）             |
| 领导者选举    | **PostgreSQL 咨询锁**(`pg_advisory_lock`) / **Dapr Actor 提醒** 用于单例职责(Outbox relay) 选主 | 多副本中仅一个实例承担单例任务（见 §1.11.6）                 |
| 故障存储/查询 | **`FaultEvent`** + **`IFaultStore`**(InfluxDB 时序 + PostgreSQL 关系，冷热分层) + 多维查询(设备/时间/级别/码/状态/指纹) | 故障可追溯·恢复·解决·查询（见 §1.13）                        |
| 自动化恢复    | **`IRecoveryStrategy`** 注册表，按故障码映射(重试/重连/重初始化/故障转移/安全态) + 指数退避 + 升级人工 | 故障自愈闭环，急停相关走安全态（见 §1.13.3 / §7.0 G10）      |
| 向量/AI       | **Microsoft.Extensions.VectorData**（RAG 向量检索）+ MEAI    | dotnet/extensions · 见 §13                                   |
| 原生 AOT      | **Native AOT** 宿主（小镜像/快启动/低攻击面）+ trimming 安全 | 受 ALC 插件约束(见 §23.2)                                    |

---

## 0.5 优先采用 .NET 内置框架与官方/社区组件（选型约束 a–f）

> 基调：**能用 ASP.NET Core / Microsoft.Extensions.* 内置能力就不引第三方**；确需第三方时优先官方/社区、MIT 协议、AOT 友好、零分配。以下逐条落实你的约束 a–f。

### 0.5.1 服务发现 / 弹性 / 限流 / 缓存（约束 a）

```csharp
// ① 服务发现：优先 Microsoft.Extensions.ServiceDiscovery（.NET 8+ 官方）
builder.Services.AddServiceDiscovery();
builder.Services.AddHttpClient("plc-backend")
    .AddServiceDiscovery();              // 自动解析 https+http://plc-backend 等服务名

// ② 弹性 + 幂等：Microsoft.Extensions.Http.Resilience（基于 Polly v8 策略）
//    Retry(重试) / CircuitBreaker(熔断) / Timeout(超时) / Fallback(降级) / LoadBalancing(负载均衡)
builder.Services.AddHttpClient("aiot")
    .AddServiceDiscovery()
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 3;
        options.Timeout.Timeout = TimeSpan.FromSeconds(2);
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);
    });
// 备注：纯进程内弹性用 Microsoft.Extensions.Resilience；Http 客户端弹性用
//       Microsoft.Extensions.Http.Resilience（二者都基于 Polly v8 同一组策略对象）。

// ③ 限流：优先 AspNetCoreRateLimit（丰富策略）；亦可叠加 .NET 内置 RateLimiter
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ④ 混合缓存：Microsoft.Extensions.Caching.Hybrid（L1 内存 + L2 分布式，自带踩踏保护）
builder.Services.AddStackExchangeRedisCache(o =>
    o.Configuration = builder.Configuration.GetConnectionString("Redis"));  // L2 = Redis 协议
builder.Services.AddHybridCache(o =>
{
    o.MaximumPayloadBytes = 1 << 20;
    o.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(5),        // L2 TTL
        LocalCacheExpiration = TimeSpan.FromMinutes(1) // L1 TTL（更短）
    };
});
// 用法：await cache.GetOrCreateAsync($"dev:{id}", async _ => await repo.LoadAsync(id, ct));
//      命中 L1 零网络；并发同键只触发一次工厂调用（stampede 保护）；支持 RemoveByTagAsync。
//      另可挂 Microsoft.Extensions.Caching.StackExchangeRedis 作为 L2 实现。
```

### 0.5.2 中间件 Mediator（约束 a：MediatR 非 MIT，**改用 MIT 的 Mediator，自带 Source Generator**）

`MediatR` 自 v12+ 转为**商业许可（非 MIT）**；生产应改用 **`Mediator`(martinothamar/Mediator, MIT)**——API 与 MediatR 近似、自带 **Source Generator 源码生成**、零运行时反射、**完整 Native AOT 支持**、热路径零分配、自带 DI 注册与 `IPipelineBehavior` 横切。Wolverine 亦是 MIT 且额外提供 Outbox/Inbox（见 §11）。

> **核心区别 / 为何是「自带 SG 代码生成」**：Mediator 不需要运行时反射扫描程序集来发现 Handler。给 Handler 打上 `[EventHandler]` 特性后，其 Source Generator 在**编译期**就把 `handler → IMediator` 的映射与 DI 注册全部生成出来（`Mediator.SourceGenerator` 生成 `ServiceCollection` 扩展）。这意味着：① 启动更快；② AOT 下 Handler 注册不丢；③ 热路径无反射分配。

```csharp
// 安装（Source Generator 必须同项目引用，且 Target 为 net8+）
//   dotnet add package Mediator.Abstractions
//   dotnet add package Mediator.SourceGenerator   // 编译期生成，ISourceGenerator
builder.Services.AddMediator();   // 由 SG 生成的扩展：编译期注册所有标注 [EventHandler] 的 Handler

// ── 命令（写路径，CQRS）──
public record WriteRegisterCommand(string DeviceId, ushort Address, short Value) : ICommand;

// [EventHandler] = 触发 Source Generator 生成 DI 注册；无需手动 AddSingleton
[EventHandler]
public sealed class WriteRegisterHandler : ICommandHandler<WriteRegisterCommand>
{
    private readonly IRegistry _registry;
    public WriteRegisterHandler(IRegistry registry) => _registry = registry;
    public ValueTask Handle(WriteRegisterCommand cmd, CancellationToken ct)
    {
        var dev = _registry.Resolve(cmd.DeviceId) ?? throw new KeyNotFoundException(cmd.DeviceId);
        return dev.Adapter.WriteAsync(new WriteCommand(cmd.Address, cmd.Value), ct);
    }
}

// ── 通知（读路径/事件，多播）──
[EventHandler]
public sealed class DeviceDataChangedHandler : INotificationHandler<DeviceDataChanged>
{
    public ValueTask Handle(DeviceDataChanged n, CancellationToken ct)
    {
        // 推送到 MQTT / 更新缓存 / 写时序库（§9/§0.4）
        return ValueTask.CompletedTask;
    }
}

// 横切（日志/遥测/重试）：IPipelineBehavior<,> 手动 AddSingleton 注册，等价于 MediatR 行为
public sealed class LoggingBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
{
    public async ValueTask<TRes> Handle(TReq request, MessageHandlerDelegate<TReq, TRes> next, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try { return await next(request, ct); }
        finally { Metrics.Request(typeof(TReq).Name, sw.Elapsed); }
    }
}
// builder.Services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

**常见误区（生产级常识）**：Mediator 的 `ICommand`/`ICommandHandler` 与 MediatR 的 `IRequest`/`IRequestHandler` 命名不同，迁移时一并替换；`AddMediator()` 来自 SG 生成而非反射扫描，若 Handler 漏标 `[EventHandler]` 则运行时报「未注册」。**绝不混用 MediatR 与 Mediator**，二者接口不兼容。

### 0.5.3 HttpClientFactory 客户端工程（约束 a）

统一用 `IHttpClientFactory`/`AddHttpClient`（连接池复用、`PrimaryHandler` 复用、弹性 handler 链、可观测），禁止 `new HttpClient()`。AIOT 设备端 SDK、EMQX 管理 API、第三方 REST 均经工厂创建。

### 0.5.4 MVVM / 高性能 / 数据同步：CommunityToolkit 全家桶（约束 a）

```csharp
// MVVM：CommunityToolkit.Mvvm — ComponentModel / RelayCommand / Messenger / 依赖注入
//   同时是 Source Generator：partial class 自动生成 ObservableObject / [RelayCommand] / [ObservableProperty]
public partial class DeviceViewModel : ObservableObject
{
    [ObservableProperty] private double _temperature;
    [RelayCommand] private async Task RefreshAsync() => Temperature = await _device.ReadAsync();
    // Messenger 跨模块解耦：WeakReferenceMessenger.Default.Send(new DeviceUpdated(deviceId));
}

// 高性能：CommunityToolkit.HighPerformance — 零分配基础件（见 0.5.5）
// 业务编排：Aspire（.NET Aspire）做本地编排/可观测/服务默认配置
builder.AddServiceDefaults();      // Aspire 默认：OTel、重试、健康探针
// 服务边车：Dapr — 进程外 sidecar 承载服务调用/状态/发布订阅/密钥，底座契约不变即可切到 Dapr 边车
// PLC 客户端⇄服务端数据同步：CommunityToolkit.Datasync（离线优先、增量同步、冲突解决）
//   服务端：AddDatasyncControllers<TEntity>()；客户端：DatasyncClient 拉取/推送
```

### 0.5.5 高性能注册与内存管理（约束 d：SpanOwner/MemoryOwner/StringPool/ParallelHelper）

全部来自 `CommunityToolkit.HighPerformance`，热路径零 GC：

```csharp
using CommunityToolkit.HighPerformance;

// SpanOwner<T>：在栈上持有 Span，超出作用域自动归还（避免 Span 逃逸到堆）
using var owner = SpanOwner<int>.Allocate(1024);
var span = owner.Span;
BinaryPrimitives.WriteInt32BigEndian(span.AsBytes(), value);

// MemoryOwner<T>：IMemoryOwner，可跨 await 传递，Dispose 归还到池
using IMemoryOwner<byte> mem = MemoryOwner<byte>.Allocate(4096);
await socket.ReceiveAsync(mem.Memory, ct);

// StringPool：高频重复字符串（如协议命令字/状态码）复用，减少分配
ReadOnlySpan<char> key = StringPool.Shared.GetOrAdd("temperature");

// ParallelHelper：零分配并行（优于 Parallel.For 的委托分配）
ParallelHelper.For(0, points.Length, i => Normalize(ref points[i]));
```

### 0.5.6 Source Generator 优先（约束 c）

- **CommunityToolkit.Mvvm**：`[ObservableProperty]`/`[RelayCommand]`/`INotifyPropertyChanged` 全部编译期生成，无运行时反射。
- **Mediator(SourceGenerator)**：Handler/DI 编译期生成（0.5.2）。
- **System.Text.RegularExpressions 源生成**：`[GeneratedRegex]` 替代运行时 `new Regex`。
- **JSON 源生成**：`[JsonSerializable(typeof(T))]` 的 `partial JsonSerializerContext` 替代反射序列化。
- 原则：**凡能编译期确定的映射/校验/序列化，一律 Source Generator**，热路径与 AOT 场景零反射、零分配。

### 0.5.7 分布式技术：Flink 或 CSGO 纤程（约束 e，参照 `E:\WorkSpace\windgodsnowdrink\vsa\csp`）

- **CSGO 纤程（协程）/ 共享串行器**：项目地址 **<https://github.com/HAM-2015/CsGo\\\\\\\*\\\\\\\*。它是\\\\\\\*\\\\\\\*专为> C# 并发流程控制与运动控制设计的框架**，面向**工业自动化及机器视觉**开发——与本文 PLC/AIOT 场景高度契合。其本地参考实现见 `vsa/csp/Cs/shared_strand.cs` 的 `shared_strand` + `work_service` + `work_engine` + `work_engine_hosted_service`，用 **TPL Dataflow(`ActionBlock`, 串行 `MaxDegreeOfParallelism=1`) + `Nito.AsyncEx.AsyncContext` + `ThreadLocal<curr_strand>`** 实现「同一 strand 内串行、跨 strand 并行」的 Go 风格纤程模型。非常适合把**每个 PLC 设备/每条协议链路**放进一个独立 strand，天然做到「单设备内消息顺序、设备间并发、无锁无粘包」，与本文 §6.6「单次解析互不影响」完全一致；同时其运动控制原语可直接编排设备上位机的轴/IO 时序。
  ```csharp
  // 每个协议插件/设备持有一个 shared_strand → 同 strand 内 post 串行执行，免去 lock
  var strand = new shared_strand();
  strand.post(() => ParseFrame(buffer));   // 顺序入队，绝不与其他帧交错
  // 多设备：每个设备一个 strand + 一个 work_engine 线程，互不影响
  // 运动控制示例（CsGo）：strand.post(() => MoveAxis(axis, target, speed));  // 轴动作串行化
  ```
- **Flink（分布式流处理）**：跨节点、跨设备的海量点位流式计算用 **CSharpFlink**（`#245`）做分布式窗口聚合/告警；纤程负责单节点内高并发与运动控制编排，Flink 负责集群级流式管线，二者分层互补。

### 0.5.8 其它组件参照 `E:\WorkSpace\windgodsnowdrink\vsa` 代码（约束 f）

- 插件加载：`vsa/PluginLoadContext.cs`（自定义 `AssemblyLoadContext` + `AssemblyDependencyResolver` + 非托管 DLL 解析 + 强签名校验）。
- 中间件+MQTT 发布订阅：`vsa/mediatr_mqtt_integration_pubsub.cs`（MediatR `IPipelineBehavior` 桥接 MQTT 发布/订阅，可等价迁移到 MIT Mediator）。
- 网关配置：`vsa/YARP_Route_Configuration_Explained.md` / `YARP_Cluster_Configuration_Comparison.md`。
- 更多：`vsa/code`、`vsa/ai`、`vsa/Wafer`、`vsa/SimpleCsvImporter` 等目录下的生产级集成片段可直接复用。

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

| 关注点    | 推荐实现                                                        | 说明                                      |
| ------ | ----------------------------------------------------------- | --------------------------------------- |
| 服务发现   | Consul / etcd（或 `Microsoft.Extensions.ServiceDiscovery` 优先） | 进程内能力注册表为主，跨节点用 Consul                  |
| 配置中心   | AgileConfig / Consul KV                                     | 支持热更新、灰度                                |
| 高性能消息  | Disruptor .NET（RingBuffer 多生产者多消费者）、SpanNetty / DotNetty    | 进程内高频事件用 Disruptor；网络传输用 SpanNetty（零拷贝） |
| 宿主     | ASP.NET Core（模块化 + 自动加载）                                    | 用 `IHostedService` 承载插件生命周期             |
| 动态加载   | `AssemblyLoadContext` + 反射 + Castle.Core（动态代理）              | Castle 用于 AOP 拦截（日志/熔断）                 |
| 内存优化   | `Memory<T>`/`Span<T>` + Dataflow + Pipelines + Channel      | 流式处理、背压、零拷贝                             |
| API 网关 | **YARP**（反向代理/路由/负载均衡/限流）                                   | 单体⇄微服务切分、边车入口                           |

### 1.4 底座组件

- **插件管理系统（PluginManager）**：扫描目录 → 校验清单（manifest/签名）→ 创建 ALC → 反射发现 `IPlugin` → 构建子容器 → 启停 → 卸载（含 GC 回收观测）。
- **插件接口库（Abstractions）**：定义 `IPlugin`、`ICapability`、`IEventBus`、`ICapabilityRegistry`、DTO、枚举。
- **依赖注入**：默认 MS.DI（`Microsoft.Extensions.DependencyInjection`）+ Scrutor 扫描 + Carter 模块化，重场景用 Autofac 做模块化注册。
- **附加底座**：能力注册表、配置中心客户端、统一遥测出口、调度器（Hangfire/Coravel）、安全（OpenIddict/Casbin）。

### 1.5 三平面（管理 / 控制 / 数据）与南北向流量（V5 新增）

大型 PLC/AIOT 平台必须用 **控制平面 / 数据平面 / 管理平面** 三者分离来解耦「规则定义」「规则执行」「规则维护」，并明确 **南北向（North-South）流量** 边界。这是生产级架构的常识性划分（借鉴 SDN / 云原生网关模型）：

```
┌──────────────────────────────────────────────────────────────────────────┐
│                          北向流量（North / 外部 ⇄ 平台）                      │
│   运维/工艺人员 ──UI/API──▶ │ 管理平面 │  ──配置/规则──▶ │ 控制平面 │         │
│   设备/上位机  ──MQTT/OPC──▶                       │           │            │
└──────────────────────────────┬───────────────────┬───────────┬───────────┘
                                 │ 下发规则           │ 生成执行体 │
                                 ▼                    ▼           │
┌──────────────────────────────────────────────────────────────────────────┐
│                          南向流量（South / 平台 ⇄ 设备）                      │
│   数据平面（执行规则/处理真实流量）：协议解析→状态机→Channel→CQRS→存储→MQTT    │
│   数据平面 ──▶ 设备写指令（Modbus FC5/6、S7 写、OPC 写）│◀── 设备采集上行      │
└──────────────────────────────────────────────────────────────────────────┘
```

| 平面                  | 职责                                                  | 入口/出口                                                                     | 关键组件                                 |
| ------------------- | --------------------------------------------------- | ------------------------------------------------------------------------- | ------------------------------------ |
| **管理平面 Management** | 用户通过 UI/API **修改配置**（设备、寄存器、规则、告警阈值、用户权限）           | 入口：Blazor/MAUI UI + FastEndpoints/Carter REST；出口：写配置库（PostgreSQL）+ 触发控制平面 | §17 UI 体系、§18 miniapi、§0.5 RBAC/ABAC |
| **控制平面 Control**    | **解析配置 → 生成执行规则**（如轮询计划、转换映射、告警规则、状态机定义），并把它下发到数据平面 | 入口：配置变更事件（Mediator `INotification`）；出口：规则注册到 `IRegistry` / 调度器 / 数据平面     | §11 Mediator、§6 注册中心、§7 状态机          |
| **数据平面 Data**       | **执行规则、处理真实流量**：协议帧解析、设备状态机、Channel 流水线、写库、MQTT 发布  | 入口：南向设备流量（MQTT/OPC/Serial）；出口：北向数据（MQTT 主题树、InfluxDB、时序/业务库）              | §5 工业流水线、§6.1 解析引擎、§9 MQTT           |

**南北向流量要点（生产级常识）**：

- **北向（North-South）**：① 人 ⇄ 平台（UI/API 经 YARP/WAF 网关进入，见 §15）；② 设备 ⇄ 平台（MQTT/OPC 经 EMQX 接入）。北向是「控制/配置」与「采集上送」的边界，必须鉴权 + WAF + 限流。
- **南向（South）**：平台 ⇄ 设备（下发写指令、采集轮询）。南向流量是**真实生产流量**，强调低延迟、幂等、背压、断线重连；配置错误可能直接操作物理设备，故控制平面下发的规则需经 **审批/灰度**（管理平面）。
- **平面间解耦原则**：管理平面改配置**不直接碰设备**；它只产生「配置事件」→ 控制平面把它编译成「规则对象」→ 数据平面消费。这样配置热更新无需重启数据平面，符合「三高一低」与热插拔。

**关键代码：配置 → 规则 的三平面流转**

```csharp
// ① 管理平面：UI 提交设备配置（FastEndpoints，见 §18）
public record UpsertDeviceConfigCommand(ModbusConfiguration Config) : ICommand;
[EventHandler]
public sealed class UpsertDeviceConfigHandler : ICommandHandler<UpsertDeviceConfigCommand>
{
    public ValueTask Handle(UpsertDeviceConfigCommand cmd, CancellationToken ct)
    {
        // 写配置库（PostgreSQL），随后发布「配置已变更」通知 → 进入控制平面
        ConfigStore.Upsert(cmd.Config);
        Mediator.Publish(new DeviceConfigChanged(cmd.Config.DeviceId), ct); // 跨平面信号
        return ValueTask.CompletedTask;
    }
}

// ② 控制平面：监听配置变更，编译成可执行规则并注册到数据平面
[EventHandler]
public sealed class DeviceConfigChangedHandler : INotificationHandler<DeviceConfigChanged>
{
    public ValueTask Handle(DeviceConfigChanged n, CancellationToken ct)
    {
        var cfg = ConfigStore.Load(n.DeviceId);
        // 生成：轮询计划（TimeWheel）、寄存器批量读取计划、字节序转换映射、告警阈值
        var rule = RuleCompiler.Compile(cfg);          // 解析配置 → 生成规则
        Registry.RegisterRule(n.DeviceId, rule);        // 下发到数据平面（热更新，无需重启）
        return ValueTask.CompletedTask;
    }
}

// ③ 数据平面：消费规则，处理真实南向流量（见 §5/§6 流水线）
//    rule 一旦注册，BackgroundService 的轮询循环立即采用新规则，下一次采集即生效。
```

---

## 1.6 领域边界上下文地图与领域模型（G1：先画域，再选技）

V5 之前的隐忧是「技术栈决定一切、业务边界模糊」（评审 G1）。一个大型 PLC/AIOT 平台必须**先画一页边界上下文地图（Bounded Context Map, C4 容器级）**&#x9501;住边界，技术栈才不会反客为主。**铁律：跨上下文只允许经「领域事件 / API」通信，禁止直连对方数据库。**

### 1.6.1 七大数据边界上下文

| 上下文                 | 职责                | 归属插件/程序集                | 跨上下文规则             |
| ------------------- | ----------------- | ----------------------- | ------------------ |
| **设备资产 Asset**      | 设备/寄存器/连接元信息 CRUD | `PlcPlatform.Asset`     | 唯一设备真相源            |
| **采集遥测 Telemetry**  | 点位采样、时序写、上送       | `PlcPlatform.Telemetry` | 只读投影，不回写资产库        |
| **指令控制 Command**    | 南向下发写指令、回执对账      | `PlcPlatform.Command`   | 强一致+幂等，唯一允许动设备     |
| **规则告警 Rule/Alert** | 轮询计划、转换映射、阈值告警    | `PlcPlatform.Rule`      | 经控制平面编译下发（§1.5）    |
| **配置 Config**       | 设备/寄存器/用户/系统配置    | `PlcPlatform.Config`    | 改配置不直接碰设备          |
| **身份 IAM**          | 认证/授权/审计          | `PlcPlatform.IAM`       | 所有南向写必经 RBAC（§1.8） |
| **诊断 Diagnosis**    | AI 质检/故障诊断/知识库    | `PlcPlatform.Diagnosis` | 消费遥测+指令事件，只读       |

```
              ┌──────────────── 领域上下文地图 (Bounded Context) ────────────────┐
  管理平面→   │  Config ──▶ Control/Rule ──▶ Command ──▶ (南向 真实设备)          │
              │    │            │                 │                              │
              │    │            │                 ├──▶ Telemetry ──▶ InfluxDB     │
              │    │            │                 │       │                      │
              │  Asset ◀──(DeviceRegistered 域内事件缓存)──┘                      │
              │    │            │                                                 │
              │  IAM ◀─── 所有南向写指令鉴权 + 审计 ───┐                          │
              │    │            │                      │                         │
              │  Diagnosis ◀──(消费遥测/指令 集成事件, 只读)──┘                    │
              └──────────────────────────────────────────────────────────────────┘
   跨上下文 = 领域事件 / API  only；禁止直连对方数据库
```

> 每个协议插件（`IProtocolAdapter` 实现）归属 **Telemetry + Command** 上下文；它只通过 `IEventBus`/API 与 `Asset`、`Rule` 通信，不直连 `Asset` 库——`Telemetry` 通过 `DeviceRegistered` 域内事件在本地建立设备缓存。

### 1.6.2 领域模型（DDD 战术）核心代码

聚合根 `Device`、值对象 `DeviceId`/`TopicPath`、实体 `Register`、领域事件 `RegisterAdded`/`CommandIssued`/`DeviceStateChanged`：

```csharp
// 值对象：强类型设备 ID，杜绝原始字符串散落
public readonly record struct DeviceId(string Value)
{
    public static DeviceId Parse(string s) => new(s.Trim());
    public TopicPath Topic => TopicPath.For(this);   // 析造南向多段主题
}

// 值对象：MQTT 主题路径（南向多段主题，见 §9）
public readonly record struct TopicPath(string Continent, string Country, string Province,
    string City, string District, string Line, string Batch, string DeviceId)
{
    public static TopicPath For(DeviceId id) => ResolveGeo(id);  // 从设备元数据解析地理维度
    public string Telemetry => $"devices/{Continent}/{Country}/{Province}/{City}/{District}/{Line}/{Batch}/{DeviceId}/telemetry";
    public string Command   => $"devices/{Continent}/{Country}/{Province}/{City}/{District}/{Line}/{Batch}/{DeviceId}/command";
}

// 实体：寄存器定义（Modbus 寄存器标准定义见 §19.1）
public sealed class Register
{
    public string Name { get; init; } = "";
    public ushort Address { get; init; }
    public byte FunctionCode { get; init; }              // 仅写读功能码，写码自动映射(§8.3)
    public string DataType { get; init; } = "INT16";
    public ByteOrder ByteOrder { get; init; } = ByteOrder.ABCD;
    public double Scale { get; init; } = 1;
    public double Offset { get; init; } = 0;
}

// 聚合根：设备（一致性边界，唯一标识 DeviceId）
public sealed class Device : IAggregateRoot
{
    private readonly List<Register> _registers = new();
    private readonly List<IDomainEvent> _events = new();
    public DeviceId Id { get; }
    public IReadOnlyList<Register> Registers => _registers;
    public Device(DeviceId id) => Id = id;

    public void AddRegister(Register r)
    {
        _registers.Add(r);
        AddEvent(new RegisterAdded(Id, r.Name, r.Address));   // 域内事件（进程内）
    }
    public void ApplyCommand(WriteCommand cmd) =>
        AddEvent(new CommandIssued(Id, cmd.Address, cmd.Value, DateTime.UtcNow)); // 南向强一致下发(§1.7)

    public IReadOnlyList<IDomainEvent> DrainEvents()
    { var snap = _events.ToArray(); _events.Clear(); return snap; }
    private void AddEvent(IDomainEvent e) => _events.Add(e);
}

// 领域事件（域内，进程内经 Mediator INotification 分发，见 §1.7）
public interface IDomainEvent { DateTime OccurredOn { get; } }
public sealed record RegisterAdded(DeviceId DeviceId, string Name, ushort Address) : IDomainEvent
{ public DateTime OccurredOn => DateTime.UtcNow; }
public sealed record CommandIssued(DeviceId DeviceId, ushort Address, short Value, DateTime At) : IDomainEvent
{ public DateTime OccurredOn => At; }
public sealed record DeviceStateChanged(DeviceId DeviceId, string From, string To) : IDomainEvent
{ public DateTime OccurredOn => DateTime.UtcNow; }
```

---

## 1.7 一致性模型：域内事件 + 领域外事件（G2 · ADR-002）

五库（SQLite/InfluxDB/PostgreSQL/Seq/ClickHouse）双写若无一致性模型，任一处失败即脏读（评审 G2）。方案是**区分两类事件**：

| 类型                         | 边界       | 传输                             | 一致性           | 典型用途             |
| -------------------------- | -------- | ------------------------------ | ------------- | ---------------- |
| **域内事件 DomainEvent**       | 进程内、同一聚合 | Mediator `INotification`（源码生成） | 与业务**同事务**强一致 | 本地读模型/索引更新、状态机推进 |
| **领域外事件 IntegrationEvent** | 跨上下文/跨进程 | MQTT/Outbox/Wolverine（§11）     | **最终一致 + 幂等** | 遥测上送、跨服务通知、派生读模型 |

```csharp
// ① 域内事件分发（进程内，Mediator 源码生成，零分配）
public interface IDomainEventDispatcher
{
    ValueTask DispatchAsync<T>(T e, CancellationToken ct) where T : IDomainEvent;
}
// 领域外事件（跨边界，最终一致）
public interface IIntegrationEventPublisher
{
    ValueTask PublishAsync(IntegrationEvent e, CancellationToken ct); // 落 Outbox 表后异步 relay
}

// ② Outbox 表（PostgreSQL，与业务写同一事务）——强一致 + 幂等发件箱
public sealed class OutboxMessage
{
    public long Id { get; set; }
    public string Topic { get; set; } = "";          // devices/{...}/{id}/integration
    public string Payload { get; set; } = "";         // System.Text.Json / MessagePack
    public string Discriminator { get; set; } = "";   // 事件类型，consumer 路由
    public long SeqNo { get; set; }                   // 每设备单调递增，consumer 去重键
    public DateTime CreatedAt { get; set; }
    public bool Sent { get; set; }
}

// ③ 南向写指令（强一致 + 幂等）：业务库 + Outbox 原子提交
await using var tx = await pg.BeginTransactionAsync(ct);
var dev = assetRepo.Get(deviceId);
dev.ApplyCommand(cmd);
await assetRepo.SaveAsync(dev, tx);
await outbox.SaveAsync(new OutboxMessage {
    Topic = dev.Id.Topic.Command, Payload = Serialize(cmd), SeqNo = nextSeq
}, tx);
await tx.CommitAsync();   // 业务 + Outbox 同一事务，绝不出现「业务写了、Outbox 没写」

// ④ 后台 Outbox relay（Wolverine/自研）：读未发送 → 发 EMQX → 设备回执对账 → 标 Sent
// ⑤ 遥测消费（最终一致，可重建读模型）：InfluxDB/Seq/ClickHouse consumer 用 (deviceId, seqNo) 幂等
await foreach (var e in mqtt.ReceiveAsync(ct))   // 订阅 devices/+/+/+/+/+/+/+/telemetry
{
    var key = (e.DeviceId, e.SeqNo);
    if (dedup.Contains(key)) continue;            // 幂等：同 (deviceId, seqNo) 只处理一次
    await influx.WriteAsync(e);                   // 写时序库
    await seq.IngestAsync(e);                     // 写热日志
    dedup.Add(key);                               // Seq/ClickHouse = 可重建派生读模型，丢了可重放 Outbox 补
}
```

> **一致性基线（ADR-002）**：① 指令/控制（南向写）= **强一致 + 幂等**（PostgreSQL+Outbox，设备回执对账）；② 遥测/日志 = **最终一致**，经 Outbox 幂等投影到 InfluxDB/Seq/ClickHouse，视为可重建读模型。Wolverine 另提供 Inbox 自动幂等消费（§11）。

---

## 1.8 南向安全基线（G3 · ADR-005：OT 平台南向即红线）

北向有 WAF/RBAC，但**南向（平台⇄PLC）的设备身份、传输加密、指令鉴权、审计**此前缺失——这是 OT/ICS 合规红线（等保/IEC 62443）。「配置错即动真设备」，南向必须是安全重点（评审 G3）。

```csharp
// ① 北向 HTTPS/TLS：LettuceEncrypt 自动签发 ACME 证书（零手动 + 自动续期）
builder.Services.AddLettuceEncrypt();   // NuGet: LettuceEncrypt
builder.WebHost.UseKestrel(o => o.ConfigureHttpsDefaults(h => h.AllowAnyClientCertificate = false));
//   内网/离线场景用内部 CA：CSharp-easy-RSA-PEM（清单 #124）自签 X.509，下发到各节点信任列表

// ② 南向 MQTT over TLS（8883）+ 每设备 X.509 凭证
//   EMQX 5.x：listeners.ssl.default.bind=8883；authn 每设备独立证书(CN=devices/{id})；
//   ACL 按主题授权（禁止越权操作他人设备）：
//     acl_rule: { permit, all, devices/${device_id}/# }   // 仅允许操作自己设备
//     acl_rule: { deny,   all, devices/# }                // 默认拒绝跨设备
var mqtt = new MqttFactory().CreateMqttClient();
await mqtt.ConnectAsync(new MqttClientOptionsBuilder()
    .WithTcpServer("emqx", 8883)
    .WithTlsOptions(o => o.UseBuiltInCertificates = true)   // 校验服务端证书
    .WithCredentials(deviceId, deviceToken)                 // 每设备凭证（PSK 或 X.509）
    .Build(), ct);

// ③ OPC UA 证书信任：OPC.Ua 证书存入证书库并加入信任列表（见 §19.4）

// ④ 南向写指令鉴权 + 审计（任何动设备的操作必经此关）
[HttpPost("/devices/{id}/command")]
[Authorize]                                              // 北向已鉴权
public async Task<IActionResult> IssueCommand(string id, [FromBody] WriteCommand cmd,
    [FromServices] IAuthorizationService auth, [FromServices] IAuditStore audit)
{
    if (!await auth.AuthorizeAsync(User, new DeviceResource(id), "WriteDevice"))  // RBAC/Casbin
        return Forbid();                                 // 无写设备权限直接拒绝
    await _bus.Publish(new CommandIssued(DeviceId.Parse(id), cmd.Address, cmd.Value, DateTime.UtcNow));
    await audit.WriteAsync(new AuditEntry("SouthboundCommand", User.Identity!.Name, id, cmd)); // 全量审计入库
    return Accepted();
}
```

> **纵深防御**：南向安全 = 传输加密(TLS/mTLS) + 身份(每设备凭证) + 授权(RBAC `RequirePermission`) + 审计(全量入库) 四层；WAF 只挡北向 Web 攻击，绝不可替代南向鉴权/审计。

---

## 1.9 架构决策记录（ADR-001 ~ 005）

采用 ADR 模板（Status / Context / Decision / Consequences），团队需 ratify 签字：

| ADR                                             | 决策                                                         | 关键取舍                                                     |
| ----------------------------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| **ADR-001 单体起步**                            | MVP 用 Modular Monolith + DotNetCorePlugins；契约先行（`Abstractions` 零实现） | ✅ 低部署成本/易调试；❌ 单体故障域大（靠 ALC 隔离缓解）       |
| **ADR-002 一致性模型**                          | 南向写强一致+幂等（PG+Outbox）；遥测最终一致（Outbox 幂等投影，可重建读模型） | ✅ 不丢业务真相；❌ 遥测秒级延迟、需幂等 consumer              |
| **ADR-003 插件清理契约**                        | `IPlugin` 必须 `IAsyncDisposable`；`PluginManager.Unload()` 先反注册再卸载（见 §3.5） | ✅ 可安全热插拔；❌ 插件作者需遵守清理清单                     |
| **ADR-004 控制平面隔离**                        | 控制平面现为同进程 `BackgroundService`+`IEventBus`，**未来切微服务=换传输**（见 §1.5/G5） | ✅ 演进零重写；❌ 当前略增间接层                               |
| **ADR-005 南向安全基线**                        | 每设备凭证 + EMQX ACL + 写指令 `RequirePermission` + 全量审计（见 §1.8） | ✅ 合规可达；❌ 接入复杂度上升、需密钥管理                     |
| **ADR-006 后端可靠性基线**                      | liveness/readiness/startup 三探针健康检查 + 优雅停机（`IHostApplicationLifetime`）+ OpenTelemetry 三信号可观测（见 §1.10.5 / §23.3） | ✅ 故障可观测/可优雅恢复；❌ 需接入 OTel 后端（Aspire/Prometheus/Grafana） |
| **ADR-007 .NET 11 Preview.7 基座 + Native AOT** | 宿主以 .NET 11 Preview.7 为目标，关键热路径走 NativeAOT/trimming；插件 ALC 需标注 trimming 安全（见 §23.1 / §23.2） | ✅ 小镜像/快启动/低攻击面；❌ 第三方库 AOT 兼容性需逐个验证    |
| **ADR-008 横向扩展 = 无状态 + 外置会话**        | 后端无状态；认证态/会话外置 Redis/PostgreSQL；多实例经负载均衡水平扩容，数据平面独立扩缩（见 §1.10.7） | ✅ 可线性扩容；❌ 需解决有状态缓存一致性（HybridCache L2 缓解） |
| **ADR-009 长时间运行稳定性与高可用基线**        | 监督者+看门狗自愈（每设备独立 Task + 退避重启 + 熔断 flapping）+ 长期运行 GC（DATAS/低延迟）+ 连接租约防泄漏 + 过载保护降级 + HA（PG 咨询锁选主 + 边缘主备 + 降级服务）+ 崩溃恢复与状态重建（见 §1.11） | ✅ 7×24 不掉线/不崩；❌ 需运维看门狗与选主组件、GC 模式需压测标定 |
| **ADR-010 协议/设备互换抽象**                   | `DeviceProfile` 点表描述文件 + `IDeviceProtocol` 统一契约 + 能力协商 + `DeviceCatalog` 热替换；新增/更换 PLC = 新增/改 Profile，上层零改动（见 §1.12，扩展 §6.2 `IProtocolAdapter`） | ✅ 协议与设备可热替换、生态可插拔；❌ Profile 需随固件升级维护、能力协商要覆盖全量协议 |
| **ADR-011 故障全生命周期管理**                  | `FaultEvent`（`correlationId` 全链路）+ `IFaultStore` 多维查询 + `IRecoveryStrategy` 自动化恢复 + 解决状态机（Open→Ack→Resolved→Closed）+ MTTR 看板（见 §1.13，复用 §1.10.6 可观测 + §1.8 审计） | ✅ 故障可追/可恢/可解/可查；❌ 需故障码标准化与知识库运营投入  |

**量化 SLO（G12，先定指标再反推参数）**：1 万设备 · 50 万点/秒 · 采集端到端 p99 < 200ms · 可用性 99.9%。据此反推：单采集节点设备数、Channel 容量、InfluxDB 分片、Outbox relay 并发。

---

## 1.10 后端可靠性架构：可扩展 · 高性能 · 插件式 · 可靠（V8）

> 本节能独立成「后端架构规格书」。它把前 9 节散落的可靠性点（弹性 §10、背压 §9、清理契约 §3.5、一致性 §1.7、测试 §22）收敛为四条互相咬合的支柱，并补齐 V7 缺失的**可观测性、优雅停机、横向扩展、API 工程**四块。所有决策与 ADR-006/007/008 对齐。

### 1.10.1 四支柱总览

```
            可扩展 ──► 垂直切片 / 契约版本 / 微服务抽取缝(§0.3)
               │
 插件式 ◄──── 底座 ────► 高性能 ──► NativeAOT / 零分配 / lock-free
               │
               ▼
            可靠 ────► 三探针 / 优雅停机 / 弹性 / 幂等 / 混沌(§22)
               │
               ▼
          可观测(OTel 三信号) + 横向扩展(无状态+外置会话)
```

四支柱的落地抓手已在全文分布：可扩展→§0.3/§3；高性能→§4.3/§5/§6/§0.5；插件式→§3.4/§3.5；可靠→§9/§10/§1.7/§22。本节给出**后端视角的统一约束与代码基线**。

### 1.10.2 可扩展（Extensibility）

1. **垂直切片优先于层切片**：每个能力（Modbus 采集、规则告警、AI 质检）是一个自带 Controller/Handler/Repository/插件的「切片」，而非跨切片的 `Controllers/` `Services/` `Repositories/` 三层目录。新增协议 = 新增切片 + 注册插件，不改动既有切片。
2. **契约版本演进**（解决「先画域，再选技」后的演进问题）：`PlcPlatform.Abstractions` 中接口加 `[Obsolete]` + 双版本并存 + 适配器；插件 `PluginManifest` 声明 `MinHostVersion`，宿主拒绝加载不兼容插件。
   ```csharp
   public sealed class PluginManifest
   {
       public string PluginId { get; init; } = "";
       public string Version { get; init; } = "1.0.0";
       public Version MinHostVersion { get; init; } = new(8, 0); // 要求宿主 ≥ V8
   }
   // 宿主加载时校验
   if (manifest.MinHostVersion > hostVersion)
       throw new PluginIncompatibleException($"{manifest.PluginId} 要求宿主 ≥ {manifest.MinHostVersion}");
   ```
3. **微服务抽取缝**（ADR-001）：切片成熟后，把其 `IEventBus` 传输从进程内 Mediator 换成 Dapr 边车 / YARP 路由，切片即变为独立进程。**契约不变**，迁移零代码改动（§0.3）。

### 1.10.3 高性能（High Performance · .NET 11 Preview.7）

.NET 11 Preview.7（C# 14）热路径武器库（与 §0.5 内置约束呼应）：

| 能力                                | .NET 11 Preview.7 落地                                    | 用途                                |
| ----------------------------------- | --------------------------------------------------------- | ----------------------------------- |
| **Native AOT**                      | `<PublishAot>true` + trimming 安全注解                    | 宿主小镜像/快启动/低攻击面（§23.2） |
| **JSON 源生成**                     | `[JsonSerializable(typeof(T))]` + `IJsonTypeInfoResolver` | 零反射反序列化，AOT 安全            |
| **`System.Threading.Lock`**         | `private readonly Lock _gate = new();`                    | 替代 `object` 锁，更低开销、可组合  |
| **`Tensor<T>` / `SparseTensor<T>`** | `System.Numerics.Tensors`（.NET 11 Preview.7）            | 边缘 ML 推理 / 多维遥测聚合         |
| **Pipelines + Channels**            | `PipeReader` + `BoundedChannel`                           | 零拷贝流式解析（§5/§6）             |
| **lock-free**                       | `Interlocked` / `RingBuffer`(Disruptor)                   | 热路径无锁计数/采样                 |

```csharp
// .NET 11 Preview.7 NativeAOT + 零反射 JSON + Lock（热路径示例）
[JsonSerializable(typeof(DeviceFrame))]
internal partial class AppJsonContext : JsonSerializerContext { }

private readonly Lock _gate = new();          // .NET 9+ 轻量锁
private int _dropped;

public void OnFrame(ReadOnlySpan<byte> span)
{
    // 零分配解析：Span 直接切片，不 new byte[]
    var frame = FrameParser.Parse(span);
    lock (_gate) { _dropped += frame.IsNoise ? 1 : 0; }   // 仅在必须加锁处加锁
    _channel.Writer.TryWrite(frame);            // 单闸门背压(§9 G6)
}
```

### 1.10.4 插件式（Plugin-based）

- **隔离**：每个协议插件 = collectible `AssemblyLoadContext` + 独立子容器 + 独立 Channel/RingBuffer/状态机（§3.4/§3.5）。
- **清理契约**：`IPlugin : IAsyncDisposable`，`PluginManager.UnloadAsync` 有序拆除（反注册→停 Timer→排空 Channel→释放连接→Unload→强制 GC 观测）。
- **sidecar 兜底**：若某插件 ALC 卸载后仍存在句柄泄漏（经典事故），宿主将其进程级隔离到 sidecar，主进程不重启即可摘除。

### 1.10.5 可靠性（Reliability）

1. **三探针健康检查**（K8s/Docker 就绪判定标准）：
   - `liveness`：进程活着即可（防死锁重启）；
   - `readiness`：依赖（PG/EMQX/InfluxDB）就绪才接流量；
   - `startup`：冷启动重任务（插件扫描/建连）期间不计入 liveness 超时。
2. **优雅停机**：收到 `SIGTERM` 后，先停止接收新请求/新帧，排空 Channel 在途消息，等待在途写指令完成，再释放连接。
   ```csharp
   builder.Services.AddHealthChecks()
       .AddNpgSql(conn, name: "pg", tags: new[] { "ready" })
       .AddAsyncCheck("emqx", async () => await mqtt.PingAsync() ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());

   var app = builder.Build();
   var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
   lifetime.ApplicationStopping.Register(() =>
   {
       _channel.Writer.Complete();                 // 1. 停止接收新帧
       _source.Cancel();                            // 2. 取消在途任务
       _pluginManager.UnloadAllAsync().GetAwaiter().GetResult(); // 3. 有序卸载插件
   });
   app.MapHealthChecks("/health/live");
   app.MapHealthChecks("/health/ready", new() { Predicate = r => r.Tags.Contains("ready") });
   app.Run();
   ```
3. **弹性 + 背压 + 幂等 + 混沌**形成闭环：Polly 熔断/重试（§10）+ BoundedChannel 单闸门（§9 G6）+ Outbox 幂等消费 `(deviceId, seqNo)`（§1.7 G2）+ 网络分区混沌（§22）。

### 1.10.6 可观测性（Observability · G7）

- **三信号**：Logs（`ILogger` + 结构化）、Metrics（`System.Diagnostics.Metrics`，自定义 `采集点/秒`、`Channel 深度`、`丢弃帧计数`）、Traces（请求/消息全链路）。
- **跨边界传播**：需把 `Activity` 从宿主透传到插件 ALC 内、并经 MQTT 头传到设备侧消费端（G7 缺口补完）。
- **统一后端**：Aspire Dashboard（开发/测试）或 OTel Collector → Prometheus + Grafana + Loki（生产）。

### 1.10.7 横向扩展与多实例

- **无状态后端**：宿主进程不持有会话；认证态（JWT/Refresh）与用户会话外置到 Redis/PostgreSQL。
- **水平扩容**：多实例经负载均衡（YARP/LB）分摊北向 API 与南向指令编排；数据平面采集节点按设备分片独立扩缩。
- **数据层扩展**：PostgreSQL 读副本（查询侧）、InfluxDB 按时间/设备分片（写侧）、Outbox relay 多实例竞争消费（幂等保证不重）。
- **发布策略**：蓝绿 / 金丝雀；南向规则变更须先灰度再全量（§1.8 审批 + §13 回滚）。

---

## 1.11 长时间运行稳定性与高可用（V9 · ADR-009）

> AIOT 平台一旦上线即 7×24 运行：最忌「跑几天就内存涨、单设备掉线拖垮整线、进程悄悄假死、过载直接雪崩」。本节把稳定性与高可用收敛为可落地的基线，与 §1.10.5 可靠性、§10 弹性、§3.5 清理契约形成闭环。

### 1.11.1 设计原则

- **故障域隔离**（每设备独立 ALC/Channel/状态机，§3.5/§6.6）→ 单点故障不扩散。
- **自愈优先**（崩溃即重启，受预算约束）→ 不靠人工重启。
- **优雅降级**（过载时丢遥测保指令、保活不保全）→ 活着比完美重要。
- **可恢复**（状态外置/可重建）→ 重启后秒级接续，不丢业务真相（§1.7 Outbox）。

### 1.11.2 监督者 + 看门狗（Supervisor & Watchdog）

每个设备会话由一个 `DeviceSessionSupervisor` 托管一个长期 `Task`；会话内异常被捕获并触发退避重启；看门狗周期心跳，连续丢失 → 判定失联 → 故障转移（§1.11.6）。

```csharp
public sealed class DeviceSessionSupervisor
{
    private readonly Lock _gate = new();           // .NET 9+ 轻量锁
    private Task? _loop;
    private int _restarts;
    private DateTime _lastHeartbeat = DateTime.UtcNow;
    private readonly DeviceOptions _opts;
    private readonly IDeviceProtocol _protocol;     // §1.12 统一契约
    private readonly ILogger _log;

    public void Start(CancellationToken outer)
    {
        _loop = Task.Run(async () =>           // 每设备独立 Task，崩溃不拖垮宿主
        {
            while (!outer.IsCancellationRequested)
            {
                try
                {
                    await RunSessionAsync(outer);              // 连接→轮询→处理
                    return;                                    // 正常退出（被停机取消）
                }
                catch (Exception ex) when (!outer.IsCancellationRequested)
                {
                    if (++_restarts > _opts.MaxRestarts)       // 重启预算（防 flapping）
                    {
                        _log.Critical(ex, "设备 {Dev} 重启超预算，升级安全态", _opts.DeviceId);
                        await EnterSafeStateAsync();           // 急停相关走安全态(§7.0 G10)
                        break;
                    }
                    var backoff = Backoff(_restarts);          // 指数退避 + 抖动
                    _log.Warning(ex, "设备 {Dev} 会话崩溃，{Ms}ms 后第 {N} 次重启",
                                 _opts.DeviceId, backoff.TotalMilliseconds, _restarts);
                    await Task.Delay(backoff, outer);
                }
            }
        }, outer);
    }

    // 看门狗：数据平面周期汇报心跳；管理平面监测丢失
    public void Heartbeat() => _lastHeartbeat = DateTime.UtcNow;
    public bool IsAlive(TimeSpan timeout) => DateTime.UtcNow - _lastHeartbeat < timeout;

    private static TimeSpan Backoff(int n) =>
        TimeSpan.FromSeconds(Math.Min(300, Math.Pow(2, n)))    // 1,2,4,8… 封顶 5min
        .Add(TimeSpan.FromMilliseconds(Random.Shared.Next(0, 500))); // 抖动防惊群
}
```

### 1.11.3 崩溃自愈与重启策略

- **重启预算** `MaxRestarts`：连续 N 次失败即停止重启，避免「崩溃→重启→崩溃」雪崩（flapping）；转安全态并告警。
- **熔断保护**：对同一设备 60s 内失败超阈值 → 打开断路器（Polly，§10），半开探测恢复。
- **安全态升级**：与急停/安全相关的故障（§7.0 G10）不盲目重试，直接 `EnterSafeStateAsync()` 断输出、上报告警。
- **全局兜底**：`AppDomain.UnhandledException` / `TaskScheduler.UnobservedTaskException` → 记录 + 视严重度决定是否进程级重启（由容器编排负责重启）。

### 1.11.4 长期运行资源稳定性（防「跑几天就崩」）

7×24 头号杀手是内存/句柄/连接缓慢泄漏与 GC 停顿。

- **GC 长稳基线（.NET 11 Preview.7）**：Server GC + **DATAS**（Dynamic Adaptation To Application Sizes，进程随负载自适应堆大小）；关键窗口用 `SustainedLowLatency`；极端低延迟段用 `GC.TryStartNoGCRegion`（采集批次内）。
  ```csharp
  // runtimeconfig.json 或环境变量
  // {
  //   "configProperties": {
  //     "System.GC.DynamicAdaptationMode": "1",   // DATAS 自适应
  //     "System.GC.Server": true,
  //     "System.GC.Concurrent": true
  //   }
  // }
  GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency; // 长稳低停顿
  if (GC.TryStartNoGCRegion(64 * 1024))                        // 关键批次零 GC
  {
      try { FlushBatch(); } finally { GC.EndNoGCRegion(); }
  }
  ```
- **连接租约防泄漏**：每设备连接设 idle timeout + max lifetime；超时强制释放并重连，杜绝「半死连接」累积（§1.12 `IDeviceProtocol.DisconnectAsync`）。
- **对象池纪律**：`ArrayPool<byte>.Shared` / `RecyclableMemoryStream` / `MemoryPool<byte>` 全程复用；解析用 `Span`；ALC 卸载强制 GC 观测（§3.5）。
- **无未处理异常逃逸**：热路径 `try/catch` 后计数不抛；全局处理器兜底。

### 1.11.5 过载保护（Load Shedding & Degraded Serving）

长稳的另一敌人是「瞬时风暴」：突发遥测把 Channel 打满 → 连锁超时 → 雪崩。

- **优先级队列**：单设备 `Channel` 拆 critical（写指令/心跳/急停）与 telemetry 两路；过载时**先丢遥测保指令**（保命不保全）。
  ```csharp
  // 优先级背压：指令/心跳走 criticalWriter（有界但容量高于遥测）
  if (!_criticalWriter.TryWrite(frame)) _metrics.DroppedCritical++;
  else if (!_telemetryWriter.TryWrite(frame)) _metrics.DroppedTelemetry++; // 过载先丢遥测
  ```
- **降级服务**：依赖（PG/EMQX/InfluxDB）部分不可用时，`readiness` 返回 `Degraded` 而非 `Unhealthy`，继续服务核心写/读，停非关键聚合。

### 1.11.6 高可用部署（HA）

- **多副本无状态核心**（§1.10.7）：北向 API/指令编排多实例，会话外置，LB 分摊。
- **单例职责选主**：Outbox relay、定时规则编译等「只能一个实例干」的职责，用 **PostgreSQL 咨询锁**选主，主挂从抢锁接管（无外部协调组件）：
  ```csharp
  await using var tx = await _pg.BeginTransactionAsync();
  var held = await _pg.ExecuteScalarAsync<int>(
      "SELECT pg_try_advisory_xact_lock(@nsp, @id)", new { nsp = 1, id = 42 });
  if (held == 1) { /* 我是主：跑 Outbox relay */ }
  ```
  或用 **Dapr Actor 提醒**做带租约的单例。
- **边缘冗余**：关键产线双采集代理（active-standby），主失联由看门狗触发备切换。
- **发布策略**：蓝绿/金丝雀（§1.8 审批 + §13 回滚）；南向规则变更先灰度。

### 1.11.7 崩溃恢复与状态重建（Crash Recovery）

- 设备会话状态外置（DeviceRegistry + Outbox）：重启后从持久化 `DeviceSessionState` 续接，不重新建链风暴。
- **读模型可重建**：遥测最终一致，InfluxDB/Seq/ClickHouse 读模型可由 Outbox 重放重建（§1.7 G2）。
- **断点续传**：采集位点/批次序号持久化，重启从最后确认位点续采，避免重复全量轮询。

---

## 1.12 协议与 PLC 设备可扩展与互换（V9 · ADR-010）

> 换 PLC 品牌、换协议（Modbus↔S7↔OPC-UA↔三菱）、换传输（TCP↔RTU），**上层业务与 UI 一行不改**——这是平台通用性的命门。本节在 §6.2 `IProtocolAdapter` 之上建立「设备互换」抽象。

### 1.12.1 统一设备模型：DeviceProfile（点表描述文件）

把「一个具体 PLC 有哪些寄存器、什么类型、什么字节序、怎么轮询」外置为 **`DeviceProfile`**（JSON/YAML），与代码解耦。新增/更换 PLC = 新增/改一个 Profile 文件，零代码改动。

```csharp
// DeviceProfile.cs（位于 PlcPlatform.Abstractions）
public sealed record DeviceProfile
{
    public string DeviceId { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string Protocol { get; init; } = "Modbus";      // Modbus/S7/OpcUa/Melsec/...
    public string Transport { get; init; } = "Tcp";        // Tcp/Rtu/Udp
    public DeviceEndpoint Endpoint { get; init; } = new();
    public Endianness Endianness { get; init; } = Endianness.Big;
    public int PollingMs { get; init; } = 1000;
    public IReadOnlyList<RegisterPoint> Points { get; init; } = [];  // 点表
    public string? ProfileSchemaVersion { get; init; } = "1.0";
}

public sealed record RegisterPoint
{
    public string Tag { get; init; } = "";                 // 业务点名（上层只读这个）
    public ushort Address { get; init; }                   // 寄存器地址
    public byte FunctionCode { get; init; }                // 03/04/06/16...
    public string DataType { get; init; } = "float";       // bool/int16/uint16/float/...
    public ByteOrder ByteOrder { get; init; } = ByteOrder.ABCD;
    public double Scale { get; init; } = 1.0;
    public double Offset { get; init; } = 0.0;
    public string? Unit { get; init; }
}
```

```json
// profiles/plc-a1.modbus.json —— 换型仅改此文件
{
  "DeviceId": "PLC-A1", "Protocol": "Modbus", "Transport": "Tcp",
  "Endpoint": { "Host": "192.168.1.10", "Port": 502, "SlaveId": 1 },
  "Endianness": "Big", "PollingMs": 1000,
  "Points": [
    { "Tag": "Temp1", "Address": 30001, "FunctionCode": 4, "DataType": "float", "Unit": "°C" },
    { "Tag": "ValveOpen", "Address": 1, "FunctionCode": 1, "DataType": "bool" }
  ]
}
```

### 1.12.2 设备协议统一契约：IDeviceProtocol

在 §6.2 `IProtocolAdapter`（流式帧出口）基础上，定义**面向设备生命周期**的统一契约，所有协议实现（含插件）都实现它：

```csharp
// 协议与设备互换的核心契约
public interface IDeviceProtocol : IAsyncDisposable
{
    string ProtocolId { get; }                                  // "Modbus"/"S7"/"OpcUa"/...
    DeviceCapabilities Capabilities { get; }                    // 能力协商(§1.12.3)
    ValueTask ConfigureAsync(DeviceProfile profile, CancellationToken ct);
    ValueTask ConnectAsync(CancellationToken ct);               // 建连（带租约/超时）
    ValueTask DisconnectAsync(CancellationToken ct);            // 释放连接（防泄漏 §1.11.4）
    IAsyncEnumerable<PointSample> PollAsync(CancellationToken ct); // 统一点表采样出口
    ValueTask WriteAsync(WriteCommand cmd, CancellationToken ct);  // 南向写（强一致 §1.7）
    ValueTask<HealthProbeResult> HealthProbeAsync(CancellationToken ct); // 心跳/探针
}
```

`PollAsync` 吐出的是平台统一的 `PointSample { Tag, Value, Quality, Timestamp }`，**与具体协议无关**——上层 Telemetry/Command 上下文只认 `Tag`（§1.6）。

### 1.12.3 能力协商（Capability Negotiation）

不同协议能力不同（S7 支持订阅、Modbus RTU 不支持、某些 PLC 禁止写）。连接后上报 `DeviceCapabilities`，上层**自适应行为**：

```csharp
public sealed record DeviceCapabilities
{
    public bool SupportsWrite { get; init; }
    public bool SupportsSubscribe { get; init; }        // true→用订阅，false→轮询
    public int MaxRegistersPerFrame { get; init; }
    public IReadOnlySet<string> NativeDataTypes { get; init; } = [];
    public int MaxConcurrentSessions { get; init; } = 1;
}
// 上层用法：if (!caps.SupportsSubscribe) 退化为轮询；if (!caps.SupportsWrite) 隐藏写按钮
```

### 1.12.4 设备注册表与热替换（DeviceCatalog）

设备类型与实例经 `DeviceCatalog` 注册；**换型 = 更新 Profile 指针**（如 PLC-A1 从 Modbus 换 S7），上层零改动；协议实现热替换（Modbus/TCP ↔ Modbus/RTU）不改上层。

```csharp
public interface IDeviceCatalog
{
    void RegisterProfile(DeviceProfile profile);             // 新增/更换 PLC = 注册 Profile
    void SwitchProtocol(string deviceId, string transport);  // 换传输(TCP↔RTU)不改点表
    IDeviceProtocol Resolve(string deviceId);                // 按 Profile 解析出协议实现
    IReadOnlyCollection<DeviceProfile> All { get; }
}
// 换型流程：catalog.RegisterProfile(s7Profile) → supervisor 重启该会话 → 上层照常按 Tag 读写
```

### 1.12.5 通用点表导入/导出（Interchange Format）

提供标准化点表格式（CSV / JSON），并借鉴 **OPC-UA Nodeset** 思路做跨平台迁移，保证换型时历史点表/告警阈值可平移：

```csharp
public interface IPointTableExchange
{
    Task ExportAsync(string deviceId, Stream outCsv, CancellationToken ct);   // 点表+量程+告警
    Task<DeviceProfile> ImportAsync(Stream inCsv, CancellationToken ct);       // 生成 Profile
}
// 换 PLC 品牌：旧品牌导出 CSV → 映射 Tag → 新品牌导入生成新 Profile
```

### 1.12.6 互换性保障

- 同契约多实现可插拔：Modbus/S7/OPC-UA/Melsec 都是 `IDeviceProtocol` 的不同实现，经 §3 插件机制加载。
- **契约测试**（§22）：对每个协议实现跑同一套 `IDeviceProtocol` 契约用例（连接/读/写/断线重连/能力协商），保证互换后行为一致。
- 迁移指南复用 §6（零分配解析）/ §8（Modbus 实战），新协议只需补 `ConfigureAsync/PollAsync/WriteAsync` 三处。

---

## 1.13 故障可追溯·恢复·解决·查询（V9 · ADR-011）

> 「故障可追溯、可恢复、可解决、可查询」是运维闭环。本节建立 **FaultEvent 模型 + 全链路关联 + 自动化恢复 + 解决流程 + 多维查询** 的完整子系统，复用 §1.10.6 可观测与 §1.8 审计。

### 1.13.1 故障事件模型（FaultEvent）

所有异常（断线、校验失败、写失败、急停、超阈值）统一为 `FaultEvent`，带**指纹**去重、**关联 ID** 串联全链路：

```csharp
public sealed record FaultEvent
{
    public Guid FaultId { get; init; } = Guid.NewGuid();
    public string DeviceId { get; init; } = "";
    public string Code { get; init; } = "";                 // 标准化故障码(如 COMM_TIMEOUT)
    public FaultSeverity Severity { get; init; }             // Info/Warning/Error/Critical
    public FaultCategory Category { get; init; }             // Comm/Protocol/Safety/Rule/...
    public string Source { get; init; } = "";                // 协议插件/规则引擎/...
    public string CorrelationId { get; init; } = "";         // 全链路关联(采集→处理→指令→存储)
    public string Fingerprint { get; init; }                 // 设备+码+短时窗 去重聚合
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string? Payload { get; init; }                    // 故障时刻设备状态快照
    public FaultStatus Status { get; init; } = FaultStatus.Open;
}
```

### 1.13.2 可追溯（Traceability）

- **全链路 `correlationId`**：采集帧 → 处理 → 下发指令 → 存储，全程透传同一 `Activity.TraceId` + 业务 `correlationId`（§1.10.6 跨 ALC/MQTT 传播）。故障时可一键拉出「这条数据从哪来到哪去」。
- **故障时刻快照**：`Payload` 存故障瞬间设备状态机态（§7.0）、最后 N 帧、连接信息，便于复盘。
- **审计闭环**（§1.8）：谁、何时、下了什么指令、结果如何，与故障时间线对齐。
- **三信号关联**：OTel Trace + Log + Metric 用同一 `correlationId` 关联，Grafana 跨信号跳转。

### 1.13.3 自动化恢复（Automated Recovery）

故障码 → 恢复策略映射；策略带退避与升级：

```csharp
public interface IRecoveryStrategy
{
    bool CanHandle(FaultEvent fault);
    ValueTask<RecoveryResult> ExecuteAsync(FaultEvent fault, CancellationToken ct);
}
// 注册示例
services.AddRecovery<ReconnectStrategy>(code: "COMM_TIMEOUT");   // 断线→重连
services.AddRecovery<ReinitSessionStrategy>(code: "SESSION_STALE"); // 会话过期→重初始化
services.AddRecovery<FailoverStrategy>(code: "EDGE_DOWN");        // 边缘掉→故障转移
services.AddRecovery<SafeStateStrategy>(code: "ESTOP_*");        // 急停相关→安全态(§7.0 G10)

public sealed class ReconnectStrategy : IRecoveryStrategy
{
    public bool CanHandle(FaultEvent f) => f.Code == "COMM_TIMEOUT";
    public async ValueTask<RecoveryResult> ExecuteAsync(FaultEvent f, CancellationToken ct)
    {
        for (var i = 1; i <= 5; i++)                  // 最大尝试 + 指数退避
        {
            try { await _catalog.Resolve(f.DeviceId).ConnectAsync(ct); return RecoveryResult.Succeeded; }
            catch when (i < 5) { await Task.Delay(Backoff(i), ct); }
        }
        return RecoveryResult.EscalateToHuman;         // 升级人工
    }
}
```

### 1.13.4 解决流程（Resolution Workflow）

故障状态机驱动「从发生到关闭」：

```csharp
// FaultStatus：Open → Acknowledged → InProgress → Resolved → Closed（或 AutoResolved）
// 状态流转由 Mediator 领域事件驱动（§1.7）
public record FaultAcknowledged(Guid FaultId, string By);
public record FaultResolved(Guid FaultId, string By, string RootCause, string? Note);
// 升级：自动恢复失败 → EscalateToHuman → 通知(§22/IM) → 人工 Ack/Resolve
```

- 责任人/处置记录（ack by、resolved by、root cause、note）随状态机落库。
- 与告警/通知集成（邮件/IM/工单）。

### 1.13.5 故障查询（Query）

`IFaultStore` 提供多维查询，存储**冷热分层**（热：PostgreSQL 关系存未结/近期；冷：InfluxDB 时序存全量指标级故障流）：

```csharp
public interface IFaultStore
{
    Task AddAsync(FaultEvent fault, CancellationToken ct);
    Task<IReadOnlyList<FaultEvent>> QueryAsync(FaultQuery q, CancellationToken ct);
}
public sealed record FaultQuery
{
    public string? DeviceId { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
    public FaultSeverity? MinSeverity { get; init; }
    public string? Code { get; init; }
    public FaultStatus? Status { get; init; }
    public string? Fingerprint { get; init; }        // 查同类故障聚合
}
// 关联查询：故障 → 相关遥测(同 correlationId) → 相关指令(同 TraceId) → 根因时间线
```

- **故障知识库/Runbook**：同类 `Fingerprint` 聚合并关联历史处置 Runbook，加速解决。
- 换型后（§1.12）故障码与 Tag 绑定，查询可按设备品牌/型号分组统计。

### 1.13.6 故障运营看板（Ops Dashboard）

- **MTBF / MTTR / 故障分布**：按设备/协议/严重度统计，复用 §16 ScottPlot 实时绘制。
- 未结故障红点、近期趋势、Top 故障码——运维一眼掌握平台健康。

> 上述 §1.11–§1.13 为 V9 核心三支柱，与 §6（协议抽象）、§1.10（可靠性）、§10（弹性）、§22（测试）形成「稳定—互换—可查」闭环；并以 ADR-009/010/011 固化。

---

## 2. 能力部分设计

### 2.1 核心服务

- **服务注册与发现**：进程内 `CapabilityRegistry` 为主，跨进程/跨节点走 Consul + `Microsoft.Extensions.ServiceDiscovery`。
- **统一能力与生命周期管理接口**：所有能力声明为 `ICapability`，由 PluginManager 编排。

### 2.2 能力组件（按领域）

- **协议/通信**：Modbus/OPC UA/BACnet/Fatek/Fuji/Melsec/Keyence/Omron（仓库已有 `*.cs` 驱动，直接包成协议插件）；上位机通讯 LOIC/llcom；网络 SpanNetty/DotNetty/SuperSocket/TouchSocket。
- **消息**：MQTTnet + MQTTnet.EventBus、ZeroMQ、MassTransit + RabbitMQ（CQRS/Saga）、csharp-nats、CAP。
- **数据**：SQLite、LiteDB 日志库、EF Core + Dapper 读写分离 + DapperAOT、Ardalis.Specification 规约、ZoneTree。
- **缓存**：Hybrid Cache、Garnet（Redis 协议）、FusionCache 混合缓存、EasyCaching、StackExchange.Redis。
- **序列化**：进程内 MemoryPack，跨进程 MessagePack / System.Text.Json / SpanJson。
- **AI/AIGC**：MEAI、Semantic Kernel、kernel-memory、OllamaSharp、AntSK 知识库、LongChain。
- **媒体/SIP/流媒体**：SIPSorcery、GB28181、JT808Gateway、LibVLCSharp/Xabe.FFmpeg、ZLMRTC/SRSManager。
- **视觉/OCR**：OpenCvSharp/EmguCV/ImageSharp、PaddleSharp OCR、YOLOv16。
- **调度/任务**：Hangfire+Redis+NCrontab、Coravel、FluentScheduler。
- **安全/认证**：OpenIddict/Keycloak、Casbin.Net 授权、BouncyCastle 加解密、AspNetCoreDataProtection。
- **可观测**：OpenTelemetry、AppMetrics、MiniProfiler、Seq/Serilog、WatchDog。
- **客户端/桌面**：Avalonia/Photino/Blazor Hybrid、MAUI（Silky）。
- **文档/报表**：QuestPDF/FastReport、MiniExcel/ClosedXML。
- **实时图表**：ScottPlot（优先，高性能、零依赖、适合高频时序）、LiveCharts2（MVVM 友好、Blazor/WPF 组件）。

### 2.3 插件化

每个能力 = 一个插件工程，输出独立目录、独立 ALC。遵循统一接口规范，由 PluginManager 管理生命周期，支持热插拔与版本共存。

### 2.4 扩展服务（基础件）

日志（Serilog/Exceptionless）、配置（AgileConfig/Consul）、缓存（FusionCache）、数据库访问（EF Core/Dapper）、HTTP 弹性客户端（`Microsoft.Extensions.Http.Resilience` + Polly）。

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
    public string? EntryType { get; set; }
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
3. **注册（入口）**：调用 `ConfigureServicesAsync`，插件只能向**自己的子容器**注册，且只能通过 `IPluginContext.SharedServices` 获取宿主能力。
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

> 关键坑：**类型标识**。跨 ALC 的类型无法互转，必须把契约程序集设为共享类型（放在宿主共享目录并用 `PreferSharedTypes`）。不可信插件建议升级为 sidecar 进程（进程级隔离）。

### 3.4 插件程序集加载最佳实践（约束 0）

底座热插拔引擎 = **DotNetCorePlugins**（`McMaster.NETCore.Plugins`，`natemcmaster/DotNetCorePlugins`）+ 自研 `PluginManager`。官方示例与原理务必参照：

- **AppWithPlugin / AssemblyLoadContext / AssemblyDependencyResolver** 权威示例与解析：<https://zread.ai/dotnet/samples/15-plugin-system-and-assembly-loading>
- 自定义加载上下文（含 `AssemblyDependencyResolver` 解析依赖、`LoadUnmanagedDll` 加载 Snap7/串口等原生库、强名称签名校验）可参照本仓库同级代码：`E:\WorkSpace\windgodsnowdrink\vsa\PluginLoadContext.cs`。

```csharp
// DotNetCorePlugins 标准写法：每个插件 = 一个 collectible ALC，依赖自动隔离
var alc = new PluginLoadContext(pluginDir);              // 复用自 vsa/PluginLoadContext.cs
var asm = alc.LoadFromAssemblyPath(Path.Combine(pluginDir, "MyProtocol.dll"));
var plugin = (IPlugin)Activator.CreateInstance(
    asm.GetTypes().First(t => typeof(IPlugin).IsAssignableFrom(t)))!;
await plugin.ConfigureServicesAsync(ctx, ct);
await plugin.StartAsync(ct);
// 卸载：alc.Unload() + 强制 GC（见 3.3 PluginSlot.Dispose）
```

要点清单：

| 关注点  | 做法                                                                     |
| ---- | ---------------------------------------------------------------------- |
| 依赖解析 | `AssemblyDependencyResolver` 自动按 `*.deps.json` 解析托管/非托管依赖              |
| 类型共享 | 契约程序集放宿主共享目录，ALC 用 `PreferSharedTypes` 复用，避免跨上下文类型转换失败                 |
| 非托管库 | 重写 `LoadUnmanagedDll` 解析 Snap7/`libserial` 等原生 DLL（Windows/Linux 分别拷贝） |
| 安全   | 强名称/Authenticode 签名校验；生产关闭不可信插件的自动加载                                   |
| 卸载   | `Unload()` 后强制 `GC.Collect()` + `WaitForPendingFinalizers()`（2–3 轮）防泄漏 |
| 版本共存 | 同一契约多版本插件可并存（各自独立 ALC），实现灰度/回滚                                         |

---

## 3.5 插件生命周期清理契约（G4 · ADR-003：热插拔的敌人是卸载）

§3 讲了 ALC 加载，但**卸载时的清理契约**才是热插拔生死线（评审 G4）。若插件在 `IEventBus` 订阅了事件、起了 `Timer`/`BackgroundService`/`Channel` reader，卸载 ALC 时这些引用仍被持有 → **插件程序集无法卸载（内存泄漏）+ 旧 handler 继续触发（幽灵处理）**。这是 ALC 热插拔最经典的生产事故。

**契约升级**：`IPlugin` 必须实现 `IAsyncDisposable`；`PluginManager.Unload()` 严格按序清理。

```csharp
// 契约：插件必须清理自身订阅 / Timer / Channel / 连接
public interface IPlugin : IAsyncDisposable
{
    Task ConfigureServicesAsync(PluginContext ctx, CancellationToken ct);
    Task StartAsync(CancellationToken ct);
    ValueTask DisposeAsync();   // 反注册事件订阅 → 停 Timer → 排空 Channel → 释放连接
}

// 插件实现（清理是强制义务）
public sealed class ModbusPlugin : IPlugin
{
    private readonly CancellationTokenSource _cts = new();
    private readonly IDisposable _sub;     // IEventBus 订阅句柄
    private PeriodicTimer? _timer;
    private readonly Channel<DeviceFrame> _channel = Channel.CreateUnbounded<DeviceFrame>();

    public ModbusPlugin(IEventBus bus) => _sub = bus.Subscribe<DeviceConfigChanged>(OnCfg);

    public ValueTask DisposeAsync()
    {
        _sub.Dispose();                    // 1) 反注册事件订阅（防幽灵 handler）
        _cts.Cancel();                     // 2) 取消后台循环 / 停 Timer
        _timer?.Dispose();
        _channel.Writer.TryComplete();     // 3) 排空 Channel（reader 自然退出）
        _adapter.Dispose();                // 4) 释放连接（TcpClient / S7Client / SerialPort）
        return default;
    }
}

// PluginManager.Unload：先清理再卸载，强制 GC 观测
public async Task UnloadAsync(string pluginId)
{
    var slot = _slots[pluginId];
    await slot.Plugin.DisposeAsync();          // 触发插件自身清理
    slot.Channel.Writer.TryComplete();         // 排空插件 Channel
    slot.Aggregator.Dispose();                 // 反注册宿主侧订阅
    slot.Alc.Unload();                         // 卸载 ALC
    for (int i = 0; i < 3; i++)                // 5) 强制 GC 两次观测
    {
        GC.Collect(2, GCCollectionMode.Forced);
        GC.WaitForPendingFinalizers();
        await Task.Delay(50);
    }
    AssertIsUnloaded(slot.WeakAssembly);        // 弱引用观测程序集是否真正卸载（防泄漏断言）
}
```

**插件必须清理清单（Plugin Cleanup Checklist）**：

1. 事件总线订阅反注册（`_sub.Dispose()`）
2. `Timer` / `BackgroundService` / `PeriodicTimer` 停止 + `CancellationTokenSource.Cancel()`
3. `Channel.Writer` 完成（`TryComplete()`），reader 排空退出
4. 设备连接释放（TCP/Serial/S7Client/OPC Session，`Dispose`）
5. 非托管资源（原生 DLL 句柄、文件、Socket）释放
6. 宿主侧聚合器/调度器反注册该插件

> **兜底**：不可信插件（第三方/动态下载）升级为 **sidecar 进程**（独立进程级隔离），宿主通过 gRPC/命名管道通信，卸载即杀进程，彻底杜绝 ALC 泄漏。

---

## 4. 插件标准出口

出口 = 插件**不直接调用**其它插件/宿主内部，而是统一通过底座抽象「流出」结果，保证可替换与零耦合。

### 4.1 三类标准出口

```csharp
// 1) 能力注册出口
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
- 遥测出口统一到 OpenTelemetry（计数/追踪/指标），不允许插件私自接第三方 APM。
- 日志出口统一 Serilog，插件只 `ILogger<T>`，不直连 Seq/文件。

### 4.3 零分配数据出口示例（Pipe + Span）

```csharp
async ValueTask ProcessAsync(PipeReader reader, CancellationToken ct)
{
    while (!ct.IsCancellationRequested)
    {
        ReadResult result = await reader.ReadAsync(ct);
        ReadOnlySequence<byte> buffer = result.Buffer;
        while (TryParseFrame(ref buffer, out var frame))   // frame 是 Span<byte>
            Handle(frame);                                  // 零拷贝处理
        reader.AdvanceTo(buffer.Start, buffer.End);
        if (result.IsCompleted) break;
    }
}
```

---

## 5. 工业级生产流水线（Channels → IAsyncEnumerable → BackgroundService → IHostedService → Dataflow）

全链路遵循「**生产者 → 流式搬运 → 生命周期编排 → 复杂流水线**」的工业级范式，把高频协议数据从高并发采集一路零分配地送到持久化/消息出口。

```
┌──────────────────────────────────────────────────────────────────────────┐
│  工业级生产流水线（单一方向，背压贯穿全程）                                  │
│                                                                            │
│  ① Channels (生产者/消费者)                                               │
│     协议插件写入 BoundedChannel<DeviceFrame>，BoundedChannelFullMode.Wait  │
│     做内存级背压；配合 Redis 深度闸门(mq:depth) 做分布式背压                  │
│            │  ReadAllAsync(ct)                                            │
│            ▼                                                              │
│  ② IAsyncEnumerable<T> + EnumeratorCancellation                          │
│     所有出口以 IAsyncEnumerable 流式吐出，消费端用 await foreach +          │
│     WithCancellation(ct) 可协作取消，永不阻塞线程                          │
│            │                                                              │
│            ▼                                                              │
│  ③ BackgroundService（长生命周期采集/消费）                               │
│     ExecuteAsync(ct) 内 await foreach 消费 Channel / IAsyncEnumerable，   │
│     自身即一个后台工作者                                                    │
│            │                                                              │
│            ▼                                                              │
│  ④ IHostedService 编排（宿主统一托管）                                    │
│     PluginManager / 各协议插件 / 消费管道都以 IHostedService 注册，       │
│     宿主负责 StartAsync/StopAsync 顺序与优雅关闭                          │
│            │                                                              │
│            ▼                                                              │
│  ⑤ Dataflow (TPL) 复杂流水线                                             │
│     TransformBlock(解析) → BatchBlock(批量) → ActionBlock(落库/发布)，    │
│     BoundedCapacity + 链接 PropagateCompletion 形成有界背压流水线          │
└──────────────────────────────────────────────────────────────────────────┘
```

### 5.1 生产级流水线代码骨架

```csharp
// ① 有界 Channel（内存背压）
var channel = Channel.CreateBounded<DeviceFrame>(new BoundedChannelOptions(2000)
{
    FullMode = BoundedChannelFullMode.Wait,   // 满则生产者阻塞，天然背压
    SingleReader = false, SingleWriter = false
});

// ② + ③ + ④：BackgroundService 既是宿主 IHostedService，又消费 IAsyncEnumerable
public sealed class TelemetryIngestService : BackgroundService
{
    private readonly ChannelReader<DeviceFrame> _reader;
    private readonly IRegistry _registry;
    public TelemetryIngestService(Channel<DeviceFrame> ch, IRegistry registry)
        => (_reader, _registry) = (ch.Reader, registry);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // ⑤ Dataflow 复杂流水线：解析 → 批量 → 落库
        var parse = new TransformBlock<DeviceFrame, NormalizedFrame>(f => Normalize(f),
            new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4, BoundedCapacity = 512 });
        var batch = new BatchBlock<NormalizedFrame>(200);
        var sink  = new ActionBlock<NormalizedFrame[]>(async b =>
            await _registry.PersistBatchAsync(b, ct),  // 批量写 InfluxDB/PostgreSQL
            new ExecutionDataflowBlockOptions { BoundedCapacity = 64 });

        parse.LinkTo(batch, new DataflowLinkOptions { PropagateCompletion = true });
        batch.LinkTo(sink,  new DataflowLinkOptions { PropagateCompletion = true });

        // ② IAsyncEnumerable + EnumeratorCancellation 消费 Channel
        await foreach (var frame in _reader.ReadAllAsync(ct).WithCancellation(ct))
            await parse.SendAsync(frame, ct);   // 背压在 BoundedCapacity 处传导

        parse.Complete();
        await sink.Completion;
    }
}

// 注册为 IHostedService（④ 编排）
builder.Services.AddHostedService<TelemetryIngestService>();
```

> 原则：**背压只在一处设闸门**（BoundedChannel 或 Dataflow 的 BoundedCapacity），避免多层缓冲堆积导致内存膨胀与尾延迟飙升。高频场景优先 Dataflow 的 `BatchBlock` 减少落库/发布次数。

---

## 6. 协议栈抽象与零分配解析

### 6.1 统一帧格式

```
| 帧头(Header) | 长度(Length) | 命令字(Command) | 载荷(Payload) | 校验(Checksum) | 帧尾(Footer) |
```

- `Header/Footer`：协议特定魔数（如 `0xAA 0x55`）。
- `Length`：Payload 字节数（注意**大小端**，按设备规范）。
- `Command`：功能码 / 命令字。
- `Checksum`：CRC16/CRC32/异或和（协议特定；校验失败直接丢弃并计数）。

### 6.2 抽象接口（位于 `PlcPlatform.Abstractions`）

```csharp
// 协议适配器：所有 PLC 协议对外统一契约
public interface IProtocolAdapter : IDisposable
{
    string ProtocolId { get; }
    ProtocolCapabilities Capabilities { get; }
    ValueTask StartAsync(DeviceOptions options, CancellationToken ct);
    ValueTask StopAsync(CancellationToken ct);
    IAsyncEnumerable<DeviceFrame> ReadAsync(CancellationToken ct);   // 统一出口：流式点表
    ValueTask WriteAsync(WriteCommand cmd, CancellationToken ct);
}

// 驱动抽象：与物理链路（串口/网口）相关的收发
public interface IDriver
{
    Endianness Endianness { get; }
    ValueTask<int> SendAsync(ReadOnlyMemory<byte> frame, CancellationToken ct);
    IAsyncEnumerable<ReadOnlyMemory<byte>> ReceiveAsync(CancellationToken ct);
}

// 设备抽象：一个具体 PLC 设备实例（连接/心跳/状态机）
public interface IDevice
{
    string DeviceId { get; }
    DeviceStateMachine State { get; }      // Stateless 状态机
    IProtocolAdapter Adapter { get; }
    DeviceOptions Options { get; }
    ValueTask HeartbeatAsync(CancellationToken ct);
}

// 数据处理通道：粘包拆包 / 校验 / 大小端归一化
public interface IDataPipeline
{
    IAsyncEnumerable<DeviceFrame> ProcessAsync(
        IAsyncEnumerable<ReadOnlyMemory<byte>> raw, CancellationToken ct);
}

// 注册中心：设备/协议/能力的全局注册与发现
public interface IRegistry
{
    void RegisterDevice(IDevice device);
    void RegisterAdapter(IProtocolAdapter adapter);
    IReadOnlyCollection<IDevice> Devices { get; }
    IDevice? Resolve(string deviceId);
}
```

> **与 V9 设备互换的关系（§1.12）**：上述 `IProtocolAdapter` 是「帧级」统一出口；V9 在其之上定义「设备生命周期级」的 `IDeviceProtocol`（`Configure/Connect/Disconnect/Poll/Write/HealthProbe`）+ `DeviceProfile` 点表描述文件 + `DeviceCapabilities` 能力协商 + `DeviceCatalog` 热替换，使**换 PLC 品牌/协议/传输只需改 Profile，上层零改动**。新协议插件应同时实现两者。

### 6.3 解析引擎 + 帧分发器（抽象隔离）

- **解析引擎**（`IDataPipeline` 实现）：基于 `System.IO.Pipelines` 的 `PipeReader` 做**零拷贝粘包拆包**，用 `ReadOnlySequence<byte>` 在 `Span` 上做帧头定位、长度截断、大小端读取、校验。
- **帧分发器**（`FrameDispatcher`）：把标准帧按 `Command` 路由到对应处理通道（读点表 / 写命令 / 告警 / 心跳），各通道独立 `Channel`，**互不影响**。

```csharp
// 粘包拆包 + 大小端 + 校验的零拷贝解析（生产级骨架）
public sealed class FrameParser : IDataPipeline
{
    private readonly FrameSpec _spec;
    public async IAsyncEnumerable<DeviceFrame> ProcessAsync(
        IAsyncEnumerable<ReadOnlyMemory<byte>> raw,
        [EnumeratorCancellation] CancellationToken ct)
    {
        await foreach (var seg in raw.WithCancellation(ct))
        {
            var span = seg.Span;
            int pos = FindHeader(span, _spec.Header);
            while (pos >= 0 && span.Length - pos >= _spec.MinSize)
            {
                int length = ReadLength(span.Slice(pos), _spec.Endianness);
                if (span.Length - pos < length + _spec.Overhead) break; // 半包，等下一段
                var payload = span.Slice(pos + _spec.HeaderLen, length);
                if (!VerifyChecksum(payload, _spec))
                { Metrics.ChecksumFailures++; pos++; continue; } // 噪声/坏帧
                yield return BuildFrame(payload, _spec.Endianness);
                pos += length + _spec.Overhead;
            }
        }
    }
}
```

### 6.4 抽象基类与设备配置

```csharp
public abstract class BaseProtocolDriver : IDriver
{
    public abstract Endianness Endianness { get; }
    protected readonly RingBuffer<byte> _ring = new(1 << 16); // 每设备独立 RingBuffer
    public abstract ValueTask<int> SendAsync(ReadOnlyMemory<byte> frame, CancellationToken ct);
    public abstract IAsyncEnumerable<ReadOnlyMemory<byte>> ReceiveAsync(CancellationToken ct);
}
```

```json
// device.modbus.1.json
{ "DeviceId": "PLC-A1", "Protocol": "Modbus", "Transport": "Tcp",
  "Host": "192.168.1.10", "Port": 502, "SlaveId": 1, "Endianness": "Big",
  "PollingMs": 1000, "HeartbeatMs": 5000, "BaudRate": 0 }
```

```csharp
builder.Services.AddOptions<DeviceOptions>()
       .Bind(builder.Configuration.GetSection("Device:PLC-A1"))
       .ValidateDataAnnotations()
       .ValidateOnStart();   // 运行时热更新：IOptionsMonitor<DeviceOptions>
```

### 6.5 心跳与健康检查

```csharp
builder.Services.AddHealthChecks()
    .AddCheck<DeviceHeartbeatHealthCheck>("plc-a1");   // 设备存活探针
// IDevice.HeartbeatAsync 按 HeartbeatMs 周期上报，失败则状态机切入 Reconnecting
```

### 6.6 生产级 PLC 问题对策表

| 问题           | 对策                                                              |
| ------------ | --------------------------------------------------------------- |
| 大小端差异        | `Endianness` 在 `IDriver` 声明，解析统一用 `BinaryPrimitives`（Span 上读写）  |
| 粘包/拆包        | `PipeReader` + `ReadOnlySequence` 累积缓冲，按 `Length` 截断            |
| 噪声/坏帧        | 帧头扫描 + 校验失败丢弃并 `Metrics.ChecksumFailures++`，不抛异常                |
| 校验失败         | 计数 + 可选重传；不影响其它帧                                                |
| 批量操作         | 命令合并 + `Channel` 批处理写；读用 `IAsyncEnumerable` 流式                  |
| RingBuffer   | 每设备独立 `RingBuffer`，背压满则丢最旧或阻塞（按策略）                              |
| 主线程⇄UI 线程    | 解析在后台 `Channel`/Task；UI 通过 `IProgress<T>` / `IScheduler`(Rx) 切回 |
| 连接设置         | `DeviceOptions` 收敛地址/端口/串口/波特率/从站号                              |
| **单次解析互不影响** | 每协议插件独立 ALC + 独立 `Channel` + 独立 `RingBuffer` + 独立状态机            |
| **GC 释放**    | `ArrayPool`/`RecyclableMemoryStream` 复用；解析用 `Span`；ALC 卸载强制 GC  |

---

## 7. PLC 设备 Stateless 状态机

> 设备状态机用 **Stateless**：声明式 `Configure` 定义转换、`Permit` 声明触发器→目标态、`Fire` 触发（见下文九态机）。Stateless 特性：声明式配置、启动期自动校验（非法转换在 `Fire` 时抛 `InvalidOperationException`）、事件驱动（`OnTransitioned`/`OnUnhandledTrigger`）、零反射高性能。

### 7.0 九态机定义（九种设备状态 + 九种设备指令）

九种设备状态：`停机 Stopped` · `启动中 Starting` · `运行中 Running` · `暂停中 Pausing` · `已暂停 Paused` · `停止中 Stopping` · `故障 Fault` · `维护 Maintenance` · `急停 EmergencyStop`  
九种设备指令：`启动 Start` · `停止 Stop` · `暂停 Pause` · `恢复 Resume` · `重置 Reset` · `急停 EmergencyStop` · `进入维护 EnterMaintenance` · `退出维护 ExitMaintenance` · `故障确认 FaultAcknowledge`

```csharp
using Stateless;

public enum DeviceState
{
    Stopped, Starting, Running, Pausing, Paused,
    Stopping, Fault, Maintenance, EmergencyStop
}

// 九种业务指令 + 五种内部信号（连接/心跳/完成由适配器产生）
public enum DeviceTrigger
{
    Start, Stop, Pause, Resume, Reset, EmergencyStop,
    EnterMaintenance, ExitMaintenance, FaultAcknowledge,   // —— 九种设备指令
    Connected, Fail, Paused, Stopped, Ready                // —— 内部信号
}

public sealed class DeviceStateMachine
{
    private readonly object _gate = new();                  // 守护：避免并发 Fire 竞态
    private readonly StateMachine<DeviceState, DeviceTrigger> _m;

    public DeviceStateMachine()
    {
        _m = new(DeviceState.Stopped);

        // Configure 转换规则；Permit(指令/信号 → 目标状态)
        _m.Configure(DeviceState.Stopped)
            .Permit(DeviceTrigger.Start, DeviceState.Starting);

        _m.Configure(DeviceState.Starting)
            .Permit(DeviceTrigger.Connected, DeviceState.Running)
            .Permit(DeviceTrigger.Fail, DeviceState.Fault)
            .Permit(DeviceTrigger.EmergencyStop, DeviceState.EmergencyStop);

        _m.Configure(DeviceState.Running)
            .Permit(DeviceTrigger.Pause, DeviceState.Pausing)
            .Permit(DeviceTrigger.Stop, DeviceState.Stopping)
            .Permit(DeviceTrigger.Fail, DeviceState.Fault)
            .Permit(DeviceTrigger.EnterMaintenance, DeviceState.Maintenance)
            .Permit(DeviceTrigger.EmergencyStop, DeviceState.EmergencyStop);

        _m.Configure(DeviceState.Pausing)
            .Permit(DeviceTrigger.Paused, DeviceState.Paused)
            .Permit(DeviceTrigger.EmergencyStop, DeviceState.EmergencyStop);  // G10：暂停中也可急停

        _m.Configure(DeviceState.Paused)
            .Permit(DeviceTrigger.Resume, DeviceState.Running)
            .Permit(DeviceTrigger.Stop, DeviceState.Stopping)
            .Permit(DeviceTrigger.EnterMaintenance, DeviceState.Maintenance)
            .Permit(DeviceTrigger.EmergencyStop, DeviceState.EmergencyStop);

        _m.Configure(DeviceState.Stopping)
            .Permit(DeviceTrigger.Stopped, DeviceState.Stopped)
            .Permit(DeviceTrigger.Fail, DeviceState.Fault);

        _m.Configure(DeviceState.Fault)
            .Permit(DeviceTrigger.FaultAcknowledge, DeviceState.Fault)   // 自环：确认并清报警锁存
            .Permit(DeviceTrigger.Reset, DeviceState.Stopped)
            .Permit(DeviceTrigger.EmergencyStop, DeviceState.EmergencyStop);

        _m.Configure(DeviceState.Maintenance)
            .Permit(DeviceTrigger.ExitMaintenance, DeviceState.Running)
            .Permit(DeviceTrigger.Stop, DeviceState.Stopping)
            .Permit(DeviceTrigger.EmergencyStop, DeviceState.EmergencyStop);

        _m.Configure(DeviceState.EmergencyStop)
            .Permit(DeviceTrigger.Reset, DeviceState.Stopped)
            .Permit(DeviceTrigger.EmergencyStop, DeviceState.EmergencyStop);  // G10：急停可幂等重入

        // 状态变化后：打点 + 立即更新 UI（见 7.0.3）
        _m.OnTransitioned(t =>
        {
            Metrics.StateTransition(t.Source.ToString(), t.Destination.ToString());
            UiDispatcher.UpdateDeviceState(t.Destination.ToString());
        });
        _m.OnUnhandledTrigger((state, trigger) =>
            Log.Warning("未处理触发 {Trigger} @ {State}", trigger, state));
    }

    public DeviceState State => _m.State;

    // 异步状态处理必须加锁验证当前态，避免竞态导致的状态错误
    public bool TryFire(DeviceTrigger trigger)
    {
        lock (_gate)
        {
            if (!_m.CanFire(trigger)) return false;   // 守护条件：非法转换直接拒绝
            _m.Fire(trigger);
            return true;
        }
    }
}
```

**完整状态转换关系表**

| 当前态              | 指令/信号                 | 目标态              | 说明           |
| ---------------- | --------------------- | ---------------- | ------------ |
| 停机 Stopped       | 启动 Start              | 启动中 Starting     | 开始连接设备       |
| 启动中 Starting     | Connected             | 运行中 Running      | 连接成功         |
| 启动中 Starting     | Fail                  | 故障 Fault         | 连接/初始化失败     |
| 启动中 Starting     | 急停 EmergencyStop      | 急停 EmergencyStop |              |
| 运行中 Running      | 暂停 Pause              | 暂停中 Pausing      |              |
| 运行中 Running      | 停止 Stop               | 停止中 Stopping     |              |
| 运行中 Running      | Fail                  | 故障 Fault         | 运行异常         |
| 运行中 Running      | 进入维护 EnterMaintenance | 维护 Maintenance   |              |
| 运行中 Running      | 急停 EmergencyStop      | 急停 EmergencyStop |              |
| 暂停中 Pausing      | Paused                | 已暂停 Paused       | 暂停完成         |
| 已暂停 Paused       | 恢复 Resume             | 运行中 Running      |              |
| 已暂停 Paused       | 停止 Stop               | 停止中 Stopping     |              |
| 已暂停 Paused       | 进入维护 EnterMaintenance | 维护 Maintenance   |              |
| 已暂停 Paused       | 急停 EmergencyStop      | 急停 EmergencyStop |              |
| 停止中 Stopping     | Stopped               | 停机 Stopped       | 停机完成         |
| 停止中 Stopping     | Fail                  | 故障 Fault         |              |
| 故障 Fault         | 故障确认 FaultAcknowledge | 故障 Fault（自环）     | 清报警锁存，状态不变   |
| 故障 Fault         | 重置 Reset              | 停机 Stopped       | 确认后复位        |
| 故障 Fault         | 急停 EmergencyStop      | 急停 EmergencyStop |              |
| 维护 Maintenance   | 退出维护 ExitMaintenance  | 运行中 Running      | 恢复生产         |
| 维护 Maintenance   | 停止 Stop               | 停止中 Stopping     |              |
| 维护 Maintenance   | 急停 EmergencyStop      | 急停 EmergencyStop |              |
| 暂停中 Pausing      | 急停 EmergencyStop      | 急停 EmergencyStop | G10：暂停中亦可急停  |
| 急停 EmergencyStop | 急停 EmergencyStop      | 急停 EmergencyStop | G10：幂等重入（自环） |
| 急停 EmergencyStop | 重置 Reset              | 停机 Stopped       | 必须复位清除急停     |

**7.0.1 异步状态处理与竞态防护**

- Stateless 的 `Fire` 是同步的；异步动作（连接、下发）应在 `Fire` 后由对应插件方法 `await` 执行，状态机只负责「态」，不阻塞。
- **必须验证当前态**：`TryFire` 用 `lock` + `_m.CanFire(trigger)` 守护，拒绝非法转换（如 `运行中` 直接收 `重置`），避免竞态导致状态错乱。
- 多设备并发：每个设备一个 `DeviceStateMachine` 实例，且**单个设备内的触发放进 §0.5.7 的 `shared_strand`**，做到「设备内串行、设备间并行、无锁」。

**7.0.2 守护条件（Guard）**  
复杂场景用 `PermitIf(trigger, dest, () => condition)` 增加运行期守卫，例如「仅在采集计数 < 阈值时才允许恢复」。

**7.0.3 状态变化后立即更新 UI**

- `OnTransitioned` 中调用 `UiDispatcher.UpdateDeviceState(...)`：PC 端经 Blazor `StateHasChanged`/MVVM `RaisePropertyChanged`；移动端 MAUI `MainThread.BeginInvokeOnMainThread`；桌面 WebView2 经 `CoreWebView2.PostWebMessageAsync` 推给前端。保证「状态一变，界面秒更」。

### 7.1 状态机解析流程图（帧解析阶段机）

协议字节流进入解析引擎后，按以下**六阶段状态机**推进，粘包自动回到阶段 1 循环：

```
┌─────────────────────────┐
│  阶段 1：定位帧头        │
│  丢弃前导噪声字节        │
└────────────┬────────────┘
             │
┌────────────▼────────────┐
│  阶段 2：读取长度字段    │  ← 字节不足则 return 等待
│  计算完整帧总字节数      │
└────────────┬────────────┘
             │
┌────────────▼────────────┐
│  阶段 3：等待完整帧      │
│  字节不足则缓冲等待      │
└────────────┬────────────┘
             │
┌────────────▼────────────┐         ┌───────────────────┐
│  阶段 4：校验通过？      │ ── 否 → │  丢弃首字节        │
└────────────┬────────────┘         │  重新搜索帧头      │
             │ 是                    └─────────┬─────────┘
┌────────────▼────────────┐                   │
│  阶段 5：上报 DataFrame │ ◄─────────────────┘
│  触发 FrameReceived 事件 │
└────────────┬────────────┘
             │
┌────────────▼────────────┐
│  阶段 6：消费帧并循环    │
│  继续解析缓冲区剩余数据  │
└────────────┬────────────┘
             │
  粘包 → 回到阶段 1（循环）
```

---

## 8. PLC 详细实现（以 Modbus 为例）

### 8.1 ModbusConfiguration 配置类

```csharp
// 连接器设置：类型，主机，端口，从站地址
public sealed class ConnectionSettings
{
    public string Type { get; set; } = "Tcp";          // Tcp / Rtu
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 502;
    public byte SlaveId { get; set; } = 1;
    public int BaudRate { get; set; }                 // RTU 串口波特率
    public string ComPort { get; set; } = "COM3";     // RTU 串口号
}

// 寄存器定义：名称，地址，功能码，数据类型，字节序，单位
public sealed class ModbusRegister
{
    public string Name { get; set; } = "";
    public ushort Address { get; set; }
    public byte FunctionCode { get; set; } = 3;       // 仅写「读」功能码
    public string DataType { get; set; } = "INT16";   // INT16 / FLOAT32 / STRING
    public string ByteOrder { get; set; } = "ABCD";   // ABCD / DCBA / DABC ...
    public double Scale { get; set; } = 1.0;
    public double Offset { get; set; } = 0.0;
    public string Unit { get; set; } = "";
}

public sealed class ModbusConfiguration
{
    public ConnectionSettings Connection { get; set; } = new();
    public List<ModbusRegister> Registers { get; set; } = new();
}
```

### 8.2 字节序配置

支持多种 PLC 厂商字节序，解析时按 `ByteOrder` 重组 `ushort[]`：

| 字节序         | 含义             | 典型厂商          |
| ----------- | -------------- | ------------- |
| ABCD        | 大端顺序（字内/字间均大端） | 西门子 Siemens   |
| DCBA        | 完全小端（字内/字间均小端） | 施耐德 Schneider |
| DABC        | 字交换大端          | 三菱 Mitsubishi |
| BADC / CDAB | 字内交换 / 字间交换    | 其它厂商          |

```csharp
// 根据 ByteOrder 把寄存器原始 ushort[] 重排为逻辑字序
static ushort[] Reorder(ushort[] raw, string byteOrder) => byteOrder switch
{
    "ABCD" => raw,
    "DCBA" => raw.Reverse().ToArray(),
    "DABC" => SwapWords(raw),          // 字交换
    "CDAB" => SwapWords(raw),
    _ => raw
};
```

### 8.3 智能功能码映射

配置文件**只需写「读」功能码**，插件自动推导对应「写」功能码：

```csharp
// 功能码1读线圈  -> 功能码5写单个线圈
// 功能码3读保持寄存器 -> 功能码6写单个寄存器
// 功能码2/4 只读，写操作抛异常
static byte MapWriteFunction(byte readFunctionCode) => readFunctionCode switch
{
    1 => 5,
    3 => 6,
    2 => throw new InvalidOperationException("线圈离散输入(FC2)只读"),
    4 => throw new InvalidOperationException("输入寄存器(FC4)只读"),
    _ => readFunctionCode
};
```

### 8.4 数据类型智能转换

```csharp
static object ConvertData(ushort[] data, ModbusRegister register)
{
    return register.DataType switch
    {
        "INT16" =>
            ((short)data[0] * register.Scale) + register.Offset,
        "FLOAT32" =>
        {
            float floatValue = ConvertToFloat(data, register.ByteOrder);
            return (floatValue * register.Scale) + register.Offset;
        },
        "String" => ConvertToString(data),     // 自动拼接 ASCII
        _ => data[0]
    };
}

static float ConvertToFloat(ushort[] data, string byteOrder)
{
    var ordered = Reorder(data, byteOrder);    // 先按字节序重排
    var bytes = new byte[4];
    BinaryPrimitives.TryWriteUInt16BigEndian(bytes.AsSpan(0), ordered[0]);
    BinaryPrimitives.TryWriteUInt16BigEndian(bytes.AsSpan(2), ordered[1]);
    return BitConverter.ToSingle(bytes, 0);
}
```

### 8.5 JSON 配置文件（新增设备只需加一段）

```json
{
  "Connection": { "Type": "Tcp", "Host": "192.168.1.10", "Port": 502, "SlaveId": 1, "BaudRate": 0 },
  "Registers": [
    { "Name": "Temperature", "Address": 30001, "FunctionCode": 3, "DataType": "FLOAT32", "ByteOrder": "ABCD", "Scale": 0.1, "Offset": 0, "Unit": "℃" },
    { "Name": "Pressure",    "Address": 30003, "FunctionCode": 3, "DataType": "INT16",   "ByteOrder": "ABCD", "Scale": 1,   "Offset": 0, "Unit": "kPa" },
    { "Name": "DeviceName",  "Address": 40001, "FunctionCode": 3, "DataType": "String",  "ByteOrder": "ABCD" }
  ]
}
```

### 8.6 插件 ModbusPlugin

```csharp
[Plugin("Modbus", "1.0.0")]
public sealed class ModbusPlugin : IPlugin
{
    // 事件：数据变化 / 错误 / 连接状态
    public event Action<string, object>? DataChanged;
    public event Action<Exception>? ErrorOccurred;

    private ModbusConfiguration? _config;
    private readonly Timer _monitor;   // ModbusMonitor 心跳/轮询
    private IModbusMaster? _master;    // NModbus4 master（复用 TCP 连接）

    public async ValueTask ConfigureServicesAsync(IPluginContext ctx, CancellationToken ct)
    {
        _config = await LoadConfigurationAsync("device.modbus.1.json", ct);
    }

    public async ValueTask StartAsync(CancellationToken ct)
    {
        await ConnectAsync(ct);
        _ = ReadAllRegistersAsync(ct);   // 后台持续轮询
    }

    // 1) 加载配置
    public ValueTask<ModbusConfiguration> LoadConfigurationAsync(string file, CancellationToken ct)
        => ValueTask.FromResult(JsonSerializer.Deserialize<ModbusConfiguration>(
            File.ReadAllText(file), AppJsonOpts)!);

    // 2) 连接设备（连接池复用 TCP）
    public async ValueTask ConnectAsync(CancellationToken ct)
    {
        var factory = new TcpMasterConnectionFactory(_config!.Connection.Host, _config.Connection.Port);
        _master = factory.CreateMaster();   // 复用底层 TCP 连接
    }

    // 3) 读取寄存器（批量读取 + 异步 + 重试）
    public async Task ReadAllRegistersAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                foreach (var reg in _config!.Registers)
                {
                    ushort[] raw = await ReadWithRetryAsync(reg, ct);
                    var value = ConvertData(raw, reg);
                    DataChanged?.Invoke(reg.Name, value);   // 推送变化
                }
                await Task.Delay(_config.PollingMs, ct);
            }
            catch (Exception ex) { ErrorOccurred?.Invoke(ex); await Task.Delay(1000, ct); }
        }
    }

    private async ValueTask<ushort[]> ReadWithRetryAsync(ModbusRegister reg, CancellationToken ct)
    {
        // 批量读取相邻寄存器一次性取回
        return await _master!.ReadHoldingRegistersAsync(_config!.Connection.SlaveId,
            reg.Address, (ushort)RegisterWordCount(reg), default);
    }
}
```

### 8.7 优化清单

| 优化项    | 实现                                                       |
| ------ | -------------------------------------------------------- |
| 批量读取优化 | 相邻寄存器一次性读取（`ReadHoldingRegisters` 多字），减少往返               |
| 连接池管理  | 复用 TCP 连接（`TcpMasterConnectionFactory` 单例 master），避免频繁建连 |
| 异步编程   | 所有 IO 操作 `async/await`，不阻塞采集线程                           |
| 错误重试机制 | 网络异常自动重试（结合 Polly 重试/熔断），失败指数退避                          |
| 心跳/监控  | `Timer`(ModbusMonitor) 周期健康检查，异常触发状态机 `Fail`/`Recover`   |
| 零分配解析  | `BinaryPrimitives` 在 `Span` 上读写，`ArrayPool` 复用缓冲区        |

---

## 9. AIOT：MQTT + Channel + Redis 背压

```csharp
// EMQX broker；Channel 生产者/消费者
// MQTT 主题（多段匹配，含 洲/国家/省/市/区/产线/批次 维度，约束 g）：
//   devices/{continent}/{country}/{province}/{city}/{district}/{line}/{batch}/{deviceId}/{type}
//   + 单级通配（设备段）   # 多级通配（从某一级起全部）
//   例：订阅 devices/asia/cn/gd/sz/nanshan/lineA/batch01/+/telemetry → 收某产线全部设备遥测
//       订阅 devices/asia/cn/# → 收全国数据

// ⚠️ 背压唯一真相源 = BoundedChannel（零外部依赖，推荐 MVP）
//    跨多个采集节点做集群级限流时，才改用 Redis 深度计数为唯一闸门（二选一，不要叠加！见 G6）
var channel = Channel.CreateBounded<DeviceFrame>(new BoundedChannelOptions(1000)
{
    FullMode = BoundedChannelFullMode.Wait,   // 满了生产者等待：不丢帧、不爆内存
    SingleReader = false, SingleWriter = false
});

async Task ProduceAsync(DeviceFrame frame, CancellationToken ct)
{
    await channel.Writer.WriteAsync(frame, ct);   // 唯一背压闸门
}

// 消费者：Channel → MQTTnet 发布到 EMQX（主题拼入 地理/产线/批次 路径）
await foreach (var frame in channel.Reader.ReadAllAsync(ct))
{
    var d = frame.Device;   // DeviceOptions 含 Continent/Country/Province/City/District/Line/Batch 等元数据
    var topic = $"devices/{d.Continent}/{d.Country}/{d.Province}/{d.City}/{d.District}/{d.Line}/{d.Batch}/{d.DeviceId}/{frame.Type}";
    await mqttClient.PublishAsync(new MqttApplicationMessage
    {
        Topic = topic,
        PayloadSegment = frame.Payload,
        QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce
    }, ct);
}
```

> 背压单闸门（G6 修正）：仅用 `BoundedChannel` 的 `Wait` 一处设闸门，避免与 Redis 双闸门状态不一致导致的空等/背压失效。多节点集群限流时切换到 Redis 深度计数作为**唯一**闸门，二者绝不叠加。

---

## 10. 弹性：Polly（超时 / 熔断 / 舱壁 / 优先级 / 死信 / 重试）

```csharp
// Polly v8 ResiliencePipeline
var pipeline = new ResiliencePipelineBuilder<DeviceFrame>()
    .AddRetry(new RetryStrategyOptions<DeviceFrame>   // 重试次数
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromMilliseconds(200),
        OnRetry = args => { Metrics.Retries++; return ValueTask.CompletedTask; }
    })
    .AddTimeout(TimeSpan.FromSeconds(2))               // 超时
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions<DeviceFrame>  // 熔断降级
    {
        BreakingDuration = TimeSpan.FromSeconds(30),
        MinimumThroughput = 20,
        ShouldHandle = new PredicateBuilder<DeviceFrame>().Handle<IOException>()
    })
    .AddConcurrencyLimiter(10)                         // 舱壁（并发上限）
    .Build();

// 死信队列：多次失败进入 DLQ
async ValueTask SendWithDlqAsync(DeviceFrame f, CancellationToken ct)
{
    try { await pipeline.ExecuteAsync(async tk => await PublishAsync(f, tk), ct); }
    catch { await deadLetter.EnqueueAsync(f, ct); }
}
```

> 优先级队列：在 `Channel` 之前用 `PriorityChannel<T>` 或按 QoS 分级多个 `Channel`（高/中/低）。

---

## 11. CQRS：Mediator(martinothamar, MIT)（进程内）+ Outbox/Inbox（Wolverine 可靠消息）

```csharp
// 命令（CQRS 写路径）— 使用 MIT 的 Mediator（martinothamar/Mediator），与 §0.5.2 一致
public record WriteRegisterCommand(string DeviceId, ushort Address, short Value) : ICommand;

[EventHandler]   // 触发 Mediator Source Generator 编译期注册（无需反射扫描，AOT 安全）
public sealed class WriteRegisterHandler : ICommandHandler<WriteRegisterCommand>
{
    private readonly IRegistry _registry;
    public WriteRegisterHandler(IRegistry registry) => _registry = registry;
    public ValueTask Handle(WriteRegisterCommand cmd, CancellationToken ct)
    {
        var dev = _registry.Resolve(cmd.DeviceId) ?? throw new KeyNotFoundException(cmd.DeviceId);
        return dev.Adapter.WriteAsync(new WriteCommand(cmd.Address, cmd.Value), ct);
    }
}
// 查询（读路径）：IQuery<T> / IQueryHandler<,>；服务注入 IMediator（或具体 Mediator），
// 调用 await mediator.Send(cmd, ct) 完成派发；横切行为用 IPipelineBehavior<,> 注册。

// Outbox/Inbox：Wolverine 持久化发件箱/收件箱保证"库写 + 消息投递"原子
builder.Host.UseWolverine(opts =>
{
    opts.UseEntityFrameworkCoreTransactions();
    opts.UsePostgresPersistenceAndMessageStore(connStr);
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.UseDurableInboxOnAllListeners();
});
```

> 若不便引入 Wolverine，可**自建事务发件箱**：业务表 + `Outbox` 表同事务写入，后台 Agent 轮询 `Outbox` 投递到 EMQX/RabbitMQ，成功即标记（幂等消费靠 `Inbox` 去重表）。

---

## 12. IOC + 模块化：Scrutor + Carter

```csharp
// Scrutor：程序集扫描 + 自动注册 + 装饰器(AOP)
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(ModbusAdapter))
    .AddClasses(c => c.AssignableTo<IProtocolAdapter>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// 外观 AOP：日志/耗时装饰器（Scrutor Decorate）
builder.Services.AddScoped<IProtocolAdapter, ModbusAdapter>();
builder.Services.Decorate<IProtocolAdapter, AdapterTelemetryDecorator>();

// Carter：模块化端点
builder.Services.AddCarter();
public sealed class DeviceModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/devices/{id}", (string id, IRegistry r) => r.Resolve(id) is { } d
            ? Results.Ok(d.Options) : Results.NotFound());
    }
}
// Program.cs: app.MapCarter();
```

---

## 13. AI：MEAI + MCP + Qdrant（RAG 质检 / 诊断）

```csharp
// Microsoft.Extensions.AI 统一 IChatClient；MCP 暴露工具；Qdrant 存设备知识向量
IChatClient chat = new OpenAIClient(apiKey).AsChatClient("gpt-4o-mini");
var qdrant = new QdrantClient("localhost");

// 1) 设备日志/手册嵌入进 Qdrant（离线）
await qdrant.UpsertAsync("plc-kb", embeddings);

// 2) 运行时：检索 + MEAI 生成诊断（通过 MCP 工具调用设备）
var hits = await qdrant.SearchAsync("plc-kb", queryVec, limit: 5);
var reply = await chat.CompleteAsync(
    $"基于以下知识：{hits.AsContext()}；设备 PLC-A1 报 0x05 错误，给出处置步骤。");
```

> MCP：`modelcontextprotocol/csharp-sdk` 把 `AIFunction` 暴露为 MCP Tools，供 MEAI/Agent 调用（读点位、下发命令）。

---

## 14. 数据库拓扑

| 数据           | 存储                                 | 用法                                 |
| ------------ | ---------------------------------- | ---------------------------------- |
| 边缘/本地业务+缓存   | **SQLite + EF Core + Dapper AOT**  | 边缘网关本地落库；热读用 `DapperAOT` 生成 AOT 代码 |
| 时序（点位/指标）    | **InfluxDB**                       | 高频采样，按设备+时间检索                      |
| 业务（订单/配置/用户） | **PostgreSQL**                     | 主业务库；Outbox/Inbox 亦落此              |
| 日志（热/冷）      | **Seq**（实时排查）+ **ClickHouse**（冷分析） | 结构化日志 → Seq；海量审计/日志分析 → ClickHouse |

```csharp
builder.Services.AddDbContext<EdgeDbContext>(o => o.UseSqlite("Data Source=edge.db"));
builder.Services.AddDbContext<BusinessDbContext>(o => o.UseNpgsql(connStr));
await influx.WritePointsAsync(points, ct);
```

---

## 15. API 网关：YARP + Aneiang.Yarp（约束 b：网关 WAL + WAF）

统一入口网关，承载「单体 ⇄ 微服务」切分、路由、负载均衡、限流、TLS 终止。**YARP** 之上叠加 **Aneiang.Yarp**（`AneiangSoft/Aneiang.Yarp`，NuGet `Aneiang.Yarp`）获得生产级网关能力：

- **网关 WAL（审计类比）**：配置变更审计（环形缓冲）+ 原子文件持久化。每次路由/集群变更写入 `ConcurrentQueue` 环形审计日志（上限 200 条，无锁、零外部依赖），并以「临时文件 + 原子 `File.Move`」落盘，断电也不损坏；运行时用 `ReaderWriterLockSlim`（读多写少）保证线程安全，支持 Snapshot & Rollback 回滚。
  
  > 措辞澄清：Aneiang.Yarp 官方称其为「配置变更审计 + 原子文件持久化」，**并非数据库意义上的 WAL 事务日志**；本文档沿用「WAL」作类比，团队勿误以为是事务日志。
- **网关 WAF（Web 应用防火墙）**：内置请求级安全过滤，拦截 **SQL 注入 / XSS / 路径遍历** 等攻击，并支持 **黑白名单** 与自动注入**安全响应头**（CSP/HSTS/X-Content-Type-Options 等）。这是 PLC/AIOT 平台对外暴露 REST/OPC 网关时的关键防线——工业设备接口一旦被注入即可越权下发指令。

> 依赖说明：`Aneiang.Yarp`（核心：动态配置/限流/熔断/审计/自动注册）+ **`Aneiang.Yarp.Dashboard`**（WAF 与可视化网关管理 `UseAneiangYarpDashboard()` 来自此包）。**WAF 默认关闭**，须显式开启（见下方配置）。

```csharp
// 基础 YARP（官方）
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// 叠加 Aneiang.Yarp：动态路由 CRUD + 限流 + 熔断 + 审计(WAL) + IP 隔离负载均衡 + 自动注册 + WAF
builder.Services.AddAneiangYarp();          // 一行接入：动态配置/限流/熔断/审计/健康检查/WAF
// builder.Services.AddAneiangYarp(enableRegistration: false);  // 生产关闭注册 API
// builder.Services.AddAneiangYarp(enableWaf: true);           // WAF 默认关闭，须显式开启（见下方配置）

var app = builder.Build();
app.UseRouting();
app.UseAneiangYarpDashboard();              // 可视化网关管理（可选）
// WAF 中间件须置于 MapReverseProxy 之前，先过滤恶意流量再路由
app.UseAneiangYarpWaf();                    // 启用 WAF 过滤（SQL 注入/XSS/路径遍历/黑白名单）
app.MapReverseProxy();                       // 路由 /devices /aiot /ai 等到后端切片
```

Aneiang.Yarp 关键能力：

| 能力          | 说明                                                                           |
| ----------- | ---------------------------------------------------------------------------- |
| 动态路由/集群     | `InMemoryConfigProvider` 运行时 CRUD，热更新无需重启；`appsettings.json` 静态配置 + 动态配置双写合并 |
| 限流          | `RateLimitConfigProvider` 限流参数（配合 §0.5.1 的 `AspNetCoreRateLimit`）            |
| 熔断          | `CircuitBreakerMiddleware` 熔断状态管理，置于 YARP 之前                                 |
| 审计 WAL      | 环形缓冲审计日志 + 原子文件写入，配置可回滚（Snapshot & Rollback）                                 |
| IP 隔离负载均衡   | 按客户端 IP 路由到本地实例，多开发者调试互不冲突                                                   |
| 自动注册        | `Aneiang.Yarp.Client`（零 YARP 依赖）微服务自注册，指数退避重试                                |
| **WAF（重点）** | 内置 Web 应用防火墙：SQL 注入 / XSS / 路径遍历检测 + IP 黑白名单 + 安全响应头注入                       |

**WAF 配置示例（appsettings.json）**：

```json
"AneiangYarp": {
  "Waf": {
    "Enabled": true,
    "SqlInjection": true,   // 拦截 ' OR 1=1 -- 等注入特征
    "Xss": true,            // 拦截 <script> 等跨站脚本
    "PathTraversal": true,  // 拦截 ../ 等路径穿越
    "BlackList": [ "10.0.0.66" ],          // 黑名单 IP 直接 403
    "WhiteList": [ "192.168.1.0/24" ],     // 白名单放行（可选）
    "SecurityHeaders": {                   // 出站自动注入安全响应头
      "Content-Security-Policy": "default-src 'self'",
      "X-Content-Type-Options": "nosniff",
      "Strict-Transport-Security": "max-age=31536000"
    }
  }
}
```

> **生产级常识**：WAF 属「纵深防御」第一层，不能替代业务鉴权（见 §0.5 的 RBAC/ABAC、Casbin/OpenAuth.Net）。对 PLC 指令类接口务必叠加 `RequirePermission` 与操作审计；WAF 拦截的攻击应写入日志库（Seq）并触发告警。

```json
// appsettings.json（YARP 基础路由，Aneiang.Yarp 另提供动态 API/持久化）
"ReverseProxy": {
  "Routes": {
    "devices": { "Match": { "Path": "/devices/{**catch-all}" }, "ClusterId": "plc-cluster" },
    "aiot":    { "Match": { "Path": "/aiot/{**catch-all}" },  "ClusterId": "aiot-cluster" }
  },
  "Clusters": {
    "plc-cluster":  { "Destinations": { "d1": { "Address": "http://plc-host:8080" } } },
    "aiot-cluster": { "Destinations": { "d1": { "Address": "http://aiot-host:8081" } } }
  }
}
```

> 演进期：底座契约不变，把高负载切片注册到 YARP/Aneiang.Yarp 背后不同 Cluster，即可从模块化单体平滑演进为微服务，无需改动插件代码。

---

## 16. 实时图表：ScottPlot（优先）/ LiveCharts2

高频时序点位渲染优先 **ScottPlot**（零依赖、可渲染百万点、性能极佳）；需要 MVVM 组件化（Blazor/WPF）时用 **LiveCharts2**。

```csharp
// ScottPlot：实时滚动折线（点位监控）
var plt = new ScottPlot.Plot(800, 400);
var sig = plt.AddSignal(new double[1000]);
int next = 0;
// 每收到一帧设备点位：
sig.Ys[next] = value; next = (next + 1) % sig.Ys.Length;
plt.Render();   // 高频刷新，开销低
```

```xml

<lvc:CartesianChart Series="{Binding Series}" />
```

---

## 17. 前端产品体系：HTMX + Tailwind + Blazor Hybrid + WebApi

构建一个 **PLC AIOT 产品**，统一风格（现代简约 · 科技天蓝 · 玻璃态），覆盖三端。本章给出完整的**设计系统 → 五大页面（营销/驾驶舱/注册/设备/社媒+Notion 仪表板）→ 多端适配 → 三段匹配布局规范**，所有代码片段均可在 `samples/PlcAiot.Web/` 找到对应的可运行自包含 HTML。

### 17.0 设计系统（Design System）— 统一视觉与交互基线

> 规则：**风格统一、页面统一、操作简单不超过三层、路径统一、布局合理、说明清晰**。所有页面共用同一套 Design Token，避免多端风格漂移。

| 维度  | 规范          | 取值 / 做法                                                                         |
| --- | ----------- | ------------------------------------------------------------------------------- |
| 主色  | 科技天蓝（brand） | `#3b82f6`(500) / `#2563eb`(600) / `#60a5fa`(400)，渐变 `from-sky-400 to-brand-600` |
| 辅助  | 语义色         | 在线=emerald、预警=amber、告警=rose、信息=slate                                            |
| 玻璃态 | `.glass`    | `bg-white/60 + backdrop-blur(14px)`；暗色 `bg-slate-900/55`                        |
| 圆角  | 卡片/按钮       | 卡片 `rounded-2xl`、按钮 `rounded-lg/xl`、胶囊 `rounded-full`                           |
| 字体  | 无衬线         | `"Segoe UI","Microsoft YaHei",system-ui`                                        |
| 主题  | 亮/暗         | `class="dark"` + `tailwind.config.darkMode='class'`，按钮一键切换并持久化                  |
| 栅格  | 响应式         | `grid md:grid-cols-3`、`lg:grid-cols-4`，移动端优先单列                                  |
| 间距  | 8 倍数        | `p-4/p-5/p-6`、`gap-4/gap-6`                                                     |

**统一数据同步（多段匹配）**：所有设备/页面通道经 MQTT 主题 `devices/{continent}/{country}/{province}/{city}/{district}/{line}/{batch}/{deviceId}/{type}` 做服务端多段匹配（洲/国家/省/市/区/产线/批次/设备ID/类型 共 9 段），服务端据 `type` 段路由到对应处理器并写入对应存储；前端统一订阅该主题树（`+` 单级、`#` 多级通配），保证三端数据一致（详见 §9、§17.8）。

### 17.1 营销官网（落地页）

**结构（6 区，单一滚动、路径统一 `/`、`#features`、`#cases`、`#pricing`）**：导航栏（Logo+菜单+CTA）→ Hero（大标题+副标题+双 CTA+渐变背景）→ 功能特性（3 列网格，图标+标题+描述）→ 客户评价轮播 → 定价表（3 方案，含「最受欢迎」高亮）→ 页脚。现代简约 · 科技天蓝 · 玻璃态。

> 可运行样板：`samples/PlcAiot.Web/landing.html`（直接浏览器打开即可，含主题无关的渐变光斑、轮播自动播放、定价高亮）。

**关键代码骨架**（Tailwind + 玻璃态 + 渐变 Hero）：

```html

<nav class="glass sticky top-0 z-30 border-b border-white/40">
  <div class="max-w-6xl mx-auto flex items-center justify-between px-6 py-4">
    <span class="font-bold text-lg">PLC·<span class="text-brand-500">AIOT</span></span>
    <div class="hidden md:flex gap-7 text-sm"><a href="#features">功能</a><a href="#pricing">定价</a><a href="#docs">文档</a></div>
    <a href="#pricing" class="bg-brand-500 hover:bg-brand-600 text-white px-4 py-2 rounded-lg">免费试用</a>
  </div>
</nav>


<header class="relative overflow-hidden">
  <div class="absolute inset-0 bg-gradient-to-br from-sky-100 via-indigo-50 to-white"></div>
  <div class="absolute -top-24 -right-24 w-96 h-96 bg-brand-300/30 rounded-full blur-3xl"></div>
  <div class="relative max-w-6xl mx-auto text-center py-28 px-6">
    <h1 class="text-5xl md:text-6xl font-extrabold text-slate-800">工业物联网，开箱即插即用</h1>
    <p class="mt-5 text-lg text-slate-500">丢一个 DLL 即上线新协议，零分配采集保障 7×24 稳定。</p>
    <div class="mt-9 flex justify-center gap-4">
      <a href="#demo" class="bg-brand-500 text-white px-7 py-3 rounded-xl shadow-lg shadow-brand-500/30">查看演示</a>
      <a href="#docs" class="border border-brand-300 text-brand-600 px-7 py-3 rounded-xl">了解更多</a>
    </div>
  </div>
</header>


<section id="features" class="grid md:grid-cols-3 gap-6 max-w-6xl mx-auto py-20 px-6">
  <div class="glass rounded-2xl p-6 border border-white/50">
    <div class="text-2xl">🔌</div><h3 class="font-semibold mt-4">协议热插拔</h3>
    <p class="text-slate-500 text-sm mt-2">Modbus / S7 / OPC-UA / 三菱 MC 即插即用。</p>
  </div>
  
</section>
```

### 17.2 后台管理数据仪表盘（运营驾驶舱 · 8 页 SPA）

**结构（操作 ≤ 2 层：驾驶舱 → 模块）**：基于 hash 路由的 **8 页 SPA**——侧边栏导航（驾驶舱/设备/告警/统计/实时看板/地图/系统/后端管理）→ 顶部搜索栏 + 用户头像 + 主题切换 + 通知铃 → 各页内容区卡片化。亮/暗主题，科技感玻璃态；移动端以横向滚动导航兜底。所有页面共用 §17.0 的 Design Token，导航/路径/层级严格遵循 §17.8。

> 可运行样板：`samples/PlcAiot.Web/cockpit.html`（8 页 SPA：侧边栏/KPI/SVG 组合图/模拟 MQTT 实时流/通知浮层/主题切换；**地图页 Three.js 全球 3D 地球**、**实时看板页 Three.js PLC 3D 结构透视**；直接浏览器打开即可，Three.js 经 importmap CDN 加载；设备详情改为右侧抽屉（信息/心跳/操作/日志/指令/计划/任务/地图 八 Tab），移动端横向导航 + 响应式画布高度 `clamp` + 页面淡入过渡，顶部全局搜索联动设备筛选）。

> **静态站点生产版（HTMX + Axios，前后端分离推荐）**：`wwwroot/`（由 `Feature.csproj` 的 ASP.NET Core `StaticFiles` 托管）——`index.html` 外壳（侧边栏/顶栏/视图容器）+ `pages/*.html` 八个 HTMX 片段（`hx-get` 切换、`#view` 注入）+ `js/app.js`（监听 `htmx:afterSwap` 按 `data-page` 渲染、Axios 拉取并缓存 `data/data.json`、SVG 折线/柱状/环图）+ `css/styles.css`（§17.0 设计令牌落地：亮/暗双主题、玻璃态、响应式可折叠侧栏）+ `data/data.json`（Mock 数据）。依赖 `vendor/htmx.min.js`、`vendor/axios.min.js` 已本地化，可离线运行；地图/实时看板改用自包含 SVG（程序化 PLC 机架 + 等距投影全球分布），无需 Three.js CDN。打开方式：由 ASP.NET Core 托管访问 `/`，或 `python3 -m http.server` 于 `wwwroot/` 根目录访问。

**八页职责与落点**：

| 页面   | 职责                                                                                 | 关键技术 / 出处                           |
| ---- | ---------------------------------------------------------------------------------- | ----------------------------------- |
| 驾驶舱  | 4 KPI（带 ↑/↓ 趋势）+ 折线/柱状组合图 + 实时活动流 + 设备状态分布 + 通知浮层                                  | SVG 图表 / 模拟 MQTT 流                  |
| 设备   | 设备卡片网格 + 状态筛选 + 详情抽屉（信息/心跳/操作/日志/指令/计划/任务/地图，钻取 ≤ 3 层）                             | §17.4 `IRegistry`                   |
| 告警   | `FaultEvent` 全生命周期（活跃/已确认/已解决），确认/解决操作                                             | §1.13                               |
| 统计   | 吞吐折线 + 状态占比环图 + 告警趋势柱状 + OEE/日均采集点                                                 | SVG                                 |
| 实时看板 | **PLC 3D 结构透视**：程序化机架（电源/CPU/DI/DO/AI/AO/通信）+ 模块着色 + 悬停拾取遥测；预留 Blender MCP→glTF 加载 | Three.js / GLTFLoader               |
| 地图   | **Three.js 全球 3D 地球**：园区设备点（按状态着色）+ 中心枢纽弧线 + 点击查看详情                                | Three.js（OrbitControls + Raycaster） |
| 系统   | 服务健康 + 资源仪表 + 边缘数据链路配置（SQLite / Mosquitto / DataSync）                              | §17.9                               |
| 后端管理 | 插件 / 用户 / 主题路由（九段）/ 连接器 四 Tab 管理                                                   | FastEndpoints / §9                  |

**技术要点**：

- **KPI 卡片**：设备在线率、今日采集点、活跃告警、AI 诊断命中率（带 ↑/↓ 趋势箭头 + 进度条）。
- **图表区**：Web 端用 ScottPlot/LiveCharts2；自包含 Demo 用内联 SVG 折线+柱状组合（离线可用，见 cockpit.html）。
- **实时活动流**：生产订阅 `devices/.../#/telemetry`（`+`/`#` 通配）；Demo 用 `setInterval` 模拟 MQTT 事件，逻辑与 `mqtt.js` 订阅一致。
- **通知中心**：SignalR/WebSocket 推送，浮层展示设备告警与 AI 诊断。
- **Three.js 地图（§地图页）**：`three@0.160` ESM（importmap）+ `OrbitControls` 自转/拖拽；`SphereGeometry` 地球 + 经纬线框 + 大气辉光；设备点按 `STATUS_COLOR` 着色，枢纽（郑州）向各园区画 `QuadraticBezierCurve3` 弧线；`Raycaster` 拾取点显示园区详情。**集成事件主题**沿用 §17.8 九段匹配。
- **PLC 3D 结构透视（§实时看板页）**：用 `BoxGeometry` 程序化拼装 PLC 机架（导轨 + 7 个模块盒 + LED），`MeshStandardMaterial` 金属质感，`OrbitControls` 旋转 + `Raycaster` 悬停高亮并联动右侧实时遥测面板。**生产级资产管线**：由 **Blender MCP**（连接 Blender 生成高保真 PLC 模型）→ 导出 `.glb`（glTF/GLB）→ 前端 `GLTFLoader` 加载 `assets/plc-rack.glb`；无外部模型时回退到上述程序化机架（见 cockpit.html 中 `boardLoadBtn` 的 GLTF 加载分支）。

**关键代码骨架（侧边栏 + 主题切换 + Three.js 引入）**：

```html


<aside class="w-60 glass border-r">
  <nav class="px-3 py-4 space-y-1 text-sm">
    <a href="#dashboard" class="nav-active ...">🛰️ 驾驶舱</a><a href="#devices">🏭 设备</a><a href="#alarms">🔔 告警</a>
    <a href="#statistics">📊 统计</a><a href="#board">📡 实时看板</a><a href="#map">🗺️ 地图</a>
    <a href="#system">⚙️ 系统</a><a href="#admin">🧩 后端管理</a>
  </nav>
  <div class="text-xs"><span class="w-2 h-2 rounded-full bg-emerald-400 animate-pulse"></span> MQTT 已连接</div>
</aside>


<script type="importmap">{ "imports": {
  "three": "https://cdn.jsdelivr.net/npm/three@0.160.0/build/three.module.js",
  "three/addons/": "https://cdn.jsdelivr.net/npm/three@0.160.0/examples/jsm/" } }</script>
<script type="module">
  import * as THREE from 'three';
  import { OrbitControls } from 'three/addons/controls/OrbitControls.js';
  import { GLTFLoader } from 'three/addons/loaders/GLTFLoader.js';
  // 地图：SphereGeometry 地球 + 设备点 + 枢纽弧线；看板：BoxGeometry 机架 + 模块拾取
</script>
```

### 17.3 用户注册表单（移动端优先）

**结构（移动端单列优先，字段清晰标签 + 错误提示）**：手机号输入（带验证码按钮）→ 设置密码（强度指示器）→ 确认密码 → 用户协议勾选 → 注册按钮（带 loading 状态）→ 第三方登录（微信/支付宝图标）。每个字段独立校验，错误就近提示。

**关键代码骨架**（HTMX 提交 + 密码强度 + loading + 第三方登录）：

```html
<form class="max-w-sm mx-auto p-6 space-y-4" hx-post="/api/register" hx-target="#result" hx-indicator="#regBtn">
  
  <label class="block text-sm">手机号</label>
  <input id="phone" required pattern="1\d{10}" placeholder="请输入手机号" class="w-full rounded-lg border px-3 py-2" />
  <div class="flex gap-2">
    <input placeholder="验证码" class="flex-1 rounded-lg border px-3 py-2" />
    <button id="codeBtn" class="px-3 py-2 rounded-lg glass" hx-post="/api/sms" hx-include="#phone">获取验证码</button>
  </div>
  
  <label class="block text-sm">设置密码</label>
  <input type="password" id="pwd" oninput="updateStrength()" class="w-full rounded-lg border px-3 py-2" />
  <div id="strength" class="h-1.5 rounded bg-slate-200 overflow-hidden">
    <span id="strengthBar" class="block h-full w-0 bg-rose-500 transition-all"></span>
  </div>
  <input type="password" placeholder="确认密码" class="w-full rounded-lg border px-3 py-2" />
  
  <label class="flex items-center gap-2 text-sm"><input type="checkbox" required /> 我已阅读并同意《用户协议》</label>
  <button id="regBtn" class="bg-brand-500 text-white w-full py-2.5 rounded-lg">注册中…</button>
  
  <div class="flex justify-center gap-4 text-2xl pt-2"><span title="微信">💬</span><span title="支付宝">🅰️</span></div>
</form>

<script>
  // 密码强度：长度 + 大小写 + 数字 + 符号
  function updateStrength(){
    const v=document.getElementById('pwd').value;
    let s=0; if(v.length>=8)s++; if(/[a-z]/.test(v)&&/[A-Z]/.test(v))s++;
    if(/\d/.test(v))s++; if(/[^A-Za-z0-9]/.test(v))s++;
    const bar=document.getElementById('strengthBar');
    bar.style.width=(s/4*100)+'%';
    bar.className='block h-full transition-all '+(s<2?'bg-rose-500':s<4?'bg-amber-500':'bg-emerald-500');
  }
</script>
```

> 说明：生产环境验证码按钮做 60s 倒计时禁用；`hx-indicator` 在提交期间自动显示 loading；密码经后端加盐哈希（Argon2/PBKDF2），前端不落明文。

### 17.4 设备注册与信息模块

**路径统一**：`/devices`（列表）→ `/devices/{id}`（详情）→ tab 切换「信息 / 心跳 / 操作 / 日志 / 指令 / 计划 / 任务 / 地图」，操作深度严格 ≤ 3 层。所有数据经 `IRegistry` + 设备 CRUD API（FastEndpoints/Carter）+ MQTT 多段匹配同步（`devices/{continent}/{country}/{province}/{city}/{district}/{line}/{batch}/{deviceId}/{type}`，见 §9、§1.12）。

| 模块    | 内容                                                | 技术落点                  |
| ----- | ------------------------------------------------- | --------------------- |
| 设备信息  | 注册、型号、协议、连接参数（`DeviceOptions`）、`DeviceProfile` 点表 | §1.12 `DeviceCatalog` |
| 心跳    | `HeartbeatAsync` + `HealthCheck` 存活探针，超时判失联       | §1.11 看门狗             |
| 操作    | 下发写命令（CQRS 写命令）→ `IProtocolAdapter.WriteAsync`    | §6                    |
| 日志    | 结构化日志 → Seq/ClickHouse，关联 `correlationId`         | §1.13                 |
| 指令    | 命令队列 → Channel → MQTT → 设备，急停走安全态                 | §9 / G10              |
| 计划/任务 | Hangfire/Coravel 定时轮询、批量操作                        | —                     |
| 地图    | Mapsui 设备地理分布                                     | `Mapsui`              |

---

### 17.5 社媒矩阵数据追踪面板（Social Media Matrix Tracker）

**场景**：电影级、数据密集型社媒分析仪表板，需多平台指标、交互式图表、悬停洞察、范围对比、深浅主题切换。  
**解决方案**：单文件 HTMX + Tailwind 实现；多平台 KPI 卡 + 内联 SVG 组合图（悬停显示明细 tooltip）+ 7/30/90 天范围对比 + 平台筛选 + 亮暗切换。  
**技术要点**：HTMX 负责局部刷新（`hx-get="/api/metrics?range=30" hx-target="#chart"`），离线 Demo 用内联 SVG 重绘；范围/平台切换即向后端请求分平台分周期指标。

> 可运行样板：`samples/PlcAiot.Web/social-matrix-tracker.html`（单文件 HTMX，多平台 KPI、悬停洞察、范围对比、深浅主题）。

```html

<div id="ranges">
  <button hx-get="/api/metrics?range=7"  hx-target="#chart" hx-swap="innerHTML" class="bg-brand-500 text-white">7天</button>
  <button hx-get="/api/metrics?range=30" hx-target="#chart" hx-swap="innerHTML">30天</button>
  <button hx-get="/api/metrics?range=90" hx-target="#chart" hx-swap="innerHTML">90天</button>
</div>
![svg](data:image/svg+xml;base64,PHN2ZyBpZD0iY2hhcnQiIHZpZXdCb3g9IjAgMCA3MDAgMjYwIiB3aWR0aD0iNzAwIiBoZWlnaHQ9IjI2MCI+PC9zdmc+)
```

---

### 17.6 Notion 风格团队仪表板（Live Artifact）

**场景**：团队运营看板，需 KPI、7 天趋势、实时活动流、关联数据库任务表，并通过 Composio 连接 Notion/Slack/Jira 等；要求**打开自动刷新、按需刷新、未绑连接器用模拟数据、可离线**。  
**解决方案**：单页自包含 HTML（无外部依赖），内联 CSS + 原生 JS；顶部「同步状态 / 数据源」标识；`refresh()` 打开即调用 + 按钮按需调用 + `setInterval` 实时流；Composio 连接器目录 chip 点击模拟绑定，未绑定回落模拟数据。  
**技术要点**：零 CDN → 真离线；`data-source` 标识区分模拟/真实；Live Artifact 形式可直接内嵌进对话。

> 可运行样板：`samples/PlcAiot.Web/notion-dashboard.html`（KPI + 7 天 SVG 趋势 + 实时活动流 + 关联任务表 + Composio 连接器目录 + 自动/按需刷新 + 模拟兜底）。

```js
// 打开即刷新 + 按需刷新 + 实时流（未绑 Composio 时用 mock 兜底）
function refresh(){ renderKpis(); renderTrend(); renderTasks(); renderConnectors();
  document.getElementById('src').textContent='数据源：模拟'; }
refresh();                                  // 打开自动刷新
document.getElementById('refresh').onclick = refresh;   // 按需刷新
setInterval(pushAct, 3000);                 // 实时活动流
```

---

### 17.7 多端适配（PC / 桌面 / 移动 + MQTT 客户端）

统一视觉（§17.0）与统一数据（§17.8），三端共用同一套 Razor 组件与 Design Token：

| 端        | 技术                                        | 形态                       | 数据通道                          |
| -------- | ----------------------------------------- | ------------------------ | ----------------------------- |
| **PC 端** | B/S 架构，Blazor **Auto**（Server 优先、WASM 渐进） | 浏览器访问，自动渲染               | SignalR + MQTT over WebSocket |
| **桌面端**  | **Blazor Hybrid + WebView2**              | 桌面应用，内嵌 WebView2         | 本地 MQTT 客户端 + 服务端桥接           |
| **移动端**  | **MAUI Hybrid**                           | iOS/Android 原生壳 + Blazor | MQTT 客户端（离线缓存 + 重连补发）         |

**客户端 MQTT 物联网同步**（PC/桌面/移动统一）：

- 设备端/客户端经 MQTT 消息中间件（`EMQX`，见 §1.8）与服务端同步；主题采用 §17.8 九段多段匹配。
- 移动端弱网：本地消息持久化 + 断线重连 + 离线消息补发（QoS1 + 本地队列），恢复后 `Outbox` 重放（见 §1.7）。
- 订阅示例（客户端按权限订阅自己可见的范围）：

```csharp
// MAUI / Blazor Hybrid 共用：MQTT 客户端订阅本园区遥测
await mqtt.SubscribeAsync("devices/asia/cn/henan/zhengzhou/+/+/#/telemetry");
mqtt.ApplicationMessageReceivedAsync += e =>
{
    var topic = e.ApplicationMessage.Topic;          // 九段主题，服务端已多段匹配
    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
    await InvokeAsync(() => DevicePointReceived(topic, payload)); // 回到 Blazor 渲染
};
```

---

### 17.8 服务端三段匹配与统一布局规范

**三段匹配（服务端路由）**：所有设备/页面通道经 MQTT 九段主题 `devices/{continent}/{country}/{province}/{city}/{district}/{line}/{batch}/{deviceId}/{type}`，服务端按 `type` 段映射处理器并写入对应存储（telemetry→时序库、alarm→告警、command→指令队列、heartbeat→存活、ai→诊断），前端按 `+/#` 通配订阅保证三端一致（详见 §9）。

**统一布局规范（强制）**：

1. **操作简单不超过三层**：导航 → 模块 → 操作；禁止第四级钻取（如「设备→详情→tab→操作」即到顶）。
2. **路径统一**：列表 `/{resource}`、详情 `/{resource}/{id}`、操作 `/{resource}/{id}/{action}`，REST 风格前后端一致。
3. **布局合理**：左侧导航（模块）+ 顶部栏（搜索/用户/主题/通知）固定，内容区卡片化（`.glass rounded-2xl`）；KPI 行 → 图表区 → 列表/流，自上而下信息密度递减。
4. **说明清晰**：每个操作有标签、占位符、错误就近提示；关键卡片附数据来源与时间戳。

### 17.9 边缘数据链路（SQLite + Mosquitto + CommunityToolkit.Datasync）

**场景**：边缘网关/采集代理需在弱网、断网环境下本地落库、本地消息流转，并在恢复连接后把数据增量同步到中心；同时尽量降低边缘侧运维负担。**解决方案**：三层边缘数据链路——① **边缘库采用 SQLite**（EF Core + DapperAOT 读写），单文件 ACID、零运维、适配 ARM 网关与 NativeAOT；② **业务数据采用 Eclipse Mosquitto** 做边缘 MQTT 订阅/发布，并通过桥接（bridge）把本地主题转发到中心 EMQX 集群；③ **数据同步平台采用 CommunityToolkit.Datasync** 做离线优先增量同步与冲突解决（设备注册表、配置、告警等必须跨断网一致的数据）。

**Why SQLite at edge**：单文件、零运维、ACID、支持 WAL 并发读、可在 NativeAOT 下与 `Microsoft.Data.Sqlite` + EF Core 配合；适合边缘缓存遥测、设备注册表、离线消息队列缓冲（见 §0.4 数据库行）。

**Why Mosquitto at edge**：轻量（MB 级内存占用）、可嵌入式运行在网关，承担本地设备与采集代理之间的 pub/sub；通过 `bridge` 配置把 `devices/#` 选择性转发到中心 EMQX，形成「边缘自治 + 中心汇聚」的双层 broker 拓扑。

**Why CommunityToolkit.Datasync**：离线优先（本地 SQLite 为源）、增量拉取/推送、内置冲突处理（自定义 `HttpClient` 中继或 Azure 中继）、与 EF Core `DbContext` 无缝集成，适合设备注册表/配置这类「弱网下也要可用、恢复后自动收敛」的数据。

**关键代码骨架（Mosquitto 桥接 + DataSync 客户端）**：

```conf
# mosquitto.conf —— 边缘 broker：本地 pub/sub + 桥接转发至中心 EMQX
listener 1883
allow_anonymous true

# 桥接：把本地设备主题转发到云端 EMQX
connection emqx-cloud
address emqx.example.com:8883
topic devices/# both 1
bridge_capath /etc/ssl/certs
bridge_certfile /etc/mosquitto/certs/edge.pem
bridge_keyfile  /etc/mosquitto/certs/edge.key
```

```csharp
// 边缘侧：CommunityToolkit.Datasync 离线优先增量同步（设备注册表）
public class DeviceContext : DbContext          // EF Core + SQLite
{
    public DbSet<DeviceEntity> Devices => Set<DeviceEntity>();
    protected override void OnConfiguring(DbContextOptionsBuilder o)
        => o.UseSqlite("Data Source=plc-edge.db");
}

// 离线优先同步：本地 SQLite ⇄ 中心 REST（CommunityToolkit.Datasync）
var client = new DatasyncClient("https://api.plc-iot.example.com/", handler);   // 自定义 HttpClient 中继
var table  = client.GetOfflineTable<DeviceEntity>("devices");
await table.PullAsync("devices");               // 增量拉取
await table.PushAsync();                        // 离线写入回推（冲突自动解决）
```

> 拓扑与配置说明见 `samples/PlcAiot.Web/cockpit.html` 的「系统」页（边缘数据链路配置卡片）；双层 broker（边缘 Mosquitto + 中心 EMQX）详见 §1.8。

---

## 18. miniapi 代码实例（base-files apps）

使用 base-field apps 的 `#` 指令式 miniapi 定义设备模块（FastEndpoints + 设备注册/心跳/指令/日志/计划）：

```
#:sdk Microsoft.NET.Sdk.Web
#:package FastEndpoints@7.2.0-beta.10
#:package Microsoft.VisualStudio.Azure.Containers.Tools.Targets@1.19.6
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property UserSecretsId=210f4926-30c7-45ca-a020-391f82b3b3a1
#:property DockerDefaultTargetOS=Linux
#:property DockerComposeProjectPath=..\docker-compose.dcproj

using FastEndpoints;

var builder = WebApplication.CreateBuilder();
builder.Services.AddFastEndpoints();
builder.Services.AddHealthChecks();
var app = builder.Build();
app.UseFastEndpoints();

// 设备注册
app.MapPost("/devices", ([FromBody] DeviceRegistrationDto dto, IRegistry reg) =>
{
    var device = reg.Create(dto);          // 创建并注册设备
    return Results.Ok(new { device.Id });
});

// 设备心跳
app.MapPost("/devices/{id}/heartbeat", (string id, IRegistry reg) =>
    reg.Resolve(id) is { } d ? (d.HeartbeatAsync(CancellationToken.None), Results.Ok("pong"))
                             : Results.NotFound());

// 下发指令（CQRS 写命令）
app.MapPost("/devices/{id}/command", async (string id, [FromBody] WriteCommand cmd, IMediator m) =>
    await m.Send(new WriteRegisterCommand(id, cmd.Address, cmd.Value)));

// 设备日志查询
app.MapGet("/devices/{id}/logs", (string id, ILogStore store) =>
    Results.Ok(store.Query(id, 100)));

// 设备计划任务注册
app.MapPost("/devices/{id}/plans", ([FromBody] DevicePlan plan, IScheduler sch) =>
    sch.Schedule(plan));

app.MapGet("/health", () => "healthy");
app.Run();
```

---

## 19. PLC 生产级实战知识点（踩坑清单与对策）

> 以下沉淀自 Modbus / OPC UA / S7 / 三菱 MC 等现场高频故障点与 7×24 运行经验，是「能跑」与「能稳定跑」的分水岭。与 §6（协议抽象）、§7（九态机）、§8（Modbus 实现）配套阅读。

### 19.1 Modbus 寄存器标准定义

Modbus 按**功能码前缀 / 地址区**区分四类寄存器，配置 `ModbusRegister` 时必须先判明区：

| 寄存器区                    | 读功能码 | 写功能码        | 地址区   | 读写 | 典型用途           |
| ----------------------- | ---- | ----------- | ----- | -- | -------------- |
| 线圈 Coils                | 0x01 | 0x05 / 0x0F | **0** | 读写 | 离散输出（启停、阀门）    |
| 离散输入 Discrete Inputs    | 0x02 | —           | **1** | 只读 | 外部数字量输入        |
| 输入寄存器 Input Registers   | 0x04 | —           | **3** | 只读 | 仪表/模拟量采集       |
| 保持寄存器 Holding Registers | 0x03 | 0x06 / 0x10 | **4** | 读写 | **最常用**，参数/工艺值 |

- **0 起始 vs 1 起始**：协议本身 0 起始；绝大多数仪表/触摸屏文档从 **1 起始**，代码读取需 `address - 1`（如文档 `40001` → 协议 `0`）。读错数据最常见的根因。
- **32 位值字节序**：32 位浮点/整数占 2 个寄存器（4 字节），**高低字、高低字节常常颠倒**，是现场最高频故障点——必须用 §8.2 的 `ByteOrder`（ABCD/DCBA/DABC…）做重排，禁止手工按固定顺序拼。

### 19.2 Modbus RTU 串口核心（System.IO.Ports）

```csharp
// RTU 串口：SerialPort + NModbus4 ModbusSerialMaster
using System.IO.Ports;

var port = new SerialPort("COM3", baudRate: 9600, Parity.None, dataBits: 8, StopBits.One);
// 485 设备必须共地（GND 连接），否则持续丢包；RS485 半双工注意收发方向(RTS)切换
port.Open();
var master = ModbusSerialMaster.CreateRtu(port);   // 复用同一串口实例（连接池）
ushort[] regs = await master.ReadHoldingRegistersAsync(slaveId, startAddr, 10);
```

- 关键参数：波特率 `BaudRate`、数据位 `DataBits`(8)、校验 `Parity`(None/Even/Odd)、停止位 `StopBits`(1/2)。
- **485 必须共地**：不共地会在现场持续丢包；屏蔽线接地、转换器供电充足才能稳定。

### 19.3 Modbus 实战踩坑清单

| # | 坑                  | 现象                               | 对策                                         |
| - | ------------------ | -------------------------------- | ------------------------------------------ |
| 1 | **地址偏移**           | 文档 1 起始，代码未 -1 → 读错数据            | 统一在 `ModbusRegister.Address` 处做「文档值→协议值」转换 |
| 2 | **浮点高低字节颠倒**       | 国产仪表高低字反转，数值乱跳                   | `ByteOrder` 重排（§8.2/§8.4）                  |
| 3 | **多线程无锁**          | 时间轮轮询 + UI 同时读写 → 报文粘包、数据错乱      | `lock` 或 §0.5.7 `shared_strand` 单设备串行      |
| 4 | **超时未释放连接**        | 断线未 `Dispose` → 端口占用、程序卡死        | `using`/`try-finally` 确保释放；连接池复用           |
| 5 | **单包长度限制**         | 单次最多读 **125** 个寄存器(0x7D)，大批量越界报错 | 按 125 分包；`ReadHoldingRegisters` 批量切分       |
| 6 | **485 串口干扰**       | 未接屏蔽地线/供电不足 → 频繁校验错误             | 共地 + 屏蔽 + 稳压；必要时加终端电阻                      |
| 7 | **Slave 从站 ID 错误** | 多仪表总线混用，站号不匹配 → 无返回              | 配置 `SlaveId` 与设备拨码/软件一致                    |

### 19.4 OPC.Ua.Core 踩坑

- **证书信任失败**：自签名证书未加入信任列表 → 直接拒绝连接；开发可临时 `SecurityPolicy.None`（生产必须上证书）。
- **安全策略不匹配**：服务器开启加密、客户端用 `SecurityPolicy.None` → 无法连接，需两端策略一致。
- **NodeId 格式混淆**：数字 ID / 字符串 ID / 命名空间索引 `ns=` 填写错误 → 读取返回 `Null`。
- **订阅丢失断线**：无心跳保活 → 网络波动后订阅永久失效；需断线重连并**重建 Subscription**。
- **Linux 证书权限**：证书文件夹读写权限不够 → 启动直接报错（目录 `600/700` + 运行用户可写）。
- **高频订阅内存泄漏**：未释放 `Subscription`/`Session` → 7×24 内存持续上涨；订阅用完即 `Dispose`，且复用长连接 Session。

### 19.5 Snap7 / S7.NETPlus 踩坑

- **DB 块无法读取**：1200/1500 程序 DB 属性需取消「优化的块访问」，否则按地址读取全为 0。
- **PLC 防火墙拦截 102 端口**：能 ping 通但连不上 → PLC 程序「保护」中勾选允许 **PUT/GET** 通讯。
- **机架/槽号错误**：1200 机架 0 槽 1；S7-300 机架 0 槽 2；200SMART 特殊路由规则，填错即失败。
- **浮点字节序错乱**：西门子 4 字节浮点**反转存储**，直接转换异常 → 需专用 `ConvertToFloat`（§8.4 思路）。
- **频繁创建 Client 句柄**：短连接不销毁 → 内存泄漏、长时间运行崩溃；**单设备单长连接**复用 `Plc`/`S7Client`。
- **Snap7 x64/x86 库缺失**：发布忘记拷贝 `snap7.dll`/`libsnap7.so` → 找不到入口；随包拷贝并区分位数。
- **多网段路由失败**：跨子网访问需配置 **S7 路由参数**（TSAP/路由器的 S7 路由表）。

### 19.6 三菱 MC 踩坑（Mitsubishi.MC，默认 8193 端口）

- **MC 协议未开启**：以太网模块参数未勾选「MC 二进制通讯」、端口 8193 未开放 → 完全连不上。
- **二进制 / ASCII 模式混淆**：程序发二进制报文、PLC 配 ASCII（或反之）→ 无返回；两端模式必须一致。
- **FX / Q 软元件地址不通用**：Q 系列支持更大地址范围，FX 超出范围直接报错；按机型裁剪地址表。
- **报文粘包并发**：多线程同时收发 → 解析错乱 → **必须加锁**或每设备 `shared_strand` 串行（§0.5.7）。
- **多 CPU 系统 CPU 编号错误**：多 CPU 机架需指定 CPU 号，默认 0 在多数多 CPU 场景无效。
- **32 位数值高低字颠倒**：温度/压力浮点读取乱码 → 统一走字节序转换方法。

### 19.7 通用优化（分层）

| 层         | 做法                                                        |
| --------- | --------------------------------------------------------- |
| 底层通信层     | 封装 Modbus/S7/OPC/MC 四类 Helper，统一接口 `ICommunicationDriver` |
| 业务轮询层     | 后台异步线程轮询，**隔离 UI**，界面不卡顿                                  |
| 断线重连层     | 心跳检测 + 断开自动重试 + **指数退避**                                  |
| 数据模型层     | 统一变量实体，兼容 Modbus/PLC/OPC UA                               |
| 日志层       | 分级日志（通讯异常 / 地址错误），现场可快速排错                                 |
| 同步读写阻塞 UI | 全部 `Task`/`ValueTask` 异步，主线程只渲染                           |
| 并发同步      | 时间轮轮询 + 写入并发 → 报文错乱 → `lock`/`shared_strand`              |
| 资源释放      | `TcpClient`/`Session`/`S7Client` 长期不 `Dispose` → 泄漏       |
| 心跳保活      | 交换机闲置断流需保活，否则程序无法感知离线                                     |
| 异常捕获      | 单条寄存器失败直接崩整个采集 → **单条 `try-catch`**                       |
| 数值转换      | 16/32 位有/无符号、浮点字节序必须兼容处理                                  |
| 重连退避      | 断网无限重连占满 CPU → 延时重试（1s/3s/5s）                             |

### 19.8 性能优化清单（7×24 硬指标）

- **单设备单长连接**：不频繁创建/销毁 Socket，连接池复用底层 TCP/`Plc`。
- **批量读写**：相邻点位一次性读回，减少报文交互（Modbus 受 125 寄存器上限分包）。
- **异步 IO**：用 `async/await`/`ValueTask` 替代同步阻塞，提升并发采集数量。
- **本地内存缓存**：变量缓存到内存，无需每次读 PLC（配合 §0.5.1 HybridCache）。
- **指数退避重连**：1s/3s/5s 退避，降低网络压力与 CPU 占用。
- **多线程隔离**：通讯 / UI / 日志三线程互不阻塞。
- **定时清理**：清理无效连接、释放托管/非托管资源（见 19.4/19.5 的 Session/Client 释放）。
- **依赖版本统一**：NuGet 统一 2026 稳定版，避免新版 API 破坏性改动。
- **发布依赖**：Windows 拷贝 Snap7、串口驱动；Linux 安装 `libserial`/串口库。
- **现场调试**：优先关闭防火墙，排查端口/IP/PLC 通讯权限。
- **浮点转换**：所有 32 位数值一律走配套转换方法，**禁止自行拼接字节**。
- **7×24 三件套**：自动重连 + 资源释放 + 日志持久化，缺一不可。

### 19.9 Stateless 九态机（完整转换关系回顾）

设备状态机完整定义见 **§7.0**（九种状态 + 九种指令 + 转换表 + 异步竞态防护 + UI 更新）。生产落地的额外要点：

- 每个设备**独立状态机实例 + 独立 `shared_strand`**（§0.5.7）：单设备内状态事件串行、设备间并行，无锁无粘包。
- **故障态**必须「故障确认（清报警锁存）→ 重置」才能回到停机，杜绝误动作。
- **急停优先级最高**：任何态收到 `EmergencyStop` 均进入 `EmergencyStop`，只有 `Reset` 可退出。
- 状态变化后**立即**经 `UiDispatcher` 推前端（§7.0.3），三端（PC/桌面/移动）秒级刷新。

---

## 20. 推荐集成框架（含 GitHub / 优缺点 / 理由）

### 20.1 插件 / 热插拔「核心框架」重点对比

| 框架                    | GitHub                         | 定位            | 优点                         | 缺点                        | 推荐理由         |
| --------------------- | ------------------------------ | ------------- | -------------------------- | ------------------------- | ------------ |
| **DotNetCorePlugins** | natemcmaster/DotNetCorePlugins | 基于 ALC 的插件加载器 | 类型共享/隔离精细、热重载、.NET 8+      | 需谨慎 GC；netstandard2.0 不支持 | **首选底座插件引擎** |
| **Oqtane**            | oqtane/oqtane.framework        | 模块化 Blazor 框架 | 动态页面合成、多租户、.NET Foundation | 偏 CMS/Web，体积大             | 模块化 UI 参考    |
| **NetPro**            | LeonKou/NetPro                 | 低侵入可插拔中间件     | 引用即初始化、不强依赖                | 维护偏 6.0                   | 能力组件封装范式参考   |
| **Foundatio**         | FoundatioNet/Foundatio         | 可插拔基础块        | 抽象统一、轻量                    | 不含 ALC 热插拔                | 能力内部可替换基础件   |
| **MEF**（内置）           | dotnet/runtime                 | 组合式发现         | 零依赖、声明式                    | 不支持 collectible 卸载        | 仅静态模块组合      |

**结论**：底座热插拔引擎 = **DotNetCorePlugins + 自研 PluginManager**；模块化 UI 参考 **Oqtane**；基础件可替换性参考 **Foundatio**；中间件封装参考 **NetPro**。

### 20.2 能力框架分类索引（对应你清单 282 项）

| 分类        | 首选                                         | GitHub                                                                       | 覆盖条目                          |
| --------- | ------------------------------------------ | ---------------------------------------------------------------------------- | ----------------------------- |
| 插件/热插拔    | DotNetCorePlugins                          | natemcmaster/DotNetCorePlugins                                               | #81–#85,#280                  |
| API/端点/网关 | FastEndpoints + Carter + **YARP**          | FastEndpoints/FastEndpoints, CarterCommunity/Carter, microsoft/reverse-proxy | #1,#2,#103,#46,#137           |
| 对象映射      | Mapster + Mapperly                         | MapsterMapper/Mapster, Riok/Mapperly                                         | #3,#4                         |
| gRPC/实时   | MagicOnion + protobuf-net + SignalR        | Cysharp/MagicOnion, protobuf-net                                             | #42,#43,#44                   |
| 消息/事件总线   | MassTransit + MQTTnet.EventBus             | MassTransit/MassTransit, dotnet/MQTTnet                                      | #15,#186,#79,#80              |
| 序列化       | MemoryPack + MessagePack                   | Cysharp/MemoryPack, MessagePack-CSharp                                       | #18,#24,#25                   |
| 缓存        | Garnet + FusionCache                       | microsoft/garnet, ZiggyCreatures/FusionCache                                 | #7,#10,#8,#9                  |
| 数据/ORM    | EF Core + Dapper(AOT) + Specification      | ardalis/Specification                                                        | #49–#56,#52                   |
| ID/时间     | Ulid + Snowflake + NodaTime                | ulid-net, Snowflake, NodaTime                                                | #86–#91,#101                  |
| 认证/授权     | OpenIddict + Keycloak + Casbin.Net         | openiddict, keycloak, CasbinNet                                              | #93–#100,#134                 |
| 调度/Job    | Hangfire + Coravel                         | HangfireIO/Hangfire, coravel                                                 | #72–#78                       |
| SIP/流媒体   | SIPSorcery + GB28181 + ZLMRTC              | sipsorcery-org, GB28181/GB28181.Solution                                     | #57–#70,#158–#160             |
| 视觉/OCR    | OpenCvSharp + PaddleSharp + ImageSharp     | shimat/opencvsharp, PaddleSharp                                              | #71,#146–#157,#154            |
| AI/AIGC   | **MEAI** + OllamaSharp + AntSK             | microsoft/semantic-kernel, ollama-sharp                                      | #217–#224                     |
| 网络/Socket | SpanNetty + TouchSocket + MsQuic           | SpanNetty, TouchSocket                                                       | #119–#123,#200–#204           |
| 服务治理      | Dapr + Aspire + Consul + Polly             | dapr, dotnet/aspire, Polly-Contrib                                           | #227–#242,#229,#278           |
| 文档/报表     | QuestPDF + MiniExcel + FastReport          | QuestPDF/QuestPDF, mini-software                                             | #125–#127,#181,#225,#226      |
| 可观测       | OpenTelemetry + Seq + MiniProfiler         | open-telemetry/opentelemetry-dotnet                                          | #27–#34,#279,#110–#112        |
| 桌面/客户端    | Avalonia + Photino + Blazor Hybrid (+MAUI) | AvaloniaUI/Avalonia, tryphotino/photino                                      | #206–#215                     |
| 安全/加解密    | BouncyCastle + SecurityHeaders + Captcha   | bc-csharp, rjmurillo/security-headers                                        | #113–#117,#131–#135,#266–#271 |
| 规则/ETL/爬虫 | NRules + ChoETL + DotnetSpider             | NRules/NRules, DotNetSpider                                                  | #257–#260,#248–#250           |
| 动态编译/脚本   | Natasha + Fody + Rougamo + CS-Script       | dotnetcore/Natasha, Fody, CS-Script                                          | #115–#117,#262–#264           |
| 图表        | **ScottPlot**（优先）/ LiveCharts2 / OxyPlot   | ScottPlot/ScottPlot, beto-rodriguez/LiveCharts2                              | #251–#256                     |

### 20.3 与现有 PLC 驱动的结合点

仓库已有 `Modbus.cs / BACnet.cs / Fatek.cs / Fuji.cs / Melsec.cs / Keyence.cs / Omron.cs / MQTT.cs` 等——天然就是协议插件。把它们各自抽成实现 `IProtocolAdapter` 的插件工程，统一入口注册到 `CapabilityRegistry`，上位机/AIOT 平台通过 `IHostServices` 解析调用，即可实现「新增一种 PLC 协议 = 丢一个 DLL 进插件目录」的热插拔体验。

---

## 21. 优缺点分析与落地路线

### 21.1 优点

- **模块化 / 可扩展**：协议、AI、媒体能力即插即用，新增 PLC 协议零改动宿主。
- **快速交付**：Vertical Slice + 现成能力插件，MVP 周期短。
- **高可用 / 高可靠**：ALC 故障域隔离、事件溯源 + 幂等、Disruptor 抗背压。
- **高性能 / 零分配**：Span/Memory、Pipe、对象池、RingBuffer，热路径低 GC。
- **低部署成本**：模块化单体起步，Docker 一键部署，按需加载。

### 21.2 缺点（与缓解）

- **部署复杂**：微服务/容器化需 K8s 知识 → MVP 先用模块化单体，演进期再切。
- **维护成本**：服务间依赖与通信复杂 → 契约先行（`Abstractions`），用 Dapr 边车降耦。
- **ALC 卸载风险**：卸载不干净会内存泄漏 → 强制 GC 回收观测 + sidecar 兜底。
- **跨 ALC 类型转换坑** → 共享契约程序集 + `PreferSharedTypes`。

### 21.3 落地路线

1. **MVP（0–3 月）**：模块化单体 + DotNetCorePlugins + 现有 PLC 驱动包成协议插件 + SQLite + MQTTnet + OpenTelemetry；工业级流水线（Channel→IAsyncEnumerable→BackgroundService→Dataflow）成型。
2. **增强（3–6 月）**：FusionCache/Garnet、MassTransit CQRS、Hangfire 调度、MEAI 诊断、YARP 网关、ScottPlot 实时图表。
3. **演进（6 月+）**：Dapr 边车 + YARP 切微服务；不可信插件转 sidecar 进程；AOT 关键路径；三端前端（Blazor Auto / Hybrid / MAUI）统一上线。

---

## 22. 质量与测试架构（G11：xUnit + Aspire + Testcontainers）

V5 之前无测试策略（评审 G11）。质量基线 = **契约测试 + 集成测试 + 端到端编排 + 压测**，全部纳入 CI。

### 22.1 xUnit 单元测试（零基础设施，热路径重点）

```csharp
public class FrameParserTests
{
    [Fact] public void 粘包_一次收两帧_拆成两帧()
    {
        var buf = Concat(Frame(0x01), Frame(0x02));      // 两帧拼接
        var frames = ParseAll(buf, out _);               // 零分配 ReadOnlySequence<byte>
        Assert.Equal(2, frames.Count);
    }
    [Fact] public void 坏帧_首字节丢弃_重新搜索帧头()
    {
        var buf = Concat(0xFF, Frame(0x01));             // 前导噪声
        var frames = ParseAll(buf, out var dropped);
        Assert.Equal(1, frames.Count);
        Assert.Equal(1, dropped);                         // 噪声字节被丢弃计数
    }
    [Theory]
    [InlineData("INT16", 100, 0, (short)100)]
    [InlineData("INT16", 0.1, 5, (short)50)]            // Scale=0.1, Offset=5 → 5.0
    public void Modbus_转换_INT16_含缩放偏移(string t, double scale, double offset, short raw)
    {
        var reg = new Register { DataType = t, Scale = scale, Offset = offset };
        var v = ConvertData(new[] { (ushort)raw }, reg);
        Assert.Equal(raw * scale + offset, v);
    }
}

public class DeviceStateMachineTests
{
    [Fact] public void 运行中_急停_进入急停态()
    {
        var m = new DeviceStateMachine();
        m.TryFire(DeviceTrigger.Start);
        m.TryFire(DeviceTrigger.Connected);
        Assert.True(m.TryFire(DeviceTrigger.EmergencyStop));
        Assert.Equal(DeviceState.EmergencyStop, m.State);
    }
    [Fact] public void 暂停中_急停_G10覆盖()   // 验证 G10 修复
    {
        var m = new DeviceStateMachine();
        m.TryFire(DeviceTrigger.Start); m.TryFire(DeviceTrigger.Connected);
        m.TryFire(DeviceTrigger.Pause); m.TryFire(DeviceTrigger.Paused);
        Assert.True(m.TryFire(DeviceTrigger.EmergencyStop));
        Assert.Equal(DeviceState.EmergencyStop, m.State);
    }
}
```

### 22.2 Testcontainers 集成测试（真实依赖：EMQX / PostgreSQL / InfluxDB）

```csharp
public class DeviceE2EFixture : IAsyncLifetime
{
    // EMQX 容器（南向真实 MQTT 代理，验证 ACL/TLS）
    private static readonly TestcontainersContainer _mqtt =
        new TestcontainersBuilder<MqttContainer>().WithImage("emqx/emqx:5").Build();
    // PostgreSQL 容器（业务 + Outbox，验证强一致/幂等）
    private static readonly TestcontainersContainer _pg =
        new TestcontainersBuilder<PostgreSqlContainer>().WithImage("postgres:16").Build();
    // InfluxDB 容器（时序，验证最终一致投影）
    private static readonly TestcontainersContainer _influx =
        new TestcontainersBuilder<InfluxDbContainer>().WithImage("influxdb:2").Build();

    public async Task InitializeAsync()
    { await _mqtt.StartAsync(); await _pg.StartAsync(); await _influx.StartAsync(); }
    public async Task DisposeAsync()
    { await _mqtt.DisposeAsync(); await _pg.DisposeAsync(); await _influx.DisposeAsync(); }
}

[Collection("e2e")]
public class CommandE2ETests : IClassFixture<DeviceE2EFixture>
{
    [Fact] public async Task 下发写指令_业务库与Outbox原子提交_设备回执对账()
    {
        // 启动数据平面（连接 _mqtt / _pg），下发 CommandIssued → 断言 PG 业务行 + Outbox 行 + 设备回执
    }
}
```

### 22.3 Aspire 测试编排（一键拉起整条数据平面做端到端）

```csharp
public class PlatformWireup : IAsyncLifetime
{
    private DistributedApplication? _app;
    public async Task InitializeAsync()
    {
        var builder = DistributedApplication.CreateBuilder();
        builder.AddContainer("emqx", "emqx/emqx:5").WithPortBinding(1883, 1883).WithPortBinding(8883, 8883);
        builder.AddPostgres("pg").WithPgWeb();
        builder.AddInfluxDb("influx");
        builder.AddProject("dataplane", "../src/DataPlane/DataPlane.csproj");
        _app = builder.Build();
        await _app.StartAsync();   // 一键拉起 EMQX+PG+InfluxDB+数据平面，跑契约/混沌/压测
    }
    public async Task DisposeAsync() => await _app!.DisposeAsync();
}
```

### 22.4 质量基线（CI 红线）

| 测试类型 | 工具                   | 触发    | 通过标准                                |
| ---- | -------------------- | ----- | ----------------------------------- |
| 契约测试 | xUnit + 反射           | 每次 PR | 插件实现全部契约接口 + 生命周期（`DisposeAsync`）存在 |
| 单元测试 | xUnit                | 每次 PR | 帧解析/状态机/类型转换 100% 覆盖                |
| 集成测试 | Testcontainers       | 每次 PR | 多库双写一致、Outbox relay、南向 ACL 生效       |
| 混沌测试 | 网络分区模拟（MQTT 断连重连）    | 每夜    | 断线指数退避重连、不丢帧、恢复后追平                  |
| 压测   | k6 + BenchmarkDotNet | 发版前   | 采集端到端 p99 < 200ms（对齐 §1.9 SLO）      |

---

## 23. .NET 11 Preview.7 技术基线与后端工程实践（V8）

> 技术基线统一到 **.NET 11 Preview.7 + C# 14**。本节给出后端工程落地的具体约定，前接 §1.10 四支柱、后接 §22 测试。

### 23.1 .NET 11 Preview.7 关键后端能力

| 能力                                  | 说明                                                         | 后端用途                              |
| ------------------------------------- | ------------------------------------------------------------ | ------------------------------------- |
| **Tiered PGO**                        | 分层 JIT \_profile-guided 优化（.NET 8+ 默认开，.NET 11 Preview.7 更稳） | 长驻进程热路径自动加速                |
| **Native AOT**                        | 整进程 AOT 编译，无 JIT、无 IL（§23.2）                      | 边缘网关小镜像/快启动                 |
| **JSON 源生成**                       | `System.Text.Json` 源生成（`JsonSerializerContext`）         | 零反射、AOT 安全、高吞吐              |
| **`System.Threading.Lock`**           | 值类型轻量锁（.NET 9+）                                      | 替代 `lock(obj)`                      |
| **`TimeProvider`**                    | 可注入时钟（.NET 8+）                                        | 测试可 Mock 时间，避免 `DateTime.Now` |
| **`HybridCache`**                     | L1 内存 + L2 分布式 + 踩踏保护（GA）                         | 设备配置/规则缓存                     |
| **`Microsoft.Extensions.VectorData`** | 向量存储抽象（.NET 11 Preview.7）                            | RAG 质检/诊断（§13）                  |
| **`Microsoft.AspNetCore.OpenApi`**    | 内建 OpenAPI 文档生成                                        | API 自描述（§23.4）                   |
| **`System.Numerics.Tensors`**         | `Tensor<T>` / `SparseTensor<T>`                              | 边缘 ML 推理                          |

### 23.2 Native AOT 宿主与插件（ADR-007）

- **宿主**：对采集/网关节点开启 `<PublishAot>true>`。AOT 下禁用运行时反射、动态加载受限，**插件 ALC 动态加载与 AOT 不兼容**——因此 AOT 仅用于「无插件、纯内置协议」的瘦边缘节点；带插件的热插拔宿主跑在 JIT/trimming 模式（标注 `[AssemblyMetadata("IsTrimmable","true")]` 并逐个验证警告）。
- **trimming 安全**：`PlcPlatform.Abstractions` 全部标注 `InternalsVisibleTo` + 避免反射；第三方库逐一跑 `dotnet publish -r linux-x64 -c Release` 验证 trimming 警告为 0。
- **风险**：AOT 镜像不支持运行时 `Assembly.Load`；插件宿主必须 JIT。两者通过 `hostconfig` 区分，不混用。

### 23.3 可观测性：OpenTelemetry 接线（§1.10.6 / ADR-006）

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t.AddSource("PlcPlatform.*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("MassTransit") /* 或 Wolverine Outbox */)
    .WithMetrics(m => m.AddMeter("PlcPlatform.Metrics")
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation())
    .UseOtlpExporter(); // → Aspire/OTel Collector
// 自定义指标
private static readonly Meter Meter = new("PlcPlatform.Metrics");
private static readonly Counter<int> Dropped = Meter.CreateCounter<int>("frames_dropped_total");
```

### 23.4 API 设计：Minimal API + TypedResults + 版本化 + OpenAPI

```csharp
var v1 = app.MapGroup("/api/v1").WithOpenApi();

v1.MapPost("/devices/{id}/command", async (string id, WriteCmd cmd, IMediator m) =>
{
    await m.Send(new IssueCommand(id, cmd));
    return TypedResults.Accepted(); // 写指令异步下发，立即 202
}).RequirePermission("device:write"); // §1.8 RBAC

v1.MapGet("/devices/{id}/telemetry", async (string id, TelemetryRepo r) =>
    TypedResults.Ok(await r.LatestAsync(id)));
```

- **版本化**：URL `/api/v1` 前缀，破坏性变更升 `v2` 并存；旧版标 `[Obsolete]` 给过渡期。
- **`TypedResults`**：避免 `IActionResult` 装箱，AOT 友好、返回类型明确。

### 23.5 限流与缓存：RateLimiting + HybridCache

```csharp
// 限流：内置 SlidingWindow（保护北向 API 与南向指令）
builder.Services.AddRateLimiter(opt =>
{
    opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetSlidingWindowLimiter(
            ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
            _ => new SlidingWindowRateLimiterOptions { PermitLimit = 200, SegmentsPerWindow = 4, Window = TimeSpan.FromMinutes(1) }));
});

// 缓存：HybridCache（L1 内存 + L2 Redis，踩踏保护）
builder.Services.AddHybridCache()
    .AddSerializer<DeviceConfig>()        // 需 JSON 源生成支持
    .UseSerialization(SystemTextJsonSerializer.Default);

var cfg = await cache.GetOrCreateAsync($"dev:{id}", async _ => await repo.LoadConfigAsync(id),
    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromSeconds(30) });
```

### 23.6 与测试架构衔接（§22）

- §22 的 xUnit/Testcontainers/Aspire 全部以 .NET 11 Preview.7 为目标框架（`net10.0`）；`TimeProvider` 用于确定性时钟测试。
- 限流/缓存逻辑用 `FakeTimeProvider` + `FakeRateLimitCounter` 单测；跨边界 trace 传播用 Aspire 端到端验证。

---

## 24. 关键技术点 .NET 11 Preview.7 实现样例（File-based Apps）

> 本节把 V8/V9 的关键设计落成**可复制即跑的单文件样例**（.NET 11 Preview.7 File-based Apps：`dotnet run x.cs`，无需 `.csproj`/`.sln`）。上一轮拟拆建的「瘦边缘 AOT 宿主 / 标准 JIT 插件宿主」两个独立 csproj 工程，此处以 **File-based App** 形式给出，便于审阅与验证——两个文件均已 `dotnet run` 实测通过。  
> 每个样例统一按 **场景 → 解决方案 → 技术要点 → 代码** 组织，简单清晰、不堆砌。

### 24.1 .NET 11 Preview.7 File-based Apps 是什么

- **单文件直跑**：一个 `.cs` 文件用顶层语句（top-level statements）书写，`dotnet run x.cs` 由 SDK 按需编译运行，不需要 `.csproj`/`.sln`，适合文档示例与一次性验证。
- **引用包**：需要 NuGet 包时用 `#r "nuget:PackageId,Version"`；跨文件复用类型用 `global using`。
- **AOT 场景**：`dotnet publish x.cs -c Release -r linux-x64 /p:PublishAot=true`（源码保持单文件，仅发布时由最小 csproj 包裹）。
- **注意**：顶层语句必须位于所有类型声明**之前**（CS8803）；类型声明紧随其后，可被顶层语句前向引用。

### 24.2 瘦边缘 AOT 宿主（File-based App）

- **场景**：边缘网关上的轻量采集宿主，要求「小镜像、快启动、低 GC 停顿、7×24 不假死」。
- **解决方案**：NativeAOT 单文件 + 常驻采集循环 + 关键批次 `NoGCRegion` 零停顿 + `CancellationToken` 优雅停机。
- **技术要点**：`System.Threading.Lock`（AOT/trimming 安全轻量锁）、`GC.TryStartNoGCRegion`（临界区零停顿，须与 `EndNoGCRegion` 配对）、顶层语句即程序入口、顶层 `await` 即 `async Main`；`Ctrl+C`/SIGTERM 触发优雅停机。

```csharp
// edge-aot-host.cs — .NET 11 Preview.7 File-based App（瘦边缘 AOT 宿主样例）
// 运行：  dotnet run edge-aot-host.cs
// 发布 AOT： dotnet publish edge-aot-host.cs -c Release -r linux-x64 /p:PublishAot=true
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
```

### 24.3 标准 JIT 插件宿主（File-based App）

- **场景**：中心/边缘服务器的插件化宿主，需运行时热插拔协议驱动，且「卸载要干净」（§3.5）。
- **解决方案**：`IPlugin:IAsyncDisposable` 生命周期契约 + `PluginManager` 有序注册/启动/**反向卸载**。
- **技术要点**：`IAsyncDisposable` 保证释放连接/定时器/Channel；**反向卸载**（后注册先停）避免依赖悬空；真实环境插件来自 DLL（`McMaster.NETCore.Plugins` / `AssemblyLoadContext`，**契约不变**，见 §1.12 / §3.5）；`Ctrl+C` 触发 `DisposeAsync` 有序清理。

```csharp
// plugin-host.cs — .NET 11 Preview.7 File-based App（标准 JIT 插件宿主样例）
// 运行： dotnet run plugin-host.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// —— 顶层语句（程序入口，必须位于类型声明之前）——
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var mgr = new PluginManager();
mgr.Register(new ModbusPlugin());
await mgr.StartAllAsync(cts.Token);
Console.WriteLine("插件宿主运行中（Ctrl+C 卸载）…");
try { await Task.Delay(Timeout.Infinite, cts.Token); }
catch (OperationCanceledException) { }
await mgr.DisposeAsync();   // 触发有序清理契约

// —— 类型声明（位于顶层语句之后）——
public interface IPlugin : IAsyncDisposable
{
    string Name { get; }
    Task StartAsync(CancellationToken ct);
}

// 示例插件：真实场景由 DLL 经 ALC 加载，此处用进程内实现演示生命周期
public sealed class ModbusPlugin : IPlugin
{
    private readonly Timer _heartbeat;
    public string Name => "Modbus";
    public ModbusPlugin() => _heartbeat = new Timer(_ => Console.WriteLine("[Modbus] 心跳"), null, 0, 1000);
    public Task StartAsync(CancellationToken ct) { Console.WriteLine("[Modbus] 已启动"); return Task.CompletedTask; }
    public async ValueTask DisposeAsync()        // 清理契约：释放定时器与连接
    {
        Console.WriteLine("[Modbus] 释放定时器与连接…");
        await _heartbeat.DisposeAsync();
    }
}

// 宿主：负责插件的注册、启动、以及「有序卸载」
public sealed class PluginManager : IAsyncDisposable
{
    private readonly List<IPlugin> _plugins = new();
    public void Register(IPlugin p) => _plugins.Add(p);
    public async Task StartAllAsync(CancellationToken ct)
    {
        foreach (var p in _plugins) await p.StartAsync(ct);
    }
    public async ValueTask DisposeAsync()
    {
        // 反向卸载：后注册先停，避免被依赖方先消失导致悬空
        for (int i = _plugins.Count - 1; i >= 0; i--)
            await _plugins[i].DisposeAsync();
        _plugins.Clear();
        Console.WriteLine("[Host] 所有插件已有序卸载");
    }
}
```

### 24.4 场景化关键技术点（长稳 · 互换 · 故障）

> 以下三节对应 V9 三大支柱，按「场景 → 解决方案 → 技术要点 → 代码」给出**最决策相关**的 .NET 片段（完整生产代码见 `samples/PlcAiot.Stability/`）。

#### 24.4.1 长稳：监督者指数退避 + 重启预算（防 flapping 雪崩）

- **场景**：某设备会话偶发崩溃，若「崩溃→立刻重启→又崩溃」循环，会拖垮整线并刷爆告警。
- **解决方案**：每次崩溃指数退避（带抖动防惊群）重启；设 `MaxRestarts` 重启预算，连续超预算则停止重启、进入安全态并升级告警。
- **技术要点**：每设备独立 `Task`（故障域隔离）；`Backoff` 指数封顶 + 随机抖动；预算耗尽转 `EnterSafeStateAsync()`（急停相关不盲目重试，见 §7.0 G10）。

```csharp
// DeviceSessionSupervisor 重启核心（节选自 §1.11.2 / PlcAiot.Supervisor）
while (!outer.IsCancellationRequested)
{
    try { await RunSessionAsync(outer); return; }            // 正常退出（被停机取消）
    catch (Exception ex) when (!outer.IsCancellationRequested)
    {
        if (++_restarts > _opts.MaxRestarts)                  // 重启预算：防 flapping
        {
            _log.Critical(ex, "设备 {Dev} 重启超预算，升级安全态", _opts.DeviceId);
            await EnterSafeStateAsync();                      // 急停相关走安全态
            break;
        }
        var backoff = Backoff(_restarts);                     // 指数退避 + 抖动
        _log.Warning(ex, "设备 {Dev} 会话崩溃，{Ms}ms 后第 {N} 次重启",
                     _opts.DeviceId, backoff.TotalMilliseconds, _restarts);
        await Task.Delay(backoff, outer);
    }
}

static TimeSpan Backoff(int n) =>
    TimeSpan.FromSeconds(Math.Min(300, Math.Pow(2, n)))       // 1,2,4,8… 封顶 5min
    .Add(TimeSpan.FromMilliseconds(Random.Shared.Next(0, 500))); // 抖动防惊群
```

#### 24.4.2 互换：换 PLC / 换协议 = 改 Profile（零代码改动）

- **场景**：现场从西门子 S7 换成三菱，或从 TCP 换 RTU，上层业务/UI 不应改一行代码。
- **解决方案**：设备「寄存器/类型/字节序/轮询」外置为 `DeviceProfile`（JSON）；换型只改 Profile 文件；`DeviceCatalog` 支持运行时热替换（TCP↔RTU 不改点表）。
- **技术要点**：`DeviceProfile` 是纯数据（record），与协议实现解耦；`DeviceCatalog.SwitchTransport` 仅切换连接参数；通用点表经 `IPointTableExchange` 导入导出（CSV / 借鉴 OPC-UA Nodeset）。

```csharp
// DeviceProfile（节选自 §1.12.1 / PlcAiot.Abstractions）
public sealed record DeviceProfile
{
    public string DeviceId { get; init; } = "";
    public string Protocol { get; init; } = "Modbus";   // Modbus/S7/OpcUa/Melsec…
    public string Transport { get; init; } = "Tcp";      // Tcp/Rtu/Udp
    public DeviceEndpoint Endpoint { get; init; } = new();
    public DeviceEndianness Endianness { get; init; } = DeviceEndianness.Big;
    public int PollingMs { get; init; } = 1000;
    public IReadOnlyList<RegisterPoint> Points { get; init; } = [];
}

// DeviceCatalog 热替换（节选自 §1.12.4）
public void SwitchTransport(string deviceId, string transport)   // TCP↔RTU 不改点表
{
    var p = _profiles[deviceId];
    _profiles[deviceId] = p with { Transport = transport };      // record with 表达式
    _log.Info("设备 {Dev} 传输切换为 {T}", deviceId, transport);
}
```

#### 24.4.3 故障：全链路可追溯 + 自动恢复 + 多维查询

- **场景**：设备掉线后，运维要能「一键查出这次故障从哪来到哪去、系统自动做了什么、现在谁在处理、历史同类故障怎么解决的」。
- **解决方案**：`FaultEvent` 带 `CorrelationId` 全链路关联 + `Fingerprint` 去重；`IRecoveryStrategy` 按故障码映射自动恢复（重连/重初始化/故障转移/急停走安全态）；`IFaultStore` + `FaultQuery` 多维查询 + 冷热分层。
- **技术要点**：`CorrelationId` 串联 OTel 三信号与 §1.8 审计闭环；`Fingerprint`（设备+故障码+时段哈希）合并同类抖动，避免告警风暴；恢复失败指数升级人工；查询落库责任人/根因/处置记录。

```csharp
// FaultEvent（节选自 §1.13.1 / PlcAiot.Abstractions）
public sealed record FaultEvent
{
    public string FaultId { get; init; } = Guid.NewGuid().ToString("N");
    public string DeviceId { get; init; } = "";
    public string Code { get; init; } = "";               // COMM_TIMEOUT / WRITE_FAIL…
    public FaultSeverity Severity { get; init; }
    public string CorrelationId { get; init; } = "";       // 全链路关联
    public string Fingerprint => $"{DeviceId}:{Code}";    // 同类故障去重键
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public FaultStatus Status { get; set; } = FaultStatus.Open;
}

// 按故障码映射恢复策略 + 多维查询（节选自 §1.13.3 / §1.13.4）
var store = new InMemoryFaultStore();
var registry = new RecoveryRegistry
{
    { "COMM_TIMEOUT", new ReconnectStrategy() },          // 自动重连
    { "SESSION_STALE", new ReinitSessionStrategy() },     // 重初始化会话
    { "DEVICE_DOWN", new FailoverStrategy() },            // 故障转移
    { "SAFETY_TRIP", new SafeStateStrategy() },           // 急停走安全态
};
var q = new FaultQuery { DeviceId = "PLC-A1", Severity = FaultSeverity.Critical };
var hits = await store.QueryAsync(q);                     // 设备/时间/级别/码/状态/指纹多维
```

---

## 附录 A：零分配「协议帧解析」完整骨架（Pipe + Dataflow + ArrayPool）

```csharp
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks.Dataflow;

var parseBlock = new TransformBlock<ReadOnlyMemory<byte>, Frame>(
    mem => ParseFrame(mem.Span),
    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4, BoundedCapacity = 1024 });

static Frame ParseFrame(ReadOnlySpan<byte> span)
{
    var pool = ArrayPool<byte>.Shared;   // 仅在必须落盘时 Rent
    return new Frame(span[0], span.Slice(1, 4).ToArray()); // 示意
}
```

## 附录 B：关键依赖版本建议（.NET 11 Preview.7 基线）

- `McMaster.NETCore.Plugins` ≥ 2.0（建议以 .NET 11 Preview.7 为目标重新编译；`net10.0` 框架）
- `Microsoft.Extensions.*` 与 SDK 同版本
- 序列化优先 `MemoryPack/MessagePack`；网络优先 `SpanNetty`；缓存优先 `Garnet`/`FusionCache`
- 图表优先 `ScottPlot`；网关 `YARP`；状态机 `Stateless`；AI `Microsoft.Extensions.AI`

---

*本 V9 合并版以「契约隔离 + ALC 热插拔 + 零分配热路径 + 工业级生产流水线」为骨架，整合底座/能力/入口/出口、协议抽象、Modbus 详细实现、AIOT 传输、CQRS、YARP 网关、实时图表、前端产品体系与 miniapi 实例；并经 V7 评审加固（G1–G11/F1–F5）、V8 后端加固（四支柱：可扩展/高性能/插件式/可靠 + OpenTelemetry 可观测 + .NET 11 Preview.7 基座），以及 **V9 长期运行稳定性加固**——**① 长时间运行稳定性与高可用（§1.11）**：监督者+看门狗自愈、长期运行 GC（DATAS）、连接租约防泄漏、过载降级、HA（PG 咨询锁选主 + 边缘主备）、崩溃恢复；**② 协议与 PLC 设备可扩展与互换（§1.12）**：`DeviceProfile` 点表 + `IDeviceProtocol` 统一契约 + 能力协商 + `DeviceCatalog` 热替换；**③ 故障全生命周期管理（§1.13）**：`FaultEvent` 全链路可追溯 + `IRecoveryStrategy` 自动化恢复 + 解决状态机 + `IFaultStore` 多维查询。技术基线统一为 .NET 11 Preview.7 + C# 14。落地时优先把现有 PLC 驱动转为协议插件并补 `DeviceProfile`，即可在最短时间内验证底座价值。*
