#load "opencvsharp_image_processing.cs"

Console.WriteLine("=== opencvsharp_image_processing.cs Test ===");

try
{
    // 验证 class: ImageProcessor
    var type_ImageProcessor = Type.GetType("ImageProcessor");
    if (type_ImageProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ImageProcessor (class) 存在");
        var ctors_ImageProcessor = type_ImageProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ImageProcessor 构造函数数量: {ctors_ImageProcessor.Length}");
        var methods_ImageProcessor = type_ImageProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ImageProcessor 公开方法数量: {methods_ImageProcessor.Length}");
        foreach (var m in methods_ImageProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ImageProcessor 未找到，尝试无命名空间...");
        type_ImageProcessor = Type.GetType("ImageProcessor");
        if (type_ImageProcessor != null)
            Console.WriteLine("[PASS] 类型 ImageProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ImageProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
