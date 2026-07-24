#load "task_scheduler_service.cs"

Console.WriteLine("=== task_scheduler_service.cs Test ===");

try
{
    // 验证 class: TaskSchedulerService
    var type_TaskSchedulerService = Type.GetType("TaskSchedulerService");
    if (type_TaskSchedulerService != null)
    {
        Console.WriteLine("[PASS] 类型 TaskSchedulerService (class) 存在");
        var ctors_TaskSchedulerService = type_TaskSchedulerService.GetConstructors();
        Console.WriteLine($"[PASS] TaskSchedulerService 构造函数数量: {ctors_TaskSchedulerService.Length}");
        var methods_TaskSchedulerService = type_TaskSchedulerService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TaskSchedulerService 公开方法数量: {methods_TaskSchedulerService.Length}");
        foreach (var m in methods_TaskSchedulerService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TaskSchedulerService 未找到，尝试无命名空间...");
        type_TaskSchedulerService = Type.GetType("TaskSchedulerService");
        if (type_TaskSchedulerService != null)
            Console.WriteLine("[PASS] 类型 TaskSchedulerService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TaskSchedulerService 可能为顶层语句或嵌套类型");
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

    // 验证 record: ScheduledTask
    var type_ScheduledTask = Type.GetType("ScheduledTask");
    if (type_ScheduledTask != null)
    {
        Console.WriteLine("[PASS] 类型 ScheduledTask (record) 存在");
        var ctors_ScheduledTask = type_ScheduledTask.GetConstructors();
        Console.WriteLine($"[PASS] ScheduledTask 构造函数数量: {ctors_ScheduledTask.Length}");
        var methods_ScheduledTask = type_ScheduledTask.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScheduledTask 公开方法数量: {methods_ScheduledTask.Length}");
        foreach (var m in methods_ScheduledTask)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScheduledTask 未找到，尝试无命名空间...");
        type_ScheduledTask = Type.GetType("ScheduledTask");
        if (type_ScheduledTask != null)
            Console.WriteLine("[PASS] 类型 ScheduledTask (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScheduledTask 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
