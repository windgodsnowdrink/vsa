# ACME 证书管理技术

## 技术背景

ACME（Automatic Certificate Management Environment）是一种用于自动化证书管理的协议，主要用于获取和续订TLS/SSL证书。本技术实现了基于ACME协议的证书自动化管理，支持多种ACME客户端库集成。

## 技术用途

- 自动获取和续订SSL证书
- 监控证书有效期
- 支持多种ACME客户端
- 分布式锁保护
- 多种算法支持（RSA、ECDSA）
- Lettuce Encrypt集成

## 安装和依赖

### 主要依赖

- .NET 10.0
- System.Security.Cryptography
- Prometheus.Client（用于监控）

### 安装方法

在主应用程序的runfile中添加以下依赖：

```yaml
#:package AcmeDotnet@2.0.0
#:package Certes@3.0.0
#:package Prometheus.Client@6.0.0
```

## 目录结构

```
acme/
├── index.yaml                   # 元数据索引说明
├── SKILL.md                    # 核心指令入口点
├── reference/                  # 引用文件
│   ├── README.md              # 创建和配置说明
│   └── examples.md            # 调用和测试说明
├── scripts/                    # 脚本和工具
│   ├── acme_cert_monitor.cs   # 证书监控脚本
│   ├── acme_certes_integration.cs # Certes客户端集成
│   ├── acme_distributed_lock.cs # 分布式锁实现
│   ├── acme_dotnet_integration.cs # ACMEdotnet客户端集成
│   ├── acme_easy_rsa_acme.cs # Easy RSA实现
│   ├── acme_ecdsa_acme.cs # ECDSA算法支持
│   └── acme_lettuce_encrypt.cs # Lettuce Encrypt集成
└── data/                    # 数据文件夹
```

## 配置说明

### 证书监控配置

在应用程序中配置证书监控服务：

```csharp
// 注册证书监控服务
builder.Services.AddSingleton<CertificateMonitor>();

// 获取监控服务
var certMonitor = app.Services.GetRequiredService<CertificateMonitor>();

// 加载证书并开始监控
var cert = new X509Certificate2("cert.pfx", "password");
certMonitor.StartMonitoring(cert);
```

## 核心功能

1. **证书自动获取**：通过ACME协议自动获取免费SSL证书
2. **证书监控**：实时监控证书有效期，提前预警
3. **多客户端支持**：支持Certes、ACMEdotnet等多种ACME客户端
4. **分布式锁**：确保在分布式环境中证书获取的唯一性
5. **多种算法支持**：支持RSA、ECDSA等多种证书算法
6. **Lettuce Encrypt集成**：与ASP.NET Core无缝集成

## 安全注意事项

1. 保护好ACME账户私钥
2. 遵守ACME协议的速率限制
3. 确保域名验证通过
4. 定期备份证书私钥
5. 监控证书续约过程

## 常见问题

### Q: 如何处理证书获取失败？
A: 实现重试机制，记录详细日志，及时告警

### Q: 如何在集群环境中使用？
A: 使用分布式锁确保证书获取的唯一性

### Q: 如何支持多个域名？
A: 在证书申请时添加多个SAN（Subject Alternative Name）

### Q: 如何监控证书状态？
A: 使用Prometheus指标监控证书有效期

## 性能优化

1. 使用通道（Channel）处理证书监控
2. 使用尾延迟优化器（TailLatencyOptimizer）优化性能
3. 使用AggressiveOptimization属性优化关键方法
4. 实现证书缓存，避免重复处理

## 扩展性

1. 支持添加更多ACME客户端
2. 实现更复杂的证书续约策略
3. 扩展证书监控的告警方式
4. 添加证书自动部署功能
5. 实现证书轮换的零停机方案