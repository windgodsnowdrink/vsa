# Version 技能

Version 技能是一个专注于版本管理的工具库，提供了语义化版本解析、比较、递增、验证等功能，帮助开发者在项目中高效管理版本号。

## 功能特性

- **语义化版本解析**：支持解析符合 Semantic Versioning 规范的版本号
- **版本比较**：提供多种比较方式，判断版本高低
- **版本递增**：支持主版本、次版本、补丁版本的递增
- **版本验证**：验证版本号是否符合规范
- **版本范围**：支持定义和检查版本范围
- **版本格式化**：自定义版本号的输出格式
- **依赖注入**：支持通过 DI 容器注册和使用
- **性能优化**：使用缓存机制提高频繁操作的性能

## 安装

### 方法一：通过 NuGet 安装

```bash
dotnet add package VersionSkill
```

### 方法二：手动引用

将 `version_core.cs` 文件添加到您的项目中。

## 注册服务

在 `Startup.cs` 或 `Program.cs` 中注册 Version 技能服务：

```csharp
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 注册 Version 技能服务
builder.Services.AddVersionSkill();

var app = builder.Build();
app.Run();
```

## 使用示例

### 基本用法

```csharp
using VersionSkill;

// 创建版本对象
var version = new SemanticVersion(1, 2, 3);
Console.WriteLine(version); // 输出: 1.2.3

// 解析版本字符串
var parsedVersion = SemanticVersion.Parse("2.0.0-alpha.1");
Console.WriteLine(parsedVersion); // 输出: 2.0.0-alpha.1

// 比较版本
var version1 = new SemanticVersion(1, 0, 0);
var version2 = new SemanticVersion(1, 1, 0);
Console.WriteLine(version1 < version2); // 输出: True

// 递增版本
var nextVersion = version1.IncrementPatch();
Console.WriteLine(nextVersion); // 输出: 1.0.1
```

### 通过依赖注入使用

```csharp
using Microsoft.Extensions.DependencyInjection;
using VersionSkill;

// 注册服务
var services = new ServiceCollection();
services.AddVersionSkill();
var serviceProvider = services.BuildServiceProvider();

// 获取版本服务
var versionService = serviceProvider.GetRequiredService<IVersionService>();

// 使用服务
var version = versionService.Parse("1.0.0");
var nextVersion = versionService.IncrementMinor(version);
Console.WriteLine(nextVersion); // 输出: 1.1.0
```

### 版本范围

```csharp
using VersionSkill;

// 创建版本范围
var versionRange = VersionRange.Parse(">=1.0.0 <2.0.0");

// 检查版本是否在范围内
var version = new SemanticVersion(1, 5, 0);
Console.WriteLine(versionRange.IsInRange(version)); // 输出: True
```

## 性能优化

Version 技能内置了缓存机制，对于频繁使用的版本解析和比较操作，会自动缓存结果以提高性能：

```csharp
// 第一次解析会缓存结果
var version1 = SemanticVersion.Parse("1.0.0");

// 第二次解析相同版本会从缓存获取，性能更高
var version2 = SemanticVersion.Parse("1.0.0");
```

## Scrutor 集成

Version 技能支持使用 Scrutor 进行自动服务注册，详见 `scrutor_demo.cs` 文件中的示例。

## 配置选项

可以通过配置文件自定义 Version 技能的行为：

```json
{
  "VersionSkill": {
    "CacheEnabled": true,
    "CacheSize": 1000,
    "StrictMode": false
  }
}
```

## 异常处理

Version 技能在遇到无效版本号时会抛出 `VersionFormatException` 异常，建议在使用时进行适当的异常处理：

```csharp
try
{
    var version = SemanticVersion.Parse("invalid-version");
}
catch (VersionFormatException ex)
{
    Console.WriteLine($"版本号格式无效: {ex.Message}");
}
```

## 相关资源

- [语义化版本规范](https://semver.org/lang/zh-CN/)
- [参考文档](./reference/README.md)
- [使用示例](./reference/examples.md)

## 贡献

欢迎提交 Issue 和 Pull Request 来帮助改进 Version 技能！

## 许可证

Version 技能采用 MIT 许可证。