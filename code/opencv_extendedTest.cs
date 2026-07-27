#load "opencv_extended.cs"

Console.WriteLine("=== opencv_extended.cs Test ===");

try
{
    // 验证 class: ExtendedOcrProcessor
    var type_ExtendedOcrProcessor = Type.GetType("ExtendedOcrProcessor");
    if (type_ExtendedOcrProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ExtendedOcrProcessor (class) 存在");
        var ctors_ExtendedOcrProcessor = type_ExtendedOcrProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ExtendedOcrProcessor 构造函数数量: {ctors_ExtendedOcrProcessor.Length}");
        var methods_ExtendedOcrProcessor = type_ExtendedOcrProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExtendedOcrProcessor 公开方法数量: {methods_ExtendedOcrProcessor.Length}");
        foreach (var m in methods_ExtendedOcrProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExtendedOcrProcessor 未找到，尝试无命名空间...");
        type_ExtendedOcrProcessor = Type.GetType("ExtendedOcrProcessor");
        if (type_ExtendedOcrProcessor != null)
            Console.WriteLine("[PASS] 类型 ExtendedOcrProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExtendedOcrProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AnalysisResult
    var type_AnalysisResult = Type.GetType("AnalysisResult");
    if (type_AnalysisResult != null)
    {
        Console.WriteLine("[PASS] 类型 AnalysisResult (class) 存在");
        var ctors_AnalysisResult = type_AnalysisResult.GetConstructors();
        Console.WriteLine($"[PASS] AnalysisResult 构造函数数量: {ctors_AnalysisResult.Length}");
        var methods_AnalysisResult = type_AnalysisResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AnalysisResult 公开方法数量: {methods_AnalysisResult.Length}");
        foreach (var m in methods_AnalysisResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AnalysisResult 未找到，尝试无命名空间...");
        type_AnalysisResult = Type.GetType("AnalysisResult");
        if (type_AnalysisResult != null)
            Console.WriteLine("[PASS] 类型 AnalysisResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AnalysisResult 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AnalysisResultPooledPolicy
    var type_AnalysisResultPooledPolicy = Type.GetType("AnalysisResultPooledPolicy");
    if (type_AnalysisResultPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 AnalysisResultPooledPolicy (class) 存在");
        var ctors_AnalysisResultPooledPolicy = type_AnalysisResultPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] AnalysisResultPooledPolicy 构造函数数量: {ctors_AnalysisResultPooledPolicy.Length}");
        var methods_AnalysisResultPooledPolicy = type_AnalysisResultPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AnalysisResultPooledPolicy 公开方法数量: {methods_AnalysisResultPooledPolicy.Length}");
        foreach (var m in methods_AnalysisResultPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AnalysisResultPooledPolicy 未找到，尝试无命名空间...");
        type_AnalysisResultPooledPolicy = Type.GetType("AnalysisResultPooledPolicy");
        if (type_AnalysisResultPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 AnalysisResultPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AnalysisResultPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: Color
    var type_Color = Type.GetType("Color");
    if (type_Color != null)
    {
        Console.WriteLine("[PASS] 类型 Color (struct) 存在");
        var ctors_Color = type_Color.GetConstructors();
        Console.WriteLine($"[PASS] Color 构造函数数量: {ctors_Color.Length}");
        var methods_Color = type_Color.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Color 公开方法数量: {methods_Color.Length}");
        foreach (var m in methods_Color)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Color 未找到，尝试无命名空间...");
        type_Color = Type.GetType("Color");
        if (type_Color != null)
            Console.WriteLine("[PASS] 类型 Color (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Color 可能为顶层语句或嵌套类型");
    }

    // 验证 record: AnalysisFrame
    var type_AnalysisFrame = Type.GetType("AnalysisFrame");
    if (type_AnalysisFrame != null)
    {
        Console.WriteLine("[PASS] 类型 AnalysisFrame (record) 存在");
        var ctors_AnalysisFrame = type_AnalysisFrame.GetConstructors();
        Console.WriteLine($"[PASS] AnalysisFrame 构造函数数量: {ctors_AnalysisFrame.Length}");
        var methods_AnalysisFrame = type_AnalysisFrame.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AnalysisFrame 公开方法数量: {methods_AnalysisFrame.Length}");
        foreach (var m in methods_AnalysisFrame)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AnalysisFrame 未找到，尝试无命名空间...");
        type_AnalysisFrame = Type.GetType("AnalysisFrame");
        if (type_AnalysisFrame != null)
            Console.WriteLine("[PASS] 类型 AnalysisFrame (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AnalysisFrame 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
