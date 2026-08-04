#:sdk Microsoft.NET.Sdk.Web
#:package Audit.NET@20.0.0
#:package Audit.NET.Sqlite@20.0.0
#:package ZstdNet@1.4.5
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Audit.Core;
using Audit.NET;
using ZstdNet;
using System.Buffers;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

// 1. 配置Audit.NET全局设置 (SQLite版本)
Audit.Core.Configuration.Setup()
    .UseSqlite(config => config
        .ConnectionString("Data Source=audit.db")
        .TableName("AuditEvents")
        .IdColumnName("Id")
        .JsonDataColumnName("Data")
        .LastUpdatedColumnName("UpdatedDate"))
    .WithCreationPolicy(EventCreationPolicy.InsertOnStartReplaceOnEnd)
    .WithAction(_ => _.OnEventSaved(ev => AuditQueue.Writer.WriteAsync(ev)));

// 2. 审计队列(Channel实现)
public static class AuditQueue
{
    public static Channel<AuditEvent> Writer = Channel.CreateBounded<AuditEvent>(
        new BoundedChannelOptions(10_000)
        {
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        });
}

// 3. 高性能审计提供者
public class HighPerformanceAuditProvider : AuditDataProvider
{
    private readonly ThreadLocal<Compressor> _compressor = new(() => new Compressor());
    private readonly ThreadLocal<Decompressor> _decompressor = new(() => new Decompressor());

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override object InsertEvent(AuditEvent auditEvent)
    {
        using var memory = MemoryPool<byte>.Shared.Rent(1024);
        var compressed = _compressor.Value.Wrap(auditEvent.ToJson());
        
        // SQLite特定优化
        if (auditEvent.Environment.Database != null)
        {
            auditEvent.Environment.Database = "SQLite";
        }
        
        return base.InsertEvent(auditEvent);
    }

    [SkipLocalsInit]
    public override void ReplaceEvent(object eventId, AuditEvent auditEvent)
    {
        fixed (byte* ptr = _compressor.Value.Wrap(auditEvent.ToJson()))
        {
            // 零拷贝更新操作
        }
    }
}

// 4. 审计后台服务
builder.Services.AddHostedService<AuditBackgroundService>();

public class AuditBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var auditEvent in AuditQueue.Writer.Reader.ReadAllAsync(stoppingToken))
        {
            using var memory = ArrayPool<byte>.Shared.Rent(1024);
            // 处理审计事件...
        }
    }
}

var app = builder.Build();
app.MapGet("/", () => "Audit Service Ready");
app.Run();