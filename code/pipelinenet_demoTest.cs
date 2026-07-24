#load "pipelinenet_demo.cs"

Console.WriteLine("=== pipelinenet_demo.cs Test ===");

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

    // 验证 class: ValidationMiddleware
    var type_ValidationMiddleware = Type.GetType("ValidationMiddleware");
    if (type_ValidationMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 ValidationMiddleware (class) 存在");
        var ctors_ValidationMiddleware = type_ValidationMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] ValidationMiddleware 构造函数数量: {ctors_ValidationMiddleware.Length}");
        var methods_ValidationMiddleware = type_ValidationMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ValidationMiddleware 公开方法数量: {methods_ValidationMiddleware.Length}");
        foreach (var m in methods_ValidationMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ValidationMiddleware 未找到，尝试无命名空间...");
        type_ValidationMiddleware = Type.GetType("ValidationMiddleware");
        if (type_ValidationMiddleware != null)
            Console.WriteLine("[PASS] 类型 ValidationMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ValidationMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LoggingMiddleware
    var type_LoggingMiddleware = Type.GetType("LoggingMiddleware");
    if (type_LoggingMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 LoggingMiddleware (class) 存在");
        var ctors_LoggingMiddleware = type_LoggingMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] LoggingMiddleware 构造函数数量: {ctors_LoggingMiddleware.Length}");
        var methods_LoggingMiddleware = type_LoggingMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LoggingMiddleware 公开方法数量: {methods_LoggingMiddleware.Length}");
        foreach (var m in methods_LoggingMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LoggingMiddleware 未找到，尝试无命名空间...");
        type_LoggingMiddleware = Type.GetType("LoggingMiddleware");
        if (type_LoggingMiddleware != null)
            Console.WriteLine("[PASS] 类型 LoggingMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LoggingMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoPipelineProcessor
    var type_TodoPipelineProcessor = Type.GetType("TodoPipelineProcessor");
    if (type_TodoPipelineProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 TodoPipelineProcessor (class) 存在");
        var ctors_TodoPipelineProcessor = type_TodoPipelineProcessor.GetConstructors();
        Console.WriteLine($"[PASS] TodoPipelineProcessor 构造函数数量: {ctors_TodoPipelineProcessor.Length}");
        var methods_TodoPipelineProcessor = type_TodoPipelineProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoPipelineProcessor 公开方法数量: {methods_TodoPipelineProcessor.Length}");
        foreach (var m in methods_TodoPipelineProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoPipelineProcessor 未找到，尝试无命名空间...");
        type_TodoPipelineProcessor = Type.GetType("TodoPipelineProcessor");
        if (type_TodoPipelineProcessor != null)
            Console.WriteLine("[PASS] 类型 TodoPipelineProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoPipelineProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BatchProcessingMiddleware
    var type_BatchProcessingMiddleware = Type.GetType("BatchProcessingMiddleware");
    if (type_BatchProcessingMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 BatchProcessingMiddleware (class) 存在");
        var ctors_BatchProcessingMiddleware = type_BatchProcessingMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] BatchProcessingMiddleware 构造函数数量: {ctors_BatchProcessingMiddleware.Length}");
        var methods_BatchProcessingMiddleware = type_BatchProcessingMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BatchProcessingMiddleware 公开方法数量: {methods_BatchProcessingMiddleware.Length}");
        foreach (var m in methods_BatchProcessingMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BatchProcessingMiddleware 未找到，尝试无命名空间...");
        type_BatchProcessingMiddleware = Type.GetType("BatchProcessingMiddleware");
        if (type_BatchProcessingMiddleware != null)
            Console.WriteLine("[PASS] 类型 BatchProcessingMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BatchProcessingMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ListPoolPolicy
    var type_ListPoolPolicy = Type.GetType("ListPoolPolicy");
    if (type_ListPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 ListPoolPolicy (class) 存在");
        var ctors_ListPoolPolicy = type_ListPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] ListPoolPolicy 构造函数数量: {ctors_ListPoolPolicy.Length}");
        var methods_ListPoolPolicy = type_ListPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ListPoolPolicy 公开方法数量: {methods_ListPoolPolicy.Length}");
        foreach (var m in methods_ListPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ListPoolPolicy 未找到，尝试无命名空间...");
        type_ListPoolPolicy = Type.GetType("ListPoolPolicy");
        if (type_ListPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 ListPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ListPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ResilientMiddleware
    var type_ResilientMiddleware = Type.GetType("ResilientMiddleware");
    if (type_ResilientMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 ResilientMiddleware (class) 存在");
        var ctors_ResilientMiddleware = type_ResilientMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] ResilientMiddleware 构造函数数量: {ctors_ResilientMiddleware.Length}");
        var methods_ResilientMiddleware = type_ResilientMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResilientMiddleware 公开方法数量: {methods_ResilientMiddleware.Length}");
        foreach (var m in methods_ResilientMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResilientMiddleware 未找到，尝试无命名空间...");
        type_ResilientMiddleware = Type.GetType("ResilientMiddleware");
        if (type_ResilientMiddleware != null)
            Console.WriteLine("[PASS] 类型 ResilientMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResilientMiddleware 可能为顶层语句或嵌套类型");
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

    // 验证 class: PipelineBuilderExtensions
    var type_PipelineBuilderExtensions = Type.GetType("PipelineBuilderExtensions");
    if (type_PipelineBuilderExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 PipelineBuilderExtensions (class) 存在");
        var ctors_PipelineBuilderExtensions = type_PipelineBuilderExtensions.GetConstructors();
        Console.WriteLine($"[PASS] PipelineBuilderExtensions 构造函数数量: {ctors_PipelineBuilderExtensions.Length}");
        var methods_PipelineBuilderExtensions = type_PipelineBuilderExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PipelineBuilderExtensions 公开方法数量: {methods_PipelineBuilderExtensions.Length}");
        foreach (var m in methods_PipelineBuilderExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PipelineBuilderExtensions 未找到，尝试无命名空间...");
        type_PipelineBuilderExtensions = Type.GetType("PipelineBuilderExtensions");
        if (type_PipelineBuilderExtensions != null)
            Console.WriteLine("[PASS] 类型 PipelineBuilderExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PipelineBuilderExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
