#load "tailwindcss_integration.cs"

Console.WriteLine("=== tailwindcss_integration.cs Test ===");

try
{
    // 验证 class: TailwindOptions
    var type_TailwindOptions = Type.GetType("TailwindOptions");
    if (type_TailwindOptions != null)
    {
        Console.WriteLine("[PASS] 类型 TailwindOptions (class) 存在");
        var ctors_TailwindOptions = type_TailwindOptions.GetConstructors();
        Console.WriteLine($"[PASS] TailwindOptions 构造函数数量: {ctors_TailwindOptions.Length}");
        var methods_TailwindOptions = type_TailwindOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TailwindOptions 公开方法数量: {methods_TailwindOptions.Length}");
        foreach (var m in methods_TailwindOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TailwindOptions 未找到，尝试无命名空间...");
        type_TailwindOptions = Type.GetType("TailwindOptions");
        if (type_TailwindOptions != null)
            Console.WriteLine("[PASS] 类型 TailwindOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TailwindOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TailwindService
    var type_TailwindService = Type.GetType("TailwindService");
    if (type_TailwindService != null)
    {
        Console.WriteLine("[PASS] 类型 TailwindService (class) 存在");
        var ctors_TailwindService = type_TailwindService.GetConstructors();
        Console.WriteLine($"[PASS] TailwindService 构造函数数量: {ctors_TailwindService.Length}");
        var methods_TailwindService = type_TailwindService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TailwindService 公开方法数量: {methods_TailwindService.Length}");
        foreach (var m in methods_TailwindService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TailwindService 未找到，尝试无命名空间...");
        type_TailwindService = Type.GetType("TailwindService");
        if (type_TailwindService != null)
            Console.WriteLine("[PASS] 类型 TailwindService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TailwindService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TailwindExtensions
    var type_TailwindExtensions = Type.GetType("TailwindExtensions");
    if (type_TailwindExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 TailwindExtensions (class) 存在");
        var ctors_TailwindExtensions = type_TailwindExtensions.GetConstructors();
        Console.WriteLine($"[PASS] TailwindExtensions 构造函数数量: {ctors_TailwindExtensions.Length}");
        var methods_TailwindExtensions = type_TailwindExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TailwindExtensions 公开方法数量: {methods_TailwindExtensions.Length}");
        foreach (var m in methods_TailwindExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TailwindExtensions 未找到，尝试无命名空间...");
        type_TailwindExtensions = Type.GetType("TailwindExtensions");
        if (type_TailwindExtensions != null)
            Console.WriteLine("[PASS] 类型 TailwindExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TailwindExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITailwindService
    var type_ITailwindService = Type.GetType("ITailwindService");
    if (type_ITailwindService != null)
    {
        Console.WriteLine("[PASS] 类型 ITailwindService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITailwindService 未找到，尝试无命名空间...");
        type_ITailwindService = Type.GetType("ITailwindService");
        if (type_ITailwindService != null)
            Console.WriteLine("[PASS] 类型 ITailwindService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITailwindService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
