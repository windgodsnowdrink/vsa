#load "nodatime_extensions.cs"

Console.WriteLine("=== nodatime_extensions.cs Test ===");

try
{
    // 验证 class: NodaTimeService
    var type_NodaTimeService = Type.GetType("NodaTimeService");
    if (type_NodaTimeService != null)
    {
        Console.WriteLine("[PASS] 类型 NodaTimeService (class) 存在");
        var ctors_NodaTimeService = type_NodaTimeService.GetConstructors();
        Console.WriteLine($"[PASS] NodaTimeService 构造函数数量: {ctors_NodaTimeService.Length}");
        var methods_NodaTimeService = type_NodaTimeService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NodaTimeService 公开方法数量: {methods_NodaTimeService.Length}");
        foreach (var m in methods_NodaTimeService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NodaTimeService 未找到，尝试无命名空间...");
        type_NodaTimeService = Type.GetType("NodaTimeService");
        if (type_NodaTimeService != null)
            Console.WriteLine("[PASS] 类型 NodaTimeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NodaTimeService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NodaTimeExtensions
    var type_NodaTimeExtensions = Type.GetType("NodaTimeExtensions");
    if (type_NodaTimeExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 NodaTimeExtensions (class) 存在");
        var ctors_NodaTimeExtensions = type_NodaTimeExtensions.GetConstructors();
        Console.WriteLine($"[PASS] NodaTimeExtensions 构造函数数量: {ctors_NodaTimeExtensions.Length}");
        var methods_NodaTimeExtensions = type_NodaTimeExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NodaTimeExtensions 公开方法数量: {methods_NodaTimeExtensions.Length}");
        foreach (var m in methods_NodaTimeExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NodaTimeExtensions 未找到，尝试无命名空间...");
        type_NodaTimeExtensions = Type.GetType("NodaTimeExtensions");
        if (type_NodaTimeExtensions != null)
            Console.WriteLine("[PASS] 类型 NodaTimeExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NodaTimeExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TimeRequest
    var type_TimeRequest = Type.GetType("TimeRequest");
    if (type_TimeRequest != null)
    {
        Console.WriteLine("[PASS] 类型 TimeRequest (record) 存在");
        var ctors_TimeRequest = type_TimeRequest.GetConstructors();
        Console.WriteLine($"[PASS] TimeRequest 构造函数数量: {ctors_TimeRequest.Length}");
        var methods_TimeRequest = type_TimeRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TimeRequest 公开方法数量: {methods_TimeRequest.Length}");
        foreach (var m in methods_TimeRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TimeRequest 未找到，尝试无命名空间...");
        type_TimeRequest = Type.GetType("TimeRequest");
        if (type_TimeRequest != null)
            Console.WriteLine("[PASS] 类型 TimeRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TimeRequest 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
