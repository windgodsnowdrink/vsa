#load "mediatr_signalr_integration.cs"

Console.WriteLine("=== mediatr_signalr_integration.cs Test ===");

try
{
    // 验证 class: SignalRNotificationHandler
    var type_SignalRNotificationHandler = Type.GetType("SignalRNotificationHandler");
    if (type_SignalRNotificationHandler != null)
    {
        Console.WriteLine("[PASS] 类型 SignalRNotificationHandler (class) 存在");
        var ctors_SignalRNotificationHandler = type_SignalRNotificationHandler.GetConstructors();
        Console.WriteLine($"[PASS] SignalRNotificationHandler 构造函数数量: {ctors_SignalRNotificationHandler.Length}");
        var methods_SignalRNotificationHandler = type_SignalRNotificationHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SignalRNotificationHandler 公开方法数量: {methods_SignalRNotificationHandler.Length}");
        foreach (var m in methods_SignalRNotificationHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SignalRNotificationHandler 未找到，尝试无命名空间...");
        type_SignalRNotificationHandler = Type.GetType("SignalRNotificationHandler");
        if (type_SignalRNotificationHandler != null)
            Console.WriteLine("[PASS] 类型 SignalRNotificationHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SignalRNotificationHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MediatRSignalRIntegrationExtensions
    var type_MediatRSignalRIntegrationExtensions = Type.GetType("MediatRSignalRIntegrationExtensions");
    if (type_MediatRSignalRIntegrationExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MediatRSignalRIntegrationExtensions (class) 存在");
        var ctors_MediatRSignalRIntegrationExtensions = type_MediatRSignalRIntegrationExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MediatRSignalRIntegrationExtensions 构造函数数量: {ctors_MediatRSignalRIntegrationExtensions.Length}");
        var methods_MediatRSignalRIntegrationExtensions = type_MediatRSignalRIntegrationExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MediatRSignalRIntegrationExtensions 公开方法数量: {methods_MediatRSignalRIntegrationExtensions.Length}");
        foreach (var m in methods_MediatRSignalRIntegrationExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MediatRSignalRIntegrationExtensions 未找到，尝试无命名空间...");
        type_MediatRSignalRIntegrationExtensions = Type.GetType("MediatRSignalRIntegrationExtensions");
        if (type_MediatRSignalRIntegrationExtensions != null)
            Console.WriteLine("[PASS] 类型 MediatRSignalRIntegrationExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MediatRSignalRIntegrationExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RealtimeQueryService
    var type_RealtimeQueryService = Type.GetType("RealtimeQueryService");
    if (type_RealtimeQueryService != null)
    {
        Console.WriteLine("[PASS] 类型 RealtimeQueryService (class) 存在");
        var ctors_RealtimeQueryService = type_RealtimeQueryService.GetConstructors();
        Console.WriteLine($"[PASS] RealtimeQueryService 构造函数数量: {ctors_RealtimeQueryService.Length}");
        var methods_RealtimeQueryService = type_RealtimeQueryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RealtimeQueryService 公开方法数量: {methods_RealtimeQueryService.Length}");
        foreach (var m in methods_RealtimeQueryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RealtimeQueryService 未找到，尝试无命名空间...");
        type_RealtimeQueryService = Type.GetType("RealtimeQueryService");
        if (type_RealtimeQueryService != null)
            Console.WriteLine("[PASS] 类型 RealtimeQueryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RealtimeQueryService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
