# mapply 智能体技能 - mapply 技能

## 技能概述

基于 .NET 10 的高性能 mapply 技能，为 .NET 开发者提供强大的对象映射功能。

## 快速开始指南

### 安装依赖

在主应用的运行文件中添加以下依赖：

`yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Mapster@7.0.0
#:package Mapperly@3.0.0
#:package Autofac@7.0.0
`

### 注册服务

在主应用中注册 mapply 服务：

`csharp
// 注册 mapply 服务
builder.Services.AddSingleton<IMapperService, MapperService>();
builder.Services.AddSingleton<IMapsterMapper, MapsterMapper>();
builder.Services.AddSingleton<IMapperlyMapper, MapperlyMapper>();
`

### 使用示例

`csharp
// 获取 mapply 服务
var mapperService = serviceProvider.GetRequiredService<IMapperService>();

// 使用 mapply 功能
var source = new SourceObject { Id = 1, Name = "Test" };
var target = mapperService.Map<SourceObject, TargetObject>(source);
Console.WriteLine($"映射结果: Id={target.Id}, Name={target.Name}");
`

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

### 高性能映射实现

mapply 技能使用多种高性能映射库，提供不同场景下的最佳映射方案：

```csharp
// Mapster 映射示例
var config = TypeAdapterConfig.GlobalSettings;
config.NewConfig<Source, Destination>()
    .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
    .Map(dest => dest.Age, src => CalculateAge(src.BirthDate));

// Mapperly 映射示例
[Mapper]
public partial class PersonMapper
{
    public partial PersonDto MapToDto(Person person);
}
```

## 导航地图

`
mapply/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? autofac_production.cs     # Autofac 生产实现
    ????? autofac_production.run.json  # 运行配置
    ????? autofac_production.setting.json  # 设置文件
    ????? mapperly.cs     # Mapperly 映射实现
    ????? mapperly.run.json  # 运行配置
    ????? mapperly.setting.json  # 设置文件
    ????? mapster.cs     # Mapster 映射实现
    ????? mapster.run.json  # 运行配置
    ????? mapster.setting.json  # 设置文件
`

## 主要功能

1. **核心功能 1**：对象映射和转换
2. **核心功能 2**：映射配置和自定义
3. **核心功能 3**：高性能映射实现
4. **高性能设计**：优化的性能实现，支持大规模对象映射
5. **易用 API**：简单直观的 API 设计
6. **可扩展架构**：支持自定义扩展和集成

## 扩展说明

本技能提供了完整的 mapply 解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现 IMapperService 接口
2. **扩展功能**：添加新的映射功能和配置选项
3. **系统集成**：与其他系统集成
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **AOT 编译**：使用 AOT 编译提升启动速度和运行性能
7. **内存优化**：使用对象池和 Span 零拷贝技术优化内存使用
