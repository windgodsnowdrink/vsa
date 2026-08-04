# Amplication Agent Skill - 参考文档

## 概述

Amplication是一个强大的低代码开发平台，用于自动化生成高质量的API、数据库和前端代码。本参考文档提供了详细的功能说明、配置选项和使用指南，帮助您充分利用Amplication技能的强大功能。

## 核心功能

### 1. Amplication项目集成

#### 功能说明

Amplication项目集成模块用于连接和管理Amplication平台上的项目，支持多项目管理、工作流集成和API密钥管理。

#### 配置选项

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| ApiKey | string | 无 | Amplication API密钥 |
| BaseUrl | string | https://api.amplication.com | Amplication API基础URL |
| ProjectId | string | 无 | 默认项目ID |
| ConnectionTimeout | TimeSpan | 00:00:30 | API连接超时时间 |
| RetryCount | int | 3 | API请求重试次数 |
| EnableDebugLogging | bool | false | 是否启用调试日志 |
| EnableMetrics | bool | true | 是否启用指标收集 |

#### 主要API

```csharp
// 初始化Amplication集成
Task InitializeAsync();

// 获取项目列表
Task<List<AmplicationProject>> GetProjectsAsync();

// 创建新项目
Task<AmplicationProject> CreateProjectAsync(ProjectCreationRequest request);

// 获取项目详情
Task<AmplicationProject> GetProjectAsync(string projectId);

// 更新项目
Task<AmplicationProject> UpdateProjectAsync(string projectId, ProjectUpdateRequest request);

// 删除项目
Task<bool> DeleteProjectAsync(string projectId);
```

### 2. API自动生成

#### 功能说明

API自动生成模块用于根据实体定义自动生成REST API或GraphQL API，支持多种数据库和认证方式。

#### 配置选项

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| ApiType | ApiType | Rest | API类型（Rest或GraphQL） |
| DatabaseType | DatabaseType | MongoDB | 数据库类型 |
| AuthenticationType | AuthenticationType | Jwt | 认证类型 |
| EnableCors | bool | true | 是否启用CORS |
| EnableSwagger | bool | true | 是否启用Swagger文档 |
| EnableAuthorization | bool | false | 是否启用授权 |
| EnablePagination | bool | true | 是否启用分页 |
| DefaultPageSize | int | 50 | 默认分页大小 |
| MaxPageSize | int | 1000 | 最大分页大小 |

#### 主要API

```csharp
// 生成API代码
Task<ApiGenerationResult> GenerateApiAsync(ApiGenerationRequest request);

// 获取API端点列表
Task<List<ApiEndpoint>> GetApiEndpointsAsync(string projectId);

// 测试API端点
Task<ApiTestResult> TestApiEndpointAsync(string endpointId, HttpRequestMessage request);

// 生成API文档
Task<ApiDocumentation> GenerateApiDocumentationAsync(string projectId);
```

### 3. 数据库设计与迁移

#### 功能说明

数据库设计与迁移模块用于自动生成数据库模型和迁移脚本，支持多种数据库类型。

#### 配置选项

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| DatabaseType | DatabaseType | MongoDB | 数据库类型 |
| ConnectionString | string | 无 | 数据库连接字符串 |
| EnableMigrations | bool | true | 是否启用迁移 |
| EnableSeedData | bool | false | 是否启用数据种子 |
| EnableIndexOptimization | bool | true | 是否启用索引优化 |
| MigrationStrategy | MigrationStrategy | Safe | 迁移策略（Safe或Auto） |
| SchemaName | string | public | 数据库模式名称（仅适用于关系型数据库） |

#### 主要API

```csharp
// 生成数据库模型
Task<DatabaseGenerationResult> GenerateDatabaseAsync(DatabaseGenerationRequest request);

// 生成迁移脚本
Task<List<MigrationScript>> GenerateMigrationScriptsAsync(string projectId);

// 执行迁移
Task<MigrationResult> ExecuteMigrationsAsync(string projectId);

// 生成数据种子
Task<SeedDataResult> GenerateSeedDataAsync(string projectId);

// 执行数据种子
Task<SeedDataResult> ExecuteSeedDataAsync(string projectId);
```

### 4. 前端代码生成

#### 功能说明

前端代码生成模块用于根据API定义自动生成前端代码，支持多种前端框架。

#### 配置选项

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| Framework | FrontendFramework | React | 前端框架 |
| UiLibrary | UiLibrary | MaterialUi | UI库 |
| EnableStateManagement | bool | true | 是否启用状态管理 |
| StateManagementLibrary | StateManagementLibrary | Redux | 状态管理库 |
| EnableRouting | bool | true | 是否启用路由 |
| EnableAuthentication | bool | true | 是否启用认证 |
| EnableTesting | bool | false | 是否启用测试 |
| EnableLinting | bool | true | 是否启用代码 linting |

#### 主要API

```csharp
// 生成前端代码
Task<FrontendGenerationResult> GenerateFrontendAsync(FrontendGenerationRequest request);

// 获取前端组件列表
Task<List<FrontendComponent>> GetFrontendComponentsAsync(string projectId);

// 生成单个组件
Task<FrontendComponentResult> GenerateComponentAsync(ComponentGenerationRequest request);
```

## 架构设计

### 1. 系统架构

```
┌─────────────────────────────────────────────────┐
│               AmplicationIntegration           │
├─────────────────┬───────────────────────────────┤
│  Configuration  │  Metrics Collection           │
├─────────────────┼───────────────────────────────┤
│                 │                               │
├─────────────────┴───────────────────────────────┤
│                 Core Modules                    │
├──────────────┬─────────────────┬────────────────┤
│  Project     │  API Generation │  Database      │
│  Management  │  Module         │  Generation    │
├──────────────┼─────────────────┼────────────────┤
│  Frontend    │  CI/CD Pipeline  │  Plugin System │
│  Generation  │  Generation      │                │
├──────────────┴─────────────────┴────────────────┤
│                 External Services               │
├──────────────┬─────────────────┬────────────────┤
│  Amplication │  Database       │  External APIs │
│  API         │  Services       │                │
└──────────────┴─────────────────┴────────────────┘
```

### 2. 核心组件

#### AmplicationService

负责与Amplication API通信，处理API请求和响应，实现重试机制和错误处理。

#### ApiGenerator

负责根据实体定义生成API代码，支持REST API和GraphQL API。

#### DatabaseGenerator

负责根据实体定义生成数据库模型和迁移脚本，支持多种数据库类型。

#### FrontendGenerator

负责根据API定义生成前端代码，支持多种前端框架。

#### CICDPipelineGenerator

负责生成CI/CD流水线配置，支持GitHub Actions、GitLab CI等。

#### PluginSystem

负责管理和执行Amplication插件，支持自定义代码生成模板。

## 性能优化

### 1. 内存优化

- 使用Span<T>和Memory<T>进行零拷贝操作
- 实现对象池减少GC压力
- 优化JSON序列化和反序列化
- 使用内存映射文件处理大文件

### 2. 并行处理

- 并行生成多个API端点
- 并行生成前端组件
- 异步API调用
- 批量处理请求

### 3. 缓存策略

- 缓存API响应
- 缓存生成的代码模板
- 缓存项目配置
- 实现智能缓存失效策略

### 4. API优化

- 实现请求合并
- 实现响应压缩
- 优化查询参数
- 实现分页和过滤

## 监控和指标

### 1. API调用指标

| 指标名称 | 类型 | 说明 |
|----------|------|------|
| amplication_api_calls_total | Counter | API调用总数 |
| amplication_api_call_success | Counter | 成功的API调用数 |
| amplication_api_call_error | Counter | 失败的API调用数 |
| amplication_api_call_duration_ms | Histogram | API调用持续时间 |
| amplication_api_rate_limit | Gauge | API速率限制 |

### 2. 代码生成指标

| 指标名称 | 类型 | 说明 |
|----------|------|------|
| amplication_code_generation_total | Counter | 代码生成总数 |
| amplication_code_generation_success | Counter | 成功的代码生成数 |
| amplication_code_generation_error | Counter | 失败的代码生成数 |
| amplication_code_generation_duration_ms | Histogram | 代码生成持续时间 |
| amplication_code_lines_generated | Counter | 生成的代码行数 |
| amplication_files_generated | Counter | 生成的文件数 |

### 3. 项目管理指标

| 指标名称 | 类型 | 说明 |
|----------|------|------|
| amplication_projects_total | Gauge | 项目总数 |
| amplication_entities_total | Gauge | 实体总数 |
| amplication_api_endpoints_total | Gauge | API端点总数 |
| amplication_database_models_total | Gauge | 数据库模型总数 |
| amplication_frontend_components_total | Gauge | 前端组件总数 |

## 故障排除

### 1. 常见问题

| 问题 | 可能原因 | 解决方案 |
|------|----------|----------|
| API调用失败 | 无效的API密钥或项目ID | 检查API密钥和项目ID是否正确 |
| 代码生成失败 | 实体定义错误 | 检查实体定义是否符合要求 |
| 数据库连接失败 | 无效的连接字符串 | 检查数据库连接字符串是否正确 |
| 前端生成失败 | 不支持的前端框架 | 检查前端框架是否受支持 |
| 插件执行失败 | 插件版本不兼容 | 检查插件版本是否与Amplication版本兼容 |

### 2. 日志分析

Amplication集成生成详细的日志，包括API请求和响应、代码生成过程、错误信息等。可以通过配置日志级别来控制日志详细程度：

```csharp
// 设置日志级别
builder.Logging.AddFilter("Amplication", LogLevel.Debug);
```

### 3. 调试技巧

- 使用Amplication CLI测试API调用
- 检查Amplication平台上的项目状态
- 查看生成的代码日志
- 使用Swagger UI测试API端点
- 检查数据库连接和权限

## 最佳实践

### 1. 项目设计

- 保持实体设计简洁明了
- 合理设计关系和索引
- 考虑性能和扩展性
- 遵循领域驱动设计原则

### 2. API设计

- 遵循RESTful设计原则
- 合理使用HTTP方法
- 提供良好的API文档
- 实现适当的认证和授权
- 考虑API版本管理

### 3. 数据库设计

- 选择适合项目的数据库类型
- 合理设计表结构和索引
- 考虑数据一致性和完整性
- 实现适当的备份策略
- 考虑数据库性能优化

### 4. 前端设计

- 选择适合项目的前端框架
- 合理设计组件结构
- 考虑状态管理方案
- 实现响应式设计
- 考虑性能优化

### 5. CI/CD设计

- 实现自动化测试
- 实现自动化部署
- 考虑回滚策略
- 实现适当的监控和告警
- 考虑安全性

## 扩展开发

### 1. 自定义代码生成模板

Amplication支持自定义代码生成模板，可以通过插件系统实现：

```csharp
// 实现自定义代码生成插件
public class CustomCodeGenerator : IAmplicationPlugin
{
    public string Name => "CustomCodeGenerator";
    public string Version => "1.0.0";
    
    public Task<PluginResult> ExecuteAsync(PluginContext context)
    {
        // 实现自定义代码生成逻辑
        var generatedCode = GenerateCustomCode(context.EntityDefinition);
        
        return Task.FromResult(new PluginResult
        {
            Success = true,
            GeneratedFiles = new List<GeneratedFile>
            {
                new GeneratedFile
                {
                    Path = "custom-code.cs",
                    Content = generatedCode,
                    FileType = FileType.SourceCode
                }
            }
        });
    }
    
    private string GenerateCustomCode(EntityDefinition entityDefinition)
    {
        // 生成自定义代码
        return $"// Custom code for {entityDefinition.Name}\n";
    }
}

// 注册自定义插件
var amplicationIntegration = app.Services.GetRequiredService<AmplicationIntegration>();
amplicationIntegration.RegisterPlugin(new CustomCodeGenerator());
```

### 2. 扩展API类型

可以通过扩展Amplication集成来支持更多API类型：

```csharp
// 实现自定义API生成器
public class CustomApiGenerator : IApiGenerator
{
    public ApiType ApiType => (ApiType)100; // 自定义API类型
    
    public Task<ApiGenerationResult> GenerateApiAsync(ApiGenerationRequest request)
    {
        // 实现自定义API生成逻辑
        return Task.FromResult(new ApiGenerationResult
        {
            Success = true,
            ApiEndpoints = new List<ApiEndpoint>(),
            GeneratedFiles = new List<GeneratedFile>()
        });
    }
}

// 注册自定义API生成器
var amplicationIntegration = app.Services.GetRequiredService<AmplicationIntegration>();
amplicationIntegration.RegisterApiGenerator(new CustomApiGenerator());
```

## 版本兼容性

| Amplication版本 | .NET版本 | 支持特性 |
|-----------------|----------|----------|
| 1.0.0 - 1.5.0 | .NET 8.0+ | 基础功能 |
| 2.0.0+ | .NET 10.0+ | 完整功能，支持AOT编译 |

## 许可证

本项目采用MIT许可证，详见LICENSE文件。

## 联系方式

- 项目地址：[GitHub Repository]
- 文档地址：[Documentation]
- 问题反馈：[Issues]
- 贡献指南：[Contributing Guide]