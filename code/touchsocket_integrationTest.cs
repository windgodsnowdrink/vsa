#load "touchsocket_integration.cs"

Console.WriteLine("=== touchsocket_integration.cs Test ===");

try
{
    // 验证 class: TouchSocketOptions
    var type_TouchSocketOptions = Type.GetType("TouchSocketOptions");
    if (type_TouchSocketOptions != null)
    {
        Console.WriteLine("[PASS] 类型 TouchSocketOptions (class) 存在");
        var ctors_TouchSocketOptions = type_TouchSocketOptions.GetConstructors();
        Console.WriteLine($"[PASS] TouchSocketOptions 构造函数数量: {ctors_TouchSocketOptions.Length}");
        var methods_TouchSocketOptions = type_TouchSocketOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TouchSocketOptions 公开方法数量: {methods_TouchSocketOptions.Length}");
        foreach (var m in methods_TouchSocketOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TouchSocketOptions 未找到，尝试无命名空间...");
        type_TouchSocketOptions = Type.GetType("TouchSocketOptions");
        if (type_TouchSocketOptions != null)
            Console.WriteLine("[PASS] 类型 TouchSocketOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TouchSocketOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TieredMemoryConfig
    var type_TieredMemoryConfig = Type.GetType("TieredMemoryConfig");
    if (type_TieredMemoryConfig != null)
    {
        Console.WriteLine("[PASS] 类型 TieredMemoryConfig (class) 存在");
        var ctors_TieredMemoryConfig = type_TieredMemoryConfig.GetConstructors();
        Console.WriteLine($"[PASS] TieredMemoryConfig 构造函数数量: {ctors_TieredMemoryConfig.Length}");
        var methods_TieredMemoryConfig = type_TieredMemoryConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredMemoryConfig 公开方法数量: {methods_TieredMemoryConfig.Length}");
        foreach (var m in methods_TieredMemoryConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredMemoryConfig 未找到，尝试无命名空间...");
        type_TieredMemoryConfig = Type.GetType("TieredMemoryConfig");
        if (type_TieredMemoryConfig != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TouchSocketService
    var type_TouchSocketService = Type.GetType("TouchSocketService");
    if (type_TouchSocketService != null)
    {
        Console.WriteLine("[PASS] 类型 TouchSocketService (class) 存在");
        var ctors_TouchSocketService = type_TouchSocketService.GetConstructors();
        Console.WriteLine($"[PASS] TouchSocketService 构造函数数量: {ctors_TouchSocketService.Length}");
        var methods_TouchSocketService = type_TouchSocketService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TouchSocketService 公开方法数量: {methods_TouchSocketService.Length}");
        foreach (var m in methods_TouchSocketService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TouchSocketService 未找到，尝试无命名空间...");
        type_TouchSocketService = Type.GetType("TouchSocketService");
        if (type_TouchSocketService != null)
            Console.WriteLine("[PASS] 类型 TouchSocketService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TouchSocketService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TouchSocketExtensions
    var type_TouchSocketExtensions = Type.GetType("TouchSocketExtensions");
    if (type_TouchSocketExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 TouchSocketExtensions (class) 存在");
        var ctors_TouchSocketExtensions = type_TouchSocketExtensions.GetConstructors();
        Console.WriteLine($"[PASS] TouchSocketExtensions 构造函数数量: {ctors_TouchSocketExtensions.Length}");
        var methods_TouchSocketExtensions = type_TouchSocketExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TouchSocketExtensions 公开方法数量: {methods_TouchSocketExtensions.Length}");
        foreach (var m in methods_TouchSocketExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TouchSocketExtensions 未找到，尝试无命名空间...");
        type_TouchSocketExtensions = Type.GetType("TouchSocketExtensions");
        if (type_TouchSocketExtensions != null)
            Console.WriteLine("[PASS] 类型 TouchSocketExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TouchSocketExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TrafficMonitor
    var type_TrafficMonitor = Type.GetType("TrafficMonitor");
    if (type_TrafficMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 TrafficMonitor (class) 存在");
        var ctors_TrafficMonitor = type_TrafficMonitor.GetConstructors();
        Console.WriteLine($"[PASS] TrafficMonitor 构造函数数量: {ctors_TrafficMonitor.Length}");
        var methods_TrafficMonitor = type_TrafficMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TrafficMonitor 公开方法数量: {methods_TrafficMonitor.Length}");
        foreach (var m in methods_TrafficMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TrafficMonitor 未找到，尝试无命名空间...");
        type_TrafficMonitor = Type.GetType("TrafficMonitor");
        if (type_TrafficMonitor != null)
            Console.WriteLine("[PASS] 类型 TrafficMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TrafficMonitor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TrafficStats
    var type_TrafficStats = Type.GetType("TrafficStats");
    if (type_TrafficStats != null)
    {
        Console.WriteLine("[PASS] 类型 TrafficStats (class) 存在");
        var ctors_TrafficStats = type_TrafficStats.GetConstructors();
        Console.WriteLine($"[PASS] TrafficStats 构造函数数量: {ctors_TrafficStats.Length}");
        var methods_TrafficStats = type_TrafficStats.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TrafficStats 公开方法数量: {methods_TrafficStats.Length}");
        foreach (var m in methods_TrafficStats)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TrafficStats 未找到，尝试无命名空间...");
        type_TrafficStats = Type.GetType("TrafficStats");
        if (type_TrafficStats != null)
            Console.WriteLine("[PASS] 类型 TrafficStats (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TrafficStats 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TokenRingBuffer
    var type_TokenRingBuffer = Type.GetType("TokenRingBuffer");
    if (type_TokenRingBuffer != null)
    {
        Console.WriteLine("[PASS] 类型 TokenRingBuffer (class) 存在");
        var ctors_TokenRingBuffer = type_TokenRingBuffer.GetConstructors();
        Console.WriteLine($"[PASS] TokenRingBuffer 构造函数数量: {ctors_TokenRingBuffer.Length}");
        var methods_TokenRingBuffer = type_TokenRingBuffer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TokenRingBuffer 公开方法数量: {methods_TokenRingBuffer.Length}");
        foreach (var m in methods_TokenRingBuffer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TokenRingBuffer 未找到，尝试无命名空间...");
        type_TokenRingBuffer = Type.GetType("TokenRingBuffer");
        if (type_TokenRingBuffer != null)
            Console.WriteLine("[PASS] 类型 TokenRingBuffer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TokenRingBuffer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TailLatencyOptimizer
    var type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
    if (type_TailLatencyOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (class) 存在");
        var ctors_TailLatencyOptimizer = type_TailLatencyOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] TailLatencyOptimizer 构造函数数量: {ctors_TailLatencyOptimizer.Length}");
        var methods_TailLatencyOptimizer = type_TailLatencyOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TailLatencyOptimizer 公开方法数量: {methods_TailLatencyOptimizer.Length}");
        foreach (var m in methods_TailLatencyOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TailLatencyOptimizer 未找到，尝试无命名空间...");
        type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
        if (type_TailLatencyOptimizer != null)
            Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TailLatencyOptimizer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TieredMemoryService
    var type_TieredMemoryService = Type.GetType("TieredMemoryService");
    if (type_TieredMemoryService != null)
    {
        Console.WriteLine("[PASS] 类型 TieredMemoryService (class) 存在");
        var ctors_TieredMemoryService = type_TieredMemoryService.GetConstructors();
        Console.WriteLine($"[PASS] TieredMemoryService 构造函数数量: {ctors_TieredMemoryService.Length}");
        var methods_TieredMemoryService = type_TieredMemoryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TieredMemoryService 公开方法数量: {methods_TieredMemoryService.Length}");
        foreach (var m in methods_TieredMemoryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TieredMemoryService 未找到，尝试无命名空间...");
        type_TieredMemoryService = Type.GetType("TieredMemoryService");
        if (type_TieredMemoryService != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITouchSocketService
    var type_ITouchSocketService = Type.GetType("ITouchSocketService");
    if (type_ITouchSocketService != null)
    {
        Console.WriteLine("[PASS] 类型 ITouchSocketService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITouchSocketService 未找到，尝试无命名空间...");
        type_ITouchSocketService = Type.GetType("ITouchSocketService");
        if (type_ITouchSocketService != null)
            Console.WriteLine("[PASS] 类型 ITouchSocketService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITouchSocketService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITrafficMonitor
    var type_ITrafficMonitor = Type.GetType("ITrafficMonitor");
    if (type_ITrafficMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 ITrafficMonitor (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITrafficMonitor 未找到，尝试无命名空间...");
        type_ITrafficMonitor = Type.GetType("ITrafficMonitor");
        if (type_ITrafficMonitor != null)
            Console.WriteLine("[PASS] 类型 ITrafficMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITrafficMonitor 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
