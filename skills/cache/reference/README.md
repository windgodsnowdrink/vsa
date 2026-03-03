# cache - 参考文档

## 概述

cache 是一个基于 .NET 10 的高性能缓存系统，专为 .NET 开发者设计，支持 AOT（提前编译）编译，提供强大的缓存管理和优化功能。

## 核心组件

### 1. 内存缓存服务
- **位置**: scripts/microsoft_cache_demo.cs
- **功能**: 提供基于内存的高性能缓存服务
- **特性**: 
  - 支持多种过期策略（绝对过期、滑动过期、相对过期）
  - 支持缓存大小限制
  - 支持缓存项优先级
  - 支持缓存项移除事件
  - 支持 AOT 编译优化

### 2. 分布式缓存服务
- **位置**: scripts/stackexchange_redis_cache.cs、scripts/garnet_redis_cache.cs
- **功能**: 提供基于 Redis/Garnet 的分布式缓存服务
- **特性**: 
  - 支持分布式缓存一致性
  - 支持缓存键前缀和命名空间
  - 支持分布式锁
  - 支持发布/订阅机制
  - 支持 AOT 编译优化

### 3. 多级缓存服务
- **位置**: scripts/cache_hybrid_integration.cs
- **功能**: 提供内存缓存 + 分布式缓存的多级缓存架构
- **特性**: 
  - 自动缓存同步
  - 支持缓存降级
  - 支持缓存预热
  - 支持缓存刷新
  - 支持 AOT 编译优化

### 4. 缓存监控服务
- **位置**: scripts/cache_heartbeat_integration.cs
- **功能**: 提供缓存监控和统计功能
- **特性**: 
  - 缓存命中率统计
  - 缓存性能指标监控
  - 缓存健康检查
  - 支持告警通知

## 使用示例

### 基本内存缓存使用

```csharp
var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

// 设置缓存
memoryCache.Set("key1", "value1", TimeSpan.FromMinutes(10));

// 获取缓存
var value1 = memoryCache.Get<string>("key1");
Console.WriteLine($"内存缓存结果: {value1}
