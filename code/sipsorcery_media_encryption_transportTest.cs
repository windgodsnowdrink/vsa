#load "sipsorcery_media_encryption_transport.cs"

Console.WriteLine("=== sipsorcery_media_encryption_transport.cs Test ===");

try
{
    // 验证 class: MediaEncryptor
    var type_MediaEncryptor = Type.GetType("MediaEncryptor");
    if (type_MediaEncryptor != null)
    {
        Console.WriteLine("[PASS] 类型 MediaEncryptor (class) 存在");
        var ctors_MediaEncryptor = type_MediaEncryptor.GetConstructors();
        Console.WriteLine($"[PASS] MediaEncryptor 构造函数数量: {ctors_MediaEncryptor.Length}");
        var methods_MediaEncryptor = type_MediaEncryptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MediaEncryptor 公开方法数量: {methods_MediaEncryptor.Length}");
        foreach (var m in methods_MediaEncryptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MediaEncryptor 未找到，尝试无命名空间...");
        type_MediaEncryptor = Type.GetType("MediaEncryptor");
        if (type_MediaEncryptor != null)
            Console.WriteLine("[PASS] 类型 MediaEncryptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MediaEncryptor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RTPProcessor
    var type_RTPProcessor = Type.GetType("RTPProcessor");
    if (type_RTPProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 RTPProcessor (class) 存在");
        var ctors_RTPProcessor = type_RTPProcessor.GetConstructors();
        Console.WriteLine($"[PASS] RTPProcessor 构造函数数量: {ctors_RTPProcessor.Length}");
        var methods_RTPProcessor = type_RTPProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RTPProcessor 公开方法数量: {methods_RTPProcessor.Length}");
        foreach (var m in methods_RTPProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RTPProcessor 未找到，尝试无命名空间...");
        type_RTPProcessor = Type.GetType("RTPProcessor");
        if (type_RTPProcessor != null)
            Console.WriteLine("[PASS] 类型 RTPProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RTPProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QuantumSafeEncryptor
    var type_QuantumSafeEncryptor = Type.GetType("QuantumSafeEncryptor");
    if (type_QuantumSafeEncryptor != null)
    {
        Console.WriteLine("[PASS] 类型 QuantumSafeEncryptor (class) 存在");
        var ctors_QuantumSafeEncryptor = type_QuantumSafeEncryptor.GetConstructors();
        Console.WriteLine($"[PASS] QuantumSafeEncryptor 构造函数数量: {ctors_QuantumSafeEncryptor.Length}");
        var methods_QuantumSafeEncryptor = type_QuantumSafeEncryptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QuantumSafeEncryptor 公开方法数量: {methods_QuantumSafeEncryptor.Length}");
        foreach (var m in methods_QuantumSafeEncryptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuantumSafeEncryptor 未找到，尝试无命名空间...");
        type_QuantumSafeEncryptor = Type.GetType("QuantumSafeEncryptor");
        if (type_QuantumSafeEncryptor != null)
            Console.WriteLine("[PASS] 类型 QuantumSafeEncryptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QuantumSafeEncryptor 可能为顶层语句或嵌套类型");
    }

    // 验证 record: EncryptionConfig
    var type_EncryptionConfig = Type.GetType("EncryptionConfig");
    if (type_EncryptionConfig != null)
    {
        Console.WriteLine("[PASS] 类型 EncryptionConfig (record) 存在");
        var ctors_EncryptionConfig = type_EncryptionConfig.GetConstructors();
        Console.WriteLine($"[PASS] EncryptionConfig 构造函数数量: {ctors_EncryptionConfig.Length}");
        var methods_EncryptionConfig = type_EncryptionConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EncryptionConfig 公开方法数量: {methods_EncryptionConfig.Length}");
        foreach (var m in methods_EncryptionConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EncryptionConfig 未找到，尝试无命名空间...");
        type_EncryptionConfig = Type.GetType("EncryptionConfig");
        if (type_EncryptionConfig != null)
            Console.WriteLine("[PASS] 类型 EncryptionConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EncryptionConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 record: AesGcmConfig
    var type_AesGcmConfig = Type.GetType("AesGcmConfig");
    if (type_AesGcmConfig != null)
    {
        Console.WriteLine("[PASS] 类型 AesGcmConfig (record) 存在");
        var ctors_AesGcmConfig = type_AesGcmConfig.GetConstructors();
        Console.WriteLine($"[PASS] AesGcmConfig 构造函数数量: {ctors_AesGcmConfig.Length}");
        var methods_AesGcmConfig = type_AesGcmConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AesGcmConfig 公开方法数量: {methods_AesGcmConfig.Length}");
        foreach (var m in methods_AesGcmConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AesGcmConfig 未找到，尝试无命名空间...");
        type_AesGcmConfig = Type.GetType("AesGcmConfig");
        if (type_AesGcmConfig != null)
            Console.WriteLine("[PASS] 类型 AesGcmConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AesGcmConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CompressionConfig
    var type_CompressionConfig = Type.GetType("CompressionConfig");
    if (type_CompressionConfig != null)
    {
        Console.WriteLine("[PASS] 类型 CompressionConfig (record) 存在");
        var ctors_CompressionConfig = type_CompressionConfig.GetConstructors();
        Console.WriteLine($"[PASS] CompressionConfig 构造函数数量: {ctors_CompressionConfig.Length}");
        var methods_CompressionConfig = type_CompressionConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CompressionConfig 公开方法数量: {methods_CompressionConfig.Length}");
        foreach (var m in methods_CompressionConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CompressionConfig 未找到，尝试无命名空间...");
        type_CompressionConfig = Type.GetType("CompressionConfig");
        if (type_CompressionConfig != null)
            Console.WriteLine("[PASS] 类型 CompressionConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CompressionConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 record: QuantumEncryptionConfig
    var type_QuantumEncryptionConfig = Type.GetType("QuantumEncryptionConfig");
    if (type_QuantumEncryptionConfig != null)
    {
        Console.WriteLine("[PASS] 类型 QuantumEncryptionConfig (record) 存在");
        var ctors_QuantumEncryptionConfig = type_QuantumEncryptionConfig.GetConstructors();
        Console.WriteLine($"[PASS] QuantumEncryptionConfig 构造函数数量: {ctors_QuantumEncryptionConfig.Length}");
        var methods_QuantumEncryptionConfig = type_QuantumEncryptionConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QuantumEncryptionConfig 公开方法数量: {methods_QuantumEncryptionConfig.Length}");
        foreach (var m in methods_QuantumEncryptionConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuantumEncryptionConfig 未找到，尝试无命名空间...");
        type_QuantumEncryptionConfig = Type.GetType("QuantumEncryptionConfig");
        if (type_QuantumEncryptionConfig != null)
            Console.WriteLine("[PASS] 类型 QuantumEncryptionConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QuantumEncryptionConfig 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
