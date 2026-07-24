#:sdk Microsoft.NET.Sdk
#:package SharpPcap@6.2.2
#:package PacketDotNet@1.4.7
#:property TargetFramework net11.0
#:property Nullable enable

using System;
using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;

public class PacketCaptureOptions
{
    public string DeviceName { get; set; } = string.Empty;
    public string PcapFile { get; set; } = string.Empty;
    public int BufferSize { get; set; } = 1024;
    public bool PromiscuousMode { get; set; } = true;
    public int ReadTimeout { get; set; } = 1000;
}

public interface IPacketCaptureService : IDisposable
{
    ChannelReader<RawCapture> PacketChannel { get; }
    void StartLiveCapture();
    void StartFileCapture();
}

public class PacketCaptureService : IPacketCaptureService
{
    private readonly PacketCaptureOptions _options;
    private readonly Channel<RawCapture> _packetChannel;
    private ICaptureDevice _device;
    private bool _disposed;

    public PacketCaptureService(IOptions<PacketCaptureOptions> options)
    {
        _options = options.Value;
        _packetChannel = Channel.CreateBounded<RawCapture>(_options.BufferSize);
    }

    public ChannelReader<RawCapture> PacketChannel => _packetChannel.Reader;

    public void StartLiveCapture()
    {
        if (_device != null)
            throw new InvalidOperationException("Capture already started");

        _device = CaptureDeviceList.Instance.FindDeviceByName(_options.DeviceName) ??
                 throw new ArgumentException("Device not found");

        _device.Open(DeviceMode.Promiscuous, _options.ReadTimeout);
        _device.OnPacketArrival += OnPacketArrival;
        _device.StartCapture();
    }

    public void StartFileCapture()
    {
        if (_device != null)
            throw new InvalidOperationException("Capture already started");

        _device = new CaptureFileReaderDevice(_options.PcapFile);
        _device.Open(DeviceMode.Promiscuous, _options.ReadTimeout);
        _device.OnPacketArrival += OnPacketArrival;
        _device.StartCapture();
    }

    private void OnPacketArrival(object sender, PacketCapture e)
    {
        _packetChannel.Writer.TryWrite(e.GetPacket());
    }

    public void Dispose()
    {
        if (_disposed) return;

        _device?.StopCapture();
        _device?.Close();
        _device?.Dispose();
        _packetChannel.Writer.Complete();
        _disposed = true;
    }
}

public static class PacketCaptureExtensions
{
    public static IServiceCollection AddPacketCapture(this IServiceCollection services, Action<PacketCaptureOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IPacketCaptureService, PacketCaptureService>();
        return services;
    }
}

// Usage example:
// var services = new ServiceCollection();
// services.AddPacketCapture(options =>
// {
//     options.DeviceName = "eth0";
//     options.PromiscuousMode = true;
// });
// var provider = services.BuildServiceProvider();
// var captureService = provider.GetRequiredService<IPacketCaptureService>();
// captureService.StartLiveCapture();