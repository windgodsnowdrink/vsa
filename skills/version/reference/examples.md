# Version 技能使用示例

## 基本使用示例

### 1. 创建和解析版本号

```csharp
using VersionSkill;

// 创建版本对象
var version1 = new SemanticVersion(1, 2, 3);
Console.WriteLine(version1); // 输出: 1.2.3

// 创建包含预发布版本的版本对象
var version2 = new SemanticVersion(2, 0, 0, "alpha.1");
Console.WriteLine(version2); // 输出: 2.0.0-alpha.1

// 创建包含构建元数据的版本对象
var version3 = new SemanticVersion(1, 0, 0, null, "build.1");
Console.WriteLine(version3); // 输出: 1.0.0+build.1

// 解析版本字符串
var version4 = SemanticVersion.Parse("1.0.0-beta.1+build.2");
Console.WriteLine(version4); // 输出: 1.0.0-beta.1+build.2

// 尝试解析版本字符串
if (SemanticVersion.TryParse("invalid-version", out var version5))
{
    Console.WriteLine($"解析成功: {version5}");
}
else
{
    Console.WriteLine("解析失败: 版本号格式无效");
}
```

### 2. 版本比较

```csharp
using VersionSkill;

// 创建版本对象
var version1 = new SemanticVersion(1, 0, 0);
var version2 = new SemanticVersion(1, 1, 0);
var version3 = new SemanticVersion(1, 0, 1);
var version4 = new SemanticVersion(1, 0, 0, "alpha");
var version5 = new SemanticVersion(1, 0, 0, "beta");

// 相等性比较
Console.WriteLine(version1 == version1); // 输出: True
Console.WriteLine(version1 == version2); // 输出: False

// 大小比较
Console.WriteLine(version1 < version2); // 输出: True
Console.WriteLine(version2 > version3); // 输出: True
Console.WriteLine(version3 > version1); // 输出: True

// 预发布版本比较
Console.WriteLine(version4 < version1); // 输出: True（预发布版本小于正式版本）
Console.WriteLine(version4 < version5); // 输出: True（alpha < beta）

// 构建元数据不参与比较
var version6 = new SemanticVersion(1, 0, 0, null, "build.1");
var version7 = new SemanticVersion(1, 0, 0, null, "build.2");
Console.WriteLine(version6 == version7); // 输出: True
```

### 3. 版本递增

```csharp
using VersionSkill;

// 创建版本对象
var version = new SemanticVersion(1, 0, 0);
Console.WriteLine($"原始版本: {version}");

// 递增补丁版本
var patchVersion = version.IncrementPatch();
Console.WriteLine($"递增补丁版本: {patchVersion}"); // 输出: 1.0.1

// 递增次版本
var minorVersion = version.IncrementMinor();
Console.WriteLine($"递增次版本: {minorVersion}"); // 输出: 1.1.0

// 递增主版本
var majorVersion = version.IncrementMajor();
Console.WriteLine($"递增主版本: {majorVersion}"); // 输出: 2.0.0

// 从非零版本递增
var version2 = new SemanticVersion(2, 3, 4);
Console.WriteLine($"原始版本: {version2}");
Console.WriteLine($"递增补丁版本: {version2.IncrementPatch()}"); // 输出: 2.3.5
Console.WriteLine($"递增次版本: {version2.IncrementMinor()}"); // 输出: 2.4.0
Console.WriteLine($"递增主版本: {version2.IncrementMajor()}"); // 输出: 3.0.0
```

### 4. 版本范围

```csharp
using VersionSkill;

// 创建版本对象
var version1 = new SemanticVersion(1, 0, 0);
var version2 = new SemanticVersion(1, 5, 0);
var version3 = new SemanticVersion(2, 0, 0);

// 精确版本匹配
var range1 = VersionRange.Parse("1.0.0");
Console.WriteLine($"1.0.0 在范围内: {range1.IsInRange(version1)}"); // 输出: True
Console.WriteLine($"1.5.0 在范围内: {range1.IsInRange(version2)}"); // 输出: False

// 大于/小于
var range2 = VersionRange.Parse(">1.0.0");
Console.WriteLine($"1.5.0 在范围内: {range2.IsInRange(version2)}"); // 输出: True

var range3 = VersionRange.Parse("<2.0.0");
Console.WriteLine($"1.5.0 在范围内: {range3.IsInRange(version2)}"); // 输出: True
Console.WriteLine($"2.0.0 在范围内: {range3.IsInRange(version3)}"); // 输出: False

// 大于等于/小于等于
var range4 = VersionRange.Parse(">=1.0.0");
Console.WriteLine($"1.0.0 在范围内: {range4.IsInRange(version1)}"); // 输出: True

var range5 = VersionRange.Parse("<=2.0.0");
Console.WriteLine($"2.0.0 在范围内: {range5.IsInRange(version3)}"); // 输出: True

// 区间范围
var range6 = VersionRange.Parse(">=1.0.0 <2.0.0");
Console.WriteLine($"1.0.0 在范围内: {range6.IsInRange(version1)}"); // 输出: True
Console.WriteLine($"1.5.0 在范围内: {range6.IsInRange(version2)}"); // 输出: True
Console.WriteLine($"2.0.0 在范围内: {range6.IsInRange(version3)}"); // 输出: False

// 波浪号范围（~1.0.0 相当于 >=1.0.0 <1.1.0）
var range7 = VersionRange.Parse("~1.0.0");
Console.WriteLine($"1.0.5 在范围内: {range7.IsInRange(new SemanticVersion(1, 0, 5))}"); // 输出: True
Console.WriteLine($"1.1.0 在范围内: {range7.IsInRange(new SemanticVersion(1, 1, 0))}"); // 输出: False

// 插入符号范围（^1.0.0 相当于 >=1.0.0 <2.0.0）
var range8 = VersionRange.Parse("^1.0.0");
Console.WriteLine($"1.5.0 在范围内: {range8.IsInRange(version2)}"); // 输出: True
Console.WriteLine($"2.0.0 在范围内: {range8.IsInRange(version3)}"); // 输出: False
```

## 高级使用示例

### 1. 通过依赖注入使用

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
var version1 = versionService.Parse("1.0.0");
Console.WriteLine($"解析版本: {version1}");

// 递增版本
var version2 = versionService.IncrementMinor(version1);
Console.WriteLine($"递增次版本: {version2}"); // 输出: 1.1.0

// 解析版本范围
var versionRange = versionService.ParseRange(">=1.0.0 <2.0.0");

// 检查版本是否在范围内
var isInRange = versionService.IsInRange(version2, versionRange);
Console.WriteLine($"版本 {version2} 在范围内: {isInRange}"); // 输出: True

// 验证版本字符串
var isValid = versionService.IsValidVersion("invalid-version");
Console.WriteLine($"版本字符串 'invalid-version' 有效: {isValid}"); // 输出: False
```

### 2. 自定义版本格式化

```csharp
using VersionSkill;

// 创建版本对象
var version = new SemanticVersion(1, 2, 3, "beta.1", "build.1");

// 标准格式
Console.WriteLine(version.ToString()); // 输出: 1.2.3-beta.1+build.1

// 仅核心版本（主版本.次版本.补丁版本）
Console.WriteLine(version.ToCoreString()); // 输出: 1.2.3

// 仅版本号（包含预发布版本）
Console.WriteLine(version.ToVersionString()); // 输出: 1.2.3-beta.1

// 自定义格式
Console.WriteLine($"主版本: {version.Major}, 次版本: {version.Minor}, 补丁版本: {version.Patch}");
if (!string.IsNullOrEmpty(version.PreRelease))
{
    Console.WriteLine($"预发布版本: {version.PreRelease}");
}
if (!string.IsNullOrEmpty(version.BuildMetadata))
{
    Console.WriteLine($"构建元数据: {version.BuildMetadata}");
}
```

### 3. 批量版本操作

```csharp
using VersionSkill;

// 创建版本列表
var versions = new List<SemanticVersion>
{
    SemanticVersion.Parse("1.0.0"),
    SemanticVersion.Parse("1.1.0"),
    SemanticVersion.Parse("1.0.1"),
    SemanticVersion.Parse("2.0.0"),
    SemanticVersion.Parse("1.2.0")
};

// 排序版本列表
versions.Sort();
Console.WriteLine("排序后的版本列表:");
foreach (var v in versions)
{
    Console.WriteLine($"  - {v}");
}

// 找到最大版本
var maxVersion = versions.Max();
Console.WriteLine($"最大版本: {maxVersion}");

// 找到最小版本
var minVersion = versions.Min();
Console.WriteLine($"最小版本: {minVersion}");

// 过滤版本范围
var versionRange = VersionRange.Parse(">=1.0.0 <2.0.0");
var filteredVersions = versions.Where(v => versionRange.IsInRange(v)).ToList();
Console.WriteLine("在范围内的版本:");
foreach (var v in filteredVersions)
{
    Console.WriteLine($"  - {v}");
}
```

## Scrutor 集成示例

### 1. 自动服务注册

```csharp
using Microsoft.Extensions.DependencyInjection;
using VersionSkill;

// 注册服务
var services = new ServiceCollection();

// 使用 Scrutor 自动注册服务
services.Scan(scan => scan
    .FromAssemblyOf<IVersionService>()
    .AddClasses()
    .AsImplementedInterfaces()
    .WithSingletonLifetime());

var serviceProvider = services.BuildServiceProvider();

// 获取版本服务
var versionService = serviceProvider.GetRequiredService<IVersionService>();

// 使用服务
var version = versionService.Parse("1.0.0");
Console.WriteLine($"解析版本: {version}");
```

### 2. 装饰器模式

```csharp
using Microsoft.Extensions.DependencyInjection;
using VersionSkill;

// 定义版本服务装饰器
public class VersionServiceDecorator : IVersionService
{
    private readonly IVersionService _innerService;
    private readonly ILogger<VersionServiceDecorator> _logger;

    public VersionServiceDecorator(IVersionService innerService, ILogger<VersionServiceDecorator> logger)
    {
        _innerService = innerService;
        _logger = logger;
    }

    public SemanticVersion Parse(string versionString)
    {
        _logger.LogInformation($"解析版本字符串: {versionString}");
        var result = _innerService.Parse(versionString);
        _logger.LogInformation($"解析结果: {result}");
        return result;
    }

    // 实现其他方法...
    public bool IsValidVersion(string versionString) => _innerService.IsValidVersion(versionString);
    public SemanticVersion IncrementMajor(SemanticVersion version) => _innerService.IncrementMajor(version);
    public SemanticVersion IncrementMinor(SemanticVersion version) => _innerService.IncrementMinor(version);
    public SemanticVersion IncrementPatch(SemanticVersion version) => _innerService.IncrementPatch(version);
    public VersionRange ParseRange(string rangeString) => _innerService.ParseRange(rangeString);
    public bool IsInRange(SemanticVersion version, VersionRange range) => _innerService.IsInRange(version, range);
}

// 注册服务
var services = new ServiceCollection();

// 添加日志服务
services.AddLogging(builder => builder.AddConsole());

// 注册原始服务
services.AddSingleton<IVersionService, VersionService>();

// 使用 Scrutor 添加装饰器
services.Decorate<IVersionService, VersionServiceDecorator>();

var serviceProvider = services.BuildServiceProvider();

// 获取版本服务（实际上是装饰器）
var versionService = serviceProvider.GetRequiredService<IVersionService>();

// 使用服务
var version = versionService.Parse("1.0.0");
Console.WriteLine($"解析版本: {version}");
// 输出: 解析版本字符串: 1.0.0
// 输出: 解析结果: 1.0.0
// 输出: 解析版本: 1.0.0
```

### 3. 泛型服务注册

```csharp
using Microsoft.Extensions.DependencyInjection;
using VersionSkill;

// 定义泛型版本服务接口
public interface IVersionService<T> where T : SemanticVersion
{
    T Parse(string versionString);
    bool IsValidVersion(string versionString);
}

// 实现泛型版本服务
public class VersionService<T> : IVersionService<T> where T : SemanticVersion
{
    public T Parse(string versionString)
    {
        var version = SemanticVersion.Parse(versionString);
        return (T)version;
    }

    public bool IsValidVersion(string versionString)
    {
        return SemanticVersion.TryParse(versionString, out _);
    }
}

// 注册服务
var services = new ServiceCollection();

// 使用 Scrutor 注册泛型服务
services.Scan(scan => scan
    .FromAssemblyOf<VersionService<SemanticVersion>>()
    .AddClasses()
    .AsImplementedInterfaces()
    .WithSingletonLifetime());

var serviceProvider = services.BuildServiceProvider();

// 获取泛型版本服务
var versionService = serviceProvider.GetRequiredService<IVersionService<SemanticVersion>>();

// 使用服务
var version = versionService.Parse("1.0.0");
Console.WriteLine($"解析版本: {version}");

var isValid = versionService.IsValidVersion("2.0.0");
Console.WriteLine($"版本字符串 '2.0.0' 有效: {isValid}");
```

## 性能测试示例

### 1. 版本解析性能测试

```csharp
using VersionSkill;
using System.Diagnostics;

// 测试版本解析性能
var stopwatch = Stopwatch.StartNew();
int iterations = 100000;
string versionString = "1.0.0";

for (int i = 0; i < iterations; i++)
{
    var version = SemanticVersion.Parse(versionString);
}

stopwatch.Stop();
Console.WriteLine($"解析 {iterations} 次版本字符串 '{versionString}' 耗时: {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"平均每次解析耗时: {stopwatch.Elapsed.TotalMilliseconds / iterations:F6}ms");
```

### 2. 版本比较性能测试

```csharp
using VersionSkill;
using System.Diagnostics;

// 测试版本比较性能
var stopwatch = Stopwatch.StartNew();
int iterations = 100000;
var version1 = new SemanticVersion(1, 0, 0);
var version2 = new SemanticVersion(1, 1, 0);

for (int i = 0; i < iterations; i++)
{
    var result = version1 < version2;
}

stopwatch.Stop();
Console.WriteLine($"比较 {iterations} 次版本耗时: {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"平均每次比较耗时: {stopwatch.Elapsed.TotalMilliseconds / iterations:F6}ms");
```

### 3. 版本范围检查性能测试

```csharp
using VersionSkill;
using System.Diagnostics;

// 测试版本范围检查性能
var stopwatch = Stopwatch.StartNew();
int iterations = 100000;
var version = new SemanticVersion(1, 5, 0);
var versionRange = VersionRange.Parse(">=1.0.0 <2.0.0");

for (int i = 0; i < iterations; i++)
{
    var result = versionRange.IsInRange(version);
}

stopwatch.Stop();
Console.WriteLine($"检查 {iterations} 次版本范围耗时: {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"平均每次检查耗时: {stopwatch.Elapsed.TotalMilliseconds / iterations:F6}ms");
```

### 4. 缓存性能测试

```csharp
using Microsoft.Extensions.DependencyInjection;
using VersionSkill;
using System.Diagnostics;

// 注册服务（启用缓存）
var services = new ServiceCollection();
services.AddVersionSkill(options =>
{
    options.CacheEnabled = true;
    options.CacheSize = 1000;
});
var serviceProvider = services.BuildServiceProvider();

// 获取版本服务
var versionService = serviceProvider.GetRequiredService<IVersionService>();

// 测试缓存性能
var stopwatch = Stopwatch.StartNew();
int iterations = 100000;
string versionString = "1.0.0";

for (int i = 0; i < iterations; i++)
{
    var version = versionService.Parse(versionString);
}

stopwatch.Stop();
Console.WriteLine($"使用缓存解析 {iterations} 次版本字符串 '{versionString}' 耗时: {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"平均每次解析耗时: {stopwatch.Elapsed.TotalMilliseconds / iterations:F6}ms");

// 禁用缓存测试
var services2 = new ServiceCollection();
services2.AddVersionSkill(options =>
{
    options.CacheEnabled = false;
});
var serviceProvider2 = services2.BuildServiceProvider();

var versionService2 = serviceProvider2.GetRequiredService<IVersionService>();

stopwatch.Restart();
for (int i = 0; i < iterations; i++)
{
    var version = versionService2.Parse(versionString);
}

stopwatch.Stop();
Console.WriteLine($"禁用缓存解析 {iterations} 次版本字符串 '{versionString}' 耗时: {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"平均每次解析耗时: {stopwatch.Elapsed.TotalMilliseconds / iterations:F6}ms");
```

## 实际应用示例

### 1. 版本控制工具

```csharp
using VersionSkill;

public class VersionManager
{
    private readonly IVersionService _versionService;

    public VersionManager(IVersionService versionService)
    {
        _versionService = versionService;
    }

    public string GetNextVersion(string currentVersion, string versionType)
    {
        var version = _versionService.Parse(currentVersion);
        SemanticVersion nextVersion;

        switch (versionType.ToLower())
        {
            case "major":
                nextVersion = _versionService.IncrementMajor(version);
                break;
            case "minor":
                nextVersion = _versionService.IncrementMinor(version);
                break;
            case "patch":
            default:
                nextVersion = _versionService.IncrementPatch(version);
                break;
        }

        return nextVersion.ToString();
    }

    public bool IsVersionCompatible(string currentVersion, string requiredVersionRange)
    {
        var version = _versionService.Parse(currentVersion);
        var versionRange = _versionService.ParseRange(requiredVersionRange);
        return _versionService.IsInRange(version, versionRange);
    }
}

// 使用示例
var services = new ServiceCollection();
services.AddVersionSkill();
var serviceProvider = services.BuildServiceProvider();

var versionService = serviceProvider.GetRequiredService<IVersionService>();
var versionManager = new VersionManager(versionService);

// 获取下一个版本
string currentVersion = "1.0.0";
string nextMajorVersion = versionManager.GetNextVersion(currentVersion, "major");
string nextMinorVersion = versionManager.GetNextVersion(currentVersion, "minor");
string nextPatchVersion = versionManager.GetNextVersion(currentVersion, "patch");

Console.WriteLine($"当前版本: {currentVersion}");
Console.WriteLine($"下一个主版本: {nextMajorVersion}"); // 输出: 2.0.0
Console.WriteLine($"下一个次版本: {nextMinorVersion}"); // 输出: 1.1.0
Console.WriteLine($"下一个补丁版本: {nextPatchVersion}"); // 输出: 1.0.1

// 检查版本兼容性
string requiredRange = ">=1.0.0 <2.0.0";
bool isCompatible = versionManager.IsVersionCompatible("1.5.0", requiredRange);
Console.WriteLine($"版本 1.5.0 兼容范围 {requiredRange}: {isCompatible}"); // 输出: True

isCompatible = versionManager.IsVersionCompatible("2.0.0", requiredRange);
Console.WriteLine($"版本 2.0.0 兼容范围 {requiredRange}: {isCompatible}"); // 输出: False
```

### 2. 依赖版本管理

```csharp
using VersionSkill;

public class DependencyManager
{
    private readonly IVersionService _versionService;
    private readonly Dictionary<string, string> _dependencies;

    public DependencyManager(IVersionService versionService)
    {
        _versionService = versionService;
        _dependencies = new Dictionary<string, string>();
    }

    public void AddDependency(string packageName, string versionRange)
    {
        _dependencies[packageName] = versionRange;
    }

    public bool IsVersionSatisfies(string packageName, string version)
    {
        if (!_dependencies.TryGetValue(packageName, out var versionRangeString))
        {
            return false;
        }

        var versionObj = _versionService.Parse(version);
        var versionRange = _versionService.ParseRange(versionRangeString);
        return _versionService.IsInRange(versionObj, versionRange);
    }

    public IEnumerable<string> GetUnsatisfiedDependencies(Dictionary<string, string> installedVersions)
    {
        var unsatisfied = new List<string>();

        foreach (var dependency in _dependencies)
        {
            var packageName = dependency.Key;
            var versionRange = dependency.Value;

            if (installedVersions.TryGetValue(packageName, out var installedVersion))
            {
                if (!IsVersionSatisfies(packageName, installedVersion))
                {
                    unsatisfied.Add($"{packageName}: 安装版本 {installedVersion} 不满足要求 {versionRange}");
                }
            }
            else
            {
                unsatisfied.Add($"{packageName}: 未安装");
            }
        }

        return unsatisfied;
    }
}

// 使用示例
var services = new ServiceCollection();
services.AddVersionSkill();
var serviceProvider = services.BuildServiceProvider();

var versionService = serviceProvider.GetRequiredService<IVersionService>();
var dependencyManager = new DependencyManager(versionService);

// 添加依赖
 dependencyManager.AddDependency("Newtonsoft.Json", ">=13.0.0 <14.0.0");
 dependencyManager.AddDependency("Microsoft.Extensions.DependencyInjection", ">=8.0.0");
 dependencyManager.AddDependency("Scrutor", "^4.0.0");

// 模拟已安装的依赖
var installedVersions = new Dictionary<string, string>
{
    { "Newtonsoft.Json", "13.0.1" },
    { "Microsoft.Extensions.DependencyInjection", "7.0.0" },
    { "Scrutor", "4.2.0" }
};

// 检查未满足的依赖
var unsatisfiedDependencies = dependencyManager.GetUnsatisfiedDependencies(installedVersions);

Console.WriteLine("未满足的依赖:");
foreach (var dependency in unsatisfiedDependencies)
{
    Console.WriteLine($"  - {dependency}");
}
// 输出:
// 未满足的依赖:
//   - Microsoft.Extensions.DependencyInjection: 安装版本 7.0.0 不满足要求 >=8.0.0
```