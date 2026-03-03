# encrypt Agent Skill - 加密技能

## 技能概述

基于 .NET 10 的高性能加密技能，为 .NET 开发者提供强大的加密功能，支持 AOT 编译以实现极致性能。

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

在主应用程序中注册加密服务：

```csharp
// 注册加密服务
builder.Services.AddEncryptAot();
```

### 使用示例

```csharp
// 获取加密服务
var encryptService = serviceProvider.GetRequiredService<IEncryptService>();

// 加密数据
var encryptResult = await encryptService.EncryptAsync("Hello World");
Console.WriteLine($"加密成功，密文: {encryptResult.EncryptedData}");
Console.WriteLine($"密钥: {encryptResult.GeneratedKey}");

// 解密数据
var decryptResult = await encryptService.DecryptAsync(encryptResult.EncryptedData!, encryptResult.GeneratedKey!);
Console.WriteLine($"解密成功，明文: {decryptResult.DecryptedData}");
```

## 导航地图

```
encrypt/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── encrypt_aot.cs          # 加密 AOT 核心实现
    ├── encrypt_aot.run.json    # 运行配置
    ├── encrypt_aot.setting.json  # 应用程序设置
    ├── aes_gcm_encryption_integration.cs          # AES-GCM 加密集成
    ├── bouncycastle_advanced.cs                  # BouncyCastle 高级功能
    ├── bouncycastle_integration.cs               # BouncyCastle 集成
    ├── netcore_encrypt_advanced.cs               # .NET Core 高级加密
    ├── netcore_encrypt_cache.cs                  # 加密缓存实现
    ├── netcore_encrypt_integration.cs            # .NET Core 加密集成
    ├── quantum_crypto_integration.cs             # 量子加密集成
    └── rsa_encryption_integration.cs             # RSA 加密集成
```

## 主要功能

1. **数据加密**：支持 AES-GCM、RSA 等多种加密算法
2. **数据解密**：支持多种加密算法的解密操作
3. **密钥生成**：自动生成安全的加密密钥
4. **哈希计算**：支持 SHA256、SHA512 等哈希算法
5. **哈希验证**：验证数据与哈希值的一致性
6. **AOT 编译支持**：基于 .NET 10 AOT 编译，提供极致的性能和启动速度
7. **高性能设计**：优化的内存使用和并发支持，适合高负载场景
8. **易用的 API**：简单直观的 API 设计，便于集成到各种应用程序
9. **可扩展架构**：支持自定义扩展和集成

## 扩展说明

本技能提供了完整的加密解决方案，您可以根据需要进行扩展：

1. **自定义加密算法**：实现 `IEncryptService` 接口来扩展或替换默认加密算法
2. **添加新的哈希算法**：扩展哈希算法支持
3. **集成硬件加密设备**：与硬件加密模块集成
4. **性能优化**：针对特定场景进行性能优化

## 最佳实践

1. **依赖注入**：使用依赖注入来管理服务，便于测试和扩展
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **错误处理**：正确处理异常情况，确保系统稳定性
4. **日志记录**：添加适当的日志记录，便于调试和监控
5. **性能监控**：监控关键性能指标，及时发现和解决性能问题
6. **AOT 编译**：使用 AOT 编译发布模式，获得最佳性能
7. **配置管理**：使用 `Options` 模式管理配置，便于环境切换
8. **密钥管理**：安全存储和管理加密密钥
9. **算法选择**：根据安全需求选择合适的加密算法

## AOT 编译说明

本技能支持 .NET 10 AOT 编译，通过以下特性实现极致性能：

- **PublishAot=true**：启用 AOT 编译
- **InvariantGlobalization=true**：使用不变全球化模式，减少包大小
- **EnableCompilationRelaxations=true**：启用编译优化
- **PublishReadyToRun=true**：启用 ReadyToRun 编译，加速启动

## 命令行使用

使用以下命令行参数运行 encrypt_aot：

```bash
# 显示帮助信息
dotnet run --project encrypt_aot.cs -- help

# 加密数据
dotnet run --project encrypt_aot.cs -- encrypt "Hello World"

# 解密数据
dotnet run --project encrypt_aot.cs -- decrypt <encryptedData> <key>

# 生成密钥
dotnet run --project encrypt_aot.cs -- genkey AES-GCM 256

# 生成哈希值
dotnet run --project encrypt_aot.cs -- genhash "Hello World" SHA512

# 验证哈希值
dotnet run --project encrypt_aot.cs -- valhash "Hello World" <hash> SHA256

# 显示版本信息
dotnet run --project encrypt_aot.cs -- version
```