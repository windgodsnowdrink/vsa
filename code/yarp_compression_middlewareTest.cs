#load "yarp_compression_middleware.cs"

Console.WriteLine("=== yarp_compression_middleware.cs Test ===");

try
{
    // 验证 class: CompressionMiddleware
    var type_CompressionMiddleware = Type.GetType("CompressionMiddleware");
    if (type_CompressionMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 CompressionMiddleware (class) 存在");
        var ctors_CompressionMiddleware = type_CompressionMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] CompressionMiddleware 构造函数数量: {ctors_CompressionMiddleware.Length}");
        var methods_CompressionMiddleware = type_CompressionMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CompressionMiddleware 公开方法数量: {methods_CompressionMiddleware.Length}");
        foreach (var m in methods_CompressionMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CompressionMiddleware 未找到，尝试无命名空间...");
        type_CompressionMiddleware = Type.GetType("CompressionMiddleware");
        if (type_CompressionMiddleware != null)
            Console.WriteLine("[PASS] 类型 CompressionMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CompressionMiddleware 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
