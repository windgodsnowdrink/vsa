# 实施计划：PLC·VSA 微单体（PlcVsa 4 项目 + 暂存）

所有任务依赖严格顺序：1 → 2 → 3 → 4 → 5 → 6。每个任务结束时执行该任务 TR 校验并在 "Completion Evidence" 下写结果。

---

## Task 1：目录骨架 + PlcVsa.Contracts
**依赖**：无。**优先级**：高。
**覆盖 AC**：R1、R2（间接依赖前置）。

创建：
```
plc/src/PlcVsa/Directory.Build.props
plc/src/PlcVsa/PlcVsa.Contracts/
  PlcVsa.Contracts.csproj           ← Sdk, net11.0, 0 PackageRef, 无 AspNetCore
  Devices/
    IDeviceProtocol.cs               ← 来自 PlcAiot.Abstractions/Devices + Plc.Plugins.Contracts 合并
    RegisterType.cs                  ← enum/byte, 用于协议
    RegisterPoint.cs
    DeviceEndpoint.cs
    DeviceConnectionOptions.cs
    IIoTClient.cs                    ← 含 Result<T> + IIoTClient + IIoTClientFactory + Disposable 基类
    DeviceCapabilities.cs
    HealthProbeResult.cs
    ByteOrder.cs
    PointSample.cs
    WriteCommand.cs
  Faults/
    FaultSeverity.cs
    FaultCategory.cs
    FaultStatus.cs
    FaultEvent.cs
    FaultQuery.cs
    IFaultStore.cs
    IRecoveryStrategy.cs
  Plugins/
    IPlugin.cs                       ← 来自 Plc.Plugins.Contracts
  Models/
    DeviceTelemetry.cs
    DeviceStatus.cs
    Alert.cs
```
*Implementation note*：直接把旧源码从 `src/PlcAiot.Abstractions/`、`src/Plc.Plugins.Contracts/`、`src/PlcAiot.Shared/Models/` 拷进新命名空间 `PlcVsa.Contracts.*`，去掉对 Microsoft.Extensions.DependencyInjection 的引用（纯 BCL 接口即可）。

**TR**：
- TR-R1a (rule)：`cat PlcVsa.Contracts.csproj` 中 `<TargetFramework>net11.0</TargetFramework>` 存在；无 `<PackageReference Include=` 项（允许 SDK 自动 import 包，但必须为 0 显式）。
- TR-R1b (rule)：`grep -l "interface IDeviceProtocol" PlcVsa.Contracts/Devices/*.cs` 找到文件；grep 该文件存在 `ConnectAsync`、`DisconnectAsync`、`Read<T>`、`Write<T>`、`ProbeHealthAsync`。
- TR-R1c (rule)：`find PlcVsa.Contracts -name "*.cs" | wc -l` ≥ 18。
- Completion Evidence：上述三个命令的输出副本。

---

## Task 2：PlcVsa.Frontend（Razor UI 单项目）
**依赖**：Task 1。**优先级**：高。
**覆盖 AC**：R1、R7。

从 `PlcAiot.Shared/` + `PlcAiot.Web/Components/` + `PlcAiot.Desktop/Components/` 合并：
```
plc/src/PlcVsa/PlcVsa.Frontend/
  PlcVsa.Frontend.csproj            ← Sdk.Razor, net11.0, StaticWebAsset enabled
  _Imports.razor                    ← 合并旧 2 份 _Imports.razor
  Components/
    App.razor                       ← Router，Finalize + Found 与原 Web 相同模式
    Routes.razor                    ← PlcAiot.Web.Components.Routes
    Layout/
      MainLayout.razor              ← 来自 Shared/Components/Layout
    Pages/
      Home.razor                    ← 来自 Shared/Components/Pages/Home.razor
  wwwroot/
    index.html                      ← 来自 PlcAiot.Desktop/wwwroot/index.html (合并 PlcAiot.Web/wwwroot 相同页面)
    css/
      app.css                       ← 来自 Shared/wwwroot/css/app.css
      plca-colors.css               ← 来自 Shared/wwwroot/css/plca-colors.css
      plca-components.css           ← 来自 Shared/wwwroot/css/plca-components.css
    pages/                          ← 10 个 HTML（来自 PlcAiot.Web/wwwroot/pages + PlcAiot.Mobile/wwwroot/pages；取 Web 版本做 baseline）
      admin.html
      alerts.html
      cockpit.html
      devices.html
      index.html
      map.html
      realtime.html
      register.html
      statistics.html
      system.html
```
*Note*：Mobile 后续通过 StaticWebAsset `_content/PlcVsa.Frontend/*` 引用。

**TR**：
- TR-R2a (rule)：csproj 中 `Sdk="Microsoft.NET.Sdk.Razor"` 且 `TargetFramework=net11.0`；`<ProjectReference>` 引用 `..\PlcVsa.Contracts\PlcVsa.Contracts.csproj`。
- TR-R2b (rule)：`find PlcVsa.Frontend/wwwroot/pages -type f | wc -l` = 10 且文件名与上面对齐。
- TR-R2c (rule)：`find PlcVsa.Frontend/wwwroot/css -type f | wc -l` ≥ 3。
- Completion Evidence：文件存在性 grep 输出。

---

## Task 3：PlcVsa.Mobile（MAUI Blazor Hybrid + 缺工作负载兼容）
**依赖**：Task 2。**优先级**：中。
**覆盖 AC**：R1、R5。

基于 PlcAiot.Mobile.csproj 修改并内联必要 platform 代码与 wwwroot→Frontend 资产切换：
```
plc/src/PlcVsa/PlcVsa.Mobile/
  PlcVsa.Mobile.csproj              ← 重写但保留 override target 逻辑
  Properties/
    launchSettings.json             ← 拷贝原 PlcAiot.Mobile/Properties/launchSettings.json
  MauiProgram.cs                    ← 精简，注入 Frontend
  App.xaml / App.xaml.cs            ← 使用 RootPage=MainPage
  MainPage.xaml / MainPage.xaml.cs  ← BlazorWebView HostPage 改为 Frontend 资产路径或 wwwroot/index.html
  Components/
    Routes.razor                    ← 与 Frontend 同步的 Router（或直接引用 Frontend.App）
    _Imports.razor
  Platforms/
    Android/*                       ← 原样拷贝自 PlcAiot.Mobile/Platforms/Android
    MacCatalyst/*                   ← 原样拷贝
    iOS/*                           ← 原样拷贝
    Windows/*                       ← 原样拷贝（含 Package.appxmanifest）
```

**关键**：
- 缺 MAUI 工作负载：`TargetFramework=net11.0`、`OutputType=Library`、`UseMaui=true` 但 `_CheckForMissingWorkload` target 在 Sdk.targets 之后覆盖；
- 有工作负载：`BuildWithMauiWorkloads=true` 切换到 `net11.0-ios;net11.0-maccatalyst;net11.0-windows10.0.19041.0` + `Exe`。

**TR**：
- TR-R3a (rule)：csproj 中 `<Target Name="_CheckForMissingWorkload" Condition="false" />` 在 `</Import>` Sdk.targets 之后。
- TR-R3b (rule)：默认 TFM=net11.0、默认 OutputType=Library；`'$(BuildWithMauiWorkloads)'=='true'` 分支 TFM 含 ios/maccatalyst/windows10.0.19041.0 且 OutputType=Exe。
- TR-R3c (rule)：ProjectReference 仅 `..\PlcVsa.Frontend\PlcVsa.Frontend.csproj`。
- Completion Evidence：三部分 XML 的 cat/grep 输出。

---

## Task 4：PlcVsa.Server（核心合并，最关键）
**依赖**：Task 1 + 2。**优先级**：最高。
**覆盖 AC**：R1、R2、R4、R6、U2、U3。

### 4.1 csproj 与基础文件
```
plc/src/PlcVsa/PlcVsa.Server/
  PlcVsa.Server.csproj              ← Sdk.Web, net11.0
  appsettings.json                  ← plc-saas/appsettings.json + PlcAiot.Web/appsettings* 合并
  appsettings.Development.json
  Program.cs                        ← CompositionRoot
  GlobalUsings.cs                   ← plc-saas/GlobalUsings.cs
  Entities.cs                       ← plc-saas/entities.cs
  Infra.cs                          ← plc-saas/infra.cs（AddSaasDb / Auth / Mediator / SignalR / RateLimiter / Ingest / AI 扩展方法）
  sql/rls.sql                       ← plc-saas/sql/rls.sql
```
*csproj 必须精确含 15 包名*（NF-R2）。

### 4.2 Infrastructure（横切基建 + 8 项集成）
```
Infrastructure/
  CompositionRoot.cs                ← Program.cs 的长 DI 注册拆到此处可选
  DisruptorEngine.cs                ← 来自 plc-vsa-demo.cs PlcVsaEngine：RingBuffer 16384 slots + 3 parallel 消费组 (log/memory/plugin) + UnpublishedEventScope 写事件
  PluginLoadContext.cs              ← 来自 vsa-demo 自定义 ALC
  PluginLoaderHost.cs               ← 来自 vsa-demo HostedService：加载 plugins 目录协议插件
  ProtocolMemoryStore.cs            ← 来自 vsa-demo：持久化协议帧到 protocol-memory/*.dat（append-only）
  BusProducer.cs / RingEvent.cs     ← vsa-demo Disruptor 事件模型
  SnetProtocolAdapter.cs            ← 空壳占位：实现 IDeviceProtocol，注释说明未来装配反编译 DLL 的入口
  IoTClientFactory.cs               ← vsa-demo 中的工厂
  DeviceCatalog.cs                  ← vsa-demo DeviceCatalog（含 Siemens/Modbus/Melsec/Omron 4 内置模拟协议）
  SaasSchemaBootstrap.cs            ← plc-saas infra.cs
  DbSeeder.cs                       ← plc-saas seed.cs
  JsonRpcHost.cs                    ← vsa-demo StreamJsonRpc Host（端口/内部管道暴露）
Hubs/
  FaultsHub.cs                      ← plc-saas faults.stream SignalR Hub
```

### 4.3 VSA Slices（21+ 个文件，真实内联 + Strip #include）
每个切片文件都从 `plc-saas/slices/xxx.cs` 原始内容**剥离 `#include` 依赖**后直接复制，再手动补必要的 using 指令；并为每个加 `static void Map(IEndpointRouteBuilder api)`：
```
Slices/
  Tenant.Register.cs
  Tenant.Activate.cs
  Identity.Login.cs
  Identity.Members.cs
  Identity.Mqtt.cs
  Billing.Usage.cs
  Billing.Plan.cs
  Billing.Quota.cs
  Devices.List.cs
  Faults.Ack.cs
  Faults.Stream.cs
  Ops.Tenants.cs
  Ops.Pricing.cs
  Ops.Roles.cs
  Ops.Health.cs
  Metering.Agent.cs
  Ingest.Bridge.cs
  Ingest.Http.cs
  Ai.Assistant.cs
  Vsa.RingStats.cs                  ← 来自 vsa-demo /ring/stats /catalog
  Vsa.DeviceMemory.cs               ← 来自 vsa-demo /memory/{protocol}/{deviceId}
  Vsa.Rpc.cs                        ← 来自 vsa-demo /rpc/{deviceId}
```

### 4.4 Program.cs（CompositionRoot）必须：
1. `var builder = WebApplication.CreateBuilder(args);`
2. Add* 顺序：`Infra.cs 扩展` → `Add vs-threading JoinableTaskContext/Factory` → `Add DisruptorEngine Singleton` → `Add DeviceCatalog` → `Add IoTClientFactory` → `Add PluginLoaderHost HostedService` → `Add MemoryCache` → `Add RazorComponents<App>` + `Add InteractiveServerComponents` → `Add OpenApi / SwaggerGen / Scalar` → `Add Antiforgery` → `Add CORS ("saas-dev")` → `Add HostedService<MeteringAgent>`；
3. Build 后 `UseRouting → UseCors → UseAuthentication → UseTenantContext → UseAuthorization → UseRateLimiter → UseAntiforgery`；
4. `var api = app.MapGroup("/api/v1");` 后依次调用 19 SaaS 切片 `Map(api)`；
5. 直接 Map vsa-demo 路由：`/`、`/ring/stats`、`/catalog`、`GET /memory/{protocol}/{deviceId}`、`POST /rpc/{deviceId}`；
6. `app.MapHub<FaultsHub>("/hubs/faults");`
7. 条件 `Mcp:Enabled` → `app.MapMcp("/mcp");`
8. Map Swagger UI + Scalar + OpenApi；
9. Map Razor Components：`app.MapRazorComponents<Frontend.App>().AddInteractiveServerRenderMode();`（引用 Frontend.App）
10. Schema + Seed：`using scope` 调 `SaasSchemaBootstrap.BootstrapAsync` + `DbSeeder.SeedAsync`；
11. 最后 `await engine.StartAsync(); await app.RunAsync(); await engine.StopAsync();`（vsa-demo Disruptor 生命周期）。

**禁止出现**：`AddControllers()`、`MapControllers()`。

**TR**：
- TR-R4a (rule)：csproj 15 包名全部出现（版本可沿用现有 vsa-demo/plc-saas 版本，精确版本不强制但包名必须出现）。
- TR-R4b (rule)：`grep -cE "AddControllers|MapControllers" Program.cs = 0`。
- TR-R4c (rule)：`find Slices -name "*.cs" | wc -l` ≥ 21。
- TR-R4d (rule)：`grep -oE '("/ring/stats"|"/catalog"|"/memory/\{protocol\}/\{deviceId\}"|"/rpc/\{deviceId\}"|"/api/v1/|"hubs/faults")' Program.cs | sort -u` 输出至少 6 种不同路由模式（证明 13 个规则路由至少都有对应字符串出现）。
- TR-R4e (rule)：`grep -cE "UnpublishedEventScope|_disruptor\.PublishEvent\(\)" Infrastructure/DisruptorEngine.cs ≥ 1`。
- TR-U2 (rubric 0-2, ≥1)：按 spec NF-U2。
- TR-U3 (rubric 0-2, ≥1)：按 spec NF-U3。
- Completion Evidence：上述命令输出；Program.cs 关键路由区段拷贝；DisruptorEngine 关键配置段拷贝。

---

## Task 5：更新 plc-saas.slnx
**依赖**：Task 1-4 全部。**优先级**：高。
**覆盖 AC**：R3。

策略：
1. 新增 4 个 PlcVsa.* Project；
2. 为原 36 项目创建 Folder `/legacy/core`（PlcAiot.*、plc-saas、plc-host、plc-vsa-demo、Contracts 旧版等）、`/legacy/plugins`（plc-host/plugins/*）、`/spikes/`（csgo-spike / dapr-spike / csharpflink-spike / verify / Demohost / Stability / nativelib）；
3. **关键**：Build 配置矩阵（如果 slnx 支持 `<BuildConfigurations>` 显式）只勾选 4 个 PlcVsa.* 项目 Build=True。如果 slnx 格式里没 BuildConfigurations 机制（slnx 简化格式），则改用 `msbuild /t:Build` 时传入 `/p:BuildOnlyPlcVsa=true` 的方式 + 配合 csproj 条件 `BuildOnlyPlcVsa=true 时跳过旧工程编译`（在 Directory.Build.targets 中放一个 SkipBuild target 对非 PlcVsa.* 命名空间项目 `Build` 前置设置 `_RunBuild=false`）。

实施：写 `plc/src/PlcVsa/Directory.Build.targets` 实现上述 fallback 跳过旧项目编译。

**TR**：
- TR-R5a (rule)：slnx `<Project Path=` 中含 4 个 PlcVsa.* 项目路径。
- TR-R5b (rule)：旧 36 项目路径在 slnx 中仍全部出现。
- TR-R5c (rule)：Directory.Build.targets 含 `SkipBuild` target 逻辑（或 slnx 明确 Build Matrix 配置）。
- Completion Evidence：slnx 4+36 Project 列表 cat 输出；Directory.Build.targets 内容。

---

## Task 6：git 暂存（stash push）+ stash bundle
**依赖**：Task 5。**优先级**：最高。
**覆盖 AC**：R8、R9、R10。

执行：
```bash
# 0) 确认在 dev
git checkout dev
# 1) 先保存最新时间戳的 stash 条目计数
before=$(git stash list | wc -l)
# 2) 暂存所有新增/修改（包含 PlcVsa / slnx）
git add -A plc/src/PlcVsa plc/src/plc-saas/plc-saas.slnx plc/src/PlcVsa/Directory.Build.props plc/src/PlcVsa/Directory.Build.targets
# 如有 /workspace/plc/sync/dev-monolith-stash.bundle 先不加入（它是产物）
git stash push -m "plc-vsa monolith WIP" -- plc/src/PlcVsa plc/src/plc-saas/plc-saas.slnx
# 3) 现在重新 unstash 内容到一个临时 WIP 分支，再创建 bundle（因为 git stash 的条目不能直接 bundle）
git stash show -p "stash@{0}" > /workspace/plc/sync/dev-monolith-stash.patch
# 同时输出 stash 列表证明
git stash list | head -n 3
# 4) 为了 bundle 友好，同时把当前 unstaged WIP 提交到一次性 refs（不污染用户真实 dev HEAD 可见历史）并 bundle
#    将 stash 应用到独立的 WIP ref:
git update-ref refs/wip/plc-vsa-monolith $(git rev-parse HEAD)
git stash apply "stash@{0}" 2>&1 || git checkout -- .  # stash apply 可能失败但我们已经有 .patch 足够
# 如果 apply 成功：
git add -A plc/src/PlcVsa plc/src/plc-saas/plc-saas.slnx 2>&1
git commit --no-gpg-sign --allow-empty -m "refs/wip placeholder" 2>&1 || true
git update-ref refs/wip/plc-vsa-monolith $(git rev-parse HEAD) 2>&1 || true
# 无论如何输出：基于 HEAD 与 stash patch 的 bundle（把 patch 一起放进 bundle 不现实，改为直接打包 patch.gz + bundle 组合）
gzip -f -k /workspace/plc/sync/dev-monolith-stash.patch
git bundle create /workspace/plc/sync/dev-monolith-stash.bundle HEAD refs/wip/plc-vsa-monolith 2>&1 || \
  git bundle create /workspace/plc/sync/dev-monolith-stash.bundle HEAD 2>&1
git bundle verify /workspace/plc/sync/dev-monolith-stash.bundle 2>&1
# 最终证据：
git branch --show-current
git status --short | head -n 50
git stash list | head -n 3
ls -la /workspace/plc/sync/dev-monolith-stash.*
```
注意：若沙箱 git 配置缺 user.name/email，临时 `git -c user.email=dev@local -c user.name=dev commit ...`。

**TR**：
- TR-R6a (rule)：`git stash list | head -1` 输出包含 `"plc-vsa monolith WIP"`。
- TR-R6b (rule)：`git status --short` 只显示未追踪文件（或为空；Staged/Clean）。
- TR-R6c (rule)：存在文件 `/workspace/plc/sync/dev-monolith-stash.patch.gz`（size > 0）。
- TR-R6d (rule)：存在文件 `/workspace/plc/sync/dev-monolith-stash.bundle`，且 `git bundle verify` 输出包含 "is okay"。
- TR-R6e (rule)：`git diff --name-only HEAD -- plc/docs/scripts plc/sync/*.ps1` 为空（脚本未修改）。
- Completion Evidence：上述 5 条命令原始输出。
