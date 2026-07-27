#load "restsharp_mesh.cs"

Console.WriteLine("=== restsharp_mesh.cs Test ===");

try
{
    // 验证 class: ChannelMeshSecurityHandler
    var type_ChannelMeshSecurityHandler = Type.GetType("ChannelMeshSecurityHandler");
    if (type_ChannelMeshSecurityHandler != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelMeshSecurityHandler (class) 存在");
        var ctors_ChannelMeshSecurityHandler = type_ChannelMeshSecurityHandler.GetConstructors();
        Console.WriteLine($"[PASS] ChannelMeshSecurityHandler 构造函数数量: {ctors_ChannelMeshSecurityHandler.Length}");
        var methods_ChannelMeshSecurityHandler = type_ChannelMeshSecurityHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelMeshSecurityHandler 公开方法数量: {methods_ChannelMeshSecurityHandler.Length}");
        foreach (var m in methods_ChannelMeshSecurityHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelMeshSecurityHandler 未找到，尝试无命名空间...");
        type_ChannelMeshSecurityHandler = Type.GetType("ChannelMeshSecurityHandler");
        if (type_ChannelMeshSecurityHandler != null)
            Console.WriteLine("[PASS] 类型 ChannelMeshSecurityHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelMeshSecurityHandler 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
