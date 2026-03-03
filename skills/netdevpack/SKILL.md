# NetDevPack Agent Skill - NetDevPack 技能

## 技能概览

基于 .NET 10 的高性能 NetDevPack 技能，为 .NET 开发者提供强大的开发工具包功能。

## 快速入门指南

### 安装依赖

在主应用的运行文件中添加以下依赖：

`yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
`

### 注册服务

在主应用中注册 NetDevPack 服务：

`csharp
// 注册 NetDevPack 服务
builder.Services.Configure<NetDevPackOptions>(options => {
    options.EnableCache = true;
    options.CacheSize = 1000;
    options.Timeout = TimeSpan.FromSeconds(30);
    options.EnableDetailedLogging = false;
});
builder.Services.AddSingleton<INetDevPackService, NetDevPackService>();
builder.Services.AddSingleton<INetDevPackEnhancedService, NetDevPackEnhancedService>();
builder.Services.AddSingleton<ICacheManager, CacheManager>();
builder.Services.AddSingleton<IEventBus, EventBus>();
`

### 使用示例

`csharp
// 获取 NetDevPack 服务
var netDevPackService = serviceProvider.GetRequiredService<INetDevPackService>();
var enhancedService = serviceProvider.GetRequiredService<INetDevPackEnhancedService>();
var cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
var eventBus = serviceProvider.GetRequiredService<IEventBus>();

// 使用基础功能
var result = await netDevPackService.DoSomethingAsync();
Console.WriteLine($"结果: {result}");

// 使用增强功能
var enhancedResult = await enhancedService.DoEnhancedOperationAsync();
Console.WriteLine($"增强操作结果: {enhancedResult}");

// 使用缓存
await cacheManager.SetAsync("key", "value", TimeSpan.FromMinutes(5));
var cachedValue = await cacheManager.GetAsync<string>("key");
Console.WriteLine($"缓存值: {cachedValue}");

// 使用事件总线
await eventBus.PublishAsync(new SampleEvent { Message = "Hello NetDevPack!" });
`

## 导航地图

`
netdevpack/
├── index.yaml                   # 元数据索引说明
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能说明
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── netdevpack_integration.cs     # NetDevPack 核心实现
    ├── netdevpack_integration.run.json  # 运行配置
    ├── netdevpack_integration.setting.json  # 设置文件
    ├── netdevpack_enhanced_integration.cs     # NetDevPack 增强实现
    ├── netdevpack_enhanced_integration.run.json  # 运行配置
    └── netdevpack_enhanced_integration.setting.json  # 设置文件
`

## 主要功能

1. **依赖注入增强**：提供更强大的依赖注入功能，支持自动注册和生命周期管理
2. **缓存管理**：高性能缓存管理，支持内存缓存和分布式缓存
3. **事件总线**：轻量级事件总线，支持发布/订阅模式
4. **对象映射**：高性能对象映射，支持复杂对象转换
5. **验证工具**：强大的验证工具，支持数据验证和业务规则验证
6. **高性能设计**：使用 Span 优化、内存池、通道和零拷贝技术
7. **易于使用的 API**：简单直观的 API 设计，支持异步操作
8. **可扩展架构**：支持自定义扩展和集成

## 扩展说明

本技能提供了完整的 NetDevPack 解决方案，您可以根据需要进行扩展：

1. **自定义服务实现**：实现自定义的服务逻辑，扩展现有功能
2. **添加新功能**：添加新的工具和功能，满足特定业务需求
3. **与其他系统集成**：将 NetDevPack 与其他系统集成，如数据库、消息队列等
4. **性能优化**：针对特定场景优化性能，如添加缓存策略、批处理等

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务生命周期，提高代码可测试性
2. **异步编程**：优先使用异步 API 避免阻塞，提高系统吞吐量
3. **错误处理**：正确处理异常情况，确保系统稳定性
4. **日志记录**：添加适当的日志记录，便于故障排查
5. **性能监控**：监控关键性能指标，及时发现和解决性能问题
6. **缓存策略**：合理使用缓存，避免缓存穿透和缓存雪崩
7. **事件设计**：设计合理的事件结构，避免事件风暴

## 配置选项

### NetDevPackOptions 配置

| 选项 | 类型 | 默认值 | 描述 |
|------|------|--------|------|
| EnableCache | bool | true | 是否启用缓存 |
| CacheSize | int | 1000 | 缓存大小 |
| Timeout | TimeSpan | 30秒 | 操作超时时间 |
| EnableDetailedLogging | bool | false | 是否启用详细日志 |

## 性能特性

- **高吞吐量**：使用通道和批处理技术，支持高并发处理
- **低延迟**：使用 Span 和零拷贝技术，减少内存分配和拷贝
- **内存优化**：使用内存池和对象池，减少 GC 压力
- **可扩展性**：支持水平扩展，适用于大规模应用

## 版本兼容性

| .NET 版本 | 兼容性 |
|-----------|--------|------|
| .NET 10.0 | ✅ 完全支持 |
| .NET 9.0  | ✅ 支持 |
| .NET 8.0  | ✅ 支持 |
