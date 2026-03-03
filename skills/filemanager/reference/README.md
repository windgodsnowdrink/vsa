# FileManager - 参考文档

## 概述

FileManager 是基于 .NET 10 AOT 编译的高性能文件管理系统，专为 .NET 开发者设计。

## 核心组件

### 1. IFileManagerService（文件管理服务接口）
- **位置**: scripts/filemanager_aot.cs
- **功能**: 定义文件管理的核心业务逻辑接口
- **特性**: 
  - 异步方法支持
  - 完整的文件操作功能
  - 统一的命令执行接口
  - 详细的结果返回

### 2. FileManagerService（文件管理服务实现）
- **位置**: scripts/filemanager_aot.cs
- **功能**: 实现文件管理的核心业务逻辑
- **特性**: 
  - 完整的文件操作实现
  - 文件版本控制支持
  - 详细的错误处理
  - 性能监控

### 3. FileManagerAotEngine（命令行引擎）
- **位置**: scripts/filemanager_aot.cs
- **功能**: 处理命令行参数和执行文件管理操作
- **特性**: 
  - 支持多种命令别名
  - 详细的帮助信息
  - 命令行参数解析
  - 结果格式化输出

## 使用示例

### 基本用法

`csharp
// 获取 FileManager 服务
var fileManagerService = serviceProvider.GetRequiredService<IFileManagerService>();

// 创建文件
var createResult = await fileManagerService.CreateFileAsync("test.txt", "Hello World");
Console.WriteLine($"创建文件结果: {(createResult.Success ? "成功" : "失败")}");

// 读取文件
var readResult = await fileManagerService.ReadFileAsync("test.txt");
Console.WriteLine($"读取文件结果: {(readResult.Success ? "成功" : "失败")}");
if (readResult.Success && readResult.Results.Count > 0)
{
    Console.WriteLine($"文件内容: {readResult.Results[2]}");
}

// 写入文件
var writeResult = await fileManagerService.WriteFileAsync("test.txt", "Updated content");
Console.WriteLine($"写入文件结果: {(writeResult.Success ? "成功" : "失败")}");

// 删除文件
var deleteResult = await fileManagerService.DeleteFileAsync("test.txt");
Console.WriteLine($"删除文件结果: {(deleteResult.Success ? "成功" : "失败")}");
`

### 高级配置

`csharp
// 配置 FileManager 选项
var fileManagerOptions = new FileManagerOptions
{
    WorkingDirectory = Environment.CurrentDirectory,
    EnableFileMonitoring = false,
    EnableVersioning = true,
    VersioningDirectory = ".versions",
    MaxVersions = 10,
    EnableTransactions = false,
    RequestTimeoutMs = 30000,
    EnableDetailedLogging = false,
    EnablePerformanceMonitoring = true
};

builder.Services.Configure<FileManagerOptions>(options => {
    options.WorkingDirectory = fileManagerOptions.WorkingDirectory;
    options.EnableFileMonitoring = fileManagerOptions.EnableFileMonitoring;
    options.EnableVersioning = fileManagerOptions.EnableVersioning;
    options.VersioningDirectory = fileManagerOptions.VersioningDirectory;
    options.MaxVersions = fileManagerOptions.MaxVersions;
    options.EnableTransactions = fileManagerOptions.EnableTransactions;
    options.RequestTimeoutMs = fileManagerOptions.RequestTimeoutMs;
    options.EnableDetailedLogging = fileManagerOptions.EnableDetailedLogging;
    options.EnablePerformanceMonitoring = fileManagerOptions.EnablePerformanceMonitoring;
});
`

## 配置选项

### FileManager 配置

`json
{
  "FileManager": {
    "WorkingDirectory": "",          // 工作目录
    "EnableFileMonitoring": false,    // 是否启用文件监控
    "EnableVersioning": false,        // 是否启用文件版本控制
    "VersioningDirectory": ".versions", // 版本控制目录
    "MaxVersions": 10,               // 最大版本数量
    "EnableTransactions": false,      // 是否启用事务
    "RequestTimeoutMs": 30000,        // 请求超时时间（毫秒）
    "EnableDetailedLogging": false,   // 是否启用详细日志
    "EnablePerformanceMonitoring": true // 是否启用性能监控
  }
}
`

## 性能优化

1. **AOT 编译**: 提前编译为本地代码，减少启动时间和内存占用
2. **异步编程**: 使用异步 API 避免阻塞
3. **内存管理**: 优化内存使用，减少 GC 压力
4. **批处理**: 对于多个文件操作，考虑批处理以提高效率
5. **版本控制**: 合理配置版本控制，避免过多版本占用磁盘空间

## 故障排除

### 常见问题

1. **文件操作失败**
   - 检查文件路径是否正确
   - 验证文件权限
   - 检查磁盘空间
   - 查看日志信息

2. **性能问题**
   - 启用 AOT 编译
   - 优化文件操作批量处理
   - 合理配置版本控制
   - 监控系统资源使用情况

3. **版本控制问题**
   - 检查版本目录权限
   - 确保磁盘空间充足
   - 合理设置最大版本数量

## 扩展开发

### 添加自定义功能

`csharp
public class CustomFileManagerService : IFileManagerService
{
    private readonly FileManagerOptions _options;
    private readonly ILogger<CustomFileManagerService> _logger;

    public CustomFileManagerService(IOptions<FileManagerOptions> options, ILogger<CustomFileManagerService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<FileManagerCommandResult> ExecuteCommandAsync(FileManagerCommandType commandType, Dictionary<string, string>? parameters = null)
    {
        // 实现自定义命令执行逻辑
        throw new NotImplementedException();
    }

    public async Task<FileManagerCommandResult> CreateFileAsync(string filePath, string content)
    {
        // 实现自定义创建文件逻辑
        throw new NotImplementedException();
    }

    // 实现其他接口方法...
}
`

### 注册自定义服务

`csharp
// 注册自定义 FileManager 服务
builder.Services.Configure<FileManagerOptions>(builder.Configuration.GetSection("FileManager"));
builder.Services.AddSingleton<IFileManagerService, CustomFileManagerService>();
builder.Services.AddSingleton<FileManagerAotEngine>();
`
