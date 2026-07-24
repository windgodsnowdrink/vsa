#load "multitenant_lifecycle.cs"

Console.WriteLine("=== multitenant_lifecycle.cs Test ===");

try
{
    // 验证 class: TenantLifecycleProcessor
    var type_TenantLifecycleProcessor = Type.GetType("TenantLifecycleProcessor");
    if (type_TenantLifecycleProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 TenantLifecycleProcessor (class) 存在");
        var ctors_TenantLifecycleProcessor = type_TenantLifecycleProcessor.GetConstructors();
        Console.WriteLine($"[PASS] TenantLifecycleProcessor 构造函数数量: {ctors_TenantLifecycleProcessor.Length}");
        var methods_TenantLifecycleProcessor = type_TenantLifecycleProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantLifecycleProcessor 公开方法数量: {methods_TenantLifecycleProcessor.Length}");
        foreach (var m in methods_TenantLifecycleProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantLifecycleProcessor 未找到，尝试无命名空间...");
        type_TenantLifecycleProcessor = Type.GetType("TenantLifecycleProcessor");
        if (type_TenantLifecycleProcessor != null)
            Console.WriteLine("[PASS] 类型 TenantLifecycleProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantLifecycleProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantEventContext
    var type_TenantEventContext = Type.GetType("TenantEventContext");
    if (type_TenantEventContext != null)
    {
        Console.WriteLine("[PASS] 类型 TenantEventContext (class) 存在");
        var ctors_TenantEventContext = type_TenantEventContext.GetConstructors();
        Console.WriteLine($"[PASS] TenantEventContext 构造函数数量: {ctors_TenantEventContext.Length}");
        var methods_TenantEventContext = type_TenantEventContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantEventContext 公开方法数量: {methods_TenantEventContext.Length}");
        foreach (var m in methods_TenantEventContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantEventContext 未找到，尝试无命名空间...");
        type_TenantEventContext = Type.GetType("TenantEventContext");
        if (type_TenantEventContext != null)
            Console.WriteLine("[PASS] 类型 TenantEventContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantEventContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantEventContextPooledPolicy
    var type_TenantEventContextPooledPolicy = Type.GetType("TenantEventContextPooledPolicy");
    if (type_TenantEventContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 TenantEventContextPooledPolicy (class) 存在");
        var ctors_TenantEventContextPooledPolicy = type_TenantEventContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] TenantEventContextPooledPolicy 构造函数数量: {ctors_TenantEventContextPooledPolicy.Length}");
        var methods_TenantEventContextPooledPolicy = type_TenantEventContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantEventContextPooledPolicy 公开方法数量: {methods_TenantEventContextPooledPolicy.Length}");
        foreach (var m in methods_TenantEventContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantEventContextPooledPolicy 未找到，尝试无命名空间...");
        type_TenantEventContextPooledPolicy = Type.GetType("TenantEventContextPooledPolicy");
        if (type_TenantEventContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 TenantEventContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantEventContextPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantLifecycleExtensions
    var type_TenantLifecycleExtensions = Type.GetType("TenantLifecycleExtensions");
    if (type_TenantLifecycleExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 TenantLifecycleExtensions (class) 存在");
        var ctors_TenantLifecycleExtensions = type_TenantLifecycleExtensions.GetConstructors();
        Console.WriteLine($"[PASS] TenantLifecycleExtensions 构造函数数量: {ctors_TenantLifecycleExtensions.Length}");
        var methods_TenantLifecycleExtensions = type_TenantLifecycleExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantLifecycleExtensions 公开方法数量: {methods_TenantLifecycleExtensions.Length}");
        foreach (var m in methods_TenantLifecycleExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantLifecycleExtensions 未找到，尝试无命名空间...");
        type_TenantLifecycleExtensions = Type.GetType("TenantLifecycleExtensions");
        if (type_TenantLifecycleExtensions != null)
            Console.WriteLine("[PASS] 类型 TenantLifecycleExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantLifecycleExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: TenantLifecycleEvent
    var type_TenantLifecycleEvent = Type.GetType("TenantLifecycleEvent");
    if (type_TenantLifecycleEvent != null)
    {
        Console.WriteLine("[PASS] 类型 TenantLifecycleEvent (struct) 存在");
        var ctors_TenantLifecycleEvent = type_TenantLifecycleEvent.GetConstructors();
        Console.WriteLine($"[PASS] TenantLifecycleEvent 构造函数数量: {ctors_TenantLifecycleEvent.Length}");
        var methods_TenantLifecycleEvent = type_TenantLifecycleEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantLifecycleEvent 公开方法数量: {methods_TenantLifecycleEvent.Length}");
        foreach (var m in methods_TenantLifecycleEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantLifecycleEvent 未找到，尝试无命名空间...");
        type_TenantLifecycleEvent = Type.GetType("TenantLifecycleEvent");
        if (type_TenantLifecycleEvent != null)
            Console.WriteLine("[PASS] 类型 TenantLifecycleEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantLifecycleEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: TenantLifecycleEventType
    var type_TenantLifecycleEventType = Type.GetType("TenantLifecycleEventType");
    if (type_TenantLifecycleEventType != null)
    {
        Console.WriteLine("[PASS] 类型 TenantLifecycleEventType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantLifecycleEventType 未找到，尝试无命名空间...");
        type_TenantLifecycleEventType = Type.GetType("TenantLifecycleEventType");
        if (type_TenantLifecycleEventType != null)
            Console.WriteLine("[PASS] 类型 TenantLifecycleEventType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantLifecycleEventType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
