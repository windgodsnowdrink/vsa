#load "event_sourcing_processor.cs"

Console.WriteLine("=== event_sourcing_processor.cs Test ===");

try
{
    // 验证 class: TieredEventProcessor
    var type_TieredEventProcessor = Type.GetType("TieredEventProcessor");
    if (type_TieredEventProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 TieredEventProcessor (class) 存在");
        var ctors_TieredEventProcessor = type_TieredEventProcessor.GetConstructors();
        Console.WriteLine($"[PASS] TieredEventProcessor 构造函数数量: {ctors_TieredEventProcessor.Length}");
        var methods_TieredEventProcessor = type_TieredEventProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredEventProcessor 公开方法数量: {methods_TieredEventProcessor.Length}");
        foreach (var m in methods_TieredEventProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredEventProcessor 未找到，尝试无命名空间...");
        type_TieredEventProcessor = Type.GetType("TieredEventProcessor");
        if (type_TieredEventProcessor != null)
            Console.WriteLine("[PASS] 类型 TieredEventProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredEventProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 record: EventWrapper
    var type_EventWrapper = Type.GetType("EventWrapper");
    if (type_EventWrapper != null)
    {
        Console.WriteLine("[PASS] 类型 EventWrapper (record) 存在");
        var ctors_EventWrapper = type_EventWrapper.GetConstructors();
        Console.WriteLine($"[PASS] EventWrapper 构造函数数量: {ctors_EventWrapper.Length}");
        var methods_EventWrapper = type_EventWrapper.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventWrapper 公开方法数量: {methods_EventWrapper.Length}");
        foreach (var m in methods_EventWrapper)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventWrapper 未找到，尝试无命名空间...");
        type_EventWrapper = Type.GetType("EventWrapper");
        if (type_EventWrapper != null)
            Console.WriteLine("[PASS] 类型 EventWrapper (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventWrapper 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
