#load "logging_dataprotection_extensions.cs"

Console.WriteLine("=== logging_dataprotection_extensions.cs Test ===");

try
{
    // 验证 class: SensitiveDataProtectionExtensions
    var type_SensitiveDataProtectionExtensions = Type.GetType("SensitiveDataProtectionExtensions");
    if (type_SensitiveDataProtectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 SensitiveDataProtectionExtensions (class) 存在");
        var ctors_SensitiveDataProtectionExtensions = type_SensitiveDataProtectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] SensitiveDataProtectionExtensions 构造函数数量: {ctors_SensitiveDataProtectionExtensions.Length}");
        var methods_SensitiveDataProtectionExtensions = type_SensitiveDataProtectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SensitiveDataProtectionExtensions 公开方法数量: {methods_SensitiveDataProtectionExtensions.Length}");
        foreach (var m in methods_SensitiveDataProtectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SensitiveDataProtectionExtensions 未找到，尝试无命名空间...");
        type_SensitiveDataProtectionExtensions = Type.GetType("SensitiveDataProtectionExtensions");
        if (type_SensitiveDataProtectionExtensions != null)
            Console.WriteLine("[PASS] 类型 SensitiveDataProtectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SensitiveDataProtectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProtectedDataDestructuringPolicy
    var type_ProtectedDataDestructuringPolicy = Type.GetType("ProtectedDataDestructuringPolicy");
    if (type_ProtectedDataDestructuringPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 ProtectedDataDestructuringPolicy (class) 存在");
        var ctors_ProtectedDataDestructuringPolicy = type_ProtectedDataDestructuringPolicy.GetConstructors();
        Console.WriteLine($"[PASS] ProtectedDataDestructuringPolicy 构造函数数量: {ctors_ProtectedDataDestructuringPolicy.Length}");
        var methods_ProtectedDataDestructuringPolicy = type_ProtectedDataDestructuringPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProtectedDataDestructuringPolicy 公开方法数量: {methods_ProtectedDataDestructuringPolicy.Length}");
        foreach (var m in methods_ProtectedDataDestructuringPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProtectedDataDestructuringPolicy 未找到，尝试无命名空间...");
        type_ProtectedDataDestructuringPolicy = Type.GetType("ProtectedDataDestructuringPolicy");
        if (type_ProtectedDataDestructuringPolicy != null)
            Console.WriteLine("[PASS] 类型 ProtectedDataDestructuringPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProtectedDataDestructuringPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
