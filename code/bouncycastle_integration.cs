#:sdk Microsoft.NET.Sdk.Web
#:package BouncyCastle.Cryptography@2.2.1
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Runtime.CompilerServices;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

[SkipLocalsInit]
public sealed class BouncyCastleCryptoService : IDisposable
{
    private readonly IBlockCipher _cipher;
    private readonly PaddedBufferedBlockCipher _bufferedCipher;
    private readonly ObjectPool<byte[]> _bufferPool;
    private readonly MemoryCache _keyCache;

    public BouncyCastleCryptoService()
    {
        // 使用AES-256-GCM模式
        _cipher = new GcmBlockCipher(new AesEngine());
        _bufferedCipher = new PaddedBufferedBlockCipher(_cipher, new Pkcs7Padding());
        
        _bufferPool = new DefaultObjectPool<byte[]>(
            new ArrayPoolPolicy(), 
            Environment.ProcessorCount * 4);
            
        _keyCache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = 100 // 缓存100个密钥
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public byte[] Encrypt(byte[] input, byte[] key, byte[] iv)
    {
        var buffer = _bufferPool.Get();
        try
        {
            var keyParam = new KeyParameter(key);
            var parameters = new ParametersWithIV(keyParam, iv);
            
            _bufferedCipher.Init(true, parameters);
            var output = new byte[_bufferedCipher.GetOutputSize(input.Length)];
            
            var len = _bufferedCipher.ProcessBytes(input, 0, input.Length, output, 0);
            len += _bufferedCipher.DoFinal(output, len);
            
            return output.AsSpan(0, len).ToArray();
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public byte[] Decrypt(byte[] input, byte[] key, byte[] iv)
    {
        var buffer = _bufferPool.Get();
        try
        {
            var keyParam = new KeyParameter(key);
            var parameters = new ParametersWithIV(keyParam, iv);
            
            _bufferedCipher.Init(false, parameters);
            var output = new byte[_bufferedCipher.GetOutputSize(input.Length)];
            
            var len = _bufferedCipher.ProcessBytes(input, 0, input.Length, output, 0);
            len += _bufferedCipher.DoFinal(output, len);
            
            return output.AsSpan(0, len).ToArray();
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public (byte[] Key, byte[] IV) GenerateKeyAndIV(int keySize = 256, int ivSize = 128)
    {
        var cacheKey = $"{keySize}-{ivSize}";
        if (_keyCache.TryGetValue<(byte[], byte[])>(cacheKey, out var cached))
            return cached;
            
        var key = new byte[keySize / 8];
        var iv = new byte[ivSize / 8];
        
        var random = new SecureRandom();
        random.NextBytes(key);
        random.NextBytes(iv);
        
        _keyCache.Set(cacheKey, (key, iv), TimeSpan.FromMinutes(30));
        return (key, iv);
    }

    public void Dispose()
    {
        _bufferedCipher.Reset();
        _keyCache.Dispose();
    }
}

// 内存池策略
[SkipLocalsInit]
public class ArrayPoolPolicy : IPooledObjectPolicy<byte[]>
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public byte[] Create()
    {
        return GC.AllocateUninitializedArray<byte>(4096, pinned: true);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool Return(byte[] obj)
    {
        Array.Clear(obj, 0, obj.Length);
        return true;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<BouncyCastleCryptoService>();

var app = builder.Build();
app.MapGet("/", () => "BouncyCastle Crypto Service Running");
app.Run();