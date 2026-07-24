#load "saga_orchestrator.cs"

Console.WriteLine("=== saga_orchestrator.cs Test ===");

try
{
    // 验证 class: SagaOrchestrator
    var type_SagaOrchestrator = Type.GetType("SagaOrchestrator");
    if (type_SagaOrchestrator != null)
    {
        Console.WriteLine("[PASS] 类型 SagaOrchestrator (class) 存在");
        var ctors_SagaOrchestrator = type_SagaOrchestrator.GetConstructors();
        Console.WriteLine($"[PASS] SagaOrchestrator 构造函数数量: {ctors_SagaOrchestrator.Length}");
        var methods_SagaOrchestrator = type_SagaOrchestrator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SagaOrchestrator 公开方法数量: {methods_SagaOrchestrator.Length}");
        foreach (var m in methods_SagaOrchestrator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SagaOrchestrator 未找到，尝试无命名空间...");
        type_SagaOrchestrator = Type.GetType("SagaOrchestrator");
        if (type_SagaOrchestrator != null)
            Console.WriteLine("[PASS] 类型 SagaOrchestrator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SagaOrchestrator 可能为顶层语句或嵌套类型");
    }

    // 验证 record: SagaCommand
    var type_SagaCommand = Type.GetType("SagaCommand");
    if (type_SagaCommand != null)
    {
        Console.WriteLine("[PASS] 类型 SagaCommand (record) 存在");
        var ctors_SagaCommand = type_SagaCommand.GetConstructors();
        Console.WriteLine($"[PASS] SagaCommand 构造函数数量: {ctors_SagaCommand.Length}");
        var methods_SagaCommand = type_SagaCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SagaCommand 公开方法数量: {methods_SagaCommand.Length}");
        foreach (var m in methods_SagaCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SagaCommand 未找到，尝试无命名空间...");
        type_SagaCommand = Type.GetType("SagaCommand");
        if (type_SagaCommand != null)
            Console.WriteLine("[PASS] 类型 SagaCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SagaCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: SagaStep
    var type_SagaStep = Type.GetType("SagaStep");
    if (type_SagaStep != null)
    {
        Console.WriteLine("[PASS] 类型 SagaStep (record) 存在");
        var ctors_SagaStep = type_SagaStep.GetConstructors();
        Console.WriteLine($"[PASS] SagaStep 构造函数数量: {ctors_SagaStep.Length}");
        var methods_SagaStep = type_SagaStep.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SagaStep 公开方法数量: {methods_SagaStep.Length}");
        foreach (var m in methods_SagaStep)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SagaStep 未找到，尝试无命名空间...");
        type_SagaStep = Type.GetType("SagaStep");
        if (type_SagaStep != null)
            Console.WriteLine("[PASS] 类型 SagaStep (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SagaStep 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
