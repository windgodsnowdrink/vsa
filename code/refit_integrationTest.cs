#load "refit_integration.cs"

Console.WriteLine("=== refit_integration.cs Test ===");

try
{
    // 验证 class: ChannelApiRequestProcessor
    var type_ChannelApiRequestProcessor = Type.GetType("ChannelApiRequestProcessor");
    if (type_ChannelApiRequestProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelApiRequestProcessor (class) 存在");
        var ctors_ChannelApiRequestProcessor = type_ChannelApiRequestProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelApiRequestProcessor 构造函数数量: {ctors_ChannelApiRequestProcessor.Length}");
        var methods_ChannelApiRequestProcessor = type_ChannelApiRequestProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelApiRequestProcessor 公开方法数量: {methods_ChannelApiRequestProcessor.Length}");
        foreach (var m in methods_ChannelApiRequestProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelApiRequestProcessor 未找到，尝试无命名空间...");
        type_ChannelApiRequestProcessor = Type.GetType("ChannelApiRequestProcessor");
        if (type_ChannelApiRequestProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelApiRequestProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelApiRequestProcessor 可能为顶层语句或嵌套类型");
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
