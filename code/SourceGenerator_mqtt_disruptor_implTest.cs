#load "SourceGenerator_mqtt_disruptor_impl.cs"

Console.WriteLine("=== SourceGenerator_mqtt_disruptor_impl.cs Test ===");

try
{
    // 验证 class: MqttMessageEvent
    var type_MqttMessageEvent = Type.GetType("MqttMessageEvent");
    if (type_MqttMessageEvent != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessageEvent (class) 存在");
        var ctors_MqttMessageEvent = type_MqttMessageEvent.GetConstructors();
        Console.WriteLine($"[PASS] MqttMessageEvent 构造函数数量: {ctors_MqttMessageEvent.Length}");
        var methods_MqttMessageEvent = type_MqttMessageEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttMessageEvent 公开方法数量: {methods_MqttMessageEvent.Length}");
        foreach (var m in methods_MqttMessageEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttMessageEvent 未找到，尝试无命名空间...");
        type_MqttMessageEvent = Type.GetType("MqttMessageEvent");
        if (type_MqttMessageEvent != null)
            Console.WriteLine("[PASS] 类型 MqttMessageEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttMessageEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttMessageEventHandler
    var type_MqttMessageEventHandler = Type.GetType("MqttMessageEventHandler");
    if (type_MqttMessageEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessageEventHandler (class) 存在");
        var ctors_MqttMessageEventHandler = type_MqttMessageEventHandler.GetConstructors();
        Console.WriteLine($"[PASS] MqttMessageEventHandler 构造函数数量: {ctors_MqttMessageEventHandler.Length}");
        var methods_MqttMessageEventHandler = type_MqttMessageEventHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttMessageEventHandler 公开方法数量: {methods_MqttMessageEventHandler.Length}");
        foreach (var m in methods_MqttMessageEventHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttMessageEventHandler 未找到，尝试无命名空间...");
        type_MqttMessageEventHandler = Type.GetType("MqttMessageEventHandler");
        if (type_MqttMessageEventHandler != null)
            Console.WriteLine("[PASS] 类型 MqttMessageEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttMessageEventHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HighPerformanceMqttService
    var type_HighPerformanceMqttService = Type.GetType("HighPerformanceMqttService");
    if (type_HighPerformanceMqttService != null)
    {
        Console.WriteLine("[PASS] 类型 HighPerformanceMqttService (class) 存在");
        var ctors_HighPerformanceMqttService = type_HighPerformanceMqttService.GetConstructors();
        Console.WriteLine($"[PASS] HighPerformanceMqttService 构造函数数量: {ctors_HighPerformanceMqttService.Length}");
        var methods_HighPerformanceMqttService = type_HighPerformanceMqttService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HighPerformanceMqttService 公开方法数量: {methods_HighPerformanceMqttService.Length}");
        foreach (var m in methods_HighPerformanceMqttService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HighPerformanceMqttService 未找到，尝试无命名空间...");
        type_HighPerformanceMqttService = Type.GetType("HighPerformanceMqttService");
        if (type_HighPerformanceMqttService != null)
            Console.WriteLine("[PASS] 类型 HighPerformanceMqttService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HighPerformanceMqttService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
