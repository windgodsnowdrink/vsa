# audit Agent Skill - audit 技能

## 技能概述

基于 .NET 10 的高性能审计日志技能，为 .NET 开发者提供强大的审计功能，包括审计日志记录、查询、分析、加密和可视化等核心功能，支持 AOT 编译优化。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Configuration@10.0.0
#:package Microsoft.EntityFrameworkCore@10.0.0
```

### 注册服务

在您的主应用程序中注册审计服务：

```csharp
// 注册审计服务
var builder = WebApplication.CreateBuilder();

// 配置审计服务
builder.Services.AddAuditServices(options => {
    options.ConnectionString = "YourConnectionString";
    options.EnableEncryption = true;
    options.RetentionDays = 365;
    options.LogLevel = AuditLogLevel.Information;
});

// 配置审计日志存储（EF Core）
builder.Services.AddDbContext<AuditDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuditDb"));
});

var app = builder.Build();

// 使用审计中间件
app.UseAuditMiddleware();

app.Run();
```

### 使用示例

```csharp
// 获取审计服务
var auditService = serviceProvider.GetRequiredService<IAuditService>();

// 记录审计日志
await auditService.LogAsync(new AuditLog {
    EventType = "UserLogin",
    UserId = "user123",
    UserName = "张三",
    Action = "登录系统",
    Resource = "系统登录",
    IpAddress = "192.168.1.100",
    Details = new Dictionary<string, object> {
        { "Browser", "Chrome" },
        { "OS", "Windows 10" },
        { "Success", true }
    }
});

// 查询审计日志
var logs = await auditService.QueryAsync(new AuditQuery {
    EventType = "UserLogin",
    StartTime = DateTime.Now.AddDays(-7),
    EndTime = DateTime.Now,
    PageIndex = 1,
    PageSize = 20
});

// 导出审计日志
var csvData = await auditService.ExportToCsvAsync(logs);
File.WriteAllBytes("audit_logs.csv", csvData);
```

## 导航地图

```
audit/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
├── scripts/                    # 脚本和工具
│   ├── audit_analysis_pipeline.cs      # 审计分析流水线
│   ├── audit_analysis_pipeline.run.json  # 审计分析流水线运行配置
│   ├── audit_analysis_pipeline.setting.json  # 审计分析流水线设置文件
│   ├── audit_encryption.cs      # 审计日志加密实现
│   ├── audit_encryption.run.json  # 审计日志加密运行配置
│   ├── audit_encryption.setting.json  # 审计日志加密设置文件
│   ├── audit_integration.cs      # 审计服务集成实现
│   ├── audit_integration.run.json  # 审计服务集成运行配置
│   ├── audit_integration.setting.json  # 审计服务集成设置文件
│   ├── audit_query_endpoint.cs      # 审计查询端点实现
│   ├── audit_query_endpoint.run.json  # 审计查询端点运行配置
│   └── audit_query_endpoint.setting.json  # 审计查询端点设置文件
```

## 主要功能

1. **审计日志记录**: 支持多种审计日志记录方式，包括中间件、过滤器、手动记录等
2. **审计日志查询**: 提供灵活的审计日志查询接口，支持多种条件过滤
3. **审计日志分析**: 支持审计日志的统计分析和可视化
4. **审计日志加密**: 支持审计日志的加密存储和传输
5. **审计日志导出**: 支持将审计日志导出为多种格式，如 CSV、Excel、PDF 等
6. **实时审计监控**: 支持实时监控审计事件，及时发现异常行为
7. **审计策略管理**: 支持配置审计策略，灵活控制审计范围和级别
8. **审计报告生成**: 支持自动生成审计报告
9. **高性能设计**: 优化的性能实现，支持高并发审计日志处理
10. **AOT 编译优化**: 支持将审计应用编译为本机代码，提高启动速度和运行性能
11. **易于使用的 API**: 简洁直观的 API 设计，降低开发复杂度
12. **可扩展架构**: 支持自定义扩展和插件开发

## 扩展说明

此技能提供完整的审计解决方案，您可以根据需要进行扩展：

1. **自定义审计日志存储**: 实现自定义的审计日志存储方式
2. **扩展审计日志类型**: 添加新的审计日志类型和事件
3. **实现自定义审计分析**: 添加自定义的审计日志分析算法
4. **集成新的审计报告模板**: 添加新的审计报告模板
5. **性能优化**: 针对特定场景优化审计日志处理性能

## 最佳实践

1. **使用依赖注入**: 使用依赖注入管理审计服务，提高代码的可测试性和可维护性
2. **异步编程**: 优先使用异步 API 进行审计操作，避免阻塞主线程
3. **合理设置审计级别**: 根据业务需求合理设置审计级别，避免过度审计
4. **定期清理审计日志**: 配置合理的审计日志保留期，定期清理过期日志
5. **加密敏感信息**: 对审计日志中的敏感信息进行加密存储
6. **监控审计性能**: 监控审计日志处理性能，及时发现和解决性能瓶颈
7. **使用 AOT 编译**: 对于性能敏感的审计应用，考虑使用 AOT 编译优化
8. **合理设计审计数据模型**: 合理设计审计数据模型，提高查询效率
9. **实现审计日志备份**: 定期备份审计日志，防止数据丢失
10. **测试审计功能**: 充分测试审计功能，确保审计日志的准确性和完整性

## AOT 编译支持

### AOT 编译配置

在项目文件中添加以下配置以支持 AOT 编译：

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <TrimMode>Full</TrimMode>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>
```

### AOT 编译命令

```bash
# 编译为 Windows x64 原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为 Linux x64 原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained
```

### AOT 编译注意事项

1. **使用 AOT 兼容的库**: 确保使用的审计相关库支持 AOT 编译
2. **避免反射**: 避免在审计处理中使用反射，或使用 Source Generator 替代
3. **资源加载**: 确保所有审计资源都能在 AOT 编译时被正确处理
4. **动态代码生成**: 避免使用动态代码生成，如 System.Reflection.Emit
5. **测试验证**: 在 AOT 编译后进行充分的测试，确保审计功能正常工作
6. **性能优化**: AOT 编译可以显著提高审计应用的启动速度和运行性能
7. **内存优化**: AOT 编译可以减少审计应用的内存占用

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加审计服务
builder.Services.AddAuditServices(options => {
    options.ConnectionString = builder.Configuration.GetConnectionString("AuditDb");
    options.EnableEncryption = true;
    options.RetentionDays = 365;
});

// 配置 EF Core 上下文
builder.Services.AddDbContext<AuditDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuditDb"));
});

var app = builder.Build();

// 使用审计中间件
app.UseAuditMiddleware();

// 定义审计 API 端点
app.MapPost("/api/audit/log", async ([FromServices] IAuditService auditService, [FromBody] AuditLog log) => {
    await auditService.LogAsync(log);
    return Results.Ok();
});

app.MapGet("/api/audit/query", async ([FromServices] IAuditService auditService, [AsParameters] AuditQuery query) => {
    var logs = await auditService.QueryAsync(query);
    return Results.Ok(logs);
});

app.MapPost("/api/audit/export", async ([FromServices] IAuditService auditService, [FromBody] AuditQuery query) => {
    var logs = await auditService.QueryAsync(query);
    var csvData = await auditService.ExportToCsvAsync(logs);
    return Results.File(csvData, "text/csv", "audit_logs.csv");
});

app.Run();
```

### 与 Minimal API 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加审计服务
builder.Services.AddAuditServices(options => {
    options.ConnectionString = builder.Configuration.GetConnectionString("AuditDb");
    options.EnableEncryption = true;
});

builder.Services.AddDbContext<AuditDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuditDb"));
});

var app = builder.Build();

// 使用审计中间件
app.UseAuditMiddleware();

// 定义 Minimal API 端点
app.MapPost("/audit/log", async (IAuditService auditService, AuditLog log) => {
    await auditService.LogAsync(log);
    return Results.Ok();
});

app.MapGet("/audit/logs", async (IAuditService auditService, [AsParameters] AuditQuery query) => {
    var logs = await auditService.QueryAsync(query);
    return Results.Ok(logs);
});

app.Run();
```
