#:sdk Microsoft.NET.Sdk.Web
#:package MiniProfiler.AspNetCore@4.2.22
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using StackExchange.Profiling;

public class ProfilerAlertService : BackgroundService
{
    private readonly IMiniProfilerStorage _storage;

    public ProfilerAlertService(IMiniProfilerStorage storage)
    {
        _storage = storage;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var recentProfiles = await _storage.ListAsync(100);
            foreach (var profiler in recentProfiles)
            {
                if (profiler.DurationMilliseconds > 1000)
                {
                    // 触发告警逻辑
                }
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}

var builder = WebApplication.CreateBuilder();
builder.Services.AddMiniProfiler();
builder.Services.AddHostedService<ProfilerAlertService>();

var app = builder.Build();
app.UseMiniProfiler();
app.MapGet("/", () => "Alerting Profiler Ready");
app.Run();