# avalonia Agent Skill - avalonia 技能

## 技能概述

基于 .NET 10 的高性能 Avalonia UI 框架技能，为 .NET 开发者提供强大的跨平台桌面应用开发功能，支持 Windows、macOS、Linux 等多种平台，具备现代化的 UI 设计和高性能的渲染引擎，支持 AOT 编译优化。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Avalonia@11.0.0
#:package Avalonia.Desktop@11.0.0
#:package Avalonia.Themes.Fluent@11.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
```

### 创建 Avalonia 应用

```csharp
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace AvaloniaApp
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
```

### 主窗口 XAML

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="AvaloniaApp.MainWindow"
        Title="Avalonia 应用示例">
    <StackPanel Margin="20">
        <TextBlock Text="欢迎使用 Avalonia 应用" FontSize="24" FontWeight="Bold" Margin="0 0 0 20"/>
        <Button Content="点击我" Click="Button_Click" Width="120" Height="30"/>
        <TextBlock x:Name="resultText" Margin="0 20 0 0" FontSize="16"/>
    </StackPanel>
</Window>
```

### 主窗口代码

```csharp
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            resultText.Text = "按钮被点击了！";
        }
    }
}
```

## 导航地图

```
avalonia/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
├── scripts/                    # 脚本和工具
    ├── avalonia_integration.cs          # Avalonia 集成示例
    ├── avalonia_integration.run.json    # Avalonia 集成运行配置
    ├── avalonia_integration.setting.json # Avalonia 集成设置文件
    ├── basic_avalonia_app.cs            # 基础 Avalonia 应用
    ├── basic_avalonia_app.run.json      # 基础 Avalonia 应用运行配置
    ├── basic_avalonia_app.setting.json   # 基础 Avalonia 应用设置文件
    ├── simple_avalonia_app.cs           # 简单 Avalonia 应用
    ├── simple_avalonia_app.run.json     # 简单 Avalonia 应用运行配置
    └── simple_avalonia_app.setting.json  # 简单 Avalonia 应用设置文件
```

## 主要功能

1. **跨平台支持**: 支持 Windows、macOS、Linux 等多种平台
2. **现代化 UI 设计**: 提供现代化的 UI 控件和主题
3. **高性能渲染**: 基于 Direct2D、Skia 等高性能渲染引擎
4. **MVVM 支持**: 内置 MVVM 框架支持
5. **XAML 支持**: 支持 XAML 标记语言
6. **响应式设计**: 支持响应式布局和自适应设计
7. **主题支持**: 支持多种主题，包括 Fluent、Material 等
8. **控件库丰富**: 提供丰富的 UI 控件库
9. **高性能设计**: 优化的性能实现，支持高并发场景
10. **AOT 编译优化**: 支持将 Avalonia 应用编译为本机代码，提高启动速度和运行性能
11. **易于使用的 API**: 简洁直观的 API 设计，降低开发复杂度
12. **可扩展架构**: 支持自定义扩展和插件开发
13. **与 .NET 生态集成**: 与 .NET 生态系统无缝集成

## 扩展说明

此技能提供完整的 Avalonia UI 框架解决方案，您可以根据需要进行扩展：

1. **自定义控件开发**: 开发自定义 Avalonia 控件
2. **主题定制**: 定制 Avalonia 应用主题
3. **扩展功能模块**: 添加新的功能模块和组件
4. **集成第三方库**: 与第三方库和服务集成
5. **性能优化**: 针对特定场景优化 Avalonia 应用性能
6. **添加新的平台支持**: 扩展到新的平台和设备

## 最佳实践

1. **使用 MVVM 模式**: 采用 MVVM 模式开发 Avalonia 应用，提高代码的可维护性和可测试性
2. **异步编程**: 优先使用异步 API 进行操作，避免阻塞 UI 线程
3. **合理使用数据绑定**: 合理使用数据绑定，减少手动 UI 更新
4. **优化 UI 渲染**: 优化 UI 渲染性能，减少不必要的重绘
5. **使用资源和样式**: 合理使用资源和样式，提高 UI 一致性和可维护性
6. **测试 UI 功能**: 充分测试 UI 功能，确保用户体验
7. **使用 AOT 编译**: 对于性能敏感的应用，考虑使用 AOT 编译优化
8. **监控应用性能**: 监控 Avalonia 应用性能，及时发现和解决性能瓶颈
9. **遵循 Avalonia 最佳实践**: 遵循 Avalonia 框架的最佳实践和设计指南
10. **保持代码简洁**: 保持代码简洁，避免过度复杂的 UI 逻辑

## AOT 编译支持

### AOT 编译配置

在项目文件中添加以下配置以支持 AOT 编译：

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <TrimMode>Full</TrimMode>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  <UseMonoAotCompiler>true</UseMonoAotCompiler>
  <MonoAotCompilerMode>Full</MonoAotCompilerMode>
</PropertyGroup>
```

### AOT 编译命令

```bash
# 编译为 Windows x64 原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为 Linux x64 原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained

# 编译为 macOS x64 原生可执行文件
dotnet publish -c Release -r osx-x64 --self-contained
```

### AOT 编译注意事项

1. **使用 AOT 兼容的库**: 确保使用的 Avalonia 相关库支持 AOT 编译
2. **避免反射**: 避免在 UI 处理中使用反射，或使用 Source Generator 替代
3. **资源加载**: 确保所有 UI 资源都能在 AOT 编译时被正确处理
4. **动态代码生成**: 避免使用动态代码生成，如 System.Reflection.Emit
5. **测试验证**: 在 AOT 编译后进行充分的测试，确保 UI 功能正常工作
6. **性能优化**: AOT 编译可以显著提高 Avalonia 应用的启动速度和运行性能
7. **内存优化**: AOT 编译可以减少 Avalonia 应用的内存占用

## 与其他系统集成

### 与 .NET 后端集成

```csharp
// Avalonia 应用与 .NET 后端 API 集成示例
public class MainWindowViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient;
    private string _result;
    
    public string Result
    {
        get => _result;
        set => this.RaiseAndSetIfChanged(ref _result, value);
    }
    
    public AsyncCommand GetDataCommand { get; }
    
    public MainWindowViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
        GetDataCommand = new AsyncCommand(async () => await GetDataAsync());
    }
    
    private async Task GetDataAsync()
    {
        try
        {
            // 调用后端 API
            var response = await _httpClient.GetAsync("https://api.example.com/data");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            Result = data;
        }
        catch (Exception ex)
        {
            Result = $"错误：{ex.Message}";
        }
    }
}
```

### 与依赖注入集成

```csharp
// Avalonia 应用与依赖注入集成示例
public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // 使用依赖注入创建主窗口
            var container = BuildContainer();
            desktop.MainWindow = container.Resolve<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private ServiceProvider BuildContainer()
    {
        var builder = new ServiceCollection();
        
        // 注册服务
        builder.AddHttpClient();
        builder.AddTransient<MainWindow>();
        builder.AddTransient<MainWindowViewModel>();
        builder.AddTransient<IDataService, DataService>();
        
        return builder.BuildServiceProvider();
    }
}
```
