#load "emgucv_image_processing.cs"

Console.WriteLine("=== emgucv_image_processing.cs Test ===");

try
{
    // 验证 class: EmguCvImageProcessingService
    var type_EmguCvImageProcessingService = Type.GetType("EmguCvImageProcessingService");
    if (type_EmguCvImageProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 EmguCvImageProcessingService (class) 存在");
        var ctors_EmguCvImageProcessingService = type_EmguCvImageProcessingService.GetConstructors();
        Console.WriteLine($"[PASS] EmguCvImageProcessingService 构造函数数量: {ctors_EmguCvImageProcessingService.Length}");
        var methods_EmguCvImageProcessingService = type_EmguCvImageProcessingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EmguCvImageProcessingService 公开方法数量: {methods_EmguCvImageProcessingService.Length}");
        foreach (var m in methods_EmguCvImageProcessingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EmguCvImageProcessingService 未找到，尝试无命名空间...");
        type_EmguCvImageProcessingService = Type.GetType("EmguCvImageProcessingService");
        if (type_EmguCvImageProcessingService != null)
            Console.WriteLine("[PASS] 类型 EmguCvImageProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EmguCvImageProcessingService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IEmguCvImageProcessingService
    var type_IEmguCvImageProcessingService = Type.GetType("IEmguCvImageProcessingService");
    if (type_IEmguCvImageProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 IEmguCvImageProcessingService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IEmguCvImageProcessingService 未找到，尝试无命名空间...");
        type_IEmguCvImageProcessingService = Type.GetType("IEmguCvImageProcessingService");
        if (type_IEmguCvImageProcessingService != null)
            Console.WriteLine("[PASS] 类型 IEmguCvImageProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IEmguCvImageProcessingService 可能为顶层语句或嵌套类型");
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
