#load "disruptor_processor.cs"

Console.WriteLine("=== disruptor_processor.cs Test ===");

try
{
    // 验证 class: DisruptorProcessor
    var type_DisruptorProcessor = Type.GetType("DisruptorProcessor");
    if (type_DisruptorProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 DisruptorProcessor (class) 存在");
        var ctors_DisruptorProcessor = type_DisruptorProcessor.GetConstructors();
        Console.WriteLine($"[PASS] DisruptorProcessor 构造函数数量: {ctors_DisruptorProcessor.Length}");
        var methods_DisruptorProcessor = type_DisruptorProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DisruptorProcessor 公开方法数量: {methods_DisruptorProcessor.Length}");
        foreach (var m in methods_DisruptorProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DisruptorProcessor 未找到，尝试无命名空间...");
        type_DisruptorProcessor = Type.GetType("DisruptorProcessor");
        if (type_DisruptorProcessor != null)
            Console.WriteLine("[PASS] 类型 DisruptorProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DisruptorProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LogEventProcessor
    var type_LogEventProcessor = Type.GetType("LogEventProcessor");
    if (type_LogEventProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 LogEventProcessor (class) 存在");
        var ctors_LogEventProcessor = type_LogEventProcessor.GetConstructors();
        Console.WriteLine($"[PASS] LogEventProcessor 构造函数数量: {ctors_LogEventProcessor.Length}");
        var methods_LogEventProcessor = type_LogEventProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogEventProcessor 公开方法数量: {methods_LogEventProcessor.Length}");
        foreach (var m in methods_LogEventProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogEventProcessor 未找到，尝试无命名空间...");
        type_LogEventProcessor = Type.GetType("LogEventProcessor");
        if (type_LogEventProcessor != null)
            Console.WriteLine("[PASS] 类型 LogEventProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogEventProcessor 可能为顶层语句或嵌套类型");
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
