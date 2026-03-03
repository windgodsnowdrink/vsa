using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;

public class PublicApiOptions
{
    public string BaseUrl { get; set; } = "https://api.publicapis.org";
    public int CacheDurationSeconds { get; set; } = 300;
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(1);
    public int CircuitBreakerThreshold { get; set; } = 5;
    public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromSeconds(30);
}

public class PublicApiClient
{
    // Art & Design API 示例
    public async IAsyncEnumerable<ArtPiece> StreamArtPiecesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in StreamAsync<ArtPiece>("art", cancellationToken))
            yield return item;
    }

    public async Task<PaginatedResponse<ArtPiece>> GetPaginatedArtPiecesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await GetPaginatedAsync<ArtPiece>("art", page, pageSize, cancellationToken);
    }

    public async Task<IReadOnlyList<ArtPiece>> BatchGetArtPiecesAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        return await BatchGetAsync<ArtPiece>("art", ids, cancellationToken);
    }

    // Authentication & Authorization API 示例
    public async IAsyncEnumerable<AuthProvider> StreamAuthProvidersAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in StreamAsync<AuthProvider>("auth", cancellationToken))
            yield return item;
    }

    // Blockchain API 示例
    public async IAsyncEnumerable<BlockchainAsset> StreamBlockchainAssetsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in StreamAsync<BlockchainAsset>("blockchain", cancellationToken))
            yield return item;
    }

    public async Task<PaginatedResponse<BlockchainAsset>> GetPaginatedBlockchainAssetsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await GetPaginatedAsync<BlockchainAsset>("blockchain", page, pageSize, cancellationToken);
    }

    // Books API 示例
    public async IAsyncEnumerable<Book> StreamBooksAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in StreamAsync<Book>("books", cancellationToken))
            yield return item;
    }

    public async Task<PaginatedResponse<Book>> GetPaginatedBooksAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await GetPaginatedAsync<Book>("books", page, pageSize, cancellationToken);
    }

    // Business API 示例
    public async IAsyncEnumerable<BusinessData> StreamBusinessDataAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in StreamAsync<BusinessData>("business", cancellationToken))
            yield return item;
    }

    // 其他类别API实现...
    public async Task<IReadOnlyList<AnimalFact>> GetAnimalFactsAsync(CancellationToken cancellationToken = default)
    {
        return await BatchGetAsync<AnimalFact>(new[] {
            "https://api.animal-facts.com/cats",
            "https://api.animal-facts.com/dogs"
        }, cancellationToken);
    }

    public IAsyncEnumerable<AnimalImage> StreamAnimalImagesAsync(string animalType, CancellationToken cancellationToken = default)
    {
        return StreamAsync<AnimalImage>($"https://api.animal-images.com/{animalType}", cancellationToken);
    }

    public async Task<PaginatedResult<Animal>> GetPaginatedAnimalsAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        return await GetPaginatedAsync<Animal>("https://api.animals.com/list", page, pageSize, cancellationToken: cancellationToken);
    }

    // Anime API 示例
    public async Task<AnimeInfo> GetAnimeInfoAsync(string animeId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<AnimeInfo>($"https://api.anime.info/{animeId}", cancellationToken: cancellationToken);
    }

    public IAsyncEnumerable<AnimeEpisode> StreamAnimeEpisodesAsync(string animeId, CancellationToken cancellationToken = default)
    {
        return StreamAsync<AnimeEpisode>($"https://api.anime.info/{animeId}/episodes", cancellationToken);
    }

    // Anti-Malware API 示例
    public async Task<MalwareReport> CheckUrlSafetyAsync(string url, CancellationToken cancellationToken = default)
    {
        return await GetAsync<MalwareReport>($"https://api.malware-check.com/url?q={Uri.EscapeDataString(url)}", cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<MalwareReport>> BatchCheckUrlsAsync(IEnumerable<string> urls, CancellationToken cancellationToken = default)
    {
        var tasks = urls.Select(url => CheckUrlSafetyAsync(url, cancellationToken));
        return await Task.WhenAll(tasks);
    }

    // 通用AOT优化方法
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private T DeserializeAotOptimized<T>(ReadOnlySpan<byte> jsonData)
    {
        return JsonSerializer.Deserialize<T>(jsonData, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            DefaultBufferSize = 1024
        });
    }
{
    private readonly ObjectPool<HttpClient> _httpClientPool;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _rateLimitSemaphores;
    private readonly Timer _cacheRefreshTimer;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PublicApiClient> _logger;
    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreaker;
    private readonly PublicApiOptions _options;

    public PublicApiClient(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<PublicApiClient> logger,
        IOptions<PublicApiOptions> options,
        ObjectPool<HttpClient> httpClientPool = null)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        _circuitBreaker = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(x => !x.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                _options.CircuitBreakerThreshold,
                _options.CircuitBreakerDuration,
                (ex, state, duration, context) =>
                {
                    _logger.LogWarning(ex.Exception, "Circuit opened for {Duration}ms", duration.TotalMilliseconds);
                },
                context =>
                {
                    _logger.LogInformation("Circuit reset");
                });
    }

    public async Task<T> GetAsync<T>(string endpoint, string cacheKey = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(cacheKey))
        {
            cacheKey = $"{endpoint}:{typeof(T).Name}";
        }

        return await ExecutePolicyAndGetResult<T>(() => _httpClient.GetAsync(endpoint), cacheKey, cancellationToken);
    }

    public async IAsyncEnumerable<T> StreamAsync<T>(string endpoint, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _circuitBreaker.ExecuteAsync(() => 
            _httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead, cancellationToken));

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        using var json = new JsonTextReader(reader);

        var serializer = new JsonSerializer();
        while (await json.ReadAsync(cancellationToken))
        {
            if (json.TokenType == JsonToken.StartObject)
            {
                yield return serializer.Deserialize<T>(json);
            }
        }
    }

    public async Task<PaginatedResult<T>> GetPaginatedAsync<T>(string endpoint, int page = 1, int pageSize = 10, string cacheKey = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(cacheKey))
        {
            cacheKey = $"{endpoint}:{page}:{pageSize}:{typeof(T).Name}";
        }

        var paginatedUri = $"{endpoint}?page={page}&pageSize={pageSize}";
        return await ExecutePolicyAndGetResult<PaginatedResult<T>>(() => _httpClient.GetAsync(paginatedUri), cacheKey, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> BatchGetAsync<T>(IEnumerable<string> endpoints, CancellationToken cancellationToken = default)
    {
        var tasks = endpoints.Select(endpoint => GetAsync<T>(endpoint, cancellationToken: cancellationToken));
        return await Task.WhenAll(tasks);
    }
    {
        var cacheKey = $"publicapi:{endpoint}";
        
        if (_cache.TryGetValue(cacheKey, out T cachedResult))
        {
            return cachedResult;
        }

        var response = await ExecuteWithResilienceAsync(
            () => _httpClient.GetAsync(endpoint, cancellationToken),
            endpoint,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        
        var result = await JsonSerializer.DeserializeAsync<T>(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            cancellationToken: cancellationToken);
            
        _cache.Set(cacheKey, result, TimeSpan.FromSeconds(_options.CacheDurationSeconds));
        
        return result!;
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        using var content = CreateJsonContent(request);
        var response = await ExecuteWithResilienceAsync(
            () => _httpClient.PostAsync(endpoint, content, cancellationToken),
            endpoint,
            cancellationToken);

        return await HandleResponse<TResponse>(response, cancellationToken);
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        using var content = CreateJsonContent(request);
        var response = await ExecuteWithResilienceAsync(
            () => _httpClient.PutAsync(endpoint, content, cancellationToken),
            endpoint,
            cancellationToken);

        return await HandleResponse<TResponse>(response, cancellationToken);
    }

    public async Task<TResponse> PatchAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        using var content = CreateJsonContent(request);
        var response = await ExecuteWithResilienceAsync(
            () => _httpClient.PatchAsync(endpoint, content, cancellationToken),
            endpoint,
            cancellationToken);

        return await HandleResponse<TResponse>(response, cancellationToken);
    }

    public async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await ExecuteWithResilienceAsync(
            () => _httpClient.DeleteAsync(endpoint, cancellationToken),
            endpoint,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private async Task<HttpResponseMessage> ExecuteWithResilienceAsync(
        Func<Task<HttpResponseMessage>> operation,
        string endpoint,
        CancellationToken cancellationToken)
    {
        return await _circuitBreaker.ExecuteAsync(async () =>
        {
            return await Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    _options.RetryCount,
                    attempt => _options.RetryInterval,
                    (outcome, delay, retryCount, context) =>
                    {
                        _logger.LogWarning(outcome.Exception ?? new Exception(outcome.Result?.StatusCode.ToString()),
                            "Retry {RetryCount} for {Endpoint}", retryCount, endpoint);
                    })
                .ExecuteAsync(operation);
        });
    }

    private HttpContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        response.EnsureSuccessStatusCode();
        return await JsonSerializer.DeserializeAsync<T>(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            cancellationToken: cancellationToken) ?? throw new InvalidOperationException("Null response content");
    }
}

// AOT优化配置
[JsonSerializable(typeof(AnimalFact))]
[JsonSerializable(typeof(AnimalImage))]
[JsonSerializable(typeof(Animal))]
[JsonSerializable(typeof(AnimeInfo))]
[JsonSerializable(typeof(AnimeEpisode))]
[JsonSerializable(typeof(MalwareReport))]
[JsonSerializable(typeof(PaginatedResult<>))]
public partial class PublicApiJsonContext : JsonSerializerContext {}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPublicApiClient(this IServiceCollection services, Action<PublicApiOptions> configure)
    {
        services.Configure(configure);
        
        services.AddHttpClient<PublicApiClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
                MaxConnectionsPerServer = 100
            });
            
        services.AddMemoryCache();
        
        return services;
    }
}