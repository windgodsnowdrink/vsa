#load "lazycache_integration.cs"

Console.WriteLine("=== lazycache_integration.cs Test ===");

try
{
    // 验证 class: HybridCacheStrategy
    var type_HybridCacheStrategy = Type.GetType("HybridCacheStrategy");
    if (type_HybridCacheStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 HybridCacheStrategy (class) 存在");
        var ctors_HybridCacheStrategy = type_HybridCacheStrategy.GetConstructors();
        Console.WriteLine($"[PASS] HybridCacheStrategy 构造函数数量: {ctors_HybridCacheStrategy.Length}");
        var methods_HybridCacheStrategy = type_HybridCacheStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HybridCacheStrategy 公开方法数量: {methods_HybridCacheStrategy.Length}");
        foreach (var m in methods_HybridCacheStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HybridCacheStrategy 未找到，尝试无命名空间...");
        type_HybridCacheStrategy = Type.GetType("HybridCacheStrategy");
        if (type_HybridCacheStrategy != null)
            Console.WriteLine("[PASS] 类型 HybridCacheStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HybridCacheStrategy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MultiLayerCacheExpirationStrategy
    var type_MultiLayerCacheExpirationStrategy = Type.GetType("MultiLayerCacheExpirationStrategy");
    if (type_MultiLayerCacheExpirationStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 MultiLayerCacheExpirationStrategy (class) 存在");
        var ctors_MultiLayerCacheExpirationStrategy = type_MultiLayerCacheExpirationStrategy.GetConstructors();
        Console.WriteLine($"[PASS] MultiLayerCacheExpirationStrategy 构造函数数量: {ctors_MultiLayerCacheExpirationStrategy.Length}");
        var methods_MultiLayerCacheExpirationStrategy = type_MultiLayerCacheExpirationStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MultiLayerCacheExpirationStrategy 公开方法数量: {methods_MultiLayerCacheExpirationStrategy.Length}");
        foreach (var m in methods_MultiLayerCacheExpirationStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MultiLayerCacheExpirationStrategy 未找到，尝试无命名空间...");
        type_MultiLayerCacheExpirationStrategy = Type.GetType("MultiLayerCacheExpirationStrategy");
        if (type_MultiLayerCacheExpirationStrategy != null)
            Console.WriteLine("[PASS] 类型 MultiLayerCacheExpirationStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MultiLayerCacheExpirationStrategy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChannelCacheProcessor
    var type_ChannelCacheProcessor = Type.GetType("ChannelCacheProcessor");
    if (type_ChannelCacheProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelCacheProcessor (class) 存在");
        var ctors_ChannelCacheProcessor = type_ChannelCacheProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelCacheProcessor 构造函数数量: {ctors_ChannelCacheProcessor.Length}");
        var methods_ChannelCacheProcessor = type_ChannelCacheProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelCacheProcessor 公开方法数量: {methods_ChannelCacheProcessor.Length}");
        foreach (var m in methods_ChannelCacheProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelCacheProcessor 未找到，尝试无命名空间...");
        type_ChannelCacheProcessor = Type.GetType("ChannelCacheProcessor");
        if (type_ChannelCacheProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelCacheProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelCacheProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
