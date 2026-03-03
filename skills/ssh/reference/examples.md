# SSH 技能使用示例

## 概述

本文件提供了 SSH 技能的详细使用示例，包括核心功能的使用方法和 Scrutor 各种用法的演示。

## 核心功能示例

### 1. SSH 连接管理

#### 基本连接示例

```csharp
var sshService = new SshService();
var connection = await sshService.ConnectAsync("example.com", 22, "user", "password");

if (connection.IsConnected)
{
    Console.WriteLine("连接成功！");
    await sshService.DisconnectAsync(connection);
}
```

#### 使用密钥认证

```csharp
var sshService = new SshService();
var connection = await sshService.ConnectWithKeyAsync(
    "example.com", 
    22, 
    "user", 
    "~/.ssh/id_rsa"
);

if (connection.IsConnected)
{
    Console.WriteLine("使用密钥认证连接成功！");
    await sshService.DisconnectAsync(connection);
}
```

### 2. 命令执行

#### 执行单个命令

```csharp
var sshService = new SshService();
var connection = await sshService.ConnectAsync("example.com", 22, "user", "password");

var result = await sshService.ExecuteCommandAsync(connection, "ls -la");
Console.WriteLine("命令执行结果:");
Console.WriteLine(result.Output);
Console.WriteLine($"退出码: {result.ExitStatus}");

await sshService.DisconnectAsync(connection);
```

#### 执行批量命令

```csharp
var sshService = new SshService();
var connection = await sshService.ConnectAsync("example.com", 22, "user", "password");

var commands = new List<string>
{
    "echo '开始执行批量命令'",
    "ls -la",
    "df -h",
    "echo '批量命令执行完成'"
};

var results = await sshService.ExecuteBatchCommandsAsync(connection, commands);

foreach (var (command, result) in results)
{
    Console.WriteLine($"命令: {command}");
    Console.WriteLine($"结果: {result.Output}");
    Console.WriteLine($"退出码: {result.ExitStatus}");
    Console.WriteLine();
}

await sshService.DisconnectAsync(connection);
```

### 3. 文件传输

#### 上传文件

```csharp
var sshService = new SshService();
var connection = await sshService.ConnectAsync("example.com", 22, "user", "password");

await sshService.UploadFileAsync(
    connection, 
    "localfile.txt", 
    "/remote/path/remotefile.txt",
    progress => Console.WriteLine($"上传进度: {progress}%"
);

Console.WriteLine("文件上传成功！");
await sshService.DisconnectAsync(connection);
```

#### 下载文件

```csharp
var sshService = new SshService();
var connection = await sshService.ConnectAsync("example.com", 22, "user", "password");

await sshService.DownloadFileAsync(
    connection, 
    "/remote/path/remotefile.txt", 
    "localfile.txt",
    progress => Console.WriteLine($"下载进度: {progress}%"
);

Console.WriteLine("文件下载成功！");
await sshService.DisconnectAsync(connection);
```

### 4. 代码生成

#### 生成 SSH 配置文件

```csharp
var generatorService = new CodeGeneratorService(new TemplateService());
var config = await generatorService.GenerateSshConfigAsync(
    "example.com", 
    22, 
    "user", 
    "~/.ssh/id_rsa"
);

File.WriteAllText("ssh_config.txt", config);
Console.WriteLine("SSH 配置文件生成成功！");
```

#### 生成 SSH 脚本

```csharp
var generatorService = new CodeGeneratorService(new TemplateService());
var parameters = new Dictionary<string, string>
{
    { "Host", "example.com" },
    { "Port", "22" },
    { "Username", "user" },
    { "Command", "ls -la" }
};

await generatorService.GenerateSshScriptAsync(
    "SshScript", 
    "ssh_script.sh", 
    parameters
);

Console.WriteLine("SSH 脚本生成成功！");
```

## Scrutor 用法示例

### 1. 按约定注册服务

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);
```

### 2. 按命名空间注册服务

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.InNamespaces("SSH.Generator.Services"))
    .AsImplementedInterfaces()
    .WithTransientLifetime()
);
```

### 3. 按属性注册服务

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.WithAttribute<ServiceAttribute>())
    .AsSelfWithInterfaces()
    .WithSingletonLifetime()
);
```

### 4. 注册装饰器

```csharp
// 基本装饰器
services.Decorate<ICodeGeneratorService, CodeGeneratorLoggingDecorator>();

// 带依赖的装饰器
services.Decorate<ITemplateService>((inner, provider) =>
    new TemplateServiceCachingDecorator(inner, provider.GetRequiredService<ICacheService>())
);
```

### 5. 多层装饰器

```csharp
// 先注册核心服务
services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();

// 然后添加多个装饰器
services.Decorate<ICodeGeneratorService, CodeGeneratorValidationDecorator>();
services.Decorate<ICodeGeneratorService, CodeGeneratorLoggingDecorator>();
services.Decorate<ICodeGeneratorService, CodeGeneratorCachingDecorator>();
```

### 6. 注册开放泛型

```csharp
services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
```

### 7. 自定义注册规则

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => 
        type.Name.StartsWith("Ssh") && type.Name.EndsWith("Manager")
    ))
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
);
```

## 高级用法示例

### 1. 使用依赖注入

```csharp
var services = new ServiceCollection();

// 注册 SSH 服务
services.AddScoped<ISshService, SshService>();
services.AddScoped<ISshSessionManager, SshSessionManager>();
services.AddScoped<ISshFileTransferService, SshFileTransferService>();
services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
services.AddScoped<ITemplateService, TemplateService>();
services.AddScoped<ICacheService, MemoryCacheService>();
services.AddScoped<ILoggerService, ConsoleLoggerService>();

// 使用 Scrutor 注册装饰器
services.Decorate<ICodeGeneratorService, CodeGeneratorLoggingDecorator>();
services.Decorate<ICodeGeneratorService, CodeGeneratorValidationDecorator>();
services.Decorate<ICodeGeneratorService, CodeGeneratorCachingDecorator>();

var serviceProvider = services.BuildServiceProvider();

// 解析服务
var sshService = serviceProvider.GetRequiredService<ISshService>();
var generatorService = serviceProvider.GetRequiredService<ICodeGeneratorService>();

// 使用服务
var connection = await sshService.ConnectAsync("example.com", 22, "user", "password");
// ... 执行操作 ...
await sshService.DisconnectAsync(connection);
```

### 2. 使用装饰器模式实现横切关注点

#### 日志装饰器

```csharp
public class CodeGeneratorLoggingDecorator : ICodeGeneratorService
{
    private readonly ICodeGeneratorService _inner;
    private readonly ILoggerService _logger;

    public CodeGeneratorLoggingDecorator(ICodeGeneratorService inner, ILoggerService logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<string> GenerateSshConfigAsync(string host, int port, string username, string privateKeyPath = null)
    {
        _logger.LogInformation("Generating SSH config for {Host}:{Port}", host, port);
        try
        {
            var result = await _inner.GenerateSshConfigAsync(host, port, username, privateKeyPath);
            _logger.LogInformation("SSH config generated successfully for {Host}:{Port}", host, port);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error generating SSH config for {Host}:{Port}: {Error}", host, port, ex.Message);
            throw;
        }
    }

    // 其他方法实现...
}
```

#### 验证装饰器

```csharp
public class CodeGeneratorValidationDecorator : ICodeGeneratorService
{
    private readonly ICodeGeneratorService _inner;

    public CodeGeneratorValidationDecorator(ICodeGeneratorService inner)
    {
        _inner = inner;
    }

    public async Task<string> GenerateSshConfigAsync(string host, int port, string username, string privateKeyPath = null)
    {
        ValidateHost(host);
        ValidatePort(port);
        ValidateUsername(username);
        ValidatePrivateKeyPath(privateKeyPath);
        return await _inner.GenerateSshConfigAsync(host, port, username, privateKeyPath);
    }

    private void ValidateHost(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new ArgumentException("Host cannot be empty.");
        }
    }

    // 其他验证方法...
}
```

#### 缓存装饰器

```csharp
public class CodeGeneratorCachingDecorator : ICodeGeneratorService
{
    private readonly ICodeGeneratorService _inner;
    private readonly ICacheService _cache;

    public CodeGeneratorCachingDecorator(ICodeGeneratorService inner, ICacheService cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<string> GenerateSshConfigAsync(string host, int port, string username, string privateKeyPath = null)
    {
        var cacheKey = $"ssh-config:{host}:{port}:{username}:{privateKeyPath}";
        var cachedResult = await _cache.GetAsync<string>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var result = await _inner.GenerateSshConfigAsync(host, port, username, privateKeyPath);
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));
        return result;
    }

    // 其他方法实现...
}
```

### 3. 批量操作示例

#### 批量执行命令

```csharp
var sshService = new SshService();
var batchService = new SshBatchService(sshService);

var targets = new List<SshTarget>
{
    new SshTarget { Host = "server1.com", Port = 22, Username = "user", Password = "pass" },
    new SshTarget { Host = "server2.com", Port = 22, Username = "user", Password = "pass" },
    new SshTarget { Host = "server3.com", Port = 22, Username = "user", Password = "pass" }
};

var command = "sudo systemctl status sshd";
var results = await batchService.ExecuteBatchCommandAsync(targets, command);

foreach (var (target, result) in results)
{
    Console.WriteLine($"Server: {target.Host}");
    Console.WriteLine($"Status: {(result.Success ? "Success" : "Failed")}");
    if (result.Success)
    {
        Console.WriteLine($"Output: {result.Output}");
    }
    else
    {
        Console.WriteLine($"Error: {result.Error}");
    }
    Console.WriteLine();
}
```

#### 批量上传文件

```csharp
var sshService = new SshService();
var batchService = new SshBatchService(sshService);

var targets = new List<SshTarget>
{
    new SshTarget { Host = "server1.com", Port = 22, Username = "user", Password = "pass" },
    new SshTarget { Host = "server2.com", Port = 22, Username = "user", Password = "pass" }
};

var results = await batchService.UploadBatchFileAsync(
    targets, 
    "local_config.txt", 
    "/etc/config.txt"
);

foreach (var (target, success, error) in results)
{
    Console.WriteLine($"Server: {target.Host}");
    Console.WriteLine($"Status: {(success ? "Success" : "Failed")}");
    if (!success)
    {
        Console.WriteLine($"Error: {error}");
    }
    Console.WriteLine();
}
```

### 4. 配置管理示例

#### 加载和保存配置

```csharp
var configService = new SshConfigService();

// 加载配置
var config = await configService.LoadConfigAsync("ssh_config.json");

// 修改配置
config.DefaultPort = 2222;
config.ConnectTimeout = TimeSpan.FromSeconds(30);
config.Servers.Add(new SshServerConfig
{
    Name = "production",
    Host = "prod.example.com",
    Port = 22,
    Username = "admin",
    PrivateKeyPath = "~/.ssh/prod_id_rsa"
});

// 保存配置
await configService.SaveConfigAsync(config, "ssh_config.json");
Console.WriteLine("配置保存成功！");
```

#### 生成配置文件

```csharp
var configService = new SshConfigService();
var generatorService = new CodeGeneratorService(new TemplateService());

var config = new SshConfig
{
    DefaultPort = 22,
    ConnectTimeout = TimeSpan.FromSeconds(15),
    Servers = new List<SshServerConfig>
    {
        new SshServerConfig
        {
            Name = "dev",
            Host = "dev.example.com",
            Port = 22,
            Username = "devuser",
            PrivateKeyPath = "~/.ssh/dev_id_rsa"
        },
        new SshServerConfig
        {
            Name = "staging",
            Host = "staging.example.com",
            Port = 22,
            Username = "staginguser",
            PrivateKeyPath = "~/.ssh/staging_id_rsa"
        }
    }
};

// 生成 OpenSSH 配置文件
var opensshConfig = await generatorService.GenerateSshConfigAsync(
    "dev.example.com", 
    22, 
    "devuser", 
    "~/.ssh/dev_id_rsa"
);

File.WriteAllText("~/.ssh/config", opensshConfig);
Console.WriteLine("OpenSSH 配置文件生成成功！");
```

## 性能优化示例

### 1. 使用连接池

```csharp
var sessionManager = new SshSessionManager(new SshService());

// 配置连接池
sessionManager.ConfigurePool(5, TimeSpan.FromMinutes(10));

// 从连接池获取连接
var session1 = await sessionManager.GetSessionAsync("example.com", 22, "user", "password");
var session2 = await sessionManager.GetSessionAsync("example.com", 22, "user", "password");

// 使用连接
await session1.ExecuteCommandAsync("ls -la");
await session2.ExecuteCommandAsync("df -h");

// 归还连接到池
sessionManager.ReturnSession(session1);
sessionManager.ReturnSession(session2);

// 关闭连接池
sessionManager.ClosePool();
```

### 2. 使用内存缓存

```csharp
var cacheService = new MemoryCacheService();
var generatorService = new CodeGeneratorService(new TemplateService());

// 缓存装饰器
var cachedGenerator = new CodeGeneratorCachingDecorator(generatorService, cacheService);

// 第一次生成（会缓存）
var config1 = await cachedGenerator.GenerateSshConfigAsync(
    "example.com", 
    22, 
    "user", 
    "~/.ssh/id_rsa"
);

// 第二次生成（会从缓存获取）
var config2 = await cachedGenerator.GenerateSshConfigAsync(
    "example.com", 
    22, 
    "user", 
    "~/.ssh/id_rsa"
);

Console.WriteLine("两次生成的结果是否相同: " + (config1 == config2));
```

## 故障排除示例

### 1. 连接失败处理

```csharp
try
{
    var sshService = new SshService();
    var connection = await sshService.ConnectAsync(
        "example.com", 
        22, 
        "user", 
        "password",
        TimeSpan.FromSeconds(10)
    );

    Console.WriteLine("连接成功！");
    await sshService.DisconnectAsync(connection);
}
catch (SshConnectionException ex)
{
    Console.WriteLine($"连接失败: {ex.Message}");
    Console.WriteLine("可能的原因:");
    Console.WriteLine("1. 网络连接问题");
    Console.WriteLine("2. SSH 服务器未运行");
    Console.WriteLine("3. 端口错误");
    Console.WriteLine("4. 认证信息错误");
}
catch (TimeoutException ex)
{
    Console.WriteLine($"连接超时: {ex.Message}");
    Console.WriteLine("请检查网络连接和服务器状态");
}
```

### 2. 命令执行失败处理

```csharp
try
{
    var sshService = new SshService();
    var connection = await sshService.ConnectAsync("example.com", 22, "user", "password");

    var result = await sshService.ExecuteCommandAsync(connection, "sudo ls -la");
    
    if (result.ExitStatus == 0)
    {
        Console.WriteLine("命令执行成功:");
        Console.WriteLine(result.Output);
    }
    else
    {
        Console.WriteLine($"命令执行失败，退出码: {result.ExitStatus}");
        Console.WriteLine($"错误信息: {result.Error}");
    }

    await sshService.DisconnectAsync(connection);
}
catch (SshCommandException ex)
{
    Console.WriteLine($"命令执行异常: {ex.Message}");
}
```

## 总结

本文件提供了 SSH 技能的详细使用示例，涵盖了核心功能的使用方法和 Scrutor 各种用法的演示。通过这些示例，您可以快速上手 SSH 技能的使用，并根据自己的需求进行扩展和定制。

如需更多帮助，请参考 SSH 技能的其他文档或联系开发团队。