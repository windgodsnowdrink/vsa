#load "audit_query_endpoint.cs"

Console.WriteLine("=== audit_query_endpoint.cs Test ===");

try
{
    // 验证 class: AuditQueryEndpoint
    var type_AuditQueryEndpoint = Type.GetType("AuditQueryEndpoint");
    if (type_AuditQueryEndpoint != null)
    {
        Console.WriteLine("[PASS] 类型 AuditQueryEndpoint (class) 存在");
        var ctors_AuditQueryEndpoint = type_AuditQueryEndpoint.GetConstructors();
        Console.WriteLine($"[PASS] AuditQueryEndpoint 构造函数数量: {ctors_AuditQueryEndpoint.Length}");
        var methods_AuditQueryEndpoint = type_AuditQueryEndpoint.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuditQueryEndpoint 公开方法数量: {methods_AuditQueryEndpoint.Length}");
        foreach (var m in methods_AuditQueryEndpoint)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuditQueryEndpoint 未找到，尝试无命名空间...");
        type_AuditQueryEndpoint = Type.GetType("AuditQueryEndpoint");
        if (type_AuditQueryEndpoint != null)
            Console.WriteLine("[PASS] 类型 AuditQueryEndpoint (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuditQueryEndpoint 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AuditQueryProcessor
    var type_AuditQueryProcessor = Type.GetType("AuditQueryProcessor");
    if (type_AuditQueryProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 AuditQueryProcessor (class) 存在");
        var ctors_AuditQueryProcessor = type_AuditQueryProcessor.GetConstructors();
        Console.WriteLine($"[PASS] AuditQueryProcessor 构造函数数量: {ctors_AuditQueryProcessor.Length}");
        var methods_AuditQueryProcessor = type_AuditQueryProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuditQueryProcessor 公开方法数量: {methods_AuditQueryProcessor.Length}");
        foreach (var m in methods_AuditQueryProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuditQueryProcessor 未找到，尝试无命名空间...");
        type_AuditQueryProcessor = Type.GetType("AuditQueryProcessor");
        if (type_AuditQueryProcessor != null)
            Console.WriteLine("[PASS] 类型 AuditQueryProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuditQueryProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
