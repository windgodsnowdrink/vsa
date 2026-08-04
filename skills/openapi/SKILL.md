# OpenAPI Agent Skill - OpenAPI 技能

## 技能概述

基于 .NET 10 和 AOT 编译的高性能 OpenAPI 技能，为 .NET 开发者提供强大的 OpenAPI 功能，包括 API 文档生成、OpenAPI 规范管理、API 客户端生成等核心功能。

## 快速开始指南

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
    options.ApiTitle = "My API";
    options.ApiVersion = "v1";
});
```

### 配置 AOT 编译

在您的项目文件中添加 AOT 编译配置：

```yaml
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true
```

### 使用示例

```csharp
// 获取 OpenAPI 服务
var openApiService = serviceProvider.GetRequiredService<IOpenAPIService>();

// 生成 OpenAPI 规范
var openApiSpec = await openApiService.GenerateOpenApiSpecAsync();
Console.WriteLine($"OpenAPI 规范生成成功，版本: {openApiSpec.Info.Version}");

// 验证 OpenAPI 规范
var validationResult = await openApiService.ValidateOpenApiSpecAsync(openApiSpec);
Console.WriteLine($"OpenAPI 规范验证: {(validationResult.IsValid ? "有效" : "无效")}");
```

## 目录结构

```
openapi/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── openapi_extensions.cs     # OpenAPI 核心实现
    ├── openapi_extensions.run.json  # 运行配置
    └── openapi_extensions.setting.json  # 设置文件
```

## 主要功能

1. **OpenAPI 规范生成**：自动生成符合 OpenAPI 规范的 API 文档
2. **API 文档生成**：集成 Swagger UI 和 Scalar API 参考
3. **API 客户端生成**：基于 OpenAPI 规范生成 API 客户端代码
4. **OpenAPI 模式验证**：验证 OpenAPI 规范的有效性和完整性
5. **Swagger UI 集成**：提供交互式 API 文档界面
6. **Scalar API 参考集成**：提供现代化的 API 参考文档
7. **高性能设计**：基于 AOT 编译的高性能实现
8. **内存优化**：优化内存使用，减少资源消耗
9. **并发支持**：支持高并发场景的 API 文档生成

## AOT 编译支持

本技能完全支持 AOT 编译，提供以下优势：

1. **启动速度快**：AOT 编译减少了运行时的 JIT 编译开销
2. **内存占用低**：减少了运行时的内存使用
3. **部署简单**：生成单一可执行文件，便于部署
4. **安全性高**：减少了运行时的攻击面

## 扩展说明

本技能提供了完整的 OpenAPI 解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现 IOpenAPIService 接口的自定义实现
2. **扩展功能**：添加新的 OpenAPI 功能和集成
3. **与其他系统集成**：与其他 API 管理系统集成
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：适当处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **AOT 优化**：根据 AOT 编译的要求优化代码

## 性能特性

1. **高性能设计**：优化的代码结构和算法
2. **内存优化**：减少内存分配和垃圾回收
3. **并发支持**：支持并行处理 API 文档生成
4. **AOT 编译**：利用 AOT 编译提升性能

## 技术特性

1. **模块化设计**：清晰的模块划分和职责分离
2. **依赖注入**：使用 Microsoft.Extensions.DependencyInjection 进行服务管理
3. **异步编程**：全面支持异步操作
4. **高性能算法**：优化的 OpenAPI 处理算法