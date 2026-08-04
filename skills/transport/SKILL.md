# Transport 技能

## 技能概述

Transport 技能是一个基于 .NET 的传输协议和机制实现库，提供了多种传输方式的统一接口和实现。该技能支持 HTTP、TCP、UDP 等网络传输协议，以及管道（Pipeline）、通道（Channel）等内存传输机制，为应用程序提供高效、可靠的传输能力。

### 主要功能

- **HTTP 传输**：支持 HTTP/HTTPS 请求和响应
- **TCP 传输**：支持可靠的 TCP 客户端和服务器
- **UDP 传输**：支持无连接的 UDP 通信
- **管道传输**：基于 System.IO.Pipelines 的高性能管道传输
- **通道传输**：基于 System.Threading.Channels 的异步通道通信
- **缓冲区管理**：高效的内存缓冲区管理
- **依赖注入**：集成 Microsoft.Extensions.DependencyInjection
- **装饰器模式**：使用 Scrutor 实现服务装饰
- **命令行接口**：完整的命令行操作支持
- **AOT 编译**：支持 Ahead-of-Time 编译，提高性能

## 技术栈

- **.NET 10.0**：目标框架
- **C#**：主要开发语言
- **System.Net.Http**：HTTP 客户端
- **System.Net.Sockets**：TCP/UDP 套接字
- **System.IO.Pipelines**：高性能管道
- **System.Threading.Channels**：异步通道
- **System.Buffers**：缓冲区管理
- **System.CommandLine**：命令行解析
- **Microsoft.Extensions.DependencyInjection**：依赖注入
- **Microsoft.Extensions.Logging**：日志系统
- **Scrutor**：依赖注入装饰器模式

## 快速开始

### 安装与配置

1. **编译技能**

```bash
cd transport/scripts
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishAot=true
```

2. **运行技能**

```bash
./bin/Release/net10.0/win-x64/publish/transport_core.exe --help
```

### 基本使用

#### HTTP 请求示例

```bash
# 发送 GET 请求
transport_core http --url "https://api.example.com/data"

# 发送 POST 请求
transport_core http --url "https://api.example.com/data" --method POST --data "{\"name\": \"test\"}"
```

#### TCP 客户端示例

```bash
# 发送 TCP 消息
transport_core tcp --host "localhost" --port 8080 --message "Hello TCP"
```

#### UDP 客户端示例

```bash
# 发送 UDP 消息
transport_core udp --host "localhost" --port 8081 --message "Hello UDP"
```

#### 管道传输示例

```bash
# 运行管道传输示例
transport_core pipeline --count 100
```

#### 通道传输示例

```bash
# 运行通道传输示例
transport_core channel --count 100
```

## 核心功能

### 1. HTTP 传输

HTTP 传输模块提供了基于 System.Net.Http 的 HTTP 客户端实现，支持以下功能：

- **GET/POST/PUT/DELETE** 等 HTTP 方法
- **请求头和响应头** 管理
- **内容类型** 自动检测
- **超时设置**
- **重试机制**
- **代理支持**

### 2. TCP 传输

TCP 传输模块提供了基于 System.Net.Sockets 的 TCP 客户端和服务器实现，支持以下功能：

- **可靠的连接** 建立和管理
- **消息发送和接收**
- **超时控制**
- **错误处理**
- **缓冲区管理**

### 3. UDP 传输

UDP 传输模块提供了基于 System.Net.Sockets 的 UDP 通信实现，支持以下功能：

- **无连接通信**
- **消息发送和接收**
- **广播和多播** 支持
- **缓冲区管理**

### 4. 管道传输

管道传输模块基于 System.IO.Pipelines 实现，提供高性能的内存数据传输，支持以下功能：

- **高性能** 数据传输
- **背压** 支持
- **内存管理**
- **异步操作**

### 5. 通道传输

通道传输模块基于 System.Threading.Channels 实现，提供异步的消息传递机制，支持以下功能：

- **异步消息传递**
- **有界和无界** 通道
- **背压** 支持
- **取消操作**

### 6. 缓冲区管理

缓冲区管理模块提供了高效的内存缓冲区管理，支持以下功能：

- **缓冲区池** 管理
- **内存分配优化**
- **零拷贝** 操作
- **Span<T> 和 Memory<T>** 支持

### 7. 依赖注入

依赖注入模块集成了 Microsoft.Extensions.DependencyInjection，提供以下功能：

- **服务注册**
- **生命周期管理**
- **构造函数注入**
- **属性注入**

### 8. 装饰器模式

装饰器模式模块使用 Scrutor 实现，提供以下功能：

- **服务装饰**
- **多层装饰**
- **条件装饰**
- **程序集扫描**

## API 参考

### IHttpTransportService

```csharp
public interface IHttpTransportService
{
    Task<string> SendRequestAsync(string url, string method = "GET", string data = null, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}
```

### ITcpTransportService

```csharp
public interface ITcpTransportService
{
    Task<string> SendMessageAsync(string host, int port, string message, CancellationToken cancellationToken = default);
    Task StartServerAsync(int port, Func<string, Task<string>> handler, CancellationToken cancellationToken = default);
}
```

### IUdpTransportService

```csharp
public interface IUdpTransportService
{
    Task SendMessageAsync(string host, int port, string message, CancellationToken cancellationToken = default);
    Task<string> ReceiveMessageAsync(int port, CancellationToken cancellationToken = default);
}
```

### IPipelineTransportService

```csharp
public interface IPipelineTransportService
{
    Task ProcessPipelineAsync(int itemCount, CancellationToken cancellationToken = default);
    Task<byte[]> ReadFromPipelineAsync(PipeReader reader, CancellationToken cancellationToken = default);
    Task WriteToPipelineAsync(PipeWriter writer, byte[] data, CancellationToken cancellationToken = default);
}
```

### IChannelTransportService

```csharp
public interface IChannelTransportService
{
    Task ProcessChannelAsync(int messageCount, CancellationToken cancellationToken = default);
    Task SendToChannelAsync<T>(ChannelWriter<T> writer, T message, CancellationToken cancellationToken = default);
    Task<T> ReceiveFromChannelAsync<T>(ChannelReader<T> reader, CancellationToken cancellationToken = default);
}
```

### IBufferTransportService

```csharp
public interface IBufferTransportService
{
    Task ProcessBufferAsync(int bufferSize, CancellationToken cancellationToken = default);
    byte[] RentBuffer(int size);
    void ReturnBuffer(byte[] buffer);
}
```

### IScrutorDemoService

```csharp
public interface IScrutorDemoService
{
    void DemonstrateBasicRegistration();
    void DemonstrateDecoratorPattern();
    void DemonstrateServiceFiltering();
    void DemonstrateLifetimeManagement();
    void DemonstrateAdvancedRegistration();
    void DemonstrateAssemblyScanning();
    void DemonstrateMultipleDecorators();
    void DemonstrateConditionalRegistration();
}
```

## 命令行接口

### 基本命令

#### http 命令

```bash
# 发送 GET 请求
transport_core http --url "https://api.example.com/data"

# 发送 POST 请求
transport_core http --url "https://api.example.com/data" --method POST --data "{\"name\": \"test\"}"

# 发送带自定义头的请求
transport_core http --url "https://api.example.com/data" --method GET --headers "Authorization: Bearer token"
```

#### tcp 命令

```bash
# 发送 TCP 消息
transport_core tcp --host "localhost" --port 8080 --message "Hello TCP"

# 启动 TCP 服务器
transport_core tcp --server --port 8080
```

#### udp 命令

```bash
# 发送 UDP 消息
transport_core udp --host "localhost" --port 8081 --message "Hello UDP"

# 接收 UDP 消息
transport_core udp --receive --port 8081
```

#### pipeline 命令

```bash
# 运行管道传输示例
transport_core pipeline --count 100

# 运行带大小限制的管道传输
transport_core pipeline --count 100 --size 1024
```

#### channel 命令

```bash
# 运行通道传输示例
transport_core channel --count 100

# 运行带边界的通道传输
transport_core channel --count 100 --bounded --capacity 10
```

#### buffer 命令

```bash
# 运行缓冲区操作示例
transport_core buffer --size 1024

# 运行缓冲区池示例
transport_core buffer --size 1024 --pool
```

#### scrutor 命令

```bash
# 运行所有 Scrutor 演示
transport_core scrutor --type all

# 运行装饰器模式演示
transport_core scrutor --type decorator

# 运行程序集扫描演示
transport_core scrutor --type scanning
```

### 全局选项

```bash
# 设置日志级别
transport_core --verbose http --url "https://api.example.com/data"

# 设置超时
transport_core --timeout 30 http --url "https://api.example.com/data"

# 设置缓冲区大小
transport_core --buffer-size 65536 tcp --host "localhost" --port 8080 --message "Hello"
```

## 配置说明

### 环境变量

| 环境变量 | 描述 | 默认值 |
|---------|------|-------|
| DOTNET_ENVIRONMENT | 运行环境 | Development |
| DOTNET_LOG_LEVEL | 日志级别 | Information |
| TRANSPORT_MAX_BUFFER_SIZE | 最大缓冲区大小 | 65536 |
| TRANSPORT_TIMEOUT | 超时时间(毫秒) | 30000 |
| TRANSPORT_HTTP_RETRY_COUNT | HTTP 重试次数 | 3 |
| TRANSPORT_HTTP_RETRY_DELAY | HTTP 重试延迟(毫秒) | 1000 |
| TRANSPORT_TCP_BUFFER_SIZE | TCP 缓冲区大小 | 8192 |
| TRANSPORT_UDP_BUFFER_SIZE | UDP 缓冲区大小 | 8192 |
| TRANSPORT_PIPELINE_MIN_SIZE | 管道最小大小 | 512 |
| TRANSPORT_PIPELINE_MAX_SIZE | 管道最大大小 | 65536 |
| TRANSPORT_CHANNEL_CAPACITY | 通道容量 | -1 (无界) |

### 配置文件

#### transport_core.setting.json

主要配置编译选项、依赖项和构建参数。

#### transport_core.run.json

主要配置运行时选项、环境变量和配置文件。

## 性能优化

### 1. 内存优化

- **使用缓冲区池**：减少内存分配和 GC 压力
- **使用 Span<T> 和 Memory<T>**：避免不必要的内存复制
- **零拷贝操作**：减少数据复制次数
- **内存对齐**：提高内存访问效率

### 2. 网络优化

- **连接池**：重用 HTTP 和 TCP 连接
- **批量操作**：减少网络往返次数
- **压缩传输**：减少数据传输大小
- **超时设置**：避免长时间阻塞

### 3. 并行处理

- **异步操作**：使用 async/await 提高并发度
- **并行请求**：同时处理多个请求
- **管道并行**：使用管道的并行处理能力
- **通道并行**：使用通道的异步特性

### 4. 配置优化

- **合理设置缓冲区大小**：根据实际需求调整
- **设置适当的超时**：避免无限等待
- **调整重试策略**：平衡可靠性和性能
- **优化线程池**：根据系统资源调整

## 部署指南

### 本地部署

1. **编译**

```bash
cd transport/scripts
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishAot=true
```

2. **运行**

```bash
./bin/Release/net10.0/win-x64/publish/transport_core.exe http --url "https://api.example.com/data"
```

### Docker 部署

**Dockerfile**:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY transport/scripts/transport_core.cs .
COPY transport/scripts/transport_core.setting.json .

RUN dotnet publish transport_core.cs -c Release -o /app/publish /p:PublishAot=true /p:SelfContained=true /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["./transport_core"]
```

**构建和运行**:

```bash
# 构建镜像
docker build -t transport-skill .

# 运行容器
docker run --rm transport-skill http --url "https://api.example.com/data"
```

### 云部署

#### Azure Functions

1. **创建函数应用**：选择 .NET 10 运行时
2. **部署代码**：使用 Azure DevOps 或 GitHub Actions
3. **配置环境变量**：设置必要的环境变量
4. **测试功能**：使用 Azure Functions 门户测试

#### AWS Lambda

1. **创建 Lambda 函数**：选择 .NET 运行时
2. **打包部署**：使用 Lambda 部署工具
3. **配置触发器**：设置 API Gateway 或其他触发器
4. **测试功能**：使用 AWS Lambda 控制台测试

#### Google Cloud Functions

1. **创建函数**：选择 .NET 运行时
2. **部署代码**：使用 gcloud 命令或 Cloud Console
3. **配置环境变量**：设置必要的环境变量
4. **测试功能**：使用 Cloud Functions 控制台测试

## 监控与日志

### 日志系统

Transport 技能使用 Microsoft.Extensions.Logging 提供日志功能：

- **日志级别**：Debug、Information、Warning、Error、Critical
- **日志输出**：控制台、文件、其他目标
- **结构化日志**：支持 JSON 格式

### 性能监控

Transport 技能提供以下性能监控功能：

- **性能指标**：通过 metricsPort 暴露
- **健康检查**：提供健康状态端点
- **资源使用**：监控 CPU、内存使用情况

### 诊断工具

- **网络诊断**：检查网络连接和延迟
- **内存诊断**：检查内存使用和泄漏
- **性能分析**：分析性能瓶颈

## 安全考虑

### 1. 输入验证

- **URL 验证**：验证 HTTP URL 的合法性
- **端口验证**：验证网络端口的有效性
- **数据验证**：验证传输数据的格式和大小
- **参数验证**：验证命令行参数

### 2. 异常处理

- **网络异常**：妥善处理网络连接失败
- **超时异常**：妥善处理操作超时
- **内存异常**：妥善处理内存不足
- **安全异常**：妥善处理安全相关异常

### 3. 资源管理

- **连接管理**：正确关闭网络连接
- **缓冲区管理**：正确归还缓冲区
- **线程管理**：避免线程泄漏
- **文件管理**：正确处理文件资源

### 4. 加密通信

- **HTTPS**：使用 TLS 加密 HTTP 通信
- **TCP 加密**：考虑使用 SSL/TLS 加密 TCP 通信
- **数据加密**：敏感数据传输前加密

## 扩展与自定义

### 1. 自定义传输服务

1. **实现接口**：继承相应的传输服务接口
2. **注册服务**：将自定义服务注册到依赖注入容器
3. **使用装饰器**：可以使用 Scrutor 装饰现有服务

### 2. 自定义命令

1. **继承 Command**：创建自定义命令类
2. **添加选项**：为命令添加必要的选项
3. **设置处理器**：实现命令处理逻辑
4. **注册命令**：将命令注册到命令行构建器

### 3. 自定义配置

1. **添加配置类**：创建配置类存储自定义设置
2. **绑定配置**：从环境变量或配置文件绑定配置
3. **使用配置**：在服务中使用配置

### 4. 自定义日志

1. **添加日志提供器**：实现自定义日志提供器
2. **配置日志**：设置日志级别和格式
3. **使用日志**：在代码中使用日志

## 故障排除

### 常见问题

#### 1. HTTP 请求失败

**症状**：HTTP 请求返回错误或超时

**可能原因**：
- URL 错误或不可访问
- 网络连接问题
- 服务器错误
- 超时设置过短

**解决方案**：
- 验证 URL 是否正确
- 检查网络连接
- 增加超时设置
- 检查服务器状态

#### 2. TCP 连接失败

**症状**：无法建立 TCP 连接或连接被拒绝

**可能原因**：
- 主机名或 IP 地址错误
- 端口号错误或未开放
- 防火墙阻止
- 服务器未运行

**解决方案**：
- 验证主机和端口
- 检查防火墙设置
- 确保服务器正在运行
- 使用 telnet 测试连接

#### 3. UDP 消息丢失

**症状**：发送的 UDP 消息未被接收

**可能原因**：
- 网络丢包
- 目标不可达
- 缓冲区溢出
- 防火墙阻止

**解决方案**：
- 检查网络连接
- 增加重试机制
- 调整缓冲区大小
- 检查防火墙设置

#### 4. 内存使用过高

**症状**：应用程序内存使用持续增长

**可能原因**：
- 缓冲区未归还
- 连接未关闭
- 资源泄漏
- 数据量过大

**解决方案**：
- 使用缓冲区池
- 正确关闭连接
- 检查资源释放
- 增加数据处理批次

#### 5. 性能下降

**症状**：操作响应时间变长

**可能原因**：
- 网络延迟增加
- 系统资源不足
- 并发连接过多
- 数据量过大

**解决方案**：
- 检查网络状况
- 增加系统资源
- 限制并发连接
- 优化数据处理

## 示例

### HTTP 传输示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddTransient<IHttpTransportService, HttpTransportService>();
        
        using var serviceProvider = services.BuildServiceProvider();
        var httpService = serviceProvider.GetRequiredService<IHttpTransportService>();
        
        var result = await httpService.SendRequestAsync("https://api.example.com/data");
        Console.WriteLine(result);
    }
}
```

### TCP 传输示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddTransient<ITcpTransportService, TcpTransportService>();
        
        using var serviceProvider = services.BuildServiceProvider();
        var tcpService = serviceProvider.GetRequiredService<ITcpTransportService>();
        
        // 启动服务器
        var cts = new CancellationTokenSource();
        var serverTask = tcpService.StartServerAsync(8080, async message => {
            Console.WriteLine($"收到消息: {message}");
            return $"已处理: {message}";
        }, cts.Token);
        
        // 发送消息
        var response = await tcpService.SendMessageAsync("localhost", 8080, "Hello TCP");
        Console.WriteLine($"服务器响应: {response}");
        
        // 停止服务器
        cts.Cancel();
        await serverTask;
    }
}
```

### 通道传输示例

```csharp
using System;
using System.Threading.Tasks;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddTransient<IChannelTransportService, ChannelTransportService>();
        
        using var serviceProvider = services.BuildServiceProvider();
        var channelService = serviceProvider.GetRequiredService<IChannelTransportService>();
        
        // 创建通道
        var channel = Channel.CreateBounded<string>(10);
        
        // 发送消息
        for (int i = 0; i < 5; i++)
        {
            await channelService.SendToChannelAsync(channel.Writer, $"消息 {i}