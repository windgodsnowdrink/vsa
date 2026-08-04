# Resilient 技能使用示例

## 概述

本文档提供了 Resilient 技能的各种使用示例，包括基本用法、高级配置、错误处理、分布式任务处理等场景。通过这些示例，开发者可以快速上手 Resilient 技能，构建更加可靠、稳定的应用。

## 基本用法

### 1. 弹性服务的基本使用

#### 1.1 注册弹性服务

```csharp
// 构建服务容器
var services = new ServiceCollection();

// 配置日志
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// 注册弹性服务
services.AddResilientServices(options =>
{
    options.Retry.Count = 3;
    options.Retry.MinBackoff = TimeSpan.FromSeconds(1);
    options.Retry.MaxBackoff = TimeSpan.FromSeconds(10);
    options.CircuitBreaker.FailureThreshold = 0.5;
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
    options.Timeout = TimeSpan.FromSeconds(30);
});

var serviceProvider = services.BuildServiceProvider();

// 获取弹性服务
var resilientService = serviceProvider.GetRequiredService<IResilientService>();
```

#### 1.2 使用重试策略

```csharp
// 使用重试策略执行操作
var result = await resilientService.ExecuteWithRetryAsync(async () =>
{
    // 模拟可能失败的操作
    if (new Random().Next(0, 2) == 0)
    {
        throw new HttpRequestException("模拟网络错误");
    }
    return "操作成功";
});

Console.WriteLine($"操作结果: {result}");
```

#### 1.3 使用熔断策略

```csharp
// 使用熔断策略执行操作
var result = await resilientService.ExecuteWithCircuitBreakerAsync(async () =>
{
    // 模拟可能失败的操作
    if (new Random().Next(0, 2) == 0)
    {
        throw new HttpRequestException("模拟网络错误");
    }
    return "操作成功";
});

Console.WriteLine($"操作结果: {result}");
```

#### 1.4 使用限速策略

```csharp
// 使用限速策略执行操作
var result = await resilientService.ExecuteWithRateLimiterAsync(async () =>
{
    // 执行需要限速的操作
    await Task.Delay(50);
    return "操作成功";
});

Console.WriteLine($"操作结果: {result}");
```

#### 1.5 使用超时策略

```csharp
// 使用超时策略执行操作
var result = await resilientService.ExecuteWithTimeoutAsync(async () =>
{
    // 模拟可能超时的操作
    await Task.Delay(40000); // 超过30秒的超时时间
    return "操作成功";
});

Console.WriteLine($"操作结果: {result}");
```

#### 1.6 使用隔离舱策略

```csharp
// 使用隔离舱策略执行操作
var result = await resilientService.ExecuteWithBulkheadAsync(async () =>
{
    // 执行需要隔离的操作
    await Task.Delay(100);
    return "操作成功";
});

Console.WriteLine($"操作结果: {result}");
```

#### 1.7 使用组合策略

```csharp
// 使用组合策略执行操作
var result = await resilientService.ExecuteWithCombinedPoliciesAsync(async () =>
{
    // 模拟可能失败的操作
    if (new Random().Next(0, 2) == 0)
    {
        throw new HttpRequestException("模拟网络错误");
    }
    await Task.Delay(50); // 模拟操作耗时
    return "操作成功";
});

Console.WriteLine($"操作结果: {result}");
```

### 2. 限速器的基本使用

#### 2.1 注册限速器服务

```csharp
// 构建服务容器
var services = new ServiceCollection();

// 配置日志
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// 注册限速器服务
services.AddRateLimiterServices(options =>
{
    options.TokenLimit = 10;
    options.TokensPerPeriod = 10;
    options.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
    options.QueueLimit = 5;
});

var serviceProvider = services.BuildServiceProvider();

// 获取限速器服务
var rateLimiterService = serviceProvider.GetRequiredService<IRateLimiterService>();
```

#### 2.2 使用限速器执行操作

```csharp
// 测试限速器
var tasks = Enumerable.Range(0, 15).Select(async i =>
{
    try
    {
        var result = await rateLimiterService.ExecuteWithRateLimiterAsync(async () =>
        {
            await Task.Delay(50); // 模拟操作
            return $"请求 {i} 完成