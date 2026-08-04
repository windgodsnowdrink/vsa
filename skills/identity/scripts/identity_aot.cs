#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.IdentityModel.Tokens.Jwt@7.0.0
#:package Microsoft.IdentityModel.Tokens@7.0.0
#:package Microsoft.AspNetCore.Authentication.JwtBearer@7.0.0
#:package System.Security.Cryptography@4.3.0
#:package System.Text.Json@8.0.0
#:package System.Text.RegularExpressions@4.3.1
#:package System.Collections.Immutable@8.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package Microsoft.Extensions.Http@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Identity AOT 引擎");
        Console.WriteLine("=" * 60);
        
        var serviceProvider = BuildServiceProvider();
        var identityService = serviceProvider.GetRequiredService<IdentityService>();
        var settings = serviceProvider.GetRequiredService<IOptions<IdentitySettings>>().Value;
        
        var command = args.Length > 0 ? args[0].ToLower() : "help";
        var arguments = args.Skip(1).ToArray();
        
        try
        {
            switch (command)
            {
                case "generate":
                case "g":
                    await GenerateToken(identityService, arguments);
                    break;
                case "validate":
                case "v":
                    await ValidateToken(identityService, arguments);
                    break;
                case "decode":
                case "d":
                    await DecodeToken(identityService, arguments);
                    break;
                case "revoke":
                case "r":
                    await RevokeToken(identityService, arguments);
                    break;
                case "refresh":
                case "re":
                    await RefreshToken(identityService, arguments);
                    break;
                case "auth":
                case "a":
                    await AuthenticateUser(identityService, arguments);
                    break;
                case "authorize":
                case "au":
                    await AuthorizeUser(identityService, arguments);
                    break;
                case "benchmark":
                case "b":
                    await RunBenchmark(identityService, arguments);
                    break;
                case "config":
                case "co":
                    ShowConfig(settings);
                    break;
                case "help":
                case "h":
                case "?":
                    ShowHelp();
                    break;
                default:
                    Console.WriteLine($"未知命令: {command}");
                    ShowHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            await identityService.DisposeAsync();
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<IdentitySettings>(options => {
            options.Issuer = "https://identity.example.com";
            options.Audience = "https://api.example.com";
            options.Key = "your-secret-key-here-change-in-production";
            options.TokenExpiry = TimeSpan.FromHours(1);
            options.RefreshTokenExpiry = TimeSpan.FromDays(7);
            options.EnableTokenRevocation = true;
            options.EnableCaching = true;
            options.CacheSize = 1000;
            options.CacheExpiry = TimeSpan.FromHours(1);
            options.Permissions = new Dictionary<string, List<string>> {
                { "admin", new List<string> { "read", "write", "delete", "admin" } },
                { "user", new List<string> { "read", "write" } },
                { "guest", new List<string> { "read" } }
            };
        });
        
        services.AddSingleton<IdentityService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
    
    private static async Task GenerateToken(IdentityService service, string[] arguments)
    {
        if (arguments.Length < 2)
        {
            Console.WriteLine("错误: 请提供用户名和角色");
            return;
        }
        
        var username = arguments[0];
        var role = arguments[1];
        var claims = new List<Claim>();
        
        if (arguments.Length > 2)
        {
            for (int i = 2; i < arguments.Length; i++)
            {
                var claimParts = arguments[i].Split(":");
                if (claimParts.Length == 2)
                {
                    claims.Add(new Claim(claimParts[0], claimParts[1]));
                }
            }
        }
        
        Console.WriteLine($"生成令牌: 用户名={username}, 角色={role}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.GenerateTokenAsync(username, role, claims);
        stopwatch.Stop();
        
        Console.WriteLine($"令牌生成完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"访问令牌: {result.AccessToken}");
        Console.WriteLine($"刷新令牌: {result.RefreshToken}");
        Console.WriteLine($"过期时间: {result.ExpiresAt}");
    }
    
    private static async Task ValidateToken(IdentityService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("错误: 请提供令牌");
            return;
        }
        
        var token = arguments[0];
        Console.WriteLine("验证令牌...");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ValidateTokenAsync(token);
        stopwatch.Stop();
        
        Console.WriteLine($"令牌验证完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"验证状态: {(result.Valid ? "有效" : "无效"}");
        
        if (result.Valid)
        {
            Console.WriteLine($"用户名: {result.Username}");
            Console.WriteLine($"角色: {result.Role}");
            Console.WriteLine($"过期时间: {result.ExpiresAt}");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task DecodeToken(IdentityService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("错误: 请提供令牌");
            return;
        }
        
        var token = arguments[0];
        Console.WriteLine("解码令牌...");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.DecodeTokenAsync(token);
        stopwatch.Stop();
        
        Console.WriteLine($"令牌解码完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"解码状态: {(result.Success ? "成功" : "失败"}");
        
        if (result.Success)
        {
            Console.WriteLine($"用户名: {result.Username}");
            Console.WriteLine($"角色: {result.Role}");
            Console.WriteLine($"发行者: {result.Issuer}");
            Console.WriteLine($"受众: {result.Audience}");
            Console.WriteLine($"颁发时间: {result.IssuedAt}");
            Console.WriteLine($"过期时间: {result.ExpiresAt}");
            
            if (result.Claims.Any())
            {
                Console.WriteLine("声明:");
                foreach (var claim in result.Claims)
                {
                    Console.WriteLine($"  {claim.Type}: {claim.Value}");
                }
            }
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task RevokeToken(IdentityService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("错误: 请提供令牌");
            return;
        }
        
        var token = arguments[0];
        Console.WriteLine("撤销令牌...");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.RevokeTokenAsync(token);
        stopwatch.Stop();
        
        Console.WriteLine($"令牌撤销完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"撤销状态: {(result.Success ? "成功" : "失败"}");
        
        if (!result.Success)
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task RefreshToken(IdentityService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("错误: 请提供刷新令牌");
            return;
        }
        
        var refreshToken = arguments[0];
        Console.WriteLine("刷新令牌...");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.RefreshTokenAsync(refreshToken);
        stopwatch.Stop();
        
        Console.WriteLine($"令牌刷新完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"刷新状态: {(result.Success ? "成功" : "失败"}");
        
        if (result.Success)
        {
            Console.WriteLine($"新访问令牌: {result.AccessToken}");
            Console.WriteLine($"新刷新令牌: {result.RefreshToken}");
            Console.WriteLine($"过期时间: {result.ExpiresAt}");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task AuthenticateUser(IdentityService service, string[] arguments)
    {
        if (arguments.Length < 2)
        {
            Console.WriteLine("错误: 请提供用户名和密码");
            return;
        }
        
        var username = arguments[0];
        var password = arguments[1];
        Console.WriteLine($"认证用户: {username}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.AuthenticateUserAsync(username, password);
        stopwatch.Stop();
        
        Console.WriteLine($"用户认证完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"认证状态: {(result.Success ? "成功" : "失败"}");
        
        if (result.Success)
        {
            Console.WriteLine($"用户名: {result.Username}");
            Console.WriteLine($"角色: {result.Role}");
            Console.WriteLine($"访问令牌: {result.AccessToken}");
            Console.WriteLine($"刷新令牌: {result.RefreshToken}");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task AuthorizeUser(IdentityService service, string[] arguments)
    {
        if (arguments.Length < 2)
        {
            Console.WriteLine("错误: 请提供令牌和权限");
            return;
        }
        
        var token = arguments[0];
        var permission = arguments[1];
        Console.WriteLine($"授权用户: 权限={permission}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.AuthorizeUserAsync(token, permission);
        stopwatch.Stop();
        
        Console.WriteLine($"用户授权完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"授权状态: {(result.Success ? "授权通过" : "授权失败"}");
        
        if (!result.Success)
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task RunBenchmark(IdentityService service, string[] arguments)
    {
        var iterations = arguments.Length > 0 ? int.Parse(arguments[0]) : 1000;
        Console.WriteLine($"运行基准测试: 迭代次数={iterations}");
        
        var stopwatch = Stopwatch.StartNew();
        var successes = 0;
        var failures = 0;
        
        for (int i = 0; i < iterations; i++)
        {
            try
            {
                var result = await service.GenerateTokenAsync($"user{i}", "user");
                var validateResult = await service.ValidateTokenAsync(result.AccessToken);
                if (validateResult.Valid)
                {
                    successes++;
                }
                else
                {
                    failures++;
                }
            }
            catch
            {
                failures++;
            }
        }
        
        stopwatch.Stop();
        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        var operationsPerSecond = iterations / elapsedSeconds;
        
        Console.WriteLine($"基准测试完成!");
        Console.WriteLine($"总用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"成功: {successes}");
        Console.WriteLine($"失败: {failures}");
        Console.WriteLine($"每秒操作数: {operationsPerSecond:F2} ops/s");
    }
    
    private static void ShowConfig(IdentitySettings settings)
    {
        Console.WriteLine("Identity 配置:");
        Console.WriteLine("=" * 60);
        Console.WriteLine($"发行者: {settings.Issuer}");
        Console.WriteLine($"受众: {settings.Audience}");
        Console.WriteLine($"令牌过期: {settings.TokenExpiry}");
        Console.WriteLine($"刷新令牌过期: {settings.RefreshTokenExpiry}");
        Console.WriteLine($"启用令牌撤销: {settings.EnableTokenRevocation}");
        Console.WriteLine($"启用缓存: {settings.EnableCaching}");
        Console.WriteLine($"缓存大小: {settings.CacheSize}");
        Console.WriteLine($"缓存过期: {settings.CacheExpiry}");
        Console.WriteLine($"权限配置: {settings.Permissions.Count} 个角色");
        foreach (var permission in settings.Permissions)
        {
            Console.WriteLine($"  {permission.Key}: {string.Join(", ", permission.Value)}");
        }
    }
    
    private static void ShowHelp()
    {
        Console.WriteLine("Identity AOT 引擎 命令帮助:");
        Console.WriteLine("=" * 60);
        Console.WriteLine("generate (g)    - 生成访问令牌");
        Console.WriteLine("validate (v)    - 验证访问令牌");
        Console.WriteLine("decode (d)      - 解码访问令牌");
        Console.WriteLine("revoke (r)      - 撤销访问令牌");
        Console.WriteLine("refresh (re)    - 刷新访问令牌");
        Console.WriteLine("auth (a)        - 认证用户");
        Console.WriteLine("authorize (au)  - 授权用户");
        Console.WriteLine("benchmark (b)   - 运行基准测试");
        Console.WriteLine("config (co)     - 显示配置信息");
        Console.WriteLine("help (h, ?)     - 显示帮助信息");
    }
}

public class IdentitySettings
{
    public string Issuer { get; set; } = "https://identity.example.com";
    public string Audience { get; set; } = "https://api.example.com";
    public string Key { get; set; } = "your-secret-key-here-change-in-production";
    public TimeSpan TokenExpiry { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan RefreshTokenExpiry { get; set; } = TimeSpan.FromDays(7);
    public bool EnableTokenRevocation { get; set; } = true;
    public bool EnableCaching { get; set; } = true;
    public int CacheSize { get; set; } = 1000;
    public TimeSpan CacheExpiry { get; set; } = TimeSpan.FromHours(1);
    public Dictionary<string, List<string>> Permissions { get; set; } = new() {
        { "admin", new List<string> { "read", "write", "delete", "admin" } },
        { "user", new List<string> { "read", "write" } },
        { "guest", new List<string> { "read" } }
    };
}

public class TokenResult
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class TokenValidationResult
{
    public bool Valid { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Error { get; set; }
}

public class TokenDecodeResult
{
    public bool Success { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public IEnumerable<Claim> Claims { get; set; } = new List<Claim>();
    public string Error { get; set; }
}

public class TokenRevocationResult
{
    public bool Success { get; set; }
    public string Error { get; set; }
}

public class TokenRefreshResult
{
    public bool Success { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Error { get; set; }
}

public class AuthenticationResult
{
    public bool Success { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Error { get; set; }
}

public class AuthorizationResult
{
    public bool Success { get; set; }
    public string Error { get; set; }
}

public class IdentityService : IAsyncDisposable
{
    private readonly ILogger<IdentityService> _logger;
    private readonly IdentitySettings _settings;
    private readonly MemoryCache _cache;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SymmetricSecurityKey _securityKey;
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters _validationParameters;
    private readonly HashSet<string> _revokedTokens;
    private readonly object _revokedTokensLock = new();
    
    public IdentityService(ILogger<IdentityService> logger, IOptions<IdentitySettings> options)
    {
        _logger = logger;
        _settings = options.Value;
        
        // 初始化缓存
        var cacheOptions = new MemoryCacheOptions {
            SizeLimit = _settings.CacheSize
        };
        _cache = new MemoryCache(cacheOptions);
        
        // 初始化 JWT 处理器
        _tokenHandler = new JwtSecurityTokenHandler();
        
        // 初始化安全密钥
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature);
        
        // 初始化令牌验证参数
        _validationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            IssuerSigningKey = _securityKey
        };
        
        // 初始化撤销令牌集合
        _revokedTokens = new HashSet<string>();
        
        _logger.LogInformation("Identity 服务初始化成功");
    }
    
    public async Task<TokenResult> GenerateTokenAsync(string username, string role, IEnumerable<Claim> additionalClaims = null)
    {
        try
        {
            _logger.LogInformation($"生成令牌: 用户名={username}, 角色={role}");
            
            // 创建声明
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };
            
            // 添加额外声明
            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }
            
            // 创建令牌
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_settings.TokenExpiry),
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                SigningCredentials = _signingCredentials
            };
            
            var token = _tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = _tokenHandler.WriteToken(token);
            
            // 生成刷新令牌
            var refreshToken = GenerateRefreshToken();
            
            // 缓存刷新令牌
            if (_settings.EnableCaching)
            {
                var refreshTokenKey = $"refresh:{refreshToken}";
                _cache.Set(refreshTokenKey, new { Username = username, Role = role }, new MemoryCacheEntryOptions {
                    AbsoluteExpirationRelativeToNow = _settings.RefreshTokenExpiry,
                    Size = 1
                });
            }
            
            return new TokenResult {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = token.ValidTo
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成令牌失败");
            throw;
        }
    }
    
    public async Task<TokenValidationResult> ValidateTokenAsync(string token)
    {
        try
        {
            _logger.LogInformation("验证令牌");
            
            // 检查令牌是否已撤销
            if (_settings.EnableTokenRevocation && IsTokenRevoked(token))
            {
                return new TokenValidationResult {
                    Valid = false,
                    Error = "令牌已被撤销"
                };
            }
            
            // 验证令牌
            ClaimsPrincipal principal;
            SecurityToken validatedToken;
            
            try
            {
                principal = _tokenHandler.ValidateToken(token, _validationParameters, out validatedToken);
            }
            catch (Exception ex)
            {
                return new TokenValidationResult {
                    Valid = false,
                    Error = ex.Message
                };
            }
            
            // 提取声明
            var username = principal.FindFirst(ClaimTypes.Name)?.Value;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value;
            var expiresAt = validatedToken.ValidTo;
            
            return new TokenValidationResult {
                Valid = true,
                Username = username,
                Role = role,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证令牌失败");
            return new TokenValidationResult {
                Valid = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<TokenDecodeResult> DecodeTokenAsync(string token)
    {
        try
        {
            _logger.LogInformation("解码令牌");
            
            // 解码令牌
            ClaimsPrincipal principal;
            SecurityToken decodedToken;
            
            try
            {
                principal = _tokenHandler.ValidateToken(token, _validationParameters, out decodedToken);
            }
            catch (Exception ex)
            {
                return new TokenDecodeResult {
                    Success = false,
                    Error = ex.Message
                };
            }
            
            // 提取声明
            var username = principal.FindFirst(ClaimTypes.Name)?.Value;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value;
            var jwtToken = decodedToken as JwtSecurityToken;
            
            return new TokenDecodeResult {
                Success = true,
                Username = username,
                Role = role,
                Issuer = jwtToken?.Issuer,
                Audience = jwtToken?.Audience,
                IssuedAt = jwtToken?.ValidFrom ?? DateTime.MinValue,
                ExpiresAt = jwtToken?.ValidTo ?? DateTime.MinValue,
                Claims = principal.Claims
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解码令牌失败");
            return new TokenDecodeResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<TokenRevocationResult> RevokeTokenAsync(string token)
    {
        try
        {
            _logger.LogInformation("撤销令牌");
            
            if (!_settings.EnableTokenRevocation)
            {
                return new TokenRevocationResult {
                    Success = false,
                    Error = "令牌撤销功能未启用"
                };
            }
            
            // 添加到撤销列表
            lock (_revokedTokensLock)
            {
                _revokedTokens.Add(token);
            }
            
            return new TokenRevocationResult {
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "撤销令牌失败");
            return new TokenRevocationResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<TokenRefreshResult> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            _logger.LogInformation("刷新令牌");
            
            // 从缓存获取刷新令牌信息
            var refreshTokenKey = $"refresh:{refreshToken}";
            var refreshTokenInfo = _cache.Get<dynamic>(refreshTokenKey);
            
            if (refreshTokenInfo == null)
            {
                return new TokenRefreshResult {
                    Success = false,
                    Error = "刷新令牌无效或已过期"
                };
            }
            
            // 生成新令牌
            var username = refreshTokenInfo.Username;
            var role = refreshTokenInfo.Role;
            var newTokenResult = await GenerateTokenAsync(username, role);
            
            // 删除旧刷新令牌
            _cache.Remove(refreshTokenKey);
            
            return new TokenRefreshResult {
                Success = true,
                AccessToken = newTokenResult.AccessToken,
                RefreshToken = newTokenResult.RefreshToken,
                ExpiresAt = newTokenResult.ExpiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新令牌失败");
            return new TokenRefreshResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<AuthenticationResult> AuthenticateUserAsync(string username, string password)
    {
        try
        {
            _logger.LogInformation($"认证用户: {username}");
            
            // 这里应该是实际的用户验证逻辑
            // 为了演示，我们使用简单的硬编码验证
            if (username == "admin" && password == "admin123")
            {
                var tokenResult = await GenerateTokenAsync(username, "admin");
                return new AuthenticationResult {
                    Success = true,
                    Username = username,
                    Role = "admin",
                    AccessToken = tokenResult.AccessToken,
                    RefreshToken = tokenResult.RefreshToken
                };
            }
            else if (username == "user" && password == "user123")
            {
                var tokenResult = await GenerateTokenAsync(username, "user");
                return new AuthenticationResult {
                    Success = true,
                    Username = username,
                    Role = "user",
                    AccessToken = tokenResult.AccessToken,
                    RefreshToken = tokenResult.RefreshToken
                };
            }
            else
            {
                return new AuthenticationResult {
                    Success = false,
                    Error = "用户名或密码错误"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "认证用户失败");
            return new AuthenticationResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<AuthorizationResult> AuthorizeUserAsync(string token, string permission)
    {
        try
        {
            _logger.LogInformation($"授权用户: 权限={permission}");
            
            // 验证令牌
            var validationResult = await ValidateTokenAsync(token);
            if (!validationResult.Valid)
            {
                return new AuthorizationResult {
                    Success = false,
                    Error = validationResult.Error
                };
            }
            
            // 检查权限
            if (_settings.Permissions.TryGetValue(validationResult.Role, out var rolePermissions))
            {
                if (rolePermissions.Contains(permission))
                {
                    return new AuthorizationResult {
                        Success = true
                    };
                }
                else
                {
                    return new AuthorizationResult {
                        Success = false,
                        Error = "用户没有所需的权限"
                    };
                }
            }
            else
            {
                return new AuthorizationResult {
                    Success = false,
                    Error = "角色不存在"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "授权用户失败");
            return new AuthorizationResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    private bool IsTokenRevoked(string token)
    {
        lock (_revokedTokensLock)
        {
            return _revokedTokens.Contains(token);
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        try
        {
            _cache.Dispose();
            _logger.LogInformation("Identity 服务已释放");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放 Identity 服务失败");
        }
        
        await Task.CompletedTask;
    }
}
