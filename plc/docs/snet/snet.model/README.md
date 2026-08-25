<h1 align="center">
  <img width="100" height="100" src="https://api.snet.cn/pic/nuget.png" alt="Snet Logo"/><br/>
  Snet
</h1>

<p align="center">
  <b>统一工业通信框架 · 35+ 协议 · 5 种中间件 · 一套 API</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0%20%7C%2010.0-purple.svg"/>
  <img src="https://img.shields.io/badge/NuGet-55%2B%20packages-brightgreen.svg"/>
  <img src="https://img.shields.io/badge/license-MIT-green"/>
  <img src="https://img.shields.io/github/directory-file-count/shunnet/SKILLS?type=dir&label=AI%20Skills&color=orange"/>
</p>

<p align="center">
  <a href="https://snet.cn"><b>🌐 官网</b></a> ·
  <a href="https://docs.snet.cn"><b>📖 文档</b></a> ·
  <a href="https://www.nuget.org/profiles/shun"><b>📦 NuGet</b></a> ·
  <a href="https://github.com/shunnet/Debug"><b>🛠️ Debug 工具</b></a> ·
  <a href="https://github.com/shunnet/Daq"><b>🔌 Daq 工具</b></a> ·
  <a href="https://github.com/shunnet/SKILLS"><b>🤖 AI Skills</b></a>
</p>

---

## ✨ 亮点速览

| 亮点 | 说明 |
|:---|:---|
| 🔌 **35+ 协议驱动** | 36 个 NuGet 包覆盖 PLC、机器人、仪器仪表、电力仪表 —— 统一 `IDaq` 接口，切换协议零改代码 |
| 🧬 **统一 API 范式** | `IDaq`(17) / `IMq`(13) / `ICommunication`(14) 子接口 —— 同一套代码适配 Modbus、Siemens、MQTT 或 Kafka |
| 📦 **地址自动组包** | `IDaq` 内置 `IPacker`：`PackerAsync` 合并分散地址为最小批量读取（N 次往返→约 1 次），`UnPackerAsync` 一键拆包 · 16 个协议族预注册 · 不支持的协议自动透传 |
| 🔄 **插件热加载** | `AssemblyLoadContext` 隔离 · ZIP 上传即部署 · 运行时加载/卸载，无需重启 |
| 🌐 **内置 WebAPI** | 6 个 HTTP 端点自动暴露 · 零额外代码实现远程启停读写 |
| ⚡ **订阅推送引擎** | `SubscribeOperate` 可配间隔/变化检测/并行任务数 · 全部驱动统一支持 |
| 📨 **自动 MQ 转发** | `AddressMqParam` 一键配置 — 采集数据自动推送到 MQTT/Kafka/RabbitMQ/NetMQ/Netty |
| 🌍 **中英文热切换** | `ILanguage` + `OnLanguageEvent` 全局热切换 · 无需刷新页面 |
| 📣 **统一事件系统** | `OnDataEvent` / `OnInfoEvent` / `OnLanguageEvent` 同步 + 异步双通道 · 订阅推送零遗漏 · UI 联动零轮询 |
| ⏱️ **异步优先设计** | 全部抽象方法 `async` + `CancellationToken` 取消 · 同步为薄封装，不重复实现逻辑 |
| 🛡️ **企业级可靠** | `ConcurrentDictionary` 单例池 (≤255) · `SemaphoreSlim` 守卫 · Channels 背压 · 为 7×24 生产设计 |
| 🎭 **虚拟地址模拟** | 5 种模拟模式 (Static/Random/Range/Order/OrderScope) · 无需真实 PLC 离线开发测试完整管道 |
| 🧰 **多格式序列化** | `ToJson` / `ToXml` / `ToProtobuf` · AES 加密 · SHA1/SHA256/MD5 哈希 · Base64 编解码开箱即用 |
| 🚰 **数据管道** | `ChannelOperate<T>` 背压控制 · 同步/异步双模式 · 高吞吐数据流转 |

## 🚀 快速开始

### IDaq — 数据采集（打开 / 读取 / 写入 / 订阅 / 组包 / 解包 / 关闭 / 释放）

```bash
dotnet add package Snet.Modbus
```

```csharp
using Snet.Modbus;
using Snet.Model.data;
using Snet.Model.@enum;
using System.Collections.Concurrent;
using System.Linq;

var op = await ModbusOperate.InstanceAsync(new ModbusData.Basics { IpAddress = "192.168.1.100", Port = 502, Station = 1, AddressStartWithZero = false });

// 打开
await op.OnAsync();

// 设置地址
var address = new Address(new List<AddressDetails>
{
    new("温度", "1", DataType.Float),
    new("压力", "2", DataType.Float)
});

// 读取
var read = await op.ReadAsync(address);
if (read.Status)
    Console.WriteLine(read.GetSource<ConcurrentDictionary<string, AddressValue>>().First().Value.ResultValue);

// 写入
await op.WriteAsync(new ConcurrentDictionary<string, object> { ["1"] = 1234f,["2"]=666.6f });

// 订阅 — 先绑定事件再订阅
op.OnDataEventAsync += async (s, e) => Console.WriteLine($"收到: {e.ResultData}");
await op.SubscribeAsync(address);

// 取消订阅
await op.UnSubscribeAsync(address);

// 组包 → 读取 → 解包（IDaq 内置 IPacker）
var packed = await op.PackerAsync(address, "ModbusTcpNet");    // 组包
var packedAddr = packed.GetSource<Address>();
var batch = (await op.ReadAsync(packedAddr)).GetSource<ConcurrentDictionary<string, AddressValue>>();
var values = (await op.UnPackerAsync(batch)).GetSource<List<ConcurrentDictionary<string, AddressValue>>>();  // 解包

// 关闭 + 释放
await op.OffAsync();
await op.DisposeAsync();
```

> 将 `Modbus` 替换为任意协议名（`Siemens`/`Mitsubishi`/`Omron`/`OPC`...）— **API 完全一致**。

### IMq — 消息中间件（打开 / 生产 / 消费 / 取消消费 / 关闭 / 释放）

```bash
dotnet add package Snet.Mqtt
```

```csharp
using Snet.Mqtt.client;

var mq = await MqttClientOperate.InstanceAsync(new MqttClientData.Basics { IpAddress = "127.0.0.1", Port = 6688 });

// 打开
await mq.OnAsync();

// 消费 — 先绑定事件再订阅主题
mq.OnDataEventAsync += async (s, e) => Console.WriteLine($"收到: {e.ResultData}");
await mq.ConsumeAsync("snet/temperature");

// 生产 — 字符串 / 原始字节
await mq.ProduceAsync("snet/temperature", "hello mqtt", System.Text.Encoding.UTF8);
await mq.ProduceAsync("snet/temperature", System.Text.Encoding.UTF8.GetBytes("payload"));

// 取消消费
await mq.UnConsumeAsync("snet/temperature");

// 关闭 + 释放
await mq.OffAsync();
await mq.DisposeAsync();
```

> 将 `Mqtt` 替换为任意中间件名（`Kafka`/`RabbitMQ`/`NetMQ`/`Netty`）— **API 完全一致**。

## 🔧 内置能力

### 数据与处理

| 能力 | 类 | 说明 |
|:---|:---|:---|
| 🚰 **数据管道** | `ChannelOperate<T>` | `System.Threading.Channels` 封装 · 背压 · 同步/异步双模式 |
| 💾 **双缓存引擎** | `ProcessCacheOperate` + `ShareCacheOperate` | 进程内 MemoryCache + 跨进程 MemoryMappedFile · 重启不丢失 |
| 📦 **组包引擎** | `PackerHandler` + `IPackerCore` | 16 个协议族注册表 · `AddressAutoPackOrPassthrough` 智能入口 · `IDaq.PackerAsync/UnPackerAsync` 直接调用 |
| 🔄 **字节转换** | `BytesHandler` + `BytesTransformHandler` | 11 种标量类型 + 数组 + 4 种字节序自动转换 |
| 📊 **地址模型** | `Address` / `AddressDetails` / `AddressValue` | 13 属性 + 8 种编码 · `AddressMqParam` 一键 MQ 转发 · `AddressParseParam` 报文解析 |
| 🎭 **虚拟地址模拟** | `VirtualAddressManage` | 5 种模式 (Static/Random/Range/Order/OrderScope) · 无需真实 PLC 离线开发测试 |

### 扩展与工具

| 能力 | 类 | 说明 |
|:---|:---|:---|
| 🧩 **插件引擎** | `PluginOperate` | `AssemblyLoadContext(isCollectible:true)` · ZIP 上传即部署 · 运行时热插拔 |
| 🔍 **反射工具** | `ReflectionOperate` | 动态加载 DLL · 调用方法 · 注册事件 |
| 🧰 **扩展方法** | `CoreExtend` | `GetSource<T>` 解包 · `ToJson/ToXml/ToProtobuf` · AES/SHA1/SHA256/MD5 · Base64 |
| 🌐 **WebAPI** | 内置于 DaqAbstract | 6 端点自动暴露 (`/api/on` `/api/read` ...) · `WAModel` 配置端口+跨域 |
| 📜 **日志系统** | `Snet.Log` (`LogCore` + `LogHelper`) | Serilog 封装 · 按日期/文件夹自动分类 · 6 级日志 (Verbose~Fatal) · 后台自动清理过期日志 |

### 框架机制

| 能力 | 类 | 说明 |
|:---|:---|:---|
| 🛡️ **企业级可靠** | `CoreUnify` | 单例池 (≤255) · `SemaphoreSlim` 守卫 · Channels 背压 · 7×24 生产设计 |
| ⚡ **订阅引擎** | `SubscribeOperate` | 可配间隔/变化检测 (ChangeOut/AllOut)/并行任务数 · 全部驱动统一支持 |
| 📬 **MQ 编排** | `MqOperate` | `AddressMqParam` 自动转发 · FilesystemWatcher 监控 · 采集→队列零代码 |
| 🌍 **多语言** | `ILanguage` + `LanguageHandler` | 中英文热切换 · `OnLanguageEvent` 全局响应 · 参数反射 (`ParamHandler`) |
| 🔗 **地址分发** | `AddressHandler` | 类型化地址处理 · 属性反射 · 扩展参数传递 |
| 🔌 **通信层** | `CommunicationAbstract` 系 | TCP/UDP/WebSocket/HTTP/Serial Client+Server · 统一 `Send/SendWait` · 断线重连 |

## 🤖 AI 辅助开发

<p align="center">
  <a href="https://github.com/shunnet/SKILLS"><b>📦 Snet AI Skills — 自然语言生成完整项目代码</b></a>
</p>

> 每个技能对应 [SKILLS 仓库](https://github.com/shunnet/SKILLS) 的一个目录，**新增技能 = 新建目录**，顶部徽章自动 +1，无需手动改。

| 技能 | 目录 | 一句话示例 |
|:---|:---|:---|
| 🔌 **DAQ-Skill** | [`DAQ-Skill/`](https://github.com/shunnet/SKILLS/tree/main/DAQ-Skill) | "连接西门子 S7-1500，读 DB1.0，MQTT 转发到 broker" → 完整项目 |
| 📡 **MQ-Skill** | [`MQ-Skill/`](https://github.com/shunnet/SKILLS/tree/main/MQ-Skill) | "MQTT 连接 127.0.0.1:1883，订阅 factory/line1 打印消息" → 收发应用 |
| 🧩 **PluginDev-Skill** | [`PluginDev-Skill/`](https://github.com/shunnet/SKILLS/tree/main/PluginDev-Skill) | "开发温湿度传感器插件，TCP 发 Modbus 帧" → ZIP 热插拔包 |
| 🖥️ **WpfUI-Skill** | [`WpfUI-Skill/`](https://github.com/shunnet/SKILLS/tree/main/WpfUI-Skill) | "创建深色主题监控面板，LED 状态灯，中英文切换" → WPF 应用 |

## 🌍 链接

| 资源 | 地址 |
|:---|:---|
| 🌐 官网 | [snet.cn](https://snet.cn) |
| 📖 文档 | [docs.snet.cn](https://docs.snet.cn) |
| 📦 NuGet | [nuget.org/profiles/shun](https://www.nuget.org/profiles/shun) |
| 💻 GitHub | [github.com/shunnet](https://github.com/shunnet) |
| 🛠️ Debug 工具 | [github.com/shunnet/Debug](https://github.com/shunnet/Debug) |
| 🔌 Daq 工具 | [github.com/shunnet/Daq](https://github.com/shunnet/Daq) |
| 🤖 AI Skills | [github.com/shunnet/SKILLS](https://github.com/shunnet/SKILLS) |
| 📝 博客 | [blog.snet.cn](https://blog.snet.cn) |

<p align="center">
  <b>MIT License</b> · 开源 · 免费商用 · <a href="https://snet.cn">snet.cn</a>
</p>
