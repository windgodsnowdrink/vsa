#load "high_frequency_10m.cs"

Console.WriteLine("=== high_frequency_10m.cs Test ===");

try
{
    // 验证 class: MessageEventHandler
    var type_MessageEventHandler = Type.GetType("MessageEventHandler");
    if (type_MessageEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 MessageEventHandler (class) 存在");
        var ctors_MessageEventHandler = type_MessageEventHandler.GetConstructors();
        Console.WriteLine($"[PASS] MessageEventHandler 构造函数数量: {ctors_MessageEventHandler.Length}");
        var methods_MessageEventHandler = type_MessageEventHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageEventHandler 公开方法数量: {methods_MessageEventHandler.Length}");
        foreach (var m in methods_MessageEventHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageEventHandler 未找到，尝试无命名空间...");
        type_MessageEventHandler = Type.GetType("MessageEventHandler");
        if (type_MessageEventHandler != null)
            Console.WriteLine("[PASS] 类型 MessageEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageEventHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LlvmJitCompiler
    var type_LlvmJitCompiler = Type.GetType("LlvmJitCompiler");
    if (type_LlvmJitCompiler != null)
    {
        Console.WriteLine("[PASS] 类型 LlvmJitCompiler (class) 存在");
        var ctors_LlvmJitCompiler = type_LlvmJitCompiler.GetConstructors();
        Console.WriteLine($"[PASS] LlvmJitCompiler 构造函数数量: {ctors_LlvmJitCompiler.Length}");
        var methods_LlvmJitCompiler = type_LlvmJitCompiler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LlvmJitCompiler 公开方法数量: {methods_LlvmJitCompiler.Length}");
        foreach (var m in methods_LlvmJitCompiler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LlvmJitCompiler 未找到，尝试无命名空间...");
        type_LlvmJitCompiler = Type.GetType("LlvmJitCompiler");
        if (type_LlvmJitCompiler != null)
            Console.WriteLine("[PASS] 类型 LlvmJitCompiler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LlvmJitCompiler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TradingService
    var type_TradingService = Type.GetType("TradingService");
    if (type_TradingService != null)
    {
        Console.WriteLine("[PASS] 类型 TradingService (class) 存在");
        var ctors_TradingService = type_TradingService.GetConstructors();
        Console.WriteLine($"[PASS] TradingService 构造函数数量: {ctors_TradingService.Length}");
        var methods_TradingService = type_TradingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TradingService 公开方法数量: {methods_TradingService.Length}");
        foreach (var m in methods_TradingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TradingService 未找到，尝试无命名空间...");
        type_TradingService = Type.GetType("TradingService");
        if (type_TradingService != null)
            Console.WriteLine("[PASS] 类型 TradingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TradingService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ClusterMessageProcessor
    var type_ClusterMessageProcessor = Type.GetType("ClusterMessageProcessor");
    if (type_ClusterMessageProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ClusterMessageProcessor (class) 存在");
        var ctors_ClusterMessageProcessor = type_ClusterMessageProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ClusterMessageProcessor 构造函数数量: {ctors_ClusterMessageProcessor.Length}");
        var methods_ClusterMessageProcessor = type_ClusterMessageProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ClusterMessageProcessor 公开方法数量: {methods_ClusterMessageProcessor.Length}");
        foreach (var m in methods_ClusterMessageProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ClusterMessageProcessor 未找到，尝试无命名空间...");
        type_ClusterMessageProcessor = Type.GetType("ClusterMessageProcessor");
        if (type_ClusterMessageProcessor != null)
            Console.WriteLine("[PASS] 类型 ClusterMessageProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ClusterMessageProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: MessageEvent
    var type_MessageEvent = Type.GetType("MessageEvent");
    if (type_MessageEvent != null)
    {
        Console.WriteLine("[PASS] 类型 MessageEvent (struct) 存在");
        var ctors_MessageEvent = type_MessageEvent.GetConstructors();
        Console.WriteLine($"[PASS] MessageEvent 构造函数数量: {ctors_MessageEvent.Length}");
        var methods_MessageEvent = type_MessageEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageEvent 公开方法数量: {methods_MessageEvent.Length}");
        foreach (var m in methods_MessageEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageEvent 未找到，尝试无命名空间...");
        type_MessageEvent = Type.GetType("MessageEvent");
        if (type_MessageEvent != null)
            Console.WriteLine("[PASS] 类型 MessageEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: ClusterMessage
    var type_ClusterMessage = Type.GetType("ClusterMessage");
    if (type_ClusterMessage != null)
    {
        Console.WriteLine("[PASS] 类型 ClusterMessage (struct) 存在");
        var ctors_ClusterMessage = type_ClusterMessage.GetConstructors();
        Console.WriteLine($"[PASS] ClusterMessage 构造函数数量: {ctors_ClusterMessage.Length}");
        var methods_ClusterMessage = type_ClusterMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ClusterMessage 公开方法数量: {methods_ClusterMessage.Length}");
        foreach (var m in methods_ClusterMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ClusterMessage 未找到，尝试无命名空间...");
        type_ClusterMessage = Type.GetType("ClusterMessage");
        if (type_ClusterMessage != null)
            Console.WriteLine("[PASS] 类型 ClusterMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ClusterMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: TradingOrder
    var type_TradingOrder = Type.GetType("TradingOrder");
    if (type_TradingOrder != null)
    {
        Console.WriteLine("[PASS] 类型 TradingOrder (struct) 存在");
        var ctors_TradingOrder = type_TradingOrder.GetConstructors();
        Console.WriteLine($"[PASS] TradingOrder 构造函数数量: {ctors_TradingOrder.Length}");
        var methods_TradingOrder = type_TradingOrder.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TradingOrder 公开方法数量: {methods_TradingOrder.Length}");
        foreach (var m in methods_TradingOrder)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TradingOrder 未找到，尝试无命名空间...");
        type_TradingOrder = Type.GetType("TradingOrder");
        if (type_TradingOrder != null)
            Console.WriteLine("[PASS] 类型 TradingOrder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TradingOrder 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
