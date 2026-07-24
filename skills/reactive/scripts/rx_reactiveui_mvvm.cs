#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package ReactiveUI@18.3.1
#:package System.Reactive@6.0.0
#:package System.Threading.Channels@8.0.0
#:package System.Runtime.CompilerServices.Unsafe@6.0.0
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property Optimize=true
#:property LangVersion=preview

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveUI;
using System.Buffers;
using System.Threading;

// 配置选项
public class ReactiveOptions
{
    public bool EnableMemoryPooling { get; set; } = true;
    public int MaxPoolSize { get; set; } = 1000;
    public bool EnableBatchProcessing { get; set; } = false;
    public int BatchSize { get; set; } = 100;
    public int BatchTimeout { get; set; } = 100;
    public bool EnableDebugLogging { get; set; } = false;
}

// Reactive 服务接口
public interface IReactiveService
{
    Task<string> DoSomethingAsync();
    Task<T> ExecuteAsync<T>(Func<Task<T>> function);
    IObservable<T> CreateObservable<T>(Func<T> function);
    void SubscribeToEvents<T>(IObservable<T> observable, Action<T> onNext);
}

// ViewModel基类
public abstract class ReactiveViewModelBase : ReactiveObject, IDisposable
{
    protected readonly CompositeDisposable _disposables;
    private readonly ObjectPool<CompositeDisposable> _disposablePool;
    
    protected ReactiveViewModelBase(ObjectPool<CompositeDisposable> disposablePool)
    {
        _disposablePool = disposablePool;
        _disposables = _disposablePool.Get();
    }
    
    protected void AddDisposable(IDisposable disposable)
    {
        _disposables.Add(disposable);
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
        _disposablePool.Return(_disposables);
    }
}

// 示例ViewModel
public class UserProfileViewModel : ReactiveViewModelBase
{
    private string _name;
    private int _age;
    
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    
    public int Age
    {
        get => _age;
        set => this.RaiseAndSetIfChanged(ref _age, value);
    }
    
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public Interaction<string, bool> ConfirmInteraction { get; } = new();
    
    public UserProfileViewModel(ObjectPool<CompositeDisposable> disposablePool)
        : base(disposablePool)
    {
        // 命令绑定
        var canSave = this.WhenAnyValue(
            x => x.Name,
            x => x.Age,
            (name, age) => !string.IsNullOrEmpty(name) && age > 0);
                    
        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);
                
        // 订阅命令执行
        SaveCommand
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => Console.WriteLine("保存成功！"))
            .DisposeWith(_disposables);
    }
    
    private async Task SaveAsync()
    {
        // 模拟保存操作
        await Task.Delay(1000);
        Console.WriteLine($"保存用户: {Name}, {Age}");
        
        // 触发交互
        var confirmed = await ConfirmInteraction.Handle($"确定要保存 {Name} 的信息吗？");
        if (confirmed)
        {
            Console.WriteLine("用户确认保存");
        }
        else
        {
            Console.WriteLine("用户取消保存");
        }
    }
}

// Reactive 服务实现
public class ReactiveService : IReactiveService
{
    private readonly ILogger<ReactiveService> _logger;
    private readonly ReactiveOptions _options;
    private readonly ObjectPool<CompositeDisposable> _disposablePool;
    
    public ReactiveService(
        ILogger<ReactiveService> logger,
        IOptions<ReactiveOptions> options,
        ObjectPool<CompositeDisposable> disposablePool)
    {
        _logger = logger;
        _options = options.Value;
        _disposablePool = disposablePool;
    }
    
    public async Task<string> DoSomethingAsync()
    {
        _logger.LogInformation("执行 Reactive 操作");
        await Task.Delay(100);
        return "Reactive 操作执行成功";
    }
    
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> function)
    {
        using var disposables = _disposablePool.Get();
        try
        {
            _logger.LogDebug("开始执行异步操作");
            var result = await function();
            _logger.LogDebug("异步操作执行完成");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "异步操作执行失败");
            throw;
        }
        finally
        {
            _disposablePool.Return(disposables);
        }
    }
    
    public IObservable<T> CreateObservable<T>(Func<T> function)
    {
        return Observable.Defer(() => {
            try
            {
                var result = function();
                return Observable.Return(result);
            }
            catch (Exception ex)
            {
                return Observable.Throw<T>(ex);
            }
        });
    }
    
    public void SubscribeToEvents<T>(IObservable<T> observable, Action<T> onNext)
    {
        using var disposables = _disposablePool.Get();
        observable
            .Subscribe(
                onNext,
                ex => _logger.LogError(ex, "事件订阅失败"),
                () => _logger.LogDebug("事件订阅完成")
            )
            .DisposeWith(disposables);
    }
}

// 依赖注入扩展
public static class ReactiveServiceCollectionExtensions
{
    public static IServiceCollection AddReactiveServices(this IServiceCollection services, Action<ReactiveOptions> configureOptions = null)
    {
        // 配置选项
        services.Configure(configureOptions ?? (options => { }));
        
        // 注册对象池
        services.AddSingleton<ObjectPool<CompositeDisposable>>(sp => {
            var options = sp.GetRequiredService<IOptions<ReactiveOptions>>().Value;
            return new ObjectPool<CompositeDisposable>(
                () => new CompositeDisposable(),
                options.MaxPoolSize
            );
        });
        
        // 注册服务
        services.AddSingleton<IReactiveService, ReactiveService>();
        
        return services;
    }
}

// 对象池实现
public class ObjectPool<T> where T : class
{
    private readonly Func<T> _objectFactory;
    private readonly ConcurrentBag<T> _objects;
    private readonly int _maxPoolSize;
    private int _currentSize;
    
    public ObjectPool(Func<T> objectFactory, int maxPoolSize = 1000)
    {
        _objectFactory = objectFactory;
        _objects = new ConcurrentBag<T>();
        _maxPoolSize = maxPoolSize;
        _currentSize = 0;
    }
    
    public T Get()
    {
        if (_objects.TryTake(out var obj))
        {
            return obj;
        }
        
        if (Interlocked.Increment(ref _currentSize) <= _maxPoolSize)
        {
            return _objectFactory();
        }
        
        Interlocked.Decrement(ref _currentSize);
        return _objectFactory();
    }
    
    public void Return(T obj)
    {
        if (obj != null && _currentSize <= _maxPoolSize)
        {
            _objects.Add(obj);
        }
    }
}

// 扩展方法
public static class DisposableExtensions
{
    public static T DisposeWith<T>(this T disposable, CompositeDisposable compositeDisposable) where T : IDisposable
    {
        compositeDisposable.Add(disposable);
        return disposable;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Reactive 技能示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置日志
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 Reactive 服务
        builder.AddReactiveServices(options => {
            options.EnableMemoryPooling = true;
            options.MaxPoolSize = 1000;
            options.EnableBatchProcessing = false;
        });
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取服务
        var reactiveService = serviceProvider.GetRequiredService<IReactiveService>();
        var disposablePool = serviceProvider.GetRequiredService<ObjectPool<CompositeDisposable>>();
        
        // 测试 Reactive 服务
        Console.WriteLine("测试 Reactive 服务...");
        var result = await reactiveService.DoSomethingAsync();
        Console.WriteLine($"结果: {result}");
        
        // 测试 ViewModel
        Console.WriteLine("\n测试 ViewModel...");
        using var viewModel = new UserProfileViewModel(disposablePool);
        
        // 绑定属性
        viewModel.Name = "张三";
        viewModel.Age = 30;
        
        Console.WriteLine($"ViewModel 属性: Name={viewModel.Name}, Age={viewModel.Age}");
        Console.WriteLine($"SaveCommand 可执行: {viewModel.SaveCommand.CanExecute(null)}");
        
        // 执行命令
        if (viewModel.SaveCommand.CanExecute(null))
        {
            Console.WriteLine("执行 SaveCommand...");
            await viewModel.SaveCommand.ExecuteAsync(null);
        }
        
        // 测试交互
        Console.WriteLine("\n测试交互...");
        viewModel.ConfirmInteraction.RegisterHandler(async interaction => {
            Console.WriteLine($"交互请求: {interaction.Input}");
            interaction.SetOutput(true); // 模拟用户确认
        });
        
        // 再次执行命令，触发交互
        if (viewModel.SaveCommand.CanExecute(null))
        {
            Console.WriteLine("再次执行 SaveCommand...");
            await viewModel.SaveCommand.ExecuteAsync(null);
        }
        
        Console.WriteLine("\n示例执行完成！");
    }
}

// 主程序入口
public class ProgramEntry
{
    public static async Task Main(string[] args)
    {
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置日志
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 Reactive 服务
        builder.AddReactiveServices(options => {
            options.EnableMemoryPooling = true;
            options.MaxPoolSize = 1000;
            options.EnableBatchProcessing = false;
        });
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 测试 Reactive 服务
        var reactiveService = serviceProvider.GetRequiredService<IReactiveService>();
        var result = await reactiveService.DoSomethingAsync();
        Console.WriteLine($"Reactive 服务测试结果: {result}");
        
        // 测试 ViewModel
        var viewModel = new UserProfileViewModel(serviceProvider.GetRequiredService<ObjectPool<CompositeDisposable>>());
        viewModel.Name = "李四";
        viewModel.Age = 25;
        
        Console.WriteLine($"ViewModel 测试: Name={viewModel.Name}, Age={viewModel.Age}");
        
        // 测试命令
        if (viewModel.SaveCommand.CanExecute(null))
        {
            await viewModel.SaveCommand.ExecuteAsync(null);
        }
        
        Console.WriteLine("\n所有测试执行完成！");
    }
}
