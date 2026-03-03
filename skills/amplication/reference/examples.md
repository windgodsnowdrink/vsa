# Amplication Agent Skill - 使用示例

## 1. 基础Amplication集成

### 1.1 初始化集成

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

// 配置服务容器
var services = new ServiceCollection();

// 添加日志服务
services.AddLogging(builder => builder.AddConsole());

// 添加HTTP客户端
services.AddHttpClient();

// 配置Amplication
var amplicationConfig = new AmplicationConfig
{
    ApiKey = "your-amplication-api-key",
    BaseUrl = "https://api.amplication.com",
    ProjectId = "your-project-id"
};

services.AddSingleton(amplicationConfig);
services.AddSingleton<AmplicationService>();
services.AddSingleton<AmplicationIntegration>();

// 构建服务提供者
using var serviceProvider = services.BuildServiceProvider();

// 获取Amplication集成服务
var amplicationIntegration = serviceProvider.GetRequiredService<AmplicationIntegration>();

// 初始化集成
await amplicationIntegration.InitializeAsync();

// 获取项目列表
var projects = await amplicationIntegration.GetProjectsAsync();
Console.WriteLine($"找到 {projects.Count} 个Amplication项目");
```

### 1.2 项目管理

```csharp
// 创建新项目
var projectRequest = new ProjectCreationRequest
{
    Name = "My New Project",
    Description = "使用Amplication技能创建的新项目",
    DatabaseType = DatabaseType.MongoDb,
    ApiType = ApiType.Rest
};

var newProject = await amplicationIntegration.CreateProjectAsync(projectRequest);
Console.WriteLine($"创建新项目成功: {newProject.Name} (ID: {newProject.Id})");

// 更新项目
var updateRequest = new ProjectUpdateRequest
{
    Name = "Updated Project Name",
    Description = "更新后的项目描述"
};

var updatedProject = await amplicationIntegration.UpdateProjectAsync(newProject.Id, updateRequest);
Console.WriteLine($"更新项目成功: {updatedProject.Name}");

// 获取项目详情
var projectDetails = await amplicationIntegration.GetProjectAsync(newProject.Id);
Console.WriteLine($"项目详情: {projectDetails.Name}, 创建时间: {projectDetails.CreatedAt}");
```

## 2. API生成示例

### 2.1 REST API生成（MongoDB）

```csharp
// 创建API生成请求
var apiRequest = new ApiGenerationRequest
{
    ProjectId = newProject.Id,
    EntityName = "Product",
    Properties = new List<EntityProperty>
    {
        new EntityProperty { Name = "Name", Type = "string", Required = true, MaxLength = 100 },
        new EntityProperty { Name = "Description", Type = "string", MaxLength = 500 },
        new EntityProperty { Name = "Price", Type = "decimal", Required = true, MinValue = 0 },
        new EntityProperty { Name = "SKU", Type = "string", Required = true, IsUnique = true },
        new EntityProperty { Name = "Category", Type = "string", Required = true },
        new EntityProperty { Name = "CreatedAt", Type = "datetime", DefaultValue = "Now" },
        new EntityProperty { Name = "UpdatedAt", Type = "datetime", DefaultValue = "Now", IsReadOnly = true },
        new EntityProperty { Name = "IsActive", Type = "boolean", DefaultValue = "true" }
    },
    ApiType = ApiType.Rest,
    DatabaseType = DatabaseType.MongoDb,
    AuthenticationType = AuthenticationType.Jwt,
    EnableCors = true,
    EnableSwagger = true,
    EnablePagination = true,
    DefaultPageSize = 50
};

// 生成API代码
var apiResult = await amplicationIntegration.GenerateApiAsync(apiRequest);

if (apiResult.Success)
{
    Console.WriteLine("API代码生成成功！");
    Console.WriteLine($"生成的API端点: {apiResult.ApiEndpoints.Count} 个");
    Console.WriteLine($"生成的数据库模型: {apiResult.DatabaseModels.Count} 个");
    
    // 显示生成的API端点
    foreach (var endpoint in apiResult.ApiEndpoints)
    {
        Console.WriteLine($"- {endpoint.HttpMethod} {endpoint.Path}");
    }
}
else
{
    Console.WriteLine($"API代码生成失败: {apiResult.ErrorMessage}");
}
```

### 2.2 GraphQL API生成（PostgreSQL）

```csharp
// 创建GraphQL API生成请求
var graphqlRequest = new ApiGenerationRequest
{
    ProjectId = newProject.Id,
    EntityName = "Order",
    Properties = new List<EntityProperty>
    {
        new EntityProperty { Name = "OrderNumber", Type = "string", Required = true, IsUnique = true },
        new EntityProperty { Name = "CustomerId", Type = "string", Required = true },
        new EntityProperty { Name = "TotalAmount", Type = "decimal", Required = true },
        new EntityProperty { Name = "OrderDate", Type = "datetime", DefaultValue = "Now" },
        new EntityProperty { Name = "Status", Type = "string", Required = true, DefaultValue = "Pending" },
        new EntityProperty { Name = "ShippingAddress", Type = "string", Required = true },
        new EntityProperty { Name = "Items", Type = "array", ItemType = "OrderItem" }
    },
    ApiType = ApiType.GraphQl,
    DatabaseType = DatabaseType.PostgreSql,
    AuthenticationType = AuthenticationType.OAuth2,
    EnableAuthorization = true
};

// 生成GraphQL API代码
var graphqlResult = await amplicationIntegration.GenerateApiAsync(graphqlRequest);

if (graphqlResult.Success)
{
    Console.WriteLine("GraphQL API代码生成成功！");
    Console.WriteLine($"生成的API端点: {graphqlResult.ApiEndpoints.Count} 个");
}
```

## 3. 数据库设计与迁移

### 3.1 数据库模型生成

```csharp
// 创建数据库生成请求
var dbRequest = new DatabaseGenerationRequest
{
    ProjectId = newProject.Id,
    EntityName = "Customer",
    Properties = new List<EntityProperty>
    {
        new EntityProperty { Name = "FirstName", Type = "string", Required = true },
        new EntityProperty { Name = "LastName", Type = "string", Required = true },
        new EntityProperty { Name = "Email", Type = "string", Required = true, IsUnique = true, Format = "email" },
        new EntityProperty { Name = "Phone", Type = "string", Format = "phone" },
        new EntityProperty { Name = "Address", Type = "object", Required = true },
        new EntityProperty { Name = "IsActive", Type = "boolean", DefaultValue = "true" }
    },
    DatabaseType = DatabaseType.MySql,
    EnableMigrations = true,
    EnableIndexOptimization = true,
    MigrationStrategy = MigrationStrategy.Safe
};

// 生成数据库模型
var dbResult = await amplicationIntegration.GenerateDatabaseAsync(dbRequest);

if (dbResult.Success)
{
    Console.WriteLine("数据库模型生成成功！");
    Console.WriteLine($"生成的数据库模型: {dbResult.DatabaseModels.Count} 个");
    Console.WriteLine($"生成的迁移脚本: {dbResult.MigrationScripts.Count} 个");
}
```

### 3.2 执行数据库迁移

```csharp
// 生成迁移脚本
var migrationScripts = await amplicationIntegration.GenerateMigrationScriptsAsync(newProject.Id);
Console.WriteLine($"生成了 {migrationScripts.Count} 个迁移脚本");

// 执行迁移
var migrationResult = await amplicationIntegration.ExecuteMigrationsAsync(newProject.Id);

if (migrationResult.Success)
{
    Console.WriteLine("数据库迁移执行成功！");
    Console.WriteLine($"应用了 {migrationResult.AppliedMigrations.Count} 个迁移");
}
else
{
    Console.WriteLine($"数据库迁移执行失败: {migrationResult.ErrorMessage}");
}
```

## 4. 前端代码生成

### 4.1 React前端生成

```csharp
// 创建前端生成请求
var frontendRequest = new FrontendGenerationRequest
{
    ProjectId = newProject.Id,
    Framework = FrontendFramework.React,
    UiLibrary = UiLibrary.MaterialUi,
    StateManagementLibrary = StateManagementLibrary.Redux,
    Components = new List<FrontendComponent>
    {
        FrontendComponent.ListView,
        FrontendComponent.Form,
        FrontendComponent.DetailView,
        FrontendComponent.Dashboard
    },
    ApiEndpoints = apiResult.ApiEndpoints,
    EnableAuthentication = true,
    EnableRouting = true,
    EnableTesting = true,
    EnableLinting = true
};

// 生成React前端代码
var frontendResult = await amplicationIntegration.GenerateFrontendAsync(frontendRequest);

if (frontendResult.Success)
{
    Console.WriteLine("React前端代码生成成功！");
    Console.WriteLine($"生成的组件: {frontendResult.Components.Count} 个");
    Console.WriteLine($"生成的页面: {frontendResult.Pages.Count} 个");
    Console.WriteLine($"生成的路由: {frontendResult.Routes.Count} 个");
}
else
{
    Console.WriteLine($"前端代码生成失败: {frontendResult.ErrorMessage}");
}
```

### 4.2 Angular前端生成

```csharp
// 创建Angular前端生成请求
var angularRequest = new FrontendGenerationRequest
{
    ProjectId = newProject.Id,
    Framework = FrontendFramework.Angular,
    UiLibrary = UiLibrary.AngularMaterial,
    StateManagementLibrary = StateManagementLibrary.NgRx,
    Components = new List<FrontendComponent>
    {
        FrontendComponent.ListView,
        FrontendComponent.Form,
        FrontendComponent.DetailView
    },
    ApiEndpoints = apiResult.ApiEndpoints
};

// 生成Angular前端代码
var angularResult = await amplicationIntegration.GenerateFrontendAsync(angularRequest);

if (angularResult.Success)
{
    Console.WriteLine("Angular前端代码生成成功！");
}
```

## 5. CI/CD流水线生成

### 5.1 GitHub Actions流水线

```csharp
// 创建CI/CD流水线生成请求
var ciRequest = new CICDPipelineRequest
{
    ProjectId = newProject.Id,
    PipelineType = PipelineType.GitHubActions,
    Stages = new List<PipelineStage>
    {
        PipelineStage.Build,
        PipelineStage.Test,
        PipelineStage.Lint,
        PipelineStage.Deploy
    },
    TestFramework = TestFramework.XUnit,
    PackageManager = PackageManager.Npm,
    DeploymentTarget = DeploymentTarget.AzureAppService,
    EnableSonarQube = true,
    EnableCodeCoverage = true
};

// 生成GitHub Actions流水线
var ciResult = await amplicationIntegration.GenerateCICDPipelineAsync(ciRequest);

if (ciResult.Success)
{
    Console.WriteLine("GitHub Actions流水线生成成功！");
    Console.WriteLine($"生成的流水线文件: {ciResult.PipelineFiles.Count} 个");
    
    // 显示生成的流水线文件
    foreach (var file in ciResult.PipelineFiles)
    {
        Console.WriteLine($"- {file.Path}");
    }
}
else
{
    Console.WriteLine($"CI/CD流水线生成失败: {ciResult.ErrorMessage}");
}
```

### 5.2 Docker配置生成

```csharp
// 创建Docker配置生成请求
var dockerRequest = new DockerGenerationRequest
{
    ProjectId = newProject.Id,
    ImageName = "my-amplication-app",
    ImageTag = "latest",
    BaseImage = "mcr.microsoft.com/dotnet/aspnet:10.0",
    EnableMultiStageBuild = true,
    EnableHealthCheck = true,
    EnableResourceLimits = true,
    Ports = new List<DockerPort>
    {
        new DockerPort { ContainerPort = 80, HostPort = 8080 },
        new DockerPort { ContainerPort = 443, HostPort = 8443 }
    },
    EnvironmentVariables = new Dictionary<string, string>
    {
        { "ASPNETCORE_ENVIRONMENT", "Production" },
        { "ASPNETCORE_URLS", "http://+:80" }
    }
};

// 生成Docker配置
var dockerResult = await amplicationIntegration.GenerateDockerConfigAsync(dockerRequest);

if (dockerResult.Success)
{
    Console.WriteLine("Docker配置生成成功！");
    Console.WriteLine($"生成的Docker文件: {dockerResult.DockerFiles.Count} 个");
}
```

## 6. 插件系统使用

### 6.1 自定义代码生成插件

```csharp
// 定义自定义插件
public class CustomValidationPlugin : IAmplicationPlugin
{
    public string Name => "CustomValidationPlugin";
    public string Version => "1.0.0";
    
    public Task<PluginResult> ExecuteAsync(PluginContext context)
    {
        var generatedFiles = new List<GeneratedFile>();
        
        // 为每个实体生成自定义验证规则
        foreach (var entity in context.Entities)
        {
            var validationCode = GenerateValidationCode(entity);
            generatedFiles.Add(new GeneratedFile
            {
                Path = $"{entity.Name}Validation.cs",
                Content = validationCode,
                FileType = FileType.SourceCode
            });
        }
        
        return Task.FromResult(new PluginResult
        {
            Success = true,
            GeneratedFiles = generatedFiles,
            Message = "自定义验证规则生成成功"
        });
    }
    
    private string GenerateValidationCode(EntityDefinition entity)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"using System.ComponentModel.DataAnnotations;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.Namespace}.Validation");
        sb.AppendLine("{");
        sb.AppendLine($"    public class {entity.Name}Validation");
        sb.AppendLine("    {");
        
        // 生成验证属性
        foreach (var property in entity.Properties)
        {
            if (property.Required)
            {
                sb.AppendLine($"        [Required(ErrorMessage = \"{property.Name} is required\")]");
            }
            
            if (property.Type == "string" && property.MaxLength > 0)
            {
                sb.AppendLine($"        [MaxLength({property.MaxLength}, ErrorMessage = \"{property.Name} must be at most {property.MaxLength} characters\")]");
            }
            
            if (property.Type == "decimal" && property.MinValue.HasValue)
            {
                sb.AppendLine($"        [Range({property.MinValue}, double.MaxValue, ErrorMessage = \"{property.Name} must be greater than or equal to {property.MinValue}\")]");
            }
            
            sb.AppendLine($"        public {property.Type} {property.Name} {{ get; set; }}");
            sb.AppendLine();
        }
        
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
}

// 注册并执行自定义插件
var customPlugin = new CustomValidationPlugin();
amplicationIntegration.RegisterPlugin(customPlugin);

// 执行插件
var pluginResult = await amplicationIntegration.ExecutePluginAsync(newProject.Id, customPlugin.Name);

if (pluginResult.Success)
{
    Console.WriteLine("自定义插件执行成功！");
    Console.WriteLine($"生成的文件: {pluginResult.GeneratedFiles.Count} 个");
}
else
{
    Console.WriteLine($"自定义插件执行失败: {pluginResult.ErrorMessage}");
}
```

## 7. 高级配置示例

### 7.1 批量实体生成

```csharp
// 创建多个实体定义
var entityRequests = new List<ApiGenerationRequest>
{
    new ApiGenerationRequest
    {
        ProjectId = newProject.Id,
        EntityName = "Product",
        Properties = new List<EntityProperty>
        {
            new EntityProperty { Name = "Name", Type = "string", Required = true },
            new EntityProperty { Name = "Price", Type = "decimal", Required = true }
        }
    },
    new ApiGenerationRequest
    {
        ProjectId = newProject.Id,
        EntityName = "Category",
        Properties = new List<EntityProperty>
        {
            new EntityProperty { Name = "Name", Type = "string", Required = true },
            new EntityProperty { Name = "Description", Type = "string" }
        }
    },
    new ApiGenerationRequest
    {
        ProjectId = newProject.Id,
        EntityName = "Review",
        Properties = new List<EntityProperty>
        {
            new EntityProperty { Name = "ProductId", Type = "string", Required = true },
            new EntityProperty { Name = "Rating", Type = "int", Required = true, MinValue = 1, MaxValue = 5 },
            new EntityProperty { Name = "Comment", Type = "string" }
        }
    }
};

// 并行生成多个实体的API
var tasks = entityRequests.Select(req => amplicationIntegration.GenerateApiAsync(req));
var results = await Task.WhenAll(tasks);

// 处理生成结果
int successCount = 0;
int failureCount = 0;

foreach (var result in results)
{
    if (result.Success)
    {
        successCount++;
    }
    else
    {
        failureCount++;
        Console.WriteLine($"实体生成失败: {result.ErrorMessage}");
    }
}

Console.WriteLine($"批量实体生成完成: 成功 {successCount} 个, 失败 {failureCount} 个");
```

### 7.2 高级API配置

```csharp
// 创建高级API配置
var advancedApiRequest = new ApiGenerationRequest
{
    ProjectId = newProject.Id,
    EntityName = "AdvancedProduct",
    Properties = new List<EntityProperty>
    {
        new EntityProperty { Name = "Name", Type = "string", Required = true, MaxLength = 200 },
        new EntityProperty { Name = "Price", Type = "decimal", Required = true, MinValue = 0, Precision = 18, Scale = 2 },
        new EntityProperty { Name = "Images", Type = "array", ItemType = "string" },
        new EntityProperty { Name = "Attributes", Type = "dictionary", Required = true },
        new EntityProperty { Name = "Metadata", Type = "json" },
        new EntityProperty { Name = "Version", Type = "int", DefaultValue = "1" },
        new EntityProperty { Name = "IsDeleted", Type = "boolean", DefaultValue = "false" }
    },
    ApiType = ApiType.Rest,
    DatabaseType = DatabaseType.MySql,
    AuthenticationType = AuthenticationType.OAuth2,
    EnableAuthorization = true,
    EnableCors = true,
    CorsOrigins = new List<string> { "https://myapp.com", "http://localhost:3000" },
    EnableSwagger = true,
    SwaggerTitle = "Advanced Product API",
    SwaggerVersion = "v2",
    EnablePagination = true,
    DefaultPageSize = 100,
    MaxPageSize = 1000,
    EnableSoftDelete = true,
    EnableAuditLogging = true,
    EnableRateLimiting = true,
    RateLimit = new RateLimitConfig
    {
        RequestsPerSecond = 100,
        BurstSize = 200
    }
};

// 生成高级API
var advancedResult = await amplicationIntegration.GenerateApiAsync(advancedApiRequest);

if (advancedResult.Success)
{
    Console.WriteLine("高级API生成成功！");
}
else
{
    Console.WriteLine($"高级API生成失败: {advancedResult.ErrorMessage}");
}
```

## 8. 代码质量检查

```csharp
// 创建代码质量检查请求
var qualityRequest = new CodeQualityRequest
{
    ProjectId = newProject.Id,
    CheckTypes = new List<CodeQualityCheck>
    {
        CodeQualityCheck.Formatting,
        CodeQualityCheck.Linting,
        CodeQualityCheck.Security,
        CodeQualityCheck.Performance,
        CodeQualityCheck.BestPractices
    },
    Languages = new List<string> { "csharp", "javascript", "typescript" },
    EnableAutoFix = true,
    EnableSonarQube = true,
    SonarQubeUrl = "https://sonarqube.mycompany.com",
    SonarQubeToken = "your-sonarqube-token",
    SonarProjectKey = $"mycompany:{newProject.Id}"
};

// 执行代码质量检查
var qualityResult = await amplicationIntegration.CheckCodeQualityAsync(qualityRequest);

if (qualityResult.Success)
{
    Console.WriteLine("代码质量检查完成！");
    Console.WriteLine($"发现 {qualityResult.Issues.Count} 个问题");
    Console.WriteLine($"自动修复了 {qualityResult.AutoFixedIssues.Count} 个问题");
    
    // 显示严重问题
    var criticalIssues = qualityResult.Issues.Where(i => i.Severity == Severity.Critical).ToList();
    if (criticalIssues.Any())
    {
        Console.WriteLine($"\n严重问题 ({criticalIssues.Count} 个):");
        foreach (var issue in criticalIssues.Take(5)) // 只显示前5个
        {
            Console.WriteLine($"- {issue.RuleId}: {issue.Message} ({issue.FilePath}:{issue.LineNumber})");
        }
    }
}
else
{
    Console.WriteLine($"代码质量检查失败: {qualityResult.ErrorMessage}");
}
```