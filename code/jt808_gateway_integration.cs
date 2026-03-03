#:sdk Microsoft.NET.Sdk
#:package JT808Gateway@latest
#:package JT1078.Protocol@latest
#:package JT809.Protocol@latest
#:property LangVersion preview
#:property TargetFramework net10.0

using System;
using System.Buffers;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using JT808.Protocol;
using JT1078.Protocol;
using JT809.Protocol;

public interface IJT808GatewayService
{
    Task ProcessJT808MessageAsync(byte[] message);
    Task ProcessJT1078MessageAsync(byte[] message);
    Task ProcessJT809MessageAsync(byte[] message);
}

public class JT808GatewayService : IJT808GatewayService
{
    private readonly ObjectPool<JT808Serializer> _jt808SerializerPool;
    private readonly ObjectPool<JT1078Serializer> _jt1078SerializerPool;
    private readonly ObjectPool<JT809Serializer> _jt809SerializerPool;
    private readonly ThreadLocal<Span<byte>> _threadLocalBuffer;
    private readonly Channel<ProtocolMessage> _messageChannel;

    public JT808GatewayService(
        ObjectPool<JT808Serializer> jt808SerializerPool,
        ObjectPool<JT1078Serializer> jt1078SerializerPool,
        ObjectPool<JT809Serializer> jt809SerializerPool)
    {
        _jt808SerializerPool = jt808SerializerPool;
        _jt1078SerializerPool = jt1078SerializerPool;
        _jt809SerializerPool = jt809SerializerPool;
        _threadLocalBuffer = new ThreadLocal<Span<byte>>(() => 
            new Span<byte>(new byte[1024 * 1024]));
        _messageChannel = Channel.CreateUnbounded<ProtocolMessage>();
    }

    public async Task ProcessJT808MessageAsync(byte[] message)
    {
        var serializer = _jt808SerializerPool.Get();
        try
        {
            // 使用线程本地Span处理消息
            var buffer = _threadLocalBuffer.Value;
            
            // 解析JT808消息
            var jt808Package = serializer.Deserialize(message);
            
            // 处理消息
            await HandleJT808PackageAsync(jt808Package);
        }
        finally
        {
            _jt808SerializerPool.Return(serializer);
        }
    }

    public async Task ProcessJT1078MessageAsync(byte[] message)
    {
        var serializer = _jt1078SerializerPool.Get();
        try
        {
            // 解析JT1078消息
            var jt1078Package = serializer.Deserialize(message);
            
            // 处理消息
            await HandleJT1078PackageAsync(jt1078Package);
        }
        finally
        {
            _jt1078SerializerPool.Return(serializer);
        }
    }

    public async Task ProcessJT809MessageAsync(byte[] message)
    {
        var serializer = _jt809SerializerPool.Get();
        try
        {
            // 解析JT809消息
            var jt809Package = serializer.Deserialize(message);
            
            // 处理消息
            await HandleJT809PackageAsync(jt809Package);
        }
        finally
        {
            _jt809SerializerPool.Return(serializer);
        }
    }

    private async Task HandleJT808PackageAsync(JT808Package package)
    {
        // JT808消息处理逻辑
        await _messageChannel.Writer.WriteAsync(new ProtocolMessage
        {
            ProtocolType = ProtocolType.JT808,
            Data = package
        });
    }

    private async Task HandleJT1078PackageAsync(JT1078Package package)
    {
        // JT1078消息处理逻辑
        await _messageChannel.Writer.WriteAsync(new ProtocolMessage
        {
            ProtocolType = ProtocolType.JT1078,
            Data = package
        });
    }

    private async Task HandleJT809PackageAsync(JT809Package package)
    {
        // JT809消息处理逻辑
        await _messageChannel.Writer.WriteAsync(new ProtocolMessage
        {
            ProtocolType = ProtocolType.JT809,
            Data = package
        });
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJT808GatewayServices(this IServiceCollection services)
    {
        services.AddSingleton<ObjectPool<JT808Serializer>>(sp =>
        {
            var policy = new DefaultPooledObjectPolicy<JT808Serializer>();
            return new DefaultObjectPool<JT808Serializer>(policy, Environment.ProcessorCount * 2);
        });

        services.AddSingleton<ObjectPool<JT1078Serializer>>(sp =>
        {
            var policy = new DefaultPooledObjectPolicy<JT1078Serializer>();
            return new DefaultObjectPool<JT1078Serializer>(policy, Environment.ProcessorCount * 2);
        });

        services.AddSingleton<ObjectPool<JT809Serializer>>(sp =>
        {
            var policy = new DefaultPooledObjectPolicy<JT809Serializer>();
            return new DefaultObjectPool<JT809Serializer>(policy, Environment.ProcessorCount * 2);
        });

        services.AddSingleton<IJT808GatewayService, JT808GatewayService>();
        return services;
    }
}