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

---

## 6. 新增层核验（2026-08-26）

> 本次增量覆盖 2026-08-25 待办评估（BACKLOG-ASSESSMENT-2026-08-25.md）中「新层」相关项：  
> **#5** plc-saas 绿构建、**#2/#3** 前端接真实后端、**#1** 容器化交付、**#8** 本报告刷新。  
> 核验方法：构建产物 + 端到端脚本 `scripts/verify-cockpit.sh`（10 项检查，自动规避 Windows 保留端口段）+ 直接读码 `file:line`。

### 6.1 一句话结论

V9 报告（§0）判定的「核心底座仅文档」已被**四块真实落地**改写：多租户 SaaS 后端（`plc-saas`）可绿构建、ADR-113 MQTT/EMQX 摄取桥已编译进该后端、边缘网关经 NamedPipe 被 WebApi 宿主（`plc-host`）以 `/api/gateway/*` 暴露且 e2e 全 PASS、前端（cockpit/devices + 根 SPA `/console/`）已同源接线真实后端。原 V9 的「samples 可运行骨架 + 根驱动占位」结论仍成立（见 §1），**但新增层已把"网关契约 / 数据平面 / 前端接线"从文档推进到可运行**。

### 6.2 plc-saas 多租户 SaaS 后端（#5 · 绿构建）

| 项 | 结果 | 证据 |
| -- | -- | -- |
| 构建（Release） | **0 错误 / 4 警告** | `dotnet build src/plc-saas -c Release` → `已成功生成`；仅 `NU1608`（Npgsql.EF 11.0.0-preview.6 需 EF Core preview.6，实际解析 preview.7，预览包漂移，非阻断） |
| strip 目标（解除潜伏编译阻断） | 已恢复 | `src/plc-saas/plc-saas.csproj` 的 `PrepareStrippedApp` target 重新启用，将 `#include`/`#r` 指令经 `strip-app.ps1`/`.sh` 剥离后编译（prior 段已完成 #5） |

> 说明：NU1608 为 .NET 11 Preview 多预览源版本错配告警；不影响编译与运行。建议后续统一 Npgsql.EF / EF Core 到同一 preview 修订号以消除告警。

### 6.3 ADR-113 真实 MQTT/EMQX 摄取桥（构建验证已解锁）

| 项 | 结果 | 证据 |
| -- | -- | -- |
| 决策状态 | Accepted · V1-now 已落地 | `docs/ADR-113-implementation.md:1,3` |
| 实现代码 | 已编译进 plc-saas（随 #5 绿构建） | `src/plc-saas/slices/ingest.bridge.cs`、`src/plc-saas/slices/ingest.http.cs`（BACKLOG #6 ✅实现） |
| 双摄取路径 | HTTP 主路径 + MQTTnet 边缘中继，统一收敛 `Ingest*Command` | `ADR-113-implementation.md:13-35`（① EMQX 规则引擎→`POST /api/v1/ingest/telemetry|fault`；② `Mqtt:Bridge:Enabled=true` 时 MQTTnet 直订 `tenants/+/devices/+/telemetry`） |
| 构建验证 | 随 #5 解除后已可绿构建 | 见 6.2（plc-saas 0 错误） |

> 端点级实测（起 PG+EMQX）受沙箱 `wslc`/网络限制未跑（BACKLOG #4 🔶），属部署前置冒烟，非代码缺陷。

### 6.4 边缘网关 + WebApi 宿主 `/api/gateway/*` 契约（e2e 全 PASS）

`plc-host` 以 **StreamJsonRpc 经 NamedPipe（`plc-gateway`）** 调边缘网关；`/api/devices/*` 走宿主内置 `IDeviceCatalog`（Tier1 插件路径），`/api/gateway/*` 走边缘网关。

| 端点 | 映射 | 证据（file:line） |
| -- | -- | -- |
| `GET /api/gateway/health` | 网关健康 | `src/plc-host/Modules/GatewayModule.cs:35` |
| `GET /api/gateway/protocols` | `string[]`（如 `builtin.sim`） | `GatewayModule.cs:38` |
| `GET /api/gateway/devices` | `DeviceCfg[]` | `GatewayModule.cs:50` |
| `POST /api/gateway/read` | 读保持寄存器 | `GatewayModule.cs:62` |
| `POST /api/gateway/write` | 写保持寄存器 | `GatewayModule.cs:86` |
| `GET/POST /api/devices/{protocolId}/...` | 宿主内置目录（Tier1 插件） | `src/plc-host/Modules/DeviceProtocolsModule.cs:32,40,60` |
| `GET /` | 重定向 `/console/` | `src/plc-host/Program.cs:27` |

**e2e 实测（`scripts/verify-cockpit.sh`，自动端口，10 项全 PASS，0 失败）：**

| 检查 | 请求 / 期望 | 实测结果 |
| -- | -- | -- |
| protocols | `GET /api/gateway/protocols` | `["builtin.sim"]` ✅ |
| devices | `GET /api/gateway/devices` | 3 台：`dev-modbus-1`(snet.modbus.tcp)、`dev-siemens-1`(snet.siemens.tcp)、`dev-sim-1`(builtin.sim) ✅ |
| read | `POST /api/gateway/read` dev-sim-1 | `{"deviceId":"dev-sim-1","data":"AAEAAg==","bytes":4}` ✅ |
| write | `POST /api/gateway/write` dev-sim-1 values=[11,22] | `{"deviceId":"dev-sim-1","written":2}` ✅ |

> 关键事实：e2e 全程 NamedPipe 通联成功，证明"宿主→网关"控制面可用；此前 Kestrel TCP 绑定偶发 `10013` 系 **Windows Hyper-V/WSL2 保留端口段**所致（非代码缺陷），验证脚本已改为从候选端口自动选首个可绑定端口自愈。

### 6.5 前端同源接线：cockpit/devices + 根 SPA `/console/`

根 SPA 与 cockpit/devices 均采用**同源托管**（无需 CORS / 反向代理），浏览器以相对路径调用 `/api/*`。

| 检查 | 期望 | 实测 | 证据 |
| -- | -- | -- | -- |
| cockpit-wired | cockpit.html 引用 `/api/gateway/devices` | PASS | `src/plc-host/bin/Debug/net11.0/wwwroot/pages/cockpit.html:1128-1129`（`apiGet('/api/gateway/devices')` + `apiGet('/api/gateway/protocols')`） |
| devices-wired | devices.html 引用 `/api/gateway/read` | PASS | e2e `devices-wired` |
| console-root-redirect | `/` → `Location: /console/` | PASS | `Program.cs:27` + e2e |
| console-index | `/console/` 含 `js/app.js` | PASS | e2e `console-index` |
| console-appjs-wired | `/console/js/app.js` 含 `/api/gateway/devices` | PASS | `wwwroot/js/app.js:82-83`（`Promise.all([axios.get("/api/gateway/devices"), axios.get("/api/gateway/protocols")])`） |
| console-devices-fragment | `/console/pages/devices.html` 含 `data-page="devices"` | PASS | `wwwroot/pages/devices.html:1` + e2e `console-devices-fragment` |

**根 SPA 分页**：`app.js` 以 `PAGE_SIZE=12` + `devPage` 状态机实现设备列表分页（`wwwroot/js/app.js:74-75, 279-322`），与 cockpit 同源方案对齐。社媒 / Notion / 多租户后端视图显式空态降级（`app.js:1080-1088`），避免未接后端时白屏。

### 6.6 容器化交付（#1 · Dockerfile）

| 交付物 | 内容 | 状态 |
| -- | -- | -- |
| `Dockerfile`（根） | 多阶段：构建 `plc-host` + `plc-gateway`（`src/plc-host`、`src/plc-gateway` 相对路径引用 `../PlcAiot.Web/wwwroot/pages`、`../../wwwroot`），运行阶段非 root `app` 用户，`EXPOSE 8080`，`ENTRYPOINT ["/app/docker-entrypoint.sh"]` | 已落盘（2026-08-26） |
| `Dockerfile.plc-saas` | 多阶段：`dotnet publish plc-saas.csproj -c Release`，运行阶段非 root，依赖外部 PG（`ConnectionStrings__Default` 注入），`EXPOSE 8080` | 已落盘 |
| `docker-entrypoint.sh` | 先起 `PlcGateway.dll`（NamedPipe），待就绪后起 `plc-host.dll`，trap 清理 | 已落盘 |
| 可复现 | `ARG DOTNET_REGISTRY` / `DOTNET_VERSION=11.0-preview` 支持受限网络镜像覆盖 | 支持 |

> 说明：当前环境无法实跑 `docker build`（无 Docker 守护）；Dockerfile 已按 .NET 11 多阶段最佳实践编写并人工复核，构建验证留作 CI 前置。

### 6.7 新增层核验总表

| 新增层 | 判定 | 关键证据 |
| -- | -- | -- |
| plc-saas 绿构建（#5） | ✅ | `dotnet build` 0 错误 / 4 警告 |
| ADR-113 MQTT/EMQX 桥（构建验证） | ✅ | 随 plc-saas 绿构建编译进后端 |
| `/api/gateway/*` 契约 | ✅ | e2e protocols/devices/read/write 全 PASS + `GatewayModule.cs` |
| cockpit/devices 同源接线 | ✅ | e2e cockpit-wired / devices-wired + `cockpit.html:1128-1129` |
| 根 SPA `/console/` 同源托管 | ✅ | e2e console-* 4 项全 PASS + `app.js:82-83` |
| 容器化交付（#1） | ✅(落盘) / ⏳(docker build 待 CI) | `Dockerfile` / `Dockerfile.plc-saas` / `docker-entrypoint.sh` |

### 6.8 遗留约束 / 风险

1. **预览包告警**：plc-saas `NU1608`（EF Core vs Npgsql.EF 版本漂移）—— 建议统一 preview 修订号。
2. **部署前置冒烟**：PG+EMQX 端到端（`ADR-113` HTTP/MQTTnet 双路径、`/api/v1/ingest/*` 端点）需 `wslc`+网络，沙箱未跑，留作部署验证。
3. **Windows 保留端口**：本地 e2e 若强制 `PORT=` 落于 Hyper-V/WSL2 保留段会 `10013`；验证脚本已自愈，部署到 Linux 容器无此问题。
4. **Docker 构建验证**：无 Docker 守护，Dockerfile 正确性依赖人工复核，建议接入 CI `docker build` 门禁。

---

## 5.1 关联文档（新增层）

- 新增层待办评估：`docs/BACKLOG-ASSESSMENT-2026-08-25.md`
- ADR-113 摄取桥：`docs/ADR-113-implementation.md`
- 容器化交付：`Dockerfile`（plc-host）、`Dockerfile.plc-saas`、`docker-entrypoint.sh`
- 端到端验证脚本：`scripts/verify-cockpit.sh`（15 项检查，自动端口；含 Tier1 插件体系）

---

## 7. ADR-118 插件化功能验证（2026-09-02）

- **结论**：P3 PLC 插件化（Tier1 ALC）已落地并可运行。7 个真实设备协议插件经 `PluginManager`（ALC 隔离）加载并自注册到宿主 `DeviceCatalog`，`/api/devices/{protocolId}/read|write` 端到端可路由到具体协议驱动。
- **修复点（构建/部署接线）**：原 `src/plc-host/plc-host.csproj` 仅以 `<None>` 引用插件 `.csproj`，既不构建插件、也不复制插件 DLL 到运行时 `plugins/`，致 `PluginManager` 扫描不到任何插件、目录恒空（`/api/devices/*` 不可用）。改为在 `plc-host.csproj` 增加 7 个 `ProjectReference(ReferenceOutputAssembly=false, Private=false)`（保持 ALC 隔离、不进宿主依赖图）+ `AfterBuild CopyPluginAssemblies` 目标，把 `plugins/<Name>/bin/<Name>.dll` 复制到 `$(OutDir)plugins/<Name>/bin/`。
- **插件清单（协议 id → 工程）**：`modbus.tcp`→DemoDevicePlugin（ModbusTcpDevicePlugin）、`melsec.mc`→MelsecDevicePlugin、`fatek`→FatekDevicePlugin、`fuji.sph`→FujiDevicePlugin、`keyence.kv`→KeyenceDevicePlugin、`lora.wan`→LoRaDevicePlugin、`bacnet.ip`→BacnetDevicePlugin。均为真实实现（含帧编解码），非空壳。
- **关键证据（file:line）**：`src/plc-host/PluginManager.cs:34`（ALC 扫描 + 热重载）、`src/plc-host/Program.cs:48-49`（启动期 `DiscoverAndLoad`+`StartAllAsync`）、`src/Plc.Plugins.Contracts/IDeviceProtocol.cs:48`（契约）、`src/plc-host/plugins/MelsecDevicePlugin/MelsecDevicePlugin.cs:19`（真实 Qna-3E 二进制帧编解码）、`src/plc-host/plugins/DemoDevicePlugin/ModbusTcpDevicePlugin.cs:13`（MBAP+PDU，含 `len-1` PDU 读修复）、`src/plc-host/PluginLoadContext.cs:23-34`（共享 `Plc.Plugins.Contracts` 回退默认 ALC，保类型身份）。
- **e2e 实测（`scripts/verify-cockpit.sh`，15 项全 PASS）**：`GET /plugins` 列出 7 插件；`GET /api/devices/protocols` 列出 7 协议（均 `supportsRead/Write=true`，`supportedTypes [3,4,1,2]`）；`POST /api/devices/modbus.tcp/read` 指向死主机 `127.0.0.1:1` 返回非 404（证明路由到已注册插件；连接失败返回 500 属预期）。
- **残留（已解决，2026-09-02）**：原 `plugins/**/*.decompiled.cs` 为反编译 SNET 生产 DLL 参考源码（标记 `<None>`，不参与编译）。2026-09-02 已去壳——51 个 `Snet.*` 重复目录（与 `docs/snet/decompiled/` 逐字节一致）从 `plugins/` 删除，规范副本留存 `docs/snet/decompiled/`；`plc-host.csproj` 移除对应 `<None>` 行。`plugins/` 现仅含 7 个可构建 ALC 插件。详见 §8 与 `src/plc-host/plugins/README.md`。

---

## 8. 协议矩阵收口（2026-09-02）

- **目标矩阵覆盖**：ADR-118 显式目标 = 7 协议（Modbus/Melsec/Fatek/Fuji/Keyence/BACnet/LoRa），均已落地为真实 ALC 插件并经 e2e 验证（15/15 PASS）。**P3 目标矩阵 100% 覆盖，无遗留空壳。**
- **去壳动作**：`plugins/` 下 51 个 `Snet.*` 目录（纯 `.decompiled.cs`、无 `.csproj`、非可构建插件）与 `docs/snet/decompiled/` 逐字节一致，作为重复副本删除；`plc-host.csproj` 同步移除 `<None Include="plugins\**\*.decompiled.cs" />`。
- **扩展候选矩阵**：SNET 参考集 41+ 驱动（Siemens/Omron/Allen-Bradley/Beckhoff/Mitsubishi/…）为未来协议迁移参考源，规范语料 `docs/snet/decompiled/`，集成蓝图 `docs/snet/RECOVERY_AND_INTEGRATION.md`；不属 ADR-118 P3 范围。
- **交付物**：`src/plc-host/plugins/README.md`（插件开发指南 + 协议矩阵）、`docs/decisions/ADR-118-plc-protocol-pluginification.md §6`（收口判定）、本 §8。
- **构建验证（已跑）**：`dotnet build src/plc-host -c Debug` → **0 警告 / 0 错误**，8 个项目全部编译（Plc.Plugins.Contracts + 7 插件 + plc-host），7 个插件 DLL 正确落到运行时 `bin/Debug/net11.0/plugins/<Name>/bin/`。
- **e2e 复验（已跑）**：`scripts/verify-cockpit.sh` **15 项全 PASS、0 失败**。关键证据：`POST /api/devices/modbus.tcp/read` 调用栈穿透至 `DemoDevicePlugin.ModbusTcpSession.EnsureConnectedAsync → ModbusTcpSession.ReadAsync → DeviceProtocolsModule.cs:51-52`，返回 500 而非 404（死主机 `127.0.0.1:1` 的 `SocketException 10061` 属预期），证明请求确实被路由到已注册插件。
- **注**：本轮曾因批量 `git rm -r` 被 SIGTERM 中断，导致 `plc-host.csproj` 的 ADR-118 接线修复（7 个 `ProjectReference` + `CopyPluginAssemblies`）这一**未提交**改动丢失；已按原方案重建，并由上述构建 + e2e 重新验证通过。

---

## 9. ADR-113 EMQX 端到端实测：可行性探测与交付（2026-09-11）

> **结论：本环境无法实跑，已转为交付「可执行脚本 + 手册」，待具备 Docker 守护的机器执行后回填结果。**

### 9.1 环境探测（实测）

| 探测项 | 结果 |
|--------|------|
| `wslc` / `wsl` / `docker` / `docker-compose` | 均已安装（`wslc` 在 `C:\Program Files\WSL\wslc`） |
| Docker Desktop 守护 | **未运行** —— `failed to connect to the docker API at npipe:////./pipe/dockerDesktopLinuxEngine` |
| `wsl.exe` | **被沙箱安全策略列入程序黑名单**，禁止启动（且不得绕过） |
| 端口 1883 / 8883 / 18083 / 5432 | 全部未开放（EMQX、PG 均未起） |
| `mosquitto_pub` / `nc` / `psql` | 未安装（故脚本改用 EMQX REST `/api/v5/publish` 发布） |
| MQTTnet | 已引用：`src/plc-saas/plc-saas.csproj:37`（v5.2.0.1603） |
| 基础设施定义 | `src/plc-saas/docker-compose.yml`（PG16 + EMQX 5.8.5）、`src/plc-saas/start-dev-env.ps1`（wslc + `docker.1ms.run` 镜像源，PG18 + EMQX 6.1.4） |

阻塞根因：**无 Docker 守护 + `wsl.exe` 被禁** → 无法拉起 PG/EMQX 容器 → 真实双路径联调不可行。

### 9.2 交付物（已落盘）

- `scripts/verify-emqx-e2e.sh` —— 自动化实测脚本，覆盖：
  - 路径①（主）：`POST /api/v1/ingest/telemetry` / `ingest/fault`（`X-Tenant-Id` + `X-Ingest-Key`）
  - 负向：错误 Key→401、未知租户→404
  - 路径②（边缘）：经 EMQX REST `/api/v5/publish` 发到 `tenants/<tid>/devices/<dev>/telemetry`
  - 持久化断言：PG `devices`（自动纳管）/ `fault_events` / `outbox_messages` / `usage_meters.telemetry_points`
- `docs/ADR-113-emqx-e2e-runbook.md` —— 手册：两套起服务方式（wslc / compose，含**镜像与凭据不一致**提示）、plc-saas 启动的环境变量覆盖（**必须覆盖 `ConnectionStrings__Default`，默认是 `Integrated Security=True` 连容器会失败**；路径②需 `Mqtt__Bridge__Enabled=true`）、断言清单、EMQX 规则引擎 SQL 与 Webhook 配置、故障排查表。

### 9.3 已知不一致（待统一，已记入手册）

1. `start-dev-env.ps1`（wslc，PG18/postgres、EMQX 6.1.4）与 `docker-compose.yml`（docker，PG16/plcsaas、EMQX 5.8.5）**镜像版本与 PG 凭据不同**，混用会踩坑。
2. `src/plc-saas/appsettings.json` 连接串为 `Integrated Security=True`，与容器 PG（用户名/密码）不匹配，必须环境变量覆盖。

### 9.4 判定

| 项 | 判定 |
|----|------|
| ADR-113 双路径代码落地 | ✅ 已随 plc-saas 绿构建编译通过（此前结论） |
| EMQX/PG 真实联调 | ⏳ **未执行** —— 环境阻塞（无 Docker 守护 + wsl 被禁）；脚本与手册已就绪 |
| EMQX 规则引擎主路径 | ⏳ 待实测（配置步骤见手册 §4） |
