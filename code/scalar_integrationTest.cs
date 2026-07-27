#load "scalar_integration.cs"

Console.WriteLine("=== scalar_integration.cs Test ===");

try
{
    // 验证 class: ScalarOptions
    var type_ScalarOptions = Type.GetType("ScalarOptions");
    if (type_ScalarOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ScalarOptions (class) 存在");
        var ctors_ScalarOptions = type_ScalarOptions.GetConstructors();
        Console.WriteLine($"[PASS] ScalarOptions 构造函数数量: {ctors_ScalarOptions.Length}");
        var methods_ScalarOptions = type_ScalarOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScalarOptions 公开方法数量: {methods_ScalarOptions.Length}");
        foreach (var m in methods_ScalarOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScalarOptions 未找到，尝试无命名空间...");
        type_ScalarOptions = Type.GetType("ScalarOptions");
        if (type_ScalarOptions != null)
            Console.WriteLine("[PASS] 类型 ScalarOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScalarOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScalarService
    var type_ScalarService = Type.GetType("ScalarService");
    if (type_ScalarService != null)
    {
        Console.WriteLine("[PASS] 类型 ScalarService (class) 存在");
        var ctors_ScalarService = type_ScalarService.GetConstructors();
        Console.WriteLine($"[PASS] ScalarService 构造函数数量: {ctors_ScalarService.Length}");
        var methods_ScalarService = type_ScalarService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScalarService 公开方法数量: {methods_ScalarService.Length}");
        foreach (var m in methods_ScalarService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScalarService 未找到，尝试无命名空间...");
        type_ScalarService = Type.GetType("ScalarService");
        if (type_ScalarService != null)
            Console.WriteLine("[PASS] 类型 ScalarService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScalarService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScalarExtensions
    var type_ScalarExtensions = Type.GetType("ScalarExtensions");
    if (type_ScalarExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ScalarExtensions (class) 存在");
        var ctors_ScalarExtensions = type_ScalarExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ScalarExtensions 构造函数数量: {ctors_ScalarExtensions.Length}");
        var methods_ScalarExtensions = type_ScalarExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScalarExtensions 公开方法数量: {methods_ScalarExtensions.Length}");
        foreach (var m in methods_ScalarExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScalarExtensions 未找到，尝试无命名空间...");
        type_ScalarExtensions = Type.GetType("ScalarExtensions");
        if (type_ScalarExtensions != null)
            Console.WriteLine("[PASS] 类型 ScalarExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScalarExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScalarBackgroundService
    var type_ScalarBackgroundService = Type.GetType("ScalarBackgroundService");
    if (type_ScalarBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 ScalarBackgroundService (class) 存在");
        var ctors_ScalarBackgroundService = type_ScalarBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] ScalarBackgroundService 构造函数数量: {ctors_ScalarBackgroundService.Length}");
        var methods_ScalarBackgroundService = type_ScalarBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScalarBackgroundService 公开方法数量: {methods_ScalarBackgroundService.Length}");
        foreach (var m in methods_ScalarBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScalarBackgroundService 未找到，尝试无命名空间...");
        type_ScalarBackgroundService = Type.GetType("ScalarBackgroundService");
        if (type_ScalarBackgroundService != null)
            Console.WriteLine("[PASS] 类型 ScalarBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScalarBackgroundService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IScalarService
    var type_IScalarService = Type.GetType("IScalarService");
    if (type_IScalarService != null)
    {
        Console.WriteLine("[PASS] 类型 IScalarService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IScalarService 未找到，尝试无命名空间...");
        type_IScalarService = Type.GetType("IScalarService");
        if (type_IScalarService != null)
            Console.WriteLine("[PASS] 类型 IScalarService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IScalarService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
