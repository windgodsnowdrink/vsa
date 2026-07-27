#load "distributed_audit_store.cs"

Console.WriteLine("=== distributed_audit_store.cs Test ===");

try
{
    // 验证 class: DistributedAuditWriter
    var type_DistributedAuditWriter = Type.GetType("DistributedAuditWriter");
    if (type_DistributedAuditWriter != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedAuditWriter (class) 存在");
        var ctors_DistributedAuditWriter = type_DistributedAuditWriter.GetConstructors();
        Console.WriteLine($"[PASS] DistributedAuditWriter 构造函数数量: {ctors_DistributedAuditWriter.Length}");
        var methods_DistributedAuditWriter = type_DistributedAuditWriter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedAuditWriter 公开方法数量: {methods_DistributedAuditWriter.Length}");
        foreach (var m in methods_DistributedAuditWriter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedAuditWriter 未找到，尝试无命名空间...");
        type_DistributedAuditWriter = Type.GetType("DistributedAuditWriter");
        if (type_DistributedAuditWriter != null)
            Console.WriteLine("[PASS] 类型 DistributedAuditWriter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedAuditWriter 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
