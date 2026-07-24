#:sdk Microsoft.NET.Sdk
#:package System.Security.Cryptography@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0

using System.Security.Cryptography;

[SkipLocalsInit]
public class AuditEncryptor
{
    private readonly ThreadLocal<AesGcm> _aesGcm;
    
    public byte[] Encrypt(AuditEvent auditEvent)
    {
        Span<byte> nonce = stackalloc byte[AesGcm.NonceByteSizes.MaxSize];
        Span<byte> tag = stackalloc byte[AesGcm.TagByteSizes.MaxSize];
        
        var plaintext = MemoryPackSerializer.Serialize(auditEvent);
        var ciphertext = new byte[plaintext.Length];
        
        _aesGcm.Value.Encrypt(nonce, plaintext.Span, ciphertext, tag);
        return ciphertext;
    }
}