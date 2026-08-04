# mailkit 智能体技能 - mailkit 技能

## 技能概述

基于 .NET 10 的高性能 mailkit 技能，为 .NET 开发者提供强大的邮件处理功能。

## 快速开始指南

### 安装依赖

在主应用的运行文件中添加以下依赖：

`yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package MailKit@4.0.0
#:package MimeKit@4.0.0
#:package System.Threading.Channels@8.0.0
`

### 注册服务

在主应用中注册 mailkit 服务：

`csharp
// 注册 mailkit 服务
builder.Services.AddSingleton<IMailKitProcessor, MailKitProcessor>();
builder.Services.AddSingleton<IMailKitService, MailKitService>();
`

### 使用示例

`csharp
// 获取 mailkit 服务
var mailKitService = serviceProvider.GetRequiredService<IMailKitService>();
var sendResult = await mailKitService.SendEmailAsync("to@example.com", "测试邮件", "邮件内容");
Console.WriteLine($"发送成功: {sendResult.Success}");
`

## AOT 架构执行

### AOT 编译配置

```yaml
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property RuntimeIdentifier=linux-x64
#:property RuntimeIdentifier=osx-x64
```

### Channel 事件处理

mailkit 技能使用 Threading.Channels 进行高效的异步邮件队列处理，支持背压控制：

```csharp
// 创建邮件处理通道
var emailChannel = Channel.CreateBounded<EmailMessage>(
    new BoundedChannelOptions(1000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true,
        FullMode = BoundedChannelFullMode.DropOldest
    });

// 处理邮件
async Task ProcessEmailsAsync()
{
    await foreach (var email in emailChannel.Reader.ReadAllAsync())
    {
        await SendEmailAsync(email);
    }
}

// 启动邮件处理任务
_ = Task.Run(ProcessEmailsAsync);
```

### 执行脚本和工作流

mailkit 技能提供了多个执行脚本，支持不同场景的使用：

1. **基础集成**：`mailkit_integration.cs` - 提供基本的邮件发送和接收功能
2. **Papercut 集成**：`papercut_integration.cs` - 提供与 Papercut 本地邮件服务器的集成功能

## 导航地图

`
mailkit/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? mailkit_integration.cs     # mailkit 核心实现
    ????? mailkit_integration.run.json  # 运行配置
    ????? mailkit_integration.setting.json  # 设置文件
    ????? papercut_integration.cs  # Papercut 集成实现
    ????? papercut_integration.run.json  # Papercut 运行配置
    ????? papercut_integration.setting.json  # Papercut 设置文件
`

## 主要功能

1. **核心功能 1**：邮件发送和接收
2. **核心功能 2**：邮件附件处理
3. **核心功能 3**：邮件模板支持
4. **高性能设计**：优化的性能实现，使用 Threading.Channels 和对象池
5. **易用 API**：简单直观的 API 设计
6. **可扩展架构**：支持自定义扩展

## 扩展说明

本技能提供了完整的 mailkit 解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现 IMailKitProcessor 接口
2. **扩展功能**：添加新的邮件处理功能
3. **系统集成**：与其他系统集成
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **AOT 编译**：使用 AOT 编译提升启动速度和运行性能
7. **内存优化**：使用对象池和 Span 零拷贝技术优化内存使用
