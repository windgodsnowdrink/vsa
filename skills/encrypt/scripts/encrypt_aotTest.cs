#load "encrypt_aot.cs"

Console.WriteLine("=== encrypt_aot Test ===");

try
{
    var t0 = typeof(Encrypt.AOT.EncryptOptions);
    Console.WriteLine($"[PASS] EncryptOptions 存在");
    var t1 = typeof(Encrypt.AOT.EncryptCommandResult);
    Console.WriteLine($"[PASS] EncryptCommandResult 存在");
    var t2 = typeof(Encrypt.AOT.EncryptService);
    Console.WriteLine($"[PASS] EncryptService 存在");
    var t3 = typeof(Encrypt.AOT.EncryptAotEngine);
    Console.WriteLine($"[PASS] EncryptAotEngine 存在");
    var t4 = typeof(Encrypt.AOT.EncryptAotExtensions);
    Console.WriteLine($"[PASS] EncryptAotExtensions 存在");
    var t5 = typeof(Encrypt.AOT.IEncryptService);
    Console.WriteLine($"[PASS] IEncryptService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(Encrypt.AOT.EncryptCommandType);
    Console.WriteLine($"[PASS] EncryptCommandType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}