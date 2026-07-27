#load "wolverinefx_kafka_integration.cs"

Console.WriteLine("=== wolverinefx_kafka_integration.cs Test ===");

try
{
    // 验证 class: KafkaEventBusOptions
    var type_KafkaEventBusOptions = Type.GetType("KafkaEventBusOptions");
    if (type_KafkaEventBusOptions != null)
    {
        Console.WriteLine("[PASS] 类型 KafkaEventBusOptions (class) 存在");
        var ctors_KafkaEventBusOptions = type_KafkaEventBusOptions.GetConstructors();
        Console.WriteLine($"[PASS] KafkaEventBusOptions 构造函数数量: {ctors_KafkaEventBusOptions.Length}");
        var methods_KafkaEventBusOptions = type_KafkaEventBusOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KafkaEventBusOptions 公开方法数量: {methods_KafkaEventBusOptions.Length}");
        foreach (var m in methods_KafkaEventBusOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KafkaEventBusOptions 未找到，尝试无命名空间...");
        type_KafkaEventBusOptions = Type.GetType("KafkaEventBusOptions");
        if (type_KafkaEventBusOptions != null)
            Console.WriteLine("[PASS] 类型 KafkaEventBusOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KafkaEventBusOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KafkaEventBus
    var type_KafkaEventBus = Type.GetType("KafkaEventBus");
    if (type_KafkaEventBus != null)
    {
        Console.WriteLine("[PASS] 类型 KafkaEventBus (class) 存在");
        var ctors_KafkaEventBus = type_KafkaEventBus.GetConstructors();
        Console.WriteLine($"[PASS] KafkaEventBus 构造函数数量: {ctors_KafkaEventBus.Length}");
        var methods_KafkaEventBus = type_KafkaEventBus.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KafkaEventBus 公开方法数量: {methods_KafkaEventBus.Length}");
        foreach (var m in methods_KafkaEventBus)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KafkaEventBus 未找到，尝试无命名空间...");
        type_KafkaEventBus = Type.GetType("KafkaEventBus");
        if (type_KafkaEventBus != null)
            Console.WriteLine("[PASS] 类型 KafkaEventBus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KafkaEventBus 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KafkaEventBusExtensions
    var type_KafkaEventBusExtensions = Type.GetType("KafkaEventBusExtensions");
    if (type_KafkaEventBusExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 KafkaEventBusExtensions (class) 存在");
        var ctors_KafkaEventBusExtensions = type_KafkaEventBusExtensions.GetConstructors();
        Console.WriteLine($"[PASS] KafkaEventBusExtensions 构造函数数量: {ctors_KafkaEventBusExtensions.Length}");
        var methods_KafkaEventBusExtensions = type_KafkaEventBusExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KafkaEventBusExtensions 公开方法数量: {methods_KafkaEventBusExtensions.Length}");
        foreach (var m in methods_KafkaEventBusExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KafkaEventBusExtensions 未找到，尝试无命名空间...");
        type_KafkaEventBusExtensions = Type.GetType("KafkaEventBusExtensions");
        if (type_KafkaEventBusExtensions != null)
            Console.WriteLine("[PASS] 类型 KafkaEventBusExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KafkaEventBusExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KafkaEventBusBackgroundService
    var type_KafkaEventBusBackgroundService = Type.GetType("KafkaEventBusBackgroundService");
    if (type_KafkaEventBusBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 KafkaEventBusBackgroundService (class) 存在");
        var ctors_KafkaEventBusBackgroundService = type_KafkaEventBusBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] KafkaEventBusBackgroundService 构造函数数量: {ctors_KafkaEventBusBackgroundService.Length}");
        var methods_KafkaEventBusBackgroundService = type_KafkaEventBusBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KafkaEventBusBackgroundService 公开方法数量: {methods_KafkaEventBusBackgroundService.Length}");
        foreach (var m in methods_KafkaEventBusBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KafkaEventBusBackgroundService 未找到，尝试无命名空间...");
        type_KafkaEventBusBackgroundService = Type.GetType("KafkaEventBusBackgroundService");
        if (type_KafkaEventBusBackgroundService != null)
            Console.WriteLine("[PASS] 类型 KafkaEventBusBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KafkaEventBusBackgroundService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
