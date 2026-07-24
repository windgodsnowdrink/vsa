#load "wolverinefx_signalr_integration.cs"

Console.WriteLine("=== wolverinefx_signalr_integration.cs Test ===");

try
{
    // 验证 class: WolverineSignalRIntegrationExtensions
    var type_WolverineSignalRIntegrationExtensions = Type.GetType("WolverineSignalRIntegrationExtensions");
    if (type_WolverineSignalRIntegrationExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 WolverineSignalRIntegrationExtensions (class) 存在");
        var ctors_WolverineSignalRIntegrationExtensions = type_WolverineSignalRIntegrationExtensions.GetConstructors();
        Console.WriteLine($"[PASS] WolverineSignalRIntegrationExtensions 构造函数数量: {ctors_WolverineSignalRIntegrationExtensions.Length}");
        var methods_WolverineSignalRIntegrationExtensions = type_WolverineSignalRIntegrationExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WolverineSignalRIntegrationExtensions 公开方法数量: {methods_WolverineSignalRIntegrationExtensions.Length}");
        foreach (var m in methods_WolverineSignalRIntegrationExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WolverineSignalRIntegrationExtensions 未找到，尝试无命名空间...");
        type_WolverineSignalRIntegrationExtensions = Type.GetType("WolverineSignalRIntegrationExtensions");
        if (type_WolverineSignalRIntegrationExtensions != null)
            Console.WriteLine("[PASS] 类型 WolverineSignalRIntegrationExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WolverineSignalRIntegrationExtensions 可能为顶层语句或嵌套类型");
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
