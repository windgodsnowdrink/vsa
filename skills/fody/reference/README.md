# Fody - 参考文档

## 概述

Fody 是基于 .NET 10 AOT 编译的高性能代码织入系统，专为 .NET 开发者设计。

## 核心组件

### 1. IFodyService（Fody 服务接口）
- **位置**: scripts/fody_aot.cs
- **功能**: 定义 Fody 服务的核心业务逻辑接口
- **特性**: 
  - 异步方法支持
  - 完整的织入功能
  - 统一的命令执行接口
  - 详细的结果返回

### 2. FodyService（Fody 服务实现）
- **位置**: scripts/fody_aot.cs
- **功能**: 实现 Fody 服务的核心业务逻辑
- **特性**: 
  - 完整的程序集织入实现
  - 织入器管理支持
  - 详细的错误处理
  - 性能监控

### 3. FodyAotEngine（命令行引擎）
- **位置**: scripts/fody_aot.cs
- **功能**: 处理命令行参数和执行织入操作
- **特性**: 
  - 支持多种命令别名
  - 详细的帮助信息
  - 命令行参数解析
  - 结果格式化输出

## 使用示例

### 基本用法

`csharp
// 获取 Fody 服务
var fodyService = serviceProvider.GetRequiredService<IFodyService>();

// 织入程序集
var weaveResult = await fodyService.WeaveAssemblyAsync("MyAssembly.dll", "Output.dll");
Console.WriteLine($"织入结果: {(weaveResult.Success ? "成功" : "失败")}");
Console.WriteLine($"执行时间: {weaveResult.ExecutionTimeMs} ms");

// 列出可用的织入器
var weaversResult = await fodyService.ListWeaversAsync();
Console.WriteLine("\n可用的织入器:");
foreach (var item in weaversResult.Results)
{
    Console.WriteLine($"- {item}");
}

// 获取版本信息
var versionResult = await fodyService.GetVersionInfoAsync();
Console.WriteLine("\n版本信息:");
foreach (var item in versionResult.Results)
{
    Console.WriteLine($"- {item}");
}
`

### 高级配置

`csharp
// 配置 Fody 选项
var fodyOptions = new FodyOptions
{
    WorkingDirectory = Environment.CurrentDirectory,
    EnableDetailedLogging = true,
    EnablePerformanceMonitoring = true,
    RequestTimeoutMs = 60000,
    EnableCache = true,
    CacheSize = 2000
};

builder.Services.Configure<FodyOptions>(options => {
    options.WorkingDirectory = fodyOptions.WorkingDirectory;
    options.EnableDetailedLogging = fodyOptions.EnableDetailedLogging;
    options.EnablePerformanceMonitoring = fodyOptions.EnablePerformanceMonitoring;
    options.RequestTimeoutMs = fodyOptions.RequestTimeoutMs;
    options.EnableCache = fodyOptions.EnableCache;
    options.CacheSize = fodyOptions.CacheSize;
});
`

## 配置选项

### Fody 配置

`json
{
  "Fody": {
    "WorkingDirectory": "",          // 工作目录
    "EnableDetailedLogging": false,    // 是否启用详细日志
    "EnablePerformanceMonitoring": true, // 是否启用性能监控
    "RequestTimeoutMs": 30000,        // 请求超时时间（毫秒）
    "EnableCache": true,              // 是否启用缓存
    "CacheSize": 1000                // 缓存大小
  }
}
`

## 性能优化

1. **AOT 编译**: 提前编译为本地代码，减少启动时间和内存占用
2. **异步编程**: 使用异步 API 避免阻塞
3. **缓存使用**: 启用缓存以提高性能，特别是对于重复织入的程序集
4. **批量处理**: 对于多个程序集的织入，考虑批处理以提高效率
5. **内存管理**: 优化内存使用，减少 GC 压力

## 故障排除

### 常见问题

1. **程序集织入失败**
   - 检查程序集文件是否存在
   - 验证程序集是否为有效的 .NET 程序集
   - 检查输出目录权限
   - 查看详细的错误信息

2. **性能问题**
   - 启用 AOT 编译
   - 启用缓存
   - 优化织入器配置
   - 监控系统资源使用情况

3. **命令行参数错误**
   - 检查命令格式是否正确
   - 验证参数数量是否足够
   - 查看帮助信息获取正确的命令格式

## 扩展开发

### 添加自定义织入器

`csharp
// 自定义织入器示例
public class CustomWeaver
{
    public void Weave(AssemblyDefinition assembly)
    {
        // 实现自定义织入逻辑
        // 1. 遍历程序集类型
        // 2. 查找需要织入的类型或方法
        // 3. 修改 IL 代码
        // 4. 保存修改
    }
}

// 集成自定义织入器到 Fody 服务
public class ExtendedFodyService : FodyService
{
    public ExtendedFodyService(IOptions<FodyOptions> options, ILogger<FodyService> logger)
        : base(options, logger)
    {}

    public async Task<FodyCommandResult> WeaveWithCustomWeaverAsync(string assemblyPath, string outputPath)
    {
        var result = new FodyCommandResult();
        
        try
        {
            // 使用 Mono.Cecil 读取程序集
            using var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath);
            
            // 使用自定义织入器
            var customWeaver = new CustomWeaver();
            customWeaver.Weave(assemblyDefinition);
            
            // 保存修改后的程序集
            assemblyDefinition.Write(outputPath);
            
            result.Success = true;
            result.Results.Add($"成功使用自定义织入器织入程序集");
            result.Results.Add($"输入: {assemblyPath}");
            result.Results.Add($"输出: {outputPath}");
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        
        return result;
    }
}
`

### 注册扩展服务

`csharp
// 注册扩展 Fody 服务
builder.Services.Configure<FodyOptions>(builder.Configuration.GetSection("Fody"));
builder.Services.AddSingleton<IFodyService, ExtendedFodyService>();
builder.Services.AddSingleton<FodyAotEngine>();
`
