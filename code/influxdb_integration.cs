#:sdk Microsoft.NET.Sdk
#:package InfluxDB.Client@3.0.0
#:package Microsoft.Extensions.Hosting@8.0.0
#:package Microsoft.Extensions.Logging@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;

public static class InfluxDbExtensions
{
    public static IServiceCollection AddInfluxDb(this IServiceCollection services, string url, string token, string org, string bucket)
    {
        services.AddSingleton(sp => InfluxDBClientFactory.Create(url, token.ToCharArray()));
        services.AddSingleton(new InfluxDbOptions(org, bucket));
        services.AddHostedService<InfluxDbBackgroundWriter>();
        return services;
    }
}

public record InfluxDbOptions(string Org, string Bucket);

public sealed class InfluxDbBackgroundWriter : BackgroundService, IInfluxDbWriter, IDisposable
{
    private readonly Channel<PointData> _channel;
    private readonly ArrayPool<PointData> _pool;
    private readonly IResiliencePipeline _resiliencePipeline;
    private readonly ILogger<InfluxDbBackgroundWriter> _logger;
    private readonly InfluxDbClient _client;
    
    public InfluxDbBackgroundWriter(
        IOptions<InfluxDbOptions> options,
        IResiliencePipelineProvider pipelineProvider,
        ILogger<InfluxDbBackgroundWriter> logger)
    {
        _channel = Channel.CreateBounded<PointData>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });
        
        _pool = ArrayPool<PointData>.Create();
        _resiliencePipeline = pipelineProvider.GetPipeline("influx-retry");
        _logger = logger;
        _client = new InfluxDbClient(options.Value);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var batch in _channel.Reader.ReadAllAsync(stoppingToken)
            .Buffer(100, TimeSpan.FromMilliseconds(100)))
        {
            try
            {
                await _resiliencePipeline.ExecuteAsync(async token =>
                {
                    using var activity = Activity.Current?.Source
                        .StartActivity("InfluxDb.WriteBatch");
                        
                    var array = _pool.Rent(batch.Count);
                    try
                    {
                        batch.CopyTo(array);
                        await _client.WriteAsync(array.AsMemory(0, batch.Count), token);
                    }
                    finally
                    {
                        _pool.Return(array);
                    }
                }, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write batch to InfluxDB");
            }
        }
    }
    
    public ValueTask WriteAsync(PointData point, CancellationToken cancellationToken = default)
    {
        return new ValueTask(_channel.Writer.WriteAsync(point, cancellationToken).AsTask());
    }
    
    public void Dispose()
    {
        _client.Dispose();
    }
} : BackgroundService
{
    private readonly InfluxDBClient _client;
    private readonly InfluxDbOptions _options;
    private readonly ILogger<InfluxDbBackgroundWriter> _logger;
    private readonly Channel<PointData> _channel = Channel.CreateBounded<PointData>(10000);
    private readonly Meter _meter = new("InfluxDB.Writer");
    private readonly Histogram<double> _writeDuration;

    public InfluxDbBackgroundWriter(InfluxDBClient client, InfluxDbOptions options, ILogger<InfluxDbBackgroundWriter> logger)
    {
        _client = client;
        _options = options;
        _logger = logger;
        _writeDuration = _meter.CreateHistogram<double>("write_duration_seconds", "seconds", "Duration of write operations");
    }

    public async ValueTask WriteAsync(PointData point, CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(point, ct);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var writeApi = _client.GetWriteApiAsync();
        var batch = new List<PointData>(1000);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var point = await _channel.Reader.ReadAsync(stoppingToken);
                batch.Add(point);
                
                if (batch.Count >= 1000 || _channel.Reader.Count > 0)
                {
                    using var timer = _writeDuration.NewTimer();
                    await writeApi.WritePointsAsync(batch, _options.Bucket, _options.Org);
                    batch.Clear();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing to InfluxDB");
            }
        }
    }
}

public sealed class InfluxDbQueryService : IInfluxDbQueryService, IDiagnosticListener
{
    private readonly InfluxDbClient _client;
    private readonly ILogger<InfluxDbQueryService> _logger;
    
    public InfluxDbQueryService(
        IOptions<InfluxDbOptions> options,
        ILogger<InfluxDbQueryService> logger)
    {
        _client = new InfluxDbClient(options.Value);
        _logger = logger;
    }
    
    public async IAsyncEnumerable<T> QueryAsync<T>(string query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var activity = Activity.Current?.Source
            .StartActivity("InfluxDb.Query");
            
        await foreach (var item in _client.QueryAsync<T>(query, cancellationToken))
        {
            yield return item;
        }
    }
    
    public void OnCompleted() { }
    
    public void OnError(Exception error)
    {
        _logger.LogError(error, "Diagnostic listener error");
    }
    
    public void OnNext(KeyValuePair<string, object?> value)
    {
        if (value.Key == "InfluxDb.Query")
        {
            _logger.LogInformation("Query executed: {Query}", value.Value);
        }
    }
}
{
    private readonly InfluxDBClient _client;
    private readonly InfluxDbOptions _options;

    public InfluxDbQueryService(InfluxDBClient client, InfluxDbOptions options)
    {
        _client = client;
        _options = options;
    }

    public async Task<List<T>> QueryAsync<T>(string fluxQuery, CancellationToken ct = default)
    {
        var queryApi = _client.GetQueryApi();
        return await queryApi.QueryAsync<T>(fluxQuery, _options.Org, ct);
    }

    public async IAsyncEnumerable<T> StreamQueryAsync<T>(string fluxQuery, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var queryApi = _client.GetQueryApi();
        await foreach (var record in queryApi.QueryAsyncEnumerable<T>(fluxQuery, _options.Org, ct))
        {
            yield return record;
        }
    }
}