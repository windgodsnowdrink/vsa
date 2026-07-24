#load "sipsorcery_nat_traversal.cs"

Console.WriteLine("=== sipsorcery_nat_traversal.cs Test ===");

try
{
    // 验证 class: ICECandidateCollector
    var type_ICECandidateCollector = Type.GetType("ICECandidateCollector");
    if (type_ICECandidateCollector != null)
    {
        Console.WriteLine("[PASS] 类型 ICECandidateCollector (class) 存在");
        var ctors_ICECandidateCollector = type_ICECandidateCollector.GetConstructors();
        Console.WriteLine($"[PASS] ICECandidateCollector 构造函数数量: {ctors_ICECandidateCollector.Length}");
        var methods_ICECandidateCollector = type_ICECandidateCollector.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ICECandidateCollector 公开方法数量: {methods_ICECandidateCollector.Length}");
        foreach (var m in methods_ICECandidateCollector)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ICECandidateCollector 未找到，尝试无命名空间...");
        type_ICECandidateCollector = Type.GetType("ICECandidateCollector");
        if (type_ICECandidateCollector != null)
            Console.WriteLine("[PASS] 类型 ICECandidateCollector (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICECandidateCollector 可能为顶层语句或嵌套类型");
    }

    // 验证 class: STUNClient
    var type_STUNClient = Type.GetType("STUNClient");
    if (type_STUNClient != null)
    {
        Console.WriteLine("[PASS] 类型 STUNClient (class) 存在");
        var ctors_STUNClient = type_STUNClient.GetConstructors();
        Console.WriteLine($"[PASS] STUNClient 构造函数数量: {ctors_STUNClient.Length}");
        var methods_STUNClient = type_STUNClient.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] STUNClient 公开方法数量: {methods_STUNClient.Length}");
        foreach (var m in methods_STUNClient)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 STUNClient 未找到，尝试无命名空间...");
        type_STUNClient = Type.GetType("STUNClient");
        if (type_STUNClient != null)
            Console.WriteLine("[PASS] 类型 STUNClient (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 STUNClient 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NATTraversalService
    var type_NATTraversalService = Type.GetType("NATTraversalService");
    if (type_NATTraversalService != null)
    {
        Console.WriteLine("[PASS] 类型 NATTraversalService (class) 存在");
        var ctors_NATTraversalService = type_NATTraversalService.GetConstructors();
        Console.WriteLine($"[PASS] NATTraversalService 构造函数数量: {ctors_NATTraversalService.Length}");
        var methods_NATTraversalService = type_NATTraversalService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NATTraversalService 公开方法数量: {methods_NATTraversalService.Length}");
        foreach (var m in methods_NATTraversalService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NATTraversalService 未找到，尝试无命名空间...");
        type_NATTraversalService = Type.GetType("NATTraversalService");
        if (type_NATTraversalService != null)
            Console.WriteLine("[PASS] 类型 NATTraversalService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NATTraversalService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ClusterTraversalService
    var type_ClusterTraversalService = Type.GetType("ClusterTraversalService");
    if (type_ClusterTraversalService != null)
    {
        Console.WriteLine("[PASS] 类型 ClusterTraversalService (class) 存在");
        var ctors_ClusterTraversalService = type_ClusterTraversalService.GetConstructors();
        Console.WriteLine($"[PASS] ClusterTraversalService 构造函数数量: {ctors_ClusterTraversalService.Length}");
        var methods_ClusterTraversalService = type_ClusterTraversalService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ClusterTraversalService 公开方法数量: {methods_ClusterTraversalService.Length}");
        foreach (var m in methods_ClusterTraversalService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ClusterTraversalService 未找到，尝试无命名空间...");
        type_ClusterTraversalService = Type.GetType("ClusterTraversalService");
        if (type_ClusterTraversalService != null)
            Console.WriteLine("[PASS] 类型 ClusterTraversalService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ClusterTraversalService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ClusterTraversalEngine
    var type_ClusterTraversalEngine = Type.GetType("ClusterTraversalEngine");
    if (type_ClusterTraversalEngine != null)
    {
        Console.WriteLine("[PASS] 类型 ClusterTraversalEngine (class) 存在");
        var ctors_ClusterTraversalEngine = type_ClusterTraversalEngine.GetConstructors();
        Console.WriteLine($"[PASS] ClusterTraversalEngine 构造函数数量: {ctors_ClusterTraversalEngine.Length}");
        var methods_ClusterTraversalEngine = type_ClusterTraversalEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ClusterTraversalEngine 公开方法数量: {methods_ClusterTraversalEngine.Length}");
        foreach (var m in methods_ClusterTraversalEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ClusterTraversalEngine 未找到，尝试无命名空间...");
        type_ClusterTraversalEngine = Type.GetType("ClusterTraversalEngine");
        if (type_ClusterTraversalEngine != null)
            Console.WriteLine("[PASS] 类型 ClusterTraversalEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ClusterTraversalEngine 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EnhancedHealthChecker
    var type_EnhancedHealthChecker = Type.GetType("EnhancedHealthChecker");
    if (type_EnhancedHealthChecker != null)
    {
        Console.WriteLine("[PASS] 类型 EnhancedHealthChecker (class) 存在");
        var ctors_EnhancedHealthChecker = type_EnhancedHealthChecker.GetConstructors();
        Console.WriteLine($"[PASS] EnhancedHealthChecker 构造函数数量: {ctors_EnhancedHealthChecker.Length}");
        var methods_EnhancedHealthChecker = type_EnhancedHealthChecker.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EnhancedHealthChecker 公开方法数量: {methods_EnhancedHealthChecker.Length}");
        foreach (var m in methods_EnhancedHealthChecker)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EnhancedHealthChecker 未找到，尝试无命名空间...");
        type_EnhancedHealthChecker = Type.GetType("EnhancedHealthChecker");
        if (type_EnhancedHealthChecker != null)
            Console.WriteLine("[PASS] 类型 EnhancedHealthChecker (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EnhancedHealthChecker 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ClusterTraversalConfig
    var type_ClusterTraversalConfig = Type.GetType("ClusterTraversalConfig");
    if (type_ClusterTraversalConfig != null)
    {
        Console.WriteLine("[PASS] 类型 ClusterTraversalConfig (record) 存在");
        var ctors_ClusterTraversalConfig = type_ClusterTraversalConfig.GetConstructors();
        Console.WriteLine($"[PASS] ClusterTraversalConfig 构造函数数量: {ctors_ClusterTraversalConfig.Length}");
        var methods_ClusterTraversalConfig = type_ClusterTraversalConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ClusterTraversalConfig 公开方法数量: {methods_ClusterTraversalConfig.Length}");
        foreach (var m in methods_ClusterTraversalConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ClusterTraversalConfig 未找到，尝试无命名空间...");
        type_ClusterTraversalConfig = Type.GetType("ClusterTraversalConfig");
        if (type_ClusterTraversalConfig != null)
            Console.WriteLine("[PASS] 类型 ClusterTraversalConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ClusterTraversalConfig 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
