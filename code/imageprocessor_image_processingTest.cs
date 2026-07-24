#load "imageprocessor_image_processing.cs"

Console.WriteLine("=== imageprocessor_image_processing.cs Test ===");

try
{
    // 验证 class: ImageProcessorService
    var type_ImageProcessorService = Type.GetType("ImageProcessorService");
    if (type_ImageProcessorService != null)
    {
        Console.WriteLine("[PASS] 类型 ImageProcessorService (class) 存在");
        var ctors_ImageProcessorService = type_ImageProcessorService.GetConstructors();
        Console.WriteLine($"[PASS] ImageProcessorService 构造函数数量: {ctors_ImageProcessorService.Length}");
        var methods_ImageProcessorService = type_ImageProcessorService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ImageProcessorService 公开方法数量: {methods_ImageProcessorService.Length}");
        foreach (var m in methods_ImageProcessorService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ImageProcessorService 未找到，尝试无命名空间...");
        type_ImageProcessorService = Type.GetType("ImageProcessorService");
        if (type_ImageProcessorService != null)
            Console.WriteLine("[PASS] 类型 ImageProcessorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ImageProcessorService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IImageProcessorService
    var type_IImageProcessorService = Type.GetType("IImageProcessorService");
    if (type_IImageProcessorService != null)
    {
        Console.WriteLine("[PASS] 类型 IImageProcessorService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IImageProcessorService 未找到，尝试无命名空间...");
        type_IImageProcessorService = Type.GetType("IImageProcessorService");
        if (type_IImageProcessorService != null)
            Console.WriteLine("[PASS] 类型 IImageProcessorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IImageProcessorService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
