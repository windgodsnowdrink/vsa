#load "opencv_computer_vision_integration.cs"

Console.WriteLine("=== opencv_computer_vision_integration.cs Test ===");

try
{
    // 验证 class: ComputerVisionService
    var type_ComputerVisionService = Type.GetType("ComputerVisionService");
    if (type_ComputerVisionService != null)
    {
        Console.WriteLine("[PASS] 类型 ComputerVisionService (class) 存在");
        var ctors_ComputerVisionService = type_ComputerVisionService.GetConstructors();
        Console.WriteLine($"[PASS] ComputerVisionService 构造函数数量: {ctors_ComputerVisionService.Length}");
        var methods_ComputerVisionService = type_ComputerVisionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ComputerVisionService 公开方法数量: {methods_ComputerVisionService.Length}");
        foreach (var m in methods_ComputerVisionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ComputerVisionService 未找到，尝试无命名空间...");
        type_ComputerVisionService = Type.GetType("ComputerVisionService");
        if (type_ComputerVisionService != null)
            Console.WriteLine("[PASS] 类型 ComputerVisionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ComputerVisionService 可能为顶层语句或嵌套类型");
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
