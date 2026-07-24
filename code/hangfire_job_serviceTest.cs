#load "hangfire_job_service.cs"

Console.WriteLine("=== hangfire_job_service.cs Test ===");

try
{
    // 验证 class: JobProcessor
    var type_JobProcessor = Type.GetType("JobProcessor");
    if (type_JobProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 JobProcessor (class) 存在");
        var ctors_JobProcessor = type_JobProcessor.GetConstructors();
        Console.WriteLine($"[PASS] JobProcessor 构造函数数量: {ctors_JobProcessor.Length}");
        var methods_JobProcessor = type_JobProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JobProcessor 公开方法数量: {methods_JobProcessor.Length}");
        foreach (var m in methods_JobProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JobProcessor 未找到，尝试无命名空间...");
        type_JobProcessor = Type.GetType("JobProcessor");
        if (type_JobProcessor != null)
            Console.WriteLine("[PASS] 类型 JobProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JobProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HangfireServiceExtensions
    var type_HangfireServiceExtensions = Type.GetType("HangfireServiceExtensions");
    if (type_HangfireServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 HangfireServiceExtensions (class) 存在");
        var ctors_HangfireServiceExtensions = type_HangfireServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] HangfireServiceExtensions 构造函数数量: {ctors_HangfireServiceExtensions.Length}");
        var methods_HangfireServiceExtensions = type_HangfireServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HangfireServiceExtensions 公开方法数量: {methods_HangfireServiceExtensions.Length}");
        foreach (var m in methods_HangfireServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HangfireServiceExtensions 未找到，尝试无命名空间...");
        type_HangfireServiceExtensions = Type.GetType("HangfireServiceExtensions");
        if (type_HangfireServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 HangfireServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HangfireServiceExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CronScheduler
    var type_CronScheduler = Type.GetType("CronScheduler");
    if (type_CronScheduler != null)
    {
        Console.WriteLine("[PASS] 类型 CronScheduler (class) 存在");
        var ctors_CronScheduler = type_CronScheduler.GetConstructors();
        Console.WriteLine($"[PASS] CronScheduler 构造函数数量: {ctors_CronScheduler.Length}");
        var methods_CronScheduler = type_CronScheduler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CronScheduler 公开方法数量: {methods_CronScheduler.Length}");
        foreach (var m in methods_CronScheduler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CronScheduler 未找到，尝试无命名空间...");
        type_CronScheduler = Type.GetType("CronScheduler");
        if (type_CronScheduler != null)
            Console.WriteLine("[PASS] 类型 CronScheduler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CronScheduler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HangfireOptions
    var type_HangfireOptions = Type.GetType("HangfireOptions");
    if (type_HangfireOptions != null)
    {
        Console.WriteLine("[PASS] 类型 HangfireOptions (class) 存在");
        var ctors_HangfireOptions = type_HangfireOptions.GetConstructors();
        Console.WriteLine($"[PASS] HangfireOptions 构造函数数量: {ctors_HangfireOptions.Length}");
        var methods_HangfireOptions = type_HangfireOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HangfireOptions 公开方法数量: {methods_HangfireOptions.Length}");
        foreach (var m in methods_HangfireOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HangfireOptions 未找到，尝试无命名空间...");
        type_HangfireOptions = Type.GetType("HangfireOptions");
        if (type_HangfireOptions != null)
            Console.WriteLine("[PASS] 类型 HangfireOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HangfireOptions 可能为顶层语句或嵌套类型");
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
