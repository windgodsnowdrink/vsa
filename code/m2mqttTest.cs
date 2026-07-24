#load "m2mqtt.cs"

Console.WriteLine("=== m2mqtt.cs Test ===");

try
{
    // 验证 class: MqttMessageProcessor
    var type_MqttMessageProcessor = Type.GetType("MqttMessageProcessor");
    if (type_MqttMessageProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessageProcessor (class) 存在");
        var ctors_MqttMessageProcessor = type_MqttMessageProcessor.GetConstructors();
        Console.WriteLine($"[PASS] MqttMessageProcessor 构造函数数量: {ctors_MqttMessageProcessor.Length}");
        var methods_MqttMessageProcessor = type_MqttMessageProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttMessageProcessor 公开方法数量: {methods_MqttMessageProcessor.Length}");
        foreach (var m in methods_MqttMessageProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttMessageProcessor 未找到，尝试无命名空间...");
        type_MqttMessageProcessor = Type.GetType("MqttMessageProcessor");
        if (type_MqttMessageProcessor != null)
            Console.WriteLine("[PASS] 类型 MqttMessageProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttMessageProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttClientService
    var type_MqttClientService = Type.GetType("MqttClientService");
    if (type_MqttClientService != null)
    {
        Console.WriteLine("[PASS] 类型 MqttClientService (class) 存在");
        var ctors_MqttClientService = type_MqttClientService.GetConstructors();
        Console.WriteLine($"[PASS] MqttClientService 构造函数数量: {ctors_MqttClientService.Length}");
        var methods_MqttClientService = type_MqttClientService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttClientService 公开方法数量: {methods_MqttClientService.Length}");
        foreach (var m in methods_MqttClientService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttClientService 未找到，尝试无命名空间...");
        type_MqttClientService = Type.GetType("MqttClientService");
        if (type_MqttClientService != null)
            Console.WriteLine("[PASS] 类型 MqttClientService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttClientService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TailLatencyOptimizer
    var type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
    if (type_TailLatencyOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (class) 存在");
        var ctors_TailLatencyOptimizer = type_TailLatencyOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] TailLatencyOptimizer 构造函数数量: {ctors_TailLatencyOptimizer.Length}");
        var methods_TailLatencyOptimizer = type_TailLatencyOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TailLatencyOptimizer 公开方法数量: {methods_TailLatencyOptimizer.Length}");
        foreach (var m in methods_TailLatencyOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TailLatencyOptimizer 未找到，尝试无命名空间...");
        type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
        if (type_TailLatencyOptimizer != null)
            Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TailLatencyOptimizer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
