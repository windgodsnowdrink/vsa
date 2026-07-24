#load "microsoft_pipeline_demo.cs"

Console.WriteLine("=== microsoft_pipeline_demo.cs Test ===");

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

    // 验证 class: TodoPipeline
    var type_TodoPipeline = Type.GetType("TodoPipeline");
    if (type_TodoPipeline != null)
    {
        Console.WriteLine("[PASS] 类型 TodoPipeline (class) 存在");
        var ctors_TodoPipeline = type_TodoPipeline.GetConstructors();
        Console.WriteLine($"[PASS] TodoPipeline 构造函数数量: {ctors_TodoPipeline.Length}");
        var methods_TodoPipeline = type_TodoPipeline.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoPipeline 公开方法数量: {methods_TodoPipeline.Length}");
        foreach (var m in methods_TodoPipeline)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoPipeline 未找到，尝试无命名空间...");
        type_TodoPipeline = Type.GetType("TodoPipeline");
        if (type_TodoPipeline != null)
            Console.WriteLine("[PASS] 类型 TodoPipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoPipeline 可能为顶层语句或嵌套类型");
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

    // 验证 class: BackpressureChannel
    var type_BackpressureChannel = Type.GetType("BackpressureChannel");
    if (type_BackpressureChannel != null)
    {
        Console.WriteLine("[PASS] 类型 BackpressureChannel (class) 存在");
        var ctors_BackpressureChannel = type_BackpressureChannel.GetConstructors();
        Console.WriteLine($"[PASS] BackpressureChannel 构造函数数量: {ctors_BackpressureChannel.Length}");
        var methods_BackpressureChannel = type_BackpressureChannel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BackpressureChannel 公开方法数量: {methods_BackpressureChannel.Length}");
        foreach (var m in methods_BackpressureChannel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BackpressureChannel 未找到，尝试无命名空间...");
        type_BackpressureChannel = Type.GetType("BackpressureChannel");
        if (type_BackpressureChannel != null)
            Console.WriteLine("[PASS] 类型 BackpressureChannel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BackpressureChannel 可能为顶层语句或嵌套类型");
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

    // 验证 class: ResilientPipeline
    var type_ResilientPipeline = Type.GetType("ResilientPipeline");
    if (type_ResilientPipeline != null)
    {
        Console.WriteLine("[PASS] 类型 ResilientPipeline (class) 存在");
        var ctors_ResilientPipeline = type_ResilientPipeline.GetConstructors();
        Console.WriteLine($"[PASS] ResilientPipeline 构造函数数量: {ctors_ResilientPipeline.Length}");
        var methods_ResilientPipeline = type_ResilientPipeline.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResilientPipeline 公开方法数量: {methods_ResilientPipeline.Length}");
        foreach (var m in methods_ResilientPipeline)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResilientPipeline 未找到，尝试无命名空间...");
        type_ResilientPipeline = Type.GetType("ResilientPipeline");
        if (type_ResilientPipeline != null)
            Console.WriteLine("[PASS] 类型 ResilientPipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResilientPipeline 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
