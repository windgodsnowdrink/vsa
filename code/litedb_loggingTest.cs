#load "litedb_logging.cs"

Console.WriteLine("=== litedb_logging.cs Test ===");

try
{
    // 验证 class: LogEntry
    var type_LogEntry = Type.GetType("LogEntry");
    if (type_LogEntry != null)
    {
        Console.WriteLine("[PASS] 类型 LogEntry (class) 存在");
        var ctors_LogEntry = type_LogEntry.GetConstructors();
        Console.WriteLine($"[PASS] LogEntry 构造函数数量: {ctors_LogEntry.Length}");
        var methods_LogEntry = type_LogEntry.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogEntry 公开方法数量: {methods_LogEntry.Length}");
        foreach (var m in methods_LogEntry)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogEntry 未找到，尝试无命名空间...");
        type_LogEntry = Type.GetType("LogEntry");
        if (type_LogEntry != null)
            Console.WriteLine("[PASS] 类型 LogEntry (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogEntry 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LiteDbLogService
    var type_LiteDbLogService = Type.GetType("LiteDbLogService");
    if (type_LiteDbLogService != null)
    {
        Console.WriteLine("[PASS] 类型 LiteDbLogService (class) 存在");
        var ctors_LiteDbLogService = type_LiteDbLogService.GetConstructors();
        Console.WriteLine($"[PASS] LiteDbLogService 构造函数数量: {ctors_LiteDbLogService.Length}");
        var methods_LiteDbLogService = type_LiteDbLogService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiteDbLogService 公开方法数量: {methods_LiteDbLogService.Length}");
        foreach (var m in methods_LiteDbLogService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiteDbLogService 未找到，尝试无命名空间...");
        type_LiteDbLogService = Type.GetType("LiteDbLogService");
        if (type_LiteDbLogService != null)
            Console.WriteLine("[PASS] 类型 LiteDbLogService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiteDbLogService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LiteDbPoolPolicy
    var type_LiteDbPoolPolicy = Type.GetType("LiteDbPoolPolicy");
    if (type_LiteDbPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 LiteDbPoolPolicy (class) 存在");
        var ctors_LiteDbPoolPolicy = type_LiteDbPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] LiteDbPoolPolicy 构造函数数量: {ctors_LiteDbPoolPolicy.Length}");
        var methods_LiteDbPoolPolicy = type_LiteDbPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiteDbPoolPolicy 公开方法数量: {methods_LiteDbPoolPolicy.Length}");
        foreach (var m in methods_LiteDbPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiteDbPoolPolicy 未找到，尝试无命名空间...");
        type_LiteDbPoolPolicy = Type.GetType("LiteDbPoolPolicy");
        if (type_LiteDbPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 LiteDbPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiteDbPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LogQueryService
    var type_LogQueryService = Type.GetType("LogQueryService");
    if (type_LogQueryService != null)
    {
        Console.WriteLine("[PASS] 类型 LogQueryService (class) 存在");
        var ctors_LogQueryService = type_LogQueryService.GetConstructors();
        Console.WriteLine($"[PASS] LogQueryService 构造函数数量: {ctors_LogQueryService.Length}");
        var methods_LogQueryService = type_LogQueryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogQueryService 公开方法数量: {methods_LogQueryService.Length}");
        foreach (var m in methods_LogQueryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogQueryService 未找到，尝试无命名空间...");
        type_LogQueryService = Type.GetType("LogQueryService");
        if (type_LogQueryService != null)
            Console.WriteLine("[PASS] 类型 LogQueryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogQueryService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LiteDbLogDemo
    var type_LiteDbLogDemo = Type.GetType("LiteDbLogDemo");
    if (type_LiteDbLogDemo != null)
    {
        Console.WriteLine("[PASS] 类型 LiteDbLogDemo (class) 存在");
        var ctors_LiteDbLogDemo = type_LiteDbLogDemo.GetConstructors();
        Console.WriteLine($"[PASS] LiteDbLogDemo 构造函数数量: {ctors_LiteDbLogDemo.Length}");
        var methods_LiteDbLogDemo = type_LiteDbLogDemo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LiteDbLogDemo 公开方法数量: {methods_LiteDbLogDemo.Length}");
        foreach (var m in methods_LiteDbLogDemo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LiteDbLogDemo 未找到，尝试无命名空间...");
        type_LiteDbLogDemo = Type.GetType("LiteDbLogDemo");
        if (type_LiteDbLogDemo != null)
            Console.WriteLine("[PASS] 类型 LiteDbLogDemo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LiteDbLogDemo 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
