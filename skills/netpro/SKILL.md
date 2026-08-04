# NetPro Agent Skill - NetPro 技能

## 技能概览

基于 .NET 10 的高性能 NetPro 技能，为 .NET 开发者提供强大的插件系统和扩展功能。

## 快速入门指南

### 安装依赖

在主应用的运行文件中添加以下依赖：

`yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.FileProviders@10.0.0
`

### 注册服务

在主应用中注册 NetPro 服务：

`csharp
// 注册 NetPro 服务
builder.Services.Configure<NetProOptions>(options => {
    options.EnablePlugins = true;
    options.PluginsPath = "plugins";
    options.EnableHotReload = true;
    options.EnableCaching = true;
});
builder.Services.AddSingleton<INetProService, NetProService>();
builder.Services.AddSingleton<INetProPluginManager, NetProPluginManager>();
builder.Services.AddSingleton<INetProExtensionManager, NetProExtensionManager>();
`

### 使用示例

`csharp
// 获取 NetPro 服务
var netProService = serviceProvider.GetRequiredService<INetProService>();
var pluginManager = serviceProvider.GetRequiredService<INetProPluginManager>();
var extensionManager = serviceProvider.GetRequiredService<INetProExtensionManager>();

// 加载插件
await pluginManager.LoadPluginsAsync();
Console.WriteLine($"已加载 {pluginManager.GetLoadedPlugins().Count} 个插件");

// 使用插件功能
foreach (var plugin in pluginManager.GetLoadedPlugins())
{
    Console.WriteLine($"插件: {plugin.Name}, 版本: {plugin.Version}");
    var result = await plugin.ExecuteAsync("Hello from NetPro!");
    Console.WriteLine($"插件执行结果: {result}");
}

// 使用扩展功能
var extensionResult = await extensionManager.ExecuteExtensionAsync("SampleExtension", new ExtensionContext { Data = "Test data" });
Console.WriteLine($"扩展执行结果: {extensionResult}");

// 使用核心功能
var coreResult = await netProService.DoSomethingAsync();
Console.WriteLine($"核心功能结果: {coreResult}");
`

## 导航地图

`
netpro/
├── index.yaml                   # 元数据索引说明
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能说明
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── netpro_plugin_service.cs     # NetPro 核心实现
    ├── netpro_plugin_service.run.json  # 运行配置
    └── netpro_plugin_service.setting.json  # 设置文件
`

## 主要功能

1. **插件系统**：支持动态加载和管理插件，实现功能扩展
2. **扩展管理器**：提供统一的扩展点管理，支持自定义扩展
3. **热重载**：支持插件和扩展的热重载，无需重启应用
4. **依赖注入**：集成 .NET 依赖注入系统，支持插件和扩展的依赖注入
5. **配置管理**：统一的配置管理，支持插件和扩展的配置
6. **高性能设计**：使用 Span 优化、内存池、通道和零拷贝技术
7. **易于使用的 API**：简单直观的 API 设计，支持异步操作
8. **可扩展架构**：支持自定义扩展和集成

## 扩展说明

本技能提供了完整的 NetPro 解决方案，您可以根据需要进行扩展：

1. **自定义插件**：实现 INetProPlugin 接口，创建自定义插件
2. **自定义扩展**：实现 INetProExtension 接口，创建自定义扩展
3. **集成其他系统**：将 NetPro 与其他系统集成，如数据库、缓存等
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务生命周期，提高代码可测试性
2. **异步编程**：优先使用异步 API 避免阻塞，提高系统吞吐量
3. **错误处理**：正确处理异常情况，确保系统稳定性
4. **日志记录**：添加适当的日志记录，便于故障排查
5. **性能监控**：监控关键性能指标，及时发现和解决性能问题
6. **插件设计**：设计合理的插件结构，避免插件间的冲突
7. **热重载**：合理使用热重载功能，提高开发效率

## 配置选项

### NetProOptions 配置

| 选项 | 类型 | 默认值 | 描述 |
|------|------|--------|------|
| EnablePlugins | bool | true | 是否启用插件系统 |
| PluginsPath | string | "plugins" | 插件目录路径 |
| EnableHotReload | bool | true | 是否启用热重载 |
| EnableCaching | bool | true | 是否启用缓存 |
| CacheSize | int | 1000 | 缓存大小 |
| Timeout | TimeSpan | 30秒 | 操作超时时间 |
| EnableDetailedLogging | bool | false | 是否启用详细日志 |

## 性能特性

- **高吞吐量**：使用通道和批处理技术，支持高并发处理
- **低延迟**：使用 Span 和零拷贝技术，减少内存分配和拷贝
- **内存优化**：使用内存池和对象池，减少 GC 压力
- **可扩展性**：支持水平扩展，适用于大规模应用

## 版本兼容性

| .NET 版本 | 兼容性 |
|-----------|--------|------|
| .NET 10.0 | ✅ 完全支持 |
| .NET 9.0  | ✅ 支持 |
| .NET 8.0  | ✅ 支持 |
