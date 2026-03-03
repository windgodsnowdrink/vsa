# etcd Agent Skill - etcd 技能

## 技能概述

基于 .NET 10 的高性能 etcd 技能，为 .NET 开发者提供强大的 etcd 功能，支持 AOT 编译以实现极致性能。

## 快速入门指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
```

### 注册服务

在主应用程序中注册 etcd 服务：

```csharp
// 注册 etcd 服务
builder.Services.AddEtcdAot();
```

### 使用示例

```csharp
// 获取 etcd 服务
var etcdService = serviceProvider.GetRequiredService<IEtcdService>();

// 设置键值对
var putResult = await etcdService.PutAsync("test-key", "test-value");
Console.WriteLine($"设置成功: {putResult.Success}");

// 获取键值对
var getResult = await etcdService.GetAsync("test-key");
Console.WriteLine($"获取成功: {getResult.Success}");
if (getResult.KeyValues != null && getResult.KeyValues.Count > 0)
{
    Console.WriteLine($"值: {getResult.KeyValues[0].Value}");
}
```

## 导航地图

```
etcd/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── etcd_aot.cs             # etcd AOT 核心实现
    ├── etcd_aot.run.json       # 运行配置
    ├── etcd_aot.setting.json   # 应用程序设置
    └── etcd_integration.cs     # etcd 集成实现
```

## 主要功能

1. **键值操作**：支持设置、获取和删除键值对
2. **前缀匹配**：支持前缀匹配查询
3. **事务处理**：支持 etcd 事务操作
4. **版本管理**：支持键的版本控制和修改追踪
5. **AOT 编译支持**：基于 .NET 10 AOT 编译，提供极致的性能和启动速度
6. **高性能设计**：优化的内存使用和并发支持，适合高负载场景
7. **易用的 API**：简单直观的 API 设计，便于集成到各种应用程序
8. **可扩展架构**：支持自定义扩展和集成

## 扩展说明

本技能提供了完整的 etcd 解决方案，您可以根据需要进行扩展：

1. **自定义服务实现**：实现 `IEtcdService` 接口来扩展或替换默认功能
2. **添加新功能**：扩展服务以支持更多 etcd 功能，如租约管理、监听等
3. **集成其他系统**：将 etcd 与其他系统集成
4. **性能优化**：针对特定场景进行性能优化

## 最佳实践

1. **依赖注入**：使用依赖注入来管理服务，便于测试和扩展
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **错误处理**：正确处理异常情况，确保系统稳定性
4. **日志记录**：添加适当的日志记录，便于调试和监控
5. **性能监控**：监控关键性能指标，及时发现和解决性能问题
6. **AOT 编译**：使用 AOT 编译发布模式，获得最佳性能
7. **配置管理**：使用 `Options` 模式管理配置，便于环境切换

## AOT 编译说明

本技能支持 .NET 10 AOT 编译，通过以下特性实现极致性能：

- **PublishAot=true**：启用 AOT 编译
- **InvariantGlobalization=true**：使用不变全球化模式，减少包大小
- **EnableCompilationRelaxations=true**：启用编译优化
- **PublishReadyToRun=true**：启用 ReadyToRun 编译，加速启动

## 命令行使用

使用以下命令行参数运行 etcd_aot：

```bash
# 显示帮助信息
dotnet run --project etcd_aot.cs -- help

# 设置键值对
dotnet run --project etcd_aot.cs -- put test-key test-value

# 获取键值对
dotnet run --project etcd_aot.cs -- get test-key

# 获取前缀匹配的键值对
dotnet run --project etcd_aot.cs -- get test --prefix

# 删除键值对
dotnet run --project etcd_aot.cs -- delete test-key

# 执行事务
dotnet run --project etcd_aot.cs -- transaction compare-key compare-value success-key success-value fail-key fail-value

# 显示版本信息
dotnet run --project etcd_aot.cs -- version
```