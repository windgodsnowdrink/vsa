#load "httprepl_mesh.cs"

Console.WriteLine("=== httprepl_mesh.cs Test ===");

try
{
    // 验证 class: ChannelMeshProcessor
    var type_ChannelMeshProcessor = Type.GetType("ChannelMeshProcessor");
    if (type_ChannelMeshProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelMeshProcessor (class) 存在");
        var ctors_ChannelMeshProcessor = type_ChannelMeshProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelMeshProcessor 构造函数数量: {ctors_ChannelMeshProcessor.Length}");
        var methods_ChannelMeshProcessor = type_ChannelMeshProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelMeshProcessor 公开方法数量: {methods_ChannelMeshProcessor.Length}");
        foreach (var m in methods_ChannelMeshProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelMeshProcessor 未找到，尝试无命名空间...");
        type_ChannelMeshProcessor = Type.GetType("ChannelMeshProcessor");
        if (type_ChannelMeshProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelMeshProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelMeshProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
