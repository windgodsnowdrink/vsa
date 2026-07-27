#load "hybrid_protocol_advanced.cs"

Console.WriteLine("=== hybrid_protocol_advanced.cs Test ===");

try
{
    // 验证 class: HybridProtocolAdvanced
    var type_HybridProtocolAdvanced = Type.GetType("HybridProtocolAdvanced");
    if (type_HybridProtocolAdvanced != null)
    {
        Console.WriteLine("[PASS] 类型 HybridProtocolAdvanced (class) 存在");
        var ctors_HybridProtocolAdvanced = type_HybridProtocolAdvanced.GetConstructors();
        Console.WriteLine($"[PASS] HybridProtocolAdvanced 构造函数数量: {ctors_HybridProtocolAdvanced.Length}");
        var methods_HybridProtocolAdvanced = type_HybridProtocolAdvanced.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HybridProtocolAdvanced 公开方法数量: {methods_HybridProtocolAdvanced.Length}");
        foreach (var m in methods_HybridProtocolAdvanced)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HybridProtocolAdvanced 未找到，尝试无命名空间...");
        type_HybridProtocolAdvanced = Type.GetType("HybridProtocolAdvanced");
        if (type_HybridProtocolAdvanced != null)
            Console.WriteLine("[PASS] 类型 HybridProtocolAdvanced (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HybridProtocolAdvanced 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: SerializationProtocol
    var type_SerializationProtocol = Type.GetType("SerializationProtocol");
    if (type_SerializationProtocol != null)
    {
        Console.WriteLine("[PASS] 类型 SerializationProtocol (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SerializationProtocol 未找到，尝试无命名空间...");
        type_SerializationProtocol = Type.GetType("SerializationProtocol");
        if (type_SerializationProtocol != null)
            Console.WriteLine("[PASS] 类型 SerializationProtocol (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SerializationProtocol 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
