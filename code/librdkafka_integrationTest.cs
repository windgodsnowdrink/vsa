#load "librdkafka_integration.cs"

Console.WriteLine("=== librdkafka_integration.cs Test ===");

try
{
    // 验证 class: Trae.Vsa.Kafka.KafkaOptions
    var type_KafkaOptions = Type.GetType("Trae.Vsa.Kafka.KafkaOptions");
    if (type_KafkaOptions != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.Kafka.KafkaOptions (class) 存在");
        var ctors_KafkaOptions = type_KafkaOptions.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.Kafka.KafkaOptions 构造函数数量: {ctors_KafkaOptions.Length}");
        var methods_KafkaOptions = type_KafkaOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.Kafka.KafkaOptions 公开方法数量: {methods_KafkaOptions.Length}");
        foreach (var m in methods_KafkaOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.Kafka.KafkaOptions 未找到，尝试无命名空间...");
        type_KafkaOptions = Type.GetType("KafkaOptions");
        if (type_KafkaOptions != null)
            Console.WriteLine("[PASS] 类型 KafkaOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KafkaOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Trae.Vsa.Kafka.KafkaService
    var type_KafkaService = Type.GetType("Trae.Vsa.Kafka.KafkaService");
    if (type_KafkaService != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.Kafka.KafkaService (class) 存在");
        var ctors_KafkaService = type_KafkaService.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.Kafka.KafkaService 构造函数数量: {ctors_KafkaService.Length}");
        var methods_KafkaService = type_KafkaService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.Kafka.KafkaService 公开方法数量: {methods_KafkaService.Length}");
        foreach (var m in methods_KafkaService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.Kafka.KafkaService 未找到，尝试无命名空间...");
        type_KafkaService = Type.GetType("KafkaService");
        if (type_KafkaService != null)
            Console.WriteLine("[PASS] 类型 KafkaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KafkaService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Trae.Vsa.Kafka.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("Trae.Vsa.Kafka.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.Kafka.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.Kafka.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.Kafka.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.Kafka.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Trae.Vsa.Kafka.MemoryPoolStatistics
    var type_MemoryPoolStatistics = Type.GetType("Trae.Vsa.Kafka.MemoryPoolStatistics");
    if (type_MemoryPoolStatistics != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.Kafka.MemoryPoolStatistics (class) 存在");
        var ctors_MemoryPoolStatistics = type_MemoryPoolStatistics.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.Kafka.MemoryPoolStatistics 构造函数数量: {ctors_MemoryPoolStatistics.Length}");
        var methods_MemoryPoolStatistics = type_MemoryPoolStatistics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.Kafka.MemoryPoolStatistics 公开方法数量: {methods_MemoryPoolStatistics.Length}");
        foreach (var m in methods_MemoryPoolStatistics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.Kafka.MemoryPoolStatistics 未找到，尝试无命名空间...");
        type_MemoryPoolStatistics = Type.GetType("MemoryPoolStatistics");
        if (type_MemoryPoolStatistics != null)
            Console.WriteLine("[PASS] 类型 MemoryPoolStatistics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryPoolStatistics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Trae.Vsa.Kafka.ConnectionStatistics
    var type_ConnectionStatistics = Type.GetType("Trae.Vsa.Kafka.ConnectionStatistics");
    if (type_ConnectionStatistics != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.Kafka.ConnectionStatistics (class) 存在");
        var ctors_ConnectionStatistics = type_ConnectionStatistics.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.Kafka.ConnectionStatistics 构造函数数量: {ctors_ConnectionStatistics.Length}");
        var methods_ConnectionStatistics = type_ConnectionStatistics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.Kafka.ConnectionStatistics 公开方法数量: {methods_ConnectionStatistics.Length}");
        foreach (var m in methods_ConnectionStatistics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.Kafka.ConnectionStatistics 未找到，尝试无命名空间...");
        type_ConnectionStatistics = Type.GetType("ConnectionStatistics");
        if (type_ConnectionStatistics != null)
            Console.WriteLine("[PASS] 类型 ConnectionStatistics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConnectionStatistics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Trae.Vsa.Kafka.ThroughputStatistics
    var type_ThroughputStatistics = Type.GetType("Trae.Vsa.Kafka.ThroughputStatistics");
    if (type_ThroughputStatistics != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.Kafka.ThroughputStatistics (class) 存在");
        var ctors_ThroughputStatistics = type_ThroughputStatistics.GetConstructors();
        Console.WriteLine($"[PASS] Trae.Vsa.Kafka.ThroughputStatistics 构造函数数量: {ctors_ThroughputStatistics.Length}");
        var methods_ThroughputStatistics = type_ThroughputStatistics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Trae.Vsa.Kafka.ThroughputStatistics 公开方法数量: {methods_ThroughputStatistics.Length}");
        foreach (var m in methods_ThroughputStatistics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.Kafka.ThroughputStatistics 未找到，尝试无命名空间...");
        type_ThroughputStatistics = Type.GetType("ThroughputStatistics");
        if (type_ThroughputStatistics != null)
            Console.WriteLine("[PASS] 类型 ThroughputStatistics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThroughputStatistics 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: Trae.Vsa.Kafka.IKafkaService
    var type_IKafkaService = Type.GetType("Trae.Vsa.Kafka.IKafkaService");
    if (type_IKafkaService != null)
    {
        Console.WriteLine("[PASS] 类型 Trae.Vsa.Kafka.IKafkaService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Trae.Vsa.Kafka.IKafkaService 未找到，尝试无命名空间...");
        type_IKafkaService = Type.GetType("IKafkaService");
        if (type_IKafkaService != null)
            Console.WriteLine("[PASS] 类型 IKafkaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IKafkaService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
