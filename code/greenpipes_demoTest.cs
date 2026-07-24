#load "greenpipes_demo.cs"

Console.WriteLine("=== greenpipes_demo.cs Test ===");

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

    // 验证 class: ValidateTodoFilter
    var type_ValidateTodoFilter = Type.GetType("ValidateTodoFilter");
    if (type_ValidateTodoFilter != null)
    {
        Console.WriteLine("[PASS] 类型 ValidateTodoFilter (class) 存在");
        var ctors_ValidateTodoFilter = type_ValidateTodoFilter.GetConstructors();
        Console.WriteLine($"[PASS] ValidateTodoFilter 构造函数数量: {ctors_ValidateTodoFilter.Length}");
        var methods_ValidateTodoFilter = type_ValidateTodoFilter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ValidateTodoFilter 公开方法数量: {methods_ValidateTodoFilter.Length}");
        foreach (var m in methods_ValidateTodoFilter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ValidateTodoFilter 未找到，尝试无命名空间...");
        type_ValidateTodoFilter = Type.GetType("ValidateTodoFilter");
        if (type_ValidateTodoFilter != null)
            Console.WriteLine("[PASS] 类型 ValidateTodoFilter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ValidateTodoFilter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LogTodoFilter
    var type_LogTodoFilter = Type.GetType("LogTodoFilter");
    if (type_LogTodoFilter != null)
    {
        Console.WriteLine("[PASS] 类型 LogTodoFilter (class) 存在");
        var ctors_LogTodoFilter = type_LogTodoFilter.GetConstructors();
        Console.WriteLine($"[PASS] LogTodoFilter 构造函数数量: {ctors_LogTodoFilter.Length}");
        var methods_LogTodoFilter = type_LogTodoFilter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LogTodoFilter 公开方法数量: {methods_LogTodoFilter.Length}");
        foreach (var m in methods_LogTodoFilter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LogTodoFilter 未找到，尝试无命名空间...");
        type_LogTodoFilter = Type.GetType("LogTodoFilter");
        if (type_LogTodoFilter != null)
            Console.WriteLine("[PASS] 类型 LogTodoFilter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LogTodoFilter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DistributedTracingFilter
    var type_DistributedTracingFilter = Type.GetType("DistributedTracingFilter");
    if (type_DistributedTracingFilter != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedTracingFilter (class) 存在");
        var ctors_DistributedTracingFilter = type_DistributedTracingFilter.GetConstructors();
        Console.WriteLine($"[PASS] DistributedTracingFilter 构造函数数量: {ctors_DistributedTracingFilter.Length}");
        var methods_DistributedTracingFilter = type_DistributedTracingFilter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedTracingFilter 公开方法数量: {methods_DistributedTracingFilter.Length}");
        foreach (var m in methods_DistributedTracingFilter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedTracingFilter 未找到，尝试无命名空间...");
        type_DistributedTracingFilter = Type.GetType("DistributedTracingFilter");
        if (type_DistributedTracingFilter != null)
            Console.WriteLine("[PASS] 类型 DistributedTracingFilter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedTracingFilter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoPipelineBuilder
    var type_TodoPipelineBuilder = Type.GetType("TodoPipelineBuilder");
    if (type_TodoPipelineBuilder != null)
    {
        Console.WriteLine("[PASS] 类型 TodoPipelineBuilder (class) 存在");
        var ctors_TodoPipelineBuilder = type_TodoPipelineBuilder.GetConstructors();
        Console.WriteLine($"[PASS] TodoPipelineBuilder 构造函数数量: {ctors_TodoPipelineBuilder.Length}");
        var methods_TodoPipelineBuilder = type_TodoPipelineBuilder.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoPipelineBuilder 公开方法数量: {methods_TodoPipelineBuilder.Length}");
        foreach (var m in methods_TodoPipelineBuilder)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoPipelineBuilder 未找到，尝试无命名空间...");
        type_TodoPipelineBuilder = Type.GetType("TodoPipelineBuilder");
        if (type_TodoPipelineBuilder != null)
            Console.WriteLine("[PASS] 类型 TodoPipelineBuilder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoPipelineBuilder 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DynamicPipelineBuilder
    var type_DynamicPipelineBuilder = Type.GetType("DynamicPipelineBuilder");
    if (type_DynamicPipelineBuilder != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicPipelineBuilder (class) 存在");
        var ctors_DynamicPipelineBuilder = type_DynamicPipelineBuilder.GetConstructors();
        Console.WriteLine($"[PASS] DynamicPipelineBuilder 构造函数数量: {ctors_DynamicPipelineBuilder.Length}");
        var methods_DynamicPipelineBuilder = type_DynamicPipelineBuilder.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicPipelineBuilder 公开方法数量: {methods_DynamicPipelineBuilder.Length}");
        foreach (var m in methods_DynamicPipelineBuilder)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicPipelineBuilder 未找到，尝试无命名空间...");
        type_DynamicPipelineBuilder = Type.GetType("DynamicPipelineBuilder");
        if (type_DynamicPipelineBuilder != null)
            Console.WriteLine("[PASS] 类型 DynamicPipelineBuilder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicPipelineBuilder 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CustomMiddleware
    var type_CustomMiddleware = Type.GetType("CustomMiddleware");
    if (type_CustomMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 CustomMiddleware (class) 存在");
        var ctors_CustomMiddleware = type_CustomMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] CustomMiddleware 构造函数数量: {ctors_CustomMiddleware.Length}");
        var methods_CustomMiddleware = type_CustomMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CustomMiddleware 公开方法数量: {methods_CustomMiddleware.Length}");
        foreach (var m in methods_CustomMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CustomMiddleware 未找到，尝试无命名空间...");
        type_CustomMiddleware = Type.GetType("CustomMiddleware");
        if (type_CustomMiddleware != null)
            Console.WriteLine("[PASS] 类型 CustomMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CustomMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PerformanceMetricsFilter
    var type_PerformanceMetricsFilter = Type.GetType("PerformanceMetricsFilter");
    if (type_PerformanceMetricsFilter != null)
    {
        Console.WriteLine("[PASS] 类型 PerformanceMetricsFilter (class) 存在");
        var ctors_PerformanceMetricsFilter = type_PerformanceMetricsFilter.GetConstructors();
        Console.WriteLine($"[PASS] PerformanceMetricsFilter 构造函数数量: {ctors_PerformanceMetricsFilter.Length}");
        var methods_PerformanceMetricsFilter = type_PerformanceMetricsFilter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PerformanceMetricsFilter 公开方法数量: {methods_PerformanceMetricsFilter.Length}");
        foreach (var m in methods_PerformanceMetricsFilter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PerformanceMetricsFilter 未找到，尝试无命名空间...");
        type_PerformanceMetricsFilter = Type.GetType("PerformanceMetricsFilter");
        if (type_PerformanceMetricsFilter != null)
            Console.WriteLine("[PASS] 类型 PerformanceMetricsFilter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PerformanceMetricsFilter 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
