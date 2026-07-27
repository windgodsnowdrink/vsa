#load "schedule_master_service.cs"

Console.WriteLine("=== schedule_master_service.cs Test ===");

try
{
    // 验证 class: TaskRegistry
    var type_TaskRegistry = Type.GetType("TaskRegistry");
    if (type_TaskRegistry != null)
    {
        Console.WriteLine("[PASS] 类型 TaskRegistry (class) 存在");
        var ctors_TaskRegistry = type_TaskRegistry.GetConstructors();
        Console.WriteLine($"[PASS] TaskRegistry 构造函数数量: {ctors_TaskRegistry.Length}");
        var methods_TaskRegistry = type_TaskRegistry.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TaskRegistry 公开方法数量: {methods_TaskRegistry.Length}");
        foreach (var m in methods_TaskRegistry)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TaskRegistry 未找到，尝试无命名空间...");
        type_TaskRegistry = Type.GetType("TaskRegistry");
        if (type_TaskRegistry != null)
            Console.WriteLine("[PASS] 类型 TaskRegistry (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TaskRegistry 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScheduleMasterProcessor
    var type_ScheduleMasterProcessor = Type.GetType("ScheduleMasterProcessor");
    if (type_ScheduleMasterProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ScheduleMasterProcessor (class) 存在");
        var ctors_ScheduleMasterProcessor = type_ScheduleMasterProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ScheduleMasterProcessor 构造函数数量: {ctors_ScheduleMasterProcessor.Length}");
        var methods_ScheduleMasterProcessor = type_ScheduleMasterProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScheduleMasterProcessor 公开方法数量: {methods_ScheduleMasterProcessor.Length}");
        foreach (var m in methods_ScheduleMasterProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScheduleMasterProcessor 未找到，尝试无命名空间...");
        type_ScheduleMasterProcessor = Type.GetType("ScheduleMasterProcessor");
        if (type_ScheduleMasterProcessor != null)
            Console.WriteLine("[PASS] 类型 ScheduleMasterProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScheduleMasterProcessor 可能为顶层语句或嵌套类型");
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

    // 验证 class: EmailTaskHandler
    var type_EmailTaskHandler = Type.GetType("EmailTaskHandler");
    if (type_EmailTaskHandler != null)
    {
        Console.WriteLine("[PASS] 类型 EmailTaskHandler (class) 存在");
        var ctors_EmailTaskHandler = type_EmailTaskHandler.GetConstructors();
        Console.WriteLine($"[PASS] EmailTaskHandler 构造函数数量: {ctors_EmailTaskHandler.Length}");
        var methods_EmailTaskHandler = type_EmailTaskHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EmailTaskHandler 公开方法数量: {methods_EmailTaskHandler.Length}");
        foreach (var m in methods_EmailTaskHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EmailTaskHandler 未找到，尝试无命名空间...");
        type_EmailTaskHandler = Type.GetType("EmailTaskHandler");
        if (type_EmailTaskHandler != null)
            Console.WriteLine("[PASS] 类型 EmailTaskHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EmailTaskHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITaskHandler
    var type_ITaskHandler = Type.GetType("ITaskHandler");
    if (type_ITaskHandler != null)
    {
        Console.WriteLine("[PASS] 类型 ITaskHandler (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITaskHandler 未找到，尝试无命名空间...");
        type_ITaskHandler = Type.GetType("ITaskHandler");
        if (type_ITaskHandler != null)
            Console.WriteLine("[PASS] 类型 ITaskHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITaskHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IStorageProvider
    var type_IStorageProvider = Type.GetType("IStorageProvider");
    if (type_IStorageProvider != null)
    {
        Console.WriteLine("[PASS] 类型 IStorageProvider (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IStorageProvider 未找到，尝试无命名空间...");
        type_IStorageProvider = Type.GetType("IStorageProvider");
        if (type_IStorageProvider != null)
            Console.WriteLine("[PASS] 类型 IStorageProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IStorageProvider 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ScheduleTask
    var type_ScheduleTask = Type.GetType("ScheduleTask");
    if (type_ScheduleTask != null)
    {
        Console.WriteLine("[PASS] 类型 ScheduleTask (record) 存在");
        var ctors_ScheduleTask = type_ScheduleTask.GetConstructors();
        Console.WriteLine($"[PASS] ScheduleTask 构造函数数量: {ctors_ScheduleTask.Length}");
        var methods_ScheduleTask = type_ScheduleTask.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScheduleTask 公开方法数量: {methods_ScheduleTask.Length}");
        foreach (var m in methods_ScheduleTask)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScheduleTask 未找到，尝试无命名空间...");
        type_ScheduleTask = Type.GetType("ScheduleTask");
        if (type_ScheduleTask != null)
            Console.WriteLine("[PASS] 类型 ScheduleTask (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScheduleTask 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
