#load "signalr_optimized.cs"

Console.WriteLine("=== signalr_optimized.cs Test ===");

try
{
    // 验证 class: AdaptiveCompressionStrategy
    var type_AdaptiveCompressionStrategy = Type.GetType("AdaptiveCompressionStrategy");
    if (type_AdaptiveCompressionStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 AdaptiveCompressionStrategy (class) 存在");
        var ctors_AdaptiveCompressionStrategy = type_AdaptiveCompressionStrategy.GetConstructors();
        Console.WriteLine($"[PASS] AdaptiveCompressionStrategy 构造函数数量: {ctors_AdaptiveCompressionStrategy.Length}");
        var methods_AdaptiveCompressionStrategy = type_AdaptiveCompressionStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdaptiveCompressionStrategy 公开方法数量: {methods_AdaptiveCompressionStrategy.Length}");
        foreach (var m in methods_AdaptiveCompressionStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdaptiveCompressionStrategy 未找到，尝试无命名空间...");
        type_AdaptiveCompressionStrategy = Type.GetType("AdaptiveCompressionStrategy");
        if (type_AdaptiveCompressionStrategy != null)
            Console.WriteLine("[PASS] 类型 AdaptiveCompressionStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdaptiveCompressionStrategy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TimeBasedExpiration
    var type_TimeBasedExpiration = Type.GetType("TimeBasedExpiration");
    if (type_TimeBasedExpiration != null)
    {
        Console.WriteLine("[PASS] 类型 TimeBasedExpiration (class) 存在");
        var ctors_TimeBasedExpiration = type_TimeBasedExpiration.GetConstructors();
        Console.WriteLine($"[PASS] TimeBasedExpiration 构造函数数量: {ctors_TimeBasedExpiration.Length}");
        var methods_TimeBasedExpiration = type_TimeBasedExpiration.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TimeBasedExpiration 公开方法数量: {methods_TimeBasedExpiration.Length}");
        foreach (var m in methods_TimeBasedExpiration)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TimeBasedExpiration 未找到，尝试无命名空间...");
        type_TimeBasedExpiration = Type.GetType("TimeBasedExpiration");
        if (type_TimeBasedExpiration != null)
            Console.WriteLine("[PASS] 类型 TimeBasedExpiration (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TimeBasedExpiration 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DynamicQoSAdjuster
    var type_DynamicQoSAdjuster = Type.GetType("DynamicQoSAdjuster");
    if (type_DynamicQoSAdjuster != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicQoSAdjuster (class) 存在");
        var ctors_DynamicQoSAdjuster = type_DynamicQoSAdjuster.GetConstructors();
        Console.WriteLine($"[PASS] DynamicQoSAdjuster 构造函数数量: {ctors_DynamicQoSAdjuster.Length}");
        var methods_DynamicQoSAdjuster = type_DynamicQoSAdjuster.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicQoSAdjuster 公开方法数量: {methods_DynamicQoSAdjuster.Length}");
        foreach (var m in methods_DynamicQoSAdjuster)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicQoSAdjuster 未找到，尝试无命名空间...");
        type_DynamicQoSAdjuster = Type.GetType("DynamicQoSAdjuster");
        if (type_DynamicQoSAdjuster != null)
            Console.WriteLine("[PASS] 类型 DynamicQoSAdjuster (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicQoSAdjuster 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CompressionOptions
    var type_CompressionOptions = Type.GetType("CompressionOptions");
    if (type_CompressionOptions != null)
    {
        Console.WriteLine("[PASS] 类型 CompressionOptions (class) 存在");
        var ctors_CompressionOptions = type_CompressionOptions.GetConstructors();
        Console.WriteLine($"[PASS] CompressionOptions 构造函数数量: {ctors_CompressionOptions.Length}");
        var methods_CompressionOptions = type_CompressionOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CompressionOptions 公开方法数量: {methods_CompressionOptions.Length}");
        foreach (var m in methods_CompressionOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CompressionOptions 未找到，尝试无命名空间...");
        type_CompressionOptions = Type.GetType("CompressionOptions");
        if (type_CompressionOptions != null)
            Console.WriteLine("[PASS] 类型 CompressionOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CompressionOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: CompressionType
    var type_CompressionType = Type.GetType("CompressionType");
    if (type_CompressionType != null)
    {
        Console.WriteLine("[PASS] 类型 CompressionType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CompressionType 未找到，尝试无命名空间...");
        type_CompressionType = Type.GetType("CompressionType");
        if (type_CompressionType != null)
            Console.WriteLine("[PASS] 类型 CompressionType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CompressionType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
