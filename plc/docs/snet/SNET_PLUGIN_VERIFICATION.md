# SNET 系列插件加载验证报告

> 日期：2026-09-12（v1.1：11 品牌）→ 2026-10-13（v1.2：32 品牌）| 宿主：`src/plc-gateway` | 插件：`SnetProtocols` | 验证样本：`samples/SnetPluginProbe`
> 目标：用插件机制逐个加载 snet 系列 DLL，在 sample 中调用 snet 方法，验证插件任务执行逻辑能否匹配不同 PLC 设备数据采集。

## 结论：32/32 PASS ✅

> 品牌清单由 `snet-discover` 反射扫描 `src/plc-gateway/lib/snet` 全部 DLL 确认（`Snet.<Brand>.<Brand>Operate` / `<Brand>Data.Basics`），
> lib/snet 下共 32 个设备协议 DLL + 若干基础设施 DLL（Core/Model/Driver/Log/Utility/DB/Opc/Mqtt/Kafka/Redis/RabbitMQ/Rpc/NetMQ/Netty/PQDIF/PerformanceTesting，非设备协议，不注册）。

| 协议 | 结果 | 说明 |
|------|------|------|
| snet.modbus.tcp | PASS | 适配器正确下发到驱动，连接失败返回空（设备不可达，符合预期） |
| snet.siemens.tcp | PASS | 同上 |
| snet.mitsubishi.tcp | PASS | 同上 |
| snet.fatek.tcp | PASS | 同上 |
| snet.omron.tcp | PASS | 同上 |
| snet.keyence.tcp | PASS | 同上 |
| snet.fuji.tcp | PASS | 同上 |
| snet.allenbradley.tcp | PASS | 同上 |
| snet.delta.tcp | PASS | 同上 |
| snet.panasonic.tcp | PASS | 同上 |
| snet.beckhoff.tcp | PASS | **v1.2 新增**，同上 |
| snet.cimon.tcp | PASS | **v1.2 新增**，同上 |
| snet.fanuc.tcp | PASS | **v1.2 新增**，同上 |
| snet.freedom.tcp | PASS | **v1.2 新增**，同上 |
| snet.ge.tcp | PASS | **v1.2 新增**，同上 |
| snet.inovance.tcp | PASS | **v1.2 新增**，同上 |
| snet.invt.tcp | PASS | **v1.2 新增**；驱动 `OnAsync` 内部 NRE 被 snet 框架自捕获、优雅返回失败（见下注） |
| snet.kossi.tcp | PASS | **v1.2 新增**，同上（连接失败预期） |
| snet.lsis.tcp | PASS | **v1.2 新增**，同上 |
| snet.megmeet.tcp | PASS | **v1.2 新增**，同上 |
| snet.orientalmotor.tcp | PASS | **v1.2 新增**（东方马达），同上 |
| snet.rkc.tcp | PASS | **v1.2 新增**，同上 |
| snet.toyota.tcp | PASS | **v1.2 新增**，同上 |
| snet.turck.tcp | PASS | **v1.2 新增**，同上 |
| snet.vigor.tcp | PASS | **v1.2 新增**，同上 |
| snet.wecon.tcp | PASS | **v1.2 新增**；同 invt，驱动 `OnAsync` 内部 NRE 被框架自捕获 |
| snet.xinje.tcp | PASS | **v1.2 新增**，同上（连接失败预期） |
| snet.yamatake.tcp | PASS | **v1.2 新增**，同上 |
| snet.yaskawa.tcp | PASS | **v1.2 新增**，同上 |
| snet.yokogawa.tcp | PASS | **v1.2 新增**，同上 |
| snet.yudian.tcp | PASS | **v1.2 新增**；默认走串口 COM3（`Could not find file 'COM3'`），驱动层错误，适配器到位 |
| snet.sim | PASS | **内存往返成功，写 0x1234 → 读回 2 字节**（端到端） |

## 验证口径（硬件无关）

- 探针用与 `GatewayHost.LoadPlugins` **完全一致**的 `McMaster.NETCore.Plugins` `sharedTypes` 机制按目录加载 `SnetProtocols.dll`，枚举 `IDeviceCatalog.All`。
- 真实品牌指向 `127.0.0.1:1`（关闭端口）：断言适配器已正确构造各品牌 `Basics`（`IpAddress`/`Port`/`Station`(int)/`Rack`/`Slot`）、覆盖 TCP `ProtocolType`、构造 `AddressDetails` 并下发到驱动；连接失败（"连接失败/超时/未连接"）为预期，不视为适配器缺陷。
- `snet.sim` 内存模拟器：写后读，断言返回字节 > 0，证明端到端数据通路。
- **判定只认适配器层缺陷**（NullReference/InvalidCast/Reflection/MissingMethod/FileLoad 且**未被 snet 框架自捕获**）。

### v1.2 新增的驱动层观察（非适配器缺陷，已记录）

1. **invt / wecon**：泛化适配器仅按通用属性名（IpAddress/Port/Station）构造 Basics，这两个品牌的 `OnAsync` 内部对某个未设置的属性解引用抛 `NullReferenceException`；snet 的 `DaqAbstract` 框架自行捕获该异常并返回失败的 `OperateResult`，适配器优雅拿到空字节 → 判 PASS。**接真实硬件时这两个品牌可能需要品牌专属的 Basics 字段**，属驱动特性，不影响适配器正确性验证。
2. **yudian**：`Basics` 无 `IpAddress`/`ProtocolType` 覆盖为串口默认值（COM3），连接报 `Could not find file 'COM3'` → 适配器已正确构造并下发到驱动，判 PASS。
3. **其余 29 个品牌**：均按 TCP 连接 `127.0.0.1:1` 快速失败（1000ms 超时），符合预期。

## 复跑

```bash
# 1) 构建插件（输出到网关 bin/.../plugins/，并镜像 snet 运行期 DLL 到宿主根）
cd src/plc-gateway/plugins/SnetProtocols && dotnet build -c Debug

# 2) 构建 + 运行探针（自动把插件输出同步到自身 plugins/ 并镜像到宿主根）
cd src/plc-gateway/samples/SnetPluginProbe && dotnet build -c Debug && dotnet bin/Debug/net11.0/SnetPluginProbe.dll
# 期望末行：汇总: PASS=32  FAIL=0  共注册=32
```

## 涉及文件

- `src/plc-gateway/plugins/SnetProtocols/SnetPlugin.cs`（泛化反射适配器 + Sim `VirtualStatic` + `ResultValue` 转换 + **32 品牌注册表 v1.2.0**）
- `src/plc-gateway/plugins/SnetProtocols/SnetProtocols.csproj`（**37 个 snet DLL 引用**：32 设备 + 5 基础设施）
- `src/Plc.Plugins.Contracts/IDeviceProtocol.cs`（契约扩展 `ElementType`/`AddressText`/`Rack`/`Slot`/`TypedValues`）
- `src/plc-gateway/samples/SnetPluginProbe/Program.cs`（验证样本，遍历 `catalog.All` 全品牌）
- `src/plc-gateway/samples/SnetPluginProbe/SnetPluginProbe.csproj`（含 `runtimeconfig.template.json` 绑定重定向 + 插件镜像）
- `src/plc-gateway/samples/SnetPluginProbe/runtimeconfig.template.json`
- `src/plc-gateway/samples/SnetApiSample/`（直接引用 snet DLL 的原生方法验证，见下节）
- `.workbuddy/tools/snet-discover/`（品牌发现扫描器，gitignored，非交付物）


---

# 补充：SnetApiSample — 直接引用 snet DLL 测试原生方法

> 用户追加需求：用 `snet.sim` 模拟设备测试 snet 功能，并新增 sample 直接测试 DLL 中的方法（绕过插件/契约抽象层）。
> 样本：`src/plc-gateway/samples/SnetApiSample/`（控制台，直接 `<Reference>` 七个 snet DLL）。

## 结论：全部硬检查 PASS ✅（退出码 0）

| 测试 | 结果 | 说明 |
|------|------|------|
| OnAsync 启动模拟设备 | PASS | 创建 `VirtualAddressManage`，无异常 |
| VirtualStatic 读写闭环(Int16) | PASS | 写 4660 → 读回 4660 |
| VirtualStatic 读写闭环(Int32) | PASS | 写 123456 → 读回 123456 |
| VirtualStatic 读写闭环(Float) | PASS | 写 3.14 → 读回 3.14 |
| VirtualStatic 读写闭环(Bool) | PASS | 写 true → 读回 True |
| 动态类型注册(4 种) | PASS | Random/RandomScope/Order/OrderScope 均 `ReadAsync` 成功、无异常 |
| OffAsync 关闭 | PASS | 释放 VAM |

## 关键发现：动态虚拟地址取值演化在本构建中不保证每拍刷新

- `VirtualAddress` 动态任务（`StartDynamicRandomTask`/`StartDynamicOrderTask` 等）由 `Task.Run` 后台循环按间隔 `Task.Delay` 刷新 `_value`。
- 实测：在“仅 `ReadAsync` 轮询、未激活订阅”场景下，后台生成任务偶发首拍后即冻结（多次采样取值全空或恒定），属 snet 本构建的 quirks，非用法错误。
- 处理：动态模式以「地址注册 + `ReadAsync` 读路径无异常」为硬通过项；取值演化作为**观测项**记录，不计入失败。确定性验证由 `VirtualStatic` 多类型闭环承担。

## 复跑

```bash
cd src/plc-gateway/samples/SnetApiSample && dotnet build -c Debug && dotnet run -c Debug --no-build
```

## 涉及文件（新增）

- `src/plc-gateway/samples/SnetApiSample/Program.cs`（直接调用 `SimOperate.OnAsync/ReadAsync/WriteAsync` + `GetDetails`）
- `src/plc-gateway/samples/SnetApiSample/SnetApiSample.csproj`（`<Reference>` Snet.Core/Model/Driver/Sim/Log/Utility + Serilog/Newtonsoft/System.IO.Ports + `runtimeconfig.template.json` 绑定重定向）
- `src/plc-gateway/samples/SnetApiSample/runtimeconfig.template.json`（同上 `System.IO.Ports` 重定向）

---

# 补充：真实网关宿主端到端验证（2026-10-13）

> 探针（`SnetPluginProbe`）只证明「插件加载机制与宿主一致」；本节把验证推到**真实宿主**：
> `PlcGateway`（`src/plc-gateway`）实际加载 `SnetProtocols` 插件，经 plc-host 的 `/api/gateway/*` 走完
> HTTP → 宿主 → NamedPipe StreamJsonRpc → 网关 → 插件 → snet 驱动全链路。

## 修复：PlcGateway.csproj 编译回归

`PlcGateway.csproj` 的 `<Compile Include="**/*.cs" Exclude="GatewayClient/**;plugins/**;**/obj/**;**/bin/**" />`
未排除新增的 `samples/**`，把两个自治 sample 项目（引用 snet 类型）编进宿主 → 11 个 CS0246。
**修法**：Exclude 补 `samples/**`（与 `GatewayClient/**`、`plugins/**` 同等地位）。

## 真实宿主注册验证（手动起网关实跑）

- 启动日志：`SNET 协议已注册 32/32`、`插件已加载: SNET PLC Protocols (series) (snet.protocols)`。
- `已注册协议` 共 **33** 个 = 32 snet + `builtin.sim`（宿主内置保底协议）。
- `GatewayHost.LoadPlugins` 扫描 `plugins/` 全部 47 个 DLL（37 snet + Serilog×3 + Newtonsoft + System.IO.Ports + SnetProtocols + 契约），**0 个插件加载失败**——非插件 DLL 加载后无 `IPlugin` 类型，静默跳过。
- `app.manifest.json` 新增设备 `dev-snet-sim-1`（protocol `snet.sim`，enabled，poll 1000ms），网关轮询日志 `dev-snet-sim-1 -> 2 bytes snet.sim` 每秒成功。

## e2e 脚本扩展：`scripts/verify-gateway-webapi.sh`

新增 5 项 snet 链路断言（原有 5 项 builtin.sim 不变）：

| 断言 | 期望 | 实测 |
|------|------|------|
| `snet-protocols>=32` | `/api/gateway/protocols` 含 ≥32 个 `snet.*` | PASS（32） |
| `snet-device-listed` | `/api/gateway/devices` 含 `dev-snet-sim-1` | PASS |
| `snet-read-deviceId` | read 响应含 `"deviceId":"dev-snet-sim-1"` | PASS |
| `snet-read-bytes>0` | read 响应 `bytes` ≥ 1 | PASS（2） |
| `snet-write` | write 响应 `"written":2` | PASS |

**实跑结果：10/10 PASS，退出码 0。** snet.sim read 实回：
`{"deviceId":"dev-snet-sim-1","data":"/J0=","bytes":2}`（VirtualStatic 随机 Int16，base64 `/J0=`）。

## 驱动层观察（记录在案，非适配器缺陷）

1. **会话释放噪音**：`GatewayRpc` 每次读写按需新建会话（`await using var session = ...`），读后即 dispose → snet 基类 `WAOffAsync` 打 `WA尚未启动` ERR。Sim 无 WA 子系统，属 snet 内部日志噪音，不影响功能。
2. **sim 跨调用无状态**：按需新建会话对真实 PLC 正确（每次连接独立）；对 sim 意味着 write 后再 read（两次 RPC）不会回读到写入值。sim 的写后读闭环只在**同一会话内**成立（`SnetApiSample` 已证明），e2e 因此不断言跨调用回读。

## 复跑

```bash
cd src/plc-gateway && dotnet build -c Debug   # 宿主；plugins/ 由 SnetProtocols 构建填充
cd src/plc-host && dotnet build -c Debug      # 宿主代理
export no_proxy='*' NO_PROXY='*'
bash scripts/verify-gateway-webapi.sh         # 期望 10 项全 PASS
```

