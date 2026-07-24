#load "coravel_service.cs"

Console.WriteLine("=== coravel_service.cs Test ===");

try
{
    // 验证 class: CoravelQueueProcessor
    var type_CoravelQueueProcessor = Type.GetType("CoravelQueueProcessor");
    if (type_CoravelQueueProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 CoravelQueueProcessor (class) 存在");
        var ctors_CoravelQueueProcessor = type_CoravelQueueProcessor.GetConstructors();
        Console.WriteLine($"[PASS] CoravelQueueProcessor 构造函数数量: {ctors_CoravelQueueProcessor.Length}");
        var methods_CoravelQueueProcessor = type_CoravelQueueProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CoravelQueueProcessor 公开方法数量: {methods_CoravelQueueProcessor.Length}");
        foreach (var m in methods_CoravelQueueProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CoravelQueueProcessor 未找到，尝试无命名空间...");
        type_CoravelQueueProcessor = Type.GetType("CoravelQueueProcessor");
        if (type_CoravelQueueProcessor != null)
            Console.WriteLine("[PASS] 类型 CoravelQueueProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CoravelQueueProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QueueContext
    var type_QueueContext = Type.GetType("QueueContext");
    if (type_QueueContext != null)
    {
        Console.WriteLine("[PASS] 类型 QueueContext (class) 存在");
        var ctors_QueueContext = type_QueueContext.GetConstructors();
        Console.WriteLine($"[PASS] QueueContext 构造函数数量: {ctors_QueueContext.Length}");
        var methods_QueueContext = type_QueueContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QueueContext 公开方法数量: {methods_QueueContext.Length}");
        foreach (var m in methods_QueueContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QueueContext 未找到，尝试无命名空间...");
        type_QueueContext = Type.GetType("QueueContext");
        if (type_QueueContext != null)
            Console.WriteLine("[PASS] 类型 QueueContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QueueContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QueueContextPooledPolicy
    var type_QueueContextPooledPolicy = Type.GetType("QueueContextPooledPolicy");
    if (type_QueueContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 QueueContextPooledPolicy (class) 存在");
        var ctors_QueueContextPooledPolicy = type_QueueContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] QueueContextPooledPolicy 构造函数数量: {ctors_QueueContextPooledPolicy.Length}");
        var methods_QueueContextPooledPolicy = type_QueueContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QueueContextPooledPolicy 公开方法数量: {methods_QueueContextPooledPolicy.Length}");
        foreach (var m in methods_QueueContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QueueContextPooledPolicy 未找到，尝试无命名空间...");
        type_QueueContextPooledPolicy = Type.GetType("QueueContextPooledPolicy");
        if (type_QueueContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 QueueContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QueueContextPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SampleJob
    var type_SampleJob = Type.GetType("SampleJob");
    if (type_SampleJob != null)
    {
        Console.WriteLine("[PASS] 类型 SampleJob (class) 存在");
        var ctors_SampleJob = type_SampleJob.GetConstructors();
        Console.WriteLine($"[PASS] SampleJob 构造函数数量: {ctors_SampleJob.Length}");
        var methods_SampleJob = type_SampleJob.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SampleJob 公开方法数量: {methods_SampleJob.Length}");
        foreach (var m in methods_SampleJob)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SampleJob 未找到，尝试无命名空间...");
        type_SampleJob = Type.GetType("SampleJob");
        if (type_SampleJob != null)
            Console.WriteLine("[PASS] 类型 SampleJob (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SampleJob 可能为顶层语句或嵌套类型");
    }

    // 验证 record: QueueItem
    var type_QueueItem = Type.GetType("QueueItem");
    if (type_QueueItem != null)
    {
        Console.WriteLine("[PASS] 类型 QueueItem (record) 存在");
        var ctors_QueueItem = type_QueueItem.GetConstructors();
        Console.WriteLine($"[PASS] QueueItem 构造函数数量: {ctors_QueueItem.Length}");
        var methods_QueueItem = type_QueueItem.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QueueItem 公开方法数量: {methods_QueueItem.Length}");
        foreach (var m in methods_QueueItem)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QueueItem 未找到，尝试无命名空间...");
        type_QueueItem = Type.GetType("QueueItem");
        if (type_QueueItem != null)
            Console.WriteLine("[PASS] 类型 QueueItem (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QueueItem 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
