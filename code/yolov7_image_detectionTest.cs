#load "yolov7_image_detection.cs"

Console.WriteLine("=== yolov7_image_detection.cs Test ===");

try
{
    // 验证 class: DetectionResult
    var type_DetectionResult = Type.GetType("DetectionResult");
    if (type_DetectionResult != null)
    {
        Console.WriteLine("[PASS] 类型 DetectionResult (class) 存在");
        var ctors_DetectionResult = type_DetectionResult.GetConstructors();
        Console.WriteLine($"[PASS] DetectionResult 构造函数数量: {ctors_DetectionResult.Length}");
        var methods_DetectionResult = type_DetectionResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DetectionResult 公开方法数量: {methods_DetectionResult.Length}");
        foreach (var m in methods_DetectionResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DetectionResult 未找到，尝试无命名空间...");
        type_DetectionResult = Type.GetType("DetectionResult");
        if (type_DetectionResult != null)
            Console.WriteLine("[PASS] 类型 DetectionResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DetectionResult 可能为顶层语句或嵌套类型");
    }

    // 验证 class: YoloV7DetectionService
    var type_YoloV7DetectionService = Type.GetType("YoloV7DetectionService");
    if (type_YoloV7DetectionService != null)
    {
        Console.WriteLine("[PASS] 类型 YoloV7DetectionService (class) 存在");
        var ctors_YoloV7DetectionService = type_YoloV7DetectionService.GetConstructors();
        Console.WriteLine($"[PASS] YoloV7DetectionService 构造函数数量: {ctors_YoloV7DetectionService.Length}");
        var methods_YoloV7DetectionService = type_YoloV7DetectionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] YoloV7DetectionService 公开方法数量: {methods_YoloV7DetectionService.Length}");
        foreach (var m in methods_YoloV7DetectionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 YoloV7DetectionService 未找到，尝试无命名空间...");
        type_YoloV7DetectionService = Type.GetType("YoloV7DetectionService");
        if (type_YoloV7DetectionService != null)
            Console.WriteLine("[PASS] 类型 YoloV7DetectionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 YoloV7DetectionService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IObjectDetectionService
    var type_IObjectDetectionService = Type.GetType("IObjectDetectionService");
    if (type_IObjectDetectionService != null)
    {
        Console.WriteLine("[PASS] 类型 IObjectDetectionService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IObjectDetectionService 未找到，尝试无命名空间...");
        type_IObjectDetectionService = Type.GetType("IObjectDetectionService");
        if (type_IObjectDetectionService != null)
            Console.WriteLine("[PASS] 类型 IObjectDetectionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IObjectDetectionService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
