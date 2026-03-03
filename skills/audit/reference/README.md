# audit - 参考文档

## 概述

audit 是一个基于 .NET 10 的高性能审计日志系统，专为 .NET 开发者设计，提供完整的审计日志记录、查询、分析、加密和可视化功能，支持 AOT 编译优化，适用于各种规模的应用程序。

## 核心组件

### 1. 审计服务 (Audit Service)
- **位置**: scripts/ 目录下的 .cs 文件
- **功能**: 审计日志的核心业务逻辑处理
- **特性**: 
  - 核心审计功能实现
  - 高性能设计和优化
  - 完善的错误处理机制
  - 详细的日志记录
  - 支持 AOT 编译
  - 模块化架构设计

### 2. 审计中间件 (Audit Middleware)
- **功能**: 自动捕获 HTTP 请求和响应信息
- **特性**: 
  - 无需修改业务代码即可记录审计日志
  - 支持自定义审计字段
  - 支持排除特定路由
  - 高性能设计，低开销

### 3. 审计查询引擎 (Audit Query Engine)
- **功能**: 提供灵活的审计日志查询功能
- **特性**: 
  - 支持多种条件过滤
  - 支持分页查询
  - 支持排序和分组
  - 高性能查询算法

### 4. 审计分析引擎 (Audit Analysis Engine)
- **功能**: 提供审计日志的统计分析功能
- **特性**: 
  - 支持多种统计维度
  - 支持实时分析
  - 支持生成可视化报告

### 5. 审计加密模块 (Audit Encryption Module)
- **功能**: 提供审计日志的加密和解密功能
- **特性**: 
  - 支持对称加密和非对称加密
  - 支持敏感字段加密
  - 支持加密密钥管理

## 功能特性

### 1. 审计日志记录
- 支持多种审计日志记录方式
- 支持自定义审计事件类型
- 支持记录用户信息、IP 地址、操作时间等
- 支持记录详细的操作上下文
- 支持批量记录审计日志

### 2. 审计日志查询
- 支持多条件组合查询
- 支持按时间范围查询
- 支持按用户、事件类型、资源等查询
- 支持分页和排序
- 支持全文搜索

### 3. 审计日志分析
- 支持审计日志的统计分析
- 支持生成审计报告
- 支持异常行为检测
- 支持趋势分析

### 4. 审计日志加密
- 支持审计日志的加密存储
- 支持敏感信息加密
- 支持加密密钥轮换
- 支持加密算法配置

### 5. 审计策略管理
- 支持配置审计策略
- 支持按资源类型配置审计级别
- 支持审计日志保留期配置
- 支持审计日志清理策略

### 6. 实时审计监控
- 支持实时监控审计事件
- 支持异常审计事件告警
- 支持审计事件可视化

### 7. 审计日志导出
- 支持将审计日志导出为多种格式
- 支持 CSV、Excel、PDF 等格式
- 支持自定义导出字段

### 8. AOT 编译支持
- 支持将审计应用编译为本机代码
- 提高启动速度和运行性能
- 减少内存占用
- 支持多种平台和架构

## 使用示例

### 基础使用

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
    IpAddress = "192.168.1.100"
});
```

### 高级配置

```csharp
// 配置审计服务
var builder = WebApplication.CreateBuilder();

builder.Services.AddAuditServices(options => {
    options.ConnectionString = "YourConnectionString";
    options.EnableEncryption = true;
    options.RetentionDays = 365;
    options.LogLevel = AuditLogLevel.Information;
    options.BatchSize = 100;
    options.EnableCache = true;
    options.CacheSize = 1000;
});
```

## 配置选项

### 审计服务配置

```json
{
  "Audit": {
    "ConnectionString": "YourConnectionString",
    "EnableEncryption": true,
    "RetentionDays": 365,
    "LogLevel": "Information",
    "BatchSize": 100,
    "EnableCache": true,
    "CacheSize": 1000,
    "EnableDetailedLogging": false,
    "EncryptionAlgorithm": "AES-256"
  }
}
```

## 性能优化

### 1. 缓存使用
- 启用缓存以提高查询性能
- 配置合理的缓存大小
- 定期清理过期缓存

### 2. 异步编程
- 使用异步 API 避免阻塞主线程
- 充分利用 .NET 10 的异步优化

### 3. 批量处理
- 使用批量记录审计日志
- 配置合理的批处理大小

### 4. 连接池
- 使用连接池管理数据库连接
- 配置合理的连接池大小

### 5. AOT 编译优化
- 启用 AOT 编译以提高性能
- 使用 AOT 兼容的库和 API
- 避免使用反射等 AOT 不友好的特性

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
</PropertyGroup>
```

### AOT 编译命令

```bash
# 编译为 Windows x64 原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为 Linux x64 原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained

# 编译为 macOS x64 原生可执行文件
dotnet publish -c Release -r osx-x64 --self-contained
```

### AOT 编译注意事项

1. **使用 AOT 兼容的库**：确保所有依赖库都支持 AOT 编译
2. **避免反射**：尽量避免使用反射，或使用 Source Generator 替代
3. **资源加载**：确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**：避免使用动态代码生成技术
5. **测试验证**：在 AOT 编译后进行充分测试

## 与其他框架集成

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
```

### 与 Minimal API 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加审计服务
builder.Services.AddAuditServices(options => {
    options.ConnectionString = builder.Configuration.GetConnectionString("AuditDb");
    options.EnableEncryption = true;
});

// 配置 EF Core 上下文
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
```

## 扩展开发

### 添加自定义审计处理器

```csharp
public class CustomAuditProcessor : IAuditProcessor
{
    public async Task ProcessAsync(AuditLog log, CancellationToken cancellationToken = default)
    {
        // 自定义审计处理逻辑
        // 例如：发送审计事件到消息队列
        // 或：将审计日志同步到其他系统
        
        await Task.CompletedTask;
    }
}

// 注册自定义审计处理器
builder.Services.AddSingleton<IAuditProcessor, CustomAuditProcessor>();
```

### 添加自定义审计存储

```csharp
public class CustomAuditStore : IAuditStore
{
    public async Task<IEnumerable<AuditLog>> QueryAsync(AuditQuery query, CancellationToken cancellationToken = default)
    {
        // 自定义审计日志查询逻辑
        return new List<AuditLog>();
    }

    public async Task SaveAsync(IEnumerable<AuditLog> logs, CancellationToken cancellationToken = default)
    {
        // 自定义审计日志存储逻辑
        await Task.CompletedTask;
    }
}

// 注册自定义审计存储
builder.Services.AddSingleton<IAuditStore, CustomAuditStore>();
```

## 故障排除

### 常见问题

1. **审计日志记录失败**
   - 检查数据库连接配置
   - 检查数据库权限
   - 检查审计服务配置
   - 查看应用程序日志

2. **审计中间件不工作**
   - 确保中间件注册顺序正确
   - 检查是否配置了排除路由
   - 查看应用程序日志

3. **审计查询性能问题**
   - 确保数据库表有适当的索引
   - 优化查询条件
   - 考虑启用缓存
   - 考虑增加数据库资源

4. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 检查是否使用了反射等不兼容特性
   - 查看编译错误信息
   - 考虑使用 Source Generator 替代反射

## 最佳实践

1. **合理设计审计事件模型**
   - 定义清晰的审计事件类型
   - 包含足够的上下文信息
   - 避免记录敏感信息（或加密存储）

2. **优化审计日志存储**
   - 定期清理过期审计日志
   - 考虑分区存储
   - 考虑使用时序数据库

3. **监控审计系统性能**
   - 监控审计日志记录延迟
   - 监控审计查询响应时间
   - 监控审计系统资源使用情况

4. **使用 AOT 编译优化**
   - 对于性能敏感的应用，考虑使用 AOT 编译
   - 在开发环境进行充分测试
   - 监控 AOT 编译后的性能提升

5. **安全配置**
   - 确保审计日志存储安全
   - 配置适当的访问权限
   - 考虑加密敏感审计数据

## 版本更新记录

### 版本 1.0.0
- 初始版本发布
- 支持审计日志记录、查询、分析功能
- 支持 AOT 编译
- 支持多种存储方式
- 支持与 ASP.NET Core 和 Minimal API 集成

## 许可证

MIT License

## 联系方式

- 项目地址：https://github.com/vsa/audit
- 问题反馈：https://github.com/vsa/audit/issues
- 文档地址：https://vsa.github.io/audit/docs

## 贡献指南

欢迎大家贡献代码和文档！请查看 CONTRIBUTING.md 文件了解贡献指南。
