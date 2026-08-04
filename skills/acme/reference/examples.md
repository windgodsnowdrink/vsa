# ACME 证书管理技术 - 使用示例

## 调用示例

### 1. 证书监控服务使用

```csharp
// 注册证书监控服务
builder.Services.AddSingleton<CertificateMonitor>();

// 获取监控服务实例
var certMonitor = app.Services.GetRequiredService<CertificateMonitor>();

// 加载证书
var certPath = Path.Combine(Environment.CurrentDirectory, "certificates", "example.com.pfx");
var cert = new X509Certificate2(certPath, "password");

// 开始监控证书
certMonitor.StartMonitoring(cert);

// 在后台启动监控任务
var cancellationTokenSource = new CancellationTokenSource();
_ = certMonitor.MonitorAsync(cancellationTokenSource.Token);
```

### 2. ACME客户端集成（Certes）

```csharp
// 初始化Certes客户端
var client = new AcmeClient(
    new Uri("https://acme-v02.api.letsencrypt.org/directory"),
    new Certes.KeyAlgorithm(KeyAlgorithm.ES256));

// 注册ACME账户
var account = await client.NewAccountAsync(
    new[] { "admin@example.com" },
    true);

// 创建订单
var order = await client.NewOrderAsync(new[] { "example.com" });

// 完成域名验证
var authz = await client.GetAuthorizationAsync(order.Authorizations.First());
var dnsChallenge = await client.CompleteChallengeAsync(
    authz.Challenges.First(c => c.Type == "dns-01"));

// 等待验证完成
await client.WaitForValidationAsync(dnsChallenge);

// 生成证书
var privateKey = KeyFactory.NewKey(KeyAlgorithm.RS256);
var certChain = await client.GenerateCertificateAsync(order, privateKey);

// 保存证书
await File.WriteAllBytesAsync(
    "example.com.pfx",
    certChain.ToPfx(privateKey).Build("password"));
```

### 3. 分布式锁使用

```csharp
// 初始化分布式锁
var distributedLock = new AcmeDistributedLock(
    redisConnectionString: "localhost:6379",
    lockName: "acme-certificate-lock",
    lockTimeout: TimeSpan.FromMinutes(30));

// 使用分布式锁获取证书
using (await distributedLock.AcquireAsync())
{
    // 执行证书获取逻辑
    await GetCertificateAsync();
}
```

## 测试示例

### 1. 单元测试示例

```csharp
[Fact]
public void CertificateMonitor_Should_Update_Prometheus_Metric()
{
    // Arrange
    var monitor = new CertificateMonitor();
    var cert = new X509Certificate2(
        "test-cert.pfx",
        "password",
        X509KeyStorageFlags.Exportable);
    
    // Act
    monitor.StartMonitoring(cert);
    
    // Assert
    var metric = Metrics.DefaultRegistry.GetMetric<IGauge>("certificate_expiry_days");
    var value = metric.WithLabels("example.com").Value;
    Assert.True(value > 0);
}
```

### 2. 集成测试示例

```csharp
[Fact]
public async Task AcmeClient_Should_Register_Account()
{
    // Arrange
    var client = new AcmeClient(
        new Uri("https://acme-staging-v02.api.letsencrypt.org/directory"),
        new Certes.KeyAlgorithm(KeyAlgorithm.ES256));
    
    // Act
    var account = await client.NewAccountAsync(
        new[] { "test@example.com" },
        true);
    
    // Assert
    Assert.NotNull(account);
    Assert.NotNull(account.Key);
}
```

## 使用场景

### 1. Web服务器证书自动化

```csharp
// 在ASP.NET Core应用中集成
var builder = WebApplication.CreateBuilder(args);

// 配置Kestrel使用ACME证书
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 443, listenOptions =>
    {
        listenOptions.UseHttps(new AcmeCertificateLoader("example.com")
        {
            Email = "admin@example.com",
            AcmeDirectoryUri = new Uri("https://acme-v02.api.letsencrypt.org/directory"),
            CertificateStorePath = "./certificates"
        });
    });
});

var app = builder.Build();
app.MapGet("/", () => "Hello, HTTPS!");
app.Run();
```

### 2. 微服务架构中的证书管理

```csharp
// 为每个微服务实例配置证书监控
public class CertificateManagementService : BackgroundService
{
    private readonly CertificateMonitor _certMonitor;
    
    public CertificateManagementService(CertificateMonitor certMonitor)
    {
        _certMonitor = certMonitor;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 加载并监控所有证书
        var certDir = Path.Combine(Environment.CurrentDirectory, "certificates");
        foreach (var certFile in Directory.GetFiles(certDir, "*.pfx"))
        {
            var cert = new X509Certificate2(certFile, "password");
            _certMonitor.StartMonitoring(cert);
        }
        
        // 启动监控任务
        await _certMonitor.MonitorAsync(stoppingToken);
    }
}
```

## 常见问题和解决方案

### 问题1：证书获取失败，提示速率限制

**解决方案**：
- 使用Let's Encrypt测试环境进行开发和测试
- 实现指数退避重试机制
- 合理规划证书获取时间，避免集中获取

```csharp
// 实现指数退避重试
public async Task<X509Certificate2> GetCertificateWithRetryAsync(
    string domain,
    int maxRetries = 5,
    CancellationToken cancellationToken = default)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await GetCertificateAsync(domain, cancellationToken);
        }
        catch (AcmeRateLimitException ex)
        {
            if (attempt == maxRetries)
            {
                throw;
            }
            
            // 指数退避
            var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
            await Task.Delay(delay, cancellationToken);
        }
    }
    
    throw new InvalidOperationException("Failed to get certificate after multiple attempts");
}
```

### 问题2：证书监控占用过多资源

**解决方案**：
- 实现证书缓存机制
- 减少监控频率
- 使用批处理方式处理多个证书

```csharp
// 实现证书缓存
public class CachedCertificateMonitor : CertificateMonitor
{
    private readonly Dictionary<string, X509Certificate2> _certCache = new();
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromHours(1);
    private DateTime _lastCacheUpdate = DateTime.MinValue;
    
    public override void StartMonitoring(X509Certificate2 cert)
    {
        var domain = cert.GetNameInfo(X509NameType.DnsName, false);
        
        // 只有当证书不在缓存中或缓存已过期时才添加到监控通道
        if (!_certCache.ContainsKey(domain) || 
            DateTime.UtcNow - _lastCacheUpdate > _cacheExpiry)
        {
            _certCache[domain] = cert;
            _lastCacheUpdate = DateTime.UtcNow;
            base.StartMonitoring(cert);
        }
    }
}
```

## 性能优化建议

1. **使用通道处理证书**：利用.NET Channels的高性能特性处理大量证书监控
2. **实现证书批处理**：批量加载和处理证书，减少I/O操作
3. **使用尾延迟优化**：减少监控操作的尾延迟
4. **合理设置监控频率**：根据证书有效期设置不同的监控频率
5. **使用异步编程**：充分利用异步编程模型，提高并发处理能力
6. **实现证书缓存**：避免重复加载和处理相同证书

## 监控和告警

### Prometheus指标

| 指标名称 | 类型 | 描述 | 标签 |
|---------|------|------|------|
| certificate_expiry_days | Gauge | 证书剩余过期天数 | domain |
| certificate_monitor_operations_total | Counter | 证书监控操作总数 | status |
| certificate_monitor_latency_seconds | Histogram | 证书监控操作延迟 | |

### Grafana面板示例

```json
{
  "title": "Certificate Expiry Monitor",
  "panels": [
    {
      "title": "证书过期天数",
      "type": "gauge",
      "targets": [
        {
          "expr": "certificate_expiry_days",
          "legendFormat": "{{domain}}",
          "interval": "",
          "refId": "A"
        }
      ]
    },
    {
      "title": "即将过期证书",
      "type": "table",
      "targets": [
        {
          "expr": "certificate_expiry_days < 30",
          "legendFormat": "{{domain}}",
          "interval": "",
          "refId": "A"
        }
      ]
    }
  ]
}
```

## 部署建议

1. **容器化部署**：将证书管理服务容器化，便于扩展和管理
2. **定时任务**：使用定时任务定期检查和续订证书
3. **高可用设计**：部署多个实例，使用分布式锁确保唯一性
4. **持久化存储**：将证书存储在持久化存储中，如NFS或云存储
5. **密钥管理**：使用密钥管理服务（KMS）安全存储私钥
6. **审计日志**：记录所有证书管理操作，便于审计和 troubleshooting

## 安全最佳实践

1. **保护私钥**：使用安全的方式存储和传输私钥
2. **使用强密码**：为PFX文件设置强密码
3. **定期轮换密钥**：定期更换证书私钥
4. **使用安全的ACME目录**：生产环境使用Let's Encrypt生产目录，测试环境使用测试目录
5. **限制证书权限**：最小化证书的使用权限
6. **监控证书使用**：监控证书的使用情况，及时发现异常
7. **实现证书吊销机制**：在证书泄露时及时吊销证书