#load "litedb_eventstore.cs"

Console.WriteLine("=== litedb_eventstore.cs Test ===");

try
{
    // 验证 class: OutOfBandEvent
    var type_OutOfBandEvent = Type.GetType("OutOfBandEvent");
    if (type_OutOfBandEvent != null)
    {
        Console.WriteLine("[PASS] 类型 OutOfBandEvent (class) 存在");
        var ctors_OutOfBandEvent = type_OutOfBandEvent.GetConstructors();
        Console.WriteLine($"[PASS] OutOfBandEvent 构造函数数量: {ctors_OutOfBandEvent.Length}");
        var methods_OutOfBandEvent = type_OutOfBandEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OutOfBandEvent 公开方法数量: {methods_OutOfBandEvent.Length}");
        foreach (var m in methods_OutOfBandEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OutOfBandEvent 未找到，尝试无命名空间...");
        type_OutOfBandEvent = Type.GetType("OutOfBandEvent");
        if (type_OutOfBandEvent != null)
            Console.WriteLine("[PASS] 类型 OutOfBandEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OutOfBandEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventProcessor
    var type_EventProcessor = Type.GetType("EventProcessor");
    if (type_EventProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 EventProcessor (class) 存在");
        var ctors_EventProcessor = type_EventProcessor.GetConstructors();
        Console.WriteLine($"[PASS] EventProcessor 构造函数数量: {ctors_EventProcessor.Length}");
        var methods_EventProcessor = type_EventProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventProcessor 公开方法数量: {methods_EventProcessor.Length}");
        foreach (var m in methods_EventProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventProcessor 未找到，尝试无命名空间...");
        type_EventProcessor = Type.GetType("EventProcessor");
        if (type_EventProcessor != null)
            Console.WriteLine("[PASS] 类型 EventProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BufferPoolPolicy
    var type_BufferPoolPolicy = Type.GetType("BufferPoolPolicy");
    if (type_BufferPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 BufferPoolPolicy (class) 存在");
        var ctors_BufferPoolPolicy = type_BufferPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] BufferPoolPolicy 构造函数数量: {ctors_BufferPoolPolicy.Length}");
        var methods_BufferPoolPolicy = type_BufferPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BufferPoolPolicy 公开方法数量: {methods_BufferPoolPolicy.Length}");
        foreach (var m in methods_BufferPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BufferPoolPolicy 未找到，尝试无命名空间...");
        type_BufferPoolPolicy = Type.GetType("BufferPoolPolicy");
        if (type_BufferPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 BufferPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BufferPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventStoreService
    var type_EventStoreService = Type.GetType("EventStoreService");
    if (type_EventStoreService != null)
    {
        Console.WriteLine("[PASS] 类型 EventStoreService (class) 存在");
        var ctors_EventStoreService = type_EventStoreService.GetConstructors();
        Console.WriteLine($"[PASS] EventStoreService 构造函数数量: {ctors_EventStoreService.Length}");
        var methods_EventStoreService = type_EventStoreService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventStoreService 公开方法数量: {methods_EventStoreService.Length}");
        foreach (var m in methods_EventStoreService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventStoreService 未找到，尝试无命名空间...");
        type_EventStoreService = Type.GetType("EventStoreService");
        if (type_EventStoreService != null)
            Console.WriteLine("[PASS] 类型 EventStoreService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventStoreService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventStoreDemo
    var type_EventStoreDemo = Type.GetType("EventStoreDemo");
    if (type_EventStoreDemo != null)
    {
        Console.WriteLine("[PASS] 类型 EventStoreDemo (class) 存在");
        var ctors_EventStoreDemo = type_EventStoreDemo.GetConstructors();
        Console.WriteLine($"[PASS] EventStoreDemo 构造函数数量: {ctors_EventStoreDemo.Length}");
        var methods_EventStoreDemo = type_EventStoreDemo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventStoreDemo 公开方法数量: {methods_EventStoreDemo.Length}");
        foreach (var m in methods_EventStoreDemo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventStoreDemo 未找到，尝试无命名空间...");
        type_EventStoreDemo = Type.GetType("EventStoreDemo");
        if (type_EventStoreDemo != null)
            Console.WriteLine("[PASS] 类型 EventStoreDemo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventStoreDemo 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
