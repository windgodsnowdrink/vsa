#load "signalr_advanced_monitoring.cs"

Console.WriteLine("=== signalr_advanced_monitoring.cs Test ===");

try
{
    // 验证 class: MonitoringDbContext
    var type_MonitoringDbContext = Type.GetType("MonitoringDbContext");
    if (type_MonitoringDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 MonitoringDbContext (class) 存在");
        var ctors_MonitoringDbContext = type_MonitoringDbContext.GetConstructors();
        Console.WriteLine($"[PASS] MonitoringDbContext 构造函数数量: {ctors_MonitoringDbContext.Length}");
        var methods_MonitoringDbContext = type_MonitoringDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MonitoringDbContext 公开方法数量: {methods_MonitoringDbContext.Length}");
        foreach (var m in methods_MonitoringDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MonitoringDbContext 未找到，尝试无命名空间...");
        type_MonitoringDbContext = Type.GetType("MonitoringDbContext");
        if (type_MonitoringDbContext != null)
            Console.WriteLine("[PASS] 类型 MonitoringDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MonitoringDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GitBasedVersioning
    var type_GitBasedVersioning = Type.GetType("GitBasedVersioning");
    if (type_GitBasedVersioning != null)
    {
        Console.WriteLine("[PASS] 类型 GitBasedVersioning (class) 存在");
        var ctors_GitBasedVersioning = type_GitBasedVersioning.GetConstructors();
        Console.WriteLine($"[PASS] GitBasedVersioning 构造函数数量: {ctors_GitBasedVersioning.Length}");
        var methods_GitBasedVersioning = type_GitBasedVersioning.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GitBasedVersioning 公开方法数量: {methods_GitBasedVersioning.Length}");
        foreach (var m in methods_GitBasedVersioning)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GitBasedVersioning 未找到，尝试无命名空间...");
        type_GitBasedVersioning = Type.GetType("GitBasedVersioning");
        if (type_GitBasedVersioning != null)
            Console.WriteLine("[PASS] 类型 GitBasedVersioning (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GitBasedVersioning 可能为顶层语句或嵌套类型");
    }

    // 验证 record: MonitoringRecord
    var type_MonitoringRecord = Type.GetType("MonitoringRecord");
    if (type_MonitoringRecord != null)
    {
        Console.WriteLine("[PASS] 类型 MonitoringRecord (record) 存在");
        var ctors_MonitoringRecord = type_MonitoringRecord.GetConstructors();
        Console.WriteLine($"[PASS] MonitoringRecord 构造函数数量: {ctors_MonitoringRecord.Length}");
        var methods_MonitoringRecord = type_MonitoringRecord.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MonitoringRecord 公开方法数量: {methods_MonitoringRecord.Length}");
        foreach (var m in methods_MonitoringRecord)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MonitoringRecord 未找到，尝试无命名空间...");
        type_MonitoringRecord = Type.GetType("MonitoringRecord");
        if (type_MonitoringRecord != null)
            Console.WriteLine("[PASS] 类型 MonitoringRecord (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MonitoringRecord 可能为顶层语句或嵌套类型");
    }

    // 验证 record: EventTrace
    var type_EventTrace = Type.GetType("EventTrace");
    if (type_EventTrace != null)
    {
        Console.WriteLine("[PASS] 类型 EventTrace (record) 存在");
        var ctors_EventTrace = type_EventTrace.GetConstructors();
        Console.WriteLine($"[PASS] EventTrace 构造函数数量: {ctors_EventTrace.Length}");
        var methods_EventTrace = type_EventTrace.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventTrace 公开方法数量: {methods_EventTrace.Length}");
        foreach (var m in methods_EventTrace)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventTrace 未找到，尝试无命名空间...");
        type_EventTrace = Type.GetType("EventTrace");
        if (type_EventTrace != null)
            Console.WriteLine("[PASS] 类型 EventTrace (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventTrace 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
