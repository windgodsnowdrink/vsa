# avalonia - 使用示例

## 快速开始

### 1. 基础 Avalonia 应用示例

```csharp
// 基础 Avalonia 应用示例
// #:sdk Microsoft.NET.Sdk
// #:package Avalonia@11.0.0
// #:package Avalonia.Desktop@11.0.0
// #:package Avalonia.Themes.Fluent@11.0.0
// #:property LangVersion=preview
// #:property TargetFramework=net11.0
// #:property Nullable=enable
// #:property ImplicitUsings=enable
// #:property OutputType=WinExe

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace BasicAvaloniaApp
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

### 2. 数据绑定示例

```csharp
// 数据绑定示例
// #:sdk Microsoft.NET.Sdk
// #:package Avalonia@11.0.0
// #:package Avalonia.Desktop@11.0.0
// #:package Avalonia.Themes.Fluent@11.0.0
// #:package ReactiveUI.Fody@17.4.2
// #:property LangVersion=preview
// #:property TargetFramework=net11.0
// #:property Nullable=enable
// #:property ImplicitUsings=enable
// #:property OutputType=WinExe

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace DataBindingApp
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
                desktop.MainWindow = new MainWindow {
                    DataContext = new MainWindowViewModel()
                };
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

    // 使用 ReactiveUI.Fody 自动生成属性更改通知
    public class MainWindowViewModel : ReactiveObject
    {
        [Reactive] public string Name { get; set; }
        [Reactive] public string Greeting { get; set; }
        
        public ReactiveCommand<Unit, Unit> ClickCommand { get; }
        
        public MainWindowViewModel()
        {
            ClickCommand = ReactiveCommand.Create(() => {
                Greeting = string.IsNullOrEmpty(Name) ? "请输入名称！" : $"你好，{Name}！";
            });
        }
    }
}
```

### 3. MVVM 模式示例

```csharp
// MVVM 模式示例
// #:sdk Microsoft.NET.Sdk
// #:package Avalonia@11.0.0
// #:package Avalonia.Desktop@11.0.0
// #:package Avalonia.Themes.Fluent@11.0.0
// #:package ReactiveUI@17.4.2
// #:property LangVersion=preview
// #:property TargetFramework=net11.0
// #:property Nullable=enable
// #:property ImplicitUsings=enable
// #:property OutputType=WinExe

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System.Reactive.Linq;

namespace MvvmApp
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
                desktop.MainWindow = new MainWindow {
                    DataContext = new MainWindowViewModel()
                };
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

    public class MainWindowViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> _isProcessing;
        private readonly ObservableAsPropertyHelper<string> _status;
        
        [Reactive] public string InputText { get; set; }
        [Reactive] public string OutputText { get; set; }
        
        public bool IsProcessing => _isProcessing.Value;
        public string Status => _status.Value;
        
        public ReactiveCommand<Unit, Unit> ProcessCommand { get; }
        
        public MainWindowViewModel()
        {
            // 命令可执行性条件
            var canProcess = this.WhenAnyValue(vm => vm.InputText)
                .Select(text => !string.IsNullOrWhiteSpace(text));
            
            // 处理命令
            ProcessCommand = ReactiveCommand.CreateFromTask(ProcessAsync, canProcess);
            
            // 处理状态
            _isProcessing = ProcessCommand.IsExecuting.ToProperty(this, vm => vm.IsProcessing);
            
            // 状态文本
            _status = ProcessCommand.IsExecuting
                .Select(executing => executing ? "处理中..." : "就绪")
                .ToProperty(this, vm => vm.Status);
        }
        
        private async Task ProcessAsync()
        {
            // 模拟异步处理
            await Task.Delay(1000);
            OutputText = InputText.ToUpper();
        }
    }
}
```

### 4. 自定义控件示例

```csharp
// 自定义控件示例
// #:sdk Microsoft.NET.Sdk
// #:package Avalonia@11.0.0
// #:package Avalonia.Desktop@11.0.0
// #:package Avalonia.Themes.Fluent@11.0.0
// #:property LangVersion=preview
// #:property TargetFramework=net11.0
// #:property Nullable=enable
// #:property ImplicitUsings=enable
// #:property OutputType=WinExe

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;

namespace CustomControlApp
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

    // 自定义控件
    public class CustomControl : Control
    {
        // 自定义属性
        public static readonly StyledProperty<string> CustomTextProperty =
            AvaloniaProperty.Register<CustomControl, string>(nameof(CustomText), "默认文本");

        public string CustomText
        {
            get => GetValue(CustomTextProperty);
            set => SetValue(CustomTextProperty, value);
        }

        public CustomControl()
        {
            // 设置默认样式
            this.Classes.Add("custom-control");
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            
            if (change.Property == CustomTextProperty)
            {
                // 处理属性变化
                InvalidateVisual();
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            
            // 自定义渲染逻辑
            var bounds = this.Bounds;
            
            // 绘制背景
            context.FillRectangle(Brushes.LightBlue, bounds);
            
            // 绘制边框
            context.DrawRectangle(new Pen(Brushes.Blue, 2), bounds);
            
            // 绘制文本
            var text = new FormattedText(
                CustomText,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                16,
                Brushes.Black);
            
            var textPosition = new Point(
                bounds.Left + 10,
                bounds.Top + (bounds.Height - text.Height) / 2);
            
            context.DrawText(text, textPosition);
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            customControl.CustomText = inputText.Text;
        }
    }
}
```

### 5. AOT 编译的 Avalonia 应用示例

```csharp
// AOT 编译的 Avalonia 应用示例
// #:sdk Microsoft.NET.Sdk
// #:package Avalonia@11.0.0
// #:package Avalonia.Desktop@11.0.0
// #:package Avalonia.Themes.Fluent@11.0.0
// #:property LangVersion=preview
// #:property TargetFramework=net11.0
// #:property Nullable=enable
// #:property ImplicitUsings=enable
// #:property OutputType=WinExe
// #:property PublishAot=true
// #:property TrimMode=Full
// #:property PublishReadyToRun=true
// #:property PublishSingleFile=true
// #:property SelfContained=true
// #:property RuntimeIdentifier=win-x64
// #:property UseMonoAotCompiler=true
// #:property MonoAotCompilerMode=Full

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace AotAvaloniaApp
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
            // 记录启动时间
            var startTime = Stopwatch.StartNew();
            
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
            
            // 停止计时
            startTime.Stop();
            Console.WriteLine($"应用启动耗时：{startTime.ElapsedMilliseconds} ms");
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }

    public partial class MainWindow : Window
    {
        private readonly Stopwatch _stopwatch = new();
        private int _clickCount = 0;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _stopwatch.Restart();
            _clickCount++;
            
            // 更新 UI
            clickCount.Text = $"点击次数：{_clickCount}";
            
            _stopwatch.Stop();
            uiTime.Text = $"UI 更新耗时：{_stopwatch.ElapsedMilliseconds} ms";
        }
    }
}
```

### 6. 响应式布局示例

```csharp
// 响应式布局示例
// #:sdk Microsoft.NET.Sdk
// #:package Avalonia@11.0.0
// #:package Avalonia.Desktop@11.0.0
// #:package Avalonia.Themes.Fluent@11.0.0
// #:property LangVersion=preview
// #:property TargetFramework=net11.0
// #:property Nullable=enable
// #:property ImplicitUsings=enable
// #:property OutputType=WinExe

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace ResponsiveLayoutApp
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

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // 响应窗口大小变化
            this.SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 根据窗口宽度调整布局
            if (e.NewSize.Width < 600)
            {
                // 窄屏布局
                mainGrid.ColumnDefinitions = new ColumnDefinitions("*");
                mainGrid.RowDefinitions = new RowDefinitions("Auto", "Auto", "*");
                leftPanel.SetValue(Grid.RowProperty, 0);
                leftPanel.SetValue(Grid.ColumnProperty, 0);
                rightPanel.SetValue(Grid.RowProperty, 1);
                rightPanel.SetValue(Grid.ColumnProperty, 0);
                contentPanel.SetValue(Grid.RowProperty, 2);
                contentPanel.SetValue(Grid.ColumnProperty, 0);
            }
            else
            {
                // 宽屏布局
                mainGrid.ColumnDefinitions = new ColumnDefinitions("200", "200", "*");
                mainGrid.RowDefinitions = new RowDefinitions("*");
                leftPanel.SetValue(Grid.RowProperty, 0);
                leftPanel.SetValue(Grid.ColumnProperty, 0);
                rightPanel.SetValue(Grid.RowProperty, 0);
                rightPanel.SetValue(Grid.ColumnProperty, 1);
                contentPanel.SetValue(Grid.RowProperty, 0);
                contentPanel.SetValue(Grid.ColumnProperty, 2);
            }
        }
    }
}
```

### 7. 与依赖注入集成示例

```csharp
// 依赖注入集成示例
// #:sdk Microsoft.NET.Sdk
// #:package Avalonia@11.0.0
// #:package Avalonia.Desktop@11.0.0
// #:package Avalonia.Themes.Fluent@11.0.0
// #:package Microsoft.Extensions.DependencyInjection@10.0.0
// #:package Microsoft.Extensions.Logging@10.0.0
// #:property LangVersion=preview
// #:property TargetFramework=net11.0
// #:property Nullable=enable
// #:property ImplicitUsings=enable
// #:property OutputType=WinExe

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace DiAvaloniaApp
{
    public class App : Application
    {
        private ServiceProvider _serviceProvider;
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // 构建依赖注入容器
            _serviceProvider = BuildServiceProvider();
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // 从容器获取主窗口
                desktop.MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            }

            base.OnFrameworkInitializationCompleted();
        }
        
        private ServiceProvider BuildServiceProvider()
        {
            var builder = new ServiceCollection();
            
            // 注册服务
            builder.AddHttpClient();
            builder.AddLogging(logging => {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });
            
            // 注册视图和视图模型
            builder.AddTransient<MainWindow>();
            builder.AddTransient<MainWindowViewModel>();
            builder.AddTransient<IDataService, DataService>();
            
            return builder.BuildServiceProvider();
        }
        
        public override void OnExit(ExitEventArgs e)
        {
            _serviceProvider.Dispose();
            base.OnExit(e);
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

    // 服务接口和实现
    public interface IDataService
    {
        Task<string> GetDataAsync();
    }

    public class DataService : IDataService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DataService> _logger;
        
        public DataService(HttpClient httpClient, ILogger<DataService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        public async Task<string> GetDataAsync()
        {
            _logger.LogInformation("获取数据中...");
            
            try
            {
                // 模拟 API 调用
                await Task.Delay(1000);
                return "从服务获取的数据";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取数据失败");
                return "获取数据失败";
            }
        }
    }

    public class MainWindowViewModel
    {
        private readonly IDataService _dataService;
        private string _data = "点击按钮获取数据";
        
        public string Data
        {
            get => _data;
            set => this.RaiseAndSetIfChanged(ref _data, value);
        }
        
        public ReactiveCommand<Unit, Unit> GetDataCommand { get; }
        
        public MainWindowViewModel(IDataService dataService)
        {
            _dataService = dataService;
            
            GetDataCommand = ReactiveCommand.CreateFromTask(async () => {
                Data = await _dataService.GetDataAsync();
            });
        }
    }
}
```

## 总结

以上示例展示了 Avalonia 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始使用基础 Avalonia 应用开发
2. 学习数据绑定和 MVVM 模式的使用
3. 开发自定义 Avalonia 控件
4. 实现响应式布局
5. 使用 AOT 编译优化 Avalonia 应用性能
6. 与依赖注入集成
7. 学习各种 Avalonia 最佳实践

所有示例均基于 .NET 10 开发，支持 AOT 编译，您可以根据需要选择合适的示例进行学习和参考。Avalonia 框架提供了强大的跨平台 UI 开发能力，结合 AOT 编译，可以显著提高应用的启动速度和运行性能，非常适合开发各种规模的桌面应用。
