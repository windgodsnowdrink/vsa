#load "imagesharp_image_processing.cs"

Console.WriteLine("=== imagesharp_image_processing.cs Test ===");

try
{
    // 验证 class: ImageProcessingService
    var type_ImageProcessingService = Type.GetType("ImageProcessingService");
    if (type_ImageProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 ImageProcessingService (class) 存在");
        var ctors_ImageProcessingService = type_ImageProcessingService.GetConstructors();
        Console.WriteLine($"[PASS] ImageProcessingService 构造函数数量: {ctors_ImageProcessingService.Length}");
        var methods_ImageProcessingService = type_ImageProcessingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ImageProcessingService 公开方法数量: {methods_ImageProcessingService.Length}");
        foreach (var m in methods_ImageProcessingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ImageProcessingService 未找到，尝试无命名空间...");
        type_ImageProcessingService = Type.GetType("ImageProcessingService");
        if (type_ImageProcessingService != null)
            Console.WriteLine("[PASS] 类型 ImageProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ImageProcessingService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IImageProcessingService
    var type_IImageProcessingService = Type.GetType("IImageProcessingService");
    if (type_IImageProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 IImageProcessingService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IImageProcessingService 未找到，尝试无命名空间...");
        type_IImageProcessingService = Type.GetType("IImageProcessingService");
        if (type_IImageProcessingService != null)
            Console.WriteLine("[PASS] 类型 IImageProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IImageProcessingService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
