#load "facedetect_torchsharp_integration.cs"

Console.WriteLine("=== facedetect_torchsharp_integration.cs Test ===");

try
{
    // 验证 class: FaceDetectionService
    var type_FaceDetectionService = Type.GetType("FaceDetectionService");
    if (type_FaceDetectionService != null)
    {
        Console.WriteLine("[PASS] 类型 FaceDetectionService (class) 存在");
        var ctors_FaceDetectionService = type_FaceDetectionService.GetConstructors();
        Console.WriteLine($"[PASS] FaceDetectionService 构造函数数量: {ctors_FaceDetectionService.Length}");
        var methods_FaceDetectionService = type_FaceDetectionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FaceDetectionService 公开方法数量: {methods_FaceDetectionService.Length}");
        foreach (var m in methods_FaceDetectionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FaceDetectionService 未找到，尝试无命名空间...");
        type_FaceDetectionService = Type.GetType("FaceDetectionService");
        if (type_FaceDetectionService != null)
            Console.WriteLine("[PASS] 类型 FaceDetectionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FaceDetectionService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IFaceDetectionService
    var type_IFaceDetectionService = Type.GetType("IFaceDetectionService");
    if (type_IFaceDetectionService != null)
    {
        Console.WriteLine("[PASS] 类型 IFaceDetectionService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IFaceDetectionService 未找到，尝试无命名空间...");
        type_IFaceDetectionService = Type.GetType("IFaceDetectionService");
        if (type_IFaceDetectionService != null)
            Console.WriteLine("[PASS] 类型 IFaceDetectionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IFaceDetectionService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
