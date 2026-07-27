# UI 技能使用示例

## 1. 基本应用结构

### 1.1 最小化应用

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UI.MinimalExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建服务容器
            var services = new ServiceCollection();
            
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
            
            // 构建服务提供程序
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            
            logger.LogInformation("UI Minimal Example Starting");
            logger.LogInformation("This is a minimal UI application structure");
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            
            logger.LogInformation("UI Minimal Example Exiting");
        }
    }
}
```

### 1.2 完整应用结构

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:package Microsoft.Extensions.Configuration@9.0.0
#:package Microsoft.Extensions.Configuration.Json@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace UI.CompleteExample
{
    public interface IUIComponent
    {
        void Render();
    }
    
    public class ButtonComponent : IUIComponent
    {
        private readonly ILogger<ButtonComponent> _logger;
        
        public ButtonComponent(ILogger<ButtonComponent> logger)
        {
            _logger = logger;
        }
        
        public void Render()
        {
            _logger.LogInformation("Rendering Button Component");
            Console.WriteLine("[Button] Click Me");
        }
    }
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建配置
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            // 构建服务容器
            var services = new ServiceCollection();
            
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });
            
            // 添加配置
            services.AddSingleton<IConfiguration>(configuration);
            
            // 添加自定义服务
            services.AddTransient<IUIComponent, ButtonComponent>();
            
            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();
            
            try
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("UI Complete Example Starting");
                
                // 使用服务
                var button = serviceProvider.GetRequiredService<IUIComponent>();
                button.Render();
                
                logger.LogInformation("UI Complete Example Running");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                
                logger.LogInformation("UI Complete Example Exiting");
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred");
                throw;
            }
        }
    }
}
```

## 2. 命令行工具使用

### 2.1 基本命令行

```bash
# 运行 Web UI
dotnet run --project scripts/ui_core.cs --ui-type web

# 运行桌面 UI
dotnet run --project scripts/ui_core.cs --ui-type desktop

# 运行移动 UI
dotnet run --project scripts/ui_core.cs --ui-type mobile

# 运行 Scrutor 示例
dotnet run --project scripts/ui_generator.cs
```

### 2.2 高级命令行选项

```bash
# 运行 Web UI，使用深色主题，中文语言
dotnet run --project scripts/ui_core.cs --ui-type web --theme dark --culture zh-CN

# 运行桌面 UI，使用系统主题，英文语言
dotnet run --project scripts/ui_core.cs --ui-type desktop --theme system --culture en-US

# 运行移动 UI，使用浅色主题，启用调试日志
dotnet run --project scripts/ui_core.cs --ui-type mobile --theme light --log-level debug

# 构建项目
dotnet build -c Release

# 发布项目（AOT 编译）
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true

# 发布到 Linux
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true

# 发布到 macOS
dotnet publish -c Release -r osx-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
```

## 3. Web UI 示例

### 3.1 基本 Web 应用

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Components.Web@9.0.0
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UI.Web.Basic
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // 添加服务
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            
            // 构建应用
            var app = builder.Build();
            
            // 配置 HTTP 管道
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            
            app.Run();
        }
    }
}
```

### 3.2 响应式设计

```razor
@* Pages/Index.razor *@
@page "/"
@using Microsoft.AspNetCore.Components.Web

<h1>Responsive Web App</h1>

<div class="container">
    <div class="row">
        <div class="col-sm-12 col-md-6 col-lg-4">
            <div class="card">
                <div class="card-header">
                    Feature 1
                </div>
                <div class="card-body">
                    <p>This feature is responsive and adapts to different screen sizes.</p>
                </div>
            </div>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4">
            <div class="card">
                <div class="card-header">
                    Feature 2
                </div>
                <div class="card-body">
                    <p>This feature is responsive and adapts to different screen sizes.</p>
                </div>
            </div>
        </div>
        <div class="col-sm-12 col-md-6 col-lg-4">
            <div class="card">
                <div class="card-header">
                    Feature 3
                </div>
                <div class="card-body">
                    <p>This feature is responsive and adapts to different screen sizes.</p>
                </div>
            </div>
        </div>
    </div>
</div>

<style>
    .container {
        padding: 20px;
    }
    
    .card {
        margin-bottom: 20px;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }
    
    @media (max-width: 576px) {
        h1 {
            font-size: 1.5rem;
        }
    }
    
    @media (min-width: 576px) {
        h1 {
            font-size: 2rem;
        }
    }
    
    @media (min-width: 992px) {
        h1 {
            font-size: 2.5rem;
        }
    }
</style>
```

### 3.3 实时通信

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR@9.0.0
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UI.Web.Realtime
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // 添加服务
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSignalR();
            
            // 构建应用
            var app = builder.Build();
            
            // 配置 HTTP 管道
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            
            app.MapBlazorHub();
            app.MapHub<ChatHub>("/chathub");
            app.MapFallbackToPage("/_Host");
            
            app.Run();
        }
    }
}
```

```razor
@* Pages/Chat.razor *@
@page "/chat"
@using Microsoft.AspNetCore.SignalR.Client
@using Microsoft.AspNetCore.Components.Web
@implements IDisposable

<h1>Real-time Chat</h1>

<div class="chat-container">
    <div class="chat-messages" @ref="messagesList">
        @foreach (var message in messages)
        {
            <div class="message">
                <strong>@message.User:</strong> @message.Message
            </div>
        }
    </div>
    
    <div class="chat-input">
        <input type="text" @bind="userName" placeholder="Your name" />
        <input type="text" @bind="messageText" placeholder="Type a message" />
        <button @onclick="SendMessage">Send</button>
    </div>
</div>

@code {
    private HubConnection hubConnection;
    private List<Message> messages = new List<Message>();
    private string userName = "User";
    private string messageText = string.Empty;
    private ElementReference messagesList;
    
    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"))
            .Build();
        
        hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            messages.Add(new Message { User = user, Message = message });
            StateHasChanged();
        });
        
        await hubConnection.StartAsync();
    }
    
    private async Task SendMessage()
    {
        if (!string.IsNullOrWhiteSpace(messageText))
        {
            await hubConnection.SendAsync("SendMessage", userName, messageText);
            messageText = string.Empty;
        }
    }
    
    public void Dispose()
    {
        hubConnection?.DisposeAsync();
    }
    
    private class Message
    {
        public string User { get; set; }
        public string Message { get; set; }
    }
}

<style>
    .chat-container {
        max-width: 800px;
        margin: 0 auto;
        border: 1px solid #ddd;
        border-radius: 8px;
        overflow: hidden;
    }
    
    .chat-messages {
        height: 400px;
        overflow-y: auto;
        padding: 20px;
        background-color: #f5f5f5;
    }
    
    .message {
        margin-bottom: 10px;
        padding: 10px;
        background-color: white;
        border-radius: 4px;
    }
    
    .chat-input {
        padding: 20px;
        background-color: white;
        border-top: 1px solid #ddd;
    }
    
    input {
        margin-right: 10px;
        padding: 8px;
        border: 1px solid #ddd;
        border-radius: 4px;
    }
    
    button {
        padding: 8px 16px;
        background-color: #007bff;
        color: white;
        border: none;
        border-radius: 4px;
        cursor: pointer;
    }
    
    button:hover {
        background-color: #0069d9;
    }
</style>
```

## 4. 桌面 UI 示例

### 4.1 基本桌面应用

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Maui@9.0.0
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Maui.Applications;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace UI.Desktop.Basic
{
    public class MainPage : ContentPage
    {
        public MainPage()
        {
            Title = "Basic Desktop App";
            
            var label = new Label
            {
                Text = "Welcome to Desktop App",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                FontSize = 24
            };
            
            var button = new Button
            {
                Text = "Click Me",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            
            button.Clicked += (sender, e) =>
            {
                label.Text = "Button clicked!";
            };
            
            Content = new StackLayout
            {
                Children = {
                    label,
                    button
                },
                Padding = new Thickness(20)
            };
        }
    }
    
    public class App : Application
    {
        public App()
        {
            MainPage = new MainPage();
        }
    }
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            
            var app = builder.Build();
            app.Run(args);
        }
    }
}
```

### 4.2 多窗口应用

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Maui@9.0.0
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Maui.Applications;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace UI.Desktop.MultiWindow
{
    public class MainPage : ContentPage
    {
        public MainPage()
        {
            Title = "Main Window";
            
            var label = new Label
            {
                Text = "Main Window Content",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                FontSize = 24
            };
            
            var openWindowButton = new Button
            {
                Text = "Open New Window",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            
            openWindowButton.Clicked += async (sender, e) =>
            {
                var newWindow = new Window(new SecondPage())
                {
                    Title = "Second Window"
                };
                
                Application.Current.OpenWindow(newWindow);
            };
            
            Content = new StackLayout
            {
                Children = {
                    label,
                    openWindowButton
                },
                Padding = new Thickness(20)
            };
        }
    }
    
    public class SecondPage : ContentPage
    {
        public SecondPage()
        {
            Title = "Second Window";
            
            var label = new Label
            {
                Text = "Second Window Content",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                FontSize = 24
            };
            
            var closeWindowButton = new Button
            {
                Text = "Close Window",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            
            closeWindowButton.Clicked += (sender, e) =>
            {
                Application.Current.CloseWindow(Application.Current.Windows.Last());
            };
            
            Content = new StackLayout
            {
                Children = {
                    label,
                    closeWindowButton
                },
                Padding = new Thickness(20)
            };
        }
    }
    
    public class App : Application
    {
        public App()
        {
            MainPage = new MainPage();
        }
    }
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            
            var app = builder.Build();
            app.Run(args);
        }
    }
}
```

### 4.3 系统集成

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Maui@9.0.0
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Maui.Applications;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Essentials;

namespace UI.Desktop.SystemIntegration
{
    public class MainPage : ContentPage
    {
        public MainPage()
        {
            Title = "System Integration";
            
            var label = new Label
            {
                Text = "System Integration Examples",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                FontSize = 24
            };
            
            var batteryButton = new Button
            {
                Text = "Check Battery",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            
            batteryButton.Clicked += async (sender, e) =>
            {
                var level = Battery.ChargeLevel * 100;
                var state = Battery.State;
                await DisplayAlert("Battery Status", $"Level: {level:F1}%\nState: {state}", "OK");
            };
            
            var networkButton = new Button
            {
                Text = "Check Network",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            
            networkButton.Clicked += async (sender, e) =>
            {
                var profile = Connectivity.NetworkAccess;
                await DisplayAlert("Network Status", $"Network Access: {profile}", "OK");
            };
            
            Content = new StackLayout
            {
                Children = {
                    label,
                    batteryButton,
                    networkButton
                },
                Padding = new Thickness(20)
            };
        }
    }
    
    public class App : Application
    {
        public App()
        {
            MainPage = new MainPage();
        }
    }
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            
            var app = builder.Build();
            app.Run(args);
        }
    }
}
```

## 5. 移动 UI 示例

### 5.1 基本移动应用

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Maui@9.0.0
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Maui.Applications;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace UI.Mobile.Basic
{
    public class MainPage : ContentPage
    {
        public MainPage()
        {
            Title = "Basic Mobile App";
            
            var label = new Label
            {
                Text = "Welcome to Mobile App",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                FontSize = 24
            };
            
            var button = new Button
            {
                Text = "Click Me",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            
            button.Clicked += (sender, e) =>
            {
                label.Text = "Button clicked!";
            };
            
            Content = new StackLayout
            {
                Children = {
                    label,
                    button
                },
                Padding = new Thickness(20)
            };
        }
    }
    
    public class App : Application
    {
        public App()
        {
            MainPage = new MainPage();
        }
    }
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            
            var app = builder.Build();
            app.Run(args);
        }
    }
}
```

### 5.2 触摸优化

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Maui@9.0.0
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Maui.Applications;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace UI.Mobile.TouchOptimization
{
    public class MainPage : ContentPage
    {
        private int tapCount = 0;
        private double lastScale = 1;
        private Point lastPoint;
        
        public MainPage()
        {
            Title = "Touch Optimization";
            
            var label = new Label
            {
                Text = "Touch Examples",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                FontSize = 24
            };
            
            var tapLabel = new Label
            {
                Text = "Tap count: 0",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            
            var touchView = new Frame
            {
                BackgroundColor = Colors.LightBlue,
                WidthRequest = 200,
                HeightRequest = 200,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Content = new Label
                {
                    Text = "Tap, Pinch, Pan",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };
            
            // 点击手势
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (sender, e) =>
            {
                tapCount++;
                tapLabel.Text = $"Tap count: {tapCount}";
            };
            touchView.GestureRecognizers.Add(tapGesture);
            
            // 捏合手势
            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += (sender, e) =>
            {
                if (e.Status == GestureStatus.Started)
                {
                    lastScale = touchView.Scale;
                }
                touchView.Scale = lastScale * e.Scale;
            };
            touchView.GestureRecognizers.Add(pinchGesture);
            
            // 平移手势
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (sender, e) =>
            {
                if (e.Status == GestureStatus.Running)
                {
                    touchView.TranslationX += e.TotalX;
                    touchView.TranslationY += e.TotalY;
                }
                else if (e.Status == GestureStatus.Completed)
                {
                    touchView.TranslationX = 0;
                    touchView.TranslationY = 0;
                }
            };
            touchView.GestureRecognizers.Add(panGesture);
            
            Content = new StackLayout
            {
                Children = {
                    label,
                    tapLabel,
                    touchView
                },
                Padding = new Thickness(20)
            };
        }
    }
    
    public class App : Application
    {
        public App()
        {
            MainPage = new MainPage();
        }
    }
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            
            var app = builder.Build();
            app.Run(args);
        }
    }
}
```

### 5.3 设备硬件访问

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Maui@9.0.0
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Maui.Applications;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Essentials;

namespace UI.Mobile.HardwareAccess
{
    public class MainPage : ContentPage
    {
        public MainPage()
        {
            Title = "Hardware Access";
            
            var label = new Label
            {
                Text = "Hardware Access Examples",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                FontSize = 24
            };
            
            var cameraButton = new Button
            {
                Text = "Take Photo",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            
            cameraButton.Clicked += async (sender, e) =>
            {
                try
                {
                    var photo = await MediaPicker.CapturePhotoAsync();
                    if (photo != null)
                    {
                        await DisplayAlert("Photo Taken", "Photo captured successfully!", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Error taking photo: {ex.Message}", "OK");
                }
            };
            
            var locationButton = new Button
            {
                Text = "Get Location",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            
            locationButton.Clicked += async (sender, e) =>
            {
                try
                {
                    var location = await Geolocation.GetLastKnownLocationAsync();
                    if (location != null)
                    {
                        await DisplayAlert("Location", $"Latitude: {location.Latitude}\nLongitude: {location.Longitude}", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Unable to get location", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Error getting location: {ex.Message}", "OK");
                }
            };
            
            Content = new StackLayout
            {
                Children = {
                    label,
                    cameraButton,
                    locationButton
                },
                Padding = new Thickness(20)
            };
        }
    }
    
    public class App : Application
    {
        public App()
        {
            MainPage = new MainPage();
        }
    }
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            
            var app = builder.Build();
            app.Run(args);
        }
    }
}
```

## 5. 主题管理示例

### 5.1 基本主题使用

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UI.Theme.Basic
{
    public enum Theme
    {
        Light,
        Dark,
        System
    }
    
    public class ThemeChangedEventArgs : EventArgs
    {
        public Theme OldTheme { get; set; }
        public Theme NewTheme { get; set; }
    }
    
    public interface IThemeManager
    {
        Theme CurrentTheme { get; }
        Task SetThemeAsync(Theme theme);
        Task ToggleThemeAsync();
        event EventHandler<ThemeChangedEventArgs> ThemeChanged;
    }
    
    public class ThemeManager : IThemeManager
    {
        private readonly ILogger<ThemeManager> _logger;
        private Theme _currentTheme;
        
        public Theme CurrentTheme => _currentTheme;
        public event EventHandler<ThemeChangedEventArgs> ThemeChanged;
        
        public ThemeManager(ILogger<ThemeManager> logger)
        {
            _logger = logger;
            _currentTheme = Theme.Light;
            _logger.LogInformation("Theme manager initialized with theme: {Theme}", _currentTheme);
        }
        
        public async Task SetThemeAsync(Theme theme)
        {
            if (_currentTheme == theme)
                return;
            
            var oldTheme = _currentTheme;
            _currentTheme = theme;
            _logger.LogInformation("Theme changed from {OldTheme} to {NewTheme}", oldTheme, _currentTheme);
            
            // 模拟保存主题设置
            await Task.Delay(50);
            
            // 触发主题变更事件
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs { OldTheme = oldTheme, NewTheme = _currentTheme });
        }
        
        public async Task ToggleThemeAsync()
        {
            var newTheme = _currentTheme == Theme.Light ? Theme.Dark : Theme.Light;
            await SetThemeAsync(newTheme);
        }
    }
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建服务容器
            var services = new ServiceCollection();
            
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
            
            // 添加主题管理器
            services.AddSingleton<IThemeManager, ThemeManager>();
            
            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();
            
            var themeManager = serviceProvider.GetRequiredService<IThemeManager>();
            
            // 监听主题变更
            themeManager.ThemeChanged += (sender, e) =>
            {
                Console.WriteLine($"Theme changed from {e.OldTheme} to {e.NewTheme}");
            };
            
            Console.WriteLine($"Current theme: {themeManager.CurrentTheme}");
            
            // 切换主题
            await themeManager.ToggleThemeAsync();
            Console.WriteLine($"Current theme: {themeManager.CurrentTheme}");
            
            // 设置特定主题
            await themeManager.SetThemeAsync(Theme.Dark);
            Console.WriteLine($"Current theme: {themeManager.CurrentTheme}");
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
```

### 5.2 自定义主题

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UI.Theme.Custom
{
    public enum Theme
    {
        Light,
        Dark,
        System,
        Custom
    }
    
    public class ThemeColors
    {
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
    }
    
    public class ThemeChangedEventArgs : EventArgs
    {
        public Theme OldTheme { get; set; }
        public Theme NewTheme { get; set; }
        public ThemeColors NewColors { get; set; }
    }
    
    public interface IThemeManager
    {
        Theme CurrentTheme { get; }
        ThemeColors CurrentColors { get; }
        Task SetThemeAsync(Theme theme);
        Task SetCustomThemeAsync(ThemeColors colors);
        Task ToggleThemeAsync();
        event EventHandler<ThemeChangedEventArgs> ThemeChanged;
    }
    
    public class ThemeManager : IThemeManager
    {
        private readonly ILogger<ThemeManager> _logger;
        private Theme _currentTheme;
        private ThemeColors _currentColors;
        private readonly Dictionary<Theme, ThemeColors> _themeColors = new Dictionary<Theme, ThemeColors>
        {
            { Theme.Light, new ThemeColors { PrimaryColor = "#007bff", SecondaryColor = "#6c757d", BackgroundColor = "#ffffff", TextColor = "#000000" } },
            { Theme.Dark, new ThemeColors { PrimaryColor = "#0069d9", SecondaryColor = "#6c757d", BackgroundColor = "#343a40", TextColor = "#ffffff" } },
            { Theme.System, new ThemeColors { PrimaryColor = "#007bff", SecondaryColor = "#6c757d", BackgroundColor = "#ffffff", TextColor = "#000000" } }
        };
        
        public Theme CurrentTheme => _currentTheme;
        public ThemeColors CurrentColors => _currentColors;
        public event EventHandler<ThemeChangedEventArgs> ThemeChanged;
        
        public ThemeManager(ILogger<ThemeManager> logger)
        {
            _logger = logger;
            _currentTheme = Theme.Light;
            _currentColors = _themeColors[_currentTheme];
            _logger.LogInformation("Theme manager initialized with theme: {Theme}", _currentTheme);
        }
        
        public async Task SetThemeAsync(Theme theme)
        {
            if (_currentTheme == theme)
                return;
            
            var oldTheme = _currentTheme;
            _currentTheme = theme;
            
            // 如果不是自定义主题，使用预定义颜色
            if (theme != Theme.Custom && _themeColors.TryGetValue(theme, out var colors))
            {
                _currentColors = colors;
            }
            
            _logger.LogInformation("Theme changed from {OldTheme} to {NewTheme}", oldTheme, _currentTheme);
            
            // 模拟保存主题设置
            await Task.Delay(50);
            
            // 触发主题变更事件
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs { OldTheme = oldTheme, NewTheme = _currentTheme, NewColors = _currentColors });
        }
        
        public async Task SetCustomThemeAsync(ThemeColors colors)
        {
            var oldTheme = _currentTheme;
            _currentTheme = Theme.Custom;
            _currentColors = colors;
            
            _logger.LogInformation("Custom theme set");
            
            // 模拟保存主题设置
            await Task.Delay(50);
            
            // 触发主题变更事件
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs { OldTheme = oldTheme, NewTheme = _currentTheme, NewColors = _currentColors });
        }
        
        public async Task ToggleThemeAsync()
        {
            var newTheme = _currentTheme == Theme.Light ? Theme.Dark : Theme.Light;
            await SetThemeAsync(newTheme);
        }
    }
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建服务容器
            var services = new ServiceCollection();
            
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
            
            // 添加主题管理器
            services.AddSingleton<IThemeManager, ThemeManager>();
            
            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();
            
            var themeManager = serviceProvider.GetRequiredService<IThemeManager>();
            
            // 监听主题变更
            themeManager.ThemeChanged += (sender, e) =>
            {
                Console.WriteLine($"Theme changed from {e.OldTheme} to {e.NewTheme}");
                Console.WriteLine($"New colors: Primary={e.NewColors.PrimaryColor}, Background={e.NewColors.BackgroundColor}");
            };
            
            Console.WriteLine($"Current theme: {themeManager.CurrentTheme}");
            Console.WriteLine($"Current colors: Primary={themeManager.CurrentColors.PrimaryColor}, Background={themeManager.CurrentColors.BackgroundColor}");
            
            // 切换主题
            await themeManager.ToggleThemeAsync();
            
            // 设置自定义主题
            var customColors = new ThemeColors
            {
                PrimaryColor = "#ff6b6b",
                SecondaryColor = "#4ecdc4",
                BackgroundColor = "#f7fff7",
                TextColor = "#2f2f2f"
            };
            await themeManager.SetCustomThemeAsync(customColors);
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
```

## 6. 本地化示例

### 6.1 基本本地化

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UI.Localization.Basic
{
    public class CultureChangedEventArgs : EventArgs
    {
        public string OldCulture { get; set; }
        public string NewCulture { get; set; }
    }
    
    public interface ILocalizationManager
    {
        string CurrentCulture { get; }
        Task SetCultureAsync(string culture);
        string GetString(string key);
        event EventHandler<CultureChangedEventArgs> CultureChanged;
    }
    
    public class LocalizationManager : ILocalizationManager
    {
        private readonly ILogger<LocalizationManager> _logger;
        private string _currentCulture;
        private readonly Dictionary<string, Dictionary<string, string>> _localizedStrings = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "en-US", new Dictionary<string, string>
                {
                    { "Welcome", "Welcome to the application" },
                    { "Hello", "Hello" },
                    { "Goodbye", "Goodbye" },
                    { "ButtonText", "Click Me" },
                    { "Message", "This is a localized message" }
                }
            },
            {
                "zh-CN", new Dictionary<string, string>
                {
                    { "Welcome", "欢迎使用应用程序" },
                    { "Hello", "你好" },
                    { "Goodbye", "再见" },
                    { "ButtonText", "点击我" },
                    { "Message", "这是一条本地化消息" }
                }
            },
            {
                "fr-FR", new Dictionary<string, string>
                {
                    { "Welcome", "Bienvenue dans l'application" },
                    { "Hello", "Bonjour" },
                    { "Goodbye", "Au revoir" },
                    { "ButtonText", "Cliquez-moi" },
                    { "Message", "Ceci est un message localisé" }
                }
            }
        };
        
        public string CurrentCulture => _currentCulture;
        public event EventHandler<CultureChangedEventArgs> CultureChanged;
        
        public LocalizationManager(ILogger<LocalizationManager> logger)
        {
            _logger = logger;
            _currentCulture = "en-US";
            _logger.LogInformation("Localization manager initialized with culture: {Culture}", _currentCulture);
        }
        
        public async Task SetCultureAsync(string culture)
        {
            if (_currentCulture == culture)
                return;
            
            var oldCulture = _currentCulture;
            _currentCulture = culture;
            _logger.LogInformation("Culture changed from {OldCulture} to {NewCulture}", oldCulture, _currentCulture);
            
            // 模拟保存文化设置
            await Task.Delay(50);
            
            // 触发文化变更事件
            CultureChanged?.Invoke(this, new CultureChangedEventArgs { OldCulture = oldCulture, NewCulture = _currentCulture });
        }
        
        public string GetString(string key)
        {
            if (_localizedStrings.TryGetValue(_currentCulture, out var cultureStrings))
            {
                if (cultureStrings.TryGetValue(key, out var value))
                {
                    return value;
                }
            }
            
            // 回退到英文
            if (_localizedStrings.TryGetValue("en-US", out var defaultStrings))
            {
                if (defaultStrings.TryGetValue(key, out var defaultValue))
                {
                    return defaultValue;
                }
            }
            
            return key; // 最终回退到键名
        }
    }
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建服务容器
            var services = new ServiceCollection();
            
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
            
            // 添加本地化管理器
            services.AddSingleton<ILocalizationManager, LocalizationManager>();
            
            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();
            
            var localizationManager = serviceProvider.GetRequiredService<ILocalizationManager>();
            
            // 监听文化变更
            localizationManager.CultureChanged += (sender, e) =>
            {
                Console.WriteLine($"Culture changed from {e.OldCulture} to {e.NewCulture}");
                DisplayLocalizedStrings(localizationManager);
            };
            
            Console.WriteLine("=== English (en-US) ===");
            DisplayLocalizedStrings(localizationManager);
            
            // 切换到中文
            Console.WriteLine("\n=== Changing to Chinese (zh-CN) ===");
            await localizationManager.SetCultureAsync("zh-CN");
            
            // 切换到法语
            Console.WriteLine("\n=== Changing to French (fr-FR) ===");
            await localizationManager.SetCultureAsync("fr-FR");
            
            // 切换到不存在的语言（应该回退到英文）
            Console.WriteLine("\n=== Changing to Non-existent Language (de-DE) ===");
            await localizationManager.SetCultureAsync("de-DE");
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        
        private static void DisplayLocalizedStrings(ILocalizationManager localizationManager)
        {
            Console.WriteLine($"Current culture: {localizationManager.CurrentCulture}");
            Console.WriteLine($"Welcome: {localizationManager.GetString("Welcome")}");
            Console.WriteLine($"Hello: {localizationManager.GetString("Hello")}");
            Console.WriteLine($"Goodbye: {localizationManager.GetString("Goodbye")}");
            Console.WriteLine($"ButtonText: {localizationManager.GetString("ButtonText")}");
            Console.WriteLine($"Message: {localizationManager.GetString("Message")}");
        }
    }
}
```

### 6.2 资源管理

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:package Microsoft.Extensions.Configuration@9.0.0
#:package Microsoft.Extensions.Configuration.Json@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace UI.Localization.ResourceManagement
{
    public class CultureChangedEventArgs : EventArgs
    {
        public string OldCulture { get; set; }
        public string NewCulture { get; set; }
    }
    
    public interface ILocalizationManager
    {
        string CurrentCulture { get; }
        Task SetCultureAsync(string culture);
        string GetString(string key);
        event EventHandler<CultureChangedEventArgs> CultureChanged;
    }
    
    public class LocalizationManager : ILocalizationManager
    {
        private readonly ILogger<LocalizationManager> _logger;
        private readonly IConfiguration _configuration;
        private string _currentCulture;
        private readonly Dictionary<string, Dictionary<string, string>> _localizedStrings = new Dictionary<string, Dictionary<string, string>>();
        
        public string CurrentCulture => _currentCulture;
        public event EventHandler<CultureChangedEventArgs> CultureChanged;
        
        public LocalizationManager(ILogger<LocalizationManager> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _currentCulture = _configuration["Localization:DefaultCulture"] ?? "en-US";
            
            // 加载本地化资源
            LoadLocalizationResources();
            
            _logger.LogInformation("Localization manager initialized with culture: {Culture}", _currentCulture);
        }
        
        private void LoadLocalizationResources()
        {
            // 模拟从文件加载资源
            _localizedStrings["en-US"] = new Dictionary<string, string>
            {
                { "Welcome", "Welcome to the application" },
                { "Hello", "Hello" },
                { "Goodbye", "Goodbye" }
            };
            
            _localizedStrings["zh-CN"] = new Dictionary<string, string>
            {
                { "Welcome", "欢迎使用应用程序" },
                { "Hello", "你好" },
                { "Goodbye", "再见" }
            };
            
            // 实际项目中，这里可以从JSON文件、数据库或其他存储中加载资源
            _logger.LogInformation("Localization resources loaded");
        }
        
        public async Task SetCultureAsync(string culture)
        {
            if (_currentCulture == culture)
                return;
            
            var oldCulture = _currentCulture;
            _currentCulture = culture;
            _logger.LogInformation("Culture changed from {OldCulture} to {NewCulture}", oldCulture, _currentCulture);
            
            // 模拟保存文化设置
            await Task.Delay(50);
            
            // 触发文化变更事件
            CultureChanged?.Invoke(this, new CultureChangedEventArgs { OldCulture = oldCulture, NewCulture = _currentCulture });
        }
        
        public string GetString(string key)
        {
            if (_localizedStrings.TryGetValue(_currentCulture, out var cultureStrings))
            {
                if (cultureStrings.TryGetValue(key, out var value))
                {
                    return value;
                }
            }
            
            // 回退到默认文化
            var defaultCulture = _configuration["Localization:DefaultCulture"] ?? "en-US";
            if (_localizedStrings.TryGetValue(defaultCulture, out var defaultStrings))
            {
                if (defaultStrings.TryGetValue(key, out var defaultValue))
                {
                    return defaultValue;
                }
            }
            
            return key; // 最终回退到键名
        }
    }
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建配置
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Localization:DefaultCulture", "en-US" },
                    { "Localization:SupportedCultures", "en-US,zh-CN,fr-FR" }
                })
                .Build();
            
            // 构建服务容器
            var services = new ServiceCollection();
            
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
            
            // 添加配置
            services.AddSingleton<IConfiguration>(configuration);
            
            // 添加本地化管理器
            services.AddSingleton<ILocalizationManager, LocalizationManager>();
            
            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();
            
            var localizationManager = serviceProvider.GetRequiredService<ILocalizationManager>();
            
            Console.WriteLine("=== Default Culture ===");
            Console.WriteLine($"Current culture: {localizationManager.CurrentCulture}");
            Console.WriteLine($"Welcome: {localizationManager.GetString("Welcome")}");
            
            // 切换到中文
            Console.WriteLine("\n=== Changing to Chinese ===");
            await localizationManager.SetCultureAsync("zh-CN");
            Console.WriteLine($"Welcome: {localizationManager.GetString("Welcome")}");
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
```

## 7. 性能监控示例

### 7.1 基本监控

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UI.Performance.Basic
{
    public class PerformanceMetrics
    {
        public string OperationName { get; set; }
        public long ExecutionTimeMs { get; set; }
        public DateTime Timestamp { get; set; }
        public long MemoryUsedBytes { get; set; }
    }
    
    public interface IPerformanceMonitor
    {
        void StartMeasurement(string operationName);
        void StopMeasurement(string operationName);
        IReadOnlyDictionary<string, PerformanceMetrics> GetMetrics();
        void ClearMetrics();
    }
    
    public class PerformanceMonitor : IPerformanceMonitor
    {
        private readonly ILogger<PerformanceMonitor> _logger;
        private readonly Dictionary<string, (long StartTime, long StartMemory)> _activeMeasurements = new Dictionary<string, (long, long)>();
        private readonly Dictionary<string, PerformanceMetrics> _metrics = new Dictionary<string, PerformanceMetrics>();
        private readonly Process _currentProcess = Process.GetCurrentProcess();
        
        public PerformanceMonitor(ILogger<PerformanceMonitor> logger)
        {
            _logger = logger;
        }
        
        public void StartMeasurement(string operationName)
        {
            _currentProcess.Refresh();
            var startMemory = _currentProcess.WorkingSet64;
            _activeMeasurements[operationName] = (DateTime.UtcNow.Ticks, startMemory);
            _logger.LogDebug("Started performance measurement for: {Operation}", operationName);
        }
        
        public void StopMeasurement(string operationName)
        {
            if (_activeMeasurements.TryGetValue(operationName, out var measurement))
            {
                _currentProcess.Refresh();
                var endMemory = _currentProcess.WorkingSet64;
                var endTime = DateTime.UtcNow.Ticks;
                var executionTimeMs = (endTime - measurement.StartTime) / TimeSpan.TicksPerMillisecond;
                var memoryUsedBytes = endMemory - measurement.StartMemory;
                
                _activeMeasurements.Remove(operationName);
                
                _metrics[operationName] = new PerformanceMetrics
                {
                    OperationName = operationName,
                    ExecutionTimeMs = executionTimeMs,
                    Timestamp = DateTime.UtcNow,
                    MemoryUsedBytes = memoryUsedBytes
                };
                
                _logger.LogDebug("Stopped performance measurement for: {Operation}, Execution time: {Time}ms, Memory used: {Memory} bytes", 
                    operationName, executionTimeMs, memoryUsedBytes);
            }
        }
        
        public IReadOnlyDictionary<string, PerformanceMetrics> GetMetrics()
        {
            return _metrics;
        }
        
        public void ClearMetrics()
        {
            _metrics.Clear();
            _activeMeasurements.Clear();
            _logger.LogDebug("Performance metrics cleared");
        }
    }
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建服务容器
            var services = new ServiceCollection();
            
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            
            // 添加性能监控器
            services.AddSingleton<IPerformanceMonitor, PerformanceMonitor>();
            
            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();
            
            var performanceMonitor = serviceProvider.GetRequiredService<IPerformanceMonitor>();
            
            // 测量操作1
            performanceMonitor.StartMeasurement("Operation1");
            await Task.Delay(100); // 模拟耗时操作
            performanceMonitor.StopMeasurement("Operation1");
            
            // 测量操作2
            performanceMonitor.StartMeasurement("Operation2");
            await Task.Delay(200); // 模拟耗时操作
            performanceMonitor.StopMeasurement("Operation2");
            
            // 测量操作3
            performanceMonitor.StartMeasurement("Operation3");
            // 模拟内存使用
            var list = new List<byte[]>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(new byte[1024]);
            }
            await Task.Delay(50);
            performanceMonitor.StopMeasurement("Operation3");
            
            // 显示性能指标
            Console.WriteLine("=== Performance Metrics ===");
            var metrics = performanceMonitor.GetMetrics();
            foreach (var metric in metrics)
            {
                Console.WriteLine($"Operation: {metric.Key}");
                Console.WriteLine($"  Execution Time: {metric.Value.ExecutionTimeMs}ms");
                Console.WriteLine($"  Memory Used: {metric.Value.MemoryUsedBytes} bytes");
                Console.WriteLine($"  Timestamp: {metric.Value.Timestamp}");
                Console.WriteLine();
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
```

### 7.2 性能优化

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UI.Performance.Optimization
{
    public class PerformanceMonitor
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        
        public void Start()
        {
            _stopwatch.Reset();
            _stopwatch.Start();
        }
        
        public void Stop()
        {
            _stopwatch.Stop();
        }
        
        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;
        public long ElapsedTicks => _stopwatch.ElapsedTicks;
    }
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var monitor = new PerformanceMonitor();
            
            // 测试字符串拼接
            Console.WriteLine("=== String Concatenation Test ===");
            
            // 传统字符串拼接
            monitor.Start();
            string result1 = "";
            for (int i = 0; i < 10000; i++)
            {
                result1 += "Test " + i + " ";
            }
            monitor.Stop();
            Console.WriteLine($"Traditional concatenation: {monitor.ElapsedMilliseconds}ms");
            
            // StringBuilder
            monitor.Start();
            var sb = new StringBuilder();
            for (int i = 0; i < 10000; i++)
            {
                sb.Append("Test ").Append(i).Append(" ");
            }
            string result2 = sb.ToString();
            monitor.Stop();
            Console.WriteLine($"StringBuilder: {monitor.ElapsedMilliseconds}ms");
            
            // 测试列表操作
            Console.WriteLine("\n=== List Operation Test ===");
            
            // 未预分配容量
            monitor.Start();
            var list1 = new List<int>();
            for (int i = 0; i < 100000; i++)
            {
                list1.Add(i);
            }
            monitor.Stop();
            Console.WriteLine($"List without capacity: {monitor.ElapsedMilliseconds}ms");
            
            // 预分配容量
            monitor.Start();
            var list2 = new List<int>(100000);
            for (int i = 0; i < 100000; i++)
            {
                list2.Add(i);
            }
            monitor.Stop();
            Console.WriteLine($"List with capacity: {monitor.ElapsedMilliseconds}ms");
            
            // 测试异步操作
            Console.WriteLine("\n=== Async Operation Test ===");
            
            // 串行执行
            monitor.Start();
            await Task.Delay(100);
            await Task.Delay(100);
            await Task.Delay(100);
            monitor.Stop();
            Console.WriteLine($"Serial execution: {monitor.ElapsedMilliseconds}ms");
            
            // 并行执行
            monitor.Start();
            await Task.WhenAll(
                Task.Delay(100),
                Task.Delay(100),
                Task.Delay(100)
            );
            monitor.Stop();
            Console.WriteLine($"Parallel execution: {monitor.ElapsedMilliseconds}ms");
            
            // 测试内存优化
            Console.WriteLine("\n=== Memory Optimization Test ===");
            
            // 测试对象池
            var objectPool = new ObjectPool<StringBuilder>(() => new StringBuilder(), 10);
            
            monitor.Start();
            for (int i = 0; i < 10000; i++)
            {
                using (var sbPooled = objectPool.Get())
                {
                    sbPooled.Item.Append("Test ").Append(i);
                    var str = sbPooled.Item.ToString();
                }
            }
            monitor.Stop();
            Console.WriteLine($"Object pool: {monitor.ElapsedMilliseconds}ms");
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
    
    public class ObjectPool<T> where T : class
    {
        private readonly Func<T> _objectFactory;
        private readonly Queue<T> _pool;
        private readonly int _maxSize;
        
        public ObjectPool(Func<T> objectFactory, int maxSize = 100)
        {
            _objectFactory = objectFactory;
            _pool = new Queue<T>(maxSize);
            _maxSize = maxSize;
        }
        
        public PooledObject Get()
        {
            if (_pool.Count > 0)
            {
                return new PooledObject(this, _pool.Dequeue());
            }
            return new PooledObject(this, _objectFactory());
        }
        
        public void Return(T item)
        {
            if (_pool.Count < _maxSize)
            {
                _pool.Enqueue(item);
            }
        }
        
        public class PooledObject : IDisposable
        {
            private readonly ObjectPool<T> _pool;
            public T Item { get; }
            
            public PooledObject(ObjectPool<T> pool, T item)
            {
                _pool = pool;
                Item = item;
            }
            
            public void Dispose()
            {
                _pool.Return(Item);
            }
        }
    }
}
```

### 7.3 内存优化

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Buffers;
using System.Text;

namespace UI.Performance.MemoryOptimization
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Memory Optimization Examples ===");
            
            // 测试 ArrayPool
            TestArrayPool();
            
            // 测试 String.Create
            TestStringCreate();
            
            // 测试 Span<T>
            TestSpan();
            
            // 测试 Memory<T>
            TestMemory();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        
        private static void TestArrayPool()
        {
            Console.WriteLine("\n=== ArrayPool Test ===");
            
            // 从池中获取数组
            var pool = ArrayPool<byte>.Shared;
            byte[] buffer = pool.Rent(1024);
            
            try
            {
                // 使用缓冲区
                for (int i = 0; i < 1024; i++)
                {
                    buffer[i] = (byte)(i % 256);
                }
                
                Console.WriteLine($"ArrayPool: Buffer rented and used successfully");
            }
            finally
            {
                // 归还缓冲区到池
                pool.Return(buffer);
                Console.WriteLine($"ArrayPool: Buffer returned to pool");
            }
        }
        
        private static void TestStringCreate()
        {
            Console.WriteLine("\n=== String.Create Test ===");
            
            int value = 42;
            string result = string.Create(10, value, (span, v) =>
            {
                // 直接写入字符到 Span<char>
                span[0] = 'T';
                span[1] = 'h';
                span[2] = 'e';
                span[3] = ' ';
                span[4] = 'v';
                span[5] = 'a';
                span[6] = 'l';
                span[7] = 'u';
                span[8] = 'e';
                span[9] = ' ';
                // 注意：这里简化了，实际应该正确处理字符串长度
            });
            
            Console.WriteLine($"String.Create: Result = {result}");
        }
        
        private static void TestSpan()
        {
            Console.WriteLine("\n=== Span<T> Test ===");
            
            // 创建数组
            int[] numbers = { 1, 2, 3, 4, 5 };
            
            // 创建 Span<int>
            Span<int> span = numbers;
            
            // 修改 Span 中的值
            span[0] = 100;
            
            // 验证原数组也被修改
            Console.WriteLine($"Span<T>: Original array[0] = {numbers[0]}");
            
            // 切片操作
            Span<int> slice = span.Slice(1, 3);
            Console.WriteLine($"Span<T>: Slice values: {slice[0]}, {slice[1]}, {slice[2]}");
        }
        
        private static void TestMemory()
        {
            Console.WriteLine("\n=== Memory<T> Test ===");
            
            // 创建数组
            int[] numbers = { 1, 2, 3, 4, 5 };
            
            // 创建 Memory<int>
            Memory<int> memory = numbers;
            
            // 获取 Span 进行修改
            Span<int> span = memory.Span;
            span[0] = 200;
            
            // 验证原数组也被修改
            Console.WriteLine($"Memory<T>: Original array[0] = {numbers[0]}");
            
            // 切片操作
            Memory<int> slice = memory.Slice(1, 3);
            Span<int> sliceSpan = slice.Span;
            Console.WriteLine($"Memory<T>: Slice values: {sliceSpan[0]}, {sliceSpan[1]}, {sliceSpan[2]}");
        }
        
        private static void TestStackAllocSpan()
        {
            Console.WriteLine("\n=== StackAlloc + Span<T> Test ===");
            
            // 使用 stackalloc 分配栈内存并创建 Span
            Span<int> stackSpan = stackalloc int[5];
            
            // 填充数据
            for (int i = 0; i < stackSpan.Length; i++)
            {
                stackSpan[i] = i * 10;
            }
            
            // 读取数据
            Console.Write("StackAlloc Span values: ");
            for (int i = 0; i < stackSpan.Length; i++)
            {
                Console.Write($"{stackSpan[i]} ");
            }
            Console.WriteLine();
        }
        
        private static void TestObjectPool()
        {
            Console.WriteLine("\n=== ObjectPool Test ===");
            
            // 创建对象池
            ObjectPool<StringBuilder> pool = ObjectPool.Create<StringBuilder>();
            
            // 从池中获取对象
            StringBuilder sb = pool.Get();
            
            try
            {
                // 使用对象
                sb.Append("Hello, ");
                sb.Append("ObjectPool!");
                Console.WriteLine($"ObjectPool: {sb.ToString()}");
            }
            finally
            {
                // 归还对象到池中
                pool.Return(sb);
            }
        }
        
        private static void TestThreadLocalSpan()
        {
            Console.WriteLine("\n=== ThreadLocal<Span<T>> Test ===");
            
            // 创建线程本地的 Span
            ThreadLocal<Span<int>> threadLocalSpan = new ThreadLocal<Span<int>>(() =>
            {
                // 为每个线程分配栈内存
                return stackalloc int[10];
            });
            
            // 启动多个线程
            Task[] tasks = new Task[3];
            for (int i = 0; i < tasks.Length; i++)
            {
                int taskId = i;
                tasks[i] = Task.Run(() =>
                {
                    Span<int> span = threadLocalSpan.Value;
                    // 填充数据
                    for (int j = 0; j < span.Length; j++)
                    {
                        span[j] = taskId * 100 + j;
                    }
                    // 读取数据
                    Console.Write($"Thread {taskId} span values: ");
                    for (int j = 0; j < 5; j++) // 只打印前5个
                    {
                        Console.Write($"{span[j]} ");
                    }
                    Console.WriteLine();
                });
            }
            
            Task.WaitAll(tasks);
            
            // 清理
            threadLocalSpan.Dispose();
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("=== High Performance Memory Management Demo ===");
            
            TestStringCreate();
            TestSpan();
            TestMemory();
            TestStackAllocSpan();
            TestObjectPool();
            TestThreadLocalSpan();
            
            Console.WriteLine("\nDemo completed.");
        }
    }
}
```

## 8. 无障碍支持示例

### 8.1 基本无障碍检查

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UI.Accessibility.Basic
{
    public enum AccessibilityStandard
    {
        WCAG21AA,
        WCAG21AAA,
        Section508
    }
    
    public class AccessibilityReport
    {
        public bool IsCompliant { get; set; }
        public List<string> Issues { get; set; } = new List<string>();
        public AccessibilityStandard Standard { get; set; }
    }
    
    public interface IAccessibilityChecker
    {
        Task<AccessibilityReport> CheckAsync();
        bool IsCompliant(AccessibilityStandard standard);
    }
    
    public class AccessibilityChecker : IAccessibilityChecker
    {
        private readonly ILogger<AccessibilityChecker> _logger;
        private readonly List<string> _issues = new List<string>();
        
        public AccessibilityChecker(ILogger<AccessibilityChecker> logger)
        {
            _logger = logger;
        }
        
        public async Task<AccessibilityReport> CheckAsync()
        {
            _logger.LogInformation("Running accessibility check");
            
            // 模拟无障碍检查过程
            await Task.Delay(200);
            
            // 模拟检查结果
            _issues.Clear();
            _issues.Add("Button missing aria-label");
            _issues.Add("Image missing alt text");
            _issues.Add("Insufficient color contrast");
            
            var report = new AccessibilityReport
            {
                IsCompliant = _issues.Count == 0,
                Issues = new List<string>(_issues),
                Standard = AccessibilityStandard.WCAG21AA
            };
            
            if (!report.IsCompliant)
            {
                _logger.LogWarning("Accessibility issues found: {Count}", _issues.Count);
            }
            else
            {
                _logger.LogInformation("Accessibility check passed");
            }
            
            return report;
        }
        
        public bool IsCompliant(AccessibilityStandard standard)
        {
            _logger.LogInformation("Checking compliance with {Standard}", standard);
            // 这里应该根据实际检查结果返回是否符合标准
            return _issues.Count == 0;
        }
    }
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建服务容器
            var services = new ServiceCollection();
            
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
            
            // 添加无障碍检查器
            services.AddSingleton<IAccessibilityChecker, AccessibilityChecker>();
            
            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();
            
            var accessibilityChecker = serviceProvider.GetRequiredService<IAccessibilityChecker>();
            
            // 运行无障碍检查
            var report = await accessibilityChecker.CheckAsync();
            
            Console.WriteLine("=== Accessibility Check Report ===");
            Console.WriteLine($"Standard: {report.Standard}");
            Console.WriteLine($"Is Compliant: {report.IsCompliant}");
            
            if (!report.IsCompliant)
            {
                Console.WriteLine("Issues found:");
                foreach (var issue in report.Issues)
                {
                    Console.WriteLine($"  - {issue}");
                }
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
```

### 8.2 无障碍 UI 设计

```razor
@* Pages/AccessiblePage.razor *@
@page "/accessible"
@using Microsoft.AspNetCore.Components.Web

<h1 aria-label="Accessible Page Title">Accessible Web App</h1>

<div class="container">
    <!-- 无障碍表单 -->
    <form aria-label="Contact Form">
        <div class="form-group">
            <label for="name" aria-required="true">Name:</label>
            <input type="text" id="name" name="name" required aria-describedby="name-help" />
            <small id="name-help">Please enter your full name</small>
        </div>
        
        <div class="form-group">
            <label for="email" aria-required="true">Email:</label>
            <input type="email" id="email" name="email" required aria-describedby="email-help" />
            <small id="email-help">Please enter a valid email address</small>
        </div>
        
        <button type="submit" aria-label="Submit Form">Submit</button>
    </form>
    
    <!-- 无障碍导航 -->
    <nav aria-label="Main Navigation">
        <ul>
            <li><a href="/" aria-current="page">Home</a></li>
            <li><a href="/about">About</a></li>
            <li><a href="/contact">Contact</a></li>
        </ul>
    </nav>
    
    <!-- 无障碍图像 -->
    <div class="image-section">
        <img src="https://example.com/image.jpg" alt="Example image description" aria-describedby="image-description" />
        <p id="image-description">This is a descriptive text for the image above</p>
    </div>
</div>

<style>
    .container {
        padding: 20px;
        max-width: 800px;
        margin: 0 auto;
    }
    
    .form-group {
        margin-bottom: 20px;
    }
    
    label {
        display: block;
        margin-bottom: 5px;
        font-weight: bold;
    }
    
    input {
        width: 100%;
        padding: 8px;
        border: 1px solid #ddd;
        border-radius: 4px;
    }
    
    button {
        padding: 10px 20px;
        background-color: #007bff;
        color: white;
        border: none;
        border-radius: 4px;
        cursor: pointer;
    }
    
    button:hover {
        background-color: #0069d9;
    }
    
    nav ul {
        list-style: none;
        padding: 0;
        display: flex;
        gap: 20px;
        margin: 20px 0;
    }
    
    nav a {
        text-decoration: none;
        color: #007bff;
    }
    
    nav a:hover {
        text-decoration: underline;
    }
    
    nav a[aria-current="page"] {
        font-weight: bold;
        text-decoration: underline;
    }
    
    .image-section {
        margin: 20px 0;
    }
    
    img {
        max-width: 100%;
        height: auto;
        border-radius: 4px;
    }
    
    /* 高对比度模式支持 */
    @media (prefers-contrast: high) {
        body {
            background-color: black;
            color: white;
        }
        
        input {
            border-color: white;
            background-color: #333;
            color: white;
        }
        
        button {
            background-color: white;
            color: black;
        }
        
        nav a {
            color: yellow;
        }
    }
    
    /* 减少动画模式支持 */
    @media (prefers-reduced-motion: reduce) {
        * {
            animation-duration: 0.01ms !important;
            animation-iteration-count: 1 !important;
            transition-duration: 0.01ms !important;
            scroll-behavior: auto !important;
        }
    }
</style>
```

## 9. 高级示例

### 9.1 多平台应用

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UI.MultiPlatform
{
    public enum UIType
    {
        Web,
        Desktop,
        Mobile
    }
    
    public interface IUIProvider
    {
        UIType Type { get; }
        Task InitializeAsync();
        Task RenderAsync();
        Task ShutdownAsync();
    }
    
    public class WebUIProvider : IUIProvider
    {
        private readonly ILogger<WebUIProvider> _logger;
        
        public UIType Type => UIType.Web;
        
        public WebUIProvider(ILogger<WebUIProvider> logger)
        {
            _logger = logger;
        }
        
        public async Task InitializeAsync()
        {
            _logger.LogInformation("Initializing Web UI provider");
            await Task.Delay(100);
        }
        
        public async Task RenderAsync()
        {
            _logger.LogInformation("Rendering Web UI");
            await Task.Delay(50);
        }
        
        public async Task ShutdownAsync()
        {
            _logger.LogInformation("Shutting down Web UI provider");
            await Task.Delay(50);
        }
    }
    
    public class DesktopUIProvider : IUIProvider
    {
        private readonly ILogger<DesktopUIProvider> _logger;
        
        public UIType Type => UIType.Desktop;
        
        public DesktopUIProvider(ILogger<DesktopUIProvider> logger)
        {
            _logger = logger;
        }
        
        public async Task InitializeAsync()
        {
            _logger.LogInformation("Initializing Desktop UI provider");
            await Task.Delay(150);
        }
        
        public async Task RenderAsync()
        {
            _logger.LogInformation("Rendering Desktop UI");
            await Task.Delay(75);
        }
        
        public async Task ShutdownAsync()
        {
            _logger.LogInformation("Shutting down Desktop UI provider");
            await Task.Delay(75);
        }
    }
    
    public class MobileUIProvider : IUIProvider
    {
        private readonly ILogger<MobileUIProvider> _logger;
        
        public UIType Type => UIType.Mobile;
        
        public MobileUIProvider(ILogger<MobileUIProvider> logger)
        {
            _logger = logger;
        }
        
        public async Task InitializeAsync()
        {
            _logger.LogInformation("Initializing Mobile UI provider");
            await Task.Delay(200);
        }
        
        public async Task RenderAsync()
        {
            _logger.LogInformation("Rendering Mobile UI");
            await Task.Delay(100);
        }
        
        public async Task ShutdownAsync()
        {
            _logger.LogInformation("Shutting down Mobile UI provider");
            await Task.Delay(100);
        }
    }
    
    public class UIApplication
    {
        private readonly ILogger<UIApplication> _logger;
        private readonly IUIProvider _uiProvider;
        
        public UIApplication(ILogger<UIApplication> logger, IUIProvider uiProvider)
        {
            _logger = logger;
            _uiProvider = uiProvider;
        }
        
        public async Task RunAsync()
        {
            _logger.LogInformation("Starting UI application with {UIProvider}", _uiProvider.Type);
            
            try
            {
                await _uiProvider.InitializeAsync();
                await _uiProvider.RenderAsync();
                
                _logger.LogInformation("UI application running. Press Ctrl+C to exit.");
                await Task.Delay(-1); // 无限等待
            }
            finally
            {
                await _uiProvider.ShutdownAsync();
                _logger.LogInformation("UI application shutdown");
            }
        }
    }
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== Multi-Platform UI Application ===");
            Console.WriteLine("Select UI type:");
            Console.WriteLine("1. Web");
            Console.WriteLine("2. Desktop");
            Console.WriteLine("3. Mobile");
            
            var input = Console.ReadLine();
            UIType uiType;
            
            switch (input)
            {
                case "1":
                    uiType = UIType.Web;
                    break;
                case "2":
                    uiType = UIType.Desktop;
                    break;
                case "3":
                    uiType = UIType.Mobile;
                    break;
                default:
                    uiType = UIType.Web;
                    break;
            }
            
            // 构建服务容器
            var services = new ServiceCollection();
            
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
            
            // 根据选择添加相应的 UI 提供程序
            switch (uiType)
            {
                case UIType.Web:
                    services.AddSingleton<IUIProvider, WebUIProvider>();
                    break;
                case UIType.Desktop:
                    services.AddSingleton<IUIProvider, DesktopUIProvider>();
                    break;
                case UIType.Mobile:
                    services.AddSingleton<IUIProvider, MobileUIProvider>();
                    break;
            }
            
            // 添加应用程序
            services.AddSingleton<UIApplication>();
            
            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();
            
            var application = serviceProvider.GetRequiredService<UIApplication>();
            await application.RunAsync();
        }
    }
}
```

### 9.2 插件系统

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:package System.Reflection@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UI.PluginSystem
{
    public interface IUIPlugin
    {
        string Name { get; }
        string Version { get; }
        Task InitializeAsync();
        Task ExecuteAsync();
        Task ShutdownAsync();
    }
    
    public class PluginManager
    {
        private readonly ILogger<PluginManager> _logger;
        private readonly List<IUIPlugin> _plugins = new List<IUIPlugin>();
        
        public PluginManager(ILogger<PluginManager> logger)
        {
            _logger = logger;
        }
        
        public void LoadPluginsFromAssembly(Assembly assembly)
        {
            _logger.LogInformation("Loading plugins from assembly: {Assembly}", assembly.FullName);
            
            var pluginTypes = assembly.GetTypes()
                .Where(type => typeof(IUIPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
            
            foreach (var pluginType in pluginTypes)
            {
                try
                {
                    var plugin = Activator.CreateInstance(pluginType) as IUIPlugin;
                    if (plugin != null)
                    {
                        _plugins.Add(plugin);
                        _logger.LogInformation("Loaded plugin: {Name} v{Version}", plugin.Name, plugin.Version);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading plugin: {Type}", pluginType.FullName);
                }
            }
        }
        
        public async Task InitializeAllAsync()
        {
            _logger.LogInformation("Initializing all plugins");
            
            foreach (var plugin in _plugins)
            {
                try
                {
                    await plugin.InitializeAsync();
                    _logger.LogInformation("Initialized plugin: {Name}", plugin.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error initializing plugin: {Name}", plugin.Name);
                }
            }
        }
        
        public async Task ExecuteAllAsync()
        {
            _logger.LogInformation("Executing all plugins");
            
            foreach (var plugin in _plugins)
            {
                try
                {
                    await plugin.ExecuteAsync();
                    _logger.LogInformation("Executed plugin: {Name}", plugin.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing plugin: {Name}", plugin.Name);
                }
            }
        }
        
        public async Task ShutdownAllAsync()
        {
            _logger.LogInformation("Shutting down all plugins");
            
            foreach (var plugin in _plugins)
            {
                try
                {
                    await plugin.ShutdownAsync();
                    _logger.LogInformation("Shutdown plugin: {Name}", plugin.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error shutting down plugin: {Name}", plugin.Name);
                }
            }
        }
        
        public IEnumerable<IUIPlugin> GetPlugins()
        {
            return _plugins;
        }
    }
    
    public class SamplePlugin : IUIPlugin
    {
        public string Name => "Sample Plugin";
        public string Version => "1.0.0";
        
        public async Task InitializeAsync()
        {
            Console.WriteLine($"Initializing {Name} v{Version}");
            await Task.Delay(50);
        }
        
        public async Task ExecuteAsync()
        {
            Console.WriteLine($"Executing {Name} v{Version}");
            await Task.Delay(100);
        }
        
        public async Task ShutdownAsync()
        {
            Console.WriteLine($"Shutting down {Name} v{Version}");
            await Task.Delay(50);
        }
    }
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建服务容器
            var services = new ServiceCollection();
            
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
            
            // 添加插件管理器
            services.AddSingleton<PluginManager>();
            
            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();
            
            var pluginManager = serviceProvider.GetRequiredService<PluginManager>();
            
            // 加载当前程序集中的插件
            pluginManager.LoadPluginsFromAssembly(Assembly.GetExecutingAssembly());
            
            // 初始化、执行和关闭插件
            await pluginManager.InitializeAllAsync();
            await pluginManager.ExecuteAllAsync();
            await pluginManager.ShutdownAllAsync();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
```

### 9.3 CI/CD 集成

#### 9.3.1 GitHub Actions

```yaml
name: UI Skill CI

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 10.0.x
    - name: Restore dependencies
      run: dotnet restore ui/
    - name: Build
      run: dotnet build ui/ --configuration Release
    - name: Test
      run: dotnet test ui/ --configuration Release
    - name: Publish
      run: dotnet publish ui/ --configuration Release --runtime linux-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
    - name: Upload artifact
      uses: actions/upload-artifact@v3
      with:
        name: ui-app
        path: ui/bin/Release/net11.0/linux-x64/publish/
```

#### 9.3.2 Azure DevOps

```yaml
trigger:
- main

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: UseDotNet@2
  inputs:
    version: '10.0.x'
    includePreviewVersions: true

- script: dotnet restore ui/
  displayName: 'Restore dependencies'

- script: dotnet build ui/ --configuration Release
  displayName: 'Build'

- script: dotnet test ui/ --configuration Release
  displayName: 'Test'

- script: dotnet publish ui/ --configuration Release --runtime linux-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
  displayName: 'Publish'

- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: 'ui/bin/Release/net11.0/linux-x64/publish/'
    ArtifactName: 'ui-app'
```

## 10. 部署示例

### 10.1 Docker 部署

**Dockerfile**

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY . .
RUN dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true -o out

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
ENTRYPOINT ["./ui_core"]
```

**docker-compose.yml**

```yaml
version: '3.8'

services:
  ui-app:
    build: .
    ports:
      - "8080:8080"
    environment:
      - DOTNET_ENVIRONMENT=Production
      - UI_TYPE=Web
      - WEB_PORT=8080
      - WEB_HOST=0.0.0.0
      - WEB_USE_HTTPS=false
    restart: unless-stopped
```

**构建和运行命令**

```bash
# 构建镜像
docker build -t ui-app .

# 运行容器
docker run -p 8080:8080 --name ui-app ui-app

# 使用 docker-compose
docker-compose up -d
```

### 10.2 Kubernetes 部署

**deployment.yaml**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: ui-app
spec:
  replicas: 3
  selector:
    matchLabels:
      app: ui-app
  template:
    metadata:
      labels:
        app: ui-app
    spec:
      containers:
      - name: ui-app
        image: ui-app:latest
        ports:
        - containerPort: 8080
        env:
        - name: DOTNET_ENVIRONMENT
          value: "Production"
        - name: UI_TYPE
          value: "Web"
        - name: WEB_PORT
          value: "8080"
        - name: WEB_HOST
          value: "0.0.0.0"
        - name: WEB_USE_HTTPS
          value: "false"
        resources:
          limits:
            cpu: "1"
            memory: "512Mi"
          requests:
            cpu: "500m"
            memory: "256Mi"
---
apiVersion: v1
kind: Service
metadata:
  name: ui-app
spec:
  selector:
    app: ui-app
  ports:
  - port: 80
    targetPort: 8080
  type: LoadBalancer
```

**部署命令**

```bash
# 部署到 Kubernetes
kubectl apply -f deployment.yaml

# 查看部署状态
kubectl get deployments

# 查看服务状态
kubectl get services

# 查看 Pod 状态
kubectl get pods
```

### 10.3 本地开发环境

**launchSettings.json**

```json
{
  "profiles": {
    "WebDevelopment": {
      "commandName": "Project",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development",
        "UI_TYPE": "Web",
        "WEB_PORT": "8080",
        "WEB_HOST": "localhost",
        "WEB_USE_HTTPS": "false"
      },
      "applicationUrl": "http://localhost:8080"
    },
    "DesktopDevelopment": {
      "commandName": "Project",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development",
        "UI_TYPE": "Desktop"
      }
    },
    "MobileDevelopment": {
      "commandName": "Project",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development",
        "UI_TYPE": "Mobile"
      }
    }
  }
}
```

**运行命令**

```bash
# 运行 Web UI 开发环境
dotnet run --project scripts/ui_core.cs --launch-profile WebDevelopment

# 运行桌面 UI 开发环境
dotnet run --project scripts/ui_core.cs --launch-profile DesktopDevelopment

# 运行移动 UI 开发环境
dotnet run --project scripts/ui_core.cs --launch-profile MobileDevelopment
```

## 11. 总结

本示例文档提供了 UI 技能的全面使用示例，涵盖了从基本应用结构到高级功能的各种场景。通过这些示例，开发者可以快速上手 UI 技能，并根据自己的需求进行定制和扩展。

### 主要示例类别

1. **基本应用结构**：最小化应用和完整应用结构
2. **命令行工具使用**：基本命令和高级命令行选项
3. **Web UI 示例**：基本 Web 应用、响应式设计和实时通信
4. **桌面 UI 示例**：基本桌面应用、多窗口应用和系统集成
5. **移动 UI 示例**：基本移动应用、触摸优化和设备硬件访问
6. **主题管理示例**：基本主题使用和自定义主题
7. **本地化示例**：基本本地化和资源管理
8. **性能监控示例**：基本监控和性能优化
9. **无障碍支持示例**：基本无障碍检查和无障碍 UI 设计
10. **高级示例**：多平台应用、插件系统和 CI/CD 集成
11. **部署示例**：Docker 部署、Kubernetes 部署和本地开发环境

这些示例展示了 UI 技能的灵活性和强大功能，为开发者提供了丰富的参考资料，帮助他们构建高质量的用户界面应用。