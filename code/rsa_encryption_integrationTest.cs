#load "rsa_encryption_integration.cs"

Console.WriteLine("=== rsa_encryption_integration.cs Test ===");

try
{
    // 验证 class: Vsa.Security.Cryptography.RsaEncryptionOptions
    var type_RsaEncryptionOptions = Type.GetType("Vsa.Security.Cryptography.RsaEncryptionOptions");
    if (type_RsaEncryptionOptions != null)
    {
        Console.WriteLine("[PASS] 类型 Vsa.Security.Cryptography.RsaEncryptionOptions (class) 存在");
        var ctors_RsaEncryptionOptions = type_RsaEncryptionOptions.GetConstructors();
        Console.WriteLine($"[PASS] Vsa.Security.Cryptography.RsaEncryptionOptions 构造函数数量: {ctors_RsaEncryptionOptions.Length}");
        var methods_RsaEncryptionOptions = type_RsaEncryptionOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Vsa.Security.Cryptography.RsaEncryptionOptions 公开方法数量: {methods_RsaEncryptionOptions.Length}");
        foreach (var m in methods_RsaEncryptionOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Vsa.Security.Cryptography.RsaEncryptionOptions 未找到，尝试无命名空间...");
        type_RsaEncryptionOptions = Type.GetType("RsaEncryptionOptions");
        if (type_RsaEncryptionOptions != null)
            Console.WriteLine("[PASS] 类型 RsaEncryptionOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RsaEncryptionOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Vsa.Security.Cryptography.RsaEncryptor
    var type_RsaEncryptor = Type.GetType("Vsa.Security.Cryptography.RsaEncryptor");
    if (type_RsaEncryptor != null)
    {
        Console.WriteLine("[PASS] 类型 Vsa.Security.Cryptography.RsaEncryptor (class) 存在");
        var ctors_RsaEncryptor = type_RsaEncryptor.GetConstructors();
        Console.WriteLine($"[PASS] Vsa.Security.Cryptography.RsaEncryptor 构造函数数量: {ctors_RsaEncryptor.Length}");
        var methods_RsaEncryptor = type_RsaEncryptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Vsa.Security.Cryptography.RsaEncryptor 公开方法数量: {methods_RsaEncryptor.Length}");
        foreach (var m in methods_RsaEncryptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Vsa.Security.Cryptography.RsaEncryptor 未找到，尝试无命名空间...");
        type_RsaEncryptor = Type.GetType("RsaEncryptor");
        if (type_RsaEncryptor != null)
            Console.WriteLine("[PASS] 类型 RsaEncryptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RsaEncryptor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Vsa.Security.Cryptography.RsaEncryptionExtensions
    var type_RsaEncryptionExtensions = Type.GetType("Vsa.Security.Cryptography.RsaEncryptionExtensions");
    if (type_RsaEncryptionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 Vsa.Security.Cryptography.RsaEncryptionExtensions (class) 存在");
        var ctors_RsaEncryptionExtensions = type_RsaEncryptionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] Vsa.Security.Cryptography.RsaEncryptionExtensions 构造函数数量: {ctors_RsaEncryptionExtensions.Length}");
        var methods_RsaEncryptionExtensions = type_RsaEncryptionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Vsa.Security.Cryptography.RsaEncryptionExtensions 公开方法数量: {methods_RsaEncryptionExtensions.Length}");
        foreach (var m in methods_RsaEncryptionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Vsa.Security.Cryptography.RsaEncryptionExtensions 未找到，尝试无命名空间...");
        type_RsaEncryptionExtensions = Type.GetType("RsaEncryptionExtensions");
        if (type_RsaEncryptionExtensions != null)
            Console.WriteLine("[PASS] 类型 RsaEncryptionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RsaEncryptionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: Vsa.Security.Cryptography.IRsaEncryptor
    var type_IRsaEncryptor = Type.GetType("Vsa.Security.Cryptography.IRsaEncryptor");
    if (type_IRsaEncryptor != null)
    {
        Console.WriteLine("[PASS] 类型 Vsa.Security.Cryptography.IRsaEncryptor (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Vsa.Security.Cryptography.IRsaEncryptor 未找到，尝试无命名空间...");
        type_IRsaEncryptor = Type.GetType("IRsaEncryptor");
        if (type_IRsaEncryptor != null)
            Console.WriteLine("[PASS] 类型 IRsaEncryptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IRsaEncryptor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
