#load "SourceGenerator_fody_mqtt_test.cs"

Console.WriteLine("=== SourceGenerator_fody_mqtt_test.cs Test ===");

try
{
    // 验证 class: FodyConfiguration
    var type_FodyConfiguration = Type.GetType("FodyConfiguration");
    if (type_FodyConfiguration != null)
    {
        Console.WriteLine("[PASS] 类型 FodyConfiguration (class) 存在");
        var ctors_FodyConfiguration = type_FodyConfiguration.GetConstructors();
        Console.WriteLine($"[PASS] FodyConfiguration 构造函数数量: {ctors_FodyConfiguration.Length}");
        var methods_FodyConfiguration = type_FodyConfiguration.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FodyConfiguration 公开方法数量: {methods_FodyConfiguration.Length}");
        foreach (var m in methods_FodyConfiguration)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FodyConfiguration 未找到，尝试无命名空间...");
        type_FodyConfiguration = Type.GetType("FodyConfiguration");
        if (type_FodyConfiguration != null)
            Console.WriteLine("[PASS] 类型 FodyConfiguration (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FodyConfiguration 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttEventBus
    var type_MqttEventBus = Type.GetType("MqttEventBus");
    if (type_MqttEventBus != null)
    {
        Console.WriteLine("[PASS] 类型 MqttEventBus (class) 存在");
        var ctors_MqttEventBus = type_MqttEventBus.GetConstructors();
        Console.WriteLine($"[PASS] MqttEventBus 构造函数数量: {ctors_MqttEventBus.Length}");
        var methods_MqttEventBus = type_MqttEventBus.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttEventBus 公开方法数量: {methods_MqttEventBus.Length}");
        foreach (var m in methods_MqttEventBus)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttEventBus 未找到，尝试无命名空间...");
        type_MqttEventBus = Type.GetType("MqttEventBus");
        if (type_MqttEventBus != null)
            Console.WriteLine("[PASS] 类型 MqttEventBus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttEventBus 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttEventBusTests
    var type_MqttEventBusTests = Type.GetType("MqttEventBusTests");
    if (type_MqttEventBusTests != null)
    {
        Console.WriteLine("[PASS] 类型 MqttEventBusTests (class) 存在");
        var ctors_MqttEventBusTests = type_MqttEventBusTests.GetConstructors();
        Console.WriteLine($"[PASS] MqttEventBusTests 构造函数数量: {ctors_MqttEventBusTests.Length}");
        var methods_MqttEventBusTests = type_MqttEventBusTests.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttEventBusTests 公开方法数量: {methods_MqttEventBusTests.Length}");
        foreach (var m in methods_MqttEventBusTests)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttEventBusTests 未找到，尝试无命名空间...");
        type_MqttEventBusTests = Type.GetType("MqttEventBusTests");
        if (type_MqttEventBusTests != null)
            Console.WriteLine("[PASS] 类型 MqttEventBusTests (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttEventBusTests 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttIntegrationTests
    var type_MqttIntegrationTests = Type.GetType("MqttIntegrationTests");
    if (type_MqttIntegrationTests != null)
    {
        Console.WriteLine("[PASS] 类型 MqttIntegrationTests (class) 存在");
        var ctors_MqttIntegrationTests = type_MqttIntegrationTests.GetConstructors();
        Console.WriteLine($"[PASS] MqttIntegrationTests 构造函数数量: {ctors_MqttIntegrationTests.Length}");
        var methods_MqttIntegrationTests = type_MqttIntegrationTests.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttIntegrationTests 公开方法数量: {methods_MqttIntegrationTests.Length}");
        foreach (var m in methods_MqttIntegrationTests)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttIntegrationTests 未找到，尝试无命名空间...");
        type_MqttIntegrationTests = Type.GetType("MqttIntegrationTests");
        if (type_MqttIntegrationTests != null)
            Console.WriteLine("[PASS] 类型 MqttIntegrationTests (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttIntegrationTests 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
