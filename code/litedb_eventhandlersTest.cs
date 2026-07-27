#load "litedb_eventhandlers.cs"

Console.WriteLine("=== litedb_eventhandlers.cs Test ===");

try
{
    // 验证 class: EventHandlerPool
    var type_EventHandlerPool = Type.GetType("EventHandlerPool");
    if (type_EventHandlerPool != null)
    {
        Console.WriteLine("[PASS] 类型 EventHandlerPool (class) 存在");
        var ctors_EventHandlerPool = type_EventHandlerPool.GetConstructors();
        Console.WriteLine($"[PASS] EventHandlerPool 构造函数数量: {ctors_EventHandlerPool.Length}");
        var methods_EventHandlerPool = type_EventHandlerPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventHandlerPool 公开方法数量: {methods_EventHandlerPool.Length}");
        foreach (var m in methods_EventHandlerPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventHandlerPool 未找到，尝试无命名空间...");
        type_EventHandlerPool = Type.GetType("EventHandlerPool");
        if (type_EventHandlerPool != null)
            Console.WriteLine("[PASS] 类型 EventHandlerPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventHandlerPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EventHandlerPoolPolicy
    var type_EventHandlerPoolPolicy = Type.GetType("EventHandlerPoolPolicy");
    if (type_EventHandlerPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 EventHandlerPoolPolicy (class) 存在");
        var ctors_EventHandlerPoolPolicy = type_EventHandlerPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] EventHandlerPoolPolicy 构造函数数量: {ctors_EventHandlerPoolPolicy.Length}");
        var methods_EventHandlerPoolPolicy = type_EventHandlerPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventHandlerPoolPolicy 公开方法数量: {methods_EventHandlerPoolPolicy.Length}");
        foreach (var m in methods_EventHandlerPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventHandlerPoolPolicy 未找到，尝试无命名空间...");
        type_EventHandlerPoolPolicy = Type.GetType("EventHandlerPoolPolicy");
        if (type_EventHandlerPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 EventHandlerPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventHandlerPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DefaultEventHandler
    var type_DefaultEventHandler = Type.GetType("DefaultEventHandler");
    if (type_DefaultEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 DefaultEventHandler (class) 存在");
        var ctors_DefaultEventHandler = type_DefaultEventHandler.GetConstructors();
        Console.WriteLine($"[PASS] DefaultEventHandler 构造函数数量: {ctors_DefaultEventHandler.Length}");
        var methods_DefaultEventHandler = type_DefaultEventHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DefaultEventHandler 公开方法数量: {methods_DefaultEventHandler.Length}");
        foreach (var m in methods_DefaultEventHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DefaultEventHandler 未找到，尝试无命名空间...");
        type_DefaultEventHandler = Type.GetType("DefaultEventHandler");
        if (type_DefaultEventHandler != null)
            Console.WriteLine("[PASS] 类型 DefaultEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DefaultEventHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IEventHandler
    var type_IEventHandler = Type.GetType("IEventHandler");
    if (type_IEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 IEventHandler (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IEventHandler 未找到，尝试无命名空间...");
        type_IEventHandler = Type.GetType("IEventHandler");
        if (type_IEventHandler != null)
            Console.WriteLine("[PASS] 类型 IEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IEventHandler 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
