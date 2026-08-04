# NetPro - 参考文档

## 功能说明

NetPro 是一个基于 .NET 10 的高性能插件系统和扩展框架，专为 .NET 开发者设计。它提供了强大的插件管理、扩展点管理、热重载和依赖注入等功能，帮助开发者构建可扩展的应用系统。

## 核心组件

### 1. INetProService (NetPro 服务)
- **位置**: scripts/netpro_plugin_service.cs
- **功能**: 核心业务逻辑处理，提供基础功能
- **特性**: 
  - 核心功能实现
  - 性能优化
  - 错误处理
  - 日志记录

### 2. INetProPluginManager (插件管理器)
- **位置**: scripts/netpro_plugin_service.cs
- **功能**: 动态加载和管理插件
- **特性**: 
  - 插件加载和卸载
  - 插件生命周期管理
  - 插件依赖解析
  - 热重载支持

### 3. INetProExtensionManager (扩展管理器)
- **位置**: scripts/netpro_plugin_service.cs
- **功能**: 管理扩展点和扩展
- **特性**: 
  - 扩展点注册和管理
  - 扩展执行
  - 扩展依赖解析
  - 热重载支持

### 4. INetProPlugin (插件接口)
- **位置**: scripts/netpro_plugin_service.cs
- **功能**: 插件的基本接口
- **特性**: 
  - 插件信息
  - 插件初始化和销毁
  - 插件执行
  - 插件配置

### 5. INetProExtension (扩展接口)
- **位置**: scripts/netpro_plugin_service.cs
- **功能**: 扩展的基本接口
- **特性**: 
  - 扩展信息
  - 扩展初始化和销毁
  - 扩展执行
  - 扩展配置

## 使用示例

### 基本用法

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
    Console.WriteLine($"插件: {plugin.Name}, 版本: {plugin.Version}
