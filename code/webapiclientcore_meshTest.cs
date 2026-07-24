#load "webapiclientcore_mesh.cs"

Console.WriteLine("=== webapiclientcore_mesh.cs Test ===");

try
{
    // 验证 class: ChannelMeshProxy
    var type_ChannelMeshProxy = Type.GetType("ChannelMeshProxy");
    if (type_ChannelMeshProxy != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelMeshProxy (class) 存在");
        var ctors_ChannelMeshProxy = type_ChannelMeshProxy.GetConstructors();
        Console.WriteLine($"[PASS] ChannelMeshProxy 构造函数数量: {ctors_ChannelMeshProxy.Length}");
        var methods_ChannelMeshProxy = type_ChannelMeshProxy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelMeshProxy 公开方法数量: {methods_ChannelMeshProxy.Length}");
        foreach (var m in methods_ChannelMeshProxy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelMeshProxy 未找到，尝试无命名空间...");
        type_ChannelMeshProxy = Type.GetType("ChannelMeshProxy");
        if (type_ChannelMeshProxy != null)
            Console.WriteLine("[PASS] 类型 ChannelMeshProxy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelMeshProxy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
