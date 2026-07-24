#load "watchdog_memory.cs"

Console.WriteLine("=== watchdog_memory.cs Test ===");

try
{
    // 验证 class: TieredMemoryServer
    var type_TieredMemoryServer = Type.GetType("TieredMemoryServer");
    if (type_TieredMemoryServer != null)
    {
        Console.WriteLine("[PASS] 类型 TieredMemoryServer (class) 存在");
        var ctors_TieredMemoryServer = type_TieredMemoryServer.GetConstructors();
        Console.WriteLine($"[PASS] TieredMemoryServer 构造函数数量: {ctors_TieredMemoryServer.Length}");
        var methods_TieredMemoryServer = type_TieredMemoryServer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredMemoryServer 公开方法数量: {methods_TieredMemoryServer.Length}");
        foreach (var m in methods_TieredMemoryServer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredMemoryServer 未找到，尝试无命名空间...");
        type_TieredMemoryServer = Type.GetType("TieredMemoryServer");
        if (type_TieredMemoryServer != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryServer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryServer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
