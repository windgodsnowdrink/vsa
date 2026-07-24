#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web 
#:package Carter@10.0.0
#:package Scrutor@7.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.1
#:package Microsoft.Extensions.Logging@10.0.1
#:package Microsoft.Extensions.Hosting@10.0.1
#:package System.Threading.Channels@10.0.1
#:package Aspire.AppHost.Sdk@13.1.0
#:package Aspire.Hosting.AppHost@13.1.0
#:package Prometheus.Client@6.0.0 
#:package ModelContextProtocol@0.4.1-preview.1
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property RollForward=Major
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property GenerateDocumentationFile=true
#:property PublishAot=false
#:property Platform=Any CPU
#:property PublicSigning=true

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using Prometheus.Client;

/// <summary>
/// 证书监控器 - 用于监控SSL证书的过期时间，并通过Prometheus指标暴露
/// </summary>
[SkipLocalsInit]
public sealed class CertificateMonitor
{
    /// <summary>
    /// Prometheus指标：证书过期天数
    /// </summary>
    private readonly IMetricFamily<IGauge> _certExpiryGauge;

    /// <summary>
    /// 证书监控通道，用于接收待监控的证书
    /// </summary>
    private readonly Channel<X509Certificate2> _monitorChannel;

    /// <summary>
    /// 尾延迟优化器，用于优化监控操作的延迟
    /// </summary>
    private readonly TailLatencyOptimizer _latencyOptimizer;

    /// <summary>
    /// 构造函数 - 初始化证书监控器
    /// </summary>
    public CertificateMonitor()
    {
        // 初始化尾延迟优化器
        _latencyOptimizer = new TailLatencyOptimizer();

        // 创建有界通道，容量为1000
        _monitorChannel = Channel.CreateBounded<X509Certificate2>(1000);

        // 创建Prometheus指标
        _certExpiryGauge = Metrics.DefaultFactory.CreateGauge(
            "certificate_expiry_days",
            "证书过期剩余天数",
            "domain");
    }

    /// <summary>
    /// 开始监控证书
    /// </summary>
    /// <param name="cert">待监控的证书</param>
    public void StartMonitoring(X509Certificate2 cert)
    {
        _monitorChannel.Writer.TryWrite(cert);
    }

    /// <summary>
    /// 监控异步方法 - 从通道读取证书并监控
    /// </summary>
    /// <param name="ct">取消令牌</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task MonitorAsync(CancellationToken ct)
    {
        await foreach (var cert in _monitorChannel.Reader.ReadAllAsync(ct))
        {
            using var latencyToken = _latencyOptimizer.BeginOperation();

            // 计算证书剩余过期天数
            var expiryDays = (cert.NotAfter - DateTime.UtcNow).TotalDays;

            // 更新Prometheus指标
            _certExpiryGauge.WithLabels(cert.GetNameInfo(X509NameType.DnsName, false))
                .Set(expiryDays);

            // 如果证书将在7天内过期，触发告警
            if (expiryDays < 7)
            {
                TriggerAlert(cert);
            }
        }
    }

    /// <summary>
/// 触发证书过期告警
/// </summary>
/// <param name="cert">即将过期的证书</param>
private void TriggerAlert(X509Certificate2 cert)
{
    // TODO: 实现告警逻辑，例如发送邮件、短信或推送通知
    Console.WriteLine($"警告：证书 {cert.GetNameInfo(X509NameType.DnsName, false)} 将在 {(cert.NotAfter - DateTime.UtcNow).TotalDays:F2} 天后过期！");
}
}

/// <summary>
/// 尾延迟优化器 - 用于优化操作的尾延迟
/// </summary>
public class TailLatencyOptimizer
{
    /// <summary>
    /// 开始操作 - 创建一个延迟令牌
    /// </summary>
    /// <returns>延迟令牌，用于跟踪操作完成</returns>
    public IDisposable BeginOperation()
    {
        return new TailLatencyToken();
    }

    /// <summary>
    /// 尾延迟令牌 - 用于跟踪操作的执行时间
    /// </summary>
    private class TailLatencyToken : IDisposable
    {
        /// <summary>
        /// 操作开始时间
        /// </summary>
        private readonly DateTime _startTime;

        /// <summary>
        /// 构造函数 - 记录操作开始时间
        /// </summary>
        public TailLatencyToken()
        {
            _startTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 释放资源 - 计算操作执行时间
        /// </summary>
        public void Dispose()
        {
            var executionTime = DateTime.UtcNow - _startTime;
            // 这里可以添加尾延迟监控逻辑
            if (executionTime.TotalMilliseconds > 100)
            {
                Console.WriteLine($"警告：操作执行时间较长 - {executionTime.TotalMilliseconds:F2}ms");
            }
        }
    }
}
