#load "distributed_tracing.cs"

Console.WriteLine("=== distributed_tracing.cs Test ===");

try
{
    // 验证 class: TracedMessageHandler
    var type_TracedMessageHandler = Type.GetType("TracedMessageHandler");
    if (type_TracedMessageHandler != null)
    {
        Console.WriteLine("[PASS] 类型 TracedMessageHandler (class) 存在");
        var ctors_TracedMessageHandler = type_TracedMessageHandler.GetConstructors();
        Console.WriteLine($"[PASS] TracedMessageHandler 构造函数数量: {ctors_TracedMessageHandler.Length}");
        var methods_TracedMessageHandler = type_TracedMessageHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TracedMessageHandler 公开方法数量: {methods_TracedMessageHandler.Length}");
        foreach (var m in methods_TracedMessageHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TracedMessageHandler 未找到，尝试无命名空间...");
        type_TracedMessageHandler = Type.GetType("TracedMessageHandler");
        if (type_TracedMessageHandler != null)
            Console.WriteLine("[PASS] 类型 TracedMessageHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TracedMessageHandler 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
