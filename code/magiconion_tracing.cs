#:sdk Microsoft.NET.Sdk.Web
#:package MagicOnion@5.0.0
#:package OpenTelemetry.Exporter.OpenTelemetryProtocol@1.7.0
#:property LangVersion preview
#:property TargetFramework net10.0

// OpenTelemetry配置
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddMagicOnionInstrumentation()
        .AddOtlpExporter(o => 
        {
            o.Endpoint = new Uri("http://collector:4317");
            o.ExportProcessorType = ExportProcessorType.Batch;
        }));

// 高性能追踪处理器
builder.Services.AddSingleton<ITraceProcessor>(sp => 
    new ChannelTraceProcessor(
        Channel.CreateBounded<Activity>(10000),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[256])));