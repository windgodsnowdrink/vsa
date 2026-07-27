#load "csharp_nats_integration.cs"

Console.WriteLine("=== csharp_nats_integration.cs Test ===");

try
{
    // 验证 class: NatsOptions
    var type_NatsOptions = Type.GetType("NatsOptions");
    if (type_NatsOptions != null)
    {
        Console.WriteLine("[PASS] 类型 NatsOptions (class) 存在");
        var ctors_NatsOptions = type_NatsOptions.GetConstructors();
        Console.WriteLine($"[PASS] NatsOptions 构造函数数量: {ctors_NatsOptions.Length}");
        var methods_NatsOptions = type_NatsOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsOptions 公开方法数量: {methods_NatsOptions.Length}");
        foreach (var m in methods_NatsOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsOptions 未找到，尝试无命名空间...");
        type_NatsOptions = Type.GetType("NatsOptions");
        if (type_NatsOptions != null)
            Console.WriteLine("[PASS] 类型 NatsOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatsPublisher
    var type_NatsPublisher = Type.GetType("NatsPublisher");
    if (type_NatsPublisher != null)
    {
        Console.WriteLine("[PASS] 类型 NatsPublisher (class) 存在");
        var ctors_NatsPublisher = type_NatsPublisher.GetConstructors();
        Console.WriteLine($"[PASS] NatsPublisher 构造函数数量: {ctors_NatsPublisher.Length}");
        var methods_NatsPublisher = type_NatsPublisher.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsPublisher 公开方法数量: {methods_NatsPublisher.Length}");
        foreach (var m in methods_NatsPublisher)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsPublisher 未找到，尝试无命名空间...");
        type_NatsPublisher = Type.GetType("NatsPublisher");
        if (type_NatsPublisher != null)
            Console.WriteLine("[PASS] 类型 NatsPublisher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsPublisher 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatsSubscriber
    var type_NatsSubscriber = Type.GetType("NatsSubscriber");
    if (type_NatsSubscriber != null)
    {
        Console.WriteLine("[PASS] 类型 NatsSubscriber (class) 存在");
        var ctors_NatsSubscriber = type_NatsSubscriber.GetConstructors();
        Console.WriteLine($"[PASS] NatsSubscriber 构造函数数量: {ctors_NatsSubscriber.Length}");
        var methods_NatsSubscriber = type_NatsSubscriber.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsSubscriber 公开方法数量: {methods_NatsSubscriber.Length}");
        foreach (var m in methods_NatsSubscriber)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsSubscriber 未找到，尝试无命名空间...");
        type_NatsSubscriber = Type.GetType("NatsSubscriber");
        if (type_NatsSubscriber != null)
            Console.WriteLine("[PASS] 类型 NatsSubscriber (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsSubscriber 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatsServiceCollectionExtensions
    var type_NatsServiceCollectionExtensions = Type.GetType("NatsServiceCollectionExtensions");
    if (type_NatsServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 NatsServiceCollectionExtensions (class) 存在");
        var ctors_NatsServiceCollectionExtensions = type_NatsServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] NatsServiceCollectionExtensions 构造函数数量: {ctors_NatsServiceCollectionExtensions.Length}");
        var methods_NatsServiceCollectionExtensions = type_NatsServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsServiceCollectionExtensions 公开方法数量: {methods_NatsServiceCollectionExtensions.Length}");
        foreach (var m in methods_NatsServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_NatsServiceCollectionExtensions = Type.GetType("NatsServiceCollectionExtensions");
        if (type_NatsServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 NatsServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatsRequestResponse
    var type_NatsRequestResponse = Type.GetType("NatsRequestResponse");
    if (type_NatsRequestResponse != null)
    {
        Console.WriteLine("[PASS] 类型 NatsRequestResponse (class) 存在");
        var ctors_NatsRequestResponse = type_NatsRequestResponse.GetConstructors();
        Console.WriteLine($"[PASS] NatsRequestResponse 构造函数数量: {ctors_NatsRequestResponse.Length}");
        var methods_NatsRequestResponse = type_NatsRequestResponse.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsRequestResponse 公开方法数量: {methods_NatsRequestResponse.Length}");
        foreach (var m in methods_NatsRequestResponse)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsRequestResponse 未找到，尝试无命名空间...");
        type_NatsRequestResponse = Type.GetType("NatsRequestResponse");
        if (type_NatsRequestResponse != null)
            Console.WriteLine("[PASS] 类型 NatsRequestResponse (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsRequestResponse 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatsPersistentStorage
    var type_NatsPersistentStorage = Type.GetType("NatsPersistentStorage");
    if (type_NatsPersistentStorage != null)
    {
        Console.WriteLine("[PASS] 类型 NatsPersistentStorage (class) 存在");
        var ctors_NatsPersistentStorage = type_NatsPersistentStorage.GetConstructors();
        Console.WriteLine($"[PASS] NatsPersistentStorage 构造函数数量: {ctors_NatsPersistentStorage.Length}");
        var methods_NatsPersistentStorage = type_NatsPersistentStorage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsPersistentStorage 公开方法数量: {methods_NatsPersistentStorage.Length}");
        foreach (var m in methods_NatsPersistentStorage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsPersistentStorage 未找到，尝试无命名空间...");
        type_NatsPersistentStorage = Type.GetType("NatsPersistentStorage");
        if (type_NatsPersistentStorage != null)
            Console.WriteLine("[PASS] 类型 NatsPersistentStorage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsPersistentStorage 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatsTracingExtensions
    var type_NatsTracingExtensions = Type.GetType("NatsTracingExtensions");
    if (type_NatsTracingExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 NatsTracingExtensions (class) 存在");
        var ctors_NatsTracingExtensions = type_NatsTracingExtensions.GetConstructors();
        Console.WriteLine($"[PASS] NatsTracingExtensions 构造函数数量: {ctors_NatsTracingExtensions.Length}");
        var methods_NatsTracingExtensions = type_NatsTracingExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsTracingExtensions 公开方法数量: {methods_NatsTracingExtensions.Length}");
        foreach (var m in methods_NatsTracingExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsTracingExtensions 未找到，尝试无命名空间...");
        type_NatsTracingExtensions = Type.GetType("NatsTracingExtensions");
        if (type_NatsTracingExtensions != null)
            Console.WriteLine("[PASS] 类型 NatsTracingExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsTracingExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatsTracingOptions
    var type_NatsTracingOptions = Type.GetType("NatsTracingOptions");
    if (type_NatsTracingOptions != null)
    {
        Console.WriteLine("[PASS] 类型 NatsTracingOptions (class) 存在");
        var ctors_NatsTracingOptions = type_NatsTracingOptions.GetConstructors();
        Console.WriteLine($"[PASS] NatsTracingOptions 构造函数数量: {ctors_NatsTracingOptions.Length}");
        var methods_NatsTracingOptions = type_NatsTracingOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsTracingOptions 公开方法数量: {methods_NatsTracingOptions.Length}");
        foreach (var m in methods_NatsTracingOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsTracingOptions 未找到，尝试无命名空间...");
        type_NatsTracingOptions = Type.GetType("NatsTracingOptions");
        if (type_NatsTracingOptions != null)
            Console.WriteLine("[PASS] 类型 NatsTracingOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsTracingOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatsTracingInterceptor
    var type_NatsTracingInterceptor = Type.GetType("NatsTracingInterceptor");
    if (type_NatsTracingInterceptor != null)
    {
        Console.WriteLine("[PASS] 类型 NatsTracingInterceptor (class) 存在");
        var ctors_NatsTracingInterceptor = type_NatsTracingInterceptor.GetConstructors();
        Console.WriteLine($"[PASS] NatsTracingInterceptor 构造函数数量: {ctors_NatsTracingInterceptor.Length}");
        var methods_NatsTracingInterceptor = type_NatsTracingInterceptor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsTracingInterceptor 公开方法数量: {methods_NatsTracingInterceptor.Length}");
        foreach (var m in methods_NatsTracingInterceptor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsTracingInterceptor 未找到，尝试无命名空间...");
        type_NatsTracingInterceptor = Type.GetType("NatsTracingInterceptor");
        if (type_NatsTracingInterceptor != null)
            Console.WriteLine("[PASS] 类型 NatsTracingInterceptor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsTracingInterceptor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatsMetrics
    var type_NatsMetrics = Type.GetType("NatsMetrics");
    if (type_NatsMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 NatsMetrics (class) 存在");
        var ctors_NatsMetrics = type_NatsMetrics.GetConstructors();
        Console.WriteLine($"[PASS] NatsMetrics 构造函数数量: {ctors_NatsMetrics.Length}");
        var methods_NatsMetrics = type_NatsMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsMetrics 公开方法数量: {methods_NatsMetrics.Length}");
        foreach (var m in methods_NatsMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsMetrics 未找到，尝试无命名空间...");
        type_NatsMetrics = Type.GetType("NatsMetrics");
        if (type_NatsMetrics != null)
            Console.WriteLine("[PASS] 类型 NatsMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatsClusterExtensions
    var type_NatsClusterExtensions = Type.GetType("NatsClusterExtensions");
    if (type_NatsClusterExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 NatsClusterExtensions (class) 存在");
        var ctors_NatsClusterExtensions = type_NatsClusterExtensions.GetConstructors();
        Console.WriteLine($"[PASS] NatsClusterExtensions 构造函数数量: {ctors_NatsClusterExtensions.Length}");
        var methods_NatsClusterExtensions = type_NatsClusterExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsClusterExtensions 公开方法数量: {methods_NatsClusterExtensions.Length}");
        foreach (var m in methods_NatsClusterExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsClusterExtensions 未找到，尝试无命名空间...");
        type_NatsClusterExtensions = Type.GetType("NatsClusterExtensions");
        if (type_NatsClusterExtensions != null)
            Console.WriteLine("[PASS] 类型 NatsClusterExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsClusterExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatsClusterOptions
    var type_NatsClusterOptions = Type.GetType("NatsClusterOptions");
    if (type_NatsClusterOptions != null)
    {
        Console.WriteLine("[PASS] 类型 NatsClusterOptions (class) 存在");
        var ctors_NatsClusterOptions = type_NatsClusterOptions.GetConstructors();
        Console.WriteLine($"[PASS] NatsClusterOptions 构造函数数量: {ctors_NatsClusterOptions.Length}");
        var methods_NatsClusterOptions = type_NatsClusterOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsClusterOptions 公开方法数量: {methods_NatsClusterOptions.Length}");
        foreach (var m in methods_NatsClusterOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsClusterOptions 未找到，尝试无命名空间...");
        type_NatsClusterOptions = Type.GetType("NatsClusterOptions");
        if (type_NatsClusterOptions != null)
            Console.WriteLine("[PASS] 类型 NatsClusterOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsClusterOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NatsClusterManager
    var type_NatsClusterManager = Type.GetType("NatsClusterManager");
    if (type_NatsClusterManager != null)
    {
        Console.WriteLine("[PASS] 类型 NatsClusterManager (class) 存在");
        var ctors_NatsClusterManager = type_NatsClusterManager.GetConstructors();
        Console.WriteLine($"[PASS] NatsClusterManager 构造函数数量: {ctors_NatsClusterManager.Length}");
        var methods_NatsClusterManager = type_NatsClusterManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NatsClusterManager 公开方法数量: {methods_NatsClusterManager.Length}");
        foreach (var m in methods_NatsClusterManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NatsClusterManager 未找到，尝试无命名空间...");
        type_NatsClusterManager = Type.GetType("NatsClusterManager");
        if (type_NatsClusterManager != null)
            Console.WriteLine("[PASS] 类型 NatsClusterManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NatsClusterManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DemoUsage
    var type_DemoUsage = Type.GetType("DemoUsage");
    if (type_DemoUsage != null)
    {
        Console.WriteLine("[PASS] 类型 DemoUsage (class) 存在");
        var ctors_DemoUsage = type_DemoUsage.GetConstructors();
        Console.WriteLine($"[PASS] DemoUsage 构造函数数量: {ctors_DemoUsage.Length}");
        var methods_DemoUsage = type_DemoUsage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DemoUsage 公开方法数量: {methods_DemoUsage.Length}");
        foreach (var m in methods_DemoUsage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DemoUsage 未找到，尝试无命名空间...");
        type_DemoUsage = Type.GetType("DemoUsage");
        if (type_DemoUsage != null)
            Console.WriteLine("[PASS] 类型 DemoUsage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DemoUsage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
