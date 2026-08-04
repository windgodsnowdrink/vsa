# Version 技能参考文档

## 概述

Version 技能是一个专注于版本管理的工具库，提供了语义化版本解析、比较、递增、验证等功能，帮助开发者在项目中高效管理版本号。

## 核心功能

### 1. 语义化版本解析

支持解析符合 [Semantic Versioning 2.0.0](https://semver.org/lang/zh-CN/) 规范的版本号，包括：
- 主版本号（Major）
- 次版本号（Minor）
- 补丁版本号（Patch）
- 预发布版本号（Pre-release）
- 构建元数据（Build metadata）

### 2. 版本比较

提供多种比较方式，判断版本高低：
- 相等性比较（==, !=）
- 大小比较（<, <=, >, >=）
- 版本范围检查

### 3. 版本递增

支持主版本、次版本、补丁版本的递增：
- `IncrementMajor()`：递增主版本号，重置次版本和补丁版本为 0
- `IncrementMinor()`：递增次版本号，重置补丁版本为 0
- `IncrementPatch()`：递增补丁版本号

### 4. 版本验证

验证版本号是否符合 Semantic Versioning 规范：
- `SemanticVersion.TryParse()`：尝试解析版本号，返回解析结果
- `SemanticVersion.IsValid()`：检查版本字符串是否有效

### 5. 版本范围

支持定义和检查版本范围：
- 精确版本匹配（如 "1.0.0"）
- 大于/小于（如 ">1.0.0", "<2.0.0"）
- 大于等于/小于等于（如 ">=1.0.0", "<=2.0.0"）
- 区间范围（如 ">=1.0.0 <2.0.0"）
- 波浪号范围（如 "~1.0.0"）
- 插入符号范围（如 "^1.0.0"）

### 6. 版本格式化

自定义版本号的输出格式：
- 标准格式（如 "1.0.0-alpha.1"）
- 仅核心版本（如 "1.0.0"）
- 包含构建元数据（如 "1.0.0+build.1"）

### 7. 依赖注入

支持通过 DI 容器注册和使用：
- `services.AddVersionSkill()`：注册 Version 技能服务
- `IVersionService`：版本服务接口

### 8. 性能优化

使用缓存机制提高频繁操作的性能：
- 版本解析缓存
- 版本比较缓存
- 可配置的缓存大小

## 架构设计

### 核心组件

1. **SemanticVersion**：表示语义化版本的核心类
2. **VersionRange**：表示版本范围的类
3. **IVersionService**：版本服务接口
4. **VersionService**：版本服务实现
5. **VersionCache**：版本缓存类
6. **VersionSkillExtensions**：依赖注入扩展方法

### 依赖关系

```
┌─────────────────┐     ┌─────────────────┐
│ SemanticVersion │ ◄── │ VersionService  │
└─────────────────┘     └─────────────────┘
        ▲                      ▲
        │                      │
┌─────────────────┐     ┌─────────────────┐
│  VersionRange   │     │ IVersionService │
└─────────────────┘     └─────────────────┘
        ▲                      ▲
        │                      │
┌─────────────────┐     ┌─────────────────┐
│  VersionCache   │     │ VersionSkill   │
└─────────────────┘     │ Extensions     │
                        └─────────────────┘
```

## 性能特性

### 缓存机制

Version 技能内置了缓存机制，对于频繁使用的版本解析和比较操作，会自动缓存结果以提高性能。缓存配置可通过 `VersionOptions` 进行调整。

### 内存使用

- **SemanticVersion** 类设计为不可变类型，减少内存分配
- 使用对象池减少重复创建对象的开销
- 缓存大小可配置，避免过度使用内存

### 线程安全

- **SemanticVersion** 类是线程安全的
- **VersionService** 类是线程安全的
- **VersionCache** 类使用线程安全的缓存实现

## 配置选项

| 选项 | 类型 | 默认值 | 描述 |
|------|------|--------|------|
| CacheEnabled | bool | true | 是否启用缓存 |
| CacheSize | int | 1000 | 缓存大小 |
| StrictMode | bool | false | 是否使用严格模式解析版本号 |

## 使用指南

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
services.AddVersionSkill(options =>
{
    options.CacheEnabled = true;
    options.CacheSize = 1000;
    options.StrictMode = false;
});
var serviceProvider = services.BuildServiceProvider();

// 获取版本服务
var versionService = serviceProvider.GetRequiredService<IVersionService>();

// 使用服务
var version = versionService.Parse("1.0.0");
var nextVersion = versionService.IncrementMinor(version);
Console.WriteLine(nextVersion); // 输出: 1.1.0

// 检查版本范围
var versionRange = versionService.ParseRange(">=1.0.0 <2.0.0");
var isInRange = versionService.IsInRange(version, versionRange);
Console.WriteLine(isInRange); // 输出: True
```

### 版本范围使用

```csharp
using VersionSkill;

// 创建版本范围
var versionRange = VersionRange.Parse(">=1.0.0 <2.0.0");

// 检查版本是否在范围内
var version1 = new SemanticVersion(1, 5, 0);
var version2 = new SemanticVersion(2, 0, 0);

Console.WriteLine(versionRange.IsInRange(version1)); // 输出: True
Console.WriteLine(versionRange.IsInRange(version2)); // 输出: False

// 使用波浪号范围（~1.0.0 相当于 >=1.0.0 <1.1.0）
var tildeRange = VersionRange.Parse("~1.0.0");

// 使用插入符号范围（^1.0.0 相当于 >=1.0.0 <2.0.0）
var caretRange = VersionRange.Parse("^1.0.0");
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

// 或者使用 TryParse 方法
if (SemanticVersion.TryParse("invalid-version", out var version))
{
    // 解析成功
}
else
{
    // 解析失败
    Console.WriteLine("版本号格式无效");
}
```

## 最佳实践

### 1. 版本号命名规范

- 遵循 Semantic Versioning 2.0.0 规范
- 预发布版本使用 `-alpha`, `-beta`, `-rc` 等前缀
- 构建元数据使用 `+` 分隔，包含构建编号、时间戳等信息

### 2. 版本控制策略

- **主版本号**：当你做了不兼容的 API 修改
- **次版本号**：当你做了向后兼容的功能性新增
- **补丁版本号**：当你做了向后兼容的问题修正

### 3. 性能优化建议

- 对于频繁使用的版本号，建议缓存 `SemanticVersion` 对象
- 对于大量版本比较操作，建议使用 `VersionService` 以利用内部缓存
- 根据实际使用场景调整缓存大小

## 常见问题

### Q: 为什么解析某些版本号会失败？

A: 可能是因为版本号不符合 Semantic Versioning 2.0.0 规范。请检查版本号格式是否正确，特别是预发布版本和构建元数据的格式。

### Q: 版本比较的规则是什么？

A: 版本比较按照以下规则进行：
1. 首先比较主版本号
2. 如果主版本号相同，比较次版本号
3. 如果次版本号相同，比较补丁版本号
4. 如果补丁版本号相同，比较预发布版本号
5. 构建元数据不参与版本比较

### Q: 如何自定义版本号的输出格式？

A: 可以通过 `ToString()` 方法的重载来自定义输出格式，或者使用 `Format()` 方法指定格式。

### Q: 缓存机制会影响内存使用吗？

A: 缓存机制会使用一定的内存，但默认缓存大小为 1000，对于大多数应用场景来说是合理的。可以通过配置选项调整缓存大小或禁用缓存。

## 示例项目

请参考 `examples.md` 文件中的详细使用示例，包括：
- 基本使用示例
- 高级使用示例
- Scrutor 集成示例
- 性能测试示例

## 贡献指南

欢迎提交 Issue 和 Pull Request 来帮助改进 Version 技能！

### 开发环境

- .NET 8.0 或更高版本
- Visual Studio 2022 或更高版本
- .NET CLI

### 测试

```bash
dotnet test
```

### 构建

```bash
dotnet build
```

## 许可证

Version 技能采用 MIT 许可证。