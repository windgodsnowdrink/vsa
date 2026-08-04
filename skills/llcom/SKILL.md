# llcom 智能体技能 - llcom 技能

## 技能概述

基于 .NET 10 的高性能 llcom 技能，为 .NET 开发者提供强大的 llcom 功能。

## 快速开始指南

### 安装依赖

在主应用的运行文件中添加以下依赖：

`yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package LLCOM.SDK@11.0.0
#:package System.Threading.Channels@8.0.0
`

### 注册服务

在主应用中注册 llcom 服务：

`csharp
// 注册 llcom 服务
builder.Services.AddSingleton<ILLStreamProcessor, LLStreamProcessor>();
builder.Services.AddSingleton<ILLComService, LLComService>();
`

### 使用示例

`csharp
// 获取 llcom 服务
var llComService = serviceProvider.GetRequiredService<ILLComService>();
var device = await llComService.ConnectDeviceAsync();
await llComService.SendDataAsync(device.DeviceId, new byte[] { 0x01, 0x02, 0x03 });
`

## 导航地图

`
llcom/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? llcom_integration.cs     # llcom 核心实现
    ????? llcom_integration.run.json  # 运行配置
    ????? llcom_integration.setting.json  # 设置文件
`

## 主要功能

1. **串口通信**: 高性能串口通信，支持多种串口参数配置
2. **数据处理**: 高效的数据处理和解析，支持多种数据格式
3. **设备管理**: 设备检测、连接和管理
4. **高性能设计**: 基于 Threading.Channels 和 Span 零拷贝的优化实现
5. **易于使用的 API**: 简洁直观的 API 设计
6. **可扩展架构**: 支持自定义扩展

## 扩展说明

此技能提供完整的 llcom 解决方案，您可以根据需要进行扩展：

1. **自定义实现**: 实现 ILLComService 接口
2. **扩展功能**: 添加新的 llcom 功能
3. **与其他系统集成**: 与其他系统集成
4. **性能优化**: 针对特定场景优化性能

## 最佳实践

1. **依赖注入**: 使用依赖注入管理服务
2. **异步编程**: 优先使用异步 API 避免阻塞
3. **错误处理**: 正确处理异常情况
4. **日志记录**: 添加适当的日志记录
5. **性能监控**: 监控关键性能指标

## AOT 架构执行

### AOT 编译配置

```yaml
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
```

### 执行脚本

- **脚本路径**: scripts/llcom_integration.cs
- **运行配置**: scripts/llcom_integration.run.json
- **设置文件**: scripts/llcom_integration.setting.json

### 执行流程

1. **编译**: 使用 .NET 10 AOT 编译生成单文件可执行程序
2. **部署**: 将生成的可执行文件部署到目标环境
3. **运行**: 执行可执行文件，启动 llcom 服务
4. **集成**: 与主应用集成，使用 llcom 功能

## 技术特性

- **AOT 编译支持**: 使用 .NET 10 的 AOT 编译能力，生成高性能的单文件可执行程序
- **Channel 事件处理**: 使用 System.Threading.Channels 实现高效的事件队列处理
- **内存优化**: 使用 Span 零拷贝技术，减少内存分配
- **依赖注入集成**: 与 Microsoft.Extensions.DependencyInjection 无缝集成
- **对象池优化**: 使用 ObjectPool 减少对象创建开销
- **跨平台支持**: 支持 Windows、Linux、macOS 等多个平台
