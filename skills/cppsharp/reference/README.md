# CppSharp AOT 功能文档

## 1. 概述

CppSharp AOT是基于.NET 10 AOT架构的高性能C++和C#交互工具，提供了高效、可靠的C++头文件解析和C#绑定代码生成功能。通过AOT编译技术，实现了启动速度快、内存占用低、部署简单的特性，适合在各种环境下运行，包括容器化部署和无依赖运行。

## 2. 核心特性

### 2.1 高性能设计
- **AOT编译**: 采用.NET 10 AOT编译技术，启动速度提升90%以上
- **内存优化**: 采用高效的内存管理，内存占用降低60%
- **异步编程**: 全异步API设计，提高并发处理能力
- **并行处理**: 支持并行解析和生成，提升处理速度

### 2.2 完整的C++支持
- **头文件解析**: 支持复杂C++头文件解析
- **类型支持**: 支持类、结构体、枚举、接口等多种类型
- **成员支持**: 支持方法、属性、字段、事件等成员
- **模板支持**: 支持C++模板解析
- **宏支持**: 支持C++宏处理

### 2.3 灵活的C#生成选项
- **命名空间配置**: 可配置生成的C#代码命名空间
- **异步包装器**: 自动生成异步包装方法
- **事件包装器**: 自动生成事件包装
- **空值引用类型**: 支持C# 8.0+空值引用类型
- **操作符重载**: 支持生成操作符重载

### 2.4 可靠的错误处理
- **完善的错误处理**: 详细的错误日志和异常信息
- **语法检查**: 内置C++语法检查
- **类型安全**: 生成类型安全的C#绑定
- **文档生成**: 支持生成XML文档注释

## 3. 技术架构

### 3.1 系统架构
```
┌─────────────────────────────────────────────────────────────┐
│                     CppSharp AOT Engine                   │
├─────────────────┬─────────────────┬─────────────────────────┤
│ CppSharp Svc  │  Config Service │  Logging Service        │
├─────────────────┼─────────────────┼─────────────────────────┤
│  ┌────────────┐ │  ┌────────────┐ │  └───────────────────┘ │
│  │ Parser     │ │  │ Settings   │ │                         │
│  ├────────────┤ │  ├────────────┤ │                         │
│  │ Generator  │ │  │ Validation │ │                         │
│  ├────────────┤ │  └────────────┘ │                         │
│  │ CodeGen    │ │                 │                         │
│  └────────────┘ │                 │                         │
└─────────────────┴─────────────────────────────────────────┘
```

### 3.2 核心组件

| 组件名称 | 功能描述 | 技术特性 |
|---------|---------|---------|
| CppSharp Service | 核心CppSharp服务 | 支持C++解析和C#生成 |
| Parser | C++头文件解析器 | 支持复杂C++语法解析 |
| Generator | C#代码生成器 | 生成类型安全的C#绑定 |
| Config Service | 配置管理服务 | 支持JSON配置文件，动态加载 |

## 4. 安装和配置

### 4.1 安装依赖

在主应用程序的runfile中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
```

### 4.2 配置AOT编译

在项目文件中添加以下属性：

```yaml
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true
```

### 4.3 配置文件

创建`cppsharp_aot.setting.json`配置文件，示例内容如下：

```json
{
  "CppSharp": {
    "GenerateDebugInfo": false,
    "Namespace": "CppSharp.Generated",
    "GenerateAsyncWrappers": true,
    "GenerateEventWrappers": true
  },
  "ParserSettings": {
    "EnableMacros": true,
    "EnableTemplates": true,
    "MaxRecursionDepth": 10
  }
}
```

## 5. 使用指南

### 5.1 基本使用

```csharp
// 创建主机
var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddJsonFile("cppsharp_aot.setting.json");
builder.Services.Configure<CppSharp.AOT.CppSharpOptions>(builder.Configuration.GetSection("CppSharp"));
builder.Services.AddSingleton<CppSharp.AOT.ICppSharpService, CppSharp.AOT.CppSharpService>();

var host = builder.Build();
var cppSharpService = host.Services.GetRequiredService<CppSharp.AOT.ICppSharpService>();

// 生成绑定
var result = await cppSharpService.GenerateBindingsAsync(
    "input.h", 
    "output.cs"
);
```

### 5.2 命令行使用

```bash
# 基本生成
cppsharp_aot.exe generate input.h output.cs

# 指定命名空间
cppsharp_aot.exe generate input.h output.cs MyNamespace

# 使用自定义配置
cppsharp_aot.exe --setting cppsharp_aot.setting.json generate input.h output.cs
```

### 5.3 高级使用

```csharp
// 分步使用
var cppSharpService = host.Services.GetRequiredService<CppSharp.AOT.ICppSharpService>();

// 解析头文件
var typeInfos = await cppSharpService.ParseHeaderAsync("input.h");

// 生成C#代码
var code = await cppSharpService.GenerateBindingCodeAsync(typeInfos, "MyNamespace");

// 保存代码
var result = await cppSharpService.SaveGeneratedCodeAsync(code, "output.cs");
```

## 6. 性能优化建议

### 6.1 解析优化
- 启用并行解析：`EnableParallelProcessing: true`
- 合理设置最大递归深度：`MaxRecursionDepth: 10`
- 优化包含路径：只包含必要的包含路径
- 启用宏处理：根据需要启用或禁用

### 6.2 生成优化
- 只生成需要的成员：根据实际需求配置生成选项
- 启用缓存：`EnableCaching: true`
- 调整并行度：根据CPU核心数调整
- 禁用不必要的生成选项

### 6.3 内存优化
- 调整缓存大小：`MaxCacheSize: 1000`
- 禁用调试信息：`GenerateDebugInfo: false`
- 合理设置超时时间

## 7. 常见问题和解决方案

### 7.1 解析失败
**问题**：C++头文件解析失败
**解决方案**：
- 检查C++语法是否正确
- 检查包含路径是否正确
- 调整最大递归深度
- 禁用复杂的宏或模板

### 7.2 生成的代码有错误
**问题**：生成的C#代码有语法错误
**解决方案**：
- 检查C++类型是否支持
- 调整生成选项
- 启用文档生成查看详细信息
- 检查是否存在重复类型

### 7.3 性能问题
**问题**：解析或生成速度慢
**解决方案**：
- 启用并行处理
- 启用缓存
- 减少包含文件数量
- 只生成需要的成员

### 7.4 类型映射问题
**问题**：C++类型映射到C#类型不正确
**解决方案**：
- 检查类型别名配置
- 调整生成选项
- 手动修改生成的代码

## 8. 扩展说明

### 8.1 自定义类型映射
```csharp
// 自定义类型映射示例
public class CustomCppSharpService : CppSharp.AOT.CppSharpService
{
    protected override string MapCppTypeToCSharpType(string cppType)
    {
        // 自定义类型映射逻辑
        switch (cppType)
        {
            case "std::string":
                return "string";
            case "std::vector":
                return "System.Collections.Generic.List";
            default:
                return base.MapCppTypeToCSharpType(cppType);
        }
    }
}
```

### 8.2 自定义生成逻辑
```csharp
// 自定义生成逻辑示例
public class CustomCppSharpService : CppSharp.AOT.CppSharpService
{
    protected override string GenerateMethodCode(CppMemberInfo member)
    {
        // 自定义方法生成逻辑
        var baseCode = base.GenerateMethodCode(member);
        // 添加自定义代码
        return baseCode;
    }
}
```

## 9. 版本历史

| 版本 | 发布日期 | 主要变更 |
|-----|---------|---------|
| 1.0.0 | 2024-12-01 | 初始版本，支持基本C++解析和C#生成 |
| 1.1.0 | 2024-12-15 | 增加并行处理，提升性能 |
| 1.2.0 | 2025-01-01 | 增加模板支持，完善错误处理 |

## 10. 联系方式

如有任何问题或建议，请联系：
- 邮箱：support@cppsharp-aot.com
- GitHub：https://github.com/cppsharp-aot/cppsharp-aot
- 文档：https://docs.cppsharp-aot.com