#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@8.0.0
#:package ZstdNet@1.4.5
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZstdNet;

var builder = WebApplication.CreateBuilder(args);

// 配置SQLite数据库上下文
builder.Services.AddDbContext<HistoryDbContext>(options => 
    options.UseSqlite("Data Source=history.db")
           .EnableAutoHistory());

// 配置高性能压缩服务
builder.Services.AddSingleton<HistoryCompressionService>();

var app = builder.Build();

// 历史记录查询API
app.MapGet("/api/history/{entityType}/{entityId}", 
    async ([FromServices] HistoryDbContext db, string entityType, string entityId) =>
{
    using var memory = MemoryPool<byte>.Shared.Rent(1024);
    var compressedData = await db.AutoHistory
        .Where(h => h.EntityId == entityId && h.EntityType == entityType)
        .OrderByDescending(h => h.Created)
        .Select(h => h.CompressedData)
        .ToArrayAsync();

    // 使用零拷贝技术处理压缩数据
    var decompressor = new Decompressor();
    return compressedData.Select(data => 
    {
        using var result = decompressor.Unwrap(data);
        return result.ToArray();
    });
});

app.Run();

// 数据库上下文
public class HistoryDbContext : DbContext
{
    public DbSet<AutoHistory> AutoHistory { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AutoHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CompressedData).IsRequired();
        });
    }
}

// 压缩服务
public class HistoryCompressionService
{
    private readonly ThreadLocal<Compressor> _compressor;
    private readonly ThreadLocal<Decompressor> _decompressor;

    public HistoryCompressionService()
    {
        _compressor = new(() => new Compressor());
        _decompressor = new(() => new Decompressor());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] Compress(string data) => _compressor.Value.Wrap(data);

    [SkipLocalsInit]
    public string Decompress(byte[] data) => _decompressor.Value.Unwrap(data).ToString();
}