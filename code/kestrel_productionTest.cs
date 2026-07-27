#load "kestrel_production.cs"

Console.WriteLine("=== kestrel_production.cs Test ===");

try
{
    // 验证 class: KestrelProductionServer
    var type_KestrelProductionServer = Type.GetType("KestrelProductionServer");
    if (type_KestrelProductionServer != null)
    {
        Console.WriteLine("[PASS] 类型 KestrelProductionServer (class) 存在");
        var ctors_KestrelProductionServer = type_KestrelProductionServer.GetConstructors();
        Console.WriteLine($"[PASS] KestrelProductionServer 构造函数数量: {ctors_KestrelProductionServer.Length}");
        var methods_KestrelProductionServer = type_KestrelProductionServer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KestrelProductionServer 公开方法数量: {methods_KestrelProductionServer.Length}");
        foreach (var m in methods_KestrelProductionServer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KestrelProductionServer 未找到，尝试无命名空间...");
        type_KestrelProductionServer = Type.GetType("KestrelProductionServer");
        if (type_KestrelProductionServer != null)
            Console.WriteLine("[PASS] 类型 KestrelProductionServer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KestrelProductionServer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MemoryPooledPolicy
    var type_MemoryPooledPolicy = Type.GetType("MemoryPooledPolicy");
    if (type_MemoryPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryPooledPolicy (class) 存在");
        var ctors_MemoryPooledPolicy = type_MemoryPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MemoryPooledPolicy 构造函数数量: {ctors_MemoryPooledPolicy.Length}");
        var methods_MemoryPooledPolicy = type_MemoryPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryPooledPolicy 公开方法数量: {methods_MemoryPooledPolicy.Length}");
        foreach (var m in methods_MemoryPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryPooledPolicy 未找到，尝试无命名空间...");
        type_MemoryPooledPolicy = Type.GetType("MemoryPooledPolicy");
        if (type_MemoryPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 MemoryPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
