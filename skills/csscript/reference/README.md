# Csscript AOT - 参考文档

## 概述

Csscript AOT是基于.NET 10 AOT架构的高性能脚本执行工具，专为.NET开发者设计，提供高效、可靠的脚本执行功能。

## 核心组件

### 1. ICsscriptService 接口
- **位置**: scripts/csscript_aot.cs
- **功能**: 定义脚本执行的核心功能接口
- **方法**:
  - `ExecuteScriptAsync`: 执行内联脚本
  - `ExecuteScriptFileAsync`: 执行脚本文件
  - `ExecuteScriptBatchAsync`: 批量执行脚本
  - `CompileScriptAsync`: 编译脚本
  - `GetStatusAsync`: 获取服务状态
  - `ResetStatusAsync`: 重置服务状态

### 2. CsscriptService 实现
- **位置**: scripts/csscript_aot.cs
- **功能**: 实现ICsscriptService接口，提供脚本执行的核心功能
- **特性**:
  - 支持内联脚本和脚本文件执行
  - 内置缓存机制，提高重复脚本执行速度
  - 详细的日志记录
  - 异步编程模型
  - 错误处理和重试机制

### 3. CsscriptAotEngine 执行引擎
- **位置**: scripts/csscript_aot.cs
- **功能**: 管理脚本执行的执行引擎
- **特性**:
  - 封装ICsscriptService的调用
  - 提供更高级别的脚本执行接口
  - 支持命令行交互

### 4. CsscriptOptions 配置选项
- **位置**: scripts/csscript_aot.cs
- **功能**: 配置选项类，用于控制脚本执行的行为
- **配置项**:
  - EnableCache: 是否启用缓存
  - CacheSize: 缓存大小
  - Timeout: 操作超时时间
  - EnableDetailedLogging: 是否启用详细日志
  - WorkerCount: 工作线程数
  - RetryCount: 重试次数
  - RetryInterval: 重试间隔
  - ScriptExecutionTimeout: 脚本执行超时时间
  - AllowExternalScripts: 是否允许执行外部脚本
  - ScriptCacheDirectory: 脚本缓存目录

### 5. 结果类
- **位置**: scripts/csscript_aot.cs
- **类**: 
  - `CsscriptResult`: 脚本执行结果
  - `ScriptCompilationResult`: 脚本编译结果
  - `CsscriptStatus`: 服务状态信息

## 使用示例

### 基本用法

```csharp
// 获取Csscript AOT引擎
var engine = serviceProvider.GetRequiredService<Csscript.AOT.CsscriptAotEngine>();

// 创建脚本代码
string scriptCode = "Console.WriteLine(\"Hello from CSScript!"); return new { Result = "Success", Message = "脚本执行成功" };";

// 执行脚本
var result = await engine.ExecuteScriptAsync(scriptCode);
Console.WriteLine($"脚本执行结果: 成功={result.Success}, 时间={result.ExecutionTimeMs}ms");
```

### 高级配置

```csharp
// 从配置文件加载配置
builder.Configuration.AddJsonFile("csscript_aot.setting.json");
builder.Services.Configure<Csscript.AOT.CsscriptOptions>(builder.Configuration.GetSection("Csscript"));

// 或者直接在代码中配置
builder.Services.Configure<Csscript.AOT.CsscriptOptions>(options => {
    options.EnableCache = true;
    options.CacheSize = 2000;
    options.Timeout = TimeSpan.FromSeconds(60);
    options.EnableDetailedLogging = true;
    options.AllowExternalScripts = false;
    options.ScriptExecutionTimeout = TimeSpan.FromSeconds(60);
});
```

## 配置选项

### 配置文件格式

```json
{
  "Csscript": {
    "EnableCache": true,
    "CacheSize": 1000,
    "Timeout": "00:00:30",
    "EnableDetailedLogging": false,
    "WorkerCount": 4,
    "RetryCount": 3,
    "RetryInterval": "00:00:00.5",
    "ScriptExecutionTimeout": "00:01:00",
    "AllowExternalScripts": false,
    "ScriptCacheDirectory": "./script_cache"
  }
}
```

### 环境变量配置

Csscript AOT支持通过环境变量进行配置，环境变量名称为配置项的大写形式，前缀为`CSSCRIPT_`：

- `CSSCRIPT_ENABLE_CACHE`: 是否启用缓存
- `CSSCRIPT_CACHE_SIZE`: 缓存大小
- `CSSCRIPT_TIMEOUT`: 超时时间（毫秒）
- `CSSCRIPT_SCRIPT_EXECUTION_TIMEOUT`: 脚本执行超时时间（毫秒）
- `CSSCRIPT_ALLOW_EXTERNAL_SCRIPTS`: 是否允许执行外部脚本

## 性能优化

1. **启用AOT编译**: 启用AOT编译以获得最佳性能
2. **合理配置缓存**: 根据实际需求配置缓存大小和启用/禁用缓存
3. **使用异步API**: 优先使用异步API，提高并发性能
4. **合理配置脚本执行超时**: 根据脚本复杂度配置合适的超时时间
5. **禁用外部脚本**: 在生产环境中禁用外部脚本执行，提高安全性
6. **合理配置日志级别**: 根据实际需求配置日志级别，避免性能影响
7. **使用批量执行**: 对于多个脚本，使用批量执行提高效率
8. **优化脚本代码**: 优化脚本代码，提高执行效率
9. **合理配置工作线程数**: 根据CPU核心数配置合适的工作线程数

## 故障排除

### 常见问题

1. **脚本执行失败**
   - 检查脚本代码是否语法正确
   - 检查脚本执行超时时间是否足够
   - 查看日志信息，了解具体错误原因
   - 检查系统资源是否充足
   - 检查是否允许执行外部脚本（如果执行的是脚本文件）

2. **性能问题**
   - 启用缓存
   - 调整WorkerCount参数
   - 减少重试次数
   - 优化脚本代码
   - 减少单次处理的脚本数量
   - 考虑使用批量执行

3. **缓存命中率低**
   - 增加缓存大小
   - 确保相同的脚本代码多次调用
   - 确保EnableCache设置为true
   - 检查缓存键生成逻辑是否合理

4. **内存占用高**
   - 减少缓存大小
   - 禁用缓存
   - 减少工作线程数
   - 分批次处理大量脚本
   - 优化脚本代码，减少内存使用

5. **服务无法启动**
   - 检查配置文件是否正确
   - 检查依赖项是否安装正确
   - 查看日志信息，了解具体错误原因
   - 检查系统资源是否充足

## 扩展开发

### 自定义脚本执行器

```csharp
// 自定义Csscript服务实现
public class CustomCsscriptService : Csscript.AOT.CsscriptService
{
    public CustomCsscriptService(ILogger<CsscriptService> logger, IOptions<Csscript.AOT.CsscriptOptions> options)
        : base(logger, options)
    {
    }
    
    // 重写ExecuteScriptAsync方法，添加自定义脚本处理逻辑
    public override async Task<Csscript.AOT.CsscriptResult> ExecuteScriptAsync(string scriptCode, Dictionary<string, object>? parameters = null)
    {
        // 自定义脚本预处理逻辑
        if (scriptCode.Contains("custom-function"))
        {
            scriptCode = scriptCode.Replace("custom-function", "// 自定义函数替换\nvar customResult = \"Custom Function Executed\"; return new { Custom = customResult };");
        }
        
        // 调用基类方法执行脚本
        return await base.ExecuteScriptAsync(scriptCode, parameters);
    }
}

// 注册自定义服务
builder.Services.AddSingleton<Csscript.AOT.ICsscriptService, CustomCsscriptService>();
```

## 安全最佳实践

1. **禁用外部脚本执行**: 在生产环境中，建议将`AllowExternalScripts`设置为`false`，防止执行恶意脚本文件
2. **限制脚本执行时间**: 合理配置`ScriptExecutionTimeout`，防止脚本无限执行导致资源耗尽
3. **启用详细日志**: 在开发和测试环境中启用详细日志，便于调试和监控
4. **定期清理缓存**: 定期清理脚本缓存，防止内存占用过高
5. **使用沙箱环境**: 考虑在沙箱环境中执行脚本，进一步提高安全性
6. **验证脚本来源**: 如果必须执行外部脚本，验证脚本来源的合法性
7. **限制脚本权限**: 限制脚本的执行权限，防止访问敏感资源

## 部署建议

1. **使用AOT编译**: 生产环境建议使用AOT编译，获得最佳性能和启动速度
2. **配置环境变量**: 使用环境变量进行配置，便于在不同环境中部署
3. **监控服务状态**: 定期获取服务状态，监控系统运行情况
4. **配置日志轮换**: 配置日志轮换，防止日志文件过大
5. **使用容器化部署**: 考虑使用Docker等容器技术进行部署，便于管理和扩展
6. **配置资源限制**: 为服务配置适当的资源限制，防止资源耗尽
7. **启用监控**: 启用应用性能监控，便于发现和解决性能问题

## 版本兼容性

- **.NET版本**: .NET 10.0
- **操作系统**: Windows, Linux, macOS
- **架构**: x86, x64, Arm64

## 依赖项

| 依赖项 | 版本 | 用途 |
|-------|------|------|
| Microsoft.Extensions.DependencyInjection | 10.0.0 | 依赖注入容器 |
| Microsoft.Extensions.Logging | 10.0.0 | 日志记录 |
| Microsoft.Extensions.Hosting | 10.0.0 | 主机管理 |
| Microsoft.Extensions.Options | 10.0.0 | 配置选项 |
| Newtonsoft.Json | 13.0.3 | JSON序列化和反序列化 |

