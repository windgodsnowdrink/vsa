#:sdk Microsoft.NET.Sdk.Web
#:package SharpPcap@6.2.2
#:package PacketDotNet@1.4.7
#:property TargetFramework=net10.0
#:property Nullable=enable

using System;
using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using System.Collections.Generic;
using System.Linq;

public class AdvancedPacketCaptureOptions : PacketCaptureOptions
{
    public bool EnableTrafficAnalysis { get; set; } = true;
    public bool EnableStatistics { get; set; } = true;
    public List<string> ProtocolFilters { get; set; } = new();
    public int AnalysisWindowSeconds { get; set; } = 60;
}

public interface IAdvancedPacketCaptureService : IPacketCaptureService
{
    Task<Dictionary<string, long>> GetProtocolDistributionAsync();
    Task<Dictionary<string, long>> GetTopTalkersAsync(int topN = 10);
    Task<Dictionary<string, long>> GetTrafficTrendAsync();
}

public class AdvancedPacketCaptureService : PacketCaptureService, IAdvancedPacketCaptureService
{
    private readonly ConcurrentDictionary<string, long> _protocolCounts = new();
    private readonly ConcurrentDictionary<string, long> _sourceIpCounts = new();
    private readonly ConcurrentDictionary<string, long> _destIpCounts = new();
    private readonly ConcurrentQueue<long> _packetSizes = new();
    private readonly AdvancedPacketCaptureOptions _advancedOptions;
    private DateTime _lastAnalysisTime = DateTime.UtcNow;

    public AdvancedPacketCaptureService(IOptions<AdvancedPacketCaptureOptions> options) 
        : base(options)
    {
        _advancedOptions = options.Value;
    }

    protected override void OnPacketArrival(object sender, PacketCapture e)
    {
        var packet = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
        
        if (_advancedOptions.ProtocolFilters.Any() && 
            !_advancedOptions.ProtocolFilters.Contains(packet.GetType().Name))
            return;

        base.OnPacketArrival(sender, e);

        if (!_advancedOptions.EnableTrafficAnalysis) 
            return;

        var protocol = packet.GetType().Name;
        _protocolCounts.AddOrUpdate(protocol, 1, (_, count) => count + 1);

        if (packet is IPPacket ipPacket)
        {
            _sourceIpCounts.AddOrUpdate(ipPacket.SourceAddress.ToString(), 1, (_, count) => count + 1);
            _destIpCounts.AddOrUpdate(ipPacket.DestinationAddress.ToString(), 1, (_, count) => count + 1);
        }

        _packetSizes.Enqueue(packet.BytesHighPerformance.Length);

        if ((DateTime.UtcNow - _lastAnalysisTime).TotalSeconds >= _advancedOptions.AnalysisWindowSeconds)
        {
            _lastAnalysisTime = DateTime.UtcNow;
        }
    }

    public Task<Dictionary<string, long>> GetProtocolDistributionAsync()
    {
        return Task.FromResult(_protocolCounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }

    public Task<Dictionary<string, long>> GetTopTalkersAsync(int topN = 10)
    {
        var combined = _sourceIpCounts.Concat(_destIpCounts)
            .GroupBy(kvp => kvp.Key)
            .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value))
            .OrderByDescending(kvp => kvp.Value)
            .Take(topN)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return Task.FromResult(combined);
    }

    public Task<Dictionary<string, long>> GetTrafficTrendAsync()
    {
        var trend = new Dictionary<string, long>
        {
            ["min"] = _packetSizes.Min(),
            ["max"] = _packetSizes.Max(),
            ["avg"] = (long)_packetSizes.Average(),
            ["total"] = _packetSizes.Sum()
        };

        return Task.FromResult(trend);
    }
}

public static class AdvancedPacketCaptureExtensions
{
    public static IServiceCollection AddAdvancedPacketCapture(this IServiceCollection services, 
        Action<AdvancedPacketCaptureOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IAdvancedPacketCaptureService, AdvancedPacketCaptureService>();
        return services;
    }
}