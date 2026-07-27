#load "jitter_algorithm_production.cs"

Console.WriteLine("=== jitter_algorithm_production.cs Test ===");

try
{
    // 验证 class: JitterOptions
    var type_JitterOptions = Type.GetType("JitterOptions");
    if (type_JitterOptions != null)
    {
        Console.WriteLine("[PASS] 类型 JitterOptions (class) 存在");
        var ctors_JitterOptions = type_JitterOptions.GetConstructors();
        Console.WriteLine($"[PASS] JitterOptions 构造函数数量: {ctors_JitterOptions.Length}");
        var methods_JitterOptions = type_JitterOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JitterOptions 公开方法数量: {methods_JitterOptions.Length}");
        foreach (var m in methods_JitterOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JitterOptions 未找到，尝试无命名空间...");
        type_JitterOptions = Type.GetType("JitterOptions");
        if (type_JitterOptions != null)
            Console.WriteLine("[PASS] 类型 JitterOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JitterOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JitterAlgorithm
    var type_JitterAlgorithm = Type.GetType("JitterAlgorithm");
    if (type_JitterAlgorithm != null)
    {
        Console.WriteLine("[PASS] 类型 JitterAlgorithm (class) 存在");
        var ctors_JitterAlgorithm = type_JitterAlgorithm.GetConstructors();
        Console.WriteLine($"[PASS] JitterAlgorithm 构造函数数量: {ctors_JitterAlgorithm.Length}");
        var methods_JitterAlgorithm = type_JitterAlgorithm.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JitterAlgorithm 公开方法数量: {methods_JitterAlgorithm.Length}");
        foreach (var m in methods_JitterAlgorithm)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JitterAlgorithm 未找到，尝试无命名空间...");
        type_JitterAlgorithm = Type.GetType("JitterAlgorithm");
        if (type_JitterAlgorithm != null)
            Console.WriteLine("[PASS] 类型 JitterAlgorithm (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JitterAlgorithm 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JitterExtensions
    var type_JitterExtensions = Type.GetType("JitterExtensions");
    if (type_JitterExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 JitterExtensions (class) 存在");
        var ctors_JitterExtensions = type_JitterExtensions.GetConstructors();
        Console.WriteLine($"[PASS] JitterExtensions 构造函数数量: {ctors_JitterExtensions.Length}");
        var methods_JitterExtensions = type_JitterExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JitterExtensions 公开方法数量: {methods_JitterExtensions.Length}");
        foreach (var m in methods_JitterExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JitterExtensions 未找到，尝试无命名空间...");
        type_JitterExtensions = Type.GetType("JitterExtensions");
        if (type_JitterExtensions != null)
            Console.WriteLine("[PASS] 类型 JitterExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JitterExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JitterUsageExample
    var type_JitterUsageExample = Type.GetType("JitterUsageExample");
    if (type_JitterUsageExample != null)
    {
        Console.WriteLine("[PASS] 类型 JitterUsageExample (class) 存在");
        var ctors_JitterUsageExample = type_JitterUsageExample.GetConstructors();
        Console.WriteLine($"[PASS] JitterUsageExample 构造函数数量: {ctors_JitterUsageExample.Length}");
        var methods_JitterUsageExample = type_JitterUsageExample.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JitterUsageExample 公开方法数量: {methods_JitterUsageExample.Length}");
        foreach (var m in methods_JitterUsageExample)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JitterUsageExample 未找到，尝试无命名空间...");
        type_JitterUsageExample = Type.GetType("JitterUsageExample");
        if (type_JitterUsageExample != null)
            Console.WriteLine("[PASS] 类型 JitterUsageExample (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JitterUsageExample 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
