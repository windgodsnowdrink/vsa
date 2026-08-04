#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package Microsoft.Extensions.Hosting@10.0.0-rc.1.25451.107
#:package System.Threading.Channels@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool.DependencyInjection@9.9.0
#:package Swashbuckle.AspNetCore@9.0.4
#:package Swashbuckle.AspNetCore.Swagger@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerGen@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerUI@9.0.4
#:package Scalar.AspNetCore@2.8.0
#:package Ardalis.ListStartupServices@1.1.4
#:package Serilog@4.3.0
#:package Serilog.AspNetCore@9.0.0
#:package Serilog.Enrichers.Environment@3.0.1
#:package Serilog.Enrichers.Process@3.0.0
#:package Serilog.Enrichers.Span@3.1.0
#:package Serilog.Enrichers.Thread@4.0.0
#:package Serilog.Exceptions@8.4.0
#:package Serilog.Extensions.Hosting@9.0.0
#:package Serilog.Extensions.Logging@9.0.2
#:package Serilog.Formatting.Compact@3.0.0
#:package Serilog.Settings.Configuration@9.0.0
#:package Serilog.Sinks.Async@2.1.0
#:package Serilog.Sinks.Console@6.0.0
#:package Serilog.Sinks.EventLog@4.0.0
#:package Serilog.Sinks.Http@9.2.0
#:package Serilog.Sinks.RollingFileAlternate@2.0.9
#:package Serilog.Sinks.Seq@9.0.0
#:package Serilog.Sinks.SpectreConsole@0.3.3
#:package Serilog.Sinks.Trace@4.0.0
#:package ModelContextProtocol@0.3.0-preview.4
#:package ModelContextProtocol.Core@0.3.0-preview.4
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property RollForward=Major
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=True
#:property Platform=Any CPU
#:property PackAsTool=True
#:property PackageType=McpServer
#:property PackageReadmeFile=README.md
#:property PackageId=AOT.SampleMcpServer
#:property PackageVersion=0.0.1-beta
#:property PackageTags=AI; MCP; server; stdio
#:property Description=An MCP server using the MCP C# SDK.

using App;
using Ardalis.ListStartupServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ModelContextProtocol.Server;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

builder.Logging.ClearProviders();               // 只保留下面的 ConsoleProvider，避免干扰
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);                   // 使用颜色化的控制台日志
builder.Logging.SetMinimumLevel(LogLevel.Trace); // 设为 Trace，最细粒度
builder.Logging.AddFilter("Microsoft.Extensions.Http", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.Extensions.ServiceDiscovery", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.Extensions.Resilience", LogLevel.Trace);
builder.Logging.AddFilter("RestEase.HttpClientFactory", LogLevel.Trace);
builder.Logging.AddFilter("App.ServiceDiscoveryHandler", LogLevel.Trace);
Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Error)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "error", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Fatal)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "fatal", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Information)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "info", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Warning)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "warning", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog(Log.Logger, true).ConfigureLogging((context, logging) =>
{
    logging.ClearProviders();
    logging.AddConfiguration(context.Configuration);
    logging.AddSerilog(Log.Logger, true);
});
builder.Services.AddOptions();
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination", "X-Platform-Type", "X-CSRF-TOKEN-HEADERNAME", "X-CSRF-TOKEN", "X-Forwarded-For", "X-Forwarded-Host", "X-Forwarded-Proto", "X-Forwarded-Prefix", "X-Body-Hash")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
// });

// 13. 主程序演示
var poolService = new ObjectPoolService();

// StringBuilder池使用示例
var parts = new[] { "Hello", "World", "Object", "Pool", "Demo" };
var result = poolService.BuildComplexString(parts);
Console.WriteLine($"String Builder Result: {result}");

// HttpClient池使用示例
try
{
    //var apiResult = await poolService.CallApiAsync("/users");
    //Console.WriteLine($"API Result Length: {apiResult?.Length ?? 0}");
}
catch (Exception ex)
{
    Console.WriteLine($"API Call Demo Skipped: {ex.Message}");
}

// 数据库连接池使用示例
var queries = new[]
{
            "SELECT * FROM Users WHERE Id = 1",
            "UPDATE Users SET LastLogin = NOW() WHERE Id = 2",
            "INSERT INTO Logs (Message) VALUES ('Test log')"
        };

poolService.ExecuteDatabaseOperations(queries);

// 性能测试
var performanceTest = new PerformanceTest();
await performanceTest.RunPerformanceComparison();

// 打印统计信息
poolService.PrintPoolStatistics();

// 线程安全示例
var threadSafeUsage = new ThreadSafeObjectPoolUsage();
var threadSafeResult = threadSafeUsage.ProcessDataSafe("Thread safe test");
Console.WriteLine($"Thread Safe Result: {threadSafeResult}");
threadSafeUsage.Cleanup();

// 测试MCP
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<App.RandomNumberTools>();

builder.Services.Configure<Ardalis.ListStartupServices.ServiceConfig>(config =>
{
    config.Services = [.. builder.Services];
    config.Path = "/services";
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapScalarApiReference(); // scalar
}
else
{
    app.UseExceptionHandler("/Error");
}
app.UseSerilogRequestLogging(opt =>
{
    opt.IncludeQueryInRequestPath = true;
});
app.UseRouting();
app.UseCors();
// app.UseAuthorization();
// app.UseSwagger();
// app.UseSwaggerUI(options =>
// {
//     options.SwaggerEndpoint("/swagger/v1/swagger.json", "Robot API V1.1.0.0");
// });
app.UseShowAllServicesMiddleware();
app.MapGet("/", () => "Mcp Agent Integration!");
app.UseEndpoints(options =>
{
    options.MapControllers();
});

await app.RunAsync();

namespace App
{
    public partial class Program;

    // 1. 自定义对象池策略 - StringBuilder池
    public class StringBuilderPooledObjectPolicy : PooledObjectPolicy<StringBuilder>
    {
        private readonly int _maxCapacity;

        public StringBuilderPooledObjectPolicy(int maxCapacity = 1024)
        {
            _maxCapacity = maxCapacity;
        }

        public override StringBuilder Create()
        {
            return new StringBuilder();
        }

        public override bool Return(StringBuilder obj)
        {
            // 重置StringBuilder状态
            if (obj.Capacity > _maxCapacity)
            {
                // 避免对象过大占用过多内存
                return false;
            }

            obj.Clear();
            return true;
        }
    }

    // 2. 自定义对象池策略 - HttpClient池
    public class HttpClientPooledObjectPolicy : PooledObjectPolicy<HttpClient>
    {
        private readonly string _baseUrl;
        private readonly TimeSpan _timeout;

        public HttpClientPooledObjectPolicy(string baseUrl, TimeSpan timeout)
        {
            _baseUrl = baseUrl;
            _timeout = timeout;
        }

        public override HttpClient Create()
        {
            var handler = new HttpClientHandler
            {
                MaxConnectionsPerServer = 10,
                UseProxy = false
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = _timeout
            };

            return client;
        }

        public override bool Return(HttpClient obj)
        {
            // 检查HttpClient是否仍然有效
            return obj != null && obj.Timeout == _timeout;
        }
    }

    // 3. 数据库连接池示例
    public class DatabaseConnection
    {
        public string ConnectionId { get; }
        public DateTime LastUsed { get; set; }
        public bool IsBusy { get; set; }

        public DatabaseConnection()
        {
            ConnectionId = Guid.NewGuid().ToString();
            LastUsed = DateTime.UtcNow;
            IsBusy = false;
        }

        public void ExecuteQuery(string sql)
        {
            // 模拟数据库操作
            Console.WriteLine($"Executing query on connection {ConnectionId}: {sql}");
            LastUsed = DateTime.UtcNow;
            IsBusy = true;

            // 模拟网络延迟
            Thread.Sleep(100);

            IsBusy = false;
        }
    }

    public class DatabaseConnectionPooledObjectPolicy : PooledObjectPolicy<DatabaseConnection>
    {
        private readonly TimeSpan _maxIdleTime = TimeSpan.FromMinutes(5);

        public override DatabaseConnection Create()
        {
            return new DatabaseConnection();
        }

        public override bool Return(DatabaseConnection obj)
        {
            // 检查连接是否超时或正在使用
            if (obj == null || obj.IsBusy)
            {
                return false;
            }

            // 检查连接是否空闲太久
            if (DateTime.UtcNow - obj.LastUsed > _maxIdleTime)
            {
                return false;
            }

            return true;
        }
    }

    // 4. 生产级服务类
    public class ObjectPoolService
    {
        private readonly ObjectPool<StringBuilder> _stringBuilderPool;
        private readonly ObjectPool<HttpClient> _httpClientPool;
        private readonly ObjectPool<DatabaseConnection> _dbConnectionPool;

        public ObjectPoolService()
        {
            // 配置StringBuilder池
            var stringBuilderPolicy = new StringBuilderPooledObjectPolicy(2048);
            _stringBuilderPool = new DefaultObjectPool<StringBuilder>(stringBuilderPolicy, 100);

            // 配置HttpClient池
            var httpClientPolicy = new HttpClientPooledObjectPolicy("https://api.example.com", TimeSpan.FromSeconds(30));
            _httpClientPool = new DefaultObjectPool<HttpClient>(httpClientPolicy, 10);

            // 配置数据库连接池
            var dbConnectionPolicy = new DatabaseConnectionPooledObjectPolicy();
            _dbConnectionPool = new DefaultObjectPool<DatabaseConnection>(dbConnectionPolicy, 20);
        }

        // 5. 使用StringBuilder池的字符串构建方法
        public string BuildComplexString(string[] parts)
        {
            var sb = _stringBuilderPool.Get();

            try
            {
                foreach (var part in parts)
                {
                    sb.Append(part);
                    sb.Append(" | ");
                }

                return sb.ToString();
            }
            finally
            {
                _stringBuilderPool.Return(sb);
            }
        }

        // 6. 使用HttpClient池的API调用方法
        public async Task<string> CallApiAsync(string endpoint)
        {
            var client = _httpClientPool.Get();

            try
            {
                var response = await client.GetAsync(endpoint);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API call failed: {ex.Message}");
                throw;
            }
            finally
            {
                _httpClientPool.Return(client);
            }
        }

        // 7. 使用数据库连接池的数据访问方法
        public void ExecuteDatabaseOperations(string[] queries)
        {
            Parallel.ForEach(queries, query =>
            {
                var connection = _dbConnectionPool.Get();

                try
                {
                    connection.ExecuteQuery(query);
                }
                finally
                {
                    _dbConnectionPool.Return(connection);
                }
            });
        }

        // 8. 对象池统计信息
        public void PrintPoolStatistics()
        {
            Console.WriteLine("=== Object Pool Statistics ===");
            Console.WriteLine($"StringBuilder Pool: Current objects");
            Console.WriteLine($"HttpClient Pool: Current objects");
            Console.WriteLine($"Database Connection Pool: Current objects");
        }
    }

    // 9. 对象池工厂模式示例
    public static class ObjectPoolFactory
    {
        public static ObjectPool<T> CreateDefaultPool<T>(int maximumRetained = 100) where T : class, new()
        {
            return new DefaultObjectPool<T>(new DefaultPooledObjectPolicy<T>(), maximumRetained);
        }

        public static ObjectPool<StringBuilder> CreateStringBuilderPool(int maxCapacity = 2048, int maximumRetained = 100)
        {
            var policy = new StringBuilderPooledObjectPolicy(maxCapacity);
            return new DefaultObjectPool<StringBuilder>(policy, maximumRetained);
        }

        public static ObjectPool<T> CreatePoolWithPolicy<T>(PooledObjectPolicy<T> policy, int maximumRetained = 100)
            where T : class
        {
            return new DefaultObjectPool<T>(policy, maximumRetained);
        }
    }

    // 10. ASP.NET Core集成示例
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // 注册对象池到DI容器
            services.AddSingleton<ObjectPoolService>();

            // 或者直接注册StringBuilder池
            services.AddSingleton(provider =>
            {
                var policy = new StringBuilderPooledObjectPolicy(4096);
                return new DefaultObjectPool<StringBuilder>(policy, 200);
            });
        }
    }

    // 11. 性能测试示例
    public class PerformanceTest
    {
        private readonly ObjectPool<StringBuilder> _pool;
        private readonly StringBuilderPooledObjectPolicy _policy;

        public PerformanceTest()
        {
            _policy = new StringBuilderPooledObjectPolicy();
            _pool = new DefaultObjectPool<StringBuilder>(_policy);
        }

        public async Task RunPerformanceComparison()
        {
            const int iterations = 10000;

            // 直接创建StringBuilder
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var sb = new StringBuilder();
                sb.Append($"Test string {i}");
                var result = sb.ToString();
            }
            stopwatch.Stop();
            Console.WriteLine($"Direct creation: {stopwatch.ElapsedMilliseconds}ms");

            // 使用对象池
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                var sb = _pool.Get();
                sb.Append($"Test string {i}");
                var result = sb.ToString();
                _pool.Return(sb);
            }
            stopwatch.Stop();
            Console.WriteLine($"Object pool usage: {stopwatch.ElapsedMilliseconds}ms");
        }
    }

    // 12. 线程安全的对象池使用示例
    public class ThreadSafeObjectPoolUsage
    {
        private readonly ObjectPool<StringBuilder> _pool;
        private static readonly ThreadLocal<StringBuilder> _localStringBuilder = new ThreadLocal<StringBuilder>();

        public ThreadSafeObjectPoolUsage()
        {
            _pool = ObjectPoolFactory.CreateStringBuilderPool();
        }

        public string ProcessDataSafe(string data)
        {
            // 线程本地StringBuilder（适用于频繁的小操作）
            if (_localStringBuilder.Value == null)
            {
                _localStringBuilder.Value = _pool.Get();
            }

            var sb = _localStringBuilder.Value;
            sb.Clear();
            sb.Append("Processed: ");
            sb.Append(data);

            return sb.ToString();
        }

        public void Cleanup()
        {
            if (_localStringBuilder.Value != null)
            {
                _pool.Return(_localStringBuilder.Value);
                _localStringBuilder.Value = null;
            }
            _localStringBuilder.Dispose();
        }
    }
    
    public class Test
    {
        // 对象池大小配置
        var pool = new DefaultObjectPool<StringBuilder>(
            new StringBuilderPooledObjectPolicy(),
            Environment.ProcessorCount * 2  // 或根据性能测试调整
        );

        //对象状态重置
        public override bool Return(StringBuilder obj)
        {
            obj.Clear();  // 必须重置状态
            return obj.Capacity <= _maxCapacity;  // 避免过大对象占用内存
        }

        // 异常安全处理
        public void ReturnTest() {
            
            var obj = _pool.Get();
            try
            {
                // 使用对象
            }
            finally
            {
                _pool.Return(obj);  // 必须在finally块中返回
            }
        }

        //线程安全考虑
        // 可以考虑ThreadLocal优化频繁的小对象使用
        private static readonly ThreadLocal<StringBuilder> _localStringBuilder =
            new ThreadLocal<StringBuilder>();
    }
}