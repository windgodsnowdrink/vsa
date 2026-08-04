#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR@8.0.0
#:package Microsoft.EntityFrameworkCore.SqlServer@8.0.0
#:package OpenTelemetry.Exporter.OpenTelemetryProtocol@1.7.0
#:package GitVersion.MsBuild@6.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using GitVersion;

var builder = WebApplication.CreateBuilder();

// 1. 监控数据持久化
builder.Services.AddDbContext<MonitoringDbContext>(options => 
    options.UseSqlServer("Server=.;Database=SignalR_Monitoring;Trusted_Connection=True;"));

// 2. 事件回溯分析
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("SignalR.Events")
        .AddEntityFrameworkCoreInstrumentation()
        .AddOtlpExporter());

// 3. 配置版本控制
builder.Services.AddSingleton<IConfigVersioning>(sp => 
    new GitBasedVersioning(
        new ThreadLocal<Span<byte>>(() => stackalloc byte[256])));

var app = builder.Build();
app.MapHub<AdvancedMonitoringHub>("/advanced-monitoring");
app.Run();

// 监控数据上下文
public class MonitoringDbContext : DbContext
{
    public DbSet<MonitoringRecord> Records { get; set; }
    public DbSet<EventTrace> EventTraces { get; set; }
    
    public MonitoringDbContext(DbContextOptions options) : base(options) {}
}

// 版本控制器
[SkipLocalsInit]
public class GitBasedVersioning : IConfigVersioning
{
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public GitBasedVersioning(ThreadLocal<Span<byte>> buffer)
    {
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe string GetCurrentVersion()
    {
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                return GitVersion.MsBuild.GitVersionTasks
                    .GetVersionInfo(".").InformationalVersion;
            }
        }
        return "0.0.0";
    }
}

public record MonitoringRecord(
    string MetricName,
    double Value,
    DateTimeOffset Timestamp);

public record EventTrace(
    string EventId,
    string EventType,
    string Payload,
    DateTimeOffset OccurredAt);