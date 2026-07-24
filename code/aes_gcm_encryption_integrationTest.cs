#load "aes_gcm_encryption_integration.cs"

Console.WriteLine("=== aes_gcm_encryption_integration.cs Test ===");

try
{
    // 验证 class: AesGcmEncryption.AesGcmOptions
    var type_AesGcmOptions = Type.GetType("AesGcmEncryption.AesGcmOptions");
    if (type_AesGcmOptions != null)
    {
        Console.WriteLine("[PASS] 类型 AesGcmEncryption.AesGcmOptions (class) 存在");
        var ctors_AesGcmOptions = type_AesGcmOptions.GetConstructors();
        Console.WriteLine($"[PASS] AesGcmEncryption.AesGcmOptions 构造函数数量: {ctors_AesGcmOptions.Length}");
        var methods_AesGcmOptions = type_AesGcmOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AesGcmEncryption.AesGcmOptions 公开方法数量: {methods_AesGcmOptions.Length}");
        foreach (var m in methods_AesGcmOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AesGcmEncryption.AesGcmOptions 未找到，尝试无命名空间...");
        type_AesGcmOptions = Type.GetType("AesGcmOptions");
        if (type_AesGcmOptions != null)
            Console.WriteLine("[PASS] 类型 AesGcmOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AesGcmOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AesGcmEncryption.AesGcmEncryptor
    var type_AesGcmEncryptor = Type.GetType("AesGcmEncryption.AesGcmEncryptor");
    if (type_AesGcmEncryptor != null)
    {
        Console.WriteLine("[PASS] 类型 AesGcmEncryption.AesGcmEncryptor (class) 存在");
        var ctors_AesGcmEncryptor = type_AesGcmEncryptor.GetConstructors();
        Console.WriteLine($"[PASS] AesGcmEncryption.AesGcmEncryptor 构造函数数量: {ctors_AesGcmEncryptor.Length}");
        var methods_AesGcmEncryptor = type_AesGcmEncryptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AesGcmEncryption.AesGcmEncryptor 公开方法数量: {methods_AesGcmEncryptor.Length}");
        foreach (var m in methods_AesGcmEncryptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AesGcmEncryption.AesGcmEncryptor 未找到，尝试无命名空间...");
        type_AesGcmEncryptor = Type.GetType("AesGcmEncryptor");
        if (type_AesGcmEncryptor != null)
            Console.WriteLine("[PASS] 类型 AesGcmEncryptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AesGcmEncryptor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AesGcmEncryption.AesGcmExtensions
    var type_AesGcmExtensions = Type.GetType("AesGcmEncryption.AesGcmExtensions");
    if (type_AesGcmExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 AesGcmEncryption.AesGcmExtensions (class) 存在");
        var ctors_AesGcmExtensions = type_AesGcmExtensions.GetConstructors();
        Console.WriteLine($"[PASS] AesGcmEncryption.AesGcmExtensions 构造函数数量: {ctors_AesGcmExtensions.Length}");
        var methods_AesGcmExtensions = type_AesGcmExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AesGcmEncryption.AesGcmExtensions 公开方法数量: {methods_AesGcmExtensions.Length}");
        foreach (var m in methods_AesGcmExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AesGcmEncryption.AesGcmExtensions 未找到，尝试无命名空间...");
        type_AesGcmExtensions = Type.GetType("AesGcmExtensions");
        if (type_AesGcmExtensions != null)
            Console.WriteLine("[PASS] 类型 AesGcmExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AesGcmExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: AesGcmEncryption.IAesGcmEncryptor
    var type_IAesGcmEncryptor = Type.GetType("AesGcmEncryption.IAesGcmEncryptor");
    if (type_IAesGcmEncryptor != null)
    {
        Console.WriteLine("[PASS] 类型 AesGcmEncryption.IAesGcmEncryptor (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AesGcmEncryption.IAesGcmEncryptor 未找到，尝试无命名空间...");
        type_IAesGcmEncryptor = Type.GetType("IAesGcmEncryptor");
        if (type_IAesGcmEncryptor != null)
            Console.WriteLine("[PASS] 类型 IAesGcmEncryptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAesGcmEncryptor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
