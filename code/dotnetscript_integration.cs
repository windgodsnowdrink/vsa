#:sdk Microsoft.NET.Sdk
#:package Microsoft.CodeAnalysis.CSharp.Scripting@4.7.0
#:package Microsoft.Extensions.DependencyModel@8.0.0-preview.3.23174.8
#:property LangVersion preview
#:property TargetFramework net11.0

public sealed class ScriptOptions
{
    public int MaxPoolSize { get; set; } = 10;
    public TimeSpan ExecutionTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableSandbox { get; set; } = true;
    public bool EnablePerformanceMonitoring { get; set; } = true;
    public string[] AllowedNamespaces { get; set; } = Array.Empty<string>();
}

public sealed class Script : IAsyncDisposable
{
    private readonly ScriptSecuritySandbox _sandbox;
    private readonly ScriptPerformanceMonitor _monitor;
    private readonly Pipe _outputPipe;
    private Task _executionTask;
    private CancellationTokenSource _cts;

    public Script(ScriptSecuritySandbox sandbox, ScriptPerformanceMonitor monitor)
    {
        _sandbox = sandbox;
        _monitor = monitor;
        _outputPipe = new Pipe();
    }

    public async Task<bool> ExecuteAsync(string scriptPath, string[] args, CancellationToken ct = default)
    {
        // ... 脚本执行实现 ...
    }

    public bool Reset()
    {
        // ... 重置脚本状态 ...
    }

    public async ValueTask DisposeAsync()
    {
        // ... 资源清理 ...
    }
}
#:property Nullable enable
#:property ImplicitUsings enable
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;

public sealed class ScriptExecutionContext : IAsyncDisposable
{
    private readonly Channel<ScriptExecutionRequest> _requestChannel;
    private readonly TransformBlock<ScriptExecutionRequest, ScriptExecutionResult> _processingBlock;
    private readonly ActionBlock<ScriptExecutionResult> _resultBlock;
    private readonly ObjectPool<Script> _scriptPool;
    private readonly ConcurrentDictionary<string, Assembly> _loadedAssemblies = new();
    private readonly ScriptSecuritySandbox _sandbox;
    private readonly ScriptPerformanceMonitor _monitor;

    public ScriptExecutionContext(ScriptOptions options)
    {
        // ... 初始化代码 ...
    }

    public async ValueTask DisposeAsync()
    {
        // ... 清理代码 ...
    }

    public async Task<ScriptExecutionResult> ExecuteAsync(ScriptExecutionRequest request, CancellationToken ct = default)
    {
        // ... 执行逻辑 ...
    }

    private sealed class ScriptSecuritySandbox
    {
        // ... 安全沙箱实现 ...
    }

    private sealed class ScriptPerformanceMonitor
    {
        // ... 性能监控实现 ...
    }
}
#:package Dotnet.Script@1.4.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package System.Threading.Channels@8.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0

public record ScriptExecutionRequest(string ScriptPath, string[] Args, bool DebugMode = false);
public record ScriptExecutionResult(bool Success, string Output, TimeSpan ExecutionTime);

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDotnetScript(this IServiceCollection services, Action<ScriptOptions> configure = null)
    {
        var options = new ScriptOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<ScriptExecutionContext>();
        services.AddSingleton<ObjectPool<Script>>(sp => 
            new DefaultObjectPool<Script>(new ScriptPooledPolicy(), options.MaxPoolSize));
        
        return services;
    }
}

internal sealed class ScriptPooledPolicy : IPooledObjectPolicy<Script>
{
    public Script Create() => new Script();
    public bool Return(Script obj) => obj.Reset();
}
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Threading.Tasks;
using Dotnet.Script.Core;
using Dotnet.Script.Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class ScriptExecutionOptions
{
    public string ScriptPath { get; set; }
    public string[] Args { get; set; } = Array.Empty<string>();
    public bool DebugMode { get; set; }
    public bool WatchMode { get; set; }
}

public class ScriptExecutor
{
    private readonly ILogger<ScriptExecutor> _logger;
    private readonly ObjectPool<ScriptCompiler> _compilerPool;
    private readonly ScriptConsole _scriptConsole;

    public ScriptExecutor(ILogger<ScriptExecutor> logger)
    {
        _logger = logger;
        _compilerPool = new DefaultObjectPool<ScriptCompiler>(new ScriptCompilerPooledPolicy());
        _scriptConsole = new ScriptConsole(logger);
    }

    public async Task<ScriptResult> ExecuteAsync(ScriptExecutionOptions options)
    {
        using var compiler = _compilerPool.Get();
        var command = new ExecuteScriptCommand(compiler, _scriptConsole, _logger);
        
        var commandOptions = new ExecuteScriptCommandOptions
        {
            ScriptPath = options.ScriptPath,
            DebugMode = options.DebugMode,
            WatchMode = options.WatchMode,
            Args = options.Args
        };

        return await command.Execute<int>(commandOptions);
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScriptExecution(this IServiceCollection services)
    {
        services.AddSingleton<ScriptExecutor>();
        services.AddSingleton<ScriptConsole>();
        services.AddObjectPool<ScriptCompiler, ScriptCompilerPooledPolicy>();
        return services;
    }
}

public class ScriptCompilerPooledPolicy : IPooledObjectPolicy<ScriptCompiler>
{
    public ScriptCompiler Create() => new ScriptCompiler(ScriptConsole.Default, null);
    
    public bool Return(ScriptCompiler obj) => true;
}

public class ScriptResult
{
    public bool Success { get; set; }
    public object? Result { get; set; }
    public Exception? Exception { get; set; }
}

// 示例用法
public static class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddConsole());
        services.AddScriptExecution();
        
        var provider = services.BuildServiceProvider();
        var executor = provider.GetRequiredService<ScriptExecutor>();
        
        var result = await executor.ExecuteAsync(new ScriptExecutionOptions
        {
            ScriptPath = "script.csx",
            Args = args
        });
        
        if (!result.Success)
        {
            Console.Error.WriteLine($"Script execution failed: {result.Exception}");
        }
    }
}