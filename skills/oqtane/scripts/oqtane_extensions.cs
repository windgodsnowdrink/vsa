#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Text.Json@10.0.0
#:package System.Net.Http.Json@10.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// 配置选项
public class OqtaneOptions
{
    public bool Enabled { get; set; } = true;
    public string SiteName { get; set; } = "Oqtane Site";
    public string DefaultLanguage { get; set; } = "en-US";
    public bool EnableModules { get; set; } = true;
    public bool EnableThemes { get; set; } = true;
    public bool EnableUsers { get; set; } = true;
    public bool EnableConfiguration { get; set; } = true;
    public bool EnableDeployment { get; set; } = true;
    public bool EnableParallelProcessing { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
}

// Oqtane 配置
public class OqtaneConfiguration
{
    public string SiteName { get; set; } = "Oqtane Site";
    public string DefaultLanguage { get; set; } = "en-US";
    public string ConnectionString { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = false;
    public int Port { get; set; } = 443;
    public string AdminEmail { get; set; } = "admin@example.com";
    public bool EnableLogging { get; set; } = true;
    public string LogLevel { get; set; } = "Information";
}

// Oqtane 模块信息
public class OqtaneModule
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public bool IsInstalled { get; set; } = false;
    public DateTime? InstallDate { get; set; }
    public DateTime? LastUpdateDate { get; set; }
}

// Oqtane 主题信息
public class OqtaneTheme
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public bool IsInstalled { get; set; } = false;
    public bool IsDefault { get; set; } = false;
}

// Oqtane 用户信息
public class OqtaneUser
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? CreateDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}

// Oqtane 服务接口
public interface IOqtaneService
{
    // 模块管理
    Task<OqtaneModule> InstallModuleAsync(string moduleName, string version, CancellationToken cancellationToken = default);
    Task<OqtaneModule> UpdateModuleAsync(string moduleName, string version, CancellationToken cancellationToken = default);
    Task<bool> UninstallModuleAsync(string moduleName, CancellationToken cancellationToken = default);
    Task<IEnumerable<OqtaneModule>> GetModulesAsync(CancellationToken cancellationToken = default);
    
    // 主题管理
    Task<OqtaneTheme> InstallThemeAsync(string themeName, string version, CancellationToken cancellationToken = default);
    Task<bool> SetDefaultThemeAsync(string themeName, CancellationToken cancellationToken = default);
    Task<IEnumerable<OqtaneTheme>> GetThemesAsync(CancellationToken cancellationToken = default);
    
    // 用户管理
    Task<OqtaneUser> CreateUserAsync(string username, string email, string password, CancellationToken cancellationToken = default);
    Task<OqtaneUser> UpdateUserAsync(string username, OqtaneUser user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> AddUserToRoleAsync(string username, string roleName, CancellationToken cancellationToken = default);
    Task<bool> RemoveUserFromRoleAsync(string username, string roleName, CancellationToken cancellationToken = default);
    Task<IEnumerable<OqtaneUser>> GetUsersAsync(CancellationToken cancellationToken = default);
    
    // 配置管理
    Task<OqtaneConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateConfigurationAsync(OqtaneConfiguration configuration, CancellationToken cancellationToken = default);
    
    // 部署工具
    Task<bool> PackageApplicationAsync(string outputPath, CancellationToken cancellationToken = default);
    Task<bool> DeployApplicationAsync(string serverUrl, string username, string password, CancellationToken cancellationToken = default);
}

// Oqtane 模块管理服务接口
public interface IOqtaneModuleService
{
    Task<OqtaneModule> InstallModuleAsync(string moduleName, string version, CancellationToken cancellationToken = default);
    Task<OqtaneModule> UpdateModuleAsync(string moduleName, string version, CancellationToken cancellationToken = default);
    Task<bool> UninstallModuleAsync(string moduleName, CancellationToken cancellationToken = default);
    Task<IEnumerable<OqtaneModule>> GetModulesAsync(CancellationToken cancellationToken = default);
}

// Oqtane 主题管理服务接口
public interface IOqtaneThemeService
{
    Task<OqtaneTheme> InstallThemeAsync(string themeName, string version, CancellationToken cancellationToken = default);
    Task<bool> SetDefaultThemeAsync(string themeName, CancellationToken cancellationToken = default);
    Task<IEnumerable<OqtaneTheme>> GetThemesAsync(CancellationToken cancellationToken = default);
}

// Oqtane 用户管理服务接口
public interface IOqtaneUserService
{
    Task<OqtaneUser> CreateUserAsync(string username, string email, string password, CancellationToken cancellationToken = default);
    Task<OqtaneUser> UpdateUserAsync(string username, OqtaneUser user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> AddUserToRoleAsync(string username, string roleName, CancellationToken cancellationToken = default);
    Task<bool> RemoveUserFromRoleAsync(string username, string roleName, CancellationToken cancellationToken = default);
    Task<IEnumerable<OqtaneUser>> GetUsersAsync(CancellationToken cancellationToken = default);
}

// Oqtane 配置管理服务接口
public interface IOqtaneConfigurationService
{
    Task<OqtaneConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateConfigurationAsync(OqtaneConfiguration configuration, CancellationToken cancellationToken = default);
}

// Oqtane 部署服务接口
public interface IOqtaneDeploymentService
{
    Task<bool> PackageApplicationAsync(string outputPath, CancellationToken cancellationToken = default);
    Task<bool> DeployApplicationAsync(string serverUrl, string username, string password, CancellationToken cancellationToken = default);
}

// Oqtane 模块管理服务实现
public class OqtaneModuleService : IOqtaneModuleService
{
    private readonly ILogger<OqtaneModuleService> _logger;
    private readonly List<OqtaneModule> _modules = new List<OqtaneModule>();

    public OqtaneModuleService(ILogger<OqtaneModuleService> logger)
    {
        _logger = logger;
        // 初始化示例模块
        _modules.Add(new OqtaneModule
        {
            Name = "Dashboard",
            Version = "1.0.0",
            Description = "Dashboard module",
            Author = "Oqtane Community",
            Website = "https://oqtane.org",
            IsInstalled = true,
            InstallDate = DateTime.Now,
            LastUpdateDate = DateTime.Now
        });
    }

    public async Task<OqtaneModule> InstallModuleAsync(string moduleName, string version, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Installing module: {ModuleName} version {Version}", moduleName, version);
        await Task.Delay(1000, cancellationToken); // 模拟安装过程

        var module = new OqtaneModule
        {
            Name = moduleName,
            Version = version,
            Description = $"{moduleName} module",
            Author = "Oqtane Community",
            Website = "https://oqtane.org",
            IsInstalled = true,
            InstallDate = DateTime.Now,
            LastUpdateDate = DateTime.Now
        };

        _modules.Add(module);
        _logger.LogInformation("Module installed: {ModuleName}", moduleName);
        return module;
    }

    public async Task<OqtaneModule> UpdateModuleAsync(string moduleName, string version, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating module: {ModuleName} to version {Version}", moduleName, version);
        await Task.Delay(800, cancellationToken); // 模拟更新过程

        var module = _modules.FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
        if (module != null)
        {
            module.Version = version;
            module.LastUpdateDate = DateTime.Now;
            _logger.LogInformation("Module updated: {ModuleName}", moduleName);
        }

        return module;
    }

    public async Task<bool> UninstallModuleAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Uninstalling module: {ModuleName}", moduleName);
        await Task.Delay(600, cancellationToken); // 模拟卸载过程

        var module = _modules.FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
        if (module != null)
        {
            _modules.Remove(module);
            _logger.LogInformation("Module uninstalled: {ModuleName}", moduleName);
            return true;
        }

        return false;
    }

    public async Task<IEnumerable<OqtaneModule>> GetModulesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting modules");
        await Task.Delay(200, cancellationToken); // 模拟获取过程
        return _modules;
    }
}

// Oqtane 主题管理服务实现
public class OqtaneThemeService : IOqtaneThemeService
{
    private readonly ILogger<OqtaneThemeService> _logger;
    private readonly List<OqtaneTheme> _themes = new List<OqtaneTheme>();

    public OqtaneThemeService(ILogger<OqtaneThemeService> logger)
    {
        _logger = logger;
        // 初始化示例主题
        _themes.Add(new OqtaneTheme
        {
            Name = "Default",
            Version = "1.0.0",
            Description = "Default theme",
            Author = "Oqtane Community",
            Website = "https://oqtane.org",
            IsInstalled = true,
            IsDefault = true
        });
    }

    public async Task<OqtaneTheme> InstallThemeAsync(string themeName, string version, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Installing theme: {ThemeName} version {Version}", themeName, version);
        await Task.Delay(800, cancellationToken); // 模拟安装过程

        var theme = new OqtaneTheme
        {
            Name = themeName,
            Version = version,
            Description = $"{themeName} theme",
            Author = "Oqtane Community",
            Website = "https://oqtane.org",
            IsInstalled = true,
            IsDefault = false
        };

        _themes.Add(theme);
        _logger.LogInformation("Theme installed: {ThemeName}", themeName);
        return theme;
    }

    public async Task<bool> SetDefaultThemeAsync(string themeName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting default theme: {ThemeName}", themeName);
        await Task.Delay(400, cancellationToken); // 模拟设置过程

        var theme = _themes.FirstOrDefault(t => t.Name.Equals(themeName, StringComparison.OrdinalIgnoreCase));
        if (theme != null)
        {
            // 重置所有主题的默认状态
            foreach (var t in _themes)
            {
                t.IsDefault = false;
            }
            // 设置当前主题为默认
            theme.IsDefault = true;
            _logger.LogInformation("Default theme set: {ThemeName}", themeName);
            return true;
        }

        return false;
    }

    public async Task<IEnumerable<OqtaneTheme>> GetThemesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting themes");
        await Task.Delay(200, cancellationToken); // 模拟获取过程
        return _themes;
    }
}

// Oqtane 用户管理服务实现
public class OqtaneUserService : IOqtaneUserService
{
    private readonly ILogger<OqtaneUserService> _logger;
    private readonly List<OqtaneUser> _users = new List<OqtaneUser>();

    public OqtaneUserService(ILogger<OqtaneUserService> logger)
    {
        _logger = logger;
        // 初始化示例用户
        _users.Add(new OqtaneUser
        {
            Username = "admin",
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User",
            IsActive = true,
            CreateDate = DateTime.Now,
            LastLoginDate = DateTime.Now,
            Roles = new List<string> { "Administrators" }
        });
    }

    public async Task<OqtaneUser> CreateUserAsync(string username, string email, string password, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user: {Username}", username);
        await Task.Delay(600, cancellationToken); // 模拟创建过程

        var user = new OqtaneUser
        {
            Username = username,
            Email = email,
            FirstName = string.Empty,
            LastName = string.Empty,
            IsActive = true,
            CreateDate = DateTime.Now,
            LastLoginDate = null,
            Roles = new List<string> { "Users" }
        };

        _users.Add(user);
        _logger.LogInformation("User created: {Username}", username);
        return user;
    }

    public async Task<OqtaneUser> UpdateUserAsync(string username, OqtaneUser user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user: {Username}", username);
        await Task.Delay(500, cancellationToken); // 模拟更新过程

        var existingUser = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (existingUser != null)
        {
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.IsActive = user.IsActive;
            _logger.LogInformation("User updated: {Username}", username);
        }

        return existingUser;
    }

    public async Task<bool> DeleteUserAsync(string username, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting user: {Username}", username);
        await Task.Delay(400, cancellationToken); // 模拟删除过程

        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user != null)
        {
            _users.Remove(user);
            _logger.LogInformation("User deleted: {Username}", username);
            return true;
        }

        return false;
    }

    public async Task<bool> AddUserToRoleAsync(string username, string roleName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding user {Username} to role {RoleName}", username, roleName);
        await Task.Delay(300, cancellationToken); // 模拟添加过程

        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user != null && !user.Roles.Contains(roleName))
        {
            user.Roles.Add(roleName);
            _logger.LogInformation("User {Username} added to role {RoleName}", username, roleName);
            return true;
        }

        return false;
    }

    public async Task<bool> RemoveUserFromRoleAsync(string username, string roleName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing user {Username} from role {RoleName}", username, roleName);
        await Task.Delay(300, cancellationToken); // 模拟移除过程

        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user != null && user.Roles.Contains(roleName))
        {
            user.Roles.Remove(roleName);
            _logger.LogInformation("User {Username} removed from role {RoleName}", username, roleName);
            return true;
        }

        return false;
    }

    public async Task<IEnumerable<OqtaneUser>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting users");
        await Task.Delay(200, cancellationToken); // 模拟获取过程
        return _users;
    }
}

// Oqtane 配置管理服务实现
public class OqtaneConfigurationService : IOqtaneConfigurationService
{
    private readonly ILogger<OqtaneConfigurationService> _logger;
    private OqtaneConfiguration _configuration;

    public OqtaneConfigurationService(ILogger<OqtaneConfigurationService> logger)
    {
        _logger = logger;
        // 初始化默认配置
        _configuration = new OqtaneConfiguration
        {
            SiteName = "Oqtane Site",
            DefaultLanguage = "en-US",
            ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=Oqtane;Trusted_Connection=True;",
            EnableSsl = false,
            Port = 443,
            AdminEmail = "admin@example.com",
            EnableLogging = true,
            LogLevel = "Information"
        };
    }

    public async Task<OqtaneConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting configuration");
        await Task.Delay(200, cancellationToken); // 模拟获取过程
        return _configuration;
    }

    public async Task<bool> UpdateConfigurationAsync(OqtaneConfiguration configuration, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating configuration");
        await Task.Delay(400, cancellationToken); // 模拟更新过程

        _configuration = configuration;
        _logger.LogInformation("Configuration updated");
        return true;
    }
}

// Oqtane 部署服务实现
public class OqtaneDeploymentService : IOqtaneDeploymentService
{
    private readonly ILogger<OqtaneDeploymentService> _logger;

    public OqtaneDeploymentService(ILogger<OqtaneDeploymentService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> PackageApplicationAsync(string outputPath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Packaging application to: {OutputPath}", outputPath);
        await Task.Delay(2000, cancellationToken); // 模拟打包过程

        // 确保输出目录存在
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 模拟创建包文件
        await File.WriteAllTextAsync(Path.Combine(outputPath, "oqtane-package.zip"), "Package content", cancellationToken);
        _logger.LogInformation("Application packaged successfully");
        return true;
    }

    public async Task<bool> DeployApplicationAsync(string serverUrl, string username, string password, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deploying application to: {ServerUrl}", serverUrl);
        await Task.Delay(3000, cancellationToken); // 模拟部署过程

        _logger.LogInformation("Application deployed successfully to {ServerUrl}", serverUrl);
        return true;
    }
}

// Oqtane 服务实现
public class OqtaneService : IOqtaneService
{
    private readonly IOqtaneModuleService _moduleService;
    private readonly IOqtaneThemeService _themeService;
    private readonly IOqtaneUserService _userService;
    private readonly IOqtaneConfigurationService _configurationService;
    private readonly IOqtaneDeploymentService _deploymentService;
    private readonly ILogger<OqtaneService> _logger;

    public OqtaneService(
        IOqtaneModuleService moduleService,
        IOqtaneThemeService themeService,
        IOqtaneUserService userService,
        IOqtaneConfigurationService configurationService,
        IOqtaneDeploymentService deploymentService,
        ILogger<OqtaneService> logger)
    {
        _moduleService = moduleService;
        _themeService = themeService;
        _userService = userService;
        _configurationService = configurationService;
        _deploymentService = deploymentService;
        _logger = logger;
    }

    // 模块管理方法
    public Task<OqtaneModule> InstallModuleAsync(string moduleName, string version, CancellationToken cancellationToken = default)
    {
        return _moduleService.InstallModuleAsync(moduleName, version, cancellationToken);
    }

    public Task<OqtaneModule> UpdateModuleAsync(string moduleName, string version, CancellationToken cancellationToken = default)
    {
        return _moduleService.UpdateModuleAsync(moduleName, version, cancellationToken);
    }

    public Task<bool> UninstallModuleAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        return _moduleService.UninstallModuleAsync(moduleName, cancellationToken);
    }

    public Task<IEnumerable<OqtaneModule>> GetModulesAsync(CancellationToken cancellationToken = default)
    {
        return _moduleService.GetModulesAsync(cancellationToken);
    }

    // 主题管理方法
    public Task<OqtaneTheme> InstallThemeAsync(string themeName, string version, CancellationToken cancellationToken = default)
    {
        return _themeService.InstallThemeAsync(themeName, version, cancellationToken);
    }

    public Task<bool> SetDefaultThemeAsync(string themeName, CancellationToken cancellationToken = default)
    {
        return _themeService.SetDefaultThemeAsync(themeName, cancellationToken);
    }

    public Task<IEnumerable<OqtaneTheme>> GetThemesAsync(CancellationToken cancellationToken = default)
    {
        return _themeService.GetThemesAsync(cancellationToken);
    }

    // 用户管理方法
    public Task<OqtaneUser> CreateUserAsync(string username, string email, string password, CancellationToken cancellationToken = default)
    {
        return _userService.CreateUserAsync(username, email, password, cancellationToken);
    }

    public Task<OqtaneUser> UpdateUserAsync(string username, OqtaneUser user, CancellationToken cancellationToken = default)
    {
        return _userService.UpdateUserAsync(username, user, cancellationToken);
    }

    public Task<bool> DeleteUserAsync(string username, CancellationToken cancellationToken = default)
    {
        return _userService.DeleteUserAsync(username, cancellationToken);
    }

    public Task<bool> AddUserToRoleAsync(string username, string roleName, CancellationToken cancellationToken = default)
    {
        return _userService.AddUserToRoleAsync(username, roleName, cancellationToken);
    }

    public Task<bool> RemoveUserFromRoleAsync(string username, string roleName, CancellationToken cancellationToken = default)
    {
        return _userService.RemoveUserFromRoleAsync(username, roleName, cancellationToken);
    }

    public Task<IEnumerable<OqtaneUser>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return _userService.GetUsersAsync(cancellationToken);
    }

    // 配置管理方法
    public Task<OqtaneConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default)
    {
        return _configurationService.GetConfigurationAsync(cancellationToken);
    }

    public Task<bool> UpdateConfigurationAsync(OqtaneConfiguration configuration, CancellationToken cancellationToken = default)
    {
        return _configurationService.UpdateConfigurationAsync(configuration, cancellationToken);
    }

    // 部署工具方法
    public Task<bool> PackageApplicationAsync(string outputPath, CancellationToken cancellationToken = default)
    {
        return _deploymentService.PackageApplicationAsync(outputPath, cancellationToken);
    }

    public Task<bool> DeployApplicationAsync(string serverUrl, string username, string password, CancellationToken cancellationToken = default)
    {
        return _deploymentService.DeployApplicationAsync(serverUrl, username, password, cancellationToken);
    }
}

// 依赖注入扩展
public static class OqtaneServiceCollectionExtensions
{
    public static IServiceCollection AddOqtaneServices(this IServiceCollection services, Action<OqtaneOptions> configureOptions = null)
    {
        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OqtaneOptions>(options => { });
        }

        // 注册服务
        services.AddSingleton<IOqtaneModuleService, OqtaneModuleService>();
        services.AddSingleton<IOqtaneThemeService, OqtaneThemeService>();
        services.AddSingleton<IOqtaneUserService, OqtaneUserService>();
        services.AddSingleton<IOqtaneConfigurationService, OqtaneConfigurationService>();
        services.AddSingleton<IOqtaneDeploymentService, OqtaneDeploymentService>();
        services.AddSingleton<IOqtaneService, OqtaneService>();

        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Oqtane 技能示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 Oqtane 服务
        services.AddOqtaneServices(options =>
        {
            options.Enabled = true;
            options.SiteName = "示例站点";
            options.DefaultLanguage = "zh-CN";
            options.EnableModules = true;
            options.EnableThemes = true;
            options.EnableUsers = true;
            options.EnableConfiguration = true;
            options.EnableDeployment = true;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var oqtaneService = serviceProvider.GetRequiredService<IOqtaneService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 模块管理
            Console.WriteLine("示例 1: 模块管理");
            var modules = await oqtaneService.GetModulesAsync();
            Console.WriteLine($"当前安装的模块数量: {modules.Count()}");
            foreach (var module in modules)
            {
                Console.WriteLine($"- {module.Name} ({module.Version})");
            }

            // 示例 2: 主题管理
            Console.WriteLine("\n示例 2: 主题管理");
            var themes = await oqtaneService.GetThemesAsync();
            Console.WriteLine($"当前安装的主题数量: {themes.Count()}");
            foreach (var theme in themes)
            {
                Console.WriteLine($"- {theme.Name} ({theme.Version}) {(theme.IsDefault ? "[默认]" : "")}");
            }

            // 示例 3: 用户管理
            Console.WriteLine("\n示例 3: 用户管理");
            var users = await oqtaneService.GetUsersAsync();
            Console.WriteLine($"当前用户数量: {users.Count()}");
            foreach (var user in users)
            {
                Console.WriteLine($"- {user.Username} ({user.Email}) - 角色: {string.Join(", ", user.Roles)}");
            }

            // 示例 4: 配置管理
            Console.WriteLine("\n示例 4: 配置管理");
            var config = await oqtaneService.GetConfigurationAsync();
            Console.WriteLine($"站点名称: {config.SiteName}");
            Console.WriteLine($"默认语言: {config.DefaultLanguage}");
            Console.WriteLine($"管理员邮箱: {config.AdminEmail}");

            // 示例 5: 部署工具
            Console.WriteLine("\n示例 5: 部署工具");
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "output");
            var packageResult = await oqtaneService.PackageApplicationAsync(outputPath);
            Console.WriteLine($"应用打包: {(packageResult ? "成功" : "失败")}");

            Console.WriteLine("\n所有示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
