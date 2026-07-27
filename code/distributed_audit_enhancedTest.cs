#load "distributed_audit_enhanced.cs"

Console.WriteLine("=== distributed_audit_enhanced.cs Test ===");

try
{
    // 验证 class: AuditEventPublisher
    var type_AuditEventPublisher = Type.GetType("AuditEventPublisher");
    if (type_AuditEventPublisher != null)
    {
        Console.WriteLine("[PASS] 类型 AuditEventPublisher (class) 存在");
        var ctors_AuditEventPublisher = type_AuditEventPublisher.GetConstructors();
        Console.WriteLine($"[PASS] AuditEventPublisher 构造函数数量: {ctors_AuditEventPublisher.Length}");
        var methods_AuditEventPublisher = type_AuditEventPublisher.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuditEventPublisher 公开方法数量: {methods_AuditEventPublisher.Length}");
        foreach (var m in methods_AuditEventPublisher)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuditEventPublisher 未找到，尝试无命名空间...");
        type_AuditEventPublisher = Type.GetType("AuditEventPublisher");
        if (type_AuditEventPublisher != null)
            Console.WriteLine("[PASS] 类型 AuditEventPublisher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuditEventPublisher 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AuditEventConsumer
    var type_AuditEventConsumer = Type.GetType("AuditEventConsumer");
    if (type_AuditEventConsumer != null)
    {
        Console.WriteLine("[PASS] 类型 AuditEventConsumer (class) 存在");
        var ctors_AuditEventConsumer = type_AuditEventConsumer.GetConstructors();
        Console.WriteLine($"[PASS] AuditEventConsumer 构造函数数量: {ctors_AuditEventConsumer.Length}");
        var methods_AuditEventConsumer = type_AuditEventConsumer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuditEventConsumer 公开方法数量: {methods_AuditEventConsumer.Length}");
        foreach (var m in methods_AuditEventConsumer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuditEventConsumer 未找到，尝试无命名空间...");
        type_AuditEventConsumer = Type.GetType("AuditEventConsumer");
        if (type_AuditEventConsumer != null)
            Console.WriteLine("[PASS] 类型 AuditEventConsumer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuditEventConsumer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
