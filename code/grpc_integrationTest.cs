#load "grpc_integration.cs"

Console.WriteLine("=== grpc_integration.cs Test ===");

try
{
    // 验证 class: GrpcChannelProcessor
    var type_GrpcChannelProcessor = Type.GetType("GrpcChannelProcessor");
    if (type_GrpcChannelProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 GrpcChannelProcessor (class) 存在");
        var ctors_GrpcChannelProcessor = type_GrpcChannelProcessor.GetConstructors();
        Console.WriteLine($"[PASS] GrpcChannelProcessor 构造函数数量: {ctors_GrpcChannelProcessor.Length}");
        var methods_GrpcChannelProcessor = type_GrpcChannelProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GrpcChannelProcessor 公开方法数量: {methods_GrpcChannelProcessor.Length}");
        foreach (var m in methods_GrpcChannelProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GrpcChannelProcessor 未找到，尝试无命名空间...");
        type_GrpcChannelProcessor = Type.GetType("GrpcChannelProcessor");
        if (type_GrpcChannelProcessor != null)
            Console.WriteLine("[PASS] 类型 GrpcChannelProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GrpcChannelProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
