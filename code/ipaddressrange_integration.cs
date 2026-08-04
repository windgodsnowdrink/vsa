using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace IPAddressRangeIntegration
{
    public enum IpRangeCacheType
    {
        None,
        Memory,
        Distributed
    }

    public class IpRangeOptions
    {
        public IpRangeCacheType CacheType { get; set; } = IpRangeCacheType.Memory;
        public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(30);
        public string? RedisConnectionString { get; set; }
        public int ObjectPoolSize { get; set; } = 10;
        public bool EnableDistributedTracing { get; set; } = false;
        public string? TracingEndpoint { get; set; }
    }

    public interface IIpRangeService
    {
        Task<bool> IsInRangeAsync(string ipAddress, string rangeDefinition, CancellationToken ct = default);
        Task<IReadOnlyCollection<string>> FindMatchingRangesAsync(string ipAddress, IEnumerable<string> rangeDefinitions, CancellationToken ct = default);
        
        // 新增高级功能
        Task<string> MergeRangesAsync(IEnumerable<string> rangeDefinitions, CancellationToken ct = default);
        Task<IReadOnlyCollection<string>> SplitRangeByCidrAsync(string rangeDefinition, int cidr, CancellationToken ct = default);
        Task<IReadOnlyCollection<IPAddress>> EnumerateRangeAsync(string rangeDefinition, CancellationToken ct = default);
        Task<IDictionary<string, bool>> BatchCheckInRangeAsync(IEnumerable<string> ipAddresses, string rangeDefinition, CancellationToken ct = default);
        Task<IPRangeInfo> GetRangeInfoAsync(string rangeDefinition, CancellationToken ct = default);
    }
    
    public class IPRangeInfo
    {
        public string StartAddress { get; set; } = null!;
        public string EndAddress { get; set; } = null!;
        public long TotalAddresses { get; set; }
        public string? NetworkClass { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsReserved { get; set; }
        public string? SuggestedCidr { get; set; }
    }

    public class IpRangeService : IIpRangeService, IDisposable
    {
        private static readonly Dictionary<string, string> NetworkClasses = new()
        {
            ["0.0.0.0-127.255.255.255"] = "A",
            ["128.0.0.0-191.255.255.255"] = "B",
            ["192.0.0.0-223.255.255.255"] = "C",
            ["224.0.0.0-239.255.255.255"] = "D",
            ["240.0.0.0-255.255.255.255"] = "E"
        };
        
        private static readonly string[] PrivateRanges = 
        {
            "10.0.0.0-10.255.255.255",
            "172.16.0.0-172.31.255.255",
            "192.168.0.0-192.168.255.255"
        };
        
        private static readonly string[] ReservedRanges = 
        {
            "0.0.0.0-0.255.255.255",
            "100.64.0.0-100.127.255.255",
            "127.0.0.0-127.255.255.255",
            "169.254.0.0-169.254.255.255",
            "192.0.0.0-192.0.0.255",
            "192.0.2.0-192.0.2.255",
            "192.88.99.0-192.88.99.255",
            "198.18.0.0-198.19.255.255",
            "198.51.100.0-198.51.100.255",
            "203.0.113.0-203.0.113.255",
            "224.0.0.0-239.255.255.255",
            "240.0.0.0-255.255.255.255"
        };
        private readonly IMemoryCache _cache;
        private readonly IpRangeOptions _options;
        private readonly Channel<(string, TaskCompletionSource<bool>)> _processingChannel;
        private readonly ObjectPool<IPAddressRange> _rangePool;
        private readonly ConcurrentDictionary<string, IPAddressRange> _rangeCache = new();
        private readonly ILogger<IpRangeService> _logger;

        public IpRangeService(
            IMemoryCache cache,
            IOptions<IpRangeOptions> options,
            ILogger<IpRangeService> logger)
        {
            _cache = cache;
            _options = options.Value;
            _logger = logger;

            _rangePool = new DefaultObjectPool<IPAddressRange>(
                new IPAddressRangePooledObjectPolicy(),
                _options.ObjectPoolSize);

            _processingChannel = Channel.CreateUnbounded<(string, TaskCompletionSource<bool>)>(
                new UnboundedChannelOptions { SingleReader = true });

            _ = Task.Run(ProcessRequestsAsync);
        }

        public async Task<bool> IsInRangeAsync(string ipAddress, string rangeDefinition, CancellationToken ct = default)
        {
            if (_options.CacheType == IpRangeCacheType.None)
            {
                return await CheckRangeWithoutCache(ipAddress, rangeDefinition, ct);
            }

            var cacheKey = $"{ipAddress}:{rangeDefinition}";
            if (_cache.TryGetValue(cacheKey, out bool result))
            {
                return result;
            }

            result = await CheckRangeWithoutCache(ipAddress, rangeDefinition, ct);
            _cache.Set(cacheKey, result, _options.CacheExpiration);
            return result;
        }

        public async Task<IReadOnlyCollection<string>> FindMatchingRangesAsync(
            string ipAddress, 
            IEnumerable<string> rangeDefinitions, 
            CancellationToken ct = default)
        {
            var results = new ConcurrentBag<string>();
            await Parallel.ForEachAsync(rangeDefinitions, ct, async (range, innerCt) =>
            {
                if (await IsInRangeAsync(ipAddress, range, innerCt))
                {
                    results.Add(range);
                }
            });
            return results.ToArray();
        }

        private async Task<bool> CheckRangeWithoutCache(string ipAddress, string rangeDefinition, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<bool>();
            await _processingChannel.Writer.WriteAsync((ipAddress, tcs), ct);
            return await tcs.Task;
        }

        private async Task ProcessRequestsAsync()
        {
            while (await _processingChannel.Reader.WaitToReadAsync())
            {
                while (_processingChannel.Reader.TryRead(out var request))
                {
                    try
                    {
                        var range = _rangePool.Get();
                        try
                        {
                            var (ipAddress, tcs) = request;
                            var result = range.Contains(IPAddress.Parse(ipAddress));
                            tcs.SetResult(result);
                        }
                        finally
                        {
                            _rangePool.Return(range);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing IP range request");
                        request.Item2.SetException(ex);
                    }
                }
            }
        }

        public async Task<string> MergeRangesAsync(IEnumerable<string> rangeDefinitions, CancellationToken ct = default)
        {
            var ranges = rangeDefinitions.Select(r => IPAddressRange.Parse(r)).ToList();
            var merged = IPAddressRange.Merge(ranges);
            return merged.ToString();
        }
        
        public async Task<IReadOnlyCollection<string>> SplitRangeByCidrAsync(string rangeDefinition, int cidr, CancellationToken ct = default)
        {
            var range = IPAddressRange.Parse(rangeDefinition);
            var subnets = range.Split(cidr);
            return subnets.Select(s => s.ToString()).ToArray();
        }
        
        public async Task<IReadOnlyCollection<IPAddress>> EnumerateRangeAsync(string rangeDefinition, CancellationToken ct = default)
        {
            var range = IPAddressRange.Parse(rangeDefinition);
            return range.GetAllAddresses().ToArray();
        }
        
        public async Task<IDictionary<string, bool>> BatchCheckInRangeAsync(IEnumerable<string> ipAddresses, string rangeDefinition, CancellationToken ct = default)
        {
            var range = IPAddressRange.Parse(rangeDefinition);
            return ipAddresses.ToDictionary(
                ip => ip, 
                ip => range.Contains(IPAddress.Parse(ip)));
        }
        
        public async Task<IPRangeInfo> GetRangeInfoAsync(string rangeDefinition, CancellationToken ct = default)
        {
            var range = IPAddressRange.Parse(rangeDefinition);
            var info = new IPRangeInfo
            {
                StartAddress = range.Begin.ToString(),
                EndAddress = range.End.ToString(),
                TotalAddresses = range.GetAllAddresses().LongCount(),
                IsPrivate = PrivateRanges.Any(r => IPAddressRange.Parse(r).Contains(range)),
                IsReserved = ReservedRanges.Any(r => IPAddressRange.Parse(r).Contains(range)),
                SuggestedCidr = CalculateOptimalCidr(range)
            };
            
            info.NetworkClass = NetworkClasses.FirstOrDefault(
                x => IPAddressRange.Parse(x.Key).Contains(range)).Value;
                
            return info;
        }
        
        private string? CalculateOptimalCidr(IPAddressRange range)
        {
            var count = range.GetAllAddresses().LongCount();
            if (count <= 0) return null;
            
            var bits = (int)Math.Ceiling(Math.Log(count, 2));
            return 32 - bits >= 0 ? $"/{32 - bits}" : null;
        }
        
        public void Dispose()
        {
            _processingChannel.Writer.Complete();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIpRangeService(this IServiceCollection services, Action<IpRangeOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<IIpRangeService, IpRangeService>();
            services.AddMemoryCache();
            return services;
        }
    }

    internal class IPAddressRangePooledObjectPolicy : IPooledObjectPolicy<IPAddressRange>
    {
        public IPAddressRange Create() => new IPAddressRange();
        public bool Return(IPAddressRange obj) => true;
    }
}