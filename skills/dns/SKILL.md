# dns Agent Skill - DNS 技能

## 技能概述

基于 .NET 10 构建的高性能 DNS 技能，为 .NET 开发者提供强大的 DNS 功能支持。该技能采用 AOT（预编译）技术，提供极致的性能表现和启动速度，适用于各种 DNS 查询场景。

## 快速入门指南

### 安装依赖

在主应用程序的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Net.NameResolution@10.0.0
```

### 注册服务

在主应用程序中注册 DNS 服务：

```csharp
// 配置 DNS 选项
builder.Configuration.AddJsonFile("dns_aot.setting.json", optional: true);
builder.Services.Configure<DnsOptions>(builder.Configuration.GetSection("Dns"));

// 注册 DNS 服务
builder.Services.AddDns();
```

### 使用示例

```csharp
// 获取 DNS 引擎实例
var engine = serviceProvider.GetRequiredService<DnsAotEngine>();

// 查询 A 记录
var result = await engine.QueryAAsync("example.com");
if (result.Success)
{
    Console.WriteLine("查询结果：");
    foreach (var ip in result.Results)
    {
        Console.WriteLine($"  - {ip}");
    }
}
```

## 导航地图

```
dns/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── dns_aot.cs             # DNS 核心实现（AOT）
    ├── dns_aot.run.json       # 运行配置
    └── dns_aot.setting.json   # 应用设置
```

## 主要功能

1. **高性能 AOT 编译**：基于 .NET 10 AOT 技术，提供极致性能和启动速度
2. **支持多种 DNS 记录类型**：A、AAAA、MX、NS、CNAME、TXT 等
3. **智能缓存机制**：自动缓存查询结果，提高查询速度
4. **批量查询支持**：支持同时查询多个域名和记录类型
5. **多 DNS 服务器支持**：可配置多个 DNS 服务器，提高查询可靠性
6. **详细的状态监控**：实时监控 DNS 服务状态和性能指标
7. **灵活的配置选项**：支持通过配置文件自定义各种参数
8. **完善的错误处理**：详细的错误信息和日志记录
9. **命令行工具支持**：提供便捷的命令行查询工具

## 扩展说明

该技能提供了完整的 DNS 解决方案，您可以根据需要进行扩展：

1. **自定义 DNS 服务器**：实现您自己的 DNS 服务器解析逻辑
2. **扩展记录类型**：添加对更多 DNS 记录类型的支持
3. **集成其他系统**：与其他系统和框架集成，实现更复杂的 DNS 功能
4. **性能优化**：针对特定场景优化 DNS 查询性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，提高代码可测试性和可维护性
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **启用缓存**：启用缓存以提高查询速度，减少网络请求
4. **合理配置 DNS 服务器**：根据实际网络环境配置合适的 DNS 服务器
5. **设置适当的超时时间**：根据网络状况调整查询超时时间
6. **日志级别控制**：在生产环境中，将日志级别设置为 Information 或更高，减少日志开销
7. **定期监控状态**：定期监控 DNS 服务状态，及时发现和解决问题

## AOT 编译说明

该技能支持 .NET 10 AOT 编译，通过预编译将应用程序编译为本地机器代码，提供以下优势：

- **极致的启动速度**：无需 JIT 编译，直接运行本地代码
- **减少内存占用**：更小的运行时占用
- **提高安全性**：减少可攻击面，提高应用程序安全性
- **跨平台支持**：支持多种操作系统和架构

## 命令行工具

该技能提供了命令行工具，支持以下命令：

- `query <domain> <record_type>`：查询 DNS 记录
- `a <domain>`：查询 A 记录 (IPv4)
- `aaaa <domain>`：查询 AAAA 记录 (IPv6)
- `mx <domain>`：查询 MX 记录 (邮件)
- `ns <domain>`：查询 NS 记录 (名称服务器)
- `cname <domain>`：查询 CNAME 记录 (别名)
- `txt <domain>`：查询 TXT 记录 (文本)
- `status`：获取 DNS 服务状态
- `reset`：重置 DNS 服务状态
- `flush`：刷新 DNS 缓存
- `demo`：运行 DNS 演示

使用示例：
```
dns_aot.exe a example.com
dns_aot.exe query gmail.com MX
dns_aot.exe status
```