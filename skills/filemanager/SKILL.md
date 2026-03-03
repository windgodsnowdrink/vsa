# FileManager Agent Skill - FileManager 技能

## 技能概述

基于 .NET 10 AOT 编译的高性能文件管理技能，为 .NET 开发者提供强大的文件管理功能。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

`yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
`

### 注册服务

在主应用程序中注册 FileManager 服务：

`csharp
// 注册 FileManager 服务
builder.Services.Configure<FileManagerOptions>(builder.Configuration.GetSection("FileManager"));
builder.Services.AddSingleton<IFileManagerService, FileManagerService>();
builder.Services.AddSingleton<FileManagerAotEngine>();
`

### 使用示例

`csharp
// 获取 FileManager 服务
var fileManagerService = serviceProvider.GetRequiredService<IFileManagerService>();

// 使用 FileManager 功能
var result = await fileManagerService.CreateFileAsync("test.txt", "Hello World");
Console.WriteLine($"创建文件结果: {(result.Success ? "成功" : "失败")}");
Console.WriteLine($"执行时间: {result.ExecutionTimeMs} ms");
`

## 导航地图

`
filemanager/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? filemanager_aot.cs     # FileManager AOT 核心实现
    ????? filemanager_aot.run.json  # 运行配置
    ????? filemanager_aot.setting.json  # 设置文件
    ????? downloader_integration.cs  # 下载器集成
    ????? file_versioning.cs  # 文件版本控制
    ????? fluentftp_integration.cs  # FTP 集成
    ????? transactional_filemgr_integration.cs  # 事务性文件管理集成
    ????? uploadstream_integration.cs  # 上传流集成
    ????? webdown_integration.cs  # Web 下载集成
`

## 主要功能

1. **文件操作**：创建、读取、写入、删除、列出、复制、移动文件
2. **文件版本控制**：支持文件历史版本管理
3. **事务支持**：可选的事务性操作
4. **高性能设计**：AOT 编译优化，减少启动时间和内存占用
5. **易用的命令行接口**：支持多种命令别名，方便使用
6. **详细的错误处理**：完善的错误捕获和日志记录
7. **可配置的选项**：通过 Options 模式支持灵活配置
8. **依赖注入**：基于 Microsoft.Extensions.DependencyInjection 的服务管理

## 扩展说明

此技能提供了完整的文件管理解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现 IFileManagerService 接口
2. **扩展功能**：添加新的文件管理功能
3. **与其他系统集成**：与其他系统集成
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **AOT 优化**：利用 AOT 编译提高性能
7. **内存管理**：注意内存使用，避免内存泄漏
