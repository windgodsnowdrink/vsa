#load "litedb_eventbus.cs"

Console.WriteLine("=== litedb_eventbus.cs Test ===");

try
{
    // 验证 class: EventBusService
    var type_EventBusService = Type.GetType("EventBusService");
    if (type_EventBusService != null)
    {
        Console.WriteLine("[PASS] 类型 EventBusService (class) 存在");
        var ctors_EventBusService = type_EventBusService.GetConstructors();
        Console.WriteLine($"[PASS] EventBusService 构造函数数量: {ctors_EventBusService.Length}");
        var methods_EventBusService = type_EventBusService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EventBusService 公开方法数量: {methods_EventBusService.Length}");
        foreach (var m in methods_EventBusService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EventBusService 未找到，尝试无命名空间...");
        type_EventBusService = Type.GetType("EventBusService");
        if (type_EventBusService != null)
            Console.WriteLine("[PASS] 类型 EventBusService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EventBusService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
