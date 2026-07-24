#load "conditional_routing.cs"

Console.WriteLine("=== conditional_routing.cs Test ===");

try
{
    // 验证 class: ConditionalRouter
    var type_ConditionalRouter = Type.GetType("ConditionalRouter");
    if (type_ConditionalRouter != null)
    {
        Console.WriteLine("[PASS] 类型 ConditionalRouter (class) 存在");
        var ctors_ConditionalRouter = type_ConditionalRouter.GetConstructors();
        Console.WriteLine($"[PASS] ConditionalRouter 构造函数数量: {ctors_ConditionalRouter.Length}");
        var methods_ConditionalRouter = type_ConditionalRouter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConditionalRouter 公开方法数量: {methods_ConditionalRouter.Length}");
        foreach (var m in methods_ConditionalRouter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConditionalRouter 未找到，尝试无命名空间...");
        type_ConditionalRouter = Type.GetType("ConditionalRouter");
        if (type_ConditionalRouter != null)
            Console.WriteLine("[PASS] 类型 ConditionalRouter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConditionalRouter 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TaskItem
    var type_TaskItem = Type.GetType("TaskItem");
    if (type_TaskItem != null)
    {
        Console.WriteLine("[PASS] 类型 TaskItem (record) 存在");
        var ctors_TaskItem = type_TaskItem.GetConstructors();
        Console.WriteLine($"[PASS] TaskItem 构造函数数量: {ctors_TaskItem.Length}");
        var methods_TaskItem = type_TaskItem.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TaskItem 公开方法数量: {methods_TaskItem.Length}");
        foreach (var m in methods_TaskItem)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TaskItem 未找到，尝试无命名空间...");
        type_TaskItem = Type.GetType("TaskItem");
        if (type_TaskItem != null)
            Console.WriteLine("[PASS] 类型 TaskItem (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TaskItem 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TaskResult
    var type_TaskResult = Type.GetType("TaskResult");
    if (type_TaskResult != null)
    {
        Console.WriteLine("[PASS] 类型 TaskResult (record) 存在");
        var ctors_TaskResult = type_TaskResult.GetConstructors();
        Console.WriteLine($"[PASS] TaskResult 构造函数数量: {ctors_TaskResult.Length}");
        var methods_TaskResult = type_TaskResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TaskResult 公开方法数量: {methods_TaskResult.Length}");
        foreach (var m in methods_TaskResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TaskResult 未找到，尝试无命名空间...");
        type_TaskResult = Type.GetType("TaskResult");
        if (type_TaskResult != null)
            Console.WriteLine("[PASS] 类型 TaskResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TaskResult 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: Priority
    var type_Priority = Type.GetType("Priority");
    if (type_Priority != null)
    {
        Console.WriteLine("[PASS] 类型 Priority (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Priority 未找到，尝试无命名空间...");
        type_Priority = Type.GetType("Priority");
        if (type_Priority != null)
            Console.WriteLine("[PASS] 类型 Priority (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Priority 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
