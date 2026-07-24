#:sdk Microsoft.NET.Sdk.Web
#:package Certes@4.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable

using System.Security.Cryptography.X509Certificates;
using Certes;
using Certes.Acme;
using System.Threading.Channels;

[SkipLocalsInit]
public sealed class CertesCertificateService : BackgroundService
{
    private readonly Channel<CertificateRequest> _requestChannel;
    private readonly ObjectPool<AcmeContext> _acmePool;
    private readonly IAcmeProtocolClient _acmeClient;
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var req in _requestChannel.Reader.ReadAllAsync(ct))
        {
            var acme = _acmePool.Get();
            try {
                var order = await acme.NewOrder()
                    .Domain(req.Domain)
                    .CreateAsync();
                
                // 完成DNS-01挑战
                var authz = await order.Authorizations().First();
                var challenge = await authz.Dns();
                await challenge.Validate();
                
                // 获取证书
                var cert = await order.Generate(
                    new CsrInfo { CommonName = req.Domain },
                    KeyAlgorithm.RS256);
                
                await SaveCertificateAsync(cert, ct);
            }
            finally {
                _acmePool.Return(acme);
            }
        }
    }
}