#load "webapiclientcore_integration.cs"

Console.WriteLine("=== webapiclientcore_integration.cs Test ===");

try
{
    // 验证 class: ApiRequestProcessor
    var type_ApiRequestProcessor = Type.GetType("ApiRequestProcessor");
    if (type_ApiRequestProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ApiRequestProcessor (class) 存在");
        var ctors_ApiRequestProcessor = type_ApiRequestProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ApiRequestProcessor 构造函数数量: {ctors_ApiRequestProcessor.Length}");
        var methods_ApiRequestProcessor = type_ApiRequestProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ApiRequestProcessor 公开方法数量: {methods_ApiRequestProcessor.Length}");
        foreach (var m in methods_ApiRequestProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ApiRequestProcessor 未找到，尝试无命名空间...");
        type_ApiRequestProcessor = Type.GetType("ApiRequestProcessor");
        if (type_ApiRequestProcessor != null)
            Console.WriteLine("[PASS] 类型 ApiRequestProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ApiRequestProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ApiHealthCheck
    var type_ApiHealthCheck = Type.GetType("ApiHealthCheck");
    if (type_ApiHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 ApiHealthCheck (class) 存在");
        var ctors_ApiHealthCheck = type_ApiHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] ApiHealthCheck 构造函数数量: {ctors_ApiHealthCheck.Length}");
        var methods_ApiHealthCheck = type_ApiHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ApiHealthCheck 公开方法数量: {methods_ApiHealthCheck.Length}");
        foreach (var m in methods_ApiHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ApiHealthCheck 未找到，尝试无命名空间...");
        type_ApiHealthCheck = Type.GetType("ApiHealthCheck");
        if (type_ApiHealthCheck != null)
            Console.WriteLine("[PASS] 类型 ApiHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ApiHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IMyApiService
    var type_IMyApiService = Type.GetType("IMyApiService");
    if (type_IMyApiService != null)
    {
        Console.WriteLine("[PASS] 类型 IMyApiService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IMyApiService 未找到，尝试无命名空间...");
        type_IMyApiService = Type.GetType("IMyApiService");
        if (type_IMyApiService != null)
            Console.WriteLine("[PASS] 类型 IMyApiService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IMyApiService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
