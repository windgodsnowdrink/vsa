# Oqtane 技能参考文档

## 1. 技能概述

Oqtane 技能是一个基于 .NET 10 的 AOT 编译技能，提供与 Oqtane 框架的无缝集成和扩展功能。Oqtane 是一个现代化的 .NET CMS（内容管理系统）和应用框架，专为构建企业级 Web 应用和网站而设计。

本技能旨在简化 Oqtane 应用的开发、部署和管理流程，提供一系列工具和功能，帮助开发者更高效地使用 Oqtane 框架。

## 2. 核心组件

### 2.1 IOqtaneService

`IOqtaneService` 是 Oqtane 技能的核心服务接口，提供了与 Oqtane 框架交互的主要方法。

**主要方法**：

- `InstallModuleAsync(string moduleName, string version, CancellationToken cancellationToken = default)` - 安装 Oqtane 模块
- `UpdateModuleAsync(string moduleName, string version, CancellationToken cancellationToken = default)` - 更新 Oqtane 模块
- `UninstallModuleAsync(string moduleName, CancellationToken cancellationToken = default)` - 卸载 Oqtane 模块
- `GetModulesAsync(CancellationToken cancellationToken = default)` - 获取所有已安装的模块
- `InstallThemeAsync(string themeName, string version, CancellationToken cancellationToken = default)` - 安装 Oqtane 主题
- `SetDefaultThemeAsync(string themeName, CancellationToken cancellationToken = default)` - 设置默认主题
- `GetThemesAsync(CancellationToken cancellationToken = default)` - 获取所有已安装的主题
- `CreateUserAsync(string username, string email, string password, CancellationToken cancellationToken = default)` - 创建用户
- `UpdateUserAsync(string username, OqtaneUser user, CancellationToken cancellationToken = default)` - 更新用户信息
- `DeleteUserAsync(string username, CancellationToken cancellationToken = default)` - 删除用户
- `AddUserToRoleAsync(string username, string roleName, CancellationToken cancellationToken = default)` - 将用户添加到角色
- `RemoveUserFromRoleAsync(string username, string roleName, CancellationToken cancellationToken = default)` - 从角色中移除用户
- `GetUsersAsync(CancellationToken cancellationToken = default)` - 获取所有用户
- `GetConfigurationAsync(CancellationToken cancellationToken = default)` - 获取应用配置
- `UpdateConfigurationAsync(OqtaneConfiguration configuration, CancellationToken cancellationToken = default)` - 更新应用配置
- `PackageApplicationAsync(string outputPath, CancellationToken cancellationToken = default)` - 打包应用
- `DeployApplicationAsync(string serverUrl, string username, string password, CancellationToken cancellationToken = default)` - 部署应用

### 2.2 IOqtaneModuleService

`IOqtaneModuleService` 负责 Oqtane 模块的管理，包括安装、更新和卸载模块。

### 2.3 IOqtaneThemeService

`IOqtaneThemeService` 负责 Oqtane 主题的管理，包括安装主题和设置默认主题。

### 2.4 IOqtaneUserService

`IOqtaneUserService` 负责 Oqtane 用户的管理，包括创建、更新和删除用户，以及用户角色管理。

### 2.5 IOqtaneConfigurationService

`IOqtaneConfigurationService` 负责 Oqtane 应用配置的管理，包括获取和更新配置。

### 2.6 IOqtaneDeploymentService

`IOqtaneDeploymentService` 负责 Oqtane 应用的部署，包括打包和部署应用。

## 3. 配置选项

### 3.1 OqtaneOptions

`OqtaneOptions` 是 Oqtane 技能的主要配置选项类，包含以下属性：

| 属性名 | 类型 | 默认值 | 描述 |
|--------|------|--------|------|
| Enabled | bool | true | 是否启用 Oqtane 技能 |
| SiteName | string | "Oqtane Site" | 站点名称 |
| DefaultLanguage | string | "en-US" | 默认语言 |
| EnableModules | bool | true | 是否启用模块管理 |
| EnableThemes | bool | true | 是否启用主题管理 |
| EnableUsers | bool | true | 是否启用用户管理 |
| EnableConfiguration | bool | true | 是否启用配置管理 |
| EnableDeployment | bool | true | 是否启用部署工具 |
| EnableParallelProcessing | bool | true | 是否启用并行处理 |
| MaxDegreeOfParallelism | int | Environment.ProcessorCount | 最大并行度 |

### 3.2 配置文件结构

Oqtane 技能的配置文件 `oqtane_extensions.setting.json` 包含以下主要部分：

- `oqtane.options` - 基本配置选项
- `oqtane.module` - 模块管理配置
- `oqtane.theme` - 主题管理配置
- `oqtane.user` - 用户管理配置
- `oqtane.configuration` - 配置管理配置
- `oqtane.deployment` - 部署工具配置
- `oqtane.aot` - AOT 编译配置
- `oqtane.logging` - 日志配置
- `oqtane.performance` - 性能配置
- `oqtane.security` - 安全配置
- `oqtane.database` - 数据库配置

## 4. 安装和设置

### 4.1 环境要求

- .NET 10 SDK 或更高版本
- 支持的操作系统：Windows、Linux、macOS
- Oqtane 4.0 或更高版本（可选）

### 4.2 安装方法

1. **克隆技能仓库**：
   ```bash
   git clone <repository-url>
   cd skills/oqtane
   ```

2. **安装依赖**：
   ```bash
   dotnet restore
   ```

3. **配置技能**：
   编辑 `scripts/oqtane_extensions.setting.json` 文件，根据需要修改配置。

4. **运行技能**：
   ```bash
   dotnet run --project scripts/oqtane_extensions.cs
   ```

### 4.3 依赖注入

在 .NET 应用中，你可以使用依赖注入来注册和使用 Oqtane 服务：

```csharp
// 注册 Oqtane 服务
services.AddOqtaneServices(options =>
{
    options.Enabled = true;
    options.SiteName = "我的站点";
    options.DefaultLanguage = "zh-CN";
    options.EnableModules = true;
    options.EnableThemes = true;
    options.EnableUsers = true;
    options.EnableConfiguration = true;
    options.EnableDeployment = true;
    options.EnableParallelProcessing = true;
    options.MaxDegreeOfParallelism = Environment.ProcessorCount;
});

// 使用 Oqtane 服务
var oqtaneService = serviceProvider.GetRequiredService<IOqtaneService>();
```

## 5. 使用指南

### 5.1 模块管理

**安装模块**：
```csharp
var module = await oqtaneService.InstallModuleAsync("Blog", "1.0.0");
Console.WriteLine($"模块安装成功: {module.Name} v{module.Version}");
```

**更新模块**：
```csharp
var module = await oqtaneService.UpdateModuleAsync("Blog", "1.1.0");
Console.WriteLine($"模块更新成功: {module.Name} v{module.Version}");
```

**卸载模块**：
```csharp
var result = await oqtaneService.UninstallModuleAsync("Blog");
Console.WriteLine($"模块卸载: {(result ? "成功" : "失败")}");
```

**获取模块列表**：
```csharp
var modules = await oqtaneService.GetModulesAsync();
foreach (var module in modules)
{
    Console.WriteLine($"- {module.Name} v{module.Version} {(module.IsInstalled ? "[已安装]" : "")}");
}
```

### 5.2 主题管理

**安装主题**：
```csharp
var theme = await oqtaneService.InstallThemeAsync("Modern", "1.0.0");
Console.WriteLine($"主题安装成功: {theme.Name} v{theme.Version}");
```

**设置默认主题**：
```csharp
var result = await oqtaneService.SetDefaultThemeAsync("Modern");
Console.WriteLine($"设置默认主题: {(result ? "成功" : "失败")}");
```

**获取主题列表**：
```csharp
var themes = await oqtaneService.GetThemesAsync();
foreach (var theme in themes)
{
    Console.WriteLine($"- {theme.Name} v{theme.Version} {(theme.IsDefault ? "[默认]" : "")}");
}
```

### 5.3 用户管理

**创建用户**：
```csharp
var user = await oqtaneService.CreateUserAsync("user1", "user1@example.com", "Password123!");
Console.WriteLine($"用户创建成功: {user.Username}");
```

**更新用户**：
```csharp
var user = new OqtaneUser
{
    Username = "user1",
    Email = "user1@example.com",
    FirstName = "用户",
    LastName = "一",
    IsActive = true
};
var updatedUser = await oqtaneService.UpdateUserAsync("user1", user);
Console.WriteLine($"用户更新成功: {updatedUser.Username}");
```

**删除用户**：
```csharp
var result = await oqtaneService.DeleteUserAsync("user1");
Console.WriteLine($"用户删除: {(result ? "成功" : "失败")}");
```

**添加用户到角色**：
```csharp
var result = await oqtaneService.AddUserToRoleAsync("user1", "Administrators");
Console.WriteLine($"添加用户到角色: {(result ? "成功" : "失败")}");
```

**获取用户列表**：
```csharp
var users = await oqtaneService.GetUsersAsync();
foreach (var user in users)
{
    Console.WriteLine($"- {user.Username} ({user.Email}) - 角色: {string.Join(", ", user.Roles)}");
}
```

### 5.4 配置管理

**获取配置**：
```csharp
var config = await oqtaneService.GetConfigurationAsync();
Console.WriteLine($"站点名称: {config.SiteName}");
Console.WriteLine($"默认语言: {config.DefaultLanguage}");
Console.WriteLine($"管理员邮箱: {config.AdminEmail}");
```

**更新配置**：
```csharp
var config = new OqtaneConfiguration
{
    SiteName = "我的新站点",
    DefaultLanguage = "zh-CN",
    ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=Oqtane;Trusted_Connection=True;",
    EnableSsl = true,
    Port = 443,
    AdminEmail = "admin@example.com",
    EnableLogging = true,
    LogLevel = "Information"
};
var result = await oqtaneService.UpdateConfigurationAsync(config);
Console.WriteLine($"配置更新: {(result ? "成功" : "失败")}");
```

### 5.5 部署工具

**打包应用**：
```csharp
var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "output");
var result = await oqtaneService.PackageApplicationAsync(outputPath);
Console.WriteLine($"应用打包: {(result ? "成功" : "失败")}");
```

**部署应用**：
```csharp
var result = await oqtaneService.DeployApplicationAsync("https://myserver.com", "username", "password");
Console.WriteLine($"应用部署: {(result ? "成功" : "失败")}");
```

## 6. AOT 编译

Oqtane 技能支持 .NET 10 AOT 编译，可以显著提升应用的启动速度和运行性能。

### 6.1 编译选项

在 `oqtane_extensions.run.json` 文件中，你可以配置以下 AOT 编译选项：

- `publishAot` - 是否启用 AOT 编译（默认: true）
- `readyToRun` - 是否启用 ReadyToRun 编译（默认: true）
- `tieredCompilation` - 是否启用分层编译（默认: true）
- `optimizeForSize` - 是否优化大小（默认: false）
- `trimMode` - 剪裁模式（默认: "partial"）

### 6.2 支持的运行时

Oqtane 技能支持以下运行时：

- win-x64
- linux-x64
- osx-x64
- win-arm64
- linux-arm64
- osx-arm64

### 6.3 编译命令

要使用 AOT 编译 Oqtane 技能，你可以使用以下命令：

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true
```

## 7. 性能优化

### 7.1 并行处理

Oqtane 技能支持并行处理，可以提高处理多个任务时的性能：

```csharp
// 启用并行处理
options.EnableParallelProcessing = true;
// 设置最大并行度
options.MaxDegreeOfParallelism = Environment.ProcessorCount;
```

### 7.2 缓存

Oqtane 技能使用缓存来提高性能，你可以在配置文件中设置缓存持续时间：

```json
"performance": {
  "enableCaching": true,
  "cacheDuration": "00:05:00"
}
```

### 7.3 压缩

启用压缩可以减少网络传输大小：

```json
"performance": {
  "enableCompression": true
}
```

## 8. 安全配置

### 8.1 SSL 配置

你可以在配置文件中启用 SSL：

```json
"security": {
  "enableSsl": true,
  "requireSsl": true
}
```

### 8.2 CORS 配置

配置 CORS 以允许跨域请求：

```json
"security": {
  "enableCors": true,
  "allowedOrigins": ["*"]
}
```

### 8.3 密码策略

配置用户密码策略：

```json
"user": {
  "passwordPolicy": {
    "requiredLength": 8,
    "requireDigit": true,
    "requireLowercase": true,
    "requireUppercase": true,
    "requireNonAlphanumeric": false
  }
}
```

## 9. 数据库配置

### 9.1 连接字符串

配置数据库连接字符串：

```json
"database": {
  "provider": "SqlServer",
  "connectionString": "Server=(localdb)\\mssqllocaldb;Database=Oqtane;Trusted_Connection=True;"
}
```

### 9.2 迁移配置

配置数据库迁移选项：

```json
"database": {
  "enableMigration": true,
  "autoMigrate": false,
  "backupBeforeMigrate": true
}
```

## 10. 日志配置

### 10.1 日志级别

配置日志级别：

```json
"logging": {
  "enabled": true,
  "level": "Information",
  "includeScopes": true
}
```

### 10.2 文件日志

配置文件日志：

```json
"logging": {
  "logFile": "oqtane.log",
  "maxFileSize": 10485760,
  "maxRetainedFiles": 5
}
```

## 11. 故障排除

### 11.1 常见问题

**问题**：模块安装失败
**解决方案**：检查网络连接，确保模块名称和版本正确，查看日志文件获取详细错误信息。

**问题**：主题切换失败
**解决方案**：确保主题已正确安装，检查主题文件是否完整，查看日志文件获取详细错误信息。

**问题**：用户创建失败
**解决方案**：检查用户名是否已存在，确保密码符合密码策略要求，查看日志文件获取详细错误信息。

**问题**：配置更新失败
**解决方案**：确保配置参数有效，检查数据库连接，查看日志文件获取详细错误信息。

**问题**：部署失败
**解决方案**：检查服务器连接信息，确保服务器有足够的空间和权限，查看日志文件获取详细错误信息。

### 11.2 日志文件

Oqtane 技能的日志文件默认位于应用根目录的 `oqtane.log` 文件中，你可以在配置文件中修改日志文件路径：

```json
"logging": {
  "logFile": "oqtane.log"
}
```

## 12. 示例代码

### 12.1 基本使用

```csharp
// 构建服务容器
var services = new ServiceCollection();

// 配置日志
services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

// 注册 Oqtane 服务
services.AddOqtaneServices(options =>
{
    options.Enabled = true;
    options.SiteName = "我的站点";
    options.DefaultLanguage = "zh-CN";
});

// 构建服务提供者
using var serviceProvider = services.BuildServiceProvider();

// 获取服务
var oqtaneService = serviceProvider.GetRequiredService<IOqtaneService>();

// 使用服务
var modules = await oqtaneService.GetModulesAsync();
Console.WriteLine($"已安装的模块数量: {modules.Count()}");
```

### 12.2 高级配置

```csharp
// 注册 Oqtane 服务并配置所有选项
services.AddOqtaneServices(options =>
{
    options.Enabled = true;
    options.SiteName = "高级配置示例";
    options.DefaultLanguage = "zh-CN";
    options.EnableModules = true;
    options.EnableThemes = true;
    options.EnableUsers = true;
    options.EnableConfiguration = true;
    options.EnableDeployment = true;
    options.EnableParallelProcessing = true;
    options.MaxDegreeOfParallelism = Environment.ProcessorCount;
});
```

## 13. 结论

Oqtane 技能是一个功能强大的工具，可以帮助开发者更高效地使用 Oqtane 框架。通过本参考文档，你应该已经了解了 Oqtane 技能的核心组件、配置选项和使用方法。

如果你有任何问题或建议，请参考 Oqtane 官方文档或联系 Oqtane 社区。

## 14. 参考资料

- [Oqtane 官方网站](https://oqtane.org)
- [Oqtane GitHub 仓库](https://github.com/oqtane/oqtane.framework)
- [Oqtane 文档](https://docs.oqtane.org)
- [.NET 10 文档](https://learn.microsoft.com/dotnet)
- [AOT 编译文档](https://learn.microsoft.com/dotnet/core/deploying/aot)
