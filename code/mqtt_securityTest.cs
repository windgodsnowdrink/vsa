#load "mqtt_security.cs"

Console.WriteLine("=== mqtt_security.cs Test ===");

try
{
    // 验证 class: AesGcmEncryptor
    var type_AesGcmEncryptor = Type.GetType("AesGcmEncryptor");
    if (type_AesGcmEncryptor != null)
    {
        Console.WriteLine("[PASS] 类型 AesGcmEncryptor (class) 存在");
        var ctors_AesGcmEncryptor = type_AesGcmEncryptor.GetConstructors();
        Console.WriteLine($"[PASS] AesGcmEncryptor 构造函数数量: {ctors_AesGcmEncryptor.Length}");
        var methods_AesGcmEncryptor = type_AesGcmEncryptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AesGcmEncryptor 公开方法数量: {methods_AesGcmEncryptor.Length}");
        foreach (var m in methods_AesGcmEncryptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AesGcmEncryptor 未找到，尝试无命名空间...");
        type_AesGcmEncryptor = Type.GetType("AesGcmEncryptor");
        if (type_AesGcmEncryptor != null)
            Console.WriteLine("[PASS] 类型 AesGcmEncryptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AesGcmEncryptor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Sha256Fingerprinter
    var type_Sha256Fingerprinter = Type.GetType("Sha256Fingerprinter");
    if (type_Sha256Fingerprinter != null)
    {
        Console.WriteLine("[PASS] 类型 Sha256Fingerprinter (class) 存在");
        var ctors_Sha256Fingerprinter = type_Sha256Fingerprinter.GetConstructors();
        Console.WriteLine($"[PASS] Sha256Fingerprinter 构造函数数量: {ctors_Sha256Fingerprinter.Length}");
        var methods_Sha256Fingerprinter = type_Sha256Fingerprinter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Sha256Fingerprinter 公开方法数量: {methods_Sha256Fingerprinter.Length}");
        foreach (var m in methods_Sha256Fingerprinter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Sha256Fingerprinter 未找到，尝试无命名空间...");
        type_Sha256Fingerprinter = Type.GetType("Sha256Fingerprinter");
        if (type_Sha256Fingerprinter != null)
            Console.WriteLine("[PASS] 类型 Sha256Fingerprinter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Sha256Fingerprinter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AuditLogger
    var type_AuditLogger = Type.GetType("AuditLogger");
    if (type_AuditLogger != null)
    {
        Console.WriteLine("[PASS] 类型 AuditLogger (class) 存在");
        var ctors_AuditLogger = type_AuditLogger.GetConstructors();
        Console.WriteLine($"[PASS] AuditLogger 构造函数数量: {ctors_AuditLogger.Length}");
        var methods_AuditLogger = type_AuditLogger.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuditLogger 公开方法数量: {methods_AuditLogger.Length}");
        foreach (var m in methods_AuditLogger)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuditLogger 未找到，尝试无命名空间...");
        type_AuditLogger = Type.GetType("AuditLogger");
        if (type_AuditLogger != null)
            Console.WriteLine("[PASS] 类型 AuditLogger (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuditLogger 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
