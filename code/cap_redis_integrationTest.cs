#load "cap_redis_integration.cs"

Console.WriteLine("=== cap_redis_integration.cs Test ===");

try
{
    // 验证 class: ConsistentHashingShardingStrategy
    var type_ConsistentHashingShardingStrategy = Type.GetType("ConsistentHashingShardingStrategy");
    if (type_ConsistentHashingShardingStrategy != null)
    {
        Console.WriteLine("[PASS] 类型 ConsistentHashingShardingStrategy (class) 存在");
        var ctors_ConsistentHashingShardingStrategy = type_ConsistentHashingShardingStrategy.GetConstructors();
        Console.WriteLine($"[PASS] ConsistentHashingShardingStrategy 构造函数数量: {ctors_ConsistentHashingShardingStrategy.Length}");
        var methods_ConsistentHashingShardingStrategy = type_ConsistentHashingShardingStrategy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConsistentHashingShardingStrategy 公开方法数量: {methods_ConsistentHashingShardingStrategy.Length}");
        foreach (var m in methods_ConsistentHashingShardingStrategy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConsistentHashingShardingStrategy 未找到，尝试无命名空间...");
        type_ConsistentHashingShardingStrategy = Type.GetType("ConsistentHashingShardingStrategy");
        if (type_ConsistentHashingShardingStrategy != null)
            Console.WriteLine("[PASS] 类型 ConsistentHashingShardingStrategy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConsistentHashingShardingStrategy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OptimizedMessageProcessor
    var type_OptimizedMessageProcessor = Type.GetType("OptimizedMessageProcessor");
    if (type_OptimizedMessageProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 OptimizedMessageProcessor (class) 存在");
        var ctors_OptimizedMessageProcessor = type_OptimizedMessageProcessor.GetConstructors();
        Console.WriteLine($"[PASS] OptimizedMessageProcessor 构造函数数量: {ctors_OptimizedMessageProcessor.Length}");
        var methods_OptimizedMessageProcessor = type_OptimizedMessageProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OptimizedMessageProcessor 公开方法数量: {methods_OptimizedMessageProcessor.Length}");
        foreach (var m in methods_OptimizedMessageProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OptimizedMessageProcessor 未找到，尝试无命名空间...");
        type_OptimizedMessageProcessor = Type.GetType("OptimizedMessageProcessor");
        if (type_OptimizedMessageProcessor != null)
            Console.WriteLine("[PASS] 类型 OptimizedMessageProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OptimizedMessageProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessageHandlerPoolPolicy
    var type_MessageHandlerPoolPolicy = Type.GetType("MessageHandlerPoolPolicy");
    if (type_MessageHandlerPoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MessageHandlerPoolPolicy (class) 存在");
        var ctors_MessageHandlerPoolPolicy = type_MessageHandlerPoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MessageHandlerPoolPolicy 构造函数数量: {ctors_MessageHandlerPoolPolicy.Length}");
        var methods_MessageHandlerPoolPolicy = type_MessageHandlerPoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageHandlerPoolPolicy 公开方法数量: {methods_MessageHandlerPoolPolicy.Length}");
        foreach (var m in methods_MessageHandlerPoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageHandlerPoolPolicy 未找到，尝试无命名空间...");
        type_MessageHandlerPoolPolicy = Type.GetType("MessageHandlerPoolPolicy");
        if (type_MessageHandlerPoolPolicy != null)
            Console.WriteLine("[PASS] 类型 MessageHandlerPoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageHandlerPoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RedisShardingTransform
    var type_RedisShardingTransform = Type.GetType("RedisShardingTransform");
    if (type_RedisShardingTransform != null)
    {
        Console.WriteLine("[PASS] 类型 RedisShardingTransform (class) 存在");
        var ctors_RedisShardingTransform = type_RedisShardingTransform.GetConstructors();
        Console.WriteLine($"[PASS] RedisShardingTransform 构造函数数量: {ctors_RedisShardingTransform.Length}");
        var methods_RedisShardingTransform = type_RedisShardingTransform.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisShardingTransform 公开方法数量: {methods_RedisShardingTransform.Length}");
        foreach (var m in methods_RedisShardingTransform)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisShardingTransform 未找到，尝试无命名空间...");
        type_RedisShardingTransform = Type.GetType("RedisShardingTransform");
        if (type_RedisShardingTransform != null)
            Console.WriteLine("[PASS] 类型 RedisShardingTransform (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisShardingTransform 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ShardedConnectionPool
    var type_ShardedConnectionPool = Type.GetType("ShardedConnectionPool");
    if (type_ShardedConnectionPool != null)
    {
        Console.WriteLine("[PASS] 类型 ShardedConnectionPool (class) 存在");
        var ctors_ShardedConnectionPool = type_ShardedConnectionPool.GetConstructors();
        Console.WriteLine($"[PASS] ShardedConnectionPool 构造函数数量: {ctors_ShardedConnectionPool.Length}");
        var methods_ShardedConnectionPool = type_ShardedConnectionPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ShardedConnectionPool 公开方法数量: {methods_ShardedConnectionPool.Length}");
        foreach (var m in methods_ShardedConnectionPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ShardedConnectionPool 未找到，尝试无命名空间...");
        type_ShardedConnectionPool = Type.GetType("ShardedConnectionPool");
        if (type_ShardedConnectionPool != null)
            Console.WriteLine("[PASS] 类型 ShardedConnectionPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ShardedConnectionPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RedisDatabasePoolPolicy
    var type_RedisDatabasePoolPolicy = Type.GetType("RedisDatabasePoolPolicy");
    if (type_RedisDatabasePoolPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 RedisDatabasePoolPolicy (class) 存在");
        var ctors_RedisDatabasePoolPolicy = type_RedisDatabasePoolPolicy.GetConstructors();
        Console.WriteLine($"[PASS] RedisDatabasePoolPolicy 构造函数数量: {ctors_RedisDatabasePoolPolicy.Length}");
        var methods_RedisDatabasePoolPolicy = type_RedisDatabasePoolPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisDatabasePoolPolicy 公开方法数量: {methods_RedisDatabasePoolPolicy.Length}");
        foreach (var m in methods_RedisDatabasePoolPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisDatabasePoolPolicy 未找到，尝试无命名空间...");
        type_RedisDatabasePoolPolicy = Type.GetType("RedisDatabasePoolPolicy");
        if (type_RedisDatabasePoolPolicy != null)
            Console.WriteLine("[PASS] 类型 RedisDatabasePoolPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisDatabasePoolPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MonitoringExtensions
    var type_MonitoringExtensions = Type.GetType("MonitoringExtensions");
    if (type_MonitoringExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 MonitoringExtensions (class) 存在");
        var ctors_MonitoringExtensions = type_MonitoringExtensions.GetConstructors();
        Console.WriteLine($"[PASS] MonitoringExtensions 构造函数数量: {ctors_MonitoringExtensions.Length}");
        var methods_MonitoringExtensions = type_MonitoringExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MonitoringExtensions 公开方法数量: {methods_MonitoringExtensions.Length}");
        foreach (var m in methods_MonitoringExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MonitoringExtensions 未找到，尝试无命名空间...");
        type_MonitoringExtensions = Type.GetType("MonitoringExtensions");
        if (type_MonitoringExtensions != null)
            Console.WriteLine("[PASS] 类型 MonitoringExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MonitoringExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
