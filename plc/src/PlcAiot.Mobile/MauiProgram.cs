using Microsoft.Extensions.Logging;
using PlcAiot.Shared.Extensions;
using PlcAiot.Shared.Services;

namespace PlcAiot.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddPlcAiotShared();
        // MAUI uses a single shared MQTT client for the whole app lifetime.
        builder.Services.AddSingleton<MqttClientService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
