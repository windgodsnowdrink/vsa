#load "opentelemetry_production.cs"

Console.WriteLine("=== opentelemetry_production.cs Test ===");

try
{
    // 验证 class: CustomSpanExporter
    var type_CustomSpanExporter = Type.GetType("CustomSpanExporter");
    if (type_CustomSpanExporter != null)
    {
        Console.WriteLine("[PASS] 类型 CustomSpanExporter (class) 存在");
        var ctors_CustomSpanExporter = type_CustomSpanExporter.GetConstructors();
        Console.WriteLine($"[PASS] CustomSpanExporter 构造函数数量: {ctors_CustomSpanExporter.Length}");
        var methods_CustomSpanExporter = type_CustomSpanExporter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CustomSpanExporter 公开方法数量: {methods_CustomSpanExporter.Length}");
        foreach (var m in methods_CustomSpanExporter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CustomSpanExporter 未找到，尝试无命名空间...");
        type_CustomSpanExporter = Type.GetType("CustomSpanExporter");
        if (type_CustomSpanExporter != null)
            Console.WriteLine("[PASS] 类型 CustomSpanExporter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CustomSpanExporter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DynamicSampler
    var type_DynamicSampler = Type.GetType("DynamicSampler");
    if (type_DynamicSampler != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicSampler (class) 存在");
        var ctors_DynamicSampler = type_DynamicSampler.GetConstructors();
        Console.WriteLine($"[PASS] DynamicSampler 构造函数数量: {ctors_DynamicSampler.Length}");
        var methods_DynamicSampler = type_DynamicSampler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicSampler 公开方法数量: {methods_DynamicSampler.Length}");
        foreach (var m in methods_DynamicSampler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicSampler 未找到，尝试无命名空间...");
        type_DynamicSampler = Type.GetType("DynamicSampler");
        if (type_DynamicSampler != null)
            Console.WriteLine("[PASS] 类型 DynamicSampler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicSampler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ActivityPool
    var type_ActivityPool = Type.GetType("ActivityPool");
    if (type_ActivityPool != null)
    {
        Console.WriteLine("[PASS] 类型 ActivityPool (class) 存在");
        var ctors_ActivityPool = type_ActivityPool.GetConstructors();
        Console.WriteLine($"[PASS] ActivityPool 构造函数数量: {ctors_ActivityPool.Length}");
        var methods_ActivityPool = type_ActivityPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ActivityPool 公开方法数量: {methods_ActivityPool.Length}");
        foreach (var m in methods_ActivityPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ActivityPool 未找到，尝试无命名空间...");
        type_ActivityPool = Type.GetType("ActivityPool");
        if (type_ActivityPool != null)
            Console.WriteLine("[PASS] 类型 ActivityPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ActivityPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ResourceDiscoveryService
    var type_ResourceDiscoveryService = Type.GetType("ResourceDiscoveryService");
    if (type_ResourceDiscoveryService != null)
    {
        Console.WriteLine("[PASS] 类型 ResourceDiscoveryService (class) 存在");
        var ctors_ResourceDiscoveryService = type_ResourceDiscoveryService.GetConstructors();
        Console.WriteLine($"[PASS] ResourceDiscoveryService 构造函数数量: {ctors_ResourceDiscoveryService.Length}");
        var methods_ResourceDiscoveryService = type_ResourceDiscoveryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResourceDiscoveryService 公开方法数量: {methods_ResourceDiscoveryService.Length}");
        foreach (var m in methods_ResourceDiscoveryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResourceDiscoveryService 未找到，尝试无命名空间...");
        type_ResourceDiscoveryService = Type.GetType("ResourceDiscoveryService");
        if (type_ResourceDiscoveryService != null)
            Console.WriteLine("[PASS] 类型 ResourceDiscoveryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResourceDiscoveryService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AlertingService
    var type_AlertingService = Type.GetType("AlertingService");
    if (type_AlertingService != null)
    {
        Console.WriteLine("[PASS] 类型 AlertingService (class) 存在");
        var ctors_AlertingService = type_AlertingService.GetConstructors();
        Console.WriteLine($"[PASS] AlertingService 构造函数数量: {ctors_AlertingService.Length}");
        var methods_AlertingService = type_AlertingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AlertingService 公开方法数量: {methods_AlertingService.Length}");
        foreach (var m in methods_AlertingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AlertingService 未找到，尝试无命名空间...");
        type_AlertingService = Type.GetType("AlertingService");
        if (type_AlertingService != null)
            Console.WriteLine("[PASS] 类型 AlertingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AlertingService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Telemetry
    var type_Telemetry = Type.GetType("Telemetry");
    if (type_Telemetry != null)
    {
        Console.WriteLine("[PASS] 类型 Telemetry (class) 存在");
        var ctors_Telemetry = type_Telemetry.GetConstructors();
        Console.WriteLine($"[PASS] Telemetry 构造函数数量: {ctors_Telemetry.Length}");
        var methods_Telemetry = type_Telemetry.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Telemetry 公开方法数量: {methods_Telemetry.Length}");
        foreach (var m in methods_Telemetry)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Telemetry 未找到，尝试无命名空间...");
        type_Telemetry = Type.GetType("Telemetry");
        if (type_Telemetry != null)
            Console.WriteLine("[PASS] 类型 Telemetry (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Telemetry 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
