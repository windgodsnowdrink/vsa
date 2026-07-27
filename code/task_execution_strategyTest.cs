#load "task_execution_strategy.cs"

Console.WriteLine("=== task_execution_strategy.cs Test ===");

try
{
    // 验证 class: TaskExecutionStrategy
    var type_TaskExecutionStrategy = Type.GetType("TaskExecutionStrategy");
    if (type_TaskExecutionStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 TaskExecutionStrategy (class) 存在");
        var ctors_TaskExecutionStrategy = type_TaskExecutionStrategy.GetConstructors();
        Console.WriteLine($"[PASS] TaskExecutionStrategy 构造函数数量: {ctors_TaskExecutionStrategy.Length}");
        var methods_TaskExecutionStrategy = type_TaskExecutionStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TaskExecutionStrategy 公开方法数量: {methods_TaskExecutionStrategy.Length}");
        foreach (var m in methods_TaskExecutionStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TaskExecutionStrategy 未找到，尝试无命名空间...");
        type_TaskExecutionStrategy = Type.GetType("TaskExecutionStrategy");
        if (type_TaskExecutionStrategy != null)
            Console.WriteLine("[PASS] 类型 TaskExecutionStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TaskExecutionStrategy 可能为顶层语句或嵌套类型");
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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
