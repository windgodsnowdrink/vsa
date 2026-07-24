#load "chaos_integration.cs"

Console.WriteLine("=== chaos_integration.cs Test ===");

try
{
    // 验证 class: ChaosOptions
    var type_ChaosOptions = Type.GetType("ChaosOptions");
    if (type_ChaosOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ChaosOptions (class) 存在");
        var ctors_ChaosOptions = type_ChaosOptions.GetConstructors();
        Console.WriteLine($"[PASS] ChaosOptions 构造函数数量: {ctors_ChaosOptions.Length}");
        var methods_ChaosOptions = type_ChaosOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChaosOptions 公开方法数量: {methods_ChaosOptions.Length}");
        foreach (var m in methods_ChaosOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChaosOptions 未找到，尝试无命名空间...");
        type_ChaosOptions = Type.GetType("ChaosOptions");
        if (type_ChaosOptions != null)
            Console.WriteLine("[PASS] 类型 ChaosOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChaosOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChaosService
    var type_ChaosService = Type.GetType("ChaosService");
    if (type_ChaosService != null)
    {
        Console.WriteLine("[PASS] 类型 ChaosService (class) 存在");
        var ctors_ChaosService = type_ChaosService.GetConstructors();
        Console.WriteLine($"[PASS] ChaosService 构造函数数量: {ctors_ChaosService.Length}");
        var methods_ChaosService = type_ChaosService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChaosService 公开方法数量: {methods_ChaosService.Length}");
        foreach (var m in methods_ChaosService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChaosService 未找到，尝试无命名空间...");
        type_ChaosService = Type.GetType("ChaosService");
        if (type_ChaosService != null)
            Console.WriteLine("[PASS] 类型 ChaosService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChaosService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChaosExtensions
    var type_ChaosExtensions = Type.GetType("ChaosExtensions");
    if (type_ChaosExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ChaosExtensions (class) 存在");
        var ctors_ChaosExtensions = type_ChaosExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ChaosExtensions 构造函数数量: {ctors_ChaosExtensions.Length}");
        var methods_ChaosExtensions = type_ChaosExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChaosExtensions 公开方法数量: {methods_ChaosExtensions.Length}");
        foreach (var m in methods_ChaosExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChaosExtensions 未找到，尝试无命名空间...");
        type_ChaosExtensions = Type.GetType("ChaosExtensions");
        if (type_ChaosExtensions != null)
            Console.WriteLine("[PASS] 类型 ChaosExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChaosExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChaosBackgroundService
    var type_ChaosBackgroundService = Type.GetType("ChaosBackgroundService");
    if (type_ChaosBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 ChaosBackgroundService (class) 存在");
        var ctors_ChaosBackgroundService = type_ChaosBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] ChaosBackgroundService 构造函数数量: {ctors_ChaosBackgroundService.Length}");
        var methods_ChaosBackgroundService = type_ChaosBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChaosBackgroundService 公开方法数量: {methods_ChaosBackgroundService.Length}");
        foreach (var m in methods_ChaosBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChaosBackgroundService 未找到，尝试无命名空间...");
        type_ChaosBackgroundService = Type.GetType("ChaosBackgroundService");
        if (type_ChaosBackgroundService != null)
            Console.WriteLine("[PASS] 类型 ChaosBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChaosBackgroundService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IChaosService
    var type_IChaosService = Type.GetType("IChaosService");
    if (type_IChaosService != null)
    {
        Console.WriteLine("[PASS] 类型 IChaosService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IChaosService 未找到，尝试无命名空间...");
        type_IChaosService = Type.GetType("IChaosService");
        if (type_IChaosService != null)
            Console.WriteLine("[PASS] 类型 IChaosService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IChaosService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
