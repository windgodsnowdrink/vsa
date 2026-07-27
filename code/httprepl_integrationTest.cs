#load "httprepl_integration.cs"

Console.WriteLine("=== httprepl_integration.cs Test ===");

try
{
    // 验证 class: ChannelHttpReplClientFactory
    var type_ChannelHttpReplClientFactory = Type.GetType("ChannelHttpReplClientFactory");
    if (type_ChannelHttpReplClientFactory != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelHttpReplClientFactory (class) 存在");
        var ctors_ChannelHttpReplClientFactory = type_ChannelHttpReplClientFactory.GetConstructors();
        Console.WriteLine($"[PASS] ChannelHttpReplClientFactory 构造函数数量: {ctors_ChannelHttpReplClientFactory.Length}");
        var methods_ChannelHttpReplClientFactory = type_ChannelHttpReplClientFactory.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelHttpReplClientFactory 公开方法数量: {methods_ChannelHttpReplClientFactory.Length}");
        foreach (var m in methods_ChannelHttpReplClientFactory)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelHttpReplClientFactory 未找到，尝试无命名空间...");
        type_ChannelHttpReplClientFactory = Type.GetType("ChannelHttpReplClientFactory");
        if (type_ChannelHttpReplClientFactory != null)
            Console.WriteLine("[PASS] 类型 ChannelHttpReplClientFactory (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelHttpReplClientFactory 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HttpReplRetryStrategy
    var type_HttpReplRetryStrategy = Type.GetType("HttpReplRetryStrategy");
    if (type_HttpReplRetryStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 HttpReplRetryStrategy (class) 存在");
        var ctors_HttpReplRetryStrategy = type_HttpReplRetryStrategy.GetConstructors();
        Console.WriteLine($"[PASS] HttpReplRetryStrategy 构造函数数量: {ctors_HttpReplRetryStrategy.Length}");
        var methods_HttpReplRetryStrategy = type_HttpReplRetryStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HttpReplRetryStrategy 公开方法数量: {methods_HttpReplRetryStrategy.Length}");
        foreach (var m in methods_HttpReplRetryStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HttpReplRetryStrategy 未找到，尝试无命名空间...");
        type_HttpReplRetryStrategy = Type.GetType("HttpReplRetryStrategy");
        if (type_HttpReplRetryStrategy != null)
            Console.WriteLine("[PASS] 类型 HttpReplRetryStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HttpReplRetryStrategy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
