# brighter - 参考文档

## 概述

brighter 是一个基于 .NET 10 的高性能 Brighter 命令查询责任分离（CQRS）系统，专为 .NET 开发者设计，支持 AOT（提前编译）编译，提供强大的消息处理和命令查询分离功能。

## 核心组件

### 1. 命令处理器 (Command Handler)
- **位置**: 应用程序中的命令处理器类
- **功能**: 处理命令，执行业务逻辑
- **特性**: 
  - 支持同步和异步处理
  - 支持事务处理
  - 支持重试和熔断机制
  - 支持 AOT 编译优化
  - 支持多种命令处理器实现

### 2. 查询处理器 (Query Handler)
- **位置**: 应用程序中的查询处理器类
- **功能**: 处理查询，返回查询结果
- **特性**: 
  - 支持同步和异步处理
  - 支持缓存查询结果
  - 支持 AOT 编译优化
  - 支持多种查询处理器实现

### 3. 命令处理器 (Command Processor)
- **位置**: Paramore.Brighter 库
- **功能**: 管理命令的发送和处理
- **特性**: 
  - 支持多种命令处理策略
  - 支持事务管理
  - 支持异步和同步处理
  - 支持 AOT 编译优化

### 4. 消息生产者 (Message Producer)
- **位置**: Paramore.Brighter 库
- **功能**: 将命令发布到消息中间件
- **特性**: 
  - 支持多种消息中间件
  - 支持事务性消息
  - 支持 AOT 编译优化

## 使用示例

### 基本命令处理

```csharp
// 定义命令
public record CreateProductCommand : Command
{
    public CreateProductCommand(Guid id, string name, decimal price, int stock)
    {
        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
        CommandId = Guid.NewGuid();
        Type = typeof(CreateProductCommand).Name;
        Version = 1;
    }
    
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
}

// 实现命令处理器
public class CreateProductCommandHandler : RequestHandlerAsync<CreateProductCommand>
{
    private readonly IProductRepository _repository;
    
    public CreateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    
    public override async Task<CreateProductCommand> HandleAsync(CreateProductCommand command)
    {
        // 实现命令处理逻辑
        var product = new Product(command.Id, command.Name, command.Price, command.Stock);
        await _repository.AddAsync(product);
        return await base.HandleAsync(command);
    }
}

// 注册命令处理器
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBrighter();
builder.Services.AddScoped<CreateProductCommandHandler>();

// 使用命令处理器
var commandProcessor = serviceProvider.GetRequiredService<IAmACommandProcessor>();
var command = new CreateProductCommand(Guid.NewGuid(), "测试产品", 19.99m, 100);
await commandProcessor.SendAsync(command);
```

### 查询处理

```csharp
// 定义查询
public record GetProductQuery : Query<GetProductQueryResult>
{
    public GetProductQuery(Guid id)
    {
        Id = id;
        QueryId = Guid.NewGuid();
        Type = typeof(GetProductQuery).Name;
        Version = 1;
    }
    
    public Guid Id { get; init; }
}

// 定义查询结果
public record GetProductQueryResult(Product Product);

// 实现查询处理器
public class GetProductQueryHandler : RequestHandlerAsync<GetProductQuery, GetProductQueryResult>
{
    private readonly IProductRepository _repository;
    
    public GetProductQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    
    public override async Task<GetProductQueryResult> HandleAsync(GetProductQuery query)
    {
        // 实现查询处理逻辑
        var product = await _repository.GetByIdAsync(query.Id);
        return new GetProductQueryResult(product);
    }
}

// 注册查询处理器
builder.Services.AddScoped<GetProductQueryHandler>();

// 使用查询处理器
var queryProcessor = serviceProvider.GetRequiredService<IAmAQueryProcessor>();
var query = new GetProductQuery(productId);
var result = await queryProcessor.SendAsync(query);
```

## 配置选项

### Brighter 配置

```json
{
  "BrighterSettings": {
    "EnableAot": true,                    // 启用 AOT 编译
    "HandlerLifetime": "Scoped",        // 处理器生命周期
    "CommandProcessorLifetime": "Scoped", // 命令处理器生命周期
    "EnableRetry": true,                 // 启用重试
    "MaxRetryAttempts": 3,               // 最大重试次数
    "RetryDelayMilliseconds": 1000,       // 重试延迟
    "EnableCircuitBreaker": true,         // 启用熔断
    "CircuitBreakerFailureThreshold": 5,  // 熔断失败阈值
    "CircuitBreakerResetTimeoutMilliseconds": 30000 // 熔断重置超时
  }
}
```

### 消息中间件配置 (RabbitMQ)

```json
{
  "RabbitMqSettings": {
    "HostName": "localhost",
    "VirtualHost": "/",
    "UserName": "guest",
    "Password": "guest",
    "Port": 5672,
    "Exchange": "brighter.exchange",
    "ExchangeType": "direct",
    "Durable": true,
    "AutoDelete": false,
    "PrefetchCount": 10
  }
}
```

## 性能优化

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译以获得最佳性能
2. **使用异步 API**: 优先使用异步 API 避免阻塞主线程
3. **优化命令处理器**: 确保命令处理器逻辑简洁高效
4. **使用缓存**: 对于频繁查询的数据，使用缓存优化查询性能
5. **批量处理**: 对于大量命令，考虑批量处理
6. **优化消息中间件配置**: 根据实际情况优化消息中间件配置
7. **使用适当的重试策略**: 根据业务需求配置适当的重试策略
8. **使用熔断机制**: 对于外部依赖，使用熔断机制保护系统
9. **优化数据库访问**: 优化数据库查询和连接管理
10. **监控性能**: 定期监控系统性能，及时发现瓶颈

## AOT 编译优化

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

<!-- 添加 AOT 兼容的依赖 -->
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Aot" Version="10.0.0" />
</ItemGroup>
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

### AOT 兼容注意事项

1. **使用 AOT 兼容的 Brighter 版本**: 确保使用的 Brighter 版本支持 AOT 编译
2. **避免反射**: 避免在命令处理器和查询处理器中使用反射
3. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **泛型类型**: 限制使用复杂的泛型类型
6. **序列化**: 确保所有需要序列化的类型都具有公共无参数构造函数
7. **测试验证**: 在 AOT 编译后进行充分测试

## 故障排除

### 常见问题

1. **命令处理器未找到**
   - 检查命令处理器是否已注册
   - 检查命令类型是否正确
   - 检查程序集是否已被扫描

2. **消息中间件连接失败**
   - 检查消息中间件配置
   - 检查网络连接
   - 检查防火墙设置
   - 查看消息中间件日志

3. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 检查是否使用了反射等不兼容的特性
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT 编译

4. **性能问题**
   - 启用性能监控
   - 检查命令处理器逻辑
   - 检查数据库查询
   - 考虑使用 AOT 编译
   - 优化消息中间件配置

5. **事务处理失败**
   - 检查事务配置
   - 检查数据库连接
   - 查看事务日志

## 扩展开发

### 自定义命令处理器

```csharp
// 自定义命令处理器基类
public abstract class CustomCommandHandler<T> : RequestHandlerAsync<T> where T : Command
{
    protected override async Task<T> HandleAsync(T command)
    {
        // 自定义前置处理逻辑
        await BeforeHandleAsync(command);
        
        // 调用派生类的处理逻辑
        var result = await HandleCoreAsync(command);
        
        // 自定义后置处理逻辑
        await AfterHandleAsync(result);
        
        return result;
    }
    
    protected abstract Task<T> HandleCoreAsync(T command);
    protected virtual Task BeforeHandleAsync(T command) => Task.CompletedTask;
    protected virtual Task AfterHandleAsync(T command) => Task.CompletedTask;
}

// 使用自定义命令处理器
public class CustomCreateProductHandler : CustomCommandHandler<CreateProductCommand>
{
    private readonly IProductRepository _repository;
    
    public CustomCreateProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    
    protected override async Task<CreateProductCommand> HandleCoreAsync(CreateProductCommand command)
    {
        // 实现命令处理逻辑
        var product = new Product(command.Id, command.Name, command.Price, command.Stock);
        await _repository.AddAsync(product);
        return command;
    }
    
    protected override Task BeforeHandleAsync(CreateProductCommand command)
    {
        // 自定义前置处理
        Console.WriteLine($"开始处理命令: {command.Type}");
        return Task.CompletedTask;
    }
}
```

### 自定义拦截器

```csharp
// 自定义命令拦截器
public class LoggingCommandInterceptor : RequestHandlerAsync<Command>
{
    private readonly ILogger<LoggingCommandInterceptor> _logger;
    
    public LoggingCommandInterceptor(ILogger<LoggingCommandInterceptor> logger)
    {
        _logger = logger;
    }
    
    public override async Task<Command> HandleAsync(Command command)
    {
        // 记录命令处理开始
        _logger.LogInformation("命令处理开始: {CommandType}, Id: {CommandId}", 
            command.Type, command.CommandId);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            // 调用下一个处理器
            var result = await base.HandleAsync(command);
            
            stopwatch.Stop();
            
            // 记录命令处理成功
            _logger.LogInformation("命令处理成功: {CommandType}, Id: {CommandId}, Duration: {Duration}ms", 
                command.Type, command.CommandId, stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // 记录命令处理失败
            _logger.LogError(ex, "命令处理失败: {CommandType}, Id: {CommandId}, Duration: {Duration}ms", 
                command.Type, command.CommandId, stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}

// 注册拦截器
builder.Services.AddScoped<LoggingCommandInterceptor>();
```

## 最佳实践

1. **使用依赖注入**: 始终使用依赖注入管理 Brighter 服务
2. **采用异步编程**: 优先使用异步 API 避免阻塞主线程
3. **合理设计命令和查询**: 遵循 CQRS 原则，合理设计命令和查询
4. **使用事务处理**: 对于关键操作，使用事务确保数据一致性
5. **实施重试机制**: 对于外部依赖，实施适当的重试机制
6. **使用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译
7. **添加适当的日志**: 添加详细的日志记录，便于调试和监控
8. **监控系统性能**: 定期监控系统性能，确保满足需求
9. **测试命令处理器**: 编写单元测试和集成测试
10. **遵循 CQRS 最佳实践**: 遵循 CQRS 架构的最佳实践

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
// 在 Startup.cs 中配置
public void ConfigureServices(IServiceCollection services)
{
    // 配置 Brighter
    services.AddBrighter(options =>
    {
        options.HandlerLifetime = ServiceLifetime.Scoped;
        options.CommandProcessorLifetime = ServiceLifetime.Scoped;
    });
    
    // 注册处理器
    services.AddHandlersFromAssemblies(typeof(Program).Assembly);
    
    // 其他服务配置
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}

// 在控制器中使用
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IAmAQueryProcessor _queryProcessor;
    
    public ProductsController(IAmACommandProcessor commandProcessor, IAmAQueryProcessor queryProcessor)
    {
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest request)
    {
        var command = new CreateProductCommand(Guid.NewGuid(), request.Name, request.Price, request.Stock);
        await _commandProcessor.SendAsync(command);
        return CreatedAtAction(nameof(Get), new { id = command.Id }, command);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetProductQuery(id);
        var result = await _queryProcessor.SendAsync(query);
        return Ok(result.Product);
    }
}
```

### 与 EF Core 集成

```csharp
// EF Core 集成示例
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Product> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }
}

// 注册 EF Core 上下文
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 注册仓库
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

## 部署建议

1. **容器化部署**: 使用 Docker 容器化应用，便于部署和扩展
2. **使用 Kubernetes**: 对于分布式应用，使用 Kubernetes 进行编排
3. **使用 CI/CD 流程**: 实现自动化构建、测试和部署
4. **使用环境变量**: 使用环境变量配置应用，便于不同环境部署
5. **实施蓝绿部署**: 减少部署风险
6. **使用 AOT 编译**: 对于生产环境，考虑使用 AOT 编译提高性能
7. **实施监控和告警**: 监控应用性能和健康状态，及时发现问题
8. **使用日志聚合**: 使用 ELK Stack 或其他日志聚合工具
9. **优化消息中间件**: 根据实际情况优化消息中间件配置
10. **实施灾难恢复**: 实施适当的灾难恢复策略

## 总结

Brighter 技能提供了一套完整的 .NET 10 CQRS 解决方案，支持 AOT 编译优化，适用于构建高性能、可扩展的分布式系统。通过遵循最佳实践和合理配置，可以构建出可靠、高效的 CQRS 应用。
