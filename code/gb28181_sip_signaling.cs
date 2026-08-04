#:sdk Microsoft.NET.Sdk
#:package GB28181.Solution@latest
#:package SIPSorcery@5.0.0
#:property LangVersion preview
#:property TargetFramework net10.0

using System;
using System.Buffers;
using System.Net;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using SIPSorcery.SIP;

public interface IGB28181SipService
{
    Task RegisterAsync(string serverIp, int serverPort, string deviceId, string password);
    Task KeepAliveAsync(string deviceId);
    Task<DeviceInfo> QueryDeviceInfoAsync(string deviceId);
    Task StartRealplayAsync(string deviceId, string channelId, IPEndPoint mediaEndpoint);
}

public class GB28181SipService : IGB28181SipService
{
    private readonly ObjectPool<SIPTransport> _sipTransportPool;
    private readonly ThreadLocal<Span<byte>> _threadLocalBuffer;
    private readonly Channel<SIPMessage> _sipChannel;

    public GB28181SipService(ObjectPool<SIPTransport> sipTransportPool)
    {
        _sipTransportPool = sipTransportPool;
        _threadLocalBuffer = new ThreadLocal<Span<byte>>(() => 
            new Span<byte>(new byte[1024]));
        _sipChannel = Channel.CreateUnbounded<SIPMessage>();
    }

    public async Task RegisterAsync(string serverIp, int serverPort, string deviceId, string password)
    {
        var transport = _sipTransportPool.Get();
        try
        {
            // 构建REGISTER消息
            var registerRequest = new SIPRequest(
                SIPMethodsEnum.REGISTER,
                new SIPURI(deviceId, serverIp, serverPort));
            
            // 添加Authorization头
            registerRequest.Header.AuthenticationHeader = new SIPAuthenticationHeader(
                "Digest", 
                deviceId, 
                password, 
                SIPURI.ParseSIPURI(registerRequest.URI.ToString()));
            
            // 发送REGISTER请求
            var registerResult = await transport.SendRequestAsync(registerRequest);
            
            // 处理注册响应
            if (registerResult.StatusCode == SIPResponseStatusCodesEnum.Ok)
            {
                // 注册成功
            }
        }
        finally
        {
            _sipTransportPool.Return(transport);
        }
    }

    public async Task KeepAliveAsync(string deviceId)
    {
        // 心跳实现
        var transport = _sipTransportPool.Get();
        try
        {
            var message = new SIPMessage(
                SIPMethodsEnum.MESSAGE,
                new SIPURI(deviceId, "", 0));
            
            await transport.SendRequestAsync(message);
        }
        finally
        {
            _sipTransportPool.Return(transport);
        }
    }

    public async Task<DeviceInfo> QueryDeviceInfoAsync(string deviceId)
    {
        // 设备查询实现
        var transport = _sipTransportPool.Get();
        try
        {
            var message = new SIPMessage(
                SIPMethodsEnum.MESSAGE,
                new SIPURI(deviceId, "", 0));
            
            message.Header.ContentType = "Application/MANSCDP+xml";
            message.Body = "<Query>";
            
            var response = await transport.SendRequestAsync(message);
            
            // 解析设备信息
            return ParseDeviceInfo(response.Body);
        }
        finally
        {
            _sipTransportPool.Return(transport);
        }
    }

    public async Task StartRealplayAsync(string deviceId, string channelId, IPEndPoint mediaEndpoint)
    {
        // 实时点播实现
        var transport = _sipTransportPool.Get();
        try
        {
            var inviteRequest = new SIPRequest(
                SIPMethodsEnum.INVITE,
                new SIPURI(deviceId, "", 0));
            
            // 构建SDP
            inviteRequest.Body = BuildSdp(mediaEndpoint);
            
            var response = await transport.SendRequestAsync(inviteRequest);
            
            if (response.StatusCode == SIPResponseStatusCodesEnum.Ok)
            {
                // 发送ACK确认
                var ack = new SIPRequest(
                    SIPMethodsEnum.ACK,
                    new SIPURI(deviceId, "", 0));
                
                await transport.SendRequestAsync(ack);
            }
        }
        finally
        {
            _sipTransportPool.Return(transport);
        }
    }

    private DeviceInfo ParseDeviceInfo(string xmlBody)
    {
        // 解析XML设备信息
        return new DeviceInfo();
    }

    private string BuildSdp(IPEndPoint mediaEndpoint)
    {
        // 构建SDP描述
        return $"v=0\no=- 0 0 IN IP4 {mediaEndpoint.Address}\n...";
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGB28181SipServices(this IServiceCollection services)
    {
        services.AddSingleton<ObjectPool<SIPTransport>>(sp =>
        {
            var policy = new DefaultPooledObjectPolicy<SIPTransport>();
            return new DefaultObjectPool<SIPTransport>(policy, Environment.ProcessorCount * 2);
        });

        services.AddSingleton<IGB28181SipService, GB28181SipService>();
        return services;
    }
}