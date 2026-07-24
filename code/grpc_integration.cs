#:sdk Microsoft.NET.Sdk.Web
#:package Grpc.AspNetCore@2.62.0
#:package Google.Protobuf@3.25.1
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using Grpc.Core;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();
builder.Services.AddGrpc(options => {
    options.MaxReceiveMessageSize = 32 * 1024 * 1024; // 32MB
    options.EnableDetailedErrors = true;
});

// 高性能通道(Disruptor模式)
var commandChannel = Channel.CreateBounded<GrpcCommand>(
    new BoundedChannelOptions(10000) {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

builder.Services.AddSingleton(commandChannel);
builder.Services.AddSingleton<IGrpcCommandProcessor>(sp => 
    new GrpcChannelProcessor(
        commandChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[1024])));

var app = builder.Build();
app.MapGrpcService<GreeterService>();
app.Run();

// 高性能处理器实现
[SkipLocalsInit]
public class GrpcChannelProcessor : IGrpcCommandProcessor
{
    // ... existing code ...
}