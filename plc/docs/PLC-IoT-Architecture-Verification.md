# PLC-IoT 总体架构（V9）实现核验报告

> 核验对象：`docs/PLC-IoT-Architecture-Complete.md`（V9 长期运行稳定性版 · 标称 .NET 11 基线）  
> 核验范围：仓库 `E:/WorkSpace/windgodsnowdrink/vsa/plc`（不含 `plc-saas/` SaaS 新层）  
> 核验方法：逐条声明对照真实代码（直接读码 + 全仓 grep + Explore 子代理交叉复核），每项给 `file:line` 证据  
> 核验日期：2026-08-16

---

## 0. 一句话结论

V9 文档是一份**详尽且高质量的设计蓝图**，但把它描绘成"生产级全栈落地"与仓库实际存在**显著偏差**：

- **真实可运行的部分集中在 `samples/PlcAiot.Stability/`**（领域骨架 + 故障子系统 + File-based 宿主样例）和 `samples/PlcAiot.Web/`（前端 mockup）。
- **仓库根目录的协议/能力驱动大多是空壳占位**（`Modbus.cs`/`MQTT.cs`/`AI.cs` 等仅一个空构造函数），少量真实实现（Melsec/LoRa/IoT/Panasonic 等）走的是另一套 `DriverBase`，**未接入文档宣称的统一契约 `IDeviceProtocol`**。
- **核心底座（PluginManager + ALC 热插拔、CQRS Mediator + Wolverine Outbox、YARP/WAF、MEAI+MCP+Qdrant、真实 MQTT/EMQX 客户端）在仓库内不存在**，仅文档描述或指向仓库外代码。

> 重要结构事实：根目录 `.cs` 文件顶部带 `#::sdk` / `#:package` 指令，说明它们**本身就是 .NET 11 File-based App 文件**（`dotnet run X.cs` 直跑），并非 `Feature.csproj` 编译的模块化单体。这与文档"Modular Monolith + PluginManager + ALC"的底座叙事并不一致——实际是一袋 File-based App 驱动桩，多数未填充。

---

## 1. 逐项核验表

| #  | 文档声明                                                                                                                     | 判定              | 证据（file:line）                                                                                                                                                                                                                                                                                                                                          | 说明                                                                                                                                           |
| -- | ------------------------------------------------------------------------------------------------------------------------ | --------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------- |
| 1  | **协议驱动实现**（Modbus/BACnet/OPC-UA/OPC-DA/Melsec/Siemens/Fatek/Fuji/Keyence/Omron/Panasonic/Schneider/SmartA2/A4/LoRa）      | **部分实现**        | 空壳：`Modbus.cs:49-54`、`MQTT.cs:49-54`、`OpcUa.cs:47`、`BACnet.cs:47`、`Siemens.cs:47`、`Orom.cs:47`、`SmartA2.cs`/`SmartA4.cs`（均 `public X(){...}` 空构造）；真实：`Melsec.cs:77,695`（HslCommunication 真实读写）、`LoRa.cs`（~58KB 真实实现）、`IoT.cs:393,1099,1125`（Serial/Tcp/Udp/Http/DB 多驱动）、`Panasonic.cs:74`、`Schneider.cs:82`、`Keyence.cs:73`、`Fatek.cs:71`、`Fuji.cs:81` | 文档将 Modbus/BACnet/OPC-UA/OPC-DA/Siemens/Omron/SmartA2/A4/MQTT/AI 标为"真实实现"不实；真正的 Melsec/LoRa/IoT 等用 `DriverBase` 体系，**未实现 `IDeviceProtocol`** |
| 2  | **`IDeviceProtocol` 统一契约 + `DeviceCatalog` 热替换 + `DeviceCapabilities`**                                                  | **部分实现**        | 契约与目录真实：`IDeviceProtocol.cs:7-17`、`DeviceCapabilities.cs`、`DeviceCatalog.cs:17,26-28`；但默认工厂全部映射到 `SimulatedModbusProtocol`（模拟），根目录真实驱动不实现该契约                                                                                                                                                                                                           | 统一契约在 samples 可用，但未能收敛根目录驱动，与 §1.12 目标脱节                                                                                                     |
| 3  | **`DeviceSessionSupervisor` 看门狗/自愈**                                                                                     | **已实现**         | `DeviceSessionSupervisor.cs:63`（重启预算）、`:70-77`（指数退避+抖动）、`:114-134`（心跳看门狗）                                                                                                                                                                                                                                                                              | 完整实现，含双通道背压，质量高                                                                                                                              |
| 4  | **故障全生命周期**（`FaultEvent` + `IFaultStore` 多维查询 + `IRecoveryStrategy` + `ResolutionStateMachine` Open→Ack→Resolved→Closed） | **已实现**         | `FaultEvent.cs`、`IFaultStore.cs:4-8`、`IRecoveryStrategy.cs:7-11`、`ResolutionStateMachine.cs:8-25`、`RecoveryRegistry.cs`、`PostgreSqlFaultStore.cs:13,24-45`（Npgsql 真 SQL）、`InfluxDbFaultStore.cs:14,21`（InfluxDB.Client）                                                                                                                                | 状态机 + PG/InfluxDB 双写均落地，是 samples 中最扎实的部分                                                                                                    |
| 5  | **插件热插拔 `PluginManager` + ALC（DotNetCorePlugins）**                                                                       | **仅文档**         | 仓库内仅 `samples/PlcAiot.Stability/file-based/plugin-host.cs:48-64`（进程内演示，无 ALC）；`PluginLoadContext` 仅出现于 `docs/*.md`，指向仓库外 `vsa/PluginLoadContext.cs`                                                                                                                                                                                                    | 真实 ALC / McMaster.DotNetCorePlugins 引擎不在仓库内                                                                                                  |
| 6  | **CQRS `Mediator`(MIT) 源码生成 + `Wolverine` Outbox/Inbox**                                                                 | **仅文档**         | 旧架构 grep `Mediator`/`Wolverine`/`Outbox` 零命中（仅 `plc-saas/` 自研 `OutboxMessages`，属 SaaS 新层，本次不评）                                                                                                                                                                                                                                                         | 旧单体无 Mediator / 源生成 / Outbox 代码                                                                                                              |
| 7  | **MQTT + EMQX 传输**                                                                                                       | **仅文档**         | `MQTT.cs:49-54`（空壳 `class MQTT{}`）；旧架构无 MQTTnet 客户端                                                                                                                                                                                                                                                                                                    | 仅文件名占位，无客户端实现                                                                                                                                |
| 8  | **数据库分层**（SQLite 边缘 / InfluxDB 时序 / PostgreSQL 业务 / Seq+ClickHouse 日志）                                                   | **部分实现**        | 真实：`PostgreSqlFaultStore.cs`、`InfluxDbFaultStore.cs`；仅文档/种子：`DNS.cs:110`（SQLite 注释）、`SeedData.json`（Seq）、docs（ClickHouse）                                                                                                                                                                                                                              | PG + InfluxDB 在 samples 故障子系统落地；SQLite/Seq/ClickHouse 无客户端代码                                                                                 |
| 9  | **前端产物 `samples/PlcAiot.Web/`**（landing / cockpit 8页SPA+Three.js / social-matrix / notion-dashboard）                     | **已实现（mockup）** | `samples/PlcAiot.Web/landing.html`、`cockpit.html:338,575`（含 Three.js）、`social-matrix-tracker.html`、`notion-dashboard.html`                                                                                                                                                                                                                             | 四文件均存在，为含 Three.js 的实景 mockup（种子数据，未接后端）                                                                                                     |
| 10 | **§24 File-based App 样例**（edge-aot-host / 标准 JIT 插件宿主，"已 dotnet run 实测通过"）                                               | **已实现（文件存在）**   | `samples/PlcAiot.Stability/file-based/edge-aot-host.cs:1,51`（NoGCRegion AOT）、`file-based/plugin-host.cs`                                                                                                                                                                                                                                               | 两文件真实存在且为合法 File-based App；"实测通过"无法从仓库验证                                                                                                     |
| 11 | **YARP 网关 / WAF**                                                                                                        | **仅文档**         | 仅 `docs/*.svg` 与 `PLC-IoT-Architecture-Complete.md:2157+`；仓库无 `AddReverseProxy` / 相关配置                                                                                                                                                                                                                                                                 | 无网关代码                                                                                                                                        |
| 12 | **AI（`Microsoft.Extensions.AI` + MCP + Qdrant）**                                                                         | **仅文档**         | `AI.cs:49-54`（空壳 `class AI{}`）；旧架构无 `IChatClient` / `QdrantClient` / `MCP`                                                                                                                                                                                                                                                                             | 仅占位                                                                                                                                          |

---

## 2. 落地占比与关键缺口

**已落地 ≈ 40%**（集中在 `samples/` 演示层）：

- ✅ 领域统一契约（`IDeviceProtocol`/`DeviceCatalog`/`DeviceCapabilities`）
- ✅ 看门狗自愈（`DeviceSessionSupervisor`）
- ✅ 故障全生命周期（FaultEvent/IFaultStore/IRecoveryStrategy/ResolutionStateMachine + PG/InfluxDB 双写）
- ✅ File-based 宿主样例（edge-aot-host / plugin-host）
- ✅ 前端 mockup（4 份 HTML + Three.js 驾驶舱）
- ✅ 部分真实协议驱动（Melsec/LoRa/IoT/Panasonic/Schneider/Keyence/Fatek/Fuji）

**最关键的"文档声称但代码缺失"三项**：

1. **协议驱动大面积空壳** —— 文档将 Modbus/BACnet/OPC-UA/OPC-DA/Siemens/Omron/SmartA2/A4/MQTT/AI 列为"真实实现"，实际均为 52 行空壳；真实驱动未实现 `IDeviceProtocol`，与 §1.12 统一契约脱节。
2. **插件热插拔引擎未实现** —— `AssemblyLoadContext` / `PluginLoadContext`(DotNetCorePlugins) 只存在于文档并指向仓库外，仓库内仅进程内演示。
3. **CQRS/Outbox、MQTT/EMQX 客户端、YARP/WAF、MEAI+MCP+Qdrant、SQLite/Seq/ClickHouse** 全部"仅文档"，根目录对应 `.cs` 是空壳或不存在。

---

## 3. 版本漂移（次要）

文档标称 **.NET 10 LTS / C# 13**；仓库实际 `Feature.csproj` 与根 `.cs` 均 `TargetFramework=net11.0` / `LangVersion=preview`（.NET 11 Preview）。文档应在头部标注"实际基线已演进至 .NET 11 Preview"，避免读者按 .NET 10 API 编写。

---

## 4. 建议（供决策）

| 优先级 | 建议                                                                                  | 理由                              |
| --- | ----------------------------------------------------------------------------------- | ------------------------------- |
| P0  | 文档头部加"实现状态声明"：明确标注 samples 为可运行骨架、根驱动多为占位、底座（PluginManager/ALC/CQRS/MQTT/YARP/AI）待建 | 避免后续读者把蓝图当已实现，踩坑                |
| P1  | 把根目录真实驱动（Melsec/LoRa/IoT 等 `DriverBase`）收敛到 `IDeviceProtocol`，消除"两套协议体系"分裂          | 兑现 §1.12 统一契约，新增 PLC=新增 Profile |
| P1  | 填充空壳驱动（Modbus/BACnet/OPC/MQTT/AI）或显式标注"规划中"                                         | 文档与代码一致性的最大缺口                   |
| P2  | 决定是否落地 PluginManager+ALC / Mediator+Outbox / YARP —— 这些是大单体→微服务演进的底座，需专项立项          | 文档已设计，但未排期                      |

---

## 5. 关联文档

- 新建 SaaS 多租户架构：`docs/saas-architecture.svg` + `docs/SaaS-Architecture-Spec.md`（已回写 Phase 3 状态 / ADR-108）
- 文档分层索引：`docs/README.md`
