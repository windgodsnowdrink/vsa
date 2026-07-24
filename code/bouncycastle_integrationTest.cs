#load "bouncycastle_integration.cs"

using System;
using System.Text;

Console.WriteLine("=== bouncycastle_integration Test ===");
int passed = 0, failed = 0;

void Report(string name, bool ok)
{
    if (ok) { passed++; Console.WriteLine($"  PASS: {name}"); }
    else { failed++; Console.WriteLine($"  FAIL: {name}"); }
}

try
{
    // Test 1: BouncyCastleCryptoService类型存在
    var cryptoType = typeof(BouncyCastleCryptoService);
    Report("BouncyCastleCryptoService class exists", cryptoType.IsClass);

    // Test 2: BouncyCastleCryptoService实现IDisposable
    Report("BouncyCastleCryptoService implements IDisposable", typeof(IDisposable).IsAssignableFrom(cryptoType));

    // Test 3: BouncyCastleCryptoService可实例化
    using var service = new BouncyCastleCryptoService();
    Report("BouncyCastleCryptoService instantiated", service != null);

    // Test 4: GenerateKeyAndIV方法存在
    var genMethod = cryptoType.GetMethod("GenerateKeyAndIV");
    Report("GenerateKeyAndIV method exists", genMethod != null);

    // Test 5: Encrypt方法存在
    var encMethod = cryptoType.GetMethod("Encrypt");
    Report("Encrypt method exists", encMethod != null);

    // Test 6: Decrypt方法存在
    var decMethod = cryptoType.GetMethod("Decrypt");
    Report("Decrypt method exists", decMethod != null);

    // Test 7: 生成密钥和IV
    var (key, iv) = service.GenerateKeyAndIV();
    Report("GenerateKeyAndIV returns key", key != null && key.Length == 32); // 256 bits = 32 bytes
    Report("GenerateKeyAndIV returns IV", iv != null && iv.Length == 16);   // 128 bits = 16 bytes

    // Test 8: 加密/解密往返测试
    var plainText = "Hello, BouncyCastle! This is a secret message.";
    var inputBytes = Encoding.UTF8.GetBytes(plainText);
    var encrypted = service.Encrypt(inputBytes, key, iv);
    Report("Encrypt produces output", encrypted != null && encrypted.Length > 0);
    Report("Encrypted differs from plaintext", !inputBytes.AsSpan().SequenceEqual(encrypted.AsSpan(0, inputBytes.Length)));

    var decrypted = service.Decrypt(encrypted, key, iv);
    Report("Decrypt produces output", decrypted != null && decrypted.Length > 0);
    var decryptedText = Encoding.UTF8.GetString(decrypted);
    Report("Round-trip: decrypted matches plaintext", decryptedText == plainText);

    // Test 9: ArrayPoolPolicy类型存在
    var poolPolicyType = typeof(ArrayPoolPolicy);
    Report("ArrayPoolPolicy class exists", poolPolicyType.IsClass);

    // Test 10: ArrayPoolPolicy可实例化
    var poolPolicy = new ArrayPoolPolicy();
    Report("ArrayPoolPolicy instantiated", poolPolicy != null);

    // Test 11: ArrayPoolPolicy.Create返回非空数组
    var array = poolPolicy.Create();
    Report("ArrayPoolPolicy.Create returns array", array != null && array.Length == 4096);

    // Test 12: ArrayPoolPolicy.Return返回true
    var returned = poolPolicy.Return(array);
    Report("ArrayPoolPolicy.Return returns true", returned);

    Console.WriteLine($"\nResults: {passed} passed, {failed} failed");
}
catch (Exception ex)
{
    Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine("=== bouncycastle_integration Test Complete ===");