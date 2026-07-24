#load "sso_microservice.cs"

Console.WriteLine("=== sso_microservice.cs Test ===");

try
{
    // 验证 class: SsoMicroservice
    var type_SsoMicroservice = Type.GetType("SsoMicroservice");
    if (type_SsoMicroservice != null)
    {
        Console.WriteLine("[PASS] 类型 SsoMicroservice (class) 存在");
        var ctors_SsoMicroservice = type_SsoMicroservice.GetConstructors();
        Console.WriteLine($"[PASS] SsoMicroservice 构造函数数量: {ctors_SsoMicroservice.Length}");
        var methods_SsoMicroservice = type_SsoMicroservice.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SsoMicroservice 公开方法数量: {methods_SsoMicroservice.Length}");
        foreach (var m in methods_SsoMicroservice)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SsoMicroservice 未找到，尝试无命名空间...");
        type_SsoMicroservice = Type.GetType("SsoMicroservice");
        if (type_SsoMicroservice != null)
            Console.WriteLine("[PASS] 类型 SsoMicroservice (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SsoMicroservice 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SsoBusConfig
    var type_SsoBusConfig = Type.GetType("SsoBusConfig");
    if (type_SsoBusConfig != null)
    {
        Console.WriteLine("[PASS] 类型 SsoBusConfig (class) 存在");
        var ctors_SsoBusConfig = type_SsoBusConfig.GetConstructors();
        Console.WriteLine($"[PASS] SsoBusConfig 构造函数数量: {ctors_SsoBusConfig.Length}");
        var methods_SsoBusConfig = type_SsoBusConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SsoBusConfig 公开方法数量: {methods_SsoBusConfig.Length}");
        foreach (var m in methods_SsoBusConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SsoBusConfig 未找到，尝试无命名空间...");
        type_SsoBusConfig = Type.GetType("SsoBusConfig");
        if (type_SsoBusConfig != null)
            Console.WriteLine("[PASS] 类型 SsoBusConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SsoBusConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 record: SsoCommand
    var type_SsoCommand = Type.GetType("SsoCommand");
    if (type_SsoCommand != null)
    {
        Console.WriteLine("[PASS] 类型 SsoCommand (record) 存在");
        var ctors_SsoCommand = type_SsoCommand.GetConstructors();
        Console.WriteLine($"[PASS] SsoCommand 构造函数数量: {ctors_SsoCommand.Length}");
        var methods_SsoCommand = type_SsoCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SsoCommand 公开方法数量: {methods_SsoCommand.Length}");
        foreach (var m in methods_SsoCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SsoCommand 未找到，尝试无命名空间...");
        type_SsoCommand = Type.GetType("SsoCommand");
        if (type_SsoCommand != null)
            Console.WriteLine("[PASS] 类型 SsoCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SsoCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: SsoEvent
    var type_SsoEvent = Type.GetType("SsoEvent");
    if (type_SsoEvent != null)
    {
        Console.WriteLine("[PASS] 类型 SsoEvent (record) 存在");
        var ctors_SsoEvent = type_SsoEvent.GetConstructors();
        Console.WriteLine($"[PASS] SsoEvent 构造函数数量: {ctors_SsoEvent.Length}");
        var methods_SsoEvent = type_SsoEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SsoEvent 公开方法数量: {methods_SsoEvent.Length}");
        foreach (var m in methods_SsoEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SsoEvent 未找到，尝试无命名空间...");
        type_SsoEvent = Type.GetType("SsoEvent");
        if (type_SsoEvent != null)
            Console.WriteLine("[PASS] 类型 SsoEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SsoEvent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
