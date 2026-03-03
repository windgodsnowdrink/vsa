#:sdk Microsoft.NET.Sdk
#:package Microsoft.AspNetCore.Cryptography.KeyDerivation@8.0.0
#:package System.Security.Cryptography@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Polly;
using System.Threading.Channels;
using System.Runtime.CompilerServices;

namespace AesGcmEncryption
{
    /// <summary>
    /// AES-GCM加密配置选项
    /// </summary>
    public class AesGcmOptions
    {
        /// <summary>
        /// 密钥长度(128/192/256位)
        /// </summary>
        public int KeySize { get; set; } = 256;

        /// <summary>
        /// 认证标签长度(128位)
        /// </summary>
        public int TagSize { get; set; } = 16;

        /// <summary>
        /// 随机数长度(96位)
        /// </summary>
        public int NonceSize { get; set; } = 12;

        /// <summary>
        /// 最大输入大小(字节)
        /// </summary>
        public int MaxInputSize { get; set; } = 100 * 1024 * 1024; // 100MB
    }

    /// <summary>
    /// AES-GCM加密器接口
    /// </summary>
    public interface IAesGcmEncryptor : IDisposable
    {
        /// <summary>
        /// 加密数据
        /// </summary>
        byte[] Encrypt(byte[] plaintext);

        /// <summary>
        /// 解密数据
        /// </summary>
        byte[] Decrypt(byte[] ciphertext);

        /// <summary>
        /// 异步加密
        /// </summary>
        ValueTask<byte[]> EncryptAsync(ReadOnlyMemory<byte> plaintext, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步解密
        /// </summary>
        ValueTask<byte[]> DecryptAsync(ReadOnlyMemory<byte> ciphertext, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// AES-GCM加密器实现
    /// </summary>
    public class AesGcmEncryptor : IAesGcmEncryptor
    {
        private readonly IOptionsMonitor<AesGcmOptions> _options;
        private readonly ILogger<AesGcmEncryptor> _logger;
        private readonly Meter _meter;
        private readonly Histogram<long> _encryptionDuration;
        private readonly Histogram<long> _decryptionDuration;
        private readonly Counter<long> _encryptionCount;
        private readonly Counter<long> _decryptionCount;
        private readonly Counter<long> _encryptionErrorCount;
        private readonly Counter<long> _decryptionErrorCount;
        private readonly CircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly MemoryPool<byte> _memoryPool;
        private readonly Channel<EncryptionJob> _encryptionChannel;
        private readonly Channel<DecryptionJob> _decryptionChannel;
        private readonly CancellationTokenSource _cts;
        private readonly Task _backgroundProcessingTask;
        private readonly byte[] _key;

        public AesGcmEncryptor(IOptionsMonitor<AesGcmOptions> options, ILogger<AesGcmEncryptor> logger)
        {
            _options = options;
            _logger = logger;
            _memoryPool = MemoryPool<byte>.Shared;
            _cts = new CancellationTokenSource();
            
            // 初始化性能监控
            _meter = new Meter("AesGcmEncryptor");
            _encryptionDuration = _meter.CreateHistogram<long>("encryption_duration_ms", "ms", "Encryption duration in milliseconds");
            _decryptionDuration = _meter.CreateHistogram<long>("decryption_duration_ms", "ms", "Decryption duration in milliseconds");
            _encryptionCount = _meter.CreateCounter<long>("encryption_count", "count", "Total encryption operations");
            _decryptionCount = _meter.CreateCounter<long>("decryption_count", "count", "Total decryption operations");
            _encryptionErrorCount = _meter.CreateCounter<long>("encryption_error_count", "count", "Total encryption errors");
            _decryptionErrorCount = _meter.CreateCounter<long>("decryption_error_count", "count", "Total decryption errors");
            
            // 初始化熔断策略
            _circuitBreakerPolicy = Policy
                .Handle<CryptographicException>()
                .CircuitBreakerAsync(
                    _options.CurrentValue.CircuitBreakerThreshold,
                    TimeSpan.FromMilliseconds(_options.CurrentValue.CircuitBreakerDurationMs),
                    onBreak: (ex, breakDelay) => _logger.LogWarning(ex, $"Circuit breaker opened for {breakDelay.TotalMilliseconds}ms"),
                    onReset: () => _logger.LogInformation("Circuit breaker reset"),
                    onHalfOpen: () => _logger.LogInformation("Circuit breaker half-open"));
            
            // 初始化处理通道
            _encryptionChannel = Channel.CreateBounded<EncryptionJob>(new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });
            
            _decryptionChannel = Channel.CreateBounded<DecryptionJob>(new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });
            
            // 启动后台处理任务
            _backgroundProcessingTask = Task.WhenAll(
                ProcessEncryptionJobsAsync(_cts.Token),
                ProcessDecryptionJobsAsync(_cts.Token));
            
            // 生成随机密钥(生产环境应从安全存储获取)
            _key = new byte[_options.CurrentValue.KeySize / 8];
            RandomNumberGenerator.Fill(_key);
        }

        public byte[] Encrypt(byte[] plaintext)
        {
            if (plaintext == null) throw new ArgumentNullException(nameof(plaintext));
            if (plaintext.Length > _options.CurrentValue.MaxInputSize)
                throw new ArgumentException("Input size exceeds maximum allowed");

            // 生成随机Nonce
            var nonce = new byte[_options.CurrentValue.NonceSize];
            RandomNumberGenerator.Fill(nonce);

            // 创建认证标签
            var tag = new byte[_options.CurrentValue.TagSize];
            var ciphertext = new byte[plaintext.Length];

            using var aesGcm = new AesGcm(_key);
            aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);

            // 组合Nonce + Ciphertext + Tag
            var result = new byte[nonce.Length + ciphertext.Length + tag.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(ciphertext, 0, result, nonce.Length, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length + ciphertext.Length, tag.Length);

            return result;
        }

        public byte[] Decrypt(byte[] ciphertext)
        {
            if (ciphertext == null) throw new ArgumentNullException(nameof(ciphertext));

            // 解析Nonce + Ciphertext + Tag
            var nonce = new byte[_options.CurrentValue.NonceSize];
            var tag = new byte[_options.CurrentValue.TagSize];
            var actualCiphertext = new byte[ciphertext.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(ciphertext, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(ciphertext, nonce.Length, actualCiphertext, 0, actualCiphertext.Length);
            Buffer.BlockCopy(ciphertext, nonce.Length + actualCiphertext.Length, tag, 0, tag.Length);

            var plaintext = new byte[actualCiphertext.Length];

            using var aesGcm = new AesGcm(_key);
            aesGcm.Decrypt(nonce, actualCiphertext, tag, plaintext);

            return plaintext;
        }

        public async ValueTask<byte[]> EncryptAsync(ReadOnlyMemory<byte> plaintext, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => Encrypt(plaintext.ToArray()), cancellationToken);
        }

        public async ValueTask<byte[]> DecryptAsync(ReadOnlyMemory<byte> ciphertext, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => Decrypt(ciphertext.ToArray()), cancellationToken);
        }

        public void Dispose()
        {
            CryptographicOperations.ZeroMemory(_key);
        }
    }

    /// <summary>
    /// AES-GCM加密扩展方法
    /// </summary>
    public static class AesGcmExtensions
    {
        public static IServiceCollection AddAesGcmEncryption(this IServiceCollection services, Action<AesGcmOptions> configure = null)
        {
            services.AddOptions<AesGcmOptions>()
                .Configure(configure ?? (opt => { }))
                .ValidateDataAnnotations();

            services.AddSingleton<IAesGcmEncryptor, AesGcmEncryptor>();
            return services;
        }
    }
}