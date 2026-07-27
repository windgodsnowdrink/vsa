#:sdk Microsoft.NET.Sdk
#:package Silky.Core@3.0.0
#:package Silky.Rpc@3.0.0
#:package Microsoft.Maui.Controls@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Silky.Core;
using Silky.Rpc;

// MAUI应用启动配置
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        // 配置MAUI
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
            
        // 添加Silky微服务支持
        builder.Services.AddSilkyServices<AppModule>();
        
        // 配置RPC服务
        builder.Services.AddRpcServices(options =>
        {
            options.UseZeroCopySerialization = true;
            options.UseCacheLineAlignment = true;
        });
        
        // 需要补充的Silky高级能力配置
        builder.Services.AddSilkyRpcServer(options => 
        {
            options.UseDisruptorPattern(); // Disruptor模式
            options.UseTokenRingBuffer();  // TokenRing缓冲区
            options.UseTailLatencyOptimizer(); // 尾延迟优化
        });
        
        // 添加AOT编译支持
        builder.Services.AddSilkyAotCompiler();
        
        // 添加二进制压缩
        builder.Services.AddBinaryCompression();
        
        // 添加Span零拷贝管道
        builder.Services.AddZeroCopyPipelines();
        
        // 添加线程专用内存池
        builder.Services.AddThreadLocalMemoryPool();
        
        return builder.Build();
    }
}

// Silky模块定义
public class AppModule : SilkyModule
{
    public override void ConfigureServices(IServiceCollection services)
    {
        // 注册微服务
        services.AddTransient<IMyMicroService, MyMicroService>();
        
        // 配置分层内存
        services.AddTieredMemory(options =>
        {
            options.UseThreadLocalSpan = true;
        });
    }
}

// MAUI主应用
public partial class App : Application
{
    public App(IMyMicroService microService)
    {
        InitializeComponent();
        
        // 使用微服务
        var result = microService.GetData();
        
        MainPage = new MainPage(result);
    }
}

// 微服务接口
public interface IMyMicroService
{
    string GetData();
}

// 微服务实现
public class MyMicroService : IMyMicroService
{
    public string GetData()
    {
        // 使用零拷贝技术处理数据
        return "Silky MAUI微服务数据";
    }
}

// MAUI主页面
public class MainPage : ContentPage
{
    public MainPage(string data)
    {
        Content = new VerticalStackLayout
        {
            Children = {
                new Label { Text = data }
            }
        };
    }
}