#load "esquio_scheduler_service.cs"

Console.WriteLine("=== esquio_scheduler_service.cs Test ===");

try
{
    // 验证 class: EsquioScheduler
    var type_EsquioScheduler = Type.GetType("EsquioScheduler");
    if (type_EsquioScheduler != null)
    {
        Console.WriteLine("[PASS] 类型 EsquioScheduler (class) 存在");
        var ctors_EsquioScheduler = type_EsquioScheduler.GetConstructors();
        Console.WriteLine($"[PASS] EsquioScheduler 构造函数数量: {ctors_EsquioScheduler.Length}");
        var methods_EsquioScheduler = type_EsquioScheduler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EsquioScheduler 公开方法数量: {methods_EsquioScheduler.Length}");
        foreach (var m in methods_EsquioScheduler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EsquioScheduler 未找到，尝试无命名空间...");
        type_EsquioScheduler = Type.GetType("EsquioScheduler");
        if (type_EsquioScheduler != null)
            Console.WriteLine("[PASS] 类型 EsquioScheduler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EsquioScheduler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JobContext
    var type_JobContext = Type.GetType("JobContext");
    if (type_JobContext != null)
    {
        Console.WriteLine("[PASS] 类型 JobContext (class) 存在");
        var ctors_JobContext = type_JobContext.GetConstructors();
        Console.WriteLine($"[PASS] JobContext 构造函数数量: {ctors_JobContext.Length}");
        var methods_JobContext = type_JobContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JobContext 公开方法数量: {methods_JobContext.Length}");
        foreach (var m in methods_JobContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JobContext 未找到，尝试无命名空间...");
        type_JobContext = Type.GetType("JobContext");
        if (type_JobContext != null)
            Console.WriteLine("[PASS] 类型 JobContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JobContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JobContextPooledPolicy
    var type_JobContextPooledPolicy = Type.GetType("JobContextPooledPolicy");
    if (type_JobContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 JobContextPooledPolicy (class) 存在");
        var ctors_JobContextPooledPolicy = type_JobContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] JobContextPooledPolicy 构造函数数量: {ctors_JobContextPooledPolicy.Length}");
        var methods_JobContextPooledPolicy = type_JobContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JobContextPooledPolicy 公开方法数量: {methods_JobContextPooledPolicy.Length}");
        foreach (var m in methods_JobContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JobContextPooledPolicy 未找到，尝试无命名空间...");
        type_JobContextPooledPolicy = Type.GetType("JobContextPooledPolicy");
        if (type_JobContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 JobContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JobContextPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ScheduledJob
    var type_ScheduledJob = Type.GetType("ScheduledJob");
    if (type_ScheduledJob != null)
    {
        Console.WriteLine("[PASS] 类型 ScheduledJob (record) 存在");
        var ctors_ScheduledJob = type_ScheduledJob.GetConstructors();
        Console.WriteLine($"[PASS] ScheduledJob 构造函数数量: {ctors_ScheduledJob.Length}");
        var methods_ScheduledJob = type_ScheduledJob.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScheduledJob 公开方法数量: {methods_ScheduledJob.Length}");
        foreach (var m in methods_ScheduledJob)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScheduledJob 未找到，尝试无命名空间...");
        type_ScheduledJob = Type.GetType("ScheduledJob");
        if (type_ScheduledJob != null)
            Console.WriteLine("[PASS] 类型 ScheduledJob (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScheduledJob 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
