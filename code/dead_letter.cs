#:sdk Microsoft.NET.Sdk.Web
#:property LangVersion preview
#:property TargetFramework net10.0

using StackExchange.Redis;

// 死信队列
public class DeadLetterQueue : IDisposable
{
    private readonly IDatabase _db;
    private readonly Channel<RedisValue> _dlqChannel;
    
    public DeadLetterQueue(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
        _dlqChannel = Channel.CreateBounded<RedisValue>(1000);
        _ = ProcessDlqMessagesAsync();
    }

    public async Task HandleFailedMessageAsync(string streamKey, string messageId, Exception ex)
    {
        await _db.StreamAddAsync(
            "dead_letter_queue",
            new NameValueEntry[]
            {
                new("original_stream", streamKey),
                new("message_id", messageId),
                new("error", ex.ToString()),
                new("timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            });
    }

    private async Task ProcessDlqMessagesAsync()
    {
        await foreach (var message in _dlqChannel.Reader.ReadAllAsync())
        {
            // 死信消息处理逻辑
        }
    }

    public void Dispose()
    {
        _dlqChannel.Writer.Complete();
    }
}