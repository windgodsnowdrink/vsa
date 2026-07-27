#load "performance_profiler_integration.cs"

Console.WriteLine("=== performance_profiler_integration.cs Test ===");

try
{
    // 验证 class: PerformanceProfilerOptions
    var type_PerformanceProfilerOptions = Type.GetType("PerformanceProfilerOptions");
    if (type_PerformanceProfilerOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PerformanceProfilerOptions (class) 存在");
        var ctors_PerformanceProfilerOptions = type_PerformanceProfilerOptions.GetConstructors();
        Console.WriteLine($"[PASS] PerformanceProfilerOptions 构造函数数量: {ctors_PerformanceProfilerOptions.Length}");
        var methods_PerformanceProfilerOptions = type_PerformanceProfilerOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PerformanceProfilerOptions 公开方法数量: {methods_PerformanceProfilerOptions.Length}");
        foreach (var m in methods_PerformanceProfilerOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PerformanceProfilerOptions 未找到，尝试无命名空间...");
        type_PerformanceProfilerOptions = Type.GetType("PerformanceProfilerOptions");
        if (type_PerformanceProfilerOptions != null)
            Console.WriteLine("[PASS] 类型 PerformanceProfilerOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PerformanceProfilerOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DotTraceProfiler
    var type_DotTraceProfiler = Type.GetType("DotTraceProfiler");
    if (type_DotTraceProfiler != null)
    {
        Console.WriteLine("[PASS] 类型 DotTraceProfiler (class) 存在");
        var ctors_DotTraceProfiler = type_DotTraceProfiler.GetConstructors();
        Console.WriteLine($"[PASS] DotTraceProfiler 构造函数数量: {ctors_DotTraceProfiler.Length}");
        var methods_DotTraceProfiler = type_DotTraceProfiler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DotTraceProfiler 公开方法数量: {methods_DotTraceProfiler.Length}");
        foreach (var m in methods_DotTraceProfiler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotTraceProfiler 未找到，尝试无命名空间...");
        type_DotTraceProfiler = Type.GetType("DotTraceProfiler");
        if (type_DotTraceProfiler != null)
            Console.WriteLine("[PASS] 类型 DotTraceProfiler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DotTraceProfiler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PerformanceProfilerExtensions
    var type_PerformanceProfilerExtensions = Type.GetType("PerformanceProfilerExtensions");
    if (type_PerformanceProfilerExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 PerformanceProfilerExtensions (class) 存在");
        var ctors_PerformanceProfilerExtensions = type_PerformanceProfilerExtensions.GetConstructors();
        Console.WriteLine($"[PASS] PerformanceProfilerExtensions 构造函数数量: {ctors_PerformanceProfilerExtensions.Length}");
        var methods_PerformanceProfilerExtensions = type_PerformanceProfilerExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PerformanceProfilerExtensions 公开方法数量: {methods_PerformanceProfilerExtensions.Length}");
        foreach (var m in methods_PerformanceProfilerExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PerformanceProfilerExtensions 未找到，尝试无命名空间...");
        type_PerformanceProfilerExtensions = Type.GetType("PerformanceProfilerExtensions");
        if (type_PerformanceProfilerExtensions != null)
            Console.WriteLine("[PASS] 类型 PerformanceProfilerExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PerformanceProfilerExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IPerformanceProfiler
    var type_IPerformanceProfiler = Type.GetType("IPerformanceProfiler");
    if (type_IPerformanceProfiler != null)
    {
        Console.WriteLine("[PASS] 类型 IPerformanceProfiler (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IPerformanceProfiler 未找到，尝试无命名空间...");
        type_IPerformanceProfiler = Type.GetType("IPerformanceProfiler");
        if (type_IPerformanceProfiler != null)
            Console.WriteLine("[PASS] 类型 IPerformanceProfiler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPerformanceProfiler 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
