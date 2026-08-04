# SSH 技能文档

## 1. 技能概述

SSH 技能是一个基于 .NET 10 和 AOT 编译技术的 SSH 客户端工具，提供高性能、安全的远程连接和文件传输功能。

### 1.1 主要特性

- **高性能 SSH 连接**：基于 SSH.NET 库，支持 SSH 协议版本 2
- **命令执行**：在远程服务器上执行命令并获取输出
- **文件传输**：支持 SCP 和 SFTP 协议的文件上传和下载
- **端口转发**：支持本地和远程端口转发
- **密钥管理**：支持密码和密钥文件认证
- **会话管理**：支持多个并发 SSH 会话
- **配置管理**：支持从配置文件加载 SSH 连接信息
- **批量操作**：支持批量执行命令和文件传输
- **Scrutor 集成**：提供高级依赖注入和服务注册功能
- **AOT 编译**：启用 AOT 编译，提高启动速度和运行时性能

### 1.2 技术栈

- **.NET 10**：目标框架
- **SSH.NET**：SSH 协议实现
- **Microsoft.Extensions.DependencyInjection**：依赖注入框架
- **Scrutor**：高级服务注册和装饰器模式
- **System.CommandLine**：命令行接口框架
- **System.Text.Json**：JSON 序列化和反序列化
- **System.Threading.Channels**：异步通道实现
- **System.IO.Pipelines**：高性能 IO 管道

## 2. 快速开始

### 2.1 安装和配置

1. **安装依赖**

   ```bash
   dotnet restore
   ```

2. **编译应用程序**

   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true -p:TrimMode=partial
   ```

3. **运行应用程序**

   ```bash
   ./ssh_core.exe connect --host example.com --port 22 --username user --password pass
   ```

### 2.2 基本用法

#### 连接到 SSH 服务器

```bash
# 使用密码认证
ssh_core.exe connect --host example.com --port 22 --username user --password pass

# 使用密钥文件认证
ssh_core.exe connect --host example.com --port 22 --username user --keyfile C:\path\to\key.pem
```

#### 执行远程命令

```bash
ssh_core.exe execute --host example.com --command "ls -la"
```

#### 上传文件

```bash
ssh_core.exe upload --local-path C:\local\file.txt --remote-path /remote/path/file.txt
```

#### 下载文件

```bash
ssh_core.exe download --remote-path /remote/path/file.txt --local-path C:\local\file.txt
```

#### 演示 Scrutor 用法

```bash
ssh_core.exe scrutor-demo
```

## 3. 核心功能

### 3.1 SSH 连接管理

- **连接建立**：建立到远程服务器的 SSH 连接
- **连接池**：管理多个 SSH 连接，提高性能
- **自动重连**：在连接断开时自动重连
- **连接超时**：设置连接超时时间
- **保持活动**：定期发送保持活动消息，防止连接断开

### 3.2 命令执行

- **同步执行**：同步执行远程命令并获取结果
- **异步执行**：异步执行远程命令，不阻塞主线程
- **命令超时**：设置命令执行超时时间
- **环境变量**：设置远程命令的环境变量
- **工作目录**：设置远程命令的工作目录

### 3.3 文件传输

- **SCP 协议**：使用 SCP 协议传输文件
- **SFTP 协议**：使用 SFTP 协议传输文件
- **文件权限**：设置上传文件的权限
- **目录递归**：支持递归上传和下载目录
- **文件校验**：支持文件传输后的校验
- **进度报告**：提供文件传输进度报告

### 3.4 端口转发

- **本地端口转发**：将本地端口转发到远程服务器
- **远程端口转发**：将远程服务器端口转发到本地
- **动态端口转发**：创建 SOCKS 代理

### 3.5 密钥管理

- **密钥生成**：生成 SSH 密钥对
- **密钥加载**：从文件加载 SSH 密钥
- **密钥转换**：支持多种密钥格式转换
- **密钥加密**：支持加密的私钥文件

### 3.6 会话管理

- **会话创建**：创建和管理 SSH 会话
- **会话共享**：在多个操作之间共享会话
- **会话关闭**：正确关闭 SSH 会话

### 3.7 配置管理

- **配置文件**：从 JSON 或 YAML 文件加载配置
- **环境变量**：从环境变量加载配置
- **命令行参数**：从命令行参数加载配置
- **配置优先级**：支持配置优先级管理

### 3.8 批量操作

- **批量命令**：在多个服务器上执行相同的命令
- **批量文件传输**：在多个服务器之间传输文件
- **并行执行**：并行执行批量操作，提高效率
- **错误处理**：处理批量操作中的错误

### 3.9 Scrutor 集成

- **服务注册**：使用 Scrutor 自动注册服务
- **装饰器模式**：使用 Scrutor 实现装饰器模式
- **依赖注入**：集成 Microsoft.Extensions.DependencyInjection
- **服务发现**：自动发现和注册服务

## 4. API 参考

### 4.1 ISshService

```csharp
public interface ISshService
{
    Task<SshConnection> ConnectAsync(string host, int port, string username, string password);
    Task<SshConnection> ConnectAsync(string host, int port, string username, string privateKeyPath, string passphrase = null);
    Task<string> ExecuteCommandAsync(SshConnection connection, string command, int timeout = 30000);
    Task UploadFileAsync(SshConnection connection, string localPath, string remotePath);
    Task DownloadFileAsync(SshConnection connection, string remotePath, string localPath);
    Task CloseConnectionAsync(SshConnection connection);
}
```

### 4.2 ISshSessionManager

```csharp
public interface ISshSessionManager
{
    Task<SshSession> CreateSessionAsync(string host, int port, string username, string password);
    Task<SshSession> CreateSessionAsync(string host, int port, string username, string privateKeyPath, string passphrase = null);
    Task CloseSessionAsync(SshSession session);
    Task<IEnumerable<SshSession>> GetActiveSessionsAsync();
}

### 4.3 ISshFileTransferService

```csharp
public interface ISshFileTransferService
{
    Task UploadAsync(SshConnection connection, string localPath, string remotePath, bool recursive = false);
    Task DownloadAsync(SshConnection connection, string remotePath, string localPath, bool recursive = false);
    Task<IEnumerable<string>> ListFilesAsync(SshConnection connection, string remotePath);
    Task CreateDirectoryAsync(SshConnection connection, string remotePath);
    Task DeleteFileAsync(SshConnection connection, string remotePath);
}
```

### 4.4 ISshConfigService

```csharp
public interface ISshConfigService
{
    Task<SshConfig> LoadConfigAsync(string configPath);
    Task SaveConfigAsync(SshConfig config, string configPath);
    Task<SshConfigEntry> GetConfigEntryAsync(string host);
}
```

### 4.5 ISshKeyManager

```csharp
public interface ISshKeyManager
{
    Task<(string PublicKey, string PrivateKey)> GenerateKeyPairAsync(string algorithm = "RSA", int keySize = 2048);
    Task<string> LoadPrivateKeyAsync(string privateKeyPath, string passphrase = null);
    Task SaveKeyPairAsync(string publicKey, string privateKey, string publicKeyPath, string privateKeyPath);
}
```

### 4.6 ISshBatchService

```csharp
public interface ISshBatchService
{
    Task<Dictionary<string, string>> ExecuteBatchCommandAsync(IEnumerable<string> hosts, string command);
    Task<Dictionary<string, bool>> UploadBatchFileAsync(IEnumerable<string> hosts, string localPath, string remotePath);
    Task<Dictionary<string, bool>> DownloadBatchFileAsync(IEnumerable<string> hosts, string remotePath, string localPath);
}
```

## 5. AOT 编译

### 5.1 编译选项

```json
{
  "compilationOptions": {
    "targetFramework": "net10.0",
    "langVersion": "preview",
    "nullable": "enable",
    "implicitUsings": "enable",
    "publishOptions": {
      "PublishAot": true,
      "TrimMode": "partial",
      "SelfContained": true,
      "PublishSingleFile": true,
      "PublishReadyToRun": true,
      "RuntimeIdentifier": "win-x64"
    }
  }
}
```

### 5.2 性能优化

- **启动速度**：AOT 编译显著提高应用程序启动速度
- **内存使用**：减少运行时内存使用
- **运行时性能**：提高运行时执行性能
- **部署大小**：减小部署包大小

### 5.3 注意事项

- **反射**：AOT 编译会影响反射功能，需要注意使用
- **动态代码生成**：AOT 编译不支持动态代码生成
- **序列化**：某些序列化库可能需要特殊处理
- **第三方库**：确保第三方库支持 AOT 编译

## 6. 示例

### 6.1 基本连接示例

```csharp
using var sshService = serviceProvider.GetRequiredService<ISshService>();

// 连接到 SSH 服务器
var connection = await sshService.ConnectAsync(
    "example.com",
    22,
    "username",
    "password"
);

// 执行命令
var result = await sshService.ExecuteCommandAsync(connection, "ls -la");
Console.WriteLine(result);

// 上传文件
await sshService.UploadFileAsync(connection, "./localfile.txt", "/remotefile.txt");

// 下载文件
await sshService.DownloadFileAsync(connection, "/remotefile.txt", "./localfile.txt");

// 关闭连接
await sshService.CloseConnectionAsync(connection);
```

### 6.2 使用密钥文件连接

```csharp
using var sshService = serviceProvider.GetRequiredService<ISshService>();

// 使用密钥文件连接
var connection = await sshService.ConnectAsync(
    "example.com",
    22,
    "username",
    "./id_rsa",
    "passphrase" // 可选
);

// 执行命令
var result = await sshService.ExecuteCommandAsync(connection, "uptime");
Console.WriteLine(result);

// 关闭连接
await sshService.CloseConnectionAsync(connection);
```

### 6.3 批量操作示例

```csharp
using var batchService = serviceProvider.GetRequiredService<ISshBatchService>();

// 批量执行命令
var hosts = new[] { "server1.com", "server2.com", "server3.com" };
var results = await batchService.ExecuteBatchCommandAsync(hosts, "df -h");

foreach (var (host, result) in results)
{
    Console.WriteLine($"=== {host} ===");
    Console.WriteLine(result);
}

// 批量上传文件
var uploadResults = await batchService.UploadBatchFileAsync(hosts, "./script.sh", "/tmp/script.sh");

foreach (var (host, success) in uploadResults)
{
    Console.WriteLine($"Upload to {host}: {(success ? "Success" : "Failed"}");
}
```

### 6.4 使用配置文件

```csharp
using var configService = serviceProvider.GetRequiredService<ISshConfigService>();
using var sshService = serviceProvider.GetRequiredService<ISshService>();

// 加载配置
var config = await configService.LoadConfigAsync("./ssh-config.json");

// 获取配置项
var entry = await configService.GetConfigEntryAsync("production");

// 连接到服务器
var connection = await sshService.ConnectAsync(
    entry.Host,
    entry.Port,
    entry.Username,
    entry.PrivateKeyPath,
    entry.Passphrase
);

// 执行命令
var result = await sshService.ExecuteCommandAsync(connection, "docker ps");
Console.WriteLine(result);

// 关闭连接
await sshService.CloseConnectionAsync(connection);
```

### 6.5 Scrutor 用法示例

```csharp
// 配置依赖注入
var services = new ServiceCollection();

// 使用 Scrutor 注册服务
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

// 使用 Scrutor 注册装饰器
services.Decorate<ISshService, SshServiceLoggingDecorator>();
services.Decorate<ISshService, SshServiceCachingDecorator>();

// 构建服务提供者
var serviceProvider = services.BuildServiceProvider();

// 使用服务
using var sshService = serviceProvider.GetRequiredService<ISshService>();
var connection = await sshService.ConnectAsync(
    "example.com",
    22,
    "username",
    "password"
);

// 执行命令
var result = await sshService.ExecuteCommandAsync(connection, "ls -la");
Console.WriteLine(result);

// 关闭连接
await sshService.CloseConnectionAsync(connection);
```

## 7. 故障排除

### 7.1 常见问题

| 问题 | 原因 | 解决方案 |
|-----|------|--------|
| 连接被拒绝 | 服务器地址或端口错误 | 检查服务器地址和端口 |
| 认证失败 | 用户名或密码错误 | 检查用户名和密码 |
| 密钥文件权限错误 | 密钥文件权限过于宽松 | 设置密钥文件权限为 600 |
| 命令执行超时 | 命令执行时间过长 | 增加命令超时时间 |
| 文件传输失败 | 路径不存在或权限不足 | 检查路径和权限 |
| 端口转发失败 | 端口被占用 | 选择其他端口 |
| 内存不足 | 批量操作规模过大 | 减小批量操作规模 |
| 反射错误 | AOT 编译影响反射 | 使用静态分析或配置修剪 |

### 7.2 调试技巧

1. **启用详细日志**

   ```bash
   export SSH_LOG_LEVEL=Debug
   ./ssh_core.exe connect --host example.com --username user --password pass
   ```

2. **检查网络连接**

   ```bash
   ping example.com
   telnet example.com 22
   ```

3. **验证 SSH 服务器状态**

   ```bash
   ssh -v user@example.com
   ```

4. **检查密钥文件**

   ```bash
   ssh-keygen -l -f ./id_rsa
   ```

5. **使用调试器**

   ```bash
   dotnet build -c Debug
   dotnet debug ./bin/Debug/net10.0/ssh_core.dll connect --host example.com --username user --password pass
   ```

### 7.3 错误处理

```csharp
try
{
    using var sshService = serviceProvider.GetRequiredService<ISshService>();
    var connection = await sshService.ConnectAsync(
        "example.com",
        22,
        "username",
        "password"
    );

    var result = await sshService.ExecuteCommandAsync(connection, "ls -la");
    Console.WriteLine(result);

    await sshService.CloseConnectionAsync(connection);
}
catch (SshConnectionException ex)
{
    Console.WriteLine($"连接错误: {ex.Message}");
}
catch (SshAuthenticationException ex)
{
    Console.WriteLine($"认证错误: {ex.Message}");
}
catch (SshCommandException ex)
{
    Console.WriteLine($"命令执行错误: {ex.Message}");
}
catch (SshException ex)
{
    Console.WriteLine($"SSH 错误: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"通用错误: {ex.Message}");
}
```

## 8. 总结

SSH 技能是一个功能强大、性能优异的 SSH 客户端工具，基于 .NET 10 和 AOT 编译技术，提供了全面的 SSH 功能，包括连接管理、命令执行、文件传输、端口转发、密钥管理等。通过集成 Scrutor，还提供了高级依赖注入和服务注册功能，使代码更加模块化和可维护。

无论是日常管理服务器、批量执行操作，还是构建复杂的自动化工具，SSH 技能都能满足您的需求。通过 AOT 编译，它具有出色的启动速度和运行时性能，适合在各种场景下使用。

希望本文档能帮助您快速上手和使用 SSH 技能。如果您有任何问题或建议，欢迎随时反馈。