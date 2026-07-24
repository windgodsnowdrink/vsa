#:sdk Microsoft.NET.Sdk.Web
#:package WolverineFx@1.10.0
#:package WolverineFx.EntityFrameworkCore@1.10.0
#:package Microsoft.EntityFrameworkCore@8.0.0
#:property TargetFramework net11.0
#:property Nullable enable

using Wolverine;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

// 本地消息表实体
public class OutboxMessage
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string MessageType { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? Error { get; set; }
}

// 数据库上下文
public class MessageDbContext : DbContext
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=messages.db");
}

// Wolverine消息处理中间件
public class OutboxMiddleware
{
    public static void Before(OutboxMessage message, MessageDbContext dbContext)
    {
        dbContext.OutboxMessages.Add(new OutboxMessage
        {
            MessageType = message.GetType().FullName!,
            Payload = JsonSerializer.Serialize(message)
        });
    }
}

// 后台消息处理服务
public class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        IServiceProvider services,
        ILogger<OutboxProcessor> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<MessageDbContext>();
                var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

                var messages = await dbContext.OutboxMessages
                    .Where(m => m.ProcessedAt == null)
                    .OrderBy(m => m.CreatedAt)
                    .Take(100)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        var messageType = Type.GetType(message.MessageType);
                        if (messageType != null)
                        {
                            var msg = JsonSerializer.Deserialize(
                                message.Payload, 
                                messageType);
                            
                            if (msg != null)
                            {
                                await messageBus.SendAsync(msg);
                                message.ProcessedAt = DateTime.UtcNow;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        message.Error = ex.ToString();
                        _logger.LogError(ex, "处理消息失败: {MessageId}", message.Id);
                    }
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理消息表时发生异常");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}

// DI扩展
public static class WolverineDependencyInjectionExtensions
{
    public static WolverineOptions UseLocalMessageTable(this WolverineOptions options)
    {
        options.Services.AddDbContext<MessageDbContext>();
        options.Services.AddHostedService<OutboxProcessor>();
        
        // 配置Wolverine使用本地消息表
        options.Policies.AddMiddleware<OutboxMiddleware>();
        
        return options;
    }
}

// 使用示例
builder.Host.UseWolverine(opts =>
{
    opts.UseLocalMessageTable();
});