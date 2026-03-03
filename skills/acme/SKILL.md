# ACME Agent Skill - 证书自动化管理

## 技能概览

ACME（Automatic Certificate Management Environment）是一种用于自动化证书管理的协议，主要用于获取和续订TLS/SSL证书。本技能实现了基于ACME协议的证书自动化管理，支持多种ACME客户端库集成，提供证书监控、自动续订、分布式锁保护等功能。

## 快速入门指南

### 安装依赖

在主应用程序的runfile中添加以下依赖：

```yaml
#:package AcmeDotnet@2.0.0
#:package Certes@3.0.0
#:package Prometheus.Client@6.0.0
```

### 注册服务

在主应用程序中注册ACME证书监控服务：

```csharp
// 注册证书监控服务
builder.Services.AddSingleton<CertificateMonitor>();
```

### 启动监控

```csharp
// 获取监控服务
var certMonitor = app.Services.GetRequiredService<CertificateMonitor>();

// 加载证书并开始监控
var cert = new X509Certificate2("cert.pfx", "password");
certMonitor.StartMonitoring(cert);
```

## 导航地图

```
acme/
├── index.yaml                   # 元数据索引说明
├── SKILL.md                    # 技能入口点 (当前文件)
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

## 主要功能

1. **证书自动获取**：通过ACME协议自动获取免费SSL证书
2. **证书监控**：实时监控证书有效期，提前预警
3. **多客户端支持**：支持Certes、ACMEdotnet等多种ACME客户端
4. **分布式锁**：确保在分布式环境中证书获取的唯一性
5. **多种算法支持**：支持RSA、ECDSA等多种证书算法
6. **Lettuce Encrypt集成**：与ASP.NET Core无缝集成
7. **Prometheus监控**：提供证书状态指标监控

## 扩展说明

本技能提供了基础的ACME证书管理实现，您可以根据需要扩展：

1. 添加更多ACME客户端支持
2. 实现更复杂的证书续约策略
3. 扩展证书监控的告警方式
4. 添加证书自动部署功能
5. 实现证书轮换的零停机方案

## 最佳实践

1. 使用分布式锁确保证书获取的唯一性
2. 实现重试机制处理证书获取失败
3. 遵守ACME协议的速率限制
4. 定期备份证书私钥
5. 监控证书续约过程
6. 使用通道（Channel）优化证书监控性能
7. 实现证书缓存，避免重复处理
