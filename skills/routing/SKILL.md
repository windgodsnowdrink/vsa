# Routing 技能文档

## 技能概述

Routing 是一个基于 .NET 10 和 AOT 编译的路由管理工具，旨在帮助开发者简化 API 路由的定义、管理和代码生成。该工具提供了直观的命令行界面，支持 RESTful API 路由管理、路由参数解析、中间件集成以及从 API 定义生成路由代码等功能。

### 核心特性

- **路由管理**：支持添加、列出、删除 RESTful API 路由
- **路由参数解析**：支持路径参数、查询参数、表单参数的解析
- **中间件集成**：支持路由级中间件和全局中间件
- **路由代码生成**：支持从 Swagger/OpenAPI 规范生成路由代码
- **AOT 编译**：采用 AOT 编译技术，提高运行性能和减少部署依赖
- **多格式输出**：支持 JSON、YAML 等多种输出格式

### 适用场景

- RESTful API 开发和管理
- 微服务架构中的服务路由配置
- 从 API 规范自动生成路由代码
- 路由管理和监控
- 快速原型开发和 API 设计

## 快速开始

### 环境要求

- .NET 10 SDK 或更高版本
- Windows 10/11 (x64)
- Visual Studio 2022 或更高版本（可选）

### 安装和使用

1. **克隆或下载**：获取 Routing 技能包
2. **构建项目**：使用 .NET CLI 构建项目
   ```bash
   dotnet build
   ```
3. **发布 AOT**：发布为 AOT 编译的单文件可执行文件
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial
   ```
4. **添加路由**：添加新的 RESTful API 路由
   ```bash
   routing_core.exe add --path "/api/users" --method "GET" --handler "UserController.GetUsers"
   ```
5. **列出路由**：查看所有已定义的路由
   ```bash
   routing_core.exe list --format "json"
   ```
6. **生成路由代码**：从 Swagger 规范生成路由代码
   ```bash
   routing_generator.exe generate --input "swagger.json" --output "Routes.cs" --language "csharp"
   ```

## 核心功能

### 路由管理

Routing 提供了全面的路由管理功能，包括：

- **添加路由**：定义新的 RESTful API 路由，指定路径、HTTP 方法和处理程序
- **列出路由**：查看所有已定义的路由，支持多种输出格式
- **删除路由**：删除指定的路由
- **路由参数**：支持路径参数、查询参数、表单参数的定义和解析
- **路由约束**：支持对路由参数的类型、格式等进行约束

#### 路由定义示例

```bash
# 添加 GET 路由
routing_core.exe add --path "/api/users" --method "GET" --handler "UserController.GetUsers"

# 添加带路径参数的 GET 路由
routing_core.exe add --path "/api/users/{id}" --method "GET" --handler "UserController.GetUserById"

# 添加 POST 路由
routing_core.exe add --path "/api/users" --method "POST" --handler "UserController.CreateUser"

# 添加 PUT 路由
routing_core.exe add --path "/api/users/{id}" --method "PUT" --handler "UserController.UpdateUser"

# 添加 DELETE 路由
routing_core.exe add --path "/api/users/{id}" --method "DELETE" --handler "UserController.DeleteUser"
```

### 路由代码生成

Routing 支持从 API 定义生成路由代码，包括：

- **Swagger/OpenAPI 支持**：从 Swagger/OpenAPI 规范生成路由代码
- **多语言支持**：支持生成 C#、TypeScript 等多种语言的路由代码
- **自定义模板**：支持使用自定义模板生成路由代码
- **集成框架**：支持生成与 ASP.NET Core、FastEndpoints 等框架集成的路由代码

#### 代码生成示例

```bash
# 从 Swagger 规范生成 C# 路由代码
routing_generator.exe generate --input "swagger.json" --output "Routes.cs" --language "csharp"

# 从 OpenAPI 规范生成 TypeScript 路由代码
routing_generator.exe generate --input "openapi.yaml" --output "routes.ts" --language "typescript"
```

### 中间件集成

Routing 支持中间件集成，包括：

- **全局中间件**：应用于所有路由的中间件
- **路由级中间件**：仅应用于特定路由的中间件
- **中间件管道**：支持定义中间件执行顺序
- **内置中间件**：提供日志记录、身份验证、授权等内置中间件

#### 中间件配置示例

```bash
# 添加全局日志中间件
routing_core.exe add-middleware --name "Logger" --handler "Middleware.Logger"

# 添加路由级身份验证中间件
routing_core.exe add-route-middleware --path "/api/users" --method "GET" --name "Authentication" --handler "Middleware.Authentication"
```

## 命令行接口

### 路由管理命令

#### add 命令

```bash
routing_core.exe add --path <path> --method <method> --handler <handler>
```

**参数**：
- `--path`：路由路径，如 "/api/users"
- `--method`：HTTP 方法，如 "GET", "POST", "PUT", "DELETE"（默认：GET）
- `--handler`：路由处理程序，如 "UserController.GetUsers"

#### list 命令

```bash
routing_core.exe list --format <format>
```

**参数**：
- `--format`：输出格式，如 "json", "yaml", "text"（默认：json）

#### remove 命令

```bash
routing_core.exe remove --path <path> --method <method>
```

**参数**：
- `--path`：路由路径，如 "/api/users"
- `--method`：HTTP 方法，如 "GET", "POST", "PUT", "DELETE"（默认：GET）

### 路由代码生成命令

#### generate 命令

```bash
routing_generator.exe generate --input <input> --output <output> --language <language>
```

**参数**：
- `--input`：输入文件路径（Swagger/OpenAPI 规范），如 "swagger.json"
- `--output`：输出文件路径，如 "Routes.cs"
- `--language`：生成语言，如 "csharp", "typescript"（默认：csharp）

## API 参考

### 核心接口

#### IRouteManager

```csharp
public interface IRouteManager
{
    Task<Route> AddRouteAsync(string path, string method, string handler, CancellationToken cancellationToken = default);
    Task<IEnumerable<Route>> GetRoutesAsync(CancellationToken cancellationToken = default);
    Task<bool> RemoveRouteAsync(string path, string method, CancellationToken cancellationToken = default);
}
```

- **方法**：
  - `AddRouteAsync`：添加新路由
  - `GetRoutesAsync`：获取所有路由
  - `RemoveRouteAsync`：删除路由

#### IRouteGenerator

```csharp
public interface IRouteGenerator
{
    Task<string> GenerateAsync(string inputPath, string language, CancellationToken cancellationToken = default);
}
```

- **方法**：
  - `GenerateAsync`：从输入文件生成路由代码

#### IRouteParser

```csharp
public interface IRouteParser
{
    Route Parse(string path, string method, string handler);
    Dictionary<string, string> ParseParameters(string path, string requestPath);
}
```

- **方法**：
  - `Parse`：解析路由定义
  - `ParseParameters`：解析路由参数

### 数据结构

#### Route

```csharp
public class Route
{
    public string Path { get; set; }
    public string Method { get; set; }
    public string Handler { get; set; }
    public List<RouteParameter> Parameters { get; set; } = new List<RouteParameter>();
    public List<MiddlewareInfo> Middlewares { get; set; } = new List<MiddlewareInfo>();
}
```

- **属性**：
  - `Path`：路由路径
  - `Method`：HTTP 方法
  - `Handler`：路由处理程序
  - `Parameters`：路由参数列表
  - `Middlewares`：路由中间件列表

#### RouteParameter

```csharp
public class RouteParameter
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Source { get; set; } // path, query, form
    public bool Required { get; set; }
    public string DefaultValue { get; set; }
}
```

- **属性**：
  - `Name`：参数名称
  - `Type`：参数类型
  - `Source`：参数来源（path, query, form）
  - `Required`：是否必填
  - `DefaultValue`：默认值

#### MiddlewareInfo

```csharp
public class MiddlewareInfo
{
    public string Name { get; set; }
    public string Handler { get; set; }
    public int Order { get; set; }
}
```

- **属性**：
  - `Name`：中间件名称
  - `Handler`：中间件处理程序
  - `Order`：执行顺序

## AOT 编译指南

### AOT 编译优势

- **性能提升**：减少运行时 JIT 编译开销，提高启动速度和执行性能
- **部署简化**：自包含部署，无需目标机器安装 .NET 运行时
- **安全性增强**：减少可攻击面，提高应用安全性
- **体积优化**：通过裁剪未使用代码，减少应用体积

### AOT 配置

在项目文件中配置 AOT 编译选项：

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <PublishAot>true</PublishAot>
  <TrimMode>partial</TrimMode>
  <SelfContained>true</SelfContained>
  <PublishSingleFile>true</PublishSingleFile>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>
```

### AOT 注意事项

1. **反射使用**：AOT 编译会裁剪未使用的代码，使用反射时需要特别注意
2. **动态类型**：减少使用 dynamic 类型，可能会影响 AOT 编译效果
3. **序列化**：确保序列化/反序列化操作兼容 AOT 编译
4. **测试**：在 AOT 编译后进行充分测试，确保功能正常

## 示例用法

### 示例 1：基本路由管理

```bash
# 添加用户相关路由
routing_core.exe add --path "/api/users" --method "GET" --handler "UserController.GetUsers"
routing_core.exe add --path "/api/users/{id}" --method "GET" --handler "UserController.GetUserById"
routing_core.exe add --path "/api/users" --method "POST" --handler "UserController.CreateUser"
routing_core.exe add --path "/api/users/{id}" --method "PUT" --handler "UserController.UpdateUser"
routing_core.exe add --path "/api/users/{id}" --method "DELETE" --handler "UserController.DeleteUser"

# 列出所有路由
routing_core.exe list --format "json"

# 删除路由
routing_core.exe remove --path "/api/users/{id}" --method "DELETE"
```

### 示例 2：路由代码生成

```bash
# 从 Swagger 规范生成 C# 路由代码
routing_generator.exe generate --input "swagger.json" --output "Routes.cs" --language "csharp"

# 从 OpenAPI 规范生成 TypeScript 路由代码
routing_generator.exe generate --input "openapi.yaml" --output "routes.ts" --language "typescript"
```

### 示例 3：中间件集成

```bash
# 添加全局日志中间件
routing_core.exe add-middleware --name "Logger" --handler "Middleware.Logger"

# 添加全局身份验证中间件
routing_core.exe add-middleware --name "Authentication" --handler "Middleware.Authentication"

# 添加路由级授权中间件
routing_core.exe add-route-middleware --path "/api/admin" --method "GET" --name "Authorization" --handler "Middleware.Authorization"
```

## 配置指南

### 配置文件

Routing 支持通过配置文件进行配置，配置文件路径为 `appsettings.json`：

```json
{
  "Routing": {
    "DefaultHandlerNamespace": "Controllers",
    "RoutePrefix": "/api",
    "EnableMiddleware": true,
    "OutputFormat": "json"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

### 环境变量

Routing 支持通过环境变量进行配置：

| 环境变量 | 描述 | 默认值 |
|---------|------|--------|
| `ROUTING_DEFAULT_HANDLER_NAMESPACE` | 默认处理程序命名空间 | Controllers |
| `ROUTING_ROUTE_PREFIX` | 路由前缀 | /api |
| `ROUTING_ENABLE_MIDDLEWARE` | 是否启用中间件 | true |
| `ROUTING_OUTPUT_FORMAT` | 输出格式 | json |
| `DOTNET_ENVIRONMENT` | .NET 环境 | Production |

## 常见问题

### 1. 路由冲突

**问题**：添加路由时出现路由冲突错误

**解决方案**：
- 检查是否已存在相同路径和方法的路由
- 使用不同的路径或方法
- 先删除冲突的路由再添加新路由

### 2. 代码生成失败

**问题**：从 Swagger 规范生成路由代码失败

**解决方案**：
- 检查 Swagger 规范文件是否有效
- 确保输入文件路径正确
- 检查生成语言是否支持

### 3. 路由参数解析错误

**问题**：路由参数解析失败

**解决方案**：
- 检查路由定义中的参数格式
- 确保请求路径与路由定义匹配
- 验证参数类型是否正确

### 4. AOT 编译失败

**问题**：发布 AOT 时出现编译错误

**解决方案**：
- 检查项目是否使用了不兼容 AOT 的功能
- 确保所有依赖项支持 AOT 编译
- 调整 TrimMode 为 partial 或 copyused

## 高级功能

### 路由约束

Routing 支持对路由参数进行约束，确保参数符合特定格式：

```bash
# 添加带约束的路由
routing_core.exe add --path "/api/users/{id:int}" --method "GET" --handler "UserController.GetUserById"
routing_core.exe add --path "/api/users/{name:alpha}" --method "GET" --handler "UserController.GetUserByName"
routing_core.exe add --path "/api/users/{id:guid}" --method "GET" --handler "UserController.GetUserById"
```

### 自定义中间件

Routing 支持自定义中间件，实现特定的业务逻辑：

```csharp
public class CustomMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 中间件逻辑
        Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
        
        // 调用下一个中间件
        await next(context);
        
        // 响应处理
        Console.WriteLine($"Response: {context.Response.StatusCode}");
    }
}
```

### 路由组

Routing 支持路由组，简化具有相同前缀的路由定义：

```bash
# 添加路由组
routing_core.exe add-group --prefix "/api/users" --handler-namespace "Controllers"

# 在组内添加路由
routing_core.exe add --path "/" --method "GET" --handler "GetUsers" --group "users"
routing_core.exe add --path "/{id}" --method "GET" --handler "GetUserById" --group "users"
routing_core.exe add --path "/" --method "POST" --handler "CreateUser" --group "users"
```

## 集成示例

### 与 ASP.NET Core 集成

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Routing.Core;

var builder = WebApplication.CreateBuilder(args);

// 添加路由服务
builder.Services.AddRoutingCore();

var app = builder.Build();

// 使用路由中间件
app.UseRoutingCore();

app.Run();
```

### 与 FastEndpoints 集成

```csharp
using FastEndpoints;
using Routing.Core;

var builder = WebApplication.CreateBuilder(args);

// 添加 FastEndpoints
builder.Services.AddFastEndpoints();

// 添加路由服务
builder.Services.AddRoutingCore();

var app = builder.Build();

// 使用路由中间件
app.UseRoutingCore();
app.UseFastEndpoints();

app.Run();
```

## 贡献指南

我们欢迎社区贡献，包括但不限于：

- **功能请求**：提出新功能或改进建议
- **错误报告**：报告使用过程中遇到的错误
- **代码贡献**：提交代码修复或功能实现
- **文档改进**：改进文档质量和完整性

### 开发环境设置

1. **克隆仓库**：`git clone https://github.com/your-repo/routing.git`
2. **安装依赖**：`dotnet restore`
3. **构建项目**：`dotnet build`
4. **运行测试**：`dotnet test`

### 提交代码

1. **创建分支**：`git checkout -b feature/your-feature`
2. **提交更改**：`git commit -m "Add your feature"`
3. **推送分支**：`git push origin feature/your-feature`
4. **创建 PR**：在 GitHub 上创建 Pull Request

## 许可证

Routing 技能使用 MIT 许可证，详情请参阅 LICENSE 文件。

## 联系方式

- **作者**：大佬
- **邮箱**：your-email@example.com
- **GitHub**：https://github.com/your-repo/routing

---

**版本历史**

- v1.0.0 (2026-01-24)：初始版本，支持路由管理、代码生成和 AOT 编译
