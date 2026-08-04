#:sdk Microsoft.NET.Sdk.Web
#:package BouncyCastle.Cryptography@2.2.1
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Runtime.CompilerServices;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

[SkipLocalsInit]
public sealed class BouncyCastleAdvancedService : IDisposable
{
    private readonly ObjectPool<byte[]> _bufferPool;
    private readonly MemoryCache _keyCache;

    public BouncyCastleAdvancedService()
    {
        _bufferPool = new DefaultObjectPool<byte[]>(
            new ArrayPoolPolicy(), 
            Environment.ProcessorCount * 4);
            
        _keyCache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = 100
        });
    }

    // 1. RSA非对称加密
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public byte[] RsaEncrypt(byte[] input, RsaKeyParameters publicKey)
    {
        var buffer = _bufferPool.Get();
        try
        {
            var cipher = new OaepEncoding(new RsaEngine(), new Sha256Digest());
            cipher.Init(true, publicKey);
            
            var output = new byte[cipher.GetOutputSize(input.Length)];
            var len = cipher.ProcessBytes(input, 0, input.Length, output, 0);
            len += cipher.DoFinal(output, len);
            
            return output.AsSpan(0, len).ToArray();
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    // 2. RSA非对称解密
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public byte[] RsaDecrypt(byte[] input, RsaKeyParameters privateKey)
    {
        var buffer = _bufferPool.Get();
        try
        {
            var cipher = new OaepEncoding(new RsaEngine(), new Sha256Digest());
            cipher.Init(false, privateKey);
            
            var output = new byte[cipher.GetOutputSize(input.Length)];
            var len = cipher.ProcessBytes(input, 0, input.Length, output, 0);
            len += cipher.DoFinal(output, len);
            
            return output.AsSpan(0, len).ToArray();
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    // 3. ECC椭圆曲线签名
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public byte[] EccSign(byte[] input, ECPrivateKeyParameters privateKey)
    {
        var signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        signer.Init(true, privateKey);
        signer.BlockUpdate(input, 0, input.Length);
        return signer.GenerateSignature();
    }

    // 4. ECC椭圆曲线验证
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool EccVerify(byte[] input, byte[] signature, ECPublicKeyParameters publicKey)
    {
        var signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        signer.Init(false, publicKey);
        signer.BlockUpdate(input, 0, input.Length);
        return signer.VerifySignature(signature);
    }

    // 5. 生成RSA密钥对
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public AsymmetricCipherKeyPair GenerateRsaKeyPair(int keySize = 2048)
    {
        var keyGen = new RsaKeyPairGenerator();
        keyGen.Init(new KeyGenerationParameters(
            new SecureRandom(new CryptoApiRandomGenerator()), 
            keySize));
        return keyGen.GenerateKeyPair();
    }

    // 6. 生成ECC密钥对
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public AsymmetricCipherKeyPair GenerateEccKeyPair(string curveName = "secp256r1")
    {
        var ecParams = ECNamedCurveTable.GetByName(curveName);
        var domainParams = new ECDomainParameters(
            ecParams.Curve, ecParams.G, ecParams.N, ecParams.H, ecParams.GetSeed());
            
        var keyGen = new ECKeyPairGenerator();
        keyGen.Init(new ECKeyGenerationParameters(
            domainParams, 
            new SecureRandom(new CryptoApiRandomGenerator())));
            
        return keyGen.GenerateKeyPair();
    }

    // 7. 证书生成与验证
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public (X509Certificate Cert, Pkcs12Store Store) GenerateCertificate(
        string subjectName, 
        AsymmetricCipherKeyPair keyPair,
        int validityYears = 1)
    {
        var random = new SecureRandom();
        var certGen = new X509V3CertificateGenerator();
        
        var serial = BigInteger.ProbablePrime(160, random);
        certGen.SetSerialNumber(serial);
        certGen.SetIssuerDN(new X509Name($"CN={subjectName}"));
        certGen.SetSubjectDN(new X509Name($"CN={subjectName}"));
        certGen.SetNotBefore(DateTime.UtcNow.Date);
        certGen.SetNotAfter(DateTime.UtcNow.AddYears(validityYears).Date);
        certGen.SetPublicKey(keyPair.Public);
        
        var signatureFactory = new Asn1SignatureFactory(
            "SHA256WithRSA", 
            keyPair.Private, 
            random);
            
        var cert = certGen.Generate(signatureFactory);
        
        var store = new Pkcs12Store();
        var entry = new AsymmetricKeyEntry(keyPair.Private);
        store.SetKeyEntry(subjectName, entry, new[] { new X509CertificateEntry(cert) });
        
        return (cert, store);
    }

    // 8. 多种哈希算法支持
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public byte[] ComputeHash(byte[] input, string algorithm = "SHA3-256")
    {
        var digest = DigestUtilities.GetDigest(algorithm);
        var output = new byte[digest.GetDigestSize()];
        digest.BlockUpdate(input, 0, input.Length);
        digest.DoFinal(output, 0);
        return output;
    }

    public void Dispose()
    {
        _keyCache.Dispose();
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<BouncyCastleAdvancedService>();

var app = builder.Build();
app.MapGet("/", () => "BouncyCastle Advanced Service Running");
app.Run();