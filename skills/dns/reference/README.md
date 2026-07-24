# dns - 参考文档

## 概述

dns 是基于 .NET 10 AOT 架构的高性能 DNS 技能，专为 .NET 开发者设计，提供强大的 DNS 查询和域名解析功能支持。该技能采用高效的缓存机制和异步编程模型，适用于各种 DNS 相关场景。

## 核心组件

### 1. DnsService（DNS 服务）
- **位置**: scripts/dns_aot.cs
- **功能**: DNS 查询核心服务，负责域名解析、缓存管理和结果处理
- **特性**: 
  - 基于 .NET 10 AOT 编译，高性能
  - 支持多种 DNS 记录类型
  - 智能缓存机制
  - 异步编程模型
  - 多 DNS 服务器支持
  - 详细的错误处理和日志记录

### 2. DnsAotEngine（DNS AOT 引擎）
- **位置**: scripts/dns_aot.cs
- **功能**: 管理 DNS 功能调用的引擎，提供简洁的 API 接口
- **特性**: 
  - 简化的 API 调用
  - 统一的错误处理
  - 支持多种查询方式
  - 批量查询支持

## 核心接口

### IDnsService
DNS 服务的核心接口，定义了所有 DNS 查询方法：

| 方法名 | 描述 | 参数 | 返回值 |
|--------|------|------|--------|
| QueryAsync | 查询 DNS 记录 | domain: string, recordType: DnsRecordType | Task<DnsQueryResult> |
| BatchQueryAsync | 批量查询 DNS 记录 | queries: List<(string Domain, DnsRecordType RecordType)> | Task<List<DnsQueryResult>> |
| QueryAAsync | 查询 A 记录 (IPv4) | domain: string | Task<DnsQueryResult> |
| QueryAAAAAsync | 查询 AAAA 记录 (IPv6) | domain: string | Task<DnsQueryResult> |
| QueryMXAsync | 查询 MX 记录 (邮件) | domain: string | Task<DnsQueryResult> |
| QueryNSAsync | 查询 NS 记录 (名称服务器) | domain: string | Task<DnsQueryResult> |
| QueryCNAMEAsync | 查询 CNAME 记录 (别名) | domain: string | Task<DnsQueryResult> |
| QueryTXTAsync | 查询 TXT 记录 (文本) | domain: string | Task<DnsQueryResult> |
| FlushCacheAsync | 刷新 DNS 缓存 | 无 | Task<bool> |
| GetStatusAsync | 获取 DNS 服务状态 | 无 | Task<DnsStatus> |
| ResetStatusAsync | 重置 DNS 服务状态 | 无 | Task<bool> |

## 数据结构

### DnsRecordType（DNS 记录类型）
```csharp
public enum DnsRecordType
{
    A,      // A 记录 (IPv4 地址)
    AAAA,   // AAAA 记录 (IPv6 地址)
    MX,     // MX 记录 (邮件交换)
    NS,     // NS 记录 (名称服务器)
    CNAME,  // CNAME 记录 (别名)
    TXT,    // TXT 记录 (文本)
    SOA,    // SOA 记录 (起始授权机构)
    PTR,    // PTR 记录 (反向查询)
    SRV     // SRV 记录 (服务定位)
}
```

### DnsQueryResult（DNS 查询结果）
```csharp
public class DnsQueryResult
{
    public bool Success { get; set; }              // 查询是否成功
    public string Domain { get; set; }             // 查询的域名
    public DnsRecordType RecordType { get; set; }   // 记录类型
    public List<string> Results { get; set; }       // 查询结果列表
    public long ExecutionTimeMs { get; set; }       // 执行时间（毫秒）
    public string? ErrorMessage { get; set; }       // 错误信息
    public string? DnsServer { get; set; }          // 使用的 DNS 服务器
}
```

### DnsOptions（DNS 配置选项）
```csharp
public class DnsOptions
{
    public List<string> DnsServers { get; set; } = new List<string> { "8.8.8.8", "8.8.4.4" };  // 默认 DNS 服务器列表
    public int TimeoutMs { get; set; } = 5000;    // 查询超时时间（毫秒）
    public bool EnableCache { get; set; } = true;  // 是否启用缓存
    public int CacheSize { get; set; } = 1000;     // 缓存大小
    public int CacheExpirationSeconds { get; set; } = 300;  // 缓存过期时间（秒）
    public bool EnableDetailedLogging { get; set; } = false;  // 是否启用详细日志
    public bool EnablePerformanceMonitoring { get; set; } = true;  // 是否启用性能监控
    public bool UseSystemDns { get; set; } = true;  // 是否使用系统默认 DNS 服务器
}
```

### DnsStatus（DNS 状态信息）
```csharp
public class DnsStatus
{
    public bool IsRunning { get; set; }              // 服务是否正常运行
    public long ProcessedQueries { get; set; }        // 已处理的查询数
    public long SuccessfulQueries { get; set; }       // 成功查询数
    public long FailedQueries { get; set; }           // 失败查询数
    public double AverageQueryTimeMs { get; set; }    // 平均查询时间（毫秒）
    public double CacheHitRate { get; set; }          // 缓存命中率（%）
    public int CurrentCacheItems { get; set; }        // 当前缓存项数量
    public int MaxCacheItems { get; set; }            // 最大缓存项数量
    public DateTime StartTime { get; set; }           // 服务启动时间
}
```

## 配置选项

### DNS 配置（dns_aot.setting.json）

```json
{
  "Dns": {
    "DnsServers": ["8.8.8.8", "8.8.4.4", "1.1.1.1"],  // DNS 服务器列表
    "TimeoutMs": 5000,                             // 查询超时时间（毫秒）
    "EnableCache": true,                           // 是否启用缓存
    "CacheSize": 1000,                            // 缓存大小
    "CacheExpirationSeconds": 300,                 // 缓存过期时间（秒）
    "EnableDetailedLogging": false,                // 是否启用详细日志
    "EnablePerformanceMonitoring": true,           // 是否启用性能监控
    "UseSystemDns": true                           // 是否使用系统默认 DNS 服务器
  }
}
```

## 运行配置（dns_aot.run.json）

```json
{
  "$schema": "https://dot.net/v1/dotnet.run.schema.json",
  "framework": "net11.0",
  "options": {
    "PublishAot": true,                   // 启用 AOT 编译
    "InvariantGlobalization": true,       // 启用不变全球化
    "EnableCompilationRelaxations": true, // 启用编译松弛
    "PublishReadyToRun": true,            // 启用 ReadyToRun
    "LangVersion": "preview",            // 语言版本
    "Nullable": true,                     // 启用可空引用类型
    "ImplicitUsings": true                // 启用隐式 using
  },
  "dependencies": {
    "Microsoft.Extensions.DependencyInjection": "10.0.0",
    "Microsoft.Extensions.Hosting": "10.0.0",
    "Microsoft.Extensions.Logging": "10.0.0",
    "Microsoft.Extensions.Options": "10.0.0",
    "System.Net.NameResolution": "10.0.0"
  }
}
```

## 性能优化

1. **启用缓存**：启用缓存可以显著提高查询速度，减少网络请求
2. **调整缓存大小**：根据实际查询量调整缓存大小，避免缓存过大或过小
3. **合理设置缓存过期时间**：根据域名记录的更新频率设置合适的缓存过期时间
4. **配置多个 DNS 服务器**：配置多个 DNS 服务器可以提高查询可靠性
5. **使用异步 API**：使用异步 API 可以提高系统的并发处理能力
6. **批量查询**：对于多个查询请求，使用批量查询可以提高效率
7. **启用 AOT 编译**：AOT 编译可以提供极致的启动速度和运行性能

## 故障排除

### 常见问题

1. **查询失败**
   - 检查 DNS 服务器配置是否正确
   - 验证网络连接是否正常
   - 检查域名是否存在
   - 查看日志获取详细错误信息

2. **查询速度慢**
   - 启用缓存
   - 调整缓存大小和过期时间
   - 配置更近的 DNS 服务器
   - 检查网络连接质量
   - 启用 AOT 编译

3. **缓存问题**
   - 刷新缓存（使用 flush 命令）
   - 调整缓存过期时间
   - 禁用缓存（临时排查问题）

4. **服务异常**
   - 查看日志获取详细错误信息
   - 检查配置文件是否正确
   - 重启服务

## 扩展开发

### 自定义 DNS 服务

1. 实现 IDnsService 接口
2. 重写需要自定义的方法
3. 注册自定义服务

```csharp
builder.Services.AddSingleton<IDnsService, CustomDnsService>();
```

### 扩展记录类型

1. 在 DnsRecordType 枚举中添加新的记录类型
2. 在 QueryAsync 方法中添加相应的处理逻辑
3. 添加对应的便捷方法（如 QueryXYZAsync）

## AOT 编译注意事项

1. **依赖项**：确保所有依赖项都支持 AOT 编译
2. **反射**：避免在运行时使用反射，或使用 AOT 友好的反射方式
3. **动态类型**：谨慎使用 dynamic 类型，可能会影响 AOT 编译效果
4. **配置文件**：AOT 编译后，配置文件路径可能需要调整
5. **测试**：在 AOT 模式下进行充分测试，确保所有功能正常工作

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
dns_aot.exe flush
dns_aot.exe demo
```

## 版本历史

| 版本 | 日期 | 描述 |
|------|------|------|
| 1.0.0 | 2026-01-03 | 初始版本，基于 .NET 10 AOT 架构 |

## 许可证

MIT License