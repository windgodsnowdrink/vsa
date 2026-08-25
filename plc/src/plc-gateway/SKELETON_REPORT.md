# plc-gateway 骨架完成报告

> 日期：2026-08-24
> 路径：`src/plc-gateway/`
> 状态：骨架打通，端到端链路验证通过（0 错误 0 警告）

## 架构全景（对应 8 项原始需求）

```
┌─────────────────────────────────────────────────────────────┐
│  plc-gateway (宿主 exe, net11.0)                              │
│  ├─ app.manifest.json       需求2: file-based app 装配配置   │
│  ├─ GatewayHost             需求3/5: DotNetCorePlugins 加载  │
│  ├─ FramePipeline           需求4: Disruptor 环形缓冲流水线   │
│  ├─ Program                 需求6: vs-threading 背压/并发     │
│  └─ GatewayRpc              需求7: StreamJsonRpc over NamedPipe│
└───────────────────────────┬─────────────────────────────────┘
                            │ 加载
                            ▼
              plugins/SnetProtocols/  (ALC 隔离)
              ├─ Snet.Core/Model/Driver/Modbus/Siemens/Mitsubishi/Log/Utility.dll
              ├─ Serilog 4.4.0 + Sinks (Console/File) 7.0.0
              ├─ Newtonsoft.Json 13.0.1
              └─ System.IO.Ports 10.0.0.11
              → 需求1/8: SNET 反编译 PLC 协议 + 消息记忆(ShareCache/ProcessCache)
```

## 已验证能力

| 能力 | 状态 | 证据 |
|------|------|------|
| 插件加载（DotNetCorePlugins） | ✅ | `插件已加载: SNET PLC Protocols (snet.protocols)` |
| SNET 协议注册 | ✅ | `已注册协议: snet.siemens.tcp, snet.modbus.tcp, snet.mitsubishi.tcp, builtin.sim` |
| Disruptor 流水线 | ✅ | `[seq N] dev-sim-1 -> 20 bytes builtin.sim` 持续输出 |
| vs-threading 并发采集 | ✅ | `AsyncSemaphore.EnterAsync` + `JoinableTaskFactory.RunAsync` |
| StreamJsonRpc NamedPipe | ✅ | `RPC 监听 NamedPipe: plc-gateway` |
| RPC read/write 设备路由 | ✅ | `gateway.read`/`gateway.write` 经设备注册表路由到 `IDeviceProtocol` 会话，结果作为 `RawFrame` 发布进 Disruptor 流水线（消息记忆）；`builtin.sim` 链路 `read 00010002` / `write+read` 一致 |
| 内置保底协议 sim | ✅ | 无真实设备时跑通采集→流水线→RPC 全链路 |
| SNET 依赖解析 | ✅ | System.IO.Ports / Serilog / Snet.Core 全部解析成功（无 FileNotFoundException） |

## 关键工程决策

1. **SNET 二进制隔离在插件内**：宿主只依赖 `Plc.Plugins.Contracts` 共享契约，SNET 的 8 个 DLL + NuGet 依赖全部在 `SnetProtocols` 插件程序集中，经 DotNetCorePlugins 的 ALC 加载。满足需求5（用 DotNetCorePlugins 而非旧 ALC 方案），同时契约作为宿主↔插件边界。

2. **SNET 按宿主根目录解析兄弟程序集**：SNET 内部用 `AssemblyLoadContext.LoadFromAssemblyPath(AppContext.BaseDirectory + "Snet.Core.dll")`，故 post-build 把 `plugins/*.dll` 同步到宿主根目录。

3. **精确版本匹配 SNET 强签名**：Serilog 4.4.0 / Sinks.File 7.0.0 / System.IO.Ports 10.0.0.11 必须与 SNET 引用的程序集版本一致，否则运行时 `FileLoadException`。

4. **manifest 设备反序列化**：用 `System.Text.Json` 直读文件 + `PropertyNameCaseInsensitive`，规避 `ConfigurationBinder` 对 `rack`/`slot` 未知字段的敏感度。

## 当前限制（下步工作）

- `dev-modbus-1` 默认 `enabled:false`：本机无 Modbus 模拟器（127.0.0.1:502 连不上）。接真实设备/模拟器后启用即可，SNET 协议注册不受影响。
- `GatewayRpc.read/write` 已端到端验证：经设备注册表路由到协议会话（`IDeviceProtocol.CreateSession`），读/写结果发布为 `RawFrame` 进 Disruptor 流水线（消息记忆）；`builtin.sim` 保底链路 `read 00010002` / `write+read` 一致，`verify-gateway-rpc.sh` 跑通 `ALL PASS`。
- SNET 会话连通性（BegOperateAsync/ConnectServerAsync 超时与重试）需针对真实硬件调参。
- 根目录协议壳（Melsec/Fatek/Fuji/Keyence/BACnet/LoRa）已按 ADR-118 迁移为独立 ALC 插件（#133–#138），`plc-host.csproj` 无悬挂壳引用，宿主构建干净。

## 运行方式

```bash
cd src/plc-gateway
dotnet build -c Debug
dotnet bin/Debug/net11.0/PlcGateway.dll
# RPC 客户端可通过 NamedPipe \\.\pipe\plc-gateway 连接，调用 gateway.listProtocols
```

## 宿主 WebApi 桥接（消费方，plc-host 侧）

`plc-host` 新增 `GatewayModule`（`ICapabilityModule` + `ICarterModule`）经 StreamJsonRpc NamedPipe 客户端
（`GatewayRpcClient`，单例复用连接 + 断线重连重试）代理本网关，把边缘 SNET 协议 + Disruptor 消息记忆
能力透传到 HTTP：

- `GET /api/gateway/health` — 连接状态
- `GET /api/gateway/protocols` / `GET /api/gateway/devices` — 协议/设备目录
- `POST /api/gateway/read`（`{deviceId,start,count,type}`）→ `{deviceId,data(base64),bytes}`
- `POST /api/gateway/write`（`{deviceId,start,type,values}`）→ `{deviceId,written}`

网关不可用时端点返回 **503**，网关 RPC 业务异常（设备未找到等）返回 **502**。
`scripts/verify-gateway-webapi.sh` 启动网关 + 宿主（端口 5088）并 curl 验证，全部 `PASS`。
