#load "scalar_api_server.cs"

Console.WriteLine("=== scalar_api_server.cs Test ===");

try
{
    // 验证 class: ScalarApiOptions
    var type_ScalarApiOptions = Type.GetType("ScalarApiOptions");
    if (type_ScalarApiOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ScalarApiOptions (class) 存在");
        var ctors_ScalarApiOptions = type_ScalarApiOptions.GetConstructors();
        Console.WriteLine($"[PASS] ScalarApiOptions 构造函数数量: {ctors_ScalarApiOptions.Length}");
        var methods_ScalarApiOptions = type_ScalarApiOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScalarApiOptions 公开方法数量: {methods_ScalarApiOptions.Length}");
        foreach (var m in methods_ScalarApiOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScalarApiOptions 未找到，尝试无命名空间...");
        type_ScalarApiOptions = Type.GetType("ScalarApiOptions");
        if (type_ScalarApiOptions != null)
            Console.WriteLine("[PASS] 类型 ScalarApiOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScalarApiOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScalarApiExtensions
    var type_ScalarApiExtensions = Type.GetType("ScalarApiExtensions");
    if (type_ScalarApiExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ScalarApiExtensions (class) 存在");
        var ctors_ScalarApiExtensions = type_ScalarApiExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ScalarApiExtensions 构造函数数量: {ctors_ScalarApiExtensions.Length}");
        var methods_ScalarApiExtensions = type_ScalarApiExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScalarApiExtensions 公开方法数量: {methods_ScalarApiExtensions.Length}");
        foreach (var m in methods_ScalarApiExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScalarApiExtensions 未找到，尝试无命名空间...");
        type_ScalarApiExtensions = Type.GetType("ScalarApiExtensions");
        if (type_ScalarApiExtensions != null)
            Console.WriteLine("[PASS] 类型 ScalarApiExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScalarApiExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScalarController
    var type_ScalarController = Type.GetType("ScalarController");
    if (type_ScalarController != null)
    {
        Console.WriteLine("[PASS] 类型 ScalarController (class) 存在");
        var ctors_ScalarController = type_ScalarController.GetConstructors();
        Console.WriteLine($"[PASS] ScalarController 构造函数数量: {ctors_ScalarController.Length}");
        var methods_ScalarController = type_ScalarController.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScalarController 公开方法数量: {methods_ScalarController.Length}");
        foreach (var m in methods_ScalarController)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScalarController 未找到，尝试无命名空间...");
        type_ScalarController = Type.GetType("ScalarController");
        if (type_ScalarController != null)
            Console.WriteLine("[PASS] 类型 ScalarController (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScalarController 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScalarRequest
    var type_ScalarRequest = Type.GetType("ScalarRequest");
    if (type_ScalarRequest != null)
    {
        Console.WriteLine("[PASS] 类型 ScalarRequest (class) 存在");
        var ctors_ScalarRequest = type_ScalarRequest.GetConstructors();
        Console.WriteLine($"[PASS] ScalarRequest 构造函数数量: {ctors_ScalarRequest.Length}");
        var methods_ScalarRequest = type_ScalarRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScalarRequest 公开方法数量: {methods_ScalarRequest.Length}");
        foreach (var m in methods_ScalarRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScalarRequest 未找到，尝试无命名空间...");
        type_ScalarRequest = Type.GetType("ScalarRequest");
        if (type_ScalarRequest != null)
            Console.WriteLine("[PASS] 类型 ScalarRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScalarRequest 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
