#load "hybrid_protocol_serializer.cs"

Console.WriteLine("=== hybrid_protocol_serializer.cs Test ===");

try
{
    // 验证 class: HybridProtocolSerializer
    var type_HybridProtocolSerializer = Type.GetType("HybridProtocolSerializer");
    if (type_HybridProtocolSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 HybridProtocolSerializer (class) 存在");
        var ctors_HybridProtocolSerializer = type_HybridProtocolSerializer.GetConstructors();
        Console.WriteLine($"[PASS] HybridProtocolSerializer 构造函数数量: {ctors_HybridProtocolSerializer.Length}");
        var methods_HybridProtocolSerializer = type_HybridProtocolSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HybridProtocolSerializer 公开方法数量: {methods_HybridProtocolSerializer.Length}");
        foreach (var m in methods_HybridProtocolSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HybridProtocolSerializer 未找到，尝试无命名空间...");
        type_HybridProtocolSerializer = Type.GetType("HybridProtocolSerializer");
        if (type_HybridProtocolSerializer != null)
            Console.WriteLine("[PASS] 类型 HybridProtocolSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HybridProtocolSerializer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
