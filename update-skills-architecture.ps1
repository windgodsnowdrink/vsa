#!/usr/bin/env pwsh

# 脚本用于按照todo架构更新skills下的技能目录结构

param(
    [string]$StartFrom = "appinsights"
)

# 获取所有技能目录
$skills = Get-ChildItem -Path "d:/Trae/vsa/skills" -Directory | Sort-Object Name | ForEach-Object { $_.Name }

# 找到开始处理的索引
$startIndex = $skills.IndexOf($StartFrom)
if ($startIndex -eq -1) {
    Write-Host "错误：找不到技能目录 $StartFrom" -ForegroundColor Red
    exit 1
}

# 从开始索引到结束的技能列表
$skillsToProcess = $skills[$startIndex..($skills.Length - 1)]

Write-Host "开始处理从 $StartFrom 开始的 $($skillsToProcess.Length) 个技能..." -ForegroundColor Green

# 处理每个技能
foreach ($skillName in $skillsToProcess) {
    Write-Host "\n处理技能：$skillName" -ForegroundColor Yellow
    
    $skillPath = "d:/Trae/vsa/skills/$skillName"
    
    # 1. 确保reference目录存在
    $referencePath = "$skillPath/reference"
    if (-not (Test-Path $referencePath)) {
        New-Item -ItemType Directory -Path $referencePath -Force | Out-Null
        Write-Host "  创建reference目录"
    }
    
    # 2. 确保scripts目录存在
    $scriptsPath = "$skillPath/scripts"
    if (-not (Test-Path $scriptsPath)) {
        New-Item -ItemType Directory -Path $scriptsPath -Force | Out-Null
        Write-Host "  创建scripts目录"
    }
    
    # 3. 创建index.yaml文件（如果不存在）
    $indexPath = "$skillPath/index.yaml"
    if (-not (Test-Path $indexPath)) {
        $indexContent = @"
# $skillName Agent Skill - $skillName技能
# 版本: 1.0.0
# 作者: VSA Architecture Team
# 描述: 基于.NET 10的$skillName技能实现
# 技能类型: 系统功能
# 依赖:
# - Microsoft.Extensions.DependencyInjection@10.0.0
# - Microsoft.Extensions.Logging@10.0.0
# 适用场景: $skillName相关功能
# 激活条件: 需要$skillName功能时激活
# 主要功能:
# - $skillName核心功能1
# - $skillName核心功能2
# - $skillName核心功能3
# 性能特性:
# - 高性能设计
# - 内存优化
# - 并发支持
# 技术特性:
# - 模块化设计
# - 依赖注入
# - 异步编程
# - 高性能算法
"@
        Set-Content -Path $indexPath -Value $indexContent
        Write-Host "  创建index.yaml文件"
    }
    
    # 4. 创建SKILL.md文件（如果不存在）
    $skillMdPath = "$skillPath/SKILL.md"
    if (-not (Test-Path $skillMdPath)) {
        $skillMdContent = @"
# $skillName Agent Skill - $skillName技能

## 技能概览

基于.NET 10的高性能$skillName技能，为.NET开发者提供强大的$skillName功能支持。

## 快速入门指南

### 安装依赖

在主应用程序的runfile中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
```

### 注册服务

在主应用程序中注册$skillName服务：

```csharp
// 注册$skillName服务
builder.Services.AddSingleton<$skillNameService>();
builder.Services.AddSingleton<I$skillNameProvider, $skillNameProvider>();
```

### 使用示例

```csharp
// 获取$skillName服务
var $skillNameService = serviceProvider.GetRequiredService<$skillNameService>();

// 使用$skillName功能
var result = await $skillNameService.DoSomethingAsync();
Console.WriteLine($"结果: {result}");
```

## 导航地图

```
$skillName/
├── index.yaml                   # 元数据索引说明
├── SKILL.md                    # 技能入口点 (当前文件)
├── reference/                  # 引用文件
│   ├── README.md              # 完整功能说明
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── $skillName_impl.cs     # $skillName核心实现
    ├── $skillName_impl.run.json  # 运行配置
    └── $skillName_impl.setting.json  # 设置文件
```

## 主要功能

1. **核心功能1**：$skillName核心功能描述
2. **核心功能2**：$skillName核心功能描述
3. **核心功能3**：$skillName核心功能描述
4. **高性能设计**：优化的性能实现
5. **易用API**：简单易用的API设计
6. **可扩展架构**：支持自定义扩展

## 扩展说明

本技能提供了完整的$skillName解决方案，您可以根据需要扩展：

1. **自定义实现**：实现I$skillNameProvider接口
2. **扩展功能**：添加新的$skillName功能
3. **集成其他系统**：与其他系统集成
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步API
3. **错误处理**：妥善处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
"@
        Set-Content -Path $skillMdPath -Value $skillMdContent
        Write-Host "  创建SKILL.md文件"
    }
    
    # 5. 创建reference/README.md文件（如果不存在）
    $readmePath = "$referencePath/README.md"
    if (-not (Test-Path $readmePath)) {
        $readmeContent = @"
# $skillName - 参考文档

## 概述

$skillName 是一个基于.NET 10的高性能$skillName系统，专为.NET开发者设计。

## 核心组件

### 1. $skillNameService ($skillName服务)
- **位置**: `scripts/$skillName_impl.cs`
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 核心功能实现
  - 性能优化
  - 错误处理
  - 日志记录

## 使用示例

### 基本使用

```csharp
var $skillNameService = serviceProvider.GetRequiredService<$skillNameService>();
var result = await $skillNameService.DoSomethingAsync();
```

### 高级配置

```csharp
var settings = new $skillNameSettings {
    EnableCache = true,
    CacheSize = 1000,
    Timeout = TimeSpan.FromSeconds(30)
};

builder.Services.Configure<$skillNameSettings>(options => {
    options.EnableCache = settings.EnableCache;
    options.CacheSize = settings.CacheSize;
    options.Timeout = settings.Timeout;
});
```

## 配置选项

### $skillNameSettings 配置

```json
{
  "$skillNameSettings": {
    "EnableCache": true,          // 启用缓存
    "CacheSize": 1000,            // 缓存大小
    "Timeout": "00:00:30",        // 超时时间
    "EnableDetailedLogging": false // 启用详细日志
  }
}
```

## 性能优化

1. **缓存使用**：启用缓存提高性能
2. **异步编程**：使用异步API避免阻塞
3. **批量处理**：批量处理提高效率
4. **连接池**：使用连接池管理资源

## 故障排除

### 常见问题

1. **连接失败**
   - 检查配置文件
   - 验证网络连接
   - 查看日志信息

2. **性能问题**
   - 启用缓存
   - 优化查询条件
   - 增加资源限制

## 扩展开发

### 添加自定义功能

```csharp
public class Custom$skillNameProvider : I$skillNameProvider
{
    public async Task<Result> DoSomethingAsync()
    {
        // 实现自定义逻辑
        return new Result();
    }
}
```
"@
        Set-Content -Path $readmePath -Value $readmeContent
        Write-Host "  创建reference/README.md文件"
    }
    
    # 6. 创建reference/examples.md文件（如果不存在）
    $examplesPath = "$referencePath/examples.md"
    if (-not (Test-Path $examplesPath)) {
        $examplesContent = @"
# $skillName - 使用示例

## 快速开始

### 1. 基础使用示例

```csharp
using System;
using $skillName;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var $skillNameService = serviceProvider.GetRequiredService<$skillNameService>();
        
        Console.WriteLine("$skillName 基础使用示例");
        Console.WriteLine("=" * 50);
        
        // 使用$skillName功能
        var result = await $skillNameService.DoSomethingAsync();
        Console.WriteLine($"结果: {result}");
        
        // 其他操作...
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<$skillNameService>();
        builder.AddSingleton<I$skillNameProvider, $skillNameProvider>();
        return builder.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("$skillName 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置$skillName设置
        builder.Configure<$skillNameSettings>(options => {
            options.EnableCache = true;
            options.CacheSize = 2000;
            options.Timeout = TimeSpan.FromSeconds(60);
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.AddSingleton<I$skillNameProvider, $skillNameProvider>();
        builder.AddSingleton<$skillNameService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<$skillNameSettings>>().Value;
        Console.WriteLine($"配置: 缓存={settings.EnableCache}, 大小={settings.CacheSize}");
        
        // 使用服务
        var $skillNameService = serviceProvider.GetRequiredService<$skillNameService>();
        var result = await $skillNameService.DoSomethingAsync();
        Console.WriteLine($"结果: {result}");
    }
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Diagnostics;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("$skillName 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var $skillNameService = serviceProvider.GetRequiredService<$skillNameService>();
        
        // 性能测试
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            await $skillNameService.DoSomethingAsync();
        }
        
        stopwatch.Stop();
        Console.WriteLine($"执行 {iterations} 次耗时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("$skillName 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var $skillNameService = serviceProvider.GetRequiredService<$skillNameService>();
        
        try
        {
            var result = await $skillNameService.DoSomethingAsync();
            Console.WriteLine($"成功: {result}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"超时错误: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"操作错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
    }
}
```

## 总结

以上示例展示了 $skillName 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基础操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目需求。
"@
        Set-Content -Path $examplesPath -Value $examplesContent
        Write-Host "  创建reference/examples.md文件"
    }
    
    # 7. 检查scripts目录下的文件，确保每个.cs文件都有对应的.run.json和.setting.json
    $csFiles = Get-ChildItem -Path $scriptsPath -Filter "*.cs" | ForEach-Object { $_.Name }
    
    foreach ($csFile in $csFiles) {
        $baseName = [System.IO.Path]::GetFileNameWithoutExtension($csFile)
        
        # 检查.run.json文件
        $runJsonPath = "$scriptsPath/$baseName.run.json"
        if (-not (Test-Path $runJsonPath)) {
            $runJsonContent = @"
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "$baseName",
      "type": "dotnet",
      "request": "launch",
      "program": "D:\Trae\vsa\skills\$skillName\scripts\$csFile",
      "args": [
        "--settings",
        "D:\Trae\vsa\skills\$skillName\scripts\$baseName.setting.json"
      ],
      "cwd": "d:\\Trae\\vsa",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DOTNET_ENVIRONMENT": "Development"
      },
      "stopAtEntry": false,
      "console": "integratedTerminal"
    }
  ]
}
"@
            Set-Content -Path $runJsonPath -Value $runJsonContent
            Write-Host "  创建 $baseName.run.json 文件"
        }
        
        # 检查.setting.json文件
        $settingJsonPath = "$scriptsPath/$baseName.setting.json"
        if (-not (Test-Path $settingJsonPath)) {
            $settingJsonContent = @"
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
"@
            Set-Content -Path $settingJsonPath -Value $settingJsonContent
            Write-Host "  创建 $baseName.setting.json 文件"
        }
    }
    
    Write-Host "  ✅ 技能 $skillName 处理完成" -ForegroundColor Green
}

Write-Host "\n" -NoNewline
Write-Host "=" * 50 -ForegroundColor Green
Write-Host "所有技能处理完成！" -ForegroundColor Green
Write-Host "=" * 50 -ForegroundColor Green
