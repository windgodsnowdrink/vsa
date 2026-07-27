#:sdk Microsoft.NET.Sdk.Web
#:package MimeKit@4.3.0
#:package MailKit@4.3.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.ObjectPool;
using System.Threading.Channels;

// 高性能MIME类型处理器
[SkipLocalsInit]
public sealed class MimeTypeProcessor
{
    private readonly Channel<MimeMessage> _channel;
    private readonly ThreadLocal<Span<byte>> _buffer;
    private readonly IObjectPool<SmtpClient> _smtpPool;
    
    public MimeTypeProcessor(
        Channel<MimeMessage> channel,
        IObjectPool<SmtpClient> smtpPool)
    {
        _channel = channel;
        _buffer = new(() => stackalloc byte[4096]);
        _smtpPool = smtpPool;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            var client = _smtpPool.Get();
            try
            {
                Span<byte> buffer = _buffer.Value;
                ProcessMimeMessage(message, buffer, client);
            }
            finally
            {
                _smtpPool.Return(client);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void ProcessMimeMessage(MimeMessage message, Span<byte> buffer, SmtpClient client)
    {
        // 1. 处理MIME头
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0) // CPU cache-line对齐
            {
                var headerSpan = buffer.Slice(0, 64);
                ProcessMimeHeader(message, headerSpan);
            }
        }

        // 2. 处理MIME体
        foreach (var part in message.BodyParts)
        {
            if (part is MimePart mimePart)
            {
                using var memory = MemoryPool<byte>.Shared.Rent(4096);
                var content = memory.Memory.Span;
                
                // 3. 文件类型映射
                var fileType = MapContentType(mimePart.ContentType);
                mimePart.ContentType = fileType;
                
                // 4. 发送处理
                TailLatencyOptimizer.Optimize(() => 
                {
                    client.Send(message);
                });
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static ContentType MapContentType(ContentType original)
    {
        // 文件类型映射表
        return original.MimeType switch
        {
            "text/plain" => new ContentType("application", "octet-stream"),
            "image/jpeg" => new ContentType("image", "jpeg"),
            "application/pdf" => new ContentType("application", "pdf"),
            _ => original
        };
    }
}

// 服务配置
public static class MimeKitExtensions
{
    public static IServiceCollection AddMimeKitServices(this IServiceCollection services)
    {
        // MIME消息通道
        services.AddSingleton<Channel<MimeMessage>>(_ => 
            Channel.CreateBounded<MimeMessage>(new BoundedChannelOptions(10000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true
            }));

        // SMTP客户端池
        services.AddSingleton<IObjectPool<SmtpClient>>(sp => 
            new DefaultObjectPool<SmtpClient>(
                new SmtpClientPooledPolicy(),
                Environment.ProcessorCount * 2));

        // MIME处理器
        services.AddHostedService<MimeTypeProcessor>();

        return services;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMimeKitServices();

var app = builder.Build();
app.MapGet("/", () => "MIME Type Mapping Service Running");
app.Run();