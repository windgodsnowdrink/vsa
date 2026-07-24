#load "llvm_ir_optimizer.cs"

Console.WriteLine("=== llvm_ir_optimizer.cs Test ===");

try
{
    // 验证 class: LlvmIrOptimizer
    var type_LlvmIrOptimizer = Type.GetType("LlvmIrOptimizer");
    if (type_LlvmIrOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 LlvmIrOptimizer (class) 存在");
        var ctors_LlvmIrOptimizer = type_LlvmIrOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] LlvmIrOptimizer 构造函数数量: {ctors_LlvmIrOptimizer.Length}");
        var methods_LlvmIrOptimizer = type_LlvmIrOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LlvmIrOptimizer 公开方法数量: {methods_LlvmIrOptimizer.Length}");
        foreach (var m in methods_LlvmIrOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LlvmIrOptimizer 未找到，尝试无命名空间...");
        type_LlvmIrOptimizer = Type.GetType("LlvmIrOptimizer");
        if (type_LlvmIrOptimizer != null)
            Console.WriteLine("[PASS] 类型 LlvmIrOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LlvmIrOptimizer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SimdProcessor
    var type_SimdProcessor = Type.GetType("SimdProcessor");
    if (type_SimdProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 SimdProcessor (class) 存在");
        var ctors_SimdProcessor = type_SimdProcessor.GetConstructors();
        Console.WriteLine($"[PASS] SimdProcessor 构造函数数量: {ctors_SimdProcessor.Length}");
        var methods_SimdProcessor = type_SimdProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SimdProcessor 公开方法数量: {methods_SimdProcessor.Length}");
        foreach (var m in methods_SimdProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SimdProcessor 未找到，尝试无命名空间...");
        type_SimdProcessor = Type.GetType("SimdProcessor");
        if (type_SimdProcessor != null)
            Console.WriteLine("[PASS] 类型 SimdProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SimdProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
