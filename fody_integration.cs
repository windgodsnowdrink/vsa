#:sdk Microsoft.NET.Sdk.Web
#:package Fody@6.8.0
#:package Costura.Fody@5.7.0
#:package PropertyChanged.Fody@3.6.0
#:package Microsoft.Extensions.Logging@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

// 1. 日志注入属性
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class LogAttribute : Attribute
{
    public LogLevel Level { get; set; } = LogLevel.Information;
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public LogAttribute(LogLevel level = LogLevel.Information)
    {
        Level = level;
    }
}

// 2. 高性能日志上下文
[StructLayout(LayoutKind.Auto)]
public class LogContext
{
    public string Message { get; set; }
    public LogLevel Level { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}

// 3. 日志处理器(Disruptor模式)
[SkipLocalsInit]
public sealed class LogProcessor : IAsyncDisposable
{
    private readonly Channel<LogContext> _logChannel;
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _cts = new();
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public LogProcessor(ILogger<LogProcessor> logger)
    {
        _logger = logger;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        _logChannel = Channel.CreateBounded<LogContext>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });
        
        _ = ProcessLogsAsync();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task EnqueueLogAsync(LogContext context)
    {
        await _logChannel.Writer.WriteAsync(context);
    }

    private async Task ProcessLogsAsync()
    {
        await foreach (var log in _logChannel.Reader.ReadAllAsync(_cts.Token))
        {
            _logger.Log(log.Level, log.Message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _logChannel.Writer.Complete();
    }
}

// 4. 动态编译拦截增强
[SkipLocalsInit]
public class MethodTimerAttribute : Attribute
{
    private readonly LogProcessor _logProcessor;
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public MethodTimerAttribute([CallerMemberName] string methodName = "")
    {
        // Fody会在编译时注入计时和日志代码
    }
}

// 5. AOT友好的动态代理增强
[StructLayout(LayoutKind.Auto)]
public class AotFriendlyProxy<T> : DispatchProxy
{
    private readonly LogProcessor _logProcessor;
    private static readonly ConcurrentDictionary<MethodInfo, MethodInfo> _methodCache = new();
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        // 使用预编译的表达式树替代反射
        var compiledMethod = _methodCache.GetOrAdd(targetMethod, m => 
        {
            var instanceParam = Expression.Parameter(typeof(T), "instance");
            var argsParam = Expression.Parameter(typeof(object[]), "args");
            
            var parameters = m.GetParameters();
            var arguments = new Expression[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                arguments[i] = Expression.Convert(
                    Expression.ArrayIndex(argsParam, Expression.Constant(i)),
                    parameters[i].ParameterType);
            }
            
            var call = Expression.Call(
                instanceParam, 
                m, 
                arguments);
            
            if (m.ReturnType == typeof(void))
            {
                return Expression.Lambda<Action<T, object[]>>(
                    call, 
                    instanceParam, 
                    argsParam).Compile();
            }
            else
            {
                return Expression.Lambda<Func<T, object[], object>>(
                    Expression.Convert(call, typeof(object)),
                    instanceParam,
                    argsParam).Compile();
            }
        });

        try
        {
            _logProcessor.Log(new LogContext 
            { 
                Message = $"Invoking {targetMethod.Name}",
                Level = LogLevel.Debug,
                Timestamp = DateTimeOffset.UtcNow
            });

            if (compiledMethod is Action<T, object[]> voidMethod)
            {
                voidMethod((T)__target, args);
                return null;
            }
            else if (compiledMethod is Func<T, object[], object> func)
            {
                return func((T)__target, args);
            }
            
            return default;
        }
        catch (Exception ex)
        {
            _logProcessor.Log(new LogContext 
            { 
                Message = $"Error in {targetMethod.Name}: {ex.Message}",
                Level = LogLevel.Error,
                Timestamp = DateTimeOffset.UtcNow
            });
            throw;
        }
    }
}

// 6. 启动配置
var builder = WebApplication.CreateBuilder(args);

// 注册日志处理器
builder.Services.AddSingleton<LogProcessor>();
builder.Services.AddSingleton<DynamicCompilationInterceptor>();
builder.Services.AddLogging(logging => 
{
    logging.AddConsole();
    logging.AddDebug();
});

// 使用AOT友好的动态代理
builder.Services.AddTransient<IService>(sp => 
    DispatchProxy.Create<IService, AotFriendlyProxy<IService>>());

var app = builder.Build();
app.MapGet("/", () => "Enhanced Fody Integration Demo");
app.Run();

// FodyWeavers.xml配置示例
/*
<Weavers xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="FodyWeavers.xsd">
  <PropertyChanged/>
  <MethodTimer/>
  <ConfigureAwait continueOnCapturedContext="false"/>
  <Log/>
</Weavers>
*/


// 4. 动态编译拦截增强
[SkipLocalsInit]
public class DynamicCompilationInterceptor
{
    private readonly ILogger _logger;
    private readonly ObjectPool<CompilationContext> _contextPool;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DynamicCompilationInterceptor(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DynamicCompilationInterceptor>();
        _contextPool = new DefaultObjectPool<CompilationContext>(
            new CompilationContextPooledPolicy(), 
            Environment.ProcessorCount * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public MethodInfo Intercept(MethodInfo originalMethod)
    {
        using var context = _contextPool.Get();
        try 
        {
            // 使用Roslyn API动态生成代理方法
            var syntaxTree = CSharpSyntaxTree.ParseText($@"
                [System.Runtime.CompilerServices.MethodImpl(
                    System.Runtime.CompilerServices.MethodImplOptions.AggressiveOptimization)]
                public static {GetTypeName(originalMethod.ReturnType)} {originalMethod.Name}(
                    {string.Join(", ", originalMethod.GetParameters()
                        .Select(p => $"{GetTypeName(p.ParameterType)} {p.Name}"))})
                {{
                    // 这里可以插入性能监控、日志记录等逻辑
                    return {originalMethod.DeclaringType?.FullName}.{originalMethod.Name}({
                        string.Join(", ", originalMethod.GetParameters().Select(p => p.Name))});
                }}");

            var compilation = CSharpCompilation.Create(
                "DynamicProxyAssembly",
                new[] { syntaxTree },
                new[] { 
                    MetadataReference.CreateFromFile(
                        typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(
                        originalMethod.DeclaringType?.Assembly.Location)
                },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release));

            using var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);
            
            if (!emitResult.Success)
            {
                throw new InvalidOperationException(
                    "Dynamic compilation failed: " + 
                    string.Join(", ", emitResult.Diagnostics));
            }

            var assembly = Assembly.Load(ms.ToArray());
            return assembly.GetType("DynamicProxy")
                .GetMethod(originalMethod.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dynamic compilation failed");
            throw;
        }
    }

    private static string GetTypeName(Type type) => 
        type.IsGenericType ? 
            $"{type.Name.Split('`')[0]}<{string.Join(", ", type.GetGenericArguments().Select(GetTypeName))}>" : 
            type.Name;
}