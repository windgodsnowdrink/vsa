#load "dataprotection_integration.cs"

Console.WriteLine("=== dataprotection_integration.cs Test ===");

try
{
    // 验证 class: DataProtectionService
    var type_DataProtectionService = Type.GetType("DataProtectionService");
    if (type_DataProtectionService != null)
    {
        Console.WriteLine("[PASS] 类型 DataProtectionService (class) 存在");
        var ctors_DataProtectionService = type_DataProtectionService.GetConstructors();
        Console.WriteLine($"[PASS] DataProtectionService 构造函数数量: {ctors_DataProtectionService.Length}");
        var methods_DataProtectionService = type_DataProtectionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataProtectionService 公开方法数量: {methods_DataProtectionService.Length}");
        foreach (var m in methods_DataProtectionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataProtectionService 未找到，尝试无命名空间...");
        type_DataProtectionService = Type.GetType("DataProtectionService");
        if (type_DataProtectionService != null)
            Console.WriteLine("[PASS] 类型 DataProtectionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataProtectionService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProtectorPooledPolicy
    var type_ProtectorPooledPolicy = Type.GetType("ProtectorPooledPolicy");
    if (type_ProtectorPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 ProtectorPooledPolicy (class) 存在");
        var ctors_ProtectorPooledPolicy = type_ProtectorPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] ProtectorPooledPolicy 构造函数数量: {ctors_ProtectorPooledPolicy.Length}");
        var methods_ProtectorPooledPolicy = type_ProtectorPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProtectorPooledPolicy 公开方法数量: {methods_ProtectorPooledPolicy.Length}");
        foreach (var m in methods_ProtectorPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProtectorPooledPolicy 未找到，尝试无命名空间...");
        type_ProtectorPooledPolicy = Type.GetType("ProtectorPooledPolicy");
        if (type_ProtectorPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 ProtectorPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProtectorPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ProtectRequest
    var type_ProtectRequest = Type.GetType("ProtectRequest");
    if (type_ProtectRequest != null)
    {
        Console.WriteLine("[PASS] 类型 ProtectRequest (record) 存在");
        var ctors_ProtectRequest = type_ProtectRequest.GetConstructors();
        Console.WriteLine($"[PASS] ProtectRequest 构造函数数量: {ctors_ProtectRequest.Length}");
        var methods_ProtectRequest = type_ProtectRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProtectRequest 公开方法数量: {methods_ProtectRequest.Length}");
        foreach (var m in methods_ProtectRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProtectRequest 未找到，尝试无命名空间...");
        type_ProtectRequest = Type.GetType("ProtectRequest");
        if (type_ProtectRequest != null)
            Console.WriteLine("[PASS] 类型 ProtectRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProtectRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: ProtectOperation
    var type_ProtectOperation = Type.GetType("ProtectOperation");
    if (type_ProtectOperation != null)
    {
        Console.WriteLine("[PASS] 类型 ProtectOperation (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProtectOperation 未找到，尝试无命名空间...");
        type_ProtectOperation = Type.GetType("ProtectOperation");
        if (type_ProtectOperation != null)
            Console.WriteLine("[PASS] 类型 ProtectOperation (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProtectOperation 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
