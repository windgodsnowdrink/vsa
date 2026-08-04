# Scrutor 技能技术参考文档

## 1. 架构概述

Scrutor 技能是一个基于 Scrutor 库的依赖注入扩展工具，专为 .NET 10 设计，支持 Ahead-of-Time (AOT) 编译。它提供了强大的程序集扫描、服务装饰和代码生成功能，简化了依赖注入的配置和管理。

### 1.1 核心组件

- **程序集扫描器 (IAssemblyScanner)**：负责扫描程序集并注册服务
- **服务装饰器 (IServiceDecorator)**：负责为服务添加装饰器
- **服务注册表 (IServiceRegistry)**：负责管理已注册的服务
- **代码生成器 (ICodeGenerator)**：负责生成依赖注入配置代码
- **模板管理器 (ITemplateManager)**：负责管理代码模板

### 1.2 工作流程

1. **程序集扫描**：扫描指定的程序集，查找符合条件的类型
2. **服务注册**：将找到的类型注册为服务
3. **服务装饰**：为已注册的服务添加装饰器
4. **代码生成**：生成依赖注入配置代码
5. **服务管理**：管理已注册的服务，提供查询和统计功能

## 2. 目录结构

```
scrutor/
├── index.yaml           # 技能配置文件
├── SKILL.md             # 技能文档
├── scripts/             # 脚本目录
│   ├── scrutor_core.cs                  # 核心功能实现
│   ├── scrutor_core.setting.json        # 核心功能配置
│   ├── scrutor_core.run.json            # 核心功能运行配置
│   ├── scrutor_generator.cs             # 代码生成功能实现
│   ├── scrutor_generator.setting.json   # 代码生成配置
│   └── scrutor_generator.run.json       # 代码生成运行配置
└── reference/           # 参考文档目录
    ├── README.md        # 技术参考文档
    └── examples.md      # 使用示例文档
```

## 3. 核心 API

### 3.1 IAssemblyScanner

程序集扫描器接口，用于扫描程序集并注册服务。

**接口定义**：

```csharp
public interface IAssemblyScanner
{
    Task<Assembly[]> LoadAssembliesAsync(string path, CancellationToken cancellationToken = default);
    Task<List<ServiceDescriptor>> ScanAssembliesAsync(Assembly[] assemblies, string pattern, string lifetime, CancellationToken cancellationToken = default);
}
```

**方法说明**：
- `LoadAssembliesAsync`：加载指定路径下的程序集
- `ScanAssembliesAsync`：扫描程序集并注册服务

### 3.2 IServiceDecorator

服务装饰器接口，用于为服务添加装饰器。

**接口定义**：

```csharp
public interface IServiceDecorator
{
    Task<ServiceDescriptor> DecorateServiceAsync(Type serviceType, Type decoratorType, string lifetime, CancellationToken cancellationToken = default);
}
```

**方法说明**：
- `DecorateServiceAsync`：为服务添加装饰器

### 3.3 IServiceRegistry

服务注册表接口，用于管理已注册的服务。

**接口定义**：

```csharp
public interface IServiceRegistry
{
    void RegisterService(ServiceDescriptor service);
    List<ServiceDescriptor> GetRegisteredServices();
}
```

**方法说明**：
- `RegisterService`：注册服务
- `GetRegisteredServices`：获取已注册的服务

### 3.4 ICodeGenerator

代码生成器接口，用于生成依赖注入配置代码。

**接口定义**：

```csharp
public interface ICodeGenerator
{
    Task<string> GenerateAsync(string outputPath, string @namespace, string className, CancellationToken cancellationToken = default);
}
```

**方法说明**：
- `GenerateAsync`：生成依赖注入配置代码

### 3.5 ITemplateManager

模板管理器接口，用于管理代码模板。

**接口定义**：

```csharp
public interface ITemplateManager
{
    Task<IEnumerable<TemplateInfo>> GetTemplatesAsync(CancellationToken cancellationToken = default);
    Task<string> GetTemplateAsync(string name, CancellationToken cancellationToken = default);
}
```

**方法说明**：
- `GetTemplatesAsync`：获取可用的模板
- `GetTemplateAsync`：获取指定的模板

## 4. 命令行接口

### 4.1 核心命令

#### 4.1.1 scan 命令

扫描程序集并注册服务。

**语法**：
```bash
scrutor_core scan --assembly <assembly> [--pattern <pattern>] [--lifetime <lifetime>]
```

**参数**：
- `--assembly`：要扫描的程序集路径（必需）
- `--pattern`：类型匹配模式（可选，默认：*）
- `--lifetime`：服务生命周期（可选，默认：scoped）

**示例**：
```bash
# 扫描当前目录下的所有程序集
scrutor_core scan --assembly "."

# 扫描指定程序集，匹配 *Service 后缀的类型
scrutor_core scan --assembly "MyApp.dll" --pattern "*Service" --lifetime "scoped"
```

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

**示例**：
```bash
# 为 IMyService 添加装饰器
scrutor_core decorate --service "IMyService" --decorator "MyServiceDecorator"
```

#### 4.1.3 list 命令

列出已注册的服务。

**语法**：
```bash
scrutor_core list [--format <format>]
```

**参数**：
- `--format`：输出格式（可选，默认：text）

**示例**：
```bash
# 以文本格式列出服务
scrutor_core list

# 以 JSON 格式列出服务
scrutor_core list --format "json"
```

### 4.2 代码生成命令

#### 4.2.1 generate 命令

生成依赖注入配置代码。

**语法**：
```bash
scrutor_generator generate --output <output> [--namespace <namespace>] [--class <class>]
```

**参数**：
- `--output`：输出文件路径（必需）
- `--namespace`：命名空间（可选，默认：Scrutor.Generated）
- `--class`：类名（可选，默认：DependencyInjection）

**示例**：
```bash
# 生成依赖注入配置代码
scrutor_generator generate --output "DependencyInjection.cs"

# 生成指定命名空间和类名的代码
scrutor_generator generate --output "DependencyInjection.cs" --namespace "MyApp.DependencyInjection" --class "DependencyInjection"
```

#### 4.2.2 template list 命令

列出可用的代码模板。

**语法**：
```bash
scrutor_generator template list
```

**示例**：
```bash
# 列出可用的代码模板
scrutor_generator template list
```

## 5. 配置选项

### 5.1 编译配置 (.setting.json)

**核心功能配置** (scrutor_core.setting.json)：

```json
{
  "version": "1.0",
  "compileOptions": {
    "targetFramework": "net10.0",
    "langVersion": "preview",
    "nullable": "enable",
    "implicitUsings": "enable",
    "publishAot": true,
    "trimMode": "partial",
    "selfContained": true,
    "publishSingleFile": true,
    "runtimeIdentifier": "win-x64"
  },
  "dependencies": {
    "Microsoft.Extensions.DependencyInjection": "8.0.0",
    "Microsoft.Extensions.Configuration": "8.0.0",
    "Microsoft.Extensions.Configuration.Json": "8.0.0",
    "System.CommandLine": "2.0.0",
    "Scrutor": "4.2.2",
    "System.Text.Json": "8.0.0"
  }
}
```

**代码生成配置** (scrutor_generator.setting.json)：

```json
{
  "version": "1.0",
  "compileOptions": {
    "targetFramework": "net10.0",
    "langVersion": "preview",
    "nullable": "enable",
    "implicitUsings": "enable",
    "publishAot": true,
    "trimMode": "partial",
    "selfContained": true,
    "publishSingleFile": true,
    "runtimeIdentifier": "win-x64"
  },
  "dependencies": {
    "Microsoft.Extensions.DependencyInjection": "8.0.0",
    "Microsoft.Extensions.Configuration": "8.0.0",
    "Microsoft.Extensions.Configuration.Json": "8.0.0",
    "System.CommandLine": "2.0.0",
    "System.Text.Json": "8.0.0"
  }
}
```

### 5.2 运行配置 (.run.json)

**核心功能运行配置** (scrutor_core.run.json)：

```json
{
  "version": "1.0",
  "profiles": {
    "Scan Assembly": {
      "commandName": "Project",
      "commandLineArgs": "scan --assembly \"..\" --pattern \"*Service\" --lifetime \"scoped\"",
      "workingDirectory": "scripts",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    }
    // 其他配置...
  }
}
```

**代码生成运行配置** (scrutor_generator.run.json)：

```json
{
  "version": "1.0",
  "profiles": {
    "Generate Dependency Injection": {
      "commandName": "Project",
      "commandLineArgs": "generate --output \"..\\generated\\DependencyInjection.cs\" --namespace \"Scrutor.Generated\" --class \"DependencyInjection\"",
      "workingDirectory": "scripts",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    }
    // 其他配置...
  }
}
```

## 6. AOT 编译配置

为了支持 AOT 编译，系统使用了以下配置：

```json
{
  "publishAot": true,
  "trimMode": "partial",
  "selfContained": true,
  "publishSingleFile": true
}
```

**关键配置说明**：
- `publishAot`：启用 AOT 编译
- `trimMode`：设置为 partial，保留必要的反射信息
- `selfContained`：生成自包含的可执行文件
- `publishSingleFile`：生成单个可执行文件

### 6.1 AOT 编译的优势

- **更快的启动速度**：AOT 编译将代码编译为本地机器码，减少了 JIT 编译的开销
- **更低的内存使用**：AOT 编译生成的代码更加紧凑，减少了内存使用
- **更好的性能**：本地机器码执行速度更快，特别是对于热点路径
- **更小的部署包**：单文件发布减少了部署包的大小
- **无需运行时**：自包含发布包含了所有必要的运行时组件

### 6.2 AOT 编译的注意事项

- **反射限制**：AOT 编译会限制反射的使用，需要确保所有反射操作都在编译时可见
- **动态代码生成**：AOT 编译不支持动态代码生成，需要避免使用 `System.Reflection.Emit` 等功能
- **类型转发**：需要确保所有类型都在编译时解析，避免类型转发问题
- **资源文件**：需要确保所有资源文件都在编译时包含，避免运行时加载问题

## 7. 依赖项

### 7.1 核心依赖项

| 依赖项 | 版本 | 用途 |
|-------|------|------|
| Microsoft.Extensions.DependencyInjection | 8.0.0 | 依赖注入容器 |
| Microsoft.Extensions.Configuration | 8.0.0 | 配置管理 |
| Microsoft.Extensions.Configuration.Json | 8.0.0 | JSON 配置支持 |
| System.CommandLine | 2.0.0 | 命令行解析库 |
| Scrutor | 4.2.2 | 依赖注入扩展库 |
| System.Text.Json | 8.0.0 | JSON 序列化库 |

## 8. 性能优化

### 8.1 程序集扫描性能

- **限制扫描范围**：只扫描必要的程序集，避免扫描过多的程序集
- **使用具体的匹配模式**：使用具体的类型匹配模式，减少匹配的类型数量
- **缓存扫描结果**：缓存扫描结果，避免重复扫描
- **并行扫描**：使用并行扫描，提高扫描速度

### 8.2 服务注册性能

- **批量注册**：使用批量注册，减少注册次数
- **合理设置生命周期**：根据服务的性质设置合理的生命周期
- **避免循环依赖**：确保服务之间没有循环依赖

### 8.3 代码生成性能

- **模板缓存**：缓存代码模板，避免重复加载
- **增量生成**：支持增量生成，只生成变化的部分
- **并行生成**：使用并行生成，提高生成速度

## 9. 安全性

### 9.1 代码安全

- **类型验证**：验证扫描到的类型，确保它们是安全的
- **装饰器验证**：验证装饰器类型，确保它们实现了正确的接口
- **代码生成安全**：确保生成的代码是安全的，避免注入恶意代码

### 9.2 配置安全

- **环境变量**：敏感配置通过环境变量传递
- **配置加密**：支持配置文件加密
- **权限控制**：限制对配置文件的访问权限

## 10. 扩展性

### 10.1 自定义程序集扫描器

通过实现 `IAssemblyScanner` 接口，可以创建自定义的程序集扫描器：

```csharp
public class CustomAssemblyScanner : IAssemblyScanner
{
    public async Task<Assembly[]> LoadAssembliesAsync(string path, CancellationToken cancellationToken = default)
    {
        // 自定义加载逻辑
    }
    
    public async Task<List<ServiceDescriptor>> ScanAssembliesAsync(Assembly[] assemblies, string pattern, string lifetime, CancellationToken cancellationToken = default)
    {
        // 自定义扫描逻辑
    }
}
```

### 10.2 自定义服务装饰器

通过实现 `IServiceDecorator` 接口，可以创建自定义的服务装饰器：

```csharp
public class CustomServiceDecorator : IServiceDecorator
{
    public async Task<ServiceDescriptor> DecorateServiceAsync(Type serviceType, Type decoratorType, string lifetime, CancellationToken cancellationToken = default)
    {
        // 自定义装饰逻辑
    }
}
```

### 10.3 自定义代码生成器

通过实现 `ICodeGenerator` 接口，可以创建自定义的代码生成器：

```csharp
public class CustomCodeGenerator : ICodeGenerator
{
    public async Task<string> GenerateAsync(string outputPath, string @namespace, string className, CancellationToken cancellationToken = default)
    {
        // 自定义生成逻辑
    }
}
```

## 11. 最佳实践

### 11.1 程序集扫描最佳实践

- **限制扫描范围**：只扫描必要的程序集，避免扫描过多的程序集
- **使用具体的匹配模式**：使用具体的类型匹配模式，减少匹配的类型数量
- **合理设置生命周期**：根据服务的性质设置合理的生命周期
- **避免循环依赖**：确保服务之间没有循环依赖

### 11.2 服务装饰最佳实践

- **保持装饰器简单**：每个装饰器只负责一个横切关注点
- **遵循装饰器模式**：确保装饰器实现与原始服务相同的接口
- **合理组织装饰器链**：根据横切关注点的优先级组织装饰器链
- **避免过度装饰**：不要为每个服务添加过多的装饰器，以免影响性能

### 11.3 代码生成最佳实践

- **定期生成代码**：在程序集变更后定期生成依赖注入配置代码
- **自定义命名空间**：使用与项目结构匹配的命名空间
- **代码审查**：生成代码后进行代码审查，确保生成的代码符合项目规范
- **版本控制**：将生成的代码纳入版本控制，以便跟踪变更

### 11.4 AOT 编译最佳实践

- **测试 AOT 编译**：在开发过程中定期测试 AOT 编译，确保代码兼容
- **避免反射**：尽量避免使用反射，或使用编译时反射替代
- **使用 trim 友好的库**：使用支持 trim 的库，避免使用不支持 trim 的库
- **优化资源使用**：优化内存和 CPU 使用，充分利用 AOT 编译的优势

## 12. 常见问题

### 12.1 程序集扫描失败

**可能原因**：
- 程序集路径不存在
- 程序集损坏
- 权限不足
- 程序集依赖项缺失

**解决方案**：
- 检查程序集路径是否正确
- 检查程序集是否损坏
- 确保有足够的权限访问程序集
- 确保程序集的依赖项已安装

### 12.2 服务注册失败

**可能原因**：
- 类型不存在
- 类型不匹配
- 循环依赖
- 权限不足

**解决方案**：
- 检查类型是否存在
- 检查类型是否匹配
- 解决循环依赖问题
- 确保有足够的权限注册服务

### 12.3 装饰器添加失败

**可能原因**：
- 服务类型不存在
- 装饰器类型不存在
- 装饰器不实现服务接口
- 权限不足

**解决方案**：
- 检查服务类型是否存在
- 检查装饰器类型是否存在
- 确保装饰器实现了服务接口
- 确保有足够的权限添加装饰器

### 12.4 代码生成失败

**可能原因**：
- 输出路径不存在
- 权限不足
- 配置错误
- 模板不存在

**解决方案**：
- 检查输出路径是否存在
- 确保有足够的权限写入输出文件
- 检查配置是否正确
- 确保模板存在

## 13. 监控与日志

### 13.1 日志记录

- **详细日志**：记录程序集扫描、服务注册、装饰器添加和代码生成的详细信息
- **错误日志**：记录错误和异常信息
- **性能日志**：记录操作的执行时间和资源使用情况

### 13.2 监控指标

- **程序集扫描统计**：扫描的程序集数量、找到的类型数量
- **服务注册统计**：注册的服务数量、不同生命周期的服务数量
- **装饰器统计**：添加的装饰器数量、装饰器链的平均长度
- **代码生成统计**：生成的代码行数、生成时间

## 14. 部署与集成

### 14.1 部署方式

- **单文件部署**：使用 `publishSingleFile` 生成单个可执行文件
- **自包含部署**：使用 `selfContained` 生成自包含的可执行文件
- **容器部署**：将生成的可执行文件打包到容器中

### 14.2 集成方式

- **命令行集成**：通过命令行工具集成到构建流程中
- **MSBuild 集成**：通过 MSBuild 任务集成到构建流程中
- **CI/CD 集成**：通过 CI/CD 管道集成到构建和部署流程中

## 15. 版本控制

| 版本 | 日期 | 变更内容 |
|------|------|----------|
| 1.0.0 | 2026-01-24 | 初始版本 |

## 16. 总结

Scrutor 技能是一个强大的依赖注入扩展工具，它通过程序集扫描、服务装饰和代码生成等功能，简化了依赖注入的配置和管理。同时，它支持 AOT 编译，提供了更好的性能和更小的部署包。

通过本文档的介绍，您应该已经了解了 Scrutor 技能的核心组件、API 规范、配置选项和最佳实践。现在，您可以开始在您的项目中使用 Scrutor 技能，简化依赖注入的配置和管理，提高应用程序的性能和可维护性。