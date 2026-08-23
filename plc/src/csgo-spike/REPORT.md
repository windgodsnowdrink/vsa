# CsGo Spike 报告（plc-host / .NET 11 Preview 7）

## ① Verdict：Compatible-with-retargeting（技术可构建可运行）；采用级 = Blocked

## ② 来源与还原+构建结果
- 来源：GitHub `HAM-2015/CsGo`，**无 NuGet 包、无 Release**。
- **无 LICENSE 文件**（GitHub API 返回 404 → 全权保留，法律上不可采用）。最近提交 2021-07-09，已停止维护。
- 构建：将 4.5.2 旧 csproj 重写为 SDK 风格并重定目标 `net11.0-windows`；仅补充 `System.IO.Ports` 包引用（SerialPort）并启用 WinForms（`control_strand` 耦合）。结果 **0 警告 / 0 错误**，产出 `CsGo.dll`（481KB）。

## ③ POC 关键输出
生产者发送 1–5、消费者收全 5，`chan.close()` 解阻塞消费者，`work_service` 正常退出（输出见末尾）。

## ④ 集成草图 + 相对 Channel 的取舍
草图 `CsGoCapability : ICapabilityModule`（`Id="concurrency.csgo"`，`Order=50`）注册单例 `CsGoScheduler`（封装 `work_service`+`shared_strand`），以 `IHostedService` 驱动 `run()`；MQTT 订阅经 strand 派发，业务用 `generator.go`/`chan` 串联（见 `CsGoCapability.cs`）。
- 增量价值：`select` 多路复用、`generator.children` 树状取消（`stop`）、内置高精度定时器、协程式 goroutine 调度语义——`System.Threading.Channels`+Polly 不直接提供。
- 代价：自研调度器 vs 线程池、必须 Windows（`net11.0-windows`）、依赖一个停更且无许可的 20K 行代码库，维护活跃度风险高。

## ⑤ 风险建议
**不引入 CsGo。** 当前 MQTT + `System.Threading.Channels` + Polly 已覆盖队列/弹性需求；CsGo 无许可、停更、仅 Windows，风险远超其 `select`/定时器收益。若确需其 select 语义，建议自行小范围实现而非依赖该库。

---
## 关键命令输出（证据）

### 库构建（net11.0-windows，加 System.IO.Ports 后）
```
已成功生成。
    0 个警告
    0 个错误
已用时间 00:00:00.25
-rwxr-xr-x 1 xueyin 121 481792 Aug 19 22:07 bin/Release/net11.0-windows/CsGo.dll
```

### POC 运行（dotnet run -c Release）
```
=== CsGo spike: producer -> consumer over chan<int> ===
[producer] sent 1
[producer] sent 2
[producer] sent 3
[producer] sent 4
[producer] sent 5
[producer] closed channel
[consumer] recv 1 (total 1)
[consumer] recv 2 (total 2)
[consumer] recv 3 (total 3)
[consumer] recv 4 (total 4)
[consumer] recv 5 (total 5)
[consumer] channel closed, exiting
=== work_service stopped; spike finished ===
```

### 还原/拉取说明
GitHub git:443 直连被网络拦截；改用 `curl` 下载 codeload tarball
`https://codeload.github.com/HAM-2015/CsGo/tar.gz/refs/heads/master`（HTTP 200，81KB）后解压。
