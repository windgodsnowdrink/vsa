# Nacos 智能体技能 - 参考文档

## 概述

Nacos 智能体技能是基于.NET 10的高性能Nacos系统，专为.NET开发者设计。它提供了完整的服务发现与配置管理功能，包括服务注册与发现、配置中心、服务健康检查等核心特性，采用AOT编译优化，提供高性能、高可扩展的Nacos集成架构。

## 核心组件

### 1. INacosService (Nacos 核心服务)
- **位置**: scripts/nacos_integration.cs
- **功能**: Nacos的核心业务逻辑处理
- **特性**: 
  - 服务注册与发现的核心实现
  - 配置中心的核心实现
  - 性能优化和缓存管理
  - 错误处理和日志记录
  - 服务健康检查的协调

### 2. INacosConfigService (Nacos 配置服务)
- **位置**: scripts/nacos_integration.cs
- **功能**: Nacos配置中心的管理和操作
- **特性**: 
  - 配置的获取和更新
  - 配置变更的监听
  - 配置缓存的管理
  - 配置版本的控制

### 3. INacosNamingService (Nacos 命名服务)
- **位置**: scripts/nacos_integration.cs
- **功能**: Nacos服务注册与发现的管理和操作
- **特性**: 
  - 服务实例的注册和注销
  - 服务实例的发现和选择
  - 服务健康检查的管理
  - 服务路由和负载均衡

## 使用示例

### 基本用法

```csharp
// 获取Nacos服务
var nacosService = serviceProvider.GetRequiredService<INacosService>();
var configService = serviceProvider.GetRequiredService<INacosConfigService>();
var namingService = serviceProvider.GetRequiredService<INacosNamingService>();

// 从配置中心获取配置
var config = await configService.GetConfigAsync("example-config", "DEFAULT_GROUP");
Console.WriteLine($"配置内容: {config}");

// 注册服务
await namingService.RegisterInstanceAsync("example-service", "127.0.0.1", 8080);
Console.WriteLine("服务注册成功");

// 发现服务
var instances = await namingService.SelectInstancesAsync("example-service", true);
Console.WriteLine($"发现服务实例数: {instances.Count}");
foreach (var instance in instances)
{
    Console.WriteLine($"服务实例: {instance.Ip}:{instance.Port}
