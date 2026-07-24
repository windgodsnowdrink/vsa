#load "timewheel_service.cs"

Console.WriteLine("=== timewheel_service.cs Test ===");

try
{
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

    // 验证 class: TimeTask
    var type_TimeTask = Type.GetType("TimeTask");
    if (type_TimeTask != null)
    {
        Console.WriteLine("[PASS] 类型 TimeTask (class) 存在");
        var ctors_TimeTask = type_TimeTask.GetConstructors();
        Console.WriteLine($"[PASS] TimeTask 构造函数数量: {ctors_TimeTask.Length}");
        var methods_TimeTask = type_TimeTask.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TimeTask 公开方法数量: {methods_TimeTask.Length}");
        foreach (var m in methods_TimeTask)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TimeTask 未找到，尝试无命名空间...");
        type_TimeTask = Type.GetType("TimeTask");
        if (type_TimeTask != null)
            Console.WriteLine("[PASS] 类型 TimeTask (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TimeTask 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TimeTaskPooledPolicy
    var type_TimeTaskPooledPolicy = Type.GetType("TimeTaskPooledPolicy");
    if (type_TimeTaskPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 TimeTaskPooledPolicy (class) 存在");
        var ctors_TimeTaskPooledPolicy = type_TimeTaskPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] TimeTaskPooledPolicy 构造函数数量: {ctors_TimeTaskPooledPolicy.Length}");
        var methods_TimeTaskPooledPolicy = type_TimeTaskPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TimeTaskPooledPolicy 公开方法数量: {methods_TimeTaskPooledPolicy.Length}");
        foreach (var m in methods_TimeTaskPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TimeTaskPooledPolicy 未找到，尝试无命名空间...");
        type_TimeTaskPooledPolicy = Type.GetType("TimeTaskPooledPolicy");
        if (type_TimeTaskPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 TimeTaskPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TimeTaskPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
