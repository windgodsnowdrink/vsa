#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Certes@4.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Certes;
using Certes.Acme;
using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Hosting;

/// <summary>
/// 证书请求类 - 用于表示ACME证书请求
/// </summary>
public class CertificateRequest
{
    /// <summary>
    /// 域名
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// 证书类型
    /// </summary>
    public CertificateType Type { get; set; }

    /// <summary>
    /// 额外的主题备用名称
    /// </summary>
    public string[] SubjectAlternativeNames { get; set; }
}

/// <summary>
/// 证书类型枚举
/// </summary>
public enum CertificateType
{
    /// <summary>
    /// 标准SSL证书
    /// </summary>
    Standard,

    /// <summary>
    /// 通配符证书
    /// </summary>
    Wildcard
}

/// <summary>
/// ACME协议客户端接口
/// </summary>
public interface IAcmeProtocolClient
{
    /// <summary>
    /// 初始化ACME上下文
    /// </summary>
    /// <returns>ACME上下文</returns>
    Task<AcmeContext> InitializeContextAsync();

    /// <summary>
    /// 验证域名所有权
    /// </summary>
    /// <param name="context">ACME上下文</param>
    /// <param name="domain">域名</param>
    /// <returns>验证结果</returns>
    Task<bool> VerifyDomainOwnershipAsync(AcmeContext context, string domain);

    /// <summary>
    /// 生成CSR信息
    /// </summary>
    /// <param name="request">证书请求</param>
    /// <returns>CSR信息</returns>
    CsrInfo GenerateCsrInfo(CertificateRequest request);
}

[SkipLocalsInit]
public sealed class CertesCertificateService : BackgroundService
{
    private readonly Channel<CertificateRequest> _requestChannel;
    private readonly ObjectPool<AcmeContext> _acmePool;
    private readonly IAcmeProtocolClient _acmeClient;

    /// <summary>
    /// 构造函数 - 初始化证书服务
    /// </summary>
    /// <param name="acmeClient">ACME协议客户端</param>
    public CertesCertificateService(IAcmeProtocolClient acmeClient)
    {
        _acmeClient = acmeClient;
        _requestChannel = Channel.CreateBounded<CertificateRequest>(100);
        _acmePool = CreateAcmeContextPool();
    }

    /// <summary>
    /// 创建ACME上下文对象池
    /// </summary>
    /// <returns>ACME上下文对象池</returns>
    private ObjectPool<AcmeContext> CreateAcmeContextPool()
    {
        var policy = new DefaultPooledObjectPolicy<AcmeContext>
        {
            Create = () => _acmeClient.InitializeContextAsync().GetAwaiter().GetResult(),
            Return = context => { /* 可以添加清理逻辑 */ }
        };

        return new DefaultObjectPool<AcmeContext>(policy, 10);
    }

    /// <summary>
    /// 提交证书请求
    /// </summary>
    /// <param name="request">证书请求</param>
    public void SubmitRequest(CertificateRequest request)
    {
        _requestChannel.Writer.TryWrite(request);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var req in _requestChannel.Reader.ReadAllAsync(ct))
        {
            var acme = _acmePool.Get();
            try
            {
                // 验证域名所有权
                var isVerified = await _acmeClient.VerifyDomainOwnershipAsync(acme, req.Domain);
                if (!isVerified)
                {
                    Console.WriteLine($"域名验证失败: {req.Domain}");
                    continue;
                }

                var order = await acme.NewOrder()
                    .Domain(req.Domain)
                    .CreateAsync();

                // 完成DNS-01挑战
                var authz = await order.Authorizations().First();
                var challenge = await authz.Dns();
                await challenge.Validate();

                // 生成CSR信息
                var csrInfo = _acmeClient.GenerateCsrInfo(req);

                // 获取证书
                var cert = await order.Generate(csrInfo, KeyAlgorithm.RS256);

                await SaveCertificateAsync(cert, req, ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"证书生成失败: {req.Domain}");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _acmePool.Return(acme);
            }
        }
    }

    /// <summary>
    /// 保存证书
    /// </summary>
    /// <param name="certificate">证书</param>
    /// <param name="request">证书请求</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>任务</returns>
    private async Task SaveCertificateAsync(Certes.Certificate certificate, CertificateRequest request, CancellationToken ct)
    {
        // 实现证书保存逻辑
        // 这里可以将证书保存到文件、密钥库或其他存储位置
        var certPem = await certificate.ToPem();
        var keyPem = await certificate.Key.ToPem();

        Console.WriteLine($"证书生成成功: {request.Domain}");
        Console.WriteLine($"证书PEM: {certPem.Substring(0, 100)}...");
        Console.WriteLine($"密钥PEM: {keyPem.Substring(0, 100)}...");

        // 模拟异步保存
        await Task.Delay(100, ct);
    }
}
