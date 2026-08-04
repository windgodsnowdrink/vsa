#:sdk Microsoft.NET.Sdk.Web
#:package OpenTelemetry.Extensions.Hosting@1.8.0
#:package OpenTelemetry.Instrumentation.AspNetCore@1.8.0
#:package OpenTelemetry.Exporter.Console@1.8.0
#:package OpenTelemetry.Exporter.OpenTelemetryProtocol@1.8.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

builder.Services.AddOpenTelemetry()
    .WithMetrics(metricsProviderBuilder => {
        metricsProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter();
    })
    .WithTracing(tracerProviderBuilder => {
        tracerProviderBuilder
            .AddSource("Carter")
            .AddAspNetCoreInstrumentation(options => {
                options.RecordException = true;
                options.Filter = (httpContext) => 
                    !httpContext.Request.Path.StartsWithSegments("/health");
            })
            .AddOtlpExporter();
    });

public abstract class CarterModuleBase : CarterModule, ICarterModule
{
    protected static readonly ActivitySource ActivitySource = new("Carter");
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        using var activity = ActivitySource.StartActivity($"{GetType().Name}.AddRoutes");
        // 原有路由注册逻辑
    }
}