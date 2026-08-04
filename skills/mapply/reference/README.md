# mapply - 参考文档

## 概述

mapply 是基于 .NET 10 开发的高性能对象映射系统，专为 .NET 开发者设计，提供强大的对象映射、依赖注入和 AOP 功能。

## 核心组件

### 1. MapsterMapper（Mapster 映射器）
- **位置**: scripts/mapster.cs
- **功能**: 使用 Mapster 库进行高性能对象映射
- **特性**: 
  - 运行时映射配置
  - 支持复杂对象映射
  - 高性能设计
  - 易于配置和扩展

### 2. MapperlyMapper（Mapperly 映射器）
- **位置**: scripts/mapperly.cs
- **功能**: 使用 Mapperly 库进行编译时映射生成
- **特性**: 
  - 编译时映射代码生成
  - 零反射开销
  - 类型安全
  - 高性能执行

### 3. AutofacContainer（Autofac 容器）
- **位置**: scripts/autofac_production.cs
- **功能**: 提供依赖注入和 AOP 功能
- **特性**: 
  - 模块化配置
  - AOP 拦截器支持
  - 生命周期管理
  - 与 ASP.NET Core 集成

## AOT 架构说明

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

### 性能优化技术

1. **Threading.Channels**: 高效的异步事件队列处理，支持背压控制
2. **ObjectPool**: 减少对象创建开销，优化内存使用
3. **Span 零拷贝**: 减少内存分配和复制
4. **TailLatencyOptimizer**: 尾延迟优化
5. **AggressiveOptimization**: 编译器级优化
6. **Cache-line 对齐**: 内存分配优化

## 使用示例

### 基本用法

```csharp
var mapperService = serviceProvider.GetRequiredService<IMapperService>();
var source = new SourceObject { Id = 1, Name = "测试" };
var target = mapperService.Map<SourceObject, TargetObject>(source);
Console.WriteLine($"映射结果: {target.Id}, {target.Name}");
```

### 高级配置

```csharp
var builder = WebApplication.CreateBuilder();

// 配置 Mapster
builder.Services.AddMapster(options => {
    options.Scan(typeof(Program).Assembly);
});

// 配置 Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder => {
    builder.RegisterModule<CoreModule>();
});

var app = builder.Build();

app.MapGet("/map", (IMapper mapper) => {
    var source = new Source { Id = 1, Name = "测试" };
    var dest = mapper.Map<Destination>(source);
    return Results.Ok(dest);
});

app.Run();
```

## 配置选项

### Mapster 配置

```json
{
  "Mapster": {
    "Enabled": true,
    "GlobalSettings": {
      "IgnoreNullValues": true,
      "PreserveReferenceBehavior": true,
      "MaxDepth": 10
    }
  }
}
```

### Mapperly 配置

```json
{
  "Mapperly": {
    "Enabled": true,
    "CodeGeneration": {
      "NullableReferenceTypes": true,
      "UseDeepCloning": true
    }
  }
}
```

### Autofac 配置

```json
{
  "Autofac": {
    "Enabled": true,
    "Modules": ["CoreModule"],
    "AOP": {
      "Enabled": true,
      "Interceptors": ["CallLogger"]
    }
  }
}
```

## 性能优化

1. **使用编译时映射**: 优先使用 Mapperly 进行编译时映射代码生成
2. **对象池使用**: 使用 ObjectPool 减少对象创建开销
3. **异步编程**: 使用异步 API 避免阻塞
4. **批量处理**: 批量处理映射请求提高效率
5. **内存优化**: 使用 Span 和 Memory 减少内存分配
6. **缓存使用**: 缓存常用映射结果

## 故障排除

### 常见问题

1. **映射失败**
   - 检查源对象和目标对象的属性名称是否匹配
   - 验证映射配置是否正确
   - 查看详细的错误日志

2. **性能问题**
   - 启用编译时映射
   - 调整对象池大小
   - 优化映射配置
   - 使用批量处理

3. **AOT 编译问题**
   - 确保所有依赖支持 AOT 编译
   - 检查运行时标识符设置
   - 验证单文件可执行配置

## 扩展开发

### 添加自定义映射器

```csharp
public class CustomMapper : IMapperService
{
    public TDest Map<TSource, TDest>(TSource source)
    {
        // 自定义映射逻辑
        var dest = Activator.CreateInstance<TDest>();
        // 执行映射
        return dest;
    }
}

// 注册自定义映射器
builder.Services.AddSingleton<IMapperService, CustomMapper>();
```

### 扩展 Autofac 模块

```csharp
public class CustomModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 注册自定义服务
        builder.RegisterType<CustomService>()
            .As<ICustomService>()
            .SingleInstance();
    }
}

// 注册自定义模块
builder.RegisterModule<CustomModule>();
```
