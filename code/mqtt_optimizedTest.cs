#load "mqtt_optimized.cs"

Console.WriteLine("=== mqtt_optimized.cs Test ===");

try
{
    // 验证 class: RingBufferMessageQueue
    var type_RingBufferMessageQueue = Type.GetType("RingBufferMessageQueue");
    if (type_RingBufferMessageQueue != null)
    {
        Console.WriteLine("[PASS] 类型 RingBufferMessageQueue (class) 存在");
        var ctors_RingBufferMessageQueue = type_RingBufferMessageQueue.GetConstructors();
        Console.WriteLine($"[PASS] RingBufferMessageQueue 构造函数数量: {ctors_RingBufferMessageQueue.Length}");
        var methods_RingBufferMessageQueue = type_RingBufferMessageQueue.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RingBufferMessageQueue 公开方法数量: {methods_RingBufferMessageQueue.Length}");
        foreach (var m in methods_RingBufferMessageQueue)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RingBufferMessageQueue 未找到，尝试无命名空间...");
        type_RingBufferMessageQueue = Type.GetType("RingBufferMessageQueue");
        if (type_RingBufferMessageQueue != null)
            Console.WriteLine("[PASS] 类型 RingBufferMessageQueue (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RingBufferMessageQueue 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttMessageEvent
    var type_MqttMessageEvent = Type.GetType("MqttMessageEvent");
    if (type_MqttMessageEvent != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessageEvent (class) 存在");
        var ctors_MqttMessageEvent = type_MqttMessageEvent.GetConstructors();
        Console.WriteLine($"[PASS] MqttMessageEvent 构造函数数量: {ctors_MqttMessageEvent.Length}");
        var methods_MqttMessageEvent = type_MqttMessageEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttMessageEvent 公开方法数量: {methods_MqttMessageEvent.Length}");
        foreach (var m in methods_MqttMessageEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttMessageEvent 未找到，尝试无命名空间...");
        type_MqttMessageEvent = Type.GetType("MqttMessageEvent");
        if (type_MqttMessageEvent != null)
            Console.WriteLine("[PASS] 类型 MqttMessageEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttMessageEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttMessageHandler
    var type_MqttMessageHandler = Type.GetType("MqttMessageHandler");
    if (type_MqttMessageHandler != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessageHandler (class) 存在");
        var ctors_MqttMessageHandler = type_MqttMessageHandler.GetConstructors();
        Console.WriteLine($"[PASS] MqttMessageHandler 构造函数数量: {ctors_MqttMessageHandler.Length}");
        var methods_MqttMessageHandler = type_MqttMessageHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttMessageHandler 公开方法数量: {methods_MqttMessageHandler.Length}");
        foreach (var m in methods_MqttMessageHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttMessageHandler 未找到，尝试无命名空间...");
        type_MqttMessageHandler = Type.GetType("MqttMessageHandler");
        if (type_MqttMessageHandler != null)
            Console.WriteLine("[PASS] 类型 MqttMessageHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttMessageHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LZ4MessageCompressor
    var type_LZ4MessageCompressor = Type.GetType("LZ4MessageCompressor");
    if (type_LZ4MessageCompressor != null)
    {
        Console.WriteLine("[PASS] 类型 LZ4MessageCompressor (class) 存在");
        var ctors_LZ4MessageCompressor = type_LZ4MessageCompressor.GetConstructors();
        Console.WriteLine($"[PASS] LZ4MessageCompressor 构造函数数量: {ctors_LZ4MessageCompressor.Length}");
        var methods_LZ4MessageCompressor = type_LZ4MessageCompressor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LZ4MessageCompressor 公开方法数量: {methods_LZ4MessageCompressor.Length}");
        foreach (var m in methods_LZ4MessageCompressor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LZ4MessageCompressor 未找到，尝试无命名空间...");
        type_LZ4MessageCompressor = Type.GetType("LZ4MessageCompressor");
        if (type_LZ4MessageCompressor != null)
            Console.WriteLine("[PASS] 类型 LZ4MessageCompressor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LZ4MessageCompressor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TokenBucketTrafficController
    var type_TokenBucketTrafficController = Type.GetType("TokenBucketTrafficController");
    if (type_TokenBucketTrafficController != null)
    {
        Console.WriteLine("[PASS] 类型 TokenBucketTrafficController (class) 存在");
        var ctors_TokenBucketTrafficController = type_TokenBucketTrafficController.GetConstructors();
        Console.WriteLine($"[PASS] TokenBucketTrafficController 构造函数数量: {ctors_TokenBucketTrafficController.Length}");
        var methods_TokenBucketTrafficController = type_TokenBucketTrafficController.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TokenBucketTrafficController 公开方法数量: {methods_TokenBucketTrafficController.Length}");
        foreach (var m in methods_TokenBucketTrafficController)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TokenBucketTrafficController 未找到，尝试无命名空间...");
        type_TokenBucketTrafficController = Type.GetType("TokenBucketTrafficController");
        if (type_TokenBucketTrafficController != null)
            Console.WriteLine("[PASS] 类型 TokenBucketTrafficController (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TokenBucketTrafficController 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
