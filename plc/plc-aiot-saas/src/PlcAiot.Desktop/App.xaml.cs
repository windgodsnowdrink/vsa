using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using PlcAiot.Desktop.Services;
using PlcAiot.Shared.Extensions;
using PlcAiot.Shared.Services;

namespace PlcAiot.Desktop;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = default!;

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddWpfBlazorWebView();
        builder.Services.AddPlcAiotShared();
        // Desktop uses a single shared MQTT client for the whole app lifetime.
        builder.Services.AddSingleton<MqttClientService>();
        builder.Services.AddSingleton<DesktopMqttService>();

        var host = builder.Build();
        Services = host.Services;

        var mqtt = Services.GetRequiredService<DesktopMqttService>();
        _ = mqtt.ConnectAsync();

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}
