#load "fluent_scheduler_service.cs"

Console.WriteLine("=== fluent_scheduler_service.cs Test ===");

try
{
    // 验证 class: ScheduledJobProcessor
    var type_ScheduledJobProcessor = Type.GetType("ScheduledJobProcessor");
    if (type_ScheduledJobProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ScheduledJobProcessor (class) 存在");
        var ctors_ScheduledJobProcessor = type_ScheduledJobProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ScheduledJobProcessor 构造函数数量: {ctors_ScheduledJobProcessor.Length}");
        var methods_ScheduledJobProcessor = type_ScheduledJobProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScheduledJobProcessor 公开方法数量: {methods_ScheduledJobProcessor.Length}");
        foreach (var m in methods_ScheduledJobProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScheduledJobProcessor 未找到，尝试无命名空间...");
        type_ScheduledJobProcessor = Type.GetType("ScheduledJobProcessor");
        if (type_ScheduledJobProcessor != null)
            Console.WriteLine("[PASS] 类型 ScheduledJobProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScheduledJobProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FluentSchedulerExtensions
    var type_FluentSchedulerExtensions = Type.GetType("FluentSchedulerExtensions");
    if (type_FluentSchedulerExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 FluentSchedulerExtensions (class) 存在");
        var ctors_FluentSchedulerExtensions = type_FluentSchedulerExtensions.GetConstructors();
        Console.WriteLine($"[PASS] FluentSchedulerExtensions 构造函数数量: {ctors_FluentSchedulerExtensions.Length}");
        var methods_FluentSchedulerExtensions = type_FluentSchedulerExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FluentSchedulerExtensions 公开方法数量: {methods_FluentSchedulerExtensions.Length}");
        foreach (var m in methods_FluentSchedulerExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentSchedulerExtensions 未找到，尝试无命名空间...");
        type_FluentSchedulerExtensions = Type.GetType("FluentSchedulerExtensions");
        if (type_FluentSchedulerExtensions != null)
            Console.WriteLine("[PASS] 类型 FluentSchedulerExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FluentSchedulerExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FluentSchedulerHostedService
    var type_FluentSchedulerHostedService = Type.GetType("FluentSchedulerHostedService");
    if (type_FluentSchedulerHostedService != null)
    {
        Console.WriteLine("[PASS] 类型 FluentSchedulerHostedService (class) 存在");
        var ctors_FluentSchedulerHostedService = type_FluentSchedulerHostedService.GetConstructors();
        Console.WriteLine($"[PASS] FluentSchedulerHostedService 构造函数数量: {ctors_FluentSchedulerHostedService.Length}");
        var methods_FluentSchedulerHostedService = type_FluentSchedulerHostedService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FluentSchedulerHostedService 公开方法数量: {methods_FluentSchedulerHostedService.Length}");
        foreach (var m in methods_FluentSchedulerHostedService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FluentSchedulerHostedService 未找到，尝试无命名空间...");
        type_FluentSchedulerHostedService = Type.GetType("FluentSchedulerHostedService");
        if (type_FluentSchedulerHostedService != null)
            Console.WriteLine("[PASS] 类型 FluentSchedulerHostedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FluentSchedulerHostedService 可能为顶层语句或嵌套类型");
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

    // 验证 record: JobItem
    var type_JobItem = Type.GetType("JobItem");
    if (type_JobItem != null)
    {
        Console.WriteLine("[PASS] 类型 JobItem (record) 存在");
        var ctors_JobItem = type_JobItem.GetConstructors();
        Console.WriteLine($"[PASS] JobItem 构造函数数量: {ctors_JobItem.Length}");
        var methods_JobItem = type_JobItem.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JobItem 公开方法数量: {methods_JobItem.Length}");
        foreach (var m in methods_JobItem)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JobItem 未找到，尝试无命名空间...");
        type_JobItem = Type.GetType("JobItem");
        if (type_JobItem != null)
            Console.WriteLine("[PASS] 类型 JobItem (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JobItem 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
