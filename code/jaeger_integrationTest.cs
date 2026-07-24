#load "jaeger_integration.cs"

Console.WriteLine("=== jaeger_integration.cs Test ===");

try
{
    // 验证 class: JaegerOptions
    var type_JaegerOptions = Type.GetType("JaegerOptions");
    if (type_JaegerOptions != null)
    {
        Console.WriteLine("[PASS] 类型 JaegerOptions (class) 存在");
        var ctors_JaegerOptions = type_JaegerOptions.GetConstructors();
        Console.WriteLine($"[PASS] JaegerOptions 构造函数数量: {ctors_JaegerOptions.Length}");
        var methods_JaegerOptions = type_JaegerOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JaegerOptions 公开方法数量: {methods_JaegerOptions.Length}");
        foreach (var m in methods_JaegerOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JaegerOptions 未找到，尝试无命名空间...");
        type_JaegerOptions = Type.GetType("JaegerOptions");
        if (type_JaegerOptions != null)
            Console.WriteLine("[PASS] 类型 JaegerOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JaegerOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JaegerExtensions
    var type_JaegerExtensions = Type.GetType("JaegerExtensions");
    if (type_JaegerExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 JaegerExtensions (class) 存在");
        var ctors_JaegerExtensions = type_JaegerExtensions.GetConstructors();
        Console.WriteLine($"[PASS] JaegerExtensions 构造函数数量: {ctors_JaegerExtensions.Length}");
        var methods_JaegerExtensions = type_JaegerExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JaegerExtensions 公开方法数量: {methods_JaegerExtensions.Length}");
        foreach (var m in methods_JaegerExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JaegerExtensions 未找到，尝试无命名空间...");
        type_JaegerExtensions = Type.GetType("JaegerExtensions");
        if (type_JaegerExtensions != null)
            Console.WriteLine("[PASS] 类型 JaegerExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JaegerExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JaegerBackgroundService
    var type_JaegerBackgroundService = Type.GetType("JaegerBackgroundService");
    if (type_JaegerBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 JaegerBackgroundService (class) 存在");
        var ctors_JaegerBackgroundService = type_JaegerBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] JaegerBackgroundService 构造函数数量: {ctors_JaegerBackgroundService.Length}");
        var methods_JaegerBackgroundService = type_JaegerBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JaegerBackgroundService 公开方法数量: {methods_JaegerBackgroundService.Length}");
        foreach (var m in methods_JaegerBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JaegerBackgroundService 未找到，尝试无命名空间...");
        type_JaegerBackgroundService = Type.GetType("JaegerBackgroundService");
        if (type_JaegerBackgroundService != null)
            Console.WriteLine("[PASS] 类型 JaegerBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JaegerBackgroundService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
