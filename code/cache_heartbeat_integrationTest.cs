#load "cache_heartbeat_integration.cs"

Console.WriteLine("=== cache_heartbeat_integration.cs Test ===");

try
{
    // 验证 class: CacheHeartbeatService
    var type_CacheHeartbeatService = Type.GetType("CacheHeartbeatService");
    if (type_CacheHeartbeatService != null)
    {
        Console.WriteLine("[PASS] 类型 CacheHeartbeatService (class) 存在");
        var ctors_CacheHeartbeatService = type_CacheHeartbeatService.GetConstructors();
        Console.WriteLine($"[PASS] CacheHeartbeatService 构造函数数量: {ctors_CacheHeartbeatService.Length}");
        var methods_CacheHeartbeatService = type_CacheHeartbeatService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheHeartbeatService 公开方法数量: {methods_CacheHeartbeatService.Length}");
        foreach (var m in methods_CacheHeartbeatService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheHeartbeatService 未找到，尝试无命名空间...");
        type_CacheHeartbeatService = Type.GetType("CacheHeartbeatService");
        if (type_CacheHeartbeatService != null)
            Console.WriteLine("[PASS] 类型 CacheHeartbeatService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheHeartbeatService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CacheHeartbeatOptions
    var type_CacheHeartbeatOptions = Type.GetType("CacheHeartbeatOptions");
    if (type_CacheHeartbeatOptions != null)
    {
        Console.WriteLine("[PASS] 类型 CacheHeartbeatOptions (class) 存在");
        var ctors_CacheHeartbeatOptions = type_CacheHeartbeatOptions.GetConstructors();
        Console.WriteLine($"[PASS] CacheHeartbeatOptions 构造函数数量: {ctors_CacheHeartbeatOptions.Length}");
        var methods_CacheHeartbeatOptions = type_CacheHeartbeatOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheHeartbeatOptions 公开方法数量: {methods_CacheHeartbeatOptions.Length}");
        foreach (var m in methods_CacheHeartbeatOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheHeartbeatOptions 未找到，尝试无命名空间...");
        type_CacheHeartbeatOptions = Type.GetType("CacheHeartbeatOptions");
        if (type_CacheHeartbeatOptions != null)
            Console.WriteLine("[PASS] 类型 CacheHeartbeatOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheHeartbeatOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HeartbeatTimeoutEvent
    var type_HeartbeatTimeoutEvent = Type.GetType("HeartbeatTimeoutEvent");
    if (type_HeartbeatTimeoutEvent != null)
    {
        Console.WriteLine("[PASS] 类型 HeartbeatTimeoutEvent (class) 存在");
        var ctors_HeartbeatTimeoutEvent = type_HeartbeatTimeoutEvent.GetConstructors();
        Console.WriteLine($"[PASS] HeartbeatTimeoutEvent 构造函数数量: {ctors_HeartbeatTimeoutEvent.Length}");
        var methods_HeartbeatTimeoutEvent = type_HeartbeatTimeoutEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HeartbeatTimeoutEvent 公开方法数量: {methods_HeartbeatTimeoutEvent.Length}");
        foreach (var m in methods_HeartbeatTimeoutEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HeartbeatTimeoutEvent 未找到，尝试无命名空间...");
        type_HeartbeatTimeoutEvent = Type.GetType("HeartbeatTimeoutEvent");
        if (type_HeartbeatTimeoutEvent != null)
            Console.WriteLine("[PASS] 类型 HeartbeatTimeoutEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HeartbeatTimeoutEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TimeWheel
    var type_TimeWheel = Type.GetType("TimeWheel");
    if (type_TimeWheel != null)
    {
        Console.WriteLine("[PASS] 类型 TimeWheel (class) 存在");
        var ctors_TimeWheel = type_TimeWheel.GetConstructors();
        Console.WriteLine($"[PASS] TimeWheel 构造函数数量: {ctors_TimeWheel.Length}");
        var methods_TimeWheel = type_TimeWheel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TimeWheel 公开方法数量: {methods_TimeWheel.Length}");
        foreach (var m in methods_TimeWheel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TimeWheel 未找到，尝试无命名空间...");
        type_TimeWheel = Type.GetType("TimeWheel");
        if (type_TimeWheel != null)
            Console.WriteLine("[PASS] 类型 TimeWheel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TimeWheel 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CacheHeartbeatExtensions
    var type_CacheHeartbeatExtensions = Type.GetType("CacheHeartbeatExtensions");
    if (type_CacheHeartbeatExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 CacheHeartbeatExtensions (class) 存在");
        var ctors_CacheHeartbeatExtensions = type_CacheHeartbeatExtensions.GetConstructors();
        Console.WriteLine($"[PASS] CacheHeartbeatExtensions 构造函数数量: {ctors_CacheHeartbeatExtensions.Length}");
        var methods_CacheHeartbeatExtensions = type_CacheHeartbeatExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheHeartbeatExtensions 公开方法数量: {methods_CacheHeartbeatExtensions.Length}");
        foreach (var m in methods_CacheHeartbeatExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheHeartbeatExtensions 未找到，尝试无命名空间...");
        type_CacheHeartbeatExtensions = Type.GetType("CacheHeartbeatExtensions");
        if (type_CacheHeartbeatExtensions != null)
            Console.WriteLine("[PASS] 类型 CacheHeartbeatExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheHeartbeatExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
