# LLM - 参考文档

## 概述

LLM 是一个基于 .NET 10 的高性能 LLM 系统，专为 .NET 开发者设计。

## 核心组件

### 1. LLMProcessor（LLM 处理器）
- **位置**: scripts/llm_integration.cs
- **功能**: 处理 LLM 模型推理
- **特性**: 
  - 高性能模型推理
  - Threading.Channels 事件处理
  - Span 零拷贝优化
  - 模型提供商集成

### 2. LLMService（LLM 服务）
- **位置**: scripts/llm_integration.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 语义记忆管理
  - 提示工程
  - 错误处理
  - 日志记录

## 使用示例

### 基本使用

`csharp
var llmService = serviceProvider.GetRequiredService<ILLMService>();
var response = await llmService.GenerateResponseAsync("Hello, LLM!");
Console.WriteLine($"LLM 响应: {response}");
`

### 高级配置

`csharp
var settings = new LLMSetting {
    EnableMemory = true,
    MaxMemorySize = 1000,
    Timeout = TimeSpan.FromSeconds(60),
    ModelName = "gpt-4"
};

builder.Services.Configure<LLMSetting>(options => {
    options.EnableMemory = settings.EnableMemory;
    options.MaxMemorySize = settings.MaxMemorySize;
    options.Timeout = settings.Timeout;
    options.ModelName = settings.ModelName;
});
`

## 配置选项

### LLM 配置

`json
{
  "LLMSetting": {
    "EnableMemory": true,          // 启用语义记忆
    "MaxMemorySize": 1000,            // 最大记忆大小
    "Timeout": "00:01:00",        // 超时时间
    "ModelName": "gpt-4",        // 模型名称
    "EnableDetailedLogging": false          // 启用详细日志
  }
}
`

## 性能优化

1. **缓存使用**: 启用缓存以提高性能
2. **异步编程**: 使用异步 API 避免阻塞
3. **批处理**: 批量处理以提高效率
4. **连接池**: 使用连接池管理资源
5. **Channel 事件处理**: 使用 Threading.Channels 实现高效的事件队列
6. **Span 零拷贝**: 使用 Span 减少内存分配和复制
7. **对象池**: 使用 ObjectPool 减少对象创建开销

## AOT 编译配置

### 构建配置

```yaml
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.AI@10.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
```

### 发布命令

```bash
# AOT 发布命令
dotnet publish scripts/llm_integration.cs -c Release -r win-x64 --aot

# Linux 发布命令
dotnet publish scripts/llm_integration.cs -c Release -r linux-x64 --aot

# macOS 发布命令
dotnet publish scripts/llm_integration.cs -c Release -r osx-x64 --aot
```

## 故障排除

### 常见问题

1. **连接失败**
   - 检查配置文件
   - 验证网络连接
   - 检查日志信息

2. **性能问题**
   - 启用缓存
   - 优化提示模板
   - 增加资源限制
   - 检查 Channel 配置

3. **内存问题**
   - 调整记忆大小
   - 优化内存使用
   - 检查 Span 使用

## 扩展开发

### 添加自定义功能

`csharp
public class CustomLLMService : ILLMService
{
    public async Task<string> GenerateResponseAsync(string prompt)
    {
        // 实现自定义逻辑
        return $"Custom response to: {prompt}";
    }
    
    public async Task<string> GenerateResponseAsync(string prompt, IEnumerable<Message> chatHistory)
    {
        // 实现自定义逻辑
        return $"Custom response with history: {prompt}";
    }
}
`

### 扩展处理器

`csharp
public class CustomLLMProcessor : ILLMProcessor
{
    public async Task<string> ProcessAsync(string prompt, LLMSetting settings)
    {
        // 实现自定义模型推理逻辑
        return $"Processed response: {prompt}";
    }
}
`
