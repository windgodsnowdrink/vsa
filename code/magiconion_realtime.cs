#:sdk Microsoft.NET.Sdk.Web
#:package MagicOnion@5.0.0
#:package System.Threading.Channels@8.0.0
#:package MessagePack@2.5.122
#:property LangVersion preview
#:property TargetFramework net11.0

using MagicOnion.Server;
using System.Threading.Channels;

// 高性能消息通道
var messageChannel = Channel.CreateBounded<StreamingMessage>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

// 零拷贝处理器
builder.Services.AddSingleton<IStreamingProcessor>(sp => 
    new ChannelStreamingProcessor(
        messageChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[1024])));

// MagicOnion服务配置
services.AddMagicOnion(options =>
{
    options.SerializerOptions = MessagePackSerializerOptions.Standard
        .WithCompression(MessagePackCompression.Lz4BlockArray);
    options.EnableCurrentContext = true;
});