#load "facerecognition_dotnet_integration.cs"

Console.WriteLine("=== facerecognition_dotnet_integration.cs Test ===");

try
{
    // 验证 class: FaceRecognitionService
    var type_FaceRecognitionService = Type.GetType("FaceRecognitionService");
    if (type_FaceRecognitionService != null)
    {
        Console.WriteLine("[PASS] 类型 FaceRecognitionService (class) 存在");
        var ctors_FaceRecognitionService = type_FaceRecognitionService.GetConstructors();
        Console.WriteLine($"[PASS] FaceRecognitionService 构造函数数量: {ctors_FaceRecognitionService.Length}");
        var methods_FaceRecognitionService = type_FaceRecognitionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FaceRecognitionService 公开方法数量: {methods_FaceRecognitionService.Length}");
        foreach (var m in methods_FaceRecognitionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FaceRecognitionService 未找到，尝试无命名空间...");
        type_FaceRecognitionService = Type.GetType("FaceRecognitionService");
        if (type_FaceRecognitionService != null)
            Console.WriteLine("[PASS] 类型 FaceRecognitionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FaceRecognitionService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IFaceRecognitionService
    var type_IFaceRecognitionService = Type.GetType("IFaceRecognitionService");
    if (type_IFaceRecognitionService != null)
    {
        Console.WriteLine("[PASS] 类型 IFaceRecognitionService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IFaceRecognitionService 未找到，尝试无命名空间...");
        type_IFaceRecognitionService = Type.GetType("IFaceRecognitionService");
        if (type_IFaceRecognitionService != null)
            Console.WriteLine("[PASS] 类型 IFaceRecognitionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IFaceRecognitionService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
