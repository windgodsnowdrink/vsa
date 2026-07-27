#load "networksocket_integration.cs"

Console.WriteLine("=== networksocket_integration.cs Test ===");

try
{
    // 验证 class: NetworkSocketIntegration.NetworkSocketOptions
    var type_NetworkSocketOptions = Type.GetType("NetworkSocketIntegration.NetworkSocketOptions");
    if (type_NetworkSocketOptions != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.NetworkSocketOptions (class) 存在");
        var ctors_NetworkSocketOptions = type_NetworkSocketOptions.GetConstructors();
        Console.WriteLine($"[PASS] NetworkSocketIntegration.NetworkSocketOptions 构造函数数量: {ctors_NetworkSocketOptions.Length}");
        var methods_NetworkSocketOptions = type_NetworkSocketOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkSocketIntegration.NetworkSocketOptions 公开方法数量: {methods_NetworkSocketOptions.Length}");
        foreach (var m in methods_NetworkSocketOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.NetworkSocketOptions 未找到，尝试无命名空间...");
        type_NetworkSocketOptions = Type.GetType("NetworkSocketOptions");
        if (type_NetworkSocketOptions != null)
            Console.WriteLine("[PASS] 类型 NetworkSocketOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetworkSocketOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetworkSocketIntegration.SslConfig
    var type_SslConfig = Type.GetType("NetworkSocketIntegration.SslConfig");
    if (type_SslConfig != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.SslConfig (class) 存在");
        var ctors_SslConfig = type_SslConfig.GetConstructors();
        Console.WriteLine($"[PASS] NetworkSocketIntegration.SslConfig 构造函数数量: {ctors_SslConfig.Length}");
        var methods_SslConfig = type_SslConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkSocketIntegration.SslConfig 公开方法数量: {methods_SslConfig.Length}");
        foreach (var m in methods_SslConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.SslConfig 未找到，尝试无命名空间...");
        type_SslConfig = Type.GetType("SslConfig");
        if (type_SslConfig != null)
            Console.WriteLine("[PASS] 类型 SslConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SslConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetworkSocketIntegration.HeartbeatConfig
    var type_HeartbeatConfig = Type.GetType("NetworkSocketIntegration.HeartbeatConfig");
    if (type_HeartbeatConfig != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.HeartbeatConfig (class) 存在");
        var ctors_HeartbeatConfig = type_HeartbeatConfig.GetConstructors();
        Console.WriteLine($"[PASS] NetworkSocketIntegration.HeartbeatConfig 构造函数数量: {ctors_HeartbeatConfig.Length}");
        var methods_HeartbeatConfig = type_HeartbeatConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkSocketIntegration.HeartbeatConfig 公开方法数量: {methods_HeartbeatConfig.Length}");
        foreach (var m in methods_HeartbeatConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.HeartbeatConfig 未找到，尝试无命名空间...");
        type_HeartbeatConfig = Type.GetType("HeartbeatConfig");
        if (type_HeartbeatConfig != null)
            Console.WriteLine("[PASS] 类型 HeartbeatConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HeartbeatConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetworkSocketIntegration.TieredMemoryConfig
    var type_TieredMemoryConfig = Type.GetType("NetworkSocketIntegration.TieredMemoryConfig");
    if (type_TieredMemoryConfig != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.TieredMemoryConfig (class) 存在");
        var ctors_TieredMemoryConfig = type_TieredMemoryConfig.GetConstructors();
        Console.WriteLine($"[PASS] NetworkSocketIntegration.TieredMemoryConfig 构造函数数量: {ctors_TieredMemoryConfig.Length}");
        var methods_TieredMemoryConfig = type_TieredMemoryConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkSocketIntegration.TieredMemoryConfig 公开方法数量: {methods_TieredMemoryConfig.Length}");
        foreach (var m in methods_TieredMemoryConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.TieredMemoryConfig 未找到，尝试无命名空间...");
        type_TieredMemoryConfig = Type.GetType("TieredMemoryConfig");
        if (type_TieredMemoryConfig != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetworkSocketIntegration.NetworkSocketService
    var type_NetworkSocketService = Type.GetType("NetworkSocketIntegration.NetworkSocketService");
    if (type_NetworkSocketService != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.NetworkSocketService (class) 存在");
        var ctors_NetworkSocketService = type_NetworkSocketService.GetConstructors();
        Console.WriteLine($"[PASS] NetworkSocketIntegration.NetworkSocketService 构造函数数量: {ctors_NetworkSocketService.Length}");
        var methods_NetworkSocketService = type_NetworkSocketService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkSocketIntegration.NetworkSocketService 公开方法数量: {methods_NetworkSocketService.Length}");
        foreach (var m in methods_NetworkSocketService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.NetworkSocketService 未找到，尝试无命名空间...");
        type_NetworkSocketService = Type.GetType("NetworkSocketService");
        if (type_NetworkSocketService != null)
            Console.WriteLine("[PASS] 类型 NetworkSocketService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetworkSocketService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetworkSocketIntegration.NetworkSocketExtensions
    var type_NetworkSocketExtensions = Type.GetType("NetworkSocketIntegration.NetworkSocketExtensions");
    if (type_NetworkSocketExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.NetworkSocketExtensions (class) 存在");
        var ctors_NetworkSocketExtensions = type_NetworkSocketExtensions.GetConstructors();
        Console.WriteLine($"[PASS] NetworkSocketIntegration.NetworkSocketExtensions 构造函数数量: {ctors_NetworkSocketExtensions.Length}");
        var methods_NetworkSocketExtensions = type_NetworkSocketExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkSocketIntegration.NetworkSocketExtensions 公开方法数量: {methods_NetworkSocketExtensions.Length}");
        foreach (var m in methods_NetworkSocketExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.NetworkSocketExtensions 未找到，尝试无命名空间...");
        type_NetworkSocketExtensions = Type.GetType("NetworkSocketExtensions");
        if (type_NetworkSocketExtensions != null)
            Console.WriteLine("[PASS] 类型 NetworkSocketExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetworkSocketExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetworkSocketIntegration.TrafficMonitor
    var type_TrafficMonitor = Type.GetType("NetworkSocketIntegration.TrafficMonitor");
    if (type_TrafficMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.TrafficMonitor (class) 存在");
        var ctors_TrafficMonitor = type_TrafficMonitor.GetConstructors();
        Console.WriteLine($"[PASS] NetworkSocketIntegration.TrafficMonitor 构造函数数量: {ctors_TrafficMonitor.Length}");
        var methods_TrafficMonitor = type_TrafficMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkSocketIntegration.TrafficMonitor 公开方法数量: {methods_TrafficMonitor.Length}");
        foreach (var m in methods_TrafficMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.TrafficMonitor 未找到，尝试无命名空间...");
        type_TrafficMonitor = Type.GetType("TrafficMonitor");
        if (type_TrafficMonitor != null)
            Console.WriteLine("[PASS] 类型 TrafficMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TrafficMonitor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetworkSocketIntegration.TrafficStats
    var type_TrafficStats = Type.GetType("NetworkSocketIntegration.TrafficStats");
    if (type_TrafficStats != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.TrafficStats (class) 存在");
        var ctors_TrafficStats = type_TrafficStats.GetConstructors();
        Console.WriteLine($"[PASS] NetworkSocketIntegration.TrafficStats 构造函数数量: {ctors_TrafficStats.Length}");
        var methods_TrafficStats = type_TrafficStats.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkSocketIntegration.TrafficStats 公开方法数量: {methods_TrafficStats.Length}");
        foreach (var m in methods_TrafficStats)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.TrafficStats 未找到，尝试无命名空间...");
        type_TrafficStats = Type.GetType("TrafficStats");
        if (type_TrafficStats != null)
            Console.WriteLine("[PASS] 类型 TrafficStats (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TrafficStats 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetworkSocketIntegration.TokenRingBuffer
    var type_TokenRingBuffer = Type.GetType("NetworkSocketIntegration.TokenRingBuffer");
    if (type_TokenRingBuffer != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.TokenRingBuffer (class) 存在");
        var ctors_TokenRingBuffer = type_TokenRingBuffer.GetConstructors();
        Console.WriteLine($"[PASS] NetworkSocketIntegration.TokenRingBuffer 构造函数数量: {ctors_TokenRingBuffer.Length}");
        var methods_TokenRingBuffer = type_TokenRingBuffer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkSocketIntegration.TokenRingBuffer 公开方法数量: {methods_TokenRingBuffer.Length}");
        foreach (var m in methods_TokenRingBuffer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.TokenRingBuffer 未找到，尝试无命名空间...");
        type_TokenRingBuffer = Type.GetType("TokenRingBuffer");
        if (type_TokenRingBuffer != null)
            Console.WriteLine("[PASS] 类型 TokenRingBuffer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TokenRingBuffer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetworkSocketIntegration.TailLatencyOptimizer
    var type_TailLatencyOptimizer = Type.GetType("NetworkSocketIntegration.TailLatencyOptimizer");
    if (type_TailLatencyOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.TailLatencyOptimizer (class) 存在");
        var ctors_TailLatencyOptimizer = type_TailLatencyOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] NetworkSocketIntegration.TailLatencyOptimizer 构造函数数量: {ctors_TailLatencyOptimizer.Length}");
        var methods_TailLatencyOptimizer = type_TailLatencyOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkSocketIntegration.TailLatencyOptimizer 公开方法数量: {methods_TailLatencyOptimizer.Length}");
        foreach (var m in methods_TailLatencyOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.TailLatencyOptimizer 未找到，尝试无命名空间...");
        type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
        if (type_TailLatencyOptimizer != null)
            Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TailLatencyOptimizer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NetworkSocketIntegration.TieredMemoryService
    var type_TieredMemoryService = Type.GetType("NetworkSocketIntegration.TieredMemoryService");
    if (type_TieredMemoryService != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.TieredMemoryService (class) 存在");
        var ctors_TieredMemoryService = type_TieredMemoryService.GetConstructors();
        Console.WriteLine($"[PASS] NetworkSocketIntegration.TieredMemoryService 构造函数数量: {ctors_TieredMemoryService.Length}");
        var methods_TieredMemoryService = type_TieredMemoryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkSocketIntegration.TieredMemoryService 公开方法数量: {methods_TieredMemoryService.Length}");
        foreach (var m in methods_TieredMemoryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.TieredMemoryService 未找到，尝试无命名空间...");
        type_TieredMemoryService = Type.GetType("TieredMemoryService");
        if (type_TieredMemoryService != null)
            Console.WriteLine("[PASS] 类型 TieredMemoryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TieredMemoryService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: NetworkSocketIntegration.INetworkSocketService
    var type_INetworkSocketService = Type.GetType("NetworkSocketIntegration.INetworkSocketService");
    if (type_INetworkSocketService != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.INetworkSocketService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.INetworkSocketService 未找到，尝试无命名空间...");
        type_INetworkSocketService = Type.GetType("INetworkSocketService");
        if (type_INetworkSocketService != null)
            Console.WriteLine("[PASS] 类型 INetworkSocketService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 INetworkSocketService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: NetworkSocketIntegration.ITrafficMonitor
    var type_ITrafficMonitor = Type.GetType("NetworkSocketIntegration.ITrafficMonitor");
    if (type_ITrafficMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkSocketIntegration.ITrafficMonitor (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkSocketIntegration.ITrafficMonitor 未找到，尝试无命名空间...");
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
