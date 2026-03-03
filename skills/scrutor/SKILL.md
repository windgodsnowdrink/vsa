# Scrutor 技能文档

## 1. 技能概述

Scrutor 技能是一个基于 Scrutor 库的依赖注入扩展工具，专为 .NET 10 设计，支持 Ahead-of-Time (AOT) 编译。它提供了强大的程序集扫描、服务装饰和代码生成功能，简化了依赖注入的配置和管理。

### 1.1 核心功能

- **程序集扫描**：自动扫描程序集并注册服务，支持类型匹配模式和生命周期配置
- **服务装饰**：为已注册的服务添加装饰器，实现横切关注点（如日志、缓存、事务等）
- **代码生成**：生成依赖注入配置代码，支持不同的输出格式和命名空间
- **AOT 兼容**：支持 AOT 编译，优化运行时性能
- **命令行接口**：提供直观的命令行工具，方便使用和集成

### 1.2 技术特点

- **基于 .NET 10**：利用最新的 .NET 10 特性和改进
- **AOT 编译**：启用 PublishAot=true，提高启动速度和运行时性能
- **单文件发布**：支持 PublishSingleFile=true，生成单个可执行文件
- **跨平台**：支持 Windows、Linux 和 macOS
- **模块化设计**：采用接口分离和依赖注入，易于扩展
- **性能优化**：优化程序集扫描和服务注册过程

## 2. 快速开始

### 2.1 环境要求

- .NET 10 SDK 或更高版本
- Windows、Linux 或 macOS 操作系统
- 支持 AOT 编译的硬件平台

### 2.2 安装和配置

1. **下载技能包**：从官方渠道下载 Scrutor 技能包
2. **解压到指定目录**：将技能包解压到您的项目目录中
3. **配置环境变量**：
   ```bash
   # Windows
   set DOTNET_ENVIRONMENT=Production
   set SCRUTOR_CONFIG_PATH=appsettings.json

   # Linux/macOS
   export DOTNET_ENVIRONMENT=Production
   export SCRUTOR_CONFIG_PATH=appsettings.json
   ```

### 2.3 基本使用示例

#### 2.3.1 扫描程序集并注册服务

```bash
# 扫描当前目录下的所有程序集
scrutor_core scan --assembly "." --pattern "*Service" --lifetime "scoped"

# 扫描指定程序集
scrutor_core scan --assembly "MyApp.dll" --pattern "*Repository" --lifetime "singleton"
```

#### 2.3.2 为服务添加装饰器

```bash
# 为 IMyService 添加装饰器
scrutor_core decorate --service "IMyService" --decorator "MyServiceDecorator" --lifetime "scoped"
```

#### 2.3.3 生成依赖注入配置代码

```bash
# 生成依赖注入配置代码
scrutor_core generate --output "DependencyInjection.cs" --namespace "MyApp.DependencyInjection" --class "DependencyInjection"
```

#### 2.3.4 列出已注册的服务

```bash
# 列出已注册的服务（文本格式）
scrutor_core list --format "text"

# 列出已注册的服务（JSON 格式）
scrutor_core list --format "json"
```

## 3. 核心功能

### 3.1 程序集扫描

程序集扫描是 Scrutor 的核心功能之一，它允许您自动扫描程序集并注册服务，而无需手动配置每个服务。

#### 3.1.1 基本扫描

```bash
# 扫描当前目录下的所有程序集
scrutor_core scan --assembly "."

# 扫描指定程序集
scrutor_core scan --assembly "MyApp.dll"
```

#### 3.1.2 高级扫描选项

- **类型匹配模式**：使用通配符匹配类型名称
  ```bash
  scrutor_core scan --assembly "." --pattern "*Service"
  ```

- **服务生命周期**：指定服务的生命周期（singleton/transient/scoped）
  ```bash
  scrutor_core scan --assembly "." --lifetime "scoped"
  ```

- **组合选项**：组合多个选项
  ```bash
  scrutor_core scan --assembly "MyApp.dll" --pattern "*Repository" --lifetime "singleton"
  ```

### 3.2 服务装饰

服务装饰允许您为已注册的服务添加装饰器，实现横切关注点（如日志、缓存、事务等），而无需修改原始服务代码。

#### 3.2.1 基本装饰

```bash
# 为 IMyService 添加装饰器
scrutor_core decorate --service "IMyService" --decorator "MyServiceDecorator"
```

#### 3.2.2 装饰器生命周期

```bash
# 为 IMyService 添加装饰器，并指定生命周期
scrutor_core decorate --service "IMyService" --decorator "MyServiceDecorator" --lifetime "scoped"
```

#### 3.2.3 多层装饰

您可以为同一个服务添加多个装饰器，形成装饰器链：

```bash
# 添加第一个装饰器
scrutor_core decorate --service "IMyService" --decorator "LoggingDecorator"

# 添加第二个装饰器
scrutor_core decorate --service "IMyService" --decorator "CachingDecorator"
```

### 3.3 代码生成

代码生成功能允许您生成依赖注入配置代码，支持不同的输出格式和命名空间，简化了依赖注入的配置过程。

#### 3.3.1 基本生成

```bash
# 生成依赖注入配置代码
scrutor_core generate --output "DependencyInjection.cs"
```

#### 3.3.2 自定义命名空间和类名

```bash
# 生成依赖注入配置代码，自定义命名空间和类名
scrutor_core generate --output "DependencyInjection.cs" --namespace "MyApp.DependencyInjection" --class "DependencyInjection"
```

#### 3.3.3 生成选项

- **输出格式**：支持 C# 代码格式
- **命名空间**：自定义生成代码的命名空间
- **类名**：自定义生成代码的类名
- **输出路径**：指定生成文件的输出路径

### 3.4 服务管理

Scrutor 提供了丰富的服务管理功能，包括列出已注册的服务、检查服务状态等。

#### 3.4.1 列出已注册的服务

```bash
# 列出已注册的服务（文本格式）
scrutor_core list --format "text"

# 列出已注册的服务（JSON 格式）
scrutor_core list --format "json"
```

#### 3.4.2 服务状态检查

Scrutor 会自动检查服务的注册状态，确保所有依赖项都已正确解析。

## 4. API 参考

### 4.1 命令行接口

#### 4.1.1 scan 命令

扫描程序集并注册服务。

**语法**：
```bash
scrutor_core scan --assembly <assembly> [--pattern <pattern>] [--lifetime <lifetime>]
```

**参数**：
- `--assembly`：要扫描的程序集路径（必需）
- `--pattern`：类型匹配模式（可选）
- `--lifetime`：服务生命周期（可选，默认：scoped）

#### 4.1.2 decorate 命令

为服务添加装饰器。

**语法**：
```bash
scrutor_core decorate --service <service> --decorator <decorator> [--lifetime <lifetime>]
```

**参数**：
- `--service`：服务类型（必需）
- `--decorator`：装饰器类型（必需）
- `--lifetime`：服务生命周期（可选，默认：scoped）

#### 4.1.3 generate 命令

生成依赖注入配置代码。

**语法**：
```bash
scrutor_core generate --output <output> [--namespace <namespace>] [--class <class>]
```

**参数**：
- `--output`：输出文件路径（必需）
- `--namespace`：命名空间（可选，默认：Scrutor.Generated）
- `--class`：类名（可选，默认：DependencyInjection）

#### 4.1.4 list 命令

列出已注册的服务。

**语法**：
```bash
scrutor_core list [--format <format>]
```

**参数**：
- `--format`：输出格式（可选，默认：text）

### 4.2 核心 API

#### 4.2.1 IAssemblyScanner

程序集扫描器接口，用于扫描程序集并注册服务。

```csharp
public interface IAssemblyScanner
{
    IAssemblyScanner FromAssemblies(params Assembly[] assemblies);
    IAssemblyScanner FromAssemblies(IEnumerable<Assembly> assemblies);
    IAssemblyScanner FromApplicationDependencies();
    IAssemblyScanner AddClasses(bool publicOnly = true);
    IAssemblyScanner AddClasses(Action<IImplementationTypeFilter> action);
    IAssemblyScanner AddClasses<T>(bool publicOnly = true) where T : class;
    IAssemblyScanner AddClasses<T>(Action<IImplementationTypeFilter> action) where T : class;
    void AsImplementedInterfaces();
    void AsSelf();
    void As<T>();
    void WithSingletonLifetime();
    void WithTransientLifetime();
    void WithScopedLifetime();
    void WithLifetime(ServiceLifetime lifetime);
}
```

#### 4.2.2 IServiceDecorator

服务装饰器接口，用于为服务添加装饰器。

```csharp
public interface IServiceDecorator
{
    IServiceDecorator Decorate<TService, TDecorator>() where TService : class where TDecorator : class, TService;
    IServiceDecorator Decorate(Type serviceType, Type decoratorType);
    IServiceDecorator WithSingletonLifetime();
    IServiceDecorator WithTransientLifetime();
    IServiceDecorator WithScopedLifetime();
    IServiceDecorator WithLifetime(ServiceLifetime lifetime);
}
```

#### 4.2.3 ICodeGenerator

代码生成器接口，用于生成依赖注入配置代码。

```csharp
public interface ICodeGenerator
{
    string Generate(string outputPath, string @namespace, string className);
    string Generate(string outputPath, string @namespace, string className, IEnumerable<ServiceDescriptor> services);
}
```

## 5. AOT 编译

### 5.1 AOT 编译配置

Scrutor 技能支持 AOT 编译，通过以下配置启用：

```json
{
  "publishAot": true,
  "trimMode": "partial",
  "selfContained": true,
  "publishSingleFile": true
}
```

### 5.2 AOT 编译的优势

- **更快的启动速度**：AOT 编译将代码编译为本地机器码，减少了 JIT 编译的开销
- **更低的内存使用**：AOT 编译生成的代码更加紧凑，减少了内存使用
- **更好的性能**：本地机器码执行速度更快，特别是对于热点路径
- **更小的部署包**：单文件发布减少了部署包的大小
- **无需运行时**：自包含发布包含了所有必要的运行时组件

### 5.3 AOT 编译的注意事项

- **反射限制**：AOT 编译会限制反射的使用，需要确保所有反射操作都在编译时可见
- **动态代码生成**：AOT 编译不支持动态代码生成，需要避免使用 `System.Reflection.Emit` 等功能
- **类型转发**：需要确保所有类型都在编译时解析，避免类型转发问题
- **资源文件**：需要确保所有资源文件都在编译时包含，避免运行时加载问题

## 6. 配置管理

### 6.1 配置文件

Scrutor 技能使用 `appsettings.json` 文件进行配置：

```json
{
  "Scrutor": {
    "AssemblyScan": {
      "Assemblies": ["MyApp.dll", "MyApp.Services.dll"],
      "Pattern": "*Service",
      "Lifetime": "Scoped"
    },
    "Decoration": {
      "Decorators": [
        {
          "Service": "IMyService",
          "Decorator": "MyServiceDecorator",
          "Lifetime": "Scoped"
        }
      ]
    },
    "CodeGeneration": {
      "OutputPath": "DependencyInjection.cs",
      "Namespace": "MyApp.DependencyInjection",
      "ClassName": "DependencyInjection"
    }
  }
}
```

### 6.2 环境变量

Scrutor 技能支持通过环境变量进行配置：

| 环境变量 | 描述 | 默认值 |
|---------|------|--------|
| DOTNET_ENVIRONMENT | .NET 环境 | Production |
| SCRUTOR_CONFIG_PATH | 配置文件路径 | appsettings.json |
| SCRUTOR_ASSEMBLY_PATH | 程序集路径 | . |
| SCRUTOR_PATTERN | 类型匹配模式 | * |
| SCRUTOR_LIFETIME | 服务生命周期 | Scoped |

### 6.3 命令行参数

Scrutor 技能支持通过命令行参数覆盖配置：

```bash
# 覆盖程序集路径
scrutor_core scan --assembly "MyApp.dll"

# 覆盖类型匹配模式
scrutor_core scan --assembly "." --pattern "*Service"

# 覆盖服务生命周期
scrutor_core scan --assembly "." --lifetime "singleton"
```

## 7. 最佳实践

### 7.1 程序集扫描最佳实践

- **限制扫描范围**：只扫描必要的程序集，避免扫描过多的程序集
- **使用具体的匹配模式**：使用具体的类型匹配模式，减少匹配的类型数量
- **合理设置生命周期**：根据服务的性质设置合理的生命周期
- **避免循环依赖**：确保服务之间没有循环依赖

### 7.2 服务装饰最佳实践

- **保持装饰器简单**：每个装饰器只负责一个横切关注点
- **遵循装饰器模式**：确保装饰器实现与原始服务相同的接口
- **合理组织装饰器链**：根据横切关注点的优先级组织装饰器链
- **避免过度装饰**：不要为每个服务添加过多的装饰器，以免影响性能

### 7.3 代码生成最佳实践

- **定期生成代码**：在程序集变更后定期生成依赖注入配置代码
- **自定义命名空间**：使用与项目结构匹配的命名空间
- **代码审查**：生成代码后进行代码审查，确保生成的代码符合项目规范
- **版本控制**：将生成的代码纳入版本控制，以便跟踪变更

### 7.4 AOT 编译最佳实践

- **测试 AOT 编译**：在开发过程中定期测试 AOT 编译，确保代码兼容
- **避免反射**：尽量避免使用反射，或使用编译时反射替代
- **使用 trim 友好的库**：使用支持 trim 的库，避免使用不支持 trim 的库
- **优化资源使用**：优化内存和 CPU 使用，充分利用 AOT 编译的优势

## 8. 故障排除

### 8.1 常见问题

#### 8.1.1 程序集扫描失败

**症状**：程序集扫描命令执行失败，显示错误信息。

**原因**：
- 程序集路径不存在
- 程序集损坏
- 权限不足

**解决方案**：
- 检查程序集路径是否正确
- 检查程序集是否损坏
- 确保有足够的权限访问程序集

#### 8.1.2 服务注册失败

**症状**：服务注册命令执行失败，显示错误信息。

**原因**：
- 类型不存在
- 类型不匹配
- 循环依赖

**解决方案**：
- 检查类型是否存在
- 检查类型是否匹配
- 解决循环依赖问题

#### 8.1.3 装饰器添加失败

**症状**：装饰器添加命令执行失败，显示错误信息。

**原因**：
- 服务类型不存在
- 装饰器类型不存在
- 装饰器不实现服务接口

**解决方案**：
- 检查服务类型是否存在
- 检查装饰器类型是否存在
- 确保装饰器实现了服务接口

#### 8.1.4 代码生成失败

**症状**：代码生成命令执行失败，显示错误信息。

**原因**：
- 输出路径不存在
- 权限不足
- 配置错误

**解决方案**：
- 检查输出路径是否存在
- 确保有足够的权限写入输出文件
- 检查配置是否正确

### 8.2 日志和诊断

Scrutor 技能提供了详细的日志和诊断信息，帮助您排查问题：

- **命令行日志**：命令执行过程中的详细日志
- **配置诊断**：配置文件的加载和解析信息
- **服务诊断**：服务注册和解析的详细信息
- **AOT 诊断**：AOT 编译的详细信息

### 8.3 性能问题

#### 8.3.1 程序集扫描性能

**症状**：程序集扫描速度慢，影响启动时间。

**解决方案**：
- 限制扫描范围，只扫描必要的程序集
- 使用具体的匹配模式，减少匹配的类型数量
- 考虑使用预编译的依赖注入配置，避免运行时扫描

#### 8.3.2 服务解析性能

**症状**：服务解析速度慢，影响请求处理时间。

**解决方案**：
- 合理设置服务生命周期，避免频繁创建和销毁服务
- 避免过度装饰，减少装饰器链的长度
- 考虑使用服务缓存，减少服务解析的开销

## 9. 示例应用

### 9.1 基本 Web 应用

以下是一个使用 Scrutor 的基本 Web 应用示例：

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 使用 Scrutor 扫描程序集并注册服务
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

// 为服务添加装饰器
builder.Services.Decorate<IMyService, LoggingDecorator>();
builder.Services.Decorate<IMyService, CachingDecorator>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.Run();
```

### 9.2 高级企业应用

以下是一个使用 Scrutor 的高级企业应用示例：

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 配置 Scrutor
var assemblyPaths = builder.Configuration.GetSection("Scrutor:AssemblyScan:Assemblies").Get<string[]>() ?? new string[0];
var pattern = builder.Configuration.GetValue<string>("Scrutor:AssemblyScan:Pattern", "*Service");
var lifetime = builder.Configuration.GetValue<string>("Scrutor:AssemblyScan:Lifetime", "Scoped");

// 扫描多个程序集
builder.Services.Scan(scan => {
    foreach (var assemblyPath in assemblyPaths)
    {
        scan.FromAssemblyPath(assemblyPath);
    }
    scan.AddClasses(classes => classes.Where(type => type.Name.EndsWith(pattern)))
        .AsImplementedInterfaces()
        .WithLifetime((ServiceLifetime)Enum.Parse(typeof(ServiceLifetime), lifetime));
});

// 添加装饰器
var decorators = builder.Configuration.GetSection("Scrutor:Decoration:Decorators").Get<List<DecoratorConfig>>() ?? new List<DecoratorConfig>();
foreach (var decorator in decorators)
{
    var serviceType = Type.GetType(decorator.Service);
    var decoratorType = Type.GetType(decorator.Decorator);
    if (serviceType != null && decoratorType != null)
    {
        builder.Services.Decorate(serviceType, decoratorType);
    }
}

var app = builder.Build();

app.MapGet("/", () => "Hello Enterprise!");
app.Run();

public class DecoratorConfig
{
    public string Service { get; set; }
    public string Decorator { get; set; }
    public string Lifetime { get; set; }
}
```

## 10. 总结

Scrutor 技能是一个强大的依赖注入扩展工具，它通过程序集扫描、服务装饰和代码生成等功能，简化了依赖注入的配置和管理。同时，它支持 AOT 编译，提供了更好的性能和更小的部署包。

通过本文档的介绍，您应该已经了解了 Scrutor 技能的核心功能、使用方法和最佳实践。现在，您可以开始在您的项目中使用 Scrutor 技能，简化依赖注入的配置和管理，提高应用程序的性能和可维护性。

### 10.1 未来发展

Scrutor 技能将继续发展和改进，未来可能会添加以下功能：

- **更多的扫描选项**：支持更多的程序集扫描选项，如基于属性的扫描
- **更丰富的装饰器支持**：支持更多的装饰器模式和选项
- **更强大的代码生成**：支持更多的代码生成选项和格式
- **更好的 AOT 支持**：进一步优化 AOT 编译的兼容性和性能
- **更多的集成**：与更多的框架和库集成，如 ASP.NET Core、Blazor 等

### 10.2 社区贡献

Scrutor 技能是一个开源项目，欢迎社区贡献：

- **提交问题**：在 GitHub 上提交问题和建议
- **贡献代码**：提交 Pull Request，贡献代码和改进
- **文档改进**：改进文档，添加更多的示例和最佳实践
- **测试**：测试 Scrutor 技能，确保其质量和稳定性

通过社区的共同努力，Scrutor 技能将变得更加完善和强大，为 .NET 开发者提供更好的依赖注入解决方案。