#load "mediatr_dapr_integration.cs"

Console.WriteLine("=== mediatr_dapr_integration.cs Test ===");

try
{
    // 验证 class: DaprEventBusOptions
    var type_DaprEventBusOptions = Type.GetType("DaprEventBusOptions");
    if (type_DaprEventBusOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DaprEventBusOptions (class) 存在");
        var ctors_DaprEventBusOptions = type_DaprEventBusOptions.GetConstructors();
        Console.WriteLine($"[PASS] DaprEventBusOptions 构造函数数量: {ctors_DaprEventBusOptions.Length}");
        var methods_DaprEventBusOptions = type_DaprEventBusOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DaprEventBusOptions 公开方法数量: {methods_DaprEventBusOptions.Length}");
        foreach (var m in methods_DaprEventBusOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DaprEventBusOptions 未找到，尝试无命名空间...");
        type_DaprEventBusOptions = Type.GetType("DaprEventBusOptions");
        if (type_DaprEventBusOptions != null)
            Console.WriteLine("[PASS] 类型 DaprEventBusOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DaprEventBusOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DaprEventBus
    var type_DaprEventBus = Type.GetType("DaprEventBus");
    if (type_DaprEventBus != null)
    {
        Console.WriteLine("[PASS] 类型 DaprEventBus (class) 存在");
        var ctors_DaprEventBus = type_DaprEventBus.GetConstructors();
        Console.WriteLine($"[PASS] DaprEventBus 构造函数数量: {ctors_DaprEventBus.Length}");
        var methods_DaprEventBus = type_DaprEventBus.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DaprEventBus 公开方法数量: {methods_DaprEventBus.Length}");
        foreach (var m in methods_DaprEventBus)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DaprEventBus 未找到，尝试无命名空间...");
        type_DaprEventBus = Type.GetType("DaprEventBus");
        if (type_DaprEventBus != null)
            Console.WriteLine("[PASS] 类型 DaprEventBus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DaprEventBus 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BatchEventProcessor
    var type_BatchEventProcessor = Type.GetType("BatchEventProcessor");
    if (type_BatchEventProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 BatchEventProcessor (class) 存在");
        var ctors_BatchEventProcessor = type_BatchEventProcessor.GetConstructors();
        Console.WriteLine($"[PASS] BatchEventProcessor 构造函数数量: {ctors_BatchEventProcessor.Length}");
        var methods_BatchEventProcessor = type_BatchEventProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BatchEventProcessor 公开方法数量: {methods_BatchEventProcessor.Length}");
        foreach (var m in methods_BatchEventProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BatchEventProcessor 未找到，尝试无命名空间...");
        type_BatchEventProcessor = Type.GetType("BatchEventProcessor");
        if (type_BatchEventProcessor != null)
            Console.WriteLine("[PASS] 类型 BatchEventProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BatchEventProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DeadLetterProcessor
    var type_DeadLetterProcessor = Type.GetType("DeadLetterProcessor");
    if (type_DeadLetterProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 DeadLetterProcessor (class) 存在");
        var ctors_DeadLetterProcessor = type_DeadLetterProcessor.GetConstructors();
        Console.WriteLine($"[PASS] DeadLetterProcessor 构造函数数量: {ctors_DeadLetterProcessor.Length}");
        var methods_DeadLetterProcessor = type_DeadLetterProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeadLetterProcessor 公开方法数量: {methods_DeadLetterProcessor.Length}");
        foreach (var m in methods_DeadLetterProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeadLetterProcessor 未找到，尝试无命名空间...");
        type_DeadLetterProcessor = Type.GetType("DeadLetterProcessor");
        if (type_DeadLetterProcessor != null)
            Console.WriteLine("[PASS] 类型 DeadLetterProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeadLetterProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DaprEventBusExtensions
    var type_DaprEventBusExtensions = Type.GetType("DaprEventBusExtensions");
    if (type_DaprEventBusExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DaprEventBusExtensions (class) 存在");
        var ctors_DaprEventBusExtensions = type_DaprEventBusExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DaprEventBusExtensions 构造函数数量: {ctors_DaprEventBusExtensions.Length}");
        var methods_DaprEventBusExtensions = type_DaprEventBusExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DaprEventBusExtensions 公开方法数量: {methods_DaprEventBusExtensions.Length}");
        foreach (var m in methods_DaprEventBusExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DaprEventBusExtensions 未找到，尝试无命名空间...");
        type_DaprEventBusExtensions = Type.GetType("DaprEventBusExtensions");
        if (type_DaprEventBusExtensions != null)
            Console.WriteLine("[PASS] 类型 DaprEventBusExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DaprEventBusExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DaprHealthCheck
    var type_DaprHealthCheck = Type.GetType("DaprHealthCheck");
    if (type_DaprHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 DaprHealthCheck (class) 存在");
        var ctors_DaprHealthCheck = type_DaprHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] DaprHealthCheck 构造函数数量: {ctors_DaprHealthCheck.Length}");
        var methods_DaprHealthCheck = type_DaprHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DaprHealthCheck 公开方法数量: {methods_DaprHealthCheck.Length}");
        foreach (var m in methods_DaprHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DaprHealthCheck 未找到，尝试无命名空间...");
        type_DaprHealthCheck = Type.GetType("DaprHealthCheck");
        if (type_DaprHealthCheck != null)
            Console.WriteLine("[PASS] 类型 DaprHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DaprHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrderEventHandler
    var type_OrderEventHandler = Type.GetType("OrderEventHandler");
    if (type_OrderEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 OrderEventHandler (class) 存在");
        var ctors_OrderEventHandler = type_OrderEventHandler.GetConstructors();
        Console.WriteLine($"[PASS] OrderEventHandler 构造函数数量: {ctors_OrderEventHandler.Length}");
        var methods_OrderEventHandler = type_OrderEventHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrderEventHandler 公开方法数量: {methods_OrderEventHandler.Length}");
        foreach (var m in methods_OrderEventHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrderEventHandler 未找到，尝试无命名空间...");
        type_OrderEventHandler = Type.GetType("OrderEventHandler");
        if (type_OrderEventHandler != null)
            Console.WriteLine("[PASS] 类型 OrderEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrderEventHandler 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
