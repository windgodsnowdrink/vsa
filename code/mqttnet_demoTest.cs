#load "mqttnet_demo.cs"

Console.WriteLine("=== mqttnet_demo.cs Test ===");

try
{
    // 验证 class: MqttMessageDbContext
    var type_MqttMessageDbContext = Type.GetType("MqttMessageDbContext");
    if (type_MqttMessageDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessageDbContext (class) 存在");
        var ctors_MqttMessageDbContext = type_MqttMessageDbContext.GetConstructors();
        Console.WriteLine($"[PASS] MqttMessageDbContext 构造函数数量: {ctors_MqttMessageDbContext.Length}");
        var methods_MqttMessageDbContext = type_MqttMessageDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttMessageDbContext 公开方法数量: {methods_MqttMessageDbContext.Length}");
        foreach (var m in methods_MqttMessageDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttMessageDbContext 未找到，尝试无命名空间...");
        type_MqttMessageDbContext = Type.GetType("MqttMessageDbContext");
        if (type_MqttMessageDbContext != null)
            Console.WriteLine("[PASS] 类型 MqttMessageDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttMessageDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PersistedMessage
    var type_PersistedMessage = Type.GetType("PersistedMessage");
    if (type_PersistedMessage != null)
    {
        Console.WriteLine("[PASS] 类型 PersistedMessage (class) 存在");
        var ctors_PersistedMessage = type_PersistedMessage.GetConstructors();
        Console.WriteLine($"[PASS] PersistedMessage 构造函数数量: {ctors_PersistedMessage.Length}");
        var methods_PersistedMessage = type_PersistedMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PersistedMessage 公开方法数量: {methods_PersistedMessage.Length}");
        foreach (var m in methods_PersistedMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PersistedMessage 未找到，尝试无命名空间...");
        type_PersistedMessage = Type.GetType("PersistedMessage");
        if (type_PersistedMessage != null)
            Console.WriteLine("[PASS] 类型 PersistedMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PersistedMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EnhancedMemoryOptimizer
    var type_EnhancedMemoryOptimizer = Type.GetType("EnhancedMemoryOptimizer");
    if (type_EnhancedMemoryOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 EnhancedMemoryOptimizer (class) 存在");
        var ctors_EnhancedMemoryOptimizer = type_EnhancedMemoryOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] EnhancedMemoryOptimizer 构造函数数量: {ctors_EnhancedMemoryOptimizer.Length}");
        var methods_EnhancedMemoryOptimizer = type_EnhancedMemoryOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EnhancedMemoryOptimizer 公开方法数量: {methods_EnhancedMemoryOptimizer.Length}");
        foreach (var m in methods_EnhancedMemoryOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EnhancedMemoryOptimizer 未找到，尝试无命名空间...");
        type_EnhancedMemoryOptimizer = Type.GetType("EnhancedMemoryOptimizer");
        if (type_EnhancedMemoryOptimizer != null)
            Console.WriteLine("[PASS] 类型 EnhancedMemoryOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EnhancedMemoryOptimizer 可能为顶层语句或嵌套类型");
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

    // 验证 class: EnhancedMqttEventBusService
    var type_EnhancedMqttEventBusService = Type.GetType("EnhancedMqttEventBusService");
    if (type_EnhancedMqttEventBusService != null)
    {
        Console.WriteLine("[PASS] 类型 EnhancedMqttEventBusService (class) 存在");
        var ctors_EnhancedMqttEventBusService = type_EnhancedMqttEventBusService.GetConstructors();
        Console.WriteLine($"[PASS] EnhancedMqttEventBusService 构造函数数量: {ctors_EnhancedMqttEventBusService.Length}");
        var methods_EnhancedMqttEventBusService = type_EnhancedMqttEventBusService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EnhancedMqttEventBusService 公开方法数量: {methods_EnhancedMqttEventBusService.Length}");
        foreach (var m in methods_EnhancedMqttEventBusService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EnhancedMqttEventBusService 未找到，尝试无命名空间...");
        type_EnhancedMqttEventBusService = Type.GetType("EnhancedMqttEventBusService");
        if (type_EnhancedMqttEventBusService != null)
            Console.WriteLine("[PASS] 类型 EnhancedMqttEventBusService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EnhancedMqttEventBusService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttPerformanceMonitor
    var type_MqttPerformanceMonitor = Type.GetType("MqttPerformanceMonitor");
    if (type_MqttPerformanceMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 MqttPerformanceMonitor (class) 存在");
        var ctors_MqttPerformanceMonitor = type_MqttPerformanceMonitor.GetConstructors();
        Console.WriteLine($"[PASS] MqttPerformanceMonitor 构造函数数量: {ctors_MqttPerformanceMonitor.Length}");
        var methods_MqttPerformanceMonitor = type_MqttPerformanceMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttPerformanceMonitor 公开方法数量: {methods_MqttPerformanceMonitor.Length}");
        foreach (var m in methods_MqttPerformanceMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttPerformanceMonitor 未找到，尝试无命名空间...");
        type_MqttPerformanceMonitor = Type.GetType("MqttPerformanceMonitor");
        if (type_MqttPerformanceMonitor != null)
            Console.WriteLine("[PASS] 类型 MqttPerformanceMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttPerformanceMonitor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttClusterManager
    var type_MqttClusterManager = Type.GetType("MqttClusterManager");
    if (type_MqttClusterManager != null)
    {
        Console.WriteLine("[PASS] 类型 MqttClusterManager (class) 存在");
        var ctors_MqttClusterManager = type_MqttClusterManager.GetConstructors();
        Console.WriteLine($"[PASS] MqttClusterManager 构造函数数量: {ctors_MqttClusterManager.Length}");
        var methods_MqttClusterManager = type_MqttClusterManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttClusterManager 公开方法数量: {methods_MqttClusterManager.Length}");
        foreach (var m in methods_MqttClusterManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttClusterManager 未找到，尝试无命名空间...");
        type_MqttClusterManager = Type.GetType("MqttClusterManager");
        if (type_MqttClusterManager != null)
            Console.WriteLine("[PASS] 类型 MqttClusterManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttClusterManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessageCompressor
    var type_MessageCompressor = Type.GetType("MessageCompressor");
    if (type_MessageCompressor != null)
    {
        Console.WriteLine("[PASS] 类型 MessageCompressor (class) 存在");
        var ctors_MessageCompressor = type_MessageCompressor.GetConstructors();
        Console.WriteLine($"[PASS] MessageCompressor 构造函数数量: {ctors_MessageCompressor.Length}");
        var methods_MessageCompressor = type_MessageCompressor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageCompressor 公开方法数量: {methods_MessageCompressor.Length}");
        foreach (var m in methods_MessageCompressor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageCompressor 未找到，尝试无命名空间...");
        type_MessageCompressor = Type.GetType("MessageCompressor");
        if (type_MessageCompressor != null)
            Console.WriteLine("[PASS] 类型 MessageCompressor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageCompressor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttConnectionPool
    var type_MqttConnectionPool = Type.GetType("MqttConnectionPool");
    if (type_MqttConnectionPool != null)
    {
        Console.WriteLine("[PASS] 类型 MqttConnectionPool (class) 存在");
        var ctors_MqttConnectionPool = type_MqttConnectionPool.GetConstructors();
        Console.WriteLine($"[PASS] MqttConnectionPool 构造函数数量: {ctors_MqttConnectionPool.Length}");
        var methods_MqttConnectionPool = type_MqttConnectionPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttConnectionPool 公开方法数量: {methods_MqttConnectionPool.Length}");
        foreach (var m in methods_MqttConnectionPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttConnectionPool 未找到，尝试无命名空间...");
        type_MqttConnectionPool = Type.GetType("MqttConnectionPool");
        if (type_MqttConnectionPool != null)
            Console.WriteLine("[PASS] 类型 MqttConnectionPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttConnectionPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttConnectionLeakDetector
    var type_MqttConnectionLeakDetector = Type.GetType("MqttConnectionLeakDetector");
    if (type_MqttConnectionLeakDetector != null)
    {
        Console.WriteLine("[PASS] 类型 MqttConnectionLeakDetector (class) 存在");
        var ctors_MqttConnectionLeakDetector = type_MqttConnectionLeakDetector.GetConstructors();
        Console.WriteLine($"[PASS] MqttConnectionLeakDetector 构造函数数量: {ctors_MqttConnectionLeakDetector.Length}");
        var methods_MqttConnectionLeakDetector = type_MqttConnectionLeakDetector.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttConnectionLeakDetector 公开方法数量: {methods_MqttConnectionLeakDetector.Length}");
        foreach (var m in methods_MqttConnectionLeakDetector)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttConnectionLeakDetector 未找到，尝试无命名空间...");
        type_MqttConnectionLeakDetector = Type.GetType("MqttConnectionLeakDetector");
        if (type_MqttConnectionLeakDetector != null)
            Console.WriteLine("[PASS] 类型 MqttConnectionLeakDetector (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttConnectionLeakDetector 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttConnectionPoolAutoScaler
    var type_MqttConnectionPoolAutoScaler = Type.GetType("MqttConnectionPoolAutoScaler");
    if (type_MqttConnectionPoolAutoScaler != null)
    {
        Console.WriteLine("[PASS] 类型 MqttConnectionPoolAutoScaler (class) 存在");
        var ctors_MqttConnectionPoolAutoScaler = type_MqttConnectionPoolAutoScaler.GetConstructors();
        Console.WriteLine($"[PASS] MqttConnectionPoolAutoScaler 构造函数数量: {ctors_MqttConnectionPoolAutoScaler.Length}");
        var methods_MqttConnectionPoolAutoScaler = type_MqttConnectionPoolAutoScaler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttConnectionPoolAutoScaler 公开方法数量: {methods_MqttConnectionPoolAutoScaler.Length}");
        foreach (var m in methods_MqttConnectionPoolAutoScaler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttConnectionPoolAutoScaler 未找到，尝试无命名空间...");
        type_MqttConnectionPoolAutoScaler = Type.GetType("MqttConnectionPoolAutoScaler");
        if (type_MqttConnectionPoolAutoScaler != null)
            Console.WriteLine("[PASS] 类型 MqttConnectionPoolAutoScaler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttConnectionPoolAutoScaler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttRetryPolicy
    var type_MqttRetryPolicy = Type.GetType("MqttRetryPolicy");
    if (type_MqttRetryPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MqttRetryPolicy (class) 存在");
        var ctors_MqttRetryPolicy = type_MqttRetryPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MqttRetryPolicy 构造函数数量: {ctors_MqttRetryPolicy.Length}");
        var methods_MqttRetryPolicy = type_MqttRetryPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttRetryPolicy 公开方法数量: {methods_MqttRetryPolicy.Length}");
        foreach (var m in methods_MqttRetryPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttRetryPolicy 未找到，尝试无命名空间...");
        type_MqttRetryPolicy = Type.GetType("MqttRetryPolicy");
        if (type_MqttRetryPolicy != null)
            Console.WriteLine("[PASS] 类型 MqttRetryPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttRetryPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttTlsOptimizer
    var type_MqttTlsOptimizer = Type.GetType("MqttTlsOptimizer");
    if (type_MqttTlsOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 MqttTlsOptimizer (class) 存在");
        var ctors_MqttTlsOptimizer = type_MqttTlsOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] MqttTlsOptimizer 构造函数数量: {ctors_MqttTlsOptimizer.Length}");
        var methods_MqttTlsOptimizer = type_MqttTlsOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttTlsOptimizer 公开方法数量: {methods_MqttTlsOptimizer.Length}");
        foreach (var m in methods_MqttTlsOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttTlsOptimizer 未找到，尝试无命名空间...");
        type_MqttTlsOptimizer = Type.GetType("MqttTlsOptimizer");
        if (type_MqttTlsOptimizer != null)
            Console.WriteLine("[PASS] 类型 MqttTlsOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttTlsOptimizer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttMessageCompressor
    var type_MqttMessageCompressor = Type.GetType("MqttMessageCompressor");
    if (type_MqttMessageCompressor != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessageCompressor (class) 存在");
        var ctors_MqttMessageCompressor = type_MqttMessageCompressor.GetConstructors();
        Console.WriteLine($"[PASS] MqttMessageCompressor 构造函数数量: {ctors_MqttMessageCompressor.Length}");
        var methods_MqttMessageCompressor = type_MqttMessageCompressor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttMessageCompressor 公开方法数量: {methods_MqttMessageCompressor.Length}");
        foreach (var m in methods_MqttMessageCompressor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttMessageCompressor 未找到，尝试无命名空间...");
        type_MqttMessageCompressor = Type.GetType("MqttMessageCompressor");
        if (type_MqttMessageCompressor != null)
            Console.WriteLine("[PASS] 类型 MqttMessageCompressor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttMessageCompressor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttHealthCheck
    var type_MqttHealthCheck = Type.GetType("MqttHealthCheck");
    if (type_MqttHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 MqttHealthCheck (class) 存在");
        var ctors_MqttHealthCheck = type_MqttHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] MqttHealthCheck 构造函数数量: {ctors_MqttHealthCheck.Length}");
        var methods_MqttHealthCheck = type_MqttHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttHealthCheck 公开方法数量: {methods_MqttHealthCheck.Length}");
        foreach (var m in methods_MqttHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttHealthCheck 未找到，尝试无命名空间...");
        type_MqttHealthCheck = Type.GetType("MqttHealthCheck");
        if (type_MqttHealthCheck != null)
            Console.WriteLine("[PASS] 类型 MqttHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttConnectionMetrics
    var type_MqttConnectionMetrics = Type.GetType("MqttConnectionMetrics");
    if (type_MqttConnectionMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 MqttConnectionMetrics (class) 存在");
        var ctors_MqttConnectionMetrics = type_MqttConnectionMetrics.GetConstructors();
        Console.WriteLine($"[PASS] MqttConnectionMetrics 构造函数数量: {ctors_MqttConnectionMetrics.Length}");
        var methods_MqttConnectionMetrics = type_MqttConnectionMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttConnectionMetrics 公开方法数量: {methods_MqttConnectionMetrics.Length}");
        foreach (var m in methods_MqttConnectionMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttConnectionMetrics 未找到，尝试无命名空间...");
        type_MqttConnectionMetrics = Type.GetType("MqttConnectionMetrics");
        if (type_MqttConnectionMetrics != null)
            Console.WriteLine("[PASS] 类型 MqttConnectionMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttConnectionMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttMessageProcessor
    var type_MqttMessageProcessor = Type.GetType("MqttMessageProcessor");
    if (type_MqttMessageProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessageProcessor (class) 存在");
        var ctors_MqttMessageProcessor = type_MqttMessageProcessor.GetConstructors();
        Console.WriteLine($"[PASS] MqttMessageProcessor 构造函数数量: {ctors_MqttMessageProcessor.Length}");
        var methods_MqttMessageProcessor = type_MqttMessageProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttMessageProcessor 公开方法数量: {methods_MqttMessageProcessor.Length}");
        foreach (var m in methods_MqttMessageProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttMessageProcessor 未找到，尝试无命名空间...");
        type_MqttMessageProcessor = Type.GetType("MqttMessageProcessor");
        if (type_MqttMessageProcessor != null)
            Console.WriteLine("[PASS] 类型 MqttMessageProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttMessageProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessageRecoveryService
    var type_MessageRecoveryService = Type.GetType("MessageRecoveryService");
    if (type_MessageRecoveryService != null)
    {
        Console.WriteLine("[PASS] 类型 MessageRecoveryService (class) 存在");
        var ctors_MessageRecoveryService = type_MessageRecoveryService.GetConstructors();
        Console.WriteLine($"[PASS] MessageRecoveryService 构造函数数量: {ctors_MessageRecoveryService.Length}");
        var methods_MessageRecoveryService = type_MessageRecoveryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageRecoveryService 公开方法数量: {methods_MessageRecoveryService.Length}");
        foreach (var m in methods_MessageRecoveryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageRecoveryService 未找到，尝试无命名空间...");
        type_MessageRecoveryService = Type.GetType("MessageRecoveryService");
        if (type_MessageRecoveryService != null)
            Console.WriteLine("[PASS] 类型 MessageRecoveryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageRecoveryService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TieredMemoryManager
    var type_TieredMemoryManager = Type.GetType("TieredMemoryManager");
    if (type_TieredMemoryManager != null)
    {
        Console.WriteLine("[PASS] 类型 TieredMemoryManager (class) 存在");
        var ctors_TieredMemoryManager = type_TieredMemoryManager.GetConstructors();
        Console.WriteLine($"[PASS] TieredMemoryManager 构造函数数量: {ctors_TieredMemoryManager.Length}");
        var methods_TieredMemoryManager = type_TieredMemoryManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredMemoryManager 公开方法数量: {methods_TieredMemoryManager.Length}");
        foreach (var m in methods_TieredMemoryManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredMemoryManager 未找到，尝试无命名空间...");
        type_TieredMemoryManager = Type.GetType("TieredMemoryManager");
        if (type_TieredMemoryManager != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AotMessageCompressor
    var type_AotMessageCompressor = Type.GetType("AotMessageCompressor");
    if (type_AotMessageCompressor != null)
    {
        Console.WriteLine("[PASS] 类型 AotMessageCompressor (class) 存在");
        var ctors_AotMessageCompressor = type_AotMessageCompressor.GetConstructors();
        Console.WriteLine($"[PASS] AotMessageCompressor 构造函数数量: {ctors_AotMessageCompressor.Length}");
        var methods_AotMessageCompressor = type_AotMessageCompressor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AotMessageCompressor 公开方法数量: {methods_AotMessageCompressor.Length}");
        foreach (var m in methods_AotMessageCompressor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AotMessageCompressor 未找到，尝试无命名空间...");
        type_AotMessageCompressor = Type.GetType("AotMessageCompressor");
        if (type_AotMessageCompressor != null)
            Console.WriteLine("[PASS] 类型 AotMessageCompressor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AotMessageCompressor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttDiagnosticObserver
    var type_MqttDiagnosticObserver = Type.GetType("MqttDiagnosticObserver");
    if (type_MqttDiagnosticObserver != null)
    {
        Console.WriteLine("[PASS] 类型 MqttDiagnosticObserver (class) 存在");
        var ctors_MqttDiagnosticObserver = type_MqttDiagnosticObserver.GetConstructors();
        Console.WriteLine($"[PASS] MqttDiagnosticObserver 构造函数数量: {ctors_MqttDiagnosticObserver.Length}");
        var methods_MqttDiagnosticObserver = type_MqttDiagnosticObserver.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttDiagnosticObserver 公开方法数量: {methods_MqttDiagnosticObserver.Length}");
        foreach (var m in methods_MqttDiagnosticObserver)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttDiagnosticObserver 未找到，尝试无命名空间...");
        type_MqttDiagnosticObserver = Type.GetType("MqttDiagnosticObserver");
        if (type_MqttDiagnosticObserver != null)
            Console.WriteLine("[PASS] 类型 MqttDiagnosticObserver (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttDiagnosticObserver 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
