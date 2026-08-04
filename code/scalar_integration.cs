#:sdk Microsoft.NET.Sdk.Web
#:package Scalar@1.0.0
#:package Microsoft.AspNetCore.Authentication.JwtBearer@8.0.0
#:package Swashbuckle.AspNetCore@6.5.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

public class ScalarOptions
{
    public bool EnableMetrics { get; set; } = true;
    public string? EncryptionKey { get; set; }
    public int BatchSize { get; set; } = 100;
    public TimeSpan BatchInterval { get; set; } = TimeSpan.FromSeconds(5);
    public int MaxRetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public int CircuitBreakerThreshold { get; set; } = 5;
    public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromMinutes(1);
    
    // API Configuration
    public string ApiTitle { get; set; } = "Scalar API";
    public string ApiVersion { get; set; } = "v1";
    public string JwtSecret { get; set; } = "your-256-bit-secret";
    public string JwtIssuer { get; set; } = "scalar-api";
    public string JwtAudience { get; set; } = "scalar-clients";
    public int JwtExpireMinutes { get; set; } = 60;
}

public interface IScalarService
{
    Task ProcessAsync<T>(T data, CancellationToken cancellationToken = default);
}

public class ScalarService : IScalarService
{
    private readonly ScalarOptions _options;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ScalarService> _logger;
    private readonly Channel<object> _channel = Channel.CreateBounded<object>(new BoundedChannelOptions(10000)
    {
        FullMode = BoundedChannelFullMode.Wait,
        SingleReader = true,
        SingleWriter = false
    });
    
    private readonly Timer _timer;
    private readonly Meter _meter = new("Scalar");
    private readonly Counter<int> _processedCounter;
    private readonly Counter<int> _errorCounter;
    private readonly Histogram<double> _processingTimeHistogram;
    private readonly AsyncPolicy _resiliencyPolicy;

    public ScalarService(
        IOptions<ScalarOptions> options,
        IMemoryCache cache,
        ILogger<ScalarService> logger)
    {
        _options = options.Value;
        _cache = cache;
        _logger = logger;
        
        _processedCounter = _meter.CreateCounter<int>("scalar.processed");
        _errorCounter = _meter.CreateCounter<int>("scalar.errors");
        _processingTimeHistogram = _meter.CreateHistogram<double>("scalar.processing_time", "ms");
        
        _resiliencyPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(_options.MaxRetryCount, 
                attempt => _options.RetryDelay,
                (ex, delay) => _logger.LogWarning(ex, "Retrying after delay {Delay}", delay))
            .WrapAsync(Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(_options.CircuitBreakerThreshold, 
                    _options.CircuitBreakerDuration,
                    (ex, duration) => _logger.LogError(ex, "Circuit breaker opened for {Duration}", duration),
                    () => _logger.LogInformation("Circuit breaker reset")));
        
        _timer = new Timer(
            ProcessBatchAsync, 
            null, 
            _options.BatchInterval, 
            _options.BatchInterval);
    }

    public async Task ProcessAsync<T>(T data, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(data!, cancellationToken);
    }

    private async Task ProcessBatchAsync(object? state = null)
    {
        var batch = new List<object>(_options.BatchSize);
        
        while (batch.Count < _options.BatchSize && 
               _channel.Reader.TryRead(out var item))
        {
            batch.Add(item);
        }
        
        if (batch.Count == 0) return;
        
        await _resiliencyPolicy.ExecuteAsync(async () =>
        {
            using var activity = new ActivityScope("ProcessBatch");
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // Process batch here
                _processedCounter.Add(batch.Count);
                _logger.LogInformation("Processed {Count} items", batch.Count);
            }
            catch (Exception ex)
            {
                _errorCounter.Add(batch.Count);
                _logger.LogError(ex, "Error processing batch");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                _processingTimeHistogram.Record(stopwatch.ElapsedMilliseconds);
            }
        });
    }

    public async ValueTask DisposeAsync()
    {
        await _timer.DisposeAsync();
        _meter.Dispose();
    }
}

public static class ScalarExtensions
{
    public static IServiceCollection AddScalarService(this IServiceCollection services, Action<ScalarOptions>? configure = null)
    {
        services.AddOptions<ScalarOptions>()
            .Configure(configure ?? (opt => { }));
            
        var options = services.BuildServiceProvider()
            .GetRequiredService<IOptions<ScalarOptions>>().Value;
            
        services.AddMemoryCache();
        services.AddSingleton<IScalarService, ScalarService>();
        services.AddHostedService<ScalarBackgroundService>();
        
        // Add JWT Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = options.JwtIssuer,
                    ValidAudience = options.JwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.JwtSecret))
                };
            });
        
        // Add Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(options.ApiVersion, new OpenApiInfo { Title = options.ApiTitle, Version = options.ApiVersion });
            
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        
        return services;
    }
    
    public static IApplicationBuilder UseScalarApi(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<ScalarOptions>>().Value;
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/{options.ApiVersion}/swagger.json", $"{options.ApiTitle} {options.ApiVersion}");
        });
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
}

internal sealed class ScalarBackgroundService : BackgroundService
{
    private readonly IScalarService _scalarService;
    
    public ScalarBackgroundService(IScalarService scalarService)
    {
        _scalarService = scalarService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _scalarService.ProcessBatchAsync(stoppingToken);
            await Task.Delay(100, stoppingToken);
        }
    }
}

[ApiController]
[Route("api/[controller]")]
public class ScalarController : ControllerBase
{
    private readonly IScalarService _scalarService;
    private readonly ILogger<ScalarController> _logger;
    private readonly Counter<int> _requestCounter;
    
    public ScalarController(
        IScalarService scalarService,
        ILogger<ScalarController> logger,
        IMeterFactory meterFactory)
    {
        _scalarService = scalarService;
        _logger = logger;
        _requestCounter = meterFactory.Create("ScalarApi").CreateCounter<int>("scalar_requests");
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        _requestCounter.Add(1);
        return Ok(new { message = "Scalar API is running" });
    }
    
    [HttpPost("process")]
    [Authorize]
    public async Task<IActionResult> Process([FromBody] object data)
    {
        _requestCounter.Add(1);
        await _scalarService.ProcessAsync(data);
        return Accepted();
    }
    
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new { Status = "Running" });
    }
}

// Usage example in Program.cs:
/*
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScalarService(opt => {
    opt.BatchSize = 200;
    opt.EnableMetrics = true;
    opt.JwtSecret = "your-secret-key";
    opt.JwtIssuer = "your-issuer";
    opt.JwtAudience = "your-audience";
});

var app = builder.Build();

app.UseScalarApi();

app.MapControllers();

app.Run();
*/