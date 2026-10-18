# 规范：PLC·VSA 微单体（net11 + minimal + VSA · 前/后端/移动端分离）

## 问题陈述
当前仓库 `plc/src` 共 36 个 csproj 工程、1 个 36 项目解决方案 `plc-saas.slnx`，存在以下痛点：
1. **架构分裂严重**：同一业务领域（租户、身份、计费、故障、设备、摄取、AI、S8 项集成）跨 SaaS/Web/WebApi/Desktop/Mobile/Shared/Abstractions/Devices/Faults/Supervisor/DemoHost/plc-host/plc-vsa-demo 等多个工程互相 ProjectReference；
2. **TFM 不统一**：net10.0 / net10.0-preview / net11.0 / net11.0-windows / net4.5.2 并存，restore 慢、CI 失败点多；
3. **前端/桌面/移动重复**：PlcAiot.Web、PlcAiot.Desktop、PlcAiot.Mobile 三份独立 pages/*.html + app.css + MainLayout/Routes 重复拷贝；
4. **后端重复起 ASP.NET Core 主机**：PlcAiot.Web（Razor SSR+WASM）、PlcAiot.WebApi（Minimal API）、plc-saas（File-based multi-tenant）、plc-vsa-demo（8 项集成 HTTP）、plc-host（模块插件化主机）各自一套 DI/启动流程、端点路由重叠；
5. **多余 spike/重复**：csgo-spike src + CsGo-master 双份拷贝、csharpflink-spike、dapr-spike、verify、DemoHost、Stability.file-based 等非主路径验证工程。

用户说：**"将文件合并为一个net11+minimal+VSA架构的微单体项目，前/后端/移动端分离，减少架构复杂度，并暂存保存到本地dev分支。"**
即：产出一个真正可构建、可启动、把上面提到的重复代码**真合并**的微单体，并把结果 `git stash` 暂存（或 stash bundle）到本地 dev 分支可随时恢复。

## 用户与目标
| 用户 | 目标 |
|---|---|
| 开发者（Windows 本机） | 主构建目标 4 项目（Server/Frontend/Mobile/Contracts）即可覆盖所有功能，原 36 工程从 Build 配置矩阵移除；一键 `dotnet build slnx` 可在 30s 级完成（不是 3+ 分钟） |
| 反编译流水线维护者 | 不破坏 `plc/docs/scripts/*.ps1`、`plc/sync/*`，同步、反编译、整合脚本都继续可用 |
| 8 项集成验证者 | Disruptor / DotNetCorePlugins / vs-threading / StreamJsonRpc / IoTClient 统一 API / Daq 插件 / File-based 协议持久化 / S7-Modbus-Melsec-Omron 协议 全部仍然在 Server 里有入口，不再需要 `plc-vsa-demo` 单独跑 |
| Git 操作者 | 结果以 `git stash` 暂存，不污染分支历史；也提供 stash 的 bundle 方便 Windows 侧取回 |

## 非目标
- 不物理删除旧工程目录（只是在解决方案 Build 矩阵中排除；保持历史可回滚）；
- 不迁移验证 Spike 工程（csgo-spike、dapr-spike、csharpflink-spike、nativelib、verify、DemoHost、stability.file-based）到微单体；
- 不做真实 PLC 联网调试；
- 不做 MAUI 真实签名打包；
- 不做数据库迁移或真实种子数据执行；
- 不拆 Blazor 到独立 WASM 部署（保留 SSR + 可选交互模式）。

## 功能需求
1. **合并后只暴露 4 个主工程**（都在 `plc/src/PlcVsa/` 下，命名与 `plc-vsa-demo` 对齐）：
   - `PlcVsa.Contracts` — 共享契约；
   - `PlcVsa.Frontend` — 单一 Blazor Razor UI（Web + Desktop 合并）；
   - `PlcVsa.Server` — 单 ASP.NET Core 宿主（Minimal API + VSA 切片）；
   - `PlcVsa.Mobile` — 单一 MAUI Blazor Hybrid 壳（无 maui workloads 时 net11.0 Library typecheck 通过）。
2. **PlcVsa.Server 真实合并能力**：把 plc-saas（21 VSA slice + Db + Auth + SignalR + Mediator + MCP + AI + Billing + Ingest + Metering + Tenants + Identity + Devices + Faults）和 plc-vsa-demo（8 项集成 + RingBuffer + 插件 + RPC + 协议 + 消息记忆 + HTTP 诊断端点）的能力代码**实打实地内联进来**，不是留接口占位。
3. **PlcVsa.Frontend 真实合并页面/样式**：从 PlcAiot.Shared、PlcAiot.Web、PlcAiot.Desktop 三处合并 App.razor / Routes.razor / MainLayout / Home.razor / css / pages HTML，保证不再三份拷贝。
4. **PlcVsa.Mobile 真实引用 Frontend 静态资产**：不再用 `CopyPages` target 从外部 pages/ 拷文件，直接用 Frontend 的 Static Web Asset 机制 + 自包含 wwwroot。
5. **VSA 切片组织**：PlcVsa.Server 目录以 `Slices/<feature>.cs` 为主（兼容 plc-saas 命名）；每个切片文件自含 DTO（record）+ `static Map(..)` 端点注册 + Handler；不保留独立 Controllers 目录（AC：Minimal）。
6. **Minimal API**：Program.cs 仅用 `AddRouting` / `MapGet` / `MapPost` / `MapGroup` / `MapHub` / `MapRazorComponents`；禁止 `AddControllers()` / `MapControllers()`。
7. **snet 扩展点保留**：在 `PlcVsa.Server/Infrastructure/SnetProtocolAdapter.cs` 放 `IDeviceProtocol` 的占位实现（不直接引用反编译产物，保护流水线边界）。
8. **git 暂存**：改造产生的所有文件变更以 `git stash push -m "plc-vsa monolith WIP"` 形式暂存到 dev 分支，并额外把该 stash 导出 `plc/sync/dev-monolith-stash.bundle`（用于 Windows 侧取回）。

## 非功能需求
### Rule（客观可验证）
- NF-R1：4 个新 csproj 的 `TargetFramework` 统一 `net11.0`（Mobile 的 MAUI 多 TFM 仅限条件分支）。
- NF-R2：`PlcVsa.Server.csproj` 的 PackageReference 清单**精确包含**以下包名（版本与已有 vsa-demo / plc-saas / PlcAiot.* 现有版本对齐）：`Disruptor`、`McMaster.NETCore.Plugins`、`Microsoft.VisualStudio.Threading`、`StreamJsonRpc`、`MQTTnet`、`Mediator.Abstractions`、`Mediator.SourceGenerator`、`Microsoft.EntityFrameworkCore.SqlServer`、`Microsoft.AspNetCore.Authentication.JwtBearer`、`Microsoft.AspNetCore.Identity.EntityFrameworkCore`、`ModelContextProtocol`、`ModelContextProtocol.AspNetCore`、`Qdrant.Client`、`System.IdentityModel.Tokens.Jwt`、`Microsoft.Extensions.AI`。
- NF-R3：构建目标项目数 ≤ 4（新主工程），原 36 工程在 slnx Build 配置矩阵中全部 `Build=False`（即 `dotnet build plc-saas.slnx` 只构建 4 个 PlcVsa.* 项目 + 可选旧工程可按需手工构建）。
- NF-R4：PlcVsa.Server `Program.cs` 必须同时注册以下端点（通过字符串匹配证明存在）：`/`、`/health`、`/ring/stats`、`/catalog`、`/memory/{protocol}/{deviceId}`、`/rpc/{deviceId}`、`/api/v1/tenants`、`/api/v1/identity`、`/api/v1/billing`、`/api/v1/devices`、`/api/v1/faults`、`/hubs/faults`、`/mcp`（条件）、`/swagger`。
- NF-R5：PlcVsa.Mobile.csproj 包含 `_CheckForMissingWorkload` override target，在缺 maui-* 工作负载时 typecheck 通过（CS 0 错误）。
- NF-R6：PlcVsa.Server `Slices/` 下 .cs 数量 ≥ 21（对应 plc-saas 原 18 slice + vsa-demo 3 个诊断 slice；数量要求证明真合并不是占位）。
- NF-R7：`PlcVsa.Frontend/wwwroot/pages/` 下存在 10 个 HTML 页面文件与原一致命名，`wwwroot/css/` 存在 3 个 CSS 文件（app.css / plca-colors.css / plca-components.css）。
- NF-R8：`plc/docs/scripts/*.ps1`、`plc/sync/99-sync-and-diff.ps1`、`plc/sync/0[1-5]*.ps1` 都未被修改（git 未追踪到改动）。
- NF-R9：改造完成后 `git stash list` 最新条目 message 为 `"plc-vsa monolith WIP"`，且 `git status --short` 工作区干净。
- NF-R10：`plc/sync/dev-monolith-stash.bundle` 存在且 `git bundle verify` 通过。

### Rubric（评估维度，0-2 分，每分阈值见 spec：≥4/6 总得分方可通过）
- NF-U1 **架构简化（0-2）**：
  - 2 = 新构建目标 ≤ 4；依赖链路 ≤ 3 层（Contracts → Frontend/Mobile → Server）；
  - 1 = 新构建 5-6 或链路 4 层；
  - 0 = 仍超过 7 个构建目标。
- NF-U2 **VSA 契合度（0-2）**：
  - 2 = 21+ 切片都自含 DTO+Map+Handler；无 Controllers 目录；
  - 1 = Controllers 目录存在但全部代码打上 `[Obsolete]` 或 Minimal API 全部路由已覆盖；
  - 0 = 以 Controllers 为主。
- NF-U3 **合并忠实度（0-2）**：
  - 2 = Slices/里每个文件引用的命名空间/类名与 plc-saas 原 slice 一一对应（类名一致），且 DisruptorEngine 中 `RING_SIZE=16384`/三消费组配置保留 vsa-demo 原值；
  - 1 = 有 ≥ 80% 对应但少量（<5 个）类/方法更名；
  - 0 = 大量类名/结构丢失。

## 约束与依赖
- 不物理删除/移动原 36 工程任何文件（沙箱权限有限 + 保持历史）。
- 沙箱缺少 .NET SDK，构建验证在 Windows 端；本 spec 通过静态代码校对 + slnx/sln Build 配置矩阵规则证明。
- plc-saas Program.cs 是 File-based 模式（带 `#include`），剥离 `#include` 后的代码才能复制到 PlcVsa.Server/Slices/*；使用 `strip-app.ps1` 流程等价展开逻辑（将 include 的 infra/slices 内容直接复制过来）。

## 假设
1. "暂存保存到本地dev分支" = git stash 推到 stash 栈 + 生成 stash bundle 供 Windows 侧 `git stash apply/pull stash`。
2. "前/后端/移动端分离" = 三个独立的 .csproj（PlcVsa.Frontend / Server / Mobile），不是不同 git 仓库。
3. 用户之前表示 `plc-saas.slnx` 是统一解决方案：**直接复用 `plc-saas.slnx` 作为唯一解决方案并更新**，不新建。
4. 用户要求"把文件合并为一个…微单体项目，前/后端/移动端分离"= 虽名为"一个"但需有分层概念（Contract/前端/后端/移动）= 4 项目实际就是用户要的"前/后端/移动端分离"。

## 开放问题（Spec Mode 过程中默认按假设处理；后续可改）
1. 旧 36 工程的物理保留位置：假设仍在原目录，不移动。如果用户要求移到 `plc/src/_legacy/` 可以单独再 commit。
2. PlcAiot.Desktop 的 WinForms 3D/WPF 组件：假设暂时不合并入 Frontend（因依赖 `net11.0-windows` TFM），留在 legacy 组。
3. plc-host/plugins/* 的 7 个 csproj：假设 Server 的 Plugins 能力代码通过 `Infrastructure/PluginLoaderHost.cs` 兼容路径加载，不要求构建这 7 个子项目。

## 验收标准
| ID | 类型 | 条款（与前文 Rule/Rubric 完全对齐） |
|---|---|---|
| AC-R1 | Rule | NF-R1（4 项目 TFM net11.0） |
| AC-R2 | Rule | NF-R2（Server 包引用清单 15 项） |
| AC-R3 | Rule | NF-R3（Build 矩阵 ≤4 项目） |
| AC-R4 | Rule | NF-R4（13 个端点静态注册存在） |
| AC-R5 | Rule | NF-R5（Mobile _CheckForMissingWorkload override） |
| AC-R6 | Rule | NF-R6（Slices .cs ≥ 21 个） |
| AC-R7 | Rule | NF-R7（Frontend 10 HTML + 3 CSS 文件） |
| AC-R8 | Rule | NF-R8（反编译脚本与 sync 脚本未修改） |
| AC-R9 | Rule | NF-R9（git stash 最新条目 message=plc-vsa monolith WIP + 工作区干净） |
| AC-R10 | Rule | NF-R10（stash bundle verify 通过） |
| AC-U1 | Rubric (0-2) | NF-U1（架构简化） |
| AC-U2 | Rubric (0-2) | NF-U2（VSA 契合度） |
| AC-U3 | Rubric (0-2) | NF-U3（合并忠实度） |
