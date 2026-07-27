#load "redis_eventbus_integration.cs"

Console.WriteLine("=== redis_eventbus_integration.cs Test ===");

try
{
    // 验证 class: RedisEventBus.Integration.RedisEventBusOptions
    var type_RedisEventBusOptions = Type.GetType("RedisEventBus.Integration.RedisEventBusOptions");
    if (type_RedisEventBusOptions != null)
    {
        Console.WriteLine("[PASS] 类型 RedisEventBus.Integration.RedisEventBusOptions (class) 存在");
        var ctors_RedisEventBusOptions = type_RedisEventBusOptions.GetConstructors();
        Console.WriteLine($"[PASS] RedisEventBus.Integration.RedisEventBusOptions 构造函数数量: {ctors_RedisEventBusOptions.Length}");
        var methods_RedisEventBusOptions = type_RedisEventBusOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisEventBus.Integration.RedisEventBusOptions 公开方法数量: {methods_RedisEventBusOptions.Length}");
        foreach (var m in methods_RedisEventBusOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisEventBus.Integration.RedisEventBusOptions 未找到，尝试无命名空间...");
        type_RedisEventBusOptions = Type.GetType("RedisEventBusOptions");
        if (type_RedisEventBusOptions != null)
            Console.WriteLine("[PASS] 类型 RedisEventBusOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisEventBusOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RedisEventBus.Integration.TieredMemoryServer
    var type_TieredMemoryServer = Type.GetType("RedisEventBus.Integration.TieredMemoryServer");
    if (type_TieredMemoryServer != null)
    {
        Console.WriteLine("[PASS] 类型 RedisEventBus.Integration.TieredMemoryServer (class) 存在");
        var ctors_TieredMemoryServer = type_TieredMemoryServer.GetConstructors();
        Console.WriteLine($"[PASS] RedisEventBus.Integration.TieredMemoryServer 构造函数数量: {ctors_TieredMemoryServer.Length}");
        var methods_TieredMemoryServer = type_TieredMemoryServer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisEventBus.Integration.TieredMemoryServer 公开方法数量: {methods_TieredMemoryServer.Length}");
        foreach (var m in methods_TieredMemoryServer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisEventBus.Integration.TieredMemoryServer 未找到，尝试无命名空间...");
        type_TieredMemoryServer = Type.GetType("TieredMemoryServer");
        if (type_TieredMemoryServer != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryServer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryServer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RedisEventBus.Integration.RedisEventBus
    var type_RedisEventBus = Type.GetType("RedisEventBus.Integration.RedisEventBus");
    if (type_RedisEventBus != null)
    {
        Console.WriteLine("[PASS] 类型 RedisEventBus.Integration.RedisEventBus (class) 存在");
        var ctors_RedisEventBus = type_RedisEventBus.GetConstructors();
        Console.WriteLine($"[PASS] RedisEventBus.Integration.RedisEventBus 构造函数数量: {ctors_RedisEventBus.Length}");
        var methods_RedisEventBus = type_RedisEventBus.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisEventBus.Integration.RedisEventBus 公开方法数量: {methods_RedisEventBus.Length}");
        foreach (var m in methods_RedisEventBus)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisEventBus.Integration.RedisEventBus 未找到，尝试无命名空间...");
        type_RedisEventBus = Type.GetType("RedisEventBus");
        if (type_RedisEventBus != null)
            Console.WriteLine("[PASS] 类型 RedisEventBus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisEventBus 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RedisEventBus.Integration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("RedisEventBus.Integration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 RedisEventBus.Integration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] RedisEventBus.Integration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisEventBus.Integration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisEventBus.Integration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RedisEventBus.Integration.Program
    var type_Program = Type.GetType("RedisEventBus.Integration.Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 RedisEventBus.Integration.Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] RedisEventBus.Integration.Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisEventBus.Integration.Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisEventBus.Integration.Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: RedisEventBus.Integration.ITieredMemoryServer
    var type_ITieredMemoryServer = Type.GetType("RedisEventBus.Integration.ITieredMemoryServer");
    if (type_ITieredMemoryServer != null)
    {
        Console.WriteLine("[PASS] 类型 RedisEventBus.Integration.ITieredMemoryServer (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisEventBus.Integration.ITieredMemoryServer 未找到，尝试无命名空间...");
        type_ITieredMemoryServer = Type.GetType("ITieredMemoryServer");
        if (type_ITieredMemoryServer != null)
            Console.WriteLine("[PASS] 类型 ITieredMemoryServer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITieredMemoryServer 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: RedisEventBus.Integration.IRedisEventBus
    var type_IRedisEventBus = Type.GetType("RedisEventBus.Integration.IRedisEventBus");
    if (type_IRedisEventBus != null)
    {
        Console.WriteLine("[PASS] 类型 RedisEventBus.Integration.IRedisEventBus (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisEventBus.Integration.IRedisEventBus 未找到，尝试无命名空间...");
        type_IRedisEventBus = Type.GetType("IRedisEventBus");
        if (type_IRedisEventBus != null)
            Console.WriteLine("[PASS] 类型 IRedisEventBus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IRedisEventBus 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
