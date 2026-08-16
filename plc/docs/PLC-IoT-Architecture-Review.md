# PLC IoT / AIOT 平台 — 架构评审与方向指引（Architect Review）

> 评审对象：`PLC-IoT-Architecture-Complete.md`（V5）、`architecture-merged.svg`（V5）
> 评审人：软件架构师（SoftwareArchitect）
> 日期：2026-08-15
> 定位：站在全局高度，给出「是否可扩展、是否高可用」的独立判断，并指明技术团队下一步方向。
> 方法：ADR + 风险分级 + 优先级路线图。每个结论都给出**取舍（trade-off）**，而非只报喜。

---

## 0. 一句话结论

文档**技术广度与工程细节已达生产级草案水准**（282 框架目录、零分配解析、九态状态机、三平面、WAF/网关、Modbus 实战踩坑），是一份**优秀的「技术选型手册」**。
但它当前是 **"技术栈清单 + 实现骨架"**，还不是 **"架构"**——**缺领域模型、缺一致性模型、缺南向安全、缺插件生命周期防泄漏契约**。
**判定：可作为 MVP 起点，但进入「增强/演进」前必须先补 G1–G4 四项（见 §3），否则会在 7×24 运行中爆雷。**

---

## 1. 设计总评（做对了什么）

| 维度 | 评价 | 说明 |
| --- | --- | --- |
| 演进路径 | ✅ 优 | Modular Monolith 起步 → 插件化 → 微服务演进，路径清晰、可逆（见 §0.3/落地路线） |
| 热插拔底座 | ✅ 优 | DotNetCorePlugins + 自研 PluginManager + `AssemblyDependencyResolver`，故障域隔离思路正确 |
| 零分配/高性能 | ✅ 优 | Span/Memory/Pipe/Dataflow/ObjectPool/ArrayPool 贯穿热路径，且有生产级骨架 |
| 协议抽象 | ✅ 优 | `IProtocolAdapter/IDriver/IDevice/IDataPipeline/IRegistry` 四层 + 统一帧格式，天然支持「丢 DLL 即上新协议」 |
| 生产级 PLC 常识 | ✅ 优 | §19 踩坑清单（字节序、粘包、485 共地、Slave ID、浮点颠倒、重连退避）是全文最值钱的部分 |
| 内置优先 | ✅ 优 | §0.5 强制 `Microsoft.Extensions.*` + MIT + AOT 友好，规避了「第三方依赖失控」风险 |
| 三平面 | ✅ 良 | 管理/控制/数据分离 + 南北向，方向正确；但**控制平面自治边界未落地**（见 G5） |
| 状态机 | ✅ 良 | 九态九指令 + `lock`+`CanFire` 守护 + `OnTransitioned` 推 UI，正确；但 E-Stop 覆盖不全（见 G10） |

---

## 2. 必须修正的事实性错误（P0，建议本周内改）

| # | 位置 | 错误 | 正确 |
| --- | --- | --- | --- |
| F1 | §20 表 `对象映射` | `Riokapp/Mapperly` | **`Riok/Mapperly`**（github.com/riok/mapperly，已核实） |
| F2 | §15 代码注释 | `// 显式开启 WAF（默认开启）` | **Aneiang.Yarp 的 WAF 默认关闭**，需 Dashboard 开启或 `Waf:Enabled:true`；注释应改为「默认关闭，需显式开启」 |
| F3 | §20 子节编号 | 标题 `## 20. 推荐集成框架`，但子节是 `### 19.1/19.2/19.3` | 子节应改为 `### 20.1/20.2/20.3` |
| F4 | §21 子节编号 | 标题 `## 21. 优缺点分析`，但子节是 `### 20.1/20.2/20.3` | 子节应改为 `### 21.1/21.2/21.3` |
| F5 | §15 依赖 | 只列 `Aneiang.Yarp` | 实际需 `Aneiang.Yarp` + `Aneiang.Yarp.Dashboard`（WAF/Dashboard 来自 Dashboard 包）；WAF 中间件需在 `app.UseRouting()` 后、`MapReverseProxy()` 前 |

> **关于「网关 WAL」的措辞澄清**：Aneiang.Yarp 官方称其为「配置变更审计（环形缓冲 `ConcurrentQueue` 上限 200）+ 原子文件持久化（`tmp`+`File.Move`）」，不是数据库意义上的 WAL。文档的「WAL」是我们对其审计+原子落盘的**类比表述**，建议在文中加一句澄清，避免团队误以为是事务日志。

> **其余 §20 表 GitHub 链接建议全量复核一遍**（如 `ZiggyCreatures/FusionCache`、`beto-rodriguez/LiveCharts2`、`ScottPlot/ScottPlot`、`protobuf-net`、`Cysharp/MagicOnion` 均正确；仅 `Riokapp` 这一处确认错误）。

---

## 3. 架构级风险与缺口（按严重度分级）

> 严重度：🔴 高（不补会在 7×24 生产爆雷）｜🟠 中（演进期必爆）｜🟡 低（健壮性/可维护性）

### 🔴 G1 — 缺领域模型 / 边界上下文（Bounded Context）
文档是 **"技术栈优先"**，几乎没有领域建模。一个大型 PLC/AIOT 平台至少应有以下边界上下文：
`设备资产(Asset)` · `采集遥测(Telemetry)` · `指令控制(Command)` · `规则告警(Rule/Alert)` · `配置(Config)` · `身份(IAM)` · `诊断(Diagnosis)`。
**取舍**：现在不画上下文地图，短期省事；长期必然出现「一个插件改了配置库把遥测也带崩」「命令与告警争抢同一张表」的耦合灾难。
**建议**：补一页 C4 的「容器+上下文」图，明确每个插件归属哪个上下文、跨上下文只允许经事件/API，不允许直连对方数据库。

### 🔴 G2 — 多库双写 / 一致性模型缺失
文档把数据分到 **SQLite / InfluxDB / PostgreSQL / Seq / ClickHouse** 五处，但**未定义一致性模型**：
- 同一笔设备事件若同时写 InfluxDB + PostgreSQL + Seq，**任一处失败即不一致**（分布式无事务）。
- 文档只在 PostgreSQL 用了 Wolverine Outbox，**InfluxDB/Seq 未纳入事务发件箱**，会出现「业务库写了、时序库没写」的脏读。
**取舍**：强一致（同一事务写多库）性能差且多数 DB 不支持；最终一致需接受短暂不一致。
**建议**：显式声明 **一致性基线**——
- 指令/控制（南向写）：**强一致 + 幂等**（命令落 PostgreSQL + Outbox，设备回执对账）。
- 遥测/日志：**最终一致**，经 Outbox → 各库幂等投影（consumer 用 `(deviceId, seqNo)` 去重）；Seq/ClickHouse 视为「可重建的派生读模型」，丢了可重放 Outbox 补。

### 🔴 G3 — 南向（设备侧）安全基线缺失
文档对北向（人/平台）有 WAF/RBAC/限流，**但南向（平台⇄PLC）几乎无安全设计**：
- MQTT/OPC 接入的**设备身份认证**（每设备凭证？mTLS？）、**传输加密（TLS）**、**证书轮换**、**指令鉴权（谁有权下发写指令）** 未定义。
- 这是 **OT/ICS 合规红线**（等保/IEC 62443）。§1.5 自己也写「配置错误即操作物理设备」——那南向必须是安全重点，而非北向附庸。
**建议**：补「南向安全基线」专节：每设备 X.509/预共享密钥 + EMQX ACL（按 `devices/{id}/+` 主题授权）+ 写指令需 `RequirePermission` + 全量操作审计入库。

### 🔴 G4 — 插件卸载与事件总线/定时器的泄漏契约缺失
§3 讲了 ALC 加载，§3.4 讲了 `AssemblyDependencyResolver`，但**未定义插件卸载时的清理契约**：
- 若插件在 `IEventBus` 订阅了事件、起了 `Timer`/`BackgroundService`/`Channel` reader，卸载 ALC 时这些**仍被事件总线/调度器持有引用 → 插件程序集无法卸载（内存泄漏）+ 旧 handler 继续触发（幽灵处理）**。
- 这是 ALC 热插拔最经典的生产事故。
**取舍**：要求每个插件实现 `IDisposable/IAsyncDisposable` 会增加插件作者负担；但不要求则必然泄漏。
**建议**：在 `IPlugin` 契约增加 `ValueTask DisposeAsync()`；`PluginManager.Unload()` 流程强制：**先反注册事件订阅 → 停 Timer/HostedService → 排空 Channel → 再 `Unload()` → 强制 `GC.Collect(2)` 两次观测**。并在文档给「插件必须清理清单」。

### 🟠 G5 — 控制平面自治边界未落地
§1.5 画了三平面，代码里管理平面 `Publish(DeviceConfigChanged)` 后在**同进程**被 `DeviceConfigChangedHandler` 处理。但演进目标是「控制平面成为独立部署」。
**问题**：现在控制平面与数据平面同进程同内存，未来拆分时要重写大量代码。
**建议**：**现在**就把控制平面实现为独立 `BackgroundService`/独立程序集，经 `IEventBus`（进程内用 Channel，未来换 MassTransit/Redis 即切分布式），数据平面只消费规则对象。这样「单体 ⇄ 微服务」是配置切换而非重写。

### 🟠 G6 — 背压双重闸门可能冲突
§5 说「背压只在一处设闸门」，但 §9 又用 **Redis `mq:depth` 闸门** + Channels `BoundedChannel` 闸门，等于**两道闸门**。
**风险**：Redis 闸门在 Channel 之外，逻辑上生产者先查 Redis 再写 Channel，二者状态可能不一致（Redis 说满、Channel 空），导致 producer 空等或 consumer 背压失效；且引入 Redis 往返增加延迟。
**建议**：明确**唯一背压真相源**——要么纯 Channel（`BoundedChannelFullMode.Wait`，零外部依赖，推荐 MVP），要么纯 Redis（跨进程限流，适合多采集节点）。二选一，不要叠加。

### 🟠 G7 — 可观察性跨边界传播缺失
OTel 提到，但**跨 ALC 插件边界、跨 MQTT 消息边界的 trace 上下文传播**未设计：
- 一条「UI 下发写指令 → CQRS → 插件 → 设备回执 → MQTT 上送」的链路，若 traceparent 不随 MQTT `UserProperty` 透传，分布式追踪会断在 MQTT 边界。
**建议**：定义「消息头契约」——所有跨进程消息（MQTT/Outbox/EventBus）必须携带 `traceparent` + `deviceId` + `correlationId`；插件内 `Activity` 从消息头恢复。

### 🟠 G8 — 时间与乱序（7×24 时序硬伤）
时序数据（InfluxDB）依赖**统一时钟**。边缘节点若未 NTP 同步，或设备离线补传导致乱序，聚合会错。
**建议**：文档补「时间基线」——边缘节点 NTP 强制同步 + 遥测带设备本地时间戳与采集序号 + 服务端做乱序水线（watermark）处理。

### 🟠 G9 — 插件契约版本演进策略缺失
`Abstractions` 一旦发版，旧插件 DLL（基于旧契约编译）丢进新宿主会 **`MissingMethodException`/类型不匹配**。
**建议**：契约加 `[AssemblyVersion]` 语义化 + 插件 `PluginManifest.MinHostVersion`；宿主加载时校验，不兼容拒绝加载并告警，而非崩溃。

### 🟡 G10 — 急停（E-Stop）覆盖不全
§7.0 状态机中 `Pausing` 态**不能接收 `EmergencyStop`**，且 `EmergencyStop` 态**不能重入**。对安全关键平台，急停应能从**任何态**（含 Pausing）触发，且可幂等重入。
**建议**：给 `Pausing` 也加 `.Permit(EmergencyStop, EmergencyStop)`；`EmergencyStop` 自环 `.Permit(EmergencyStop, EmergencyStop)`。

### 🟡 G11 — 无测试架构
282 清单提到 xunit/Bogus，但**无测试策略**：插件契约测试、网络分区/断线混沌测试、插件卸载泄漏测试、背压/过载压测均未设计。
**建议**：补「质量基线」——插件需过契约测试（接口签名/生命周期）；CI 跑网络分区混沌测试（模拟 MQTT 断连重连）；发布前跑 k6/BenchmarkDotNet 压测取 p99。

### 🟡 G12 — 无量化 SLO
文档未给任何量化目标（管理多少设备、每秒多少点位、p99 延迟、可用性 99.9%？）。选型（Disruptor vs Channels、InfluxDB 分片）缺乏依据。
**建议**：先定 SLO（如 1 万设备、50 万点/秒、采集端到端 p99<200ms、可用性 99.9%），再反推架构参数。

### 🟡 G13 — 南向下发规则灰度/回滚只有提及无设计
§1.5 说「审批/灰度」，但无机制：规则版本化、灰度设备组、异常自动回滚。
**建议**：规则对象带 `Version` + `CanaryDeviceIds`；下发后监控该组异常率，超阈值自动回滚到上一版本。

---

## 4. 关键架构决策记录（建议团队 ratify 的 ADR）

> 采用 ADR 模板：Status / Context / Decision / Consequences。以下为草案，需团队评审签字。

### ADR-001：单体起步、插件化、演进微服务
- **Context**：团队规模与边界尚不清晰，过早微服务成本高。
- **Decision**：MVP 用 Modular Monolith + DotNetCorePlugins；契约先行（`Abstractions` 零实现）。
- **Consequences**：✅ 低部署成本、易调试；❌ 单体故障域大（靠 ALC 隔离缓解）。

### ADR-002：一致性模型（命令强一致 / 遥测最终一致）
- **Context**：五库双写有不一致风险（G2）。
- **Decision**：南向写指令走 PostgreSQL+Outbox 强一致+幂等；遥测/日志经 Outbox 幂等投影到 InfluxDB/Seq/ClickHouse，视为可重建读模型。
- **Consequences**：✅ 不丢业务真相；❌ 遥测有秒级延迟、需实现幂等 consumer。

### ADR-003：插件生命周期清理契约
- **Context**：ALC 卸载 + 事件订阅/定时器持有引用会泄漏（G4）。
- **Decision**：`IPlugin` 必须实现 `IAsyncDisposable`；`PluginManager.Unload()` 先反注册再卸载。
- **Consequences**：✅ 可安全热插拔；❌ 插件作者需遵守清理清单。

### ADR-004：控制平面自治边界（现在就隔离）
- **Context**：未来要拆微服务，现在同进程会重写（G5）。
- **Decision**：控制平面实现为独立 BackgroundService + `IEventBus`，数据平面只消费规则对象。
- **Consequences**：✅ 切微服务=换传输；❌ 当前略增间接层。

### ADR-005：南向安全基线
- **Context**：OT/ICS 合规与「配置错即动设备」风险（G3）。
- **Decision**：每设备凭证 + EMQX ACL + 写指令 `RequirePermission` + 全量审计。
- **Consequences**：✅ 合规可达；❌ 接入复杂度上升、需密钥管理。

---

## 5. 给技术团队的方向与优先级路线图

**当前阶段判断**：V5 已达「MVP 技术蓝图」水平，但**不能直接进演进期**。先把地基打牢。

### 优先级 P0（2 周内，阻塞生产）
1. 修 F1–F5 事实错误。
2. 落地 **G4 插件清理契约**（否则热插拔就是定时炸弹）。
3. 落地 **ADR-002 一致性模型**（Outbox 覆盖所有写库路径）。
4. 落地 **ADR-005 南向安全基线**（等保/IEC 62443 前置）。

### 优先级 P1（MVP 内）
5. 补 **G1 领域/上下文地图**（一页 C4 容器图即可）。
6. 落地 **ADR-004 控制平面隔离**。
7. 定 **G12 SLO 量化指标**，反推架构参数。
8. 解决 **G6 背压单闸门**。

### 优先级 P2（增强/演进期）
9. G7 跨边界 trace 传播、G8 时间/乱序、G9 契约版本、G10 E-Stop 补全。
10. G11 测试架构（契约/混沌/压测）、G13 规则灰度回滚。
11. YARP/Aneiang.Yarp 切微服务（契约不变）。

---

## 6. 对两份交付物的修订清单（可直接执行）

**`PLC-IoT-Architecture-Complete.md`**
- [ ] F1：`Riokapp/Mapperly` → `Riok/Mapperly`
- [ ] F2：§15 WAF 默认关闭说明修正
- [ ] F3/F4：§20/§21 子节编号修正
- [ ] F5：§15 补 `Aneiang.Yarp.Dashboard` 依赖与中间件顺序
- [ ] 新增 §G：南向安全基线（ADR-005）
- [ ] 新增 §：一致性模型与双写对策（ADR-002）
- [ ] 新增 §：插件生命周期清理契约（ADR-003）
- [ ] 新增 §：领域/边界上下文地图（G1）
- [ ] 修正 §7.0：E-Stop 覆盖 Pausing + 自环（G10）
- [ ] 修正 §9：背压去重为单闸门（G6）
- [ ] 新增：量化 SLO 章节（G12）

**`architecture-merged.svg`**
- [ ] 数据平面区加「南向安全基线」徽标（mTLS/ACL/审计）
- [ ] 插件隔离区加「卸载清理：反注册→停 Timer→排空 Channel→GC」注记（G4）
- [ ] 控制平面框标注「独立 BackgroundService / 未来独立部署」（G5）
- [ ] 三平面加「一致性：命令强一致 / 遥测最终一致」注记（G2）
- [ ] 背压处标注「唯一闸门：Channel 或 Redis 二选一」（G6）

---

## 7. 架构师寄语

这份文档最大的价值，是它**把 282 个框架从「清单」变成了「有约束的选型」**（内置优先、MIT、AOT 友好、零分配），这是很多团队做不到的纪律。
但它现在**重「技」轻「域」**——技术决定了一切，业务边界却模糊。

**给团队的三句话方向**：
1. **先画域，再选技**：用一页上下文地图锁住边界，技术栈才不会反客为主。
2. **热插拔的敌人不是加载，是卸载**：G4 不解决，热插拔就是内存泄漏的放大器。
3. **OT 平台，南向即红线**：北向 WAF 再强，南向没凭证/没审计，一行错配就能动真设备。

> 下一步：若认可上述方向，我可**直接落地 P0 修订**（修正 F1–F5 并补充 G4 插件清理契约、ADR-002/005 两节到文档，同步更新 SVG），把 V5 升到「可进 MVP 的 V6」。
