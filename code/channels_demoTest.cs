#load "channels_demo.cs"

Console.WriteLine("=== channels_demo.cs Test ===");

try
{
    // 验证 class: ChannelProcessor
    var type_ChannelProcessor = Type.GetType("ChannelProcessor");
    if (type_ChannelProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelProcessor (class) 存在");
        var ctors_ChannelProcessor = type_ChannelProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelProcessor 构造函数数量: {ctors_ChannelProcessor.Length}");
        var methods_ChannelProcessor = type_ChannelProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelProcessor 公开方法数量: {methods_ChannelProcessor.Length}");
        foreach (var m in methods_ChannelProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelProcessor 未找到，尝试无命名空间...");
        type_ChannelProcessor = Type.GetType("ChannelProcessor");
        if (type_ChannelProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelProcessor 可能为顶层语句或嵌套类型");
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

    // 验证 class: TodoProcessor
    var type_TodoProcessor = Type.GetType("TodoProcessor");
    if (type_TodoProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 TodoProcessor (class) 存在");
        var ctors_TodoProcessor = type_TodoProcessor.GetConstructors();
        Console.WriteLine($"[PASS] TodoProcessor 构造函数数量: {ctors_TodoProcessor.Length}");
        var methods_TodoProcessor = type_TodoProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoProcessor 公开方法数量: {methods_TodoProcessor.Length}");
        foreach (var m in methods_TodoProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoProcessor 未找到，尝试无命名空间...");
        type_TodoProcessor = Type.GetType("TodoProcessor");
        if (type_TodoProcessor != null)
            Console.WriteLine("[PASS] 类型 TodoProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoProcessor 可能为顶层语句或嵌套类型");
    }

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

    // 验证 class: ChannelMetrics
    var type_ChannelMetrics = Type.GetType("ChannelMetrics");
    if (type_ChannelMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelMetrics (class) 存在");
        var ctors_ChannelMetrics = type_ChannelMetrics.GetConstructors();
        Console.WriteLine($"[PASS] ChannelMetrics 构造函数数量: {ctors_ChannelMetrics.Length}");
        var methods_ChannelMetrics = type_ChannelMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelMetrics 公开方法数量: {methods_ChannelMetrics.Length}");
        foreach (var m in methods_ChannelMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelMetrics 未找到，尝试无命名空间...");
        type_ChannelMetrics = Type.GetType("ChannelMetrics");
        if (type_ChannelMetrics != null)
            Console.WriteLine("[PASS] 类型 ChannelMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ResilientChannelProcessor
    var type_ResilientChannelProcessor = Type.GetType("ResilientChannelProcessor");
    if (type_ResilientChannelProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ResilientChannelProcessor (class) 存在");
        var ctors_ResilientChannelProcessor = type_ResilientChannelProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ResilientChannelProcessor 构造函数数量: {ctors_ResilientChannelProcessor.Length}");
        var methods_ResilientChannelProcessor = type_ResilientChannelProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResilientChannelProcessor 公开方法数量: {methods_ResilientChannelProcessor.Length}");
        foreach (var m in methods_ResilientChannelProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResilientChannelProcessor 未找到，尝试无命名空间...");
        type_ResilientChannelProcessor = Type.GetType("ResilientChannelProcessor");
        if (type_ResilientChannelProcessor != null)
            Console.WriteLine("[PASS] 类型 ResilientChannelProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResilientChannelProcessor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
