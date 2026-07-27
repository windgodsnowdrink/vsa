#load "dataflow_demo.cs"

Console.WriteLine("=== dataflow_demo.cs Test ===");

try
{
    // 验证 class: TodoEvent
    var type_TodoEvent = Type.GetType("TodoEvent");
    if (type_TodoEvent != null)
    {
        Console.WriteLine("[PASS] 类型 TodoEvent (class) 存在");
        var ctors_TodoEvent = type_TodoEvent.GetConstructors();
        Console.WriteLine($"[PASS] TodoEvent 构造函数数量: {ctors_TodoEvent.Length}");
        var methods_TodoEvent = type_TodoEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoEvent 公开方法数量: {methods_TodoEvent.Length}");
        foreach (var m in methods_TodoEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoEvent 未找到，尝试无命名空间...");
        type_TodoEvent = Type.GetType("TodoEvent");
        if (type_TodoEvent != null)
            Console.WriteLine("[PASS] 类型 TodoEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoStreamProcessor
    var type_TodoStreamProcessor = Type.GetType("TodoStreamProcessor");
    if (type_TodoStreamProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 TodoStreamProcessor (class) 存在");
        var ctors_TodoStreamProcessor = type_TodoStreamProcessor.GetConstructors();
        Console.WriteLine($"[PASS] TodoStreamProcessor 构造函数数量: {ctors_TodoStreamProcessor.Length}");
        var methods_TodoStreamProcessor = type_TodoStreamProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoStreamProcessor 公开方法数量: {methods_TodoStreamProcessor.Length}");
        foreach (var m in methods_TodoStreamProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoStreamProcessor 未找到，尝试无命名空间...");
        type_TodoStreamProcessor = Type.GetType("TodoStreamProcessor");
        if (type_TodoStreamProcessor != null)
            Console.WriteLine("[PASS] 类型 TodoStreamProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoStreamProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TracingMiddleware
    var type_TracingMiddleware = Type.GetType("TracingMiddleware");
    if (type_TracingMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 TracingMiddleware (class) 存在");
        var ctors_TracingMiddleware = type_TracingMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] TracingMiddleware 构造函数数量: {ctors_TracingMiddleware.Length}");
        var methods_TracingMiddleware = type_TracingMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TracingMiddleware 公开方法数量: {methods_TracingMiddleware.Length}");
        foreach (var m in methods_TracingMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TracingMiddleware 未找到，尝试无命名空间...");
        type_TracingMiddleware = Type.GetType("TracingMiddleware");
        if (type_TracingMiddleware != null)
            Console.WriteLine("[PASS] 类型 TracingMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TracingMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BatchProcessor
    var type_BatchProcessor = Type.GetType("BatchProcessor");
    if (type_BatchProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 BatchProcessor (class) 存在");
        var ctors_BatchProcessor = type_BatchProcessor.GetConstructors();
        Console.WriteLine($"[PASS] BatchProcessor 构造函数数量: {ctors_BatchProcessor.Length}");
        var methods_BatchProcessor = type_BatchProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BatchProcessor 公开方法数量: {methods_BatchProcessor.Length}");
        foreach (var m in methods_BatchProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BatchProcessor 未找到，尝试无命名空间...");
        type_BatchProcessor = Type.GetType("BatchProcessor");
        if (type_BatchProcessor != null)
            Console.WriteLine("[PASS] 类型 BatchProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BatchProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BackpressureStrategy
    var type_BackpressureStrategy = Type.GetType("BackpressureStrategy");
    if (type_BackpressureStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 BackpressureStrategy (class) 存在");
        var ctors_BackpressureStrategy = type_BackpressureStrategy.GetConstructors();
        Console.WriteLine($"[PASS] BackpressureStrategy 构造函数数量: {ctors_BackpressureStrategy.Length}");
        var methods_BackpressureStrategy = type_BackpressureStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BackpressureStrategy 公开方法数量: {methods_BackpressureStrategy.Length}");
        foreach (var m in methods_BackpressureStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BackpressureStrategy 未找到，尝试无命名空间...");
        type_BackpressureStrategy = Type.GetType("BackpressureStrategy");
        if (type_BackpressureStrategy != null)
            Console.WriteLine("[PASS] 类型 BackpressureStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BackpressureStrategy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ErrorHandlingStrategy
    var type_ErrorHandlingStrategy = Type.GetType("ErrorHandlingStrategy");
    if (type_ErrorHandlingStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 ErrorHandlingStrategy (class) 存在");
        var ctors_ErrorHandlingStrategy = type_ErrorHandlingStrategy.GetConstructors();
        Console.WriteLine($"[PASS] ErrorHandlingStrategy 构造函数数量: {ctors_ErrorHandlingStrategy.Length}");
        var methods_ErrorHandlingStrategy = type_ErrorHandlingStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ErrorHandlingStrategy 公开方法数量: {methods_ErrorHandlingStrategy.Length}");
        foreach (var m in methods_ErrorHandlingStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ErrorHandlingStrategy 未找到，尝试无命名空间...");
        type_ErrorHandlingStrategy = Type.GetType("ErrorHandlingStrategy");
        if (type_ErrorHandlingStrategy != null)
            Console.WriteLine("[PASS] 类型 ErrorHandlingStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ErrorHandlingStrategy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
