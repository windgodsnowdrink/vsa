#:sdk Microsoft.NET.Sdk.Web
#:package Swashbuckle.AspNetCore@6.5.0
#:package Microsoft.OpenApi@1.6.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;

public static class OpenApiMetrics
{
    public static readonly Meter Meter = new("OpenAPI");
    public static readonly Counter<int> RequestCounter = Meter.CreateCounter<int>("openapi_requests");
    public static readonly Histogram<double> ResponseTime = Meter.CreateHistogram<double>("openapi_response_time", "ms");
    public static readonly ObservableGauge<int> CacheHitRate = Meter.CreateObservableGauge<int>("openapi_cache_hit_rate", () => 
        CachingSwaggerProvider.CacheHitCount * 100 / (CachingSwaggerProvider.CacheHitCount + CachingSwaggerProvider.CacheMissCount));
}

public class CachingSwaggerProvider : ISwaggerProvider
{
    private readonly ISwaggerProvider _inner;
    private readonly IMemoryCache _cache;
    public static int CacheHitCount = 0;
    public static int CacheMissCount = 0;

    public CachingSwaggerProvider(ISwaggerProvider inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public OpenApiDocument GetSwagger(string documentName, string? host = null, string? basePath = null)
    {
        var cacheKey = $"{documentName}_{host}_{basePath}";
        
        if (_cache.TryGetValue(cacheKey, out OpenApiDocument? cachedDoc))
        {
            Interlocked.Increment(ref CacheHitCount);
            return cachedDoc!;
        }

        Interlocked.Increment(ref CacheMissCount);
        var doc = _inner.GetSwagger(documentName, host, basePath);
        _cache.Set(cacheKey, doc);
        return doc;
    }
}

public class OpenApiMetricsService : IHostedService
{
    private readonly OpenApiOptions _options;
    private Timer? _timer;

    public OpenApiMetricsService(IOptions<OpenApiOptions> options)
    {
        _options = options.Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_options.EnableMetrics)
        {
            _timer = new Timer(_ => 
            {
                OpenApiMetrics.RequestCounter.Add(1);
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        return Task.CompletedTask;
    }
}

public class OpenApiOptions
{
    public string Title { get; set; } = "API Documentation";
    public string Version { get; set; } = "v1";
    public string Description { get; set; } = string.Empty;
    public bool EnableMetrics { get; set; } = true;
    public bool EnableSecurity { get; set; } = true;
    public bool EnableCaching { get; set; } = true;
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);
    public List<string> Versions { get; set; } = new() { "v1" };
    public bool EnableRequestTracking { get; set; } = true;
}

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApi(this IServiceCollection services, Action<OpenApiOptions> configure)
    {
        var options = new OpenApiOptions();
        configure(options);

        services.AddSingleton(options);
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(options.Version, new OpenApiInfo 
            { 
                Title = options.Title, 
                Version = options.Version,
                Description = options.Description
            });

            if (options.EnableSecurity)
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
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
            }
        });

        if (options.EnableMetrics)
        {
            services.AddSingleton(OpenApiMetrics.Meter);
            services.AddHostedService<OpenApiMetricsService>();
        }
        
        if (options.EnableCaching)
        {
            services.AddMemoryCache();
            services.Decorate<ISwaggerProvider, CachingSwaggerProvider>();
        }

        return services;
    }

    public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<OpenApiOptions>();
        
        app.UseSwagger(c => 
        {
            c.RouteTemplate = "swagger/{documentName}/swagger.json";
            c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                if (options.EnableCaching)
                {
                    httpReq.HttpContext.Response.Headers.CacheControl = 
                        $"public,max-age={(int)options.CacheDuration.TotalSeconds}";
                }
            });
        });
        
        app.UseSwaggerUI(c => 
        {
            foreach (var version in options.Versions)
            {
                c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{options.Title} {version}");
            }
            c.RoutePrefix = string.Empty;
            c.ConfigObject.AdditionalItems["requestSnippetsEnabled"] = options.EnableRequestTracking;
        });

        return app;
    }
}

// Example usage in Program.cs:
/*
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options => 
{
    options.Title = "My API";
    options.Version = "v1";
    options.Description = "Production-ready API documentation";
    options.EnableMetrics = true;
    options.EnableSecurity = true;
});

var app = builder.Build();
app.UseOpenApi();
app.Run();
*/