#:sdk Microsoft.NET.Sdk.Web
#:package System.Security.Cryptography
#:package Microsoft.Extensions.DependencyInjection
#:package Microsoft.Extensions.Options
#:package Microsoft.Extensions.Logging
#:package System.Buffers
#:package System.IO.Pipelines
#:package System.Threading.Channels
#:package Polly
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Buffers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Vsa.Security.Cryptography
{
    /// <summary>
    /// RSA加密选项配置类
    /// </summary>
    public class RsaEncryptionOptions
    {
        /// <summary>
        /// 获取或设置RSA密钥大小(2048/3072/4096位)
        /// </summary>
        public int KeySize { get; set; } = 4096;

        /// <summary>
        /// 获取或设置填充模式
        /// </summary>
        public RSAEncryptionPadding EncryptionPadding { get; set; } = RSAEncryptionPadding.OaepSHA256;

        /// <summary>
        /// 获取或设置签名填充模式
        /// </summary>
        public RSASignaturePadding SignaturePadding { get; set; } = RSASignaturePadding.Pss;

        /// <summary>
        /// 获取或设置哈希算法
        /// </summary>
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        /// <summary>
        /// 获取或设置最大允许的输入数据大小(字节)
        /// </summary>
        public int MaxInputSize { get; set; } = 245; // RSA 4096 with OAEP can encrypt max 446 bytes

        /// <summary>
        /// 获取或设置加密/解密操作的超时时间(毫秒)
        /// </summary>
        public int TimeoutMs { get; set; } = 5000;

        /// <summary>
        /// 获取或设置熔断器在多少次连续失败后打开
        /// </summary>
        public int CircuitBreakerThreshold { get; set; } = 5;

        /// <summary>
        /// 获取或设置熔断器打开的持续时间(毫秒)
        /// </summary>
        public int CircuitBreakerDurationMs { get; set; } = 30000;
    }

    /// <summary>
    /// 定义RSA加密和解密操作的接口
    /// </summary>
    public interface IRsaEncryptor : IDisposable
    {
        /// <summary>
        /// 使用公钥加密数据
        /// </summary>
        ValueTask<byte[]> EncryptAsync(byte[] plaintext, CancellationToken cancellationToken = default);

        /// <summary>
        /// 使用私钥解密数据
        /// </summary>
        ValueTask<byte[]> DecryptAsync(byte[] ciphertext, CancellationToken cancellationToken = default);

        /// <summary>
        /// 使用私钥创建签名
        /// </summary>
        ValueTask<byte[]> SignDataAsync(byte[] data, CancellationToken cancellationToken = default);

        /// <summary>
        /// 使用公钥验证签名
        /// </summary>
        ValueTask<bool> VerifyDataAsync(byte[] data, byte[] signature, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取公钥
        /// </summary>
        string GetPublicKey();
    }

    /// <summary>
    /// RSA加密器实现
    /// </summary>
    public class RsaEncryptor : IRsaEncryptor
    {
        private readonly ILogger<RsaEncryptor> _logger;
        private readonly IOptionsMonitor<RsaEncryptionOptions> _optionsMonitor;
        private readonly RSA _rsa;
        private readonly Meter _meter;
        private readonly Histogram<long> _encryptionDuration;
        private readonly Histogram<long> _decryptionDuration;
        private readonly Counter<long> _encryptionCount;
        private readonly Counter<long> _decryptionCount;
        private readonly Counter<long> _signatureCount;
        private readonly Counter<long> _verificationCount;
        private readonly Counter<long> _errorCount;
        private readonly CircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly MemoryPool<byte> _memoryPool;

        public RsaEncryptor(IOptionsMonitor<RsaEncryptionOptions> optionsMonitor, ILogger<RsaEncryptor> logger)
        {
            _optionsMonitor = optionsMonitor;
            _logger = logger;
            _memoryPool = MemoryPool<byte>.Shared;

            // 初始化RSA实例
            _rsa = RSA.Create(_optionsMonitor.CurrentValue.KeySize);

            // 初始化性能监控
            _meter = new Meter("RsaEncryptor");
            _encryptionDuration = _meter.CreateHistogram<long>("encryption_duration_ms", "ms", "Encryption duration in milliseconds");
            _decryptionDuration = _meter.CreateHistogram<long>("decryption_duration_ms", "ms", "Decryption duration in milliseconds");
            _encryptionCount = _meter.CreateCounter<long>("encryption_count", "count", "Total encryption operations");
            _decryptionCount = _meter.CreateCounter<long>("decryption_count", "count", "Total decryption operations");
            _signatureCount = _meter.CreateCounter<long>("signature_count", "count", "Total signature operations");
            _verificationCount = _meter.CreateCounter<long>("verification_count", "count", "Total verification operations");
            _errorCount = _meter.CreateCounter<long>("error_count", "count", "Total error operations");

            // 初始化熔断策略
            _circuitBreakerPolicy = Policy
                .Handle<CryptographicException>()
                .CircuitBreakerAsync(
                    _optionsMonitor.CurrentValue.CircuitBreakerThreshold,
                    TimeSpan.FromMilliseconds(_optionsMonitor.CurrentValue.CircuitBreakerDurationMs),
                    onBreak: (ex, breakDelay) => _logger.LogWarning(ex, $"Circuit breaker opened for {breakDelay.TotalMilliseconds}ms"),
                    onReset: () => _logger.LogInformation("Circuit breaker reset"),
                    onHalfOpen: () => _logger.LogInformation("Circuit breaker half-open"));
        }

        public async Task<byte[]> EncryptAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            using var activity = new ActivitySource("RsaEncryption").StartActivity("Encrypt");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // 原有加密逻辑
                return result;
            }
            finally
            {
                _encryptionDuration.Record(stopwatch.Elapsed.TotalSeconds);
            }
            {
                if (plaintext == null) throw new ArgumentNullException(nameof(plaintext));
                if (plaintext.Length > _optionsMonitor.CurrentValue.MaxInputSize)
                    throw new ArgumentException($"Input size exceeds maximum allowed {_optionsMonitor.CurrentValue.MaxInputSize} bytes");

                using var activity = new ActivitySource("RsaEncryptor").StartActivity("Encrypt");
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        using var memoryOwner = _memoryPool.Rent(plaintext.Length);
                        plaintext.CopyTo(memoryOwner.Memory);

                        var result = _rsa.Encrypt(memoryOwner.Memory.Span, _optionsMonitor.CurrentValue.EncryptionPadding);
                        _encryptionCount.Add(1);
                        return result;
                    });
                }
                catch (Exception ex)
                {
                    _errorCount.Add(1);
                    _logger.LogError(ex, "Encryption failed");
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    _encryptionDuration.Record(stopwatch.ElapsedMilliseconds);
                }
            }

            public async ValueTask<byte[]> DecryptAsync(byte[] ciphertext, CancellationToken cancellationToken = default)
            {
                if (ciphertext == null) throw new ArgumentNullException(nameof(ciphertext));

                using var activity = new ActivitySource("RsaEncryptor").StartActivity("Decrypt");
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        using var memoryOwner = _memoryPool.Rent(ciphertext.Length);
                        ciphertext.CopyTo(memoryOwner.Memory);

                        var result = _rsa.Decrypt(memoryOwner.Memory.Span, _optionsMonitor.CurrentValue.EncryptionPadding);
                        _decryptionCount.Add(1);
                        return result;
                    });
                }
                catch (Exception ex)
                {
                    _errorCount.Add(1);
                    _logger.LogError(ex, "Decryption failed");
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    _decryptionDuration.Record(stopwatch.ElapsedMilliseconds);
                }
            }

            public async ValueTask<byte[]> SignDataAsync(byte[] data, CancellationToken cancellationToken = default)
            {
                if (data == null) throw new ArgumentNullException(nameof(data));

                using var activity = new ActivitySource("RsaEncryptor").StartActivity("SignData");
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        using var memoryOwner = _memoryPool.Rent(data.Length);
                        data.CopyTo(memoryOwner.Memory);

                        var result = _rsa.SignData(memoryOwner.Memory.Span, _optionsMonitor.CurrentValue.HashAlgorithm, _optionsMonitor.CurrentValue.SignaturePadding);
                        _signatureCount.Add(1);
                        return result;
                    });
                }
                catch (Exception ex)
                {
                    _errorCount.Add(1);
                    _logger.LogError(ex, "SignData failed");
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    _decryptionDuration.Record(stopwatch.ElapsedMilliseconds);
                }
            }

            public async ValueTask<bool> VerifyDataAsync(byte[] data, byte[] signature, CancellationToken cancellationToken = default)
            {
                if (data == null) throw new ArgumentNullException(nameof(data));
                if (signature == null) throw new ArgumentNullException(nameof(signature));

                using var activity = new ActivitySource("RsaEncryptor").StartActivity("VerifyData");
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        using var dataMemory = _memoryPool.Rent(data.Length);
                        using var signatureMemory = _memoryPool.Rent(signature.Length);
                        data.CopyTo(dataMemory.Memory);
                        signature.CopyTo(signatureMemory.Memory);

                        var result = _rsa.VerifyData(dataMemory.Memory.Span, signatureMemory.Memory.Span, _optionsMonitor.CurrentValue.HashAlgorithm, _optionsMonitor.CurrentValue.SignaturePadding);
                        _verificationCount.Add(1);
                        return result;
                    });
                }
                catch (Exception ex)
                {
                    _errorCount.Add(1);
                    _logger.LogError(ex, "VerifyData failed");
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    _decryptionDuration.Record(stopwatch.ElapsedMilliseconds);
                }
            }

            public string GetPublicKey()
            {
                return _rsa.ExportSubjectPublicKeyInfoPem();
            }

            public void Dispose()
            {
                _rsa?.Dispose();
                _meter?.Dispose();
            }
        }

        /// <summary>
        /// RSA加密扩展方法
        /// </summary>
        public static class RsaEncryptionExtensions
        {
            public static IServiceCollection AddRsaEncryption(this IServiceCollection services, Action<RsaEncryptionOptions> configure = null)
            {
                services.AddOptions<RsaEncryptionOptions>()
                    .Configure(configure ?? (opt => { }))
                    .ValidateDataAnnotations();

                services.AddSingleton<IRsaEncryptor, RsaEncryptor>();
                return services;
            }
        }
    }
}