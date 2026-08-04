# OpenAPI 技能详细参考文档

## 技能概述

OpenAPI 技能是一个基于 .NET 10 和 AOT 编译的高性能 OpenAPI 解决方案，为 .NET 开发者提供强大的 API 文档生成、OpenAPI 规范管理和 API 客户端生成功能。

### 主要特性

- **OpenAPI 规范生成**：自动生成符合 OpenAPI 规范的 API 文档
- **API 文档生成**：集成 Swagger UI 和 Scalar API 参考
- **API 客户端生成**：基于 OpenAPI 规范生成 API 客户端代码
- **OpenAPI 模式验证**：验证 OpenAPI 规范的有效性和完整性
- **Swagger UI 集成**：提供交互式 API 文档界面
- **Scalar API 参考集成**：提供现代化的 API 参考文档
- **高性能设计**：基于 AOT 编译的高性能实现
- **内存优化**：优化内存使用，减少资源消耗
- **并发支持**：支持高并发场景的 API 文档生成

## 核心组件

### IOpenAPIService 接口

OpenAPI 服务的核心接口，提供生成和验证 OpenAPI 规范的功能。

**主要方法**：

- `GenerateOpenApiSpecAsync()`：生成 OpenAPI 规范文档
- `ValidateOpenApiSpecAsync(OpenApiDocument)`：验证 OpenAPI 规范的有效性
- `GenerateOpenApiJsonAsync()`：生成 OpenAPI JSON 格式规范
- `GenerateOpenApiYamlAsync()`：生成 OpenAPI YAML 格式规范
- `SaveOpenApiSpecAsync(string)`：保存 OpenAPI 规范到文件
- `GetApiOperationsAsync()`：获取 API 操作列表

### IOpenApiDocumentGenerator 接口

OpenAPI 文档生成器接口，负责生成 OpenAPI 文档。

**主要方法**：

- `GenerateDocumentAsync(OpenAPIOptions)`：根据配置选项生成 OpenAPI 文档

### IOpenApiValidator 接口

OpenAPI 文档验证器接口，负责验证 OpenAPI 文档的有效性。

**主要方法**：

- `ValidateAsync(OpenApiDocument)`：验证 OpenAPI 文档的有效性

### OpenAPIOptions 配置选项

OpenAPI 服务的配置选项，包含以下设置：

| 配置项 | 类型 | 默认值 | 描述 |
|-------|------|-------|------|
| Enabled | bool | true | 是否启用 OpenAPI 服务 |
| EnableSwaggerUI | bool | true | 是否启用 Swagger UI |
| EnableScalarUI | bool | true | 是否启用 Scalar UI |
| ApiTitle | string | "API" | API 标题 |
| ApiVersion | string | "v1" | API 版本 |
| ApiDescription | string | "API Description" | API 描述 |
| ContactName | string | "Contact" | 联系人姓名 |
| ContactEmail | string | "contact@example.com" | 联系人邮箱 |
| LicenseName | string | "License" | 许可证名称 |
| LicenseUrl | string | "https://example.com/license" | 许可证 URL |
| EnableXmlComments | bool | false | 是否启用 XML 注释 |
| XmlCommentsPath | string | "" | XML 注释文件路径 |
| EnableAuthentication | bool | false | 是否启用认证 |
| AuthenticationScheme | string | "Bearer" | 认证方案 |
| EnableParallelProcessing | bool | true | 是否启用并行处理 |
| MaxDegreeOfParallelism | int | Environment.ProcessorCount | 最大并行度 |

## 快速开始

### 安装依赖

在您的主应用程序的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Swashbuckle.AspNetCore@6.5.0
#:package Microsoft.OpenApi@1.6.0
#:package System.Text.Json@10.0.0
#:package System.Net.Http.Json@10.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0
```

### 注册服务

在您的主应用程序中注册 OpenAPI 服务：

```csharp
// 注册 OpenAPI 服务
builder.Services.AddOpenAPIServices(options =>
{
    options.Enabled = true;
    options.EnableSwaggerUI = true;
    options.EnableScalarUI = true;
    options.ApiTitle = "示例 API";
    options.ApiVersion = "v1";
    options.ApiDescription = "这是一个示例 API 文档";
    options.ContactName = "开发者";
    options.ContactEmail = "developer@example.com";
    options.LicenseName = "MIT";
    options.LicenseUrl = "https://opensource.org/licenses/MIT";
});
```

### 使用服务

```csharp
// 获取 OpenAPI 服务
var openApiService = serviceProvider.GetRequiredService<IOpenAPIService>();

// 生成 OpenAPI 规范
var openApiSpec = await openApiService.GenerateOpenApiSpecAsync();
Console.WriteLine($"OpenAPI 规范生成成功，版本: {openApiSpec.Info.Version}");

// 验证 OpenAPI 规范
var validationResult = await openApiService.ValidateOpenApiSpecAsync(openApiSpec);
Console.WriteLine($"OpenAPI 规范验证: {(validationResult.IsValid ? "有效" : "无效")}");

// 生成 JSON 格式规范
var jsonSpec = await openApiService.GenerateOpenApiJsonAsync();
Console.WriteLine("OpenAPI JSON 规范生成成功");

// 保存规范到文件
var saveResult = await openApiService.SaveOpenApiSpecAsync("openapi.json");
Console.WriteLine($"OpenAPI 规范保存: {(saveResult ? "成功" : "失败")}");
```

## 配置选项

### 基本配置

在 `openapi_extensions.setting.json` 文件中，您可以配置以下选项：

```json
{
  "openapi": {
    "options": {
      "enabled": true,
      "enableSwaggerUI": true,
      "enableScalarUI": true,
      "apiTitle": "示例 API",
      "apiVersion": "v1",
      "apiDescription": "这是一个示例 API 文档",
      "contactName": "开发者",
      "contactEmail": "developer@example.com",
      "licenseName": "MIT",
      "licenseUrl": "https://opensource.org/licenses/MIT",
      "enableXmlComments": false,
      "xmlCommentsPath": "",
      "enableAuthentication": false,
      "authenticationScheme": "Bearer",
      "enableParallelProcessing": true,
      "maxDegreeOfParallelism": 4
    }
  }
}
```

### Swagger UI 配置

```json
{
  "openapi": {
    "swagger": {
      "enabled": true,
      "routePrefix": "swagger",
      "docExpansion": "none",
      "defaultModelsExpandDepth": 1,
      "defaultModelExpandDepth": 1,
      "displayOperationId": false,
      "filter": null,
      "operationsSorter": null,
      "tagsSorter": null,
      "validatorUrl": null
    }
  }
}
```

### Scalar UI 配置

```json
{
  "openapi": {
    "scalar": {
      "enabled": true,
      "routePrefix": "api-reference",
      "theme": "default",
      "defaultSchemaExpandLevel": 2,
      "showExtensions": false,
      "sortTagsAlphabetically": true
    }
  }
}
```

### AOT 编译配置

```json
{
  "openapi": {
    "aot": {
      "publishAot": true,
      "readyToRun": true,
      "tieredCompilation": true,
      "optimizeForSize": false,
      "trimMode": "partial"
    }
  }
}
```

## 运行配置

在 `openapi_extensions.run.json` 文件中，您可以配置不同环境下的运行设置：

### 开发环境

```json
{
  "profiles": {
    "Development": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DOTNET_PUBLISH_AOT": "0",
        "OPENAPI_ENABLED": "true",
        "OPENAPI_ENABLE_SWAGGER": "true",
        "OPENAPI_ENABLE_SCALAR": "true"
      },
      "applicationUrl": "http://localhost:5000;https://localhost:5001",
      "commandLineArgs": "--verbose"
    }
  }
}
```

### 生产环境

```json
{
  "profiles": {
    "Production": {
      "commandName": "Project",
      "launchBrowser": false,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Production",
        "DOTNET_PUBLISH_AOT": "1",
        "OPENAPI_ENABLED": "true",
        "OPENAPI_ENABLE_SWAGGER": "true",
        "OPENAPI_ENABLE_SCALAR": "true"
      },
      "applicationUrl": "http://0.0.0.0:80;https://0.0.0.0:443",
      "commandLineArgs": "--optimize"
    }
  }
}
```

### 性能测试环境

```json
{
  "profiles": {
    "PerformanceTest": {
      "commandName": "Project",
      "launchBrowser": false,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Performance",
        "DOTNET_PUBLISH_AOT": "1",
        "OPENAPI_ENABLED": "true",
        "OPENAPI_ENABLE_SWAGGER": "false",
        "OPENAPI_ENABLE_SCALAR": "false",
        "OPENAPI_ENABLE_PARALLEL_PROCESSING": "true",
        "OPENAPI_MAX_DEGREE_OF_PARALLELISM": "8"
      },
      "applicationUrl": "http://localhost:5002",
      "commandLineArgs": "--performance-test --iterations=1000"
    }
  }
}
```

## AOT 编译

### 配置 AOT 编译

在您的项目文件中添加以下配置：

```yaml
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true
```

### 支持的运行时

- win-x64
- linux-x64
- osx-x64
- win-arm64
- linux-arm64
- osx-arm64

### AOT 编译命令

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true
```

## 性能优化

### 内存优化

1. **使用对象池**：对于频繁创建的对象，使用对象池减少内存分配
2. **避免字符串拼接**：使用 `StringBuilder` 进行字符串拼接
3. **使用 `using` 语句**：确保及时释放资源
4. **优化序列化**：使用 `System.Text.Json` 进行高效序列化

### 并发优化

1. **启用并行处理**：设置 `EnableParallelProcessing` 为 `true`
2. **调整并行度**：根据系统资源调整 `MaxDegreeOfParallelism`
3. **使用数据流**：使用 `System.Threading.Tasks.Dataflow` 进行高效的并行处理

### 启动优化

1. **使用 AOT 编译**：减少启动时间和内存使用
2. **预热服务**：在应用启动时预热 OpenAPI 服务
3. **缓存结果**：缓存 OpenAPI 规范，避免重复生成

## 故障排除

### 常见问题

1. **OpenAPI 规范生成失败**
   - 检查配置选项是否正确
   - 确保所有依赖项已正确安装
   - 检查是否有足够的内存和 CPU 资源

2. **Swagger UI 无法访问**
   - 检查 `EnableSwaggerUI` 是否设置为 `true`
   - 检查路由前缀是否正确
   - 检查应用程序是否正在运行

3. **AOT 编译失败**
   - 确保使用 .NET 10 或更高版本
   - 检查项目文件中的 AOT 配置是否正确
   - 确保所有依赖项支持 AOT 编译

### 日志记录

OpenAPI 服务使用 `Microsoft.Extensions.Logging` 进行日志记录，您可以通过配置日志级别来获取更详细的信息：

```csharp
services.AddLogging(builder => 
    builder.AddConsole()
           .SetMinimumLevel(LogLevel.Debug)
);
```

## 部署指南

### 容器化部署

您可以使用 Docker 容器化部署 OpenAPI 服务：

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["YourProject.csproj", "."]
RUN dotnet restore "./YourProject.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "YourProject.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YourProject.csproj" -c Release -o /app/publish /p:PublishAot=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./YourProject"]
```

### 云部署

OpenAPI 服务可以部署到各种云平台，如 Azure、AWS、GCP 等。以下是部署到 Azure App Service 的示例：

1. **创建 Azure App Service**：选择 .NET 8 或更高版本的运行时
2. **配置部署**：使用 GitHub Actions 或 Azure DevOps 进行持续部署
3. **设置环境变量**：配置必要的环境变量
4. **部署应用**：将编译后的应用部署到 Azure App Service

## 扩展开发

### 自定义文档生成器

您可以通过实现 `IOpenApiDocumentGenerator` 接口来自定义文档生成器：

```csharp
public class CustomDocumentGenerator : IOpenApiDocumentGenerator
{
    private readonly ILogger<CustomDocumentGenerator> _logger;

    public CustomDocumentGenerator(ILogger<CustomDocumentGenerator> logger)
    {
        _logger = logger;
    }

    public async Task<OpenApiDocument> GenerateDocumentAsync(OpenAPIOptions options, CancellationToken cancellationToken = default)
    {
        // 自定义文档生成逻辑
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = options.ApiTitle,
                Version = options.ApiVersion,
                Description = options.ApiDescription
            },
            Paths = new OpenApiPaths(),
            Components = new OpenApiComponents()
        };

        // 添加自定义路径和操作
        // ...

        return await Task.FromResult(document);
    }
}
```

### 自定义验证器

您可以通过实现 `IOpenApiValidator` 接口来自定义验证器：

```csharp
public class CustomValidator : IOpenApiValidator
{
    private readonly ILogger<CustomValidator> _logger;

    public CustomValidator(ILogger<CustomValidator> logger)
    {
        _logger = logger;
    }

    public async Task<OpenApiValidationResult> ValidateAsync(OpenApiDocument openApiDoc, CancellationToken cancellationToken = default)
    {
        var result = new OpenApiValidationResult { IsValid = true };

        // 自定义验证逻辑
        // ...

        return await Task.FromResult(result);
    }
}
```

### 注册自定义实现

```csharp
// 注册自定义实现
builder.Services.AddSingleton<IOpenApiDocumentGenerator, CustomDocumentGenerator>();
builder.Services.AddSingleton<IOpenApiValidator, CustomValidator>();
builder.Services.AddSingleton<IOpenAPIService, OpenAPIService>();
```

## 总结

OpenAPI 技能是一个功能强大、性能优异的 OpenAPI 解决方案，为 .NET 开发者提供了全面的 API 文档生成和管理功能。通过 AOT 编译和性能优化，它能够在各种场景下高效运行，满足不同规模应用的需求。

无论是构建小型 API 还是大型微服务架构，OpenAPI 技能都能为您提供清晰、准确的 API 文档，帮助您更好地管理和维护 API。
