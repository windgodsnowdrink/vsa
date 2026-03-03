# MiniAPI 智能体技能 - MiniAPI 技能

## 技能概述

基于 .NET 10 的高性能 MiniAPI 技能，为 .NET 开发者提供强大的 MiniAPI 功能。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.AspNetCore.Http.Abstractions@10.0.0
#:package Microsoft.AspNetCore.Routing@10.0.0
```

### 注册服务

在主应用程序中注册 MiniAPI 服务：

```csharp
// 注册 MiniAPI 服务
var builder = WebApplication.CreateBuilder(args);

// 添加服务
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置中间件
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 定义 API 端点
app.MapGet("/", () => "Hello World!")
   .WithName("GetHello")
   .WithOpenApi();

app.Run();
```

### 使用示例

```csharp
// 创建 MiniAPI 应用
var builder = WebApplication.CreateBuilder(args);

// 添加依赖注入服务
builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

// 定义 API 端点
app.MapGet("/todos", async (ITodoService todoService) =>
{
    var todos = await todoService.GetAllAsync();
    return Results.Ok(todos);
});

app.MapGet("/todos/{id}", async (int id, ITodoService todoService) =>
{
    var todo = await todoService.GetByIdAsync(id);
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});

app.MapPost("/todos", async (Todo todo, ITodoService todoService) =>
{
    var createdTodo = await todoService.CreateAsync(todo);
    return Results.Created($"/todos/{createdTodo.Id}", createdTodo);
});

app.MapPut("/todos/{id}", async (int id, Todo todo, ITodoService todoService) =>
{
    var updatedTodo = await todoService.UpdateAsync(id, todo);
    return updatedTodo is not null ? Results.Ok(updatedTodo) : Results.NotFound();
});

app.MapDelete("/todos/{id}", async (int id, ITodoService todoService) =>
{
    var success = await todoService.DeleteAsync(id);
    return success ? Results.NoContent() : Results.NotFound();
});

app.Run();
```

## AOT 架构执行

### AOT 编译配置

```yaml
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property RuntimeIdentifier=linux-x64
#:property RuntimeIdentifier=osx-x64
```

### 执行流程

1. **编译阶段**：使用 .NET 10 的 AOT 编译功能将代码编译为本地机器码
2. **打包阶段**：将编译后的代码打包为单文件可执行文件
3. **部署阶段**：将打包后的可执行文件部署到目标环境
4. **运行阶段**：执行单文件可执行文件，处理 API 请求

## 导航地图

```
miniapi/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? *.cs                    # MiniAPI 核心实现
    ????? *.run.json              # 运行配置
    ????? *.setting.json          # 设置文件
```

## 主要功能

1. **高性能 API 路由**：基于 .NET 10 的高性能路由系统
2. **依赖注入集成**：无缝集成 Microsoft.Extensions.DependencyInjection
3. **中间件支持**：支持自定义中间件和内置中间件
4. **异步编程模型**：基于 async/await 的非阻塞操作
5. **无状态设计**：适合构建无状态服务和微服务
6. **Swagger 集成**：自动生成 API 文档
7. **健康检查**：内置健康检查端点
8. **CORS 支持**：跨域资源共享配置
9. **认证授权**：支持各种认证授权方案
10. **性能监控**：集成性能监控工具

## 扩展说明

本技能提供了完整的 MiniAPI 解决方案，您可以根据需要进行扩展：

1. **自定义中间件**：实现自定义中间件处理请求
2. **扩展功能**：添加新的 API 端点和服务
3. **与其他系统集成**：与数据库、消息队列等系统集成
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **API 设计**：遵循 RESTful API 设计规范
7. **安全性**：确保 API 的安全性
8. **可测试性**：设计可测试的 API 端点

## 性能优化建议

1. **内存分配优化**：减少不必要的内存分配
2. **GC 压力优化**：减少 GC 触发次数
3. **并发优化**：使用线程安全的代码
4. **批处理优化**：批量处理请求提高效率
5. **缓存使用**：合理使用缓存提高性能
6. **网络传输优化**：优化网络传输中的数据处理
7. **中间件优化**：减少不必要的中间件
8. **路由优化**：使用高效的路由设计

## AOT 编译最佳实践

1. **避免反射**：使用静态分析可检测的代码
2. **避免动态类型**：使用强类型
3. **避免运行时代码生成**：使用预编译代码
4. **优化内存使用**：使用 Span<T> 和 Memory<T>
5. **减少依赖**：最小化依赖项
6. **使用值类型**：减少 GC 压力
7. **避免大对象分配**：避免分配大于 85KB 的对象
8. **使用对象池**：对于频繁创建和销毁的对象，使用对象池
