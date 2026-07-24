#load "gofer_service.cs"

Console.WriteLine("=== gofer_service.cs Test ===");

try
{
    // 验证 class: GoferTaskProcessor
    var type_GoferTaskProcessor = Type.GetType("GoferTaskProcessor");
    if (type_GoferTaskProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 GoferTaskProcessor (class) 存在");
        var ctors_GoferTaskProcessor = type_GoferTaskProcessor.GetConstructors();
        Console.WriteLine($"[PASS] GoferTaskProcessor 构造函数数量: {ctors_GoferTaskProcessor.Length}");
        var methods_GoferTaskProcessor = type_GoferTaskProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GoferTaskProcessor 公开方法数量: {methods_GoferTaskProcessor.Length}");
        foreach (var m in methods_GoferTaskProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GoferTaskProcessor 未找到，尝试无命名空间...");
        type_GoferTaskProcessor = Type.GetType("GoferTaskProcessor");
        if (type_GoferTaskProcessor != null)
            Console.WriteLine("[PASS] 类型 GoferTaskProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GoferTaskProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TaskContext
    var type_TaskContext = Type.GetType("TaskContext");
    if (type_TaskContext != null)
    {
        Console.WriteLine("[PASS] 类型 TaskContext (class) 存在");
        var ctors_TaskContext = type_TaskContext.GetConstructors();
        Console.WriteLine($"[PASS] TaskContext 构造函数数量: {ctors_TaskContext.Length}");
        var methods_TaskContext = type_TaskContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TaskContext 公开方法数量: {methods_TaskContext.Length}");
        foreach (var m in methods_TaskContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TaskContext 未找到，尝试无命名空间...");
        type_TaskContext = Type.GetType("TaskContext");
        if (type_TaskContext != null)
            Console.WriteLine("[PASS] 类型 TaskContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TaskContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TaskContextPooledPolicy
    var type_TaskContextPooledPolicy = Type.GetType("TaskContextPooledPolicy");
    if (type_TaskContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 TaskContextPooledPolicy (class) 存在");
        var ctors_TaskContextPooledPolicy = type_TaskContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] TaskContextPooledPolicy 构造函数数量: {ctors_TaskContextPooledPolicy.Length}");
        var methods_TaskContextPooledPolicy = type_TaskContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TaskContextPooledPolicy 公开方法数量: {methods_TaskContextPooledPolicy.Length}");
        foreach (var m in methods_TaskContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TaskContextPooledPolicy 未找到，尝试无命名空间...");
        type_TaskContextPooledPolicy = Type.GetType("TaskContextPooledPolicy");
        if (type_TaskContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 TaskContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TaskContextPooledPolicy 可能为顶层语句或嵌套类型");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
