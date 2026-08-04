#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Options@8.0.0
#:package System.Diagnostics.DiagnosticSource@8.0.0
#:property TargetFramework=net10.0
#:property Nullable=enable

using System.Security.Cryptography;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FingerprintIntegration;

public class Sha256FingerprintOptions
{
    public int HashIterations { get; set; } = 100000;
    public int SaltSize { get; set; } = 32;
    public int MaxConcurrentOperations { get; set; } = 100;
    public TimeSpan OperationTimeout { get; set; } = TimeSpan.FromSeconds(30);
}

public interface ISha256FingerprintService
{
    Task<byte[]> ComputeFingerprintAsync(byte[] data, CancellationToken ct = default);
    Task<bool> VerifyFingerprintAsync(byte[] data, byte[] expectedHash, CancellationToken ct = default);
    IAsyncEnumerable<byte[]> ComputeFingerprintsAsync(IAsyncEnumerable<byte[]> dataStream);
}

public class Sha256FingerprintService : ISha256FingerprintService, IDisposable
{
    private readonly Sha256FingerprintOptions _options;
    private readonly ILogger<Sha256FingerprintService> _logger;
    private readonly Meter _meter = new("Sha256Fingerprint");
    private readonly Channel<Func<Task>> _workChannel;
    private readonly CancellationTokenSource _cts = new();
    private readonly MemoryPool<byte> _memoryPool = MemoryPool<byte>.Shared;
    private readonly ActivitySource _activitySource;
    private readonly Histogram<double> _processingTimeHistogram;
    private readonly Counter<int> _processedItemsCounter;
    [ThreadStatic]
    private static Span<byte> _threadLocalBuffer;
    
    public Sha256FingerprintService(
        IOptions<Sha256FingerprintOptions> options, 
        ILogger<Sha256FingerprintService> logger)
    {
        _options = options.Value;
        _logger = logger;
        
        _workChannel = Channel.CreateBounded<Func<Task>>(
            new BoundedChannelOptions(_options.MaxConcurrentOperations)
            {
                SingleWriter = false,
                SingleReader = false,
                FullMode = BoundedChannelFullMode.Wait
            });
        
        _activitySource = new ActivitySource("Sha256FingerprintService");
        _processingTimeHistogram = _meter.CreateHistogram<double>("processing_time_ms", "milliseconds");
        _processedItemsCounter = _meter.CreateCounter<int>("processed_items", "items");
        
        StartWorkers();
    }

    private void StartWorkers()
    {
        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            _ = Task.Run(async () =>
            {
                while (await _workChannel.Reader.WaitToReadAsync(_cts.Token))
                {
                    if (_workChannel.Reader.TryRead(out var workItem))
                    {
                        try { await workItem(); }
                        catch (Exception ex) { _logger.LogError(ex, "Fingerprint computation failed"); }
                    }
                }
            }, _cts.Token);
        }
    }

    public async Task<byte[]> ComputeFingerprintAsync(byte[] data, CancellationToken ct = default)
    {
        using var activity = new ActivitySource(nameof(Sha256FingerprintService)).StartActivity("ComputeFingerprint");
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct, _cts.Token);
        cts.CancelAfter(_options.OperationTimeout);
        
        var tcs = new TaskCompletionSource<byte[]>();
        await _workChannel.Writer.WriteAsync(async () =>
        {
            try
            {
                using var memory = _memoryPool.Rent(data.Length + _options.SaltSize);
                using var sha256 = SHA256.Create();
                
                var salt = memory.Memory.Slice(0, _options.SaltSize);
                RandomNumberGenerator.Fill(salt.Span);
                
                data.CopyTo(memory.Memory.Slice(_options.SaltSize));
                
                var hash = memory.Memory;
                for (int i = 0; i < _options.HashIterations; i++)
                {
                    hash = sha256.ComputeHash(hemory.Memory.Span);
                }
                
                tcs.TrySetResult(hash.ToArray());
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }, cts.Token);
        
        return await tcs.Task;
    }

    public async Task<bool> VerifyFingerprintAsync(byte[] data, byte[] expectedHash, CancellationToken ct = default)
    {
        var computedHash = await ComputeFingerprintAsync(data, ct);
        return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
    }

    public async IAsyncEnumerable<byte[]> ComputeFingerprintsAsync(IAsyncEnumerable<byte[]> dataStream)
    {
        await foreach (var data in dataStream)
        {
            yield return await ComputeFingerprintAsync(data);
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _meter.Dispose();
        _workChannel.Writer.Complete();
        GC.SuppressFinalize(this);
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSha256FingerprintService(
        this IServiceCollection services,
        Action<Sha256FingerprintOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<ISha256FingerprintService, Sha256FingerprintService>();
        return services;
    }
}