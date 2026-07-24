#load "skaia_image_processing.cs"

Console.WriteLine("=== skaia_image_processing.cs Test ===");

try
{
    // 验证 class: SkaiaImageProcessingService
    var type_SkaiaImageProcessingService = Type.GetType("SkaiaImageProcessingService");
    if (type_SkaiaImageProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 SkaiaImageProcessingService (class) 存在");
        var ctors_SkaiaImageProcessingService = type_SkaiaImageProcessingService.GetConstructors();
        Console.WriteLine($"[PASS] SkaiaImageProcessingService 构造函数数量: {ctors_SkaiaImageProcessingService.Length}");
        var methods_SkaiaImageProcessingService = type_SkaiaImageProcessingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SkaiaImageProcessingService 公开方法数量: {methods_SkaiaImageProcessingService.Length}");
        foreach (var m in methods_SkaiaImageProcessingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SkaiaImageProcessingService 未找到，尝试无命名空间...");
        type_SkaiaImageProcessingService = Type.GetType("SkaiaImageProcessingService");
        if (type_SkaiaImageProcessingService != null)
            Console.WriteLine("[PASS] 类型 SkaiaImageProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SkaiaImageProcessingService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: ISkaiaImageProcessingService
    var type_ISkaiaImageProcessingService = Type.GetType("ISkaiaImageProcessingService");
    if (type_ISkaiaImageProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 ISkaiaImageProcessingService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ISkaiaImageProcessingService 未找到，尝试无命名空间...");
        type_ISkaiaImageProcessingService = Type.GetType("ISkaiaImageProcessingService");
        if (type_ISkaiaImageProcessingService != null)
            Console.WriteLine("[PASS] 类型 ISkaiaImageProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ISkaiaImageProcessingService 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: FilterType
    var type_FilterType = Type.GetType("FilterType");
    if (type_FilterType != null)
    {
        Console.WriteLine("[PASS] 类型 FilterType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FilterType 未找到，尝试无命名空间...");
        type_FilterType = Type.GetType("FilterType");
        if (type_FilterType != null)
            Console.WriteLine("[PASS] 类型 FilterType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FilterType 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: ImageFormat
    var type_ImageFormat = Type.GetType("ImageFormat");
    if (type_ImageFormat != null)
    {
        Console.WriteLine("[PASS] 类型 ImageFormat (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ImageFormat 未找到，尝试无命名空间...");
        type_ImageFormat = Type.GetType("ImageFormat");
        if (type_ImageFormat != null)
            Console.WriteLine("[PASS] 类型 ImageFormat (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ImageFormat 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
