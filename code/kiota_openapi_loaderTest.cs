#load "kiota_openapi_loader.cs"

Console.WriteLine("=== kiota_openapi_loader.cs Test ===");

try
{
    // 验证 class: ChannelOpenApiLoader
    var type_ChannelOpenApiLoader = Type.GetType("ChannelOpenApiLoader");
    if (type_ChannelOpenApiLoader != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelOpenApiLoader (class) 存在");
        var ctors_ChannelOpenApiLoader = type_ChannelOpenApiLoader.GetConstructors();
        Console.WriteLine($"[PASS] ChannelOpenApiLoader 构造函数数量: {ctors_ChannelOpenApiLoader.Length}");
        var methods_ChannelOpenApiLoader = type_ChannelOpenApiLoader.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelOpenApiLoader 公开方法数量: {methods_ChannelOpenApiLoader.Length}");
        foreach (var m in methods_ChannelOpenApiLoader)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelOpenApiLoader 未找到，尝试无命名空间...");
        type_ChannelOpenApiLoader = Type.GetType("ChannelOpenApiLoader");
        if (type_ChannelOpenApiLoader != null)
            Console.WriteLine("[PASS] 类型 ChannelOpenApiLoader (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelOpenApiLoader 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
