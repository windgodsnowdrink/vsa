#:sdk Microsoft.NET.Sdk.Web
#:package Yarp.ReverseProxy@2.0.0
#:package Microsoft.AspNetCore.Cryptography.KeyDerivation@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

[SkipLocalsInit]
public sealed class SignatureMiddleware
{
    private readonly SignatureOptions _options;
    private readonly ThreadLocal<byte[]> _signatureBuffer;

    public SignatureMiddleware(IOptions<SignatureOptions> options)
    {
        _options = options.Value;
        _signatureBuffer = new ThreadLocal<byte[]>(() => new byte[32]);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue("X-Signature", out var signature))
        {
            context.Response.StatusCode = 401;
            return;
        }
        
        using var buffer = MemoryPool<byte>.Shared.Rent(4096);
        var memory = buffer.Memory;
        
        // 实现签名验证逻辑
        if (!ValidateSignature(context.Request, signature!))
        {
            context.Response.StatusCode = 403;
            return;
        }
        
        await next(context);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private bool ValidateSignature(HttpRequest request, string signature)
    {
        // 实现基于HMAC-SHA256的签名验证
        // ... 实际签名验证逻辑
        return true;
    }
}

public class SignatureOptions
{
    public string SecretKey { get; set; } = "default-secret-key";
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(5);
}