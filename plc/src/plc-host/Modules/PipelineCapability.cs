using System.Diagnostics;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plc.Host.Foundation;
using Plc.Host.Pipeline;
using Plc.Plugins.Contracts;

namespace Plc.Host.Modules;

/// <summary>
/// 采集管道能力（ADR-119 M3）：把 Disruptor 4.0.0 环形缓冲区装配进宿主，
/// 为采集侧提供无锁、有背压的帧流水线。首个阶段默认挂 <see cref="MetricsFrameHandler"/> 做计量。
/// </summary>
/// <remarks>
/// 配置项：<c>Pipeline:RingSize</c>（2 的幂，默认 1024）、<c>Pipeline:DropOnFull</c>（默认 true）、
/// <c>Pipeline:ShutdownTimeoutMs</c>（默认 5000）。
/// </remarks>
public sealed class PipelineCapability : ICapabilityModule, ICarterModule
{
    public string Id => "pipeline.acquisition";

    public string Name => "采集管道（Disruptor 环形缓冲区）";

    // 晚于并发底座（2）、早于设备协议模块（5）
    public int Order => 3;

    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton(sp => BuildOptions(sp.GetRequiredService<IConfiguration>()));
        services.AddSingleton<MetricsFrameHandler>();
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<PipelineOptions>();
            var logger = sp.GetRequiredService<ILogger<AcquisitionPipeline>>();
            var metrics = sp.GetRequiredService<MetricsFrameHandler>();

            var pipeline = new AcquisitionPipeline(options, logger);
            pipeline.AddStage(metrics);
            pipeline.Start();
            return pipeline;
        });
    }

    private static PipelineOptions BuildOptions(IConfiguration config)
    {
        var ringSize = config.GetValue<int?>("Pipeline:RingSize");
        if (ringSize is not null && (ringSize < 2 || (ringSize & (ringSize - 1)) != 0))
        {
            // 配置写错时向上取到最近的 2 的幂并记录，而不是让 Disruptor 在运行时炸掉
            var corrected = 1;
            while (corrected < ringSize.Value)
            {
                corrected <<= 1;
            }

            ringSize = corrected;
        }

        return new PipelineOptions(
            RingSize: ringSize ?? 1024,
            DropOnFull: config.GetValue<bool?>("Pipeline:DropOnFull") ?? true,
            ShutdownTimeoutMs: config.GetValue<int?>("Pipeline:ShutdownTimeoutMs") ?? 5000);
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sys/pipeline", (
            AcquisitionPipeline pipeline, MetricsFrameHandler metrics, PipelineOptions options) =>
            Results.Ok(new
            {
                ringSize = options.RingSize,
                dropOnFull = options.DropOnFull,
                started = pipeline.IsStarted,
                remainingCapacity = pipeline.RemainingCapacity,
                published = pipeline.Published,
                dropped = pipeline.Dropped,
                failed = pipeline.Failed,
                processed = metrics.Processed,
                failedFrames = metrics.FailedFrames,
                avgLatencyMs = metrics.AverageLatency?.TotalMilliseconds,
                maxLatencyMs = metrics.MaxLatency?.TotalMilliseconds,
            }));

        /// <summary>
        /// 吞吐探针：连续投递 <paramref name="count"/> 帧，返回实测吞吐与丢弃数（M6 门禁用）。
        /// 用 <see cref="AcquisitionPipeline.TryPublish"/>，因此缓冲区满时丢弃而非阻塞。
        /// </summary>
        app.MapPost("/sys/pipeline/probe", (
            AcquisitionPipeline pipeline,
            MetricsFrameHandler metrics,
            int count = 1000,
            int payloadBytes = 64) =>
        {
            count = Math.Clamp(count, 1, 200_000);
            payloadBytes = Math.Clamp(payloadBytes, 0, 4096);

            var request = new ReadRequest(0, 1, RegisterType.HoldingRegister);
            var payload = new byte[payloadBytes];
            Random.Shared.NextBytes(payload);

            var publishedBefore = pipeline.Published;
            var droppedBefore = pipeline.Dropped;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                pipeline.TryPublish(DeviceFrame.Read("probe.sim", "probe-device", request, payload, i));
            }

            sw.Stop();

            return Results.Ok(new
            {
                requested = count,
                published = pipeline.Published - publishedBefore,
                dropped = pipeline.Dropped - droppedBefore,
                elapsedMs = sw.ElapsedMilliseconds,
                throughputPerSec = sw.Elapsed.TotalSeconds > 0
                    ? Math.Round((pipeline.Published - publishedBefore) / sw.Elapsed.TotalSeconds)
                    : 0,
                processed = metrics.Processed,
            });
        });
    }
}
