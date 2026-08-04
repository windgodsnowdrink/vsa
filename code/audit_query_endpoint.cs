#:sdk Microsoft.NET.Sdk.Web
#:package FastEndpoints@6.1.0
#:package Audit.NET.Sqlite@20.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable

using FastEndpoints;
using Audit.Core;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton(Channel.CreateBounded<AuditQuery>(1000));

var app = builder.Build();
app.UseFastEndpoints();
app.Run();

// 高性能审计查询端点
public class AuditQueryEndpoint : Endpoint<AuditQueryRequest>
{
    private readonly ChannelWriter<AuditQuery> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public override void Configure() => Get("/api/audit/query");
    
    public override async Task HandleAsync(AuditQueryRequest req, CancellationToken ct)
    {
        using var memory = MemoryPool<byte>.Shared.Rent(1024);
        var query = new AuditQuery(req);
        await _writer.WriteAsync(query, ct);
        
        // 使用零拷贝技术处理响应
        var response = new AuditQueryResponse();
        await SendAsync(response, cancellation: ct);
    }
}

// 查询处理器
[SkipLocalsInit]
public class AuditQueryProcessor : BackgroundService
{
    private readonly ChannelReader<AuditQuery> _reader;
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var query in _reader.ReadAllAsync(ct))
        {
            // 使用Span优化查询处理
            Span<byte> buffer = stackalloc byte[256];
            var events = AuditEventQuery.Where(query.Criteria).ToList();
        }
    }
}