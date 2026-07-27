#load "kiota_cache.cs"

Console.WriteLine("=== kiota_cache.cs Test ===");

try
{
    // 验证 class: ChannelCacheHandler
    var type_ChannelCacheHandler = Type.GetType("ChannelCacheHandler");
    if (type_ChannelCacheHandler != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelCacheHandler (class) 存在");
        var ctors_ChannelCacheHandler = type_ChannelCacheHandler.GetConstructors();
        Console.WriteLine($"[PASS] ChannelCacheHandler 构造函数数量: {ctors_ChannelCacheHandler.Length}");
        var methods_ChannelCacheHandler = type_ChannelCacheHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelCacheHandler 公开方法数量: {methods_ChannelCacheHandler.Length}");
        foreach (var m in methods_ChannelCacheHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelCacheHandler 未找到，尝试无命名空间...");
        type_ChannelCacheHandler = Type.GetType("ChannelCacheHandler");
        if (type_ChannelCacheHandler != null)
            Console.WriteLine("[PASS] 类型 ChannelCacheHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelCacheHandler 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
