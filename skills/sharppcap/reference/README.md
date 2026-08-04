# SharpPcap 技能技术参考文档

## 1. 架构说明

SharpPcap 技能采用模块化架构设计，基于 .NET 10 和 AOT 编译技术，提供高效的网络数据包捕获和分析能力。

### 1.1 核心组件

| 组件 | 描述 | 实现文件 |
|------|------|----------|
| 数据包捕获服务 | 负责从网络接口捕获数据包 | `sharppcap_core.cs` 中的 `PacketCaptureService` |
| 数据包分析服务 | 负责分析捕获的数据包 | `sharppcap_core.cs` 中的 `PacketAnalyzerService` |
| 数据包过滤服务 | 负责过滤数据包 | `sharppcap_core.cs` 中的 `PacketFilterService` |
| 网络统计信息服务 | 负责收集网络接口统计信息 | `sharppcap_core.cs` 中的 `NetworkStatsService` |
| 代码生成服务 | 负责生成网络协议解析器和分析器代码 | `sharppcap_generator.cs` 中的 `CodeGeneratorService` |
| 命令行工具 | 提供命令行界面，方便用户使用各种功能 | `sharppcap_core.cs` 中的 `SharpPcapCli` 和 `sharppcap_generator.cs` 中的 `CodeGeneratorCli` |

### 1.2 数据流

1. **捕获流程**：用户通过命令行工具或 API 指定网络接口、过滤器和输出文件，捕获服务开始捕获数据包并保存到文件。
2. **分析流程**：用户通过命令行工具或 API 指定输入文件和协议类型，分析服务读取文件并分析数据包。
3. **过滤流程**：用户通过命令行工具或 API 指定输入文件、过滤表达式和输出文件，过滤服务读取文件并过滤数据包。
4. **统计流程**：用户通过命令行工具或 API 指定网络接口和统计间隔，统计服务收集并展示网络接口统计信息。
5. **代码生成流程**：用户通过命令行工具或 API 指定生成类型和协议，代码生成服务生成相应的代码并保存到文件。

### 1.3 AOT 编译架构

SharpPcap 技能采用 AOT（Ahead-of-Time）编译技术，提供以下优势：

- **启动速度快**：预编译代码，减少运行时 JIT 编译开销
- **运行时性能高**：优化的机器代码执行效率更高
- **内存占用小**：通过裁剪未使用的代码，减少应用程序体积
- **部署简单**：生成单个可执行文件，无需安装 .NET 运行时

## 2. API 参考

### 2.1 IPacketCaptureService

```csharp
public interface IPacketCaptureService
{
    Task<IEnumerable<NetworkInterfaceInfo>> GetNetworkInterfacesAsync();
    Task StartCaptureAsync(string interfaceName, string filter, string outputPath, int durationSeconds);
    Task StopCaptureAsync();
}
```

#### 方法说明

- **GetNetworkInterfacesAsync**：获取所有可用的网络接口列表
  - 返回值：网络接口信息列表

- **StartCaptureAsync**：开始在指定网络接口上捕获数据包
  - **interfaceName**：网络接口名称
  - **filter**：数据包过滤器表达式（BPF 语法）
  - **outputPath**：输出文件路径
  - **durationSeconds**：捕获持续时间（秒）

- **StopCaptureAsync**：停止捕获数据包

### 2.2 IPacketAnalyzerService

```csharp
public interface IPacketAnalyzerService
{
    Task<PacketAnalysisResult> AnalyzeFileAsync(string filePath, string protocol);
    Task<PacketAnalysisResult> AnalyzePacketAsync(byte[] packetData);
}
```

#### 方法说明

- **AnalyzeFileAsync**：分析指定的数据包文件
  - **filePath**：输入文件路径
  - **protocol**：协议类型（可选）
  - 返回值：数据包分析结果

- **AnalyzePacketAsync**：分析单个数据包
  - **packetData**：数据包数据
  - 返回值：数据包分析结果

### 2.3 IPacketFilterService

```csharp
public interface IPacketFilterService
{
    Task FilterFileAsync(string inputPath, string filterExpression, string outputPath);
    Task<byte[]> FilterPacketAsync(byte[] packetData, string filterExpression);
}
```

#### 方法说明

- **FilterFileAsync**：过滤指定的数据包文件
  - **inputPath**：输入文件路径
  - **filterExpression**：过滤表达式
  - **outputPath**：输出文件路径

- **FilterPacketAsync**：过滤单个数据包
  - **packetData**：数据包数据
  - **filterExpression**：过滤表达式
  - 返回值：过滤后的数据包数据（如果匹配）或空数组（如果不匹配）

### 2.4 INetworkStatsService

```csharp
public interface INetworkStatsService
{
    Task<NetworkStats> GetStatsAsync(string interfaceName);
    Task StartStatsMonitoringAsync(string interfaceName, int intervalSeconds, Action<NetworkStats> callback);
    Task StopStatsMonitoringAsync();
}
```

#### 方法说明

- **GetStatsAsync**：获取指定网络接口的统计信息
  - **interfaceName**：网络接口名称
  - 返回值：网络统计信息

- **StartStatsMonitoringAsync**：开始监控指定网络接口的统计信息
  - **interfaceName**：网络接口名称
  - **intervalSeconds**：统计间隔（秒）
  - **callback**：回调函数，用于处理统计信息

- **StopStatsMonitoringAsync**：停止监控网络接口的统计信息

### 2.5 ICodeGeneratorService

```csharp
public interface ICodeGeneratorService
{
    Task<string> GenerateParserAsync(string protocol);
    Task<string> GenerateAnalyzerAsync(string protocol);
    Task SaveGeneratedCodeAsync(string code, string outputPath);
}
```

#### 方法说明

- **GenerateParserAsync**：生成指定协议的解析器代码
  - **protocol**：协议类型
  - 返回值：生成的代码

- **GenerateAnalyzerAsync**：生成指定协议的分析器代码
  - **protocol**：协议类型
  - 返回值：生成的代码

- **SaveGeneratedCodeAsync**：保存生成的代码到文件
  - **code**：生成的代码
  - **outputPath**：输出文件路径

## 3. 配置选项

### 3.1 编译配置（.setting.json）

```json
{
  "compilationOptions": {
    "targetFramework": "net10.0",
    "langVersion": "preview",
    "nullable": true,
    "implicitUsings": true
  },
  "publishOptions": {
    "publishAot": true,
    "trimMode": "partial",
    "selfContained": true,
    "publishSingleFile": true,
    "runtimeIdentifier": "win-x64"
  },
  "dependencies": {
    "SharpPcap": "6.3.0",
    "PacketDotNet": "1.4.7",
    "Microsoft.Extensions.DependencyInjection": "10.0.0",
    "Microsoft.Extensions.Logging": "10.0.0",
    "System.CommandLine": "2.0.0",
    "System.Text.Json": "10.0.0"
  }
}
```

### 3.2 运行配置（.run.json）

```json
{
  "profiles": [
    {
      "name": "List Interfaces",
      "commandName": "Project",
      "commandLineArgs": "interfaces",
      "workingDirectory": "$(ProjectDir)",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    },
    {
      "name": "Capture Packets",
      "commandName": "Project",
      "commandLineArgs": "capture --interface {interface} --filter "tcp port 80" --output capture.pcap --duration 60",
      "workingDirectory": "$(ProjectDir)",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    }
  ]
}
```

### 3.3 命令行选项

#### 3.3.1 捕获命令

```bash
sharppcap_core capture --interface <interface> --filter <filter> --output <output> --duration <duration>
```

- **--interface**：网络接口名称
- **--filter**：数据包过滤器表达式（BPF 语法）
- **--output**：输出文件路径
- **--duration**：捕获持续时间（秒）

#### 3.3.2 分析命令

```bash
sharppcap_core analyze --input <input> --protocol <protocol>
```

- **--input**：输入文件路径
- **--protocol**：协议类型（可选）

#### 3.3.3 过滤命令

```bash
sharppcap_core filter --input <input> --expression <expression> --output <output>
```

- **--input**：输入文件路径
- **--expression**：过滤表达式
- **--output**：输出文件路径

#### 3.3.4 统计命令

```bash
sharppcap_core stats --interface <interface> --interval <interval>
```

- **--interface**：网络接口名称
- **--interval**：统计间隔（秒）

#### 3.3.5 接口命令

```bash
sharppcap_core interfaces
```

- 列出所有可用的网络接口

#### 3.3.6 代码生成命令

```bash
sharppcap_generator generate --type <type> --protocol <protocol> --output <output>
```

- **--type**：生成类型（parser/analyzer）
- **--protocol**：协议类型
- **--output**：输出文件路径

## 4. 依赖项

| 依赖项 | 版本 | 用途 | 来源 |
|--------|------|------|------|
| SharpPcap | 6.3.0 | 网络数据包捕获库 | NuGet |
| PacketDotNet | 1.4.7 | 网络数据包分析库 | NuGet |
| Microsoft.Extensions.DependencyInjection | 10.0.0 | 依赖注入框架 | NuGet |
| Microsoft.Extensions.Logging | 10.0.0 | 日志记录框架 | NuGet |
| System.CommandLine | 2.0.0 | 命令行解析库 | NuGet |
| System.Text.Json | 10.0.0 | JSON 序列化库 | NuGet |

## 5. 性能优化

### 5.1 捕获性能优化

1. **使用过滤器**：合理使用 BPF 过滤器表达式，减少需要处理的数据包数量
2. **调整缓冲区大小**：根据网络流量调整捕获缓冲区大小，避免丢包
3. **使用文件缓冲**：对于大量数据，使用文件缓冲减少内存使用
4. **多线程处理**：对于高速网络，考虑使用多线程处理数据包

### 5.2 分析性能优化

1. **增量分析**：对于大型捕获文件，使用增量分析减少内存使用
2. **并行分析**：对于多个文件，使用并行分析提高速度
3. **缓存结果**：对于重复分析，缓存结果减少计算开销

### 5.3 内存优化

1. **使用 Span<T>**：对于数据包处理，使用 Span<T> 减少内存分配
2. **对象池**：使用对象池重用对象，减少 GC 压力
3. **流式处理**：对于大型文件，使用流式处理减少内存使用

## 6. 扩展指南

### 6.1 扩展协议支持

1. **实现自定义解析器**：继承 `BaseParser` 类，实现特定协议的解析逻辑
2. **实现自定义分析器**：继承 `BaseAnalyzer` 类，实现特定协议的分析逻辑
3. **注册解析器和分析器**：在服务容器中注册自定义解析器和分析器

### 6.2 扩展命令行功能

1. **添加新命令**：在 `SharpPcapCli` 类中添加新的命令
2. **添加新选项**：为现有命令添加新的选项
3. **自定义输出格式**：实现自定义的输出格式，如 JSON、XML 等

### 6.3 集成其他系统

1. **与监控系统集成**：将网络统计信息发送到监控系统
2. **与安全系统集成**：将异常数据包信息发送到安全系统
3. **与存储系统集成**：将捕获的数据包存储到分布式存储系统

## 7. 故障排除

### 7.1 常见问题

| 问题 | 可能原因 | 解决方案 |
|------|---------|----------|
| 网络接口不可用 | 接口不存在或权限不足 | 检查接口名称是否正确，以管理员权限运行 |
| 过滤器表达式错误 | BPF 语法错误 | 检查过滤器表达式语法是否正确 |
| 文件写入失败 | 路径不存在或权限不足 | 检查输出路径是否存在，是否有写入权限 |
| 内存占用过高 | 数据包数量过多 | 使用过滤器减少数据包数量，或使用流式处理 |
| 性能问题 | 网络流量过大 | 调整捕获缓冲区大小，使用更精确的过滤器 |
| 权限不足 | 缺少管理员权限 | 以管理员权限运行应用程序 |
| 兼容性问题 | SharpPcap 版本与操作系统不兼容 | 安装与操作系统兼容的 SharpPcap 版本 |

### 7.2 日志记录

SharpPcap 技能使用 Microsoft.Extensions.Logging 框架进行日志记录，可通过配置日志级别来控制日志输出：

```csharp
builder.Logging.AddConsole(options =>
{
    options.LogLevel = LogLevel.Information;
});
```

### 7.3 调试技巧

1. **启用详细日志**：将日志级别设置为 Debug 或 Trace，查看详细的执行过程
2. **使用网络工具**：使用 Wireshark 等网络工具验证捕获结果
3. **测试过滤器**：使用简单的过滤器表达式测试捕获功能
4. **检查系统资源**：监控 CPU、内存和磁盘使用情况，确保系统资源充足

## 8. 安全注意事项

### 8.1 权限管理

1. **以非管理员权限运行**：在可能的情况下，以非管理员权限运行应用程序
2. **最小权限原则**：只授予应用程序必要的权限
3. **权限验证**：在执行需要权限的操作前，验证用户权限

### 8.2 数据安全

1. **敏感数据处理**：对于包含敏感信息的数据包，采取适当的保护措施
2. **数据存储**：加密存储捕获的数据包，避免敏感信息泄露
3. **数据传输**：在传输捕获的数据包时，使用加密通道

### 8.3 网络安全

1. **过滤器使用**：使用过滤器避免捕获敏感信息
2. **捕获限制**：限制捕获持续时间和数据包数量，避免网络拥塞
3. **异常检测**：实现异常检测，及时发现和处理异常网络行为

## 9. 部署指南

### 9.1 构建和发布

1. **构建项目**：使用 .NET 10 SDK 构建项目
   ```bash
   dotnet build -c Release
   ```

2. **发布项目**：使用 AOT 编译发布项目
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
   ```

### 9.2 部署选项

1. **独立部署**：将发布的可执行文件复制到目标机器上直接运行
2. **容器部署**：将应用程序打包到容器中运行
3. **服务部署**：将应用程序注册为系统服务，实现自动启动

### 9.3 配置管理

1. **环境变量**：使用环境变量配置应用程序
2. **配置文件**：使用 JSON 或 XML 配置文件配置应用程序
3. **命令行参数**：通过命令行参数覆盖默认配置

## 10. 版本历史

| 版本 | 日期 | 变更内容 |
|------|------|----------|
| 1.0.0 | 2026-01-01 | 初始版本，实现基本的网络数据包捕获、分析、过滤和统计功能 |

## 11. 参考资料

1. **SharpPcap 文档**：https://github.com/dotpcap/sharppcap/wiki
2. **PacketDotNet 文档**：https://github.com/dotpcap/packetnet/wiki
3. **BPF 过滤器语法**：https://www.tcpdump.org/manpages/pcap-filter.7.html
4. **.NET 10 文档**：https://docs.microsoft.com/dotnet/
5. **AOT 编译文档**：https://docs.microsoft.com/dotnet/core/deploying/native-aot

## 12. 联系方式

如有问题或建议，请联系：

- 邮箱：support@sharppcapskill.com
- GitHub：https://github.com/sharppcapskill
- 社区：https://community.sharppcapskill.com
