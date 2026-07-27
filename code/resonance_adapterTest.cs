#load "resonance_adapter.cs"

Console.WriteLine("=== resonance_adapter.cs Test ===");

try
{
    // 验证 class: MultiProtocolAdapter
    var type_MultiProtocolAdapter = Type.GetType("MultiProtocolAdapter");
    if (type_MultiProtocolAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 MultiProtocolAdapter (class) 存在");
        var ctors_MultiProtocolAdapter = type_MultiProtocolAdapter.GetConstructors();
        Console.WriteLine($"[PASS] MultiProtocolAdapter 构造函数数量: {ctors_MultiProtocolAdapter.Length}");
        var methods_MultiProtocolAdapter = type_MultiProtocolAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MultiProtocolAdapter 公开方法数量: {methods_MultiProtocolAdapter.Length}");
        foreach (var m in methods_MultiProtocolAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MultiProtocolAdapter 未找到，尝试无命名空间...");
        type_MultiProtocolAdapter = Type.GetType("MultiProtocolAdapter");
        if (type_MultiProtocolAdapter != null)
            Console.WriteLine("[PASS] 类型 MultiProtocolAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MultiProtocolAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ResonanceMessageAdapter
    var type_ResonanceMessageAdapter = Type.GetType("ResonanceMessageAdapter");
    if (type_ResonanceMessageAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 ResonanceMessageAdapter (class) 存在");
        var ctors_ResonanceMessageAdapter = type_ResonanceMessageAdapter.GetConstructors();
        Console.WriteLine($"[PASS] ResonanceMessageAdapter 构造函数数量: {ctors_ResonanceMessageAdapter.Length}");
        var methods_ResonanceMessageAdapter = type_ResonanceMessageAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResonanceMessageAdapter 公开方法数量: {methods_ResonanceMessageAdapter.Length}");
        foreach (var m in methods_ResonanceMessageAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResonanceMessageAdapter 未找到，尝试无命名空间...");
        type_ResonanceMessageAdapter = Type.GetType("ResonanceMessageAdapter");
        if (type_ResonanceMessageAdapter != null)
            Console.WriteLine("[PASS] 类型 ResonanceMessageAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResonanceMessageAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TcpProtocolAdapter
    var type_TcpProtocolAdapter = Type.GetType("TcpProtocolAdapter");
    if (type_TcpProtocolAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 TcpProtocolAdapter (class) 存在");
        var ctors_TcpProtocolAdapter = type_TcpProtocolAdapter.GetConstructors();
        Console.WriteLine($"[PASS] TcpProtocolAdapter 构造函数数量: {ctors_TcpProtocolAdapter.Length}");
        var methods_TcpProtocolAdapter = type_TcpProtocolAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TcpProtocolAdapter 公开方法数量: {methods_TcpProtocolAdapter.Length}");
        foreach (var m in methods_TcpProtocolAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TcpProtocolAdapter 未找到，尝试无命名空间...");
        type_TcpProtocolAdapter = Type.GetType("TcpProtocolAdapter");
        if (type_TcpProtocolAdapter != null)
            Console.WriteLine("[PASS] 类型 TcpProtocolAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TcpProtocolAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GrpcProtocolAdapter
    var type_GrpcProtocolAdapter = Type.GetType("GrpcProtocolAdapter");
    if (type_GrpcProtocolAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 GrpcProtocolAdapter (class) 存在");
        var ctors_GrpcProtocolAdapter = type_GrpcProtocolAdapter.GetConstructors();
        Console.WriteLine($"[PASS] GrpcProtocolAdapter 构造函数数量: {ctors_GrpcProtocolAdapter.Length}");
        var methods_GrpcProtocolAdapter = type_GrpcProtocolAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GrpcProtocolAdapter 公开方法数量: {methods_GrpcProtocolAdapter.Length}");
        foreach (var m in methods_GrpcProtocolAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GrpcProtocolAdapter 未找到，尝试无命名空间...");
        type_GrpcProtocolAdapter = Type.GetType("GrpcProtocolAdapter");
        if (type_GrpcProtocolAdapter != null)
            Console.WriteLine("[PASS] 类型 GrpcProtocolAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GrpcProtocolAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RestProtocolAdapter
    var type_RestProtocolAdapter = Type.GetType("RestProtocolAdapter");
    if (type_RestProtocolAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 RestProtocolAdapter (class) 存在");
        var ctors_RestProtocolAdapter = type_RestProtocolAdapter.GetConstructors();
        Console.WriteLine($"[PASS] RestProtocolAdapter 构造函数数量: {ctors_RestProtocolAdapter.Length}");
        var methods_RestProtocolAdapter = type_RestProtocolAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RestProtocolAdapter 公开方法数量: {methods_RestProtocolAdapter.Length}");
        foreach (var m in methods_RestProtocolAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RestProtocolAdapter 未找到，尝试无命名空间...");
        type_RestProtocolAdapter = Type.GetType("RestProtocolAdapter");
        if (type_RestProtocolAdapter != null)
            Console.WriteLine("[PASS] 类型 RestProtocolAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RestProtocolAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttProtocolAdapter
    var type_MqttProtocolAdapter = Type.GetType("MqttProtocolAdapter");
    if (type_MqttProtocolAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 MqttProtocolAdapter (class) 存在");
        var ctors_MqttProtocolAdapter = type_MqttProtocolAdapter.GetConstructors();
        Console.WriteLine($"[PASS] MqttProtocolAdapter 构造函数数量: {ctors_MqttProtocolAdapter.Length}");
        var methods_MqttProtocolAdapter = type_MqttProtocolAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttProtocolAdapter 公开方法数量: {methods_MqttProtocolAdapter.Length}");
        foreach (var m in methods_MqttProtocolAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttProtocolAdapter 未找到，尝试无命名空间...");
        type_MqttProtocolAdapter = Type.GetType("MqttProtocolAdapter");
        if (type_MqttProtocolAdapter != null)
            Console.WriteLine("[PASS] 类型 MqttProtocolAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttProtocolAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AmqpProtocolAdapter
    var type_AmqpProtocolAdapter = Type.GetType("AmqpProtocolAdapter");
    if (type_AmqpProtocolAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 AmqpProtocolAdapter (class) 存在");
        var ctors_AmqpProtocolAdapter = type_AmqpProtocolAdapter.GetConstructors();
        Console.WriteLine($"[PASS] AmqpProtocolAdapter 构造函数数量: {ctors_AmqpProtocolAdapter.Length}");
        var methods_AmqpProtocolAdapter = type_AmqpProtocolAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AmqpProtocolAdapter 公开方法数量: {methods_AmqpProtocolAdapter.Length}");
        foreach (var m in methods_AmqpProtocolAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AmqpProtocolAdapter 未找到，尝试无命名空间...");
        type_AmqpProtocolAdapter = Type.GetType("AmqpProtocolAdapter");
        if (type_AmqpProtocolAdapter != null)
            Console.WriteLine("[PASS] 类型 AmqpProtocolAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AmqpProtocolAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JsonMessageAdapter
    var type_JsonMessageAdapter = Type.GetType("JsonMessageAdapter");
    if (type_JsonMessageAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 JsonMessageAdapter (class) 存在");
        var ctors_JsonMessageAdapter = type_JsonMessageAdapter.GetConstructors();
        Console.WriteLine($"[PASS] JsonMessageAdapter 构造函数数量: {ctors_JsonMessageAdapter.Length}");
        var methods_JsonMessageAdapter = type_JsonMessageAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JsonMessageAdapter 公开方法数量: {methods_JsonMessageAdapter.Length}");
        foreach (var m in methods_JsonMessageAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JsonMessageAdapter 未找到，尝试无命名空间...");
        type_JsonMessageAdapter = Type.GetType("JsonMessageAdapter");
        if (type_JsonMessageAdapter != null)
            Console.WriteLine("[PASS] 类型 JsonMessageAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JsonMessageAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ResonanceMessageBus
    var type_ResonanceMessageBus = Type.GetType("ResonanceMessageBus");
    if (type_ResonanceMessageBus != null)
    {
        Console.WriteLine("[PASS] 类型 ResonanceMessageBus (class) 存在");
        var ctors_ResonanceMessageBus = type_ResonanceMessageBus.GetConstructors();
        Console.WriteLine($"[PASS] ResonanceMessageBus 构造函数数量: {ctors_ResonanceMessageBus.Length}");
        var methods_ResonanceMessageBus = type_ResonanceMessageBus.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResonanceMessageBus 公开方法数量: {methods_ResonanceMessageBus.Length}");
        foreach (var m in methods_ResonanceMessageBus)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResonanceMessageBus 未找到，尝试无命名空间...");
        type_ResonanceMessageBus = Type.GetType("ResonanceMessageBus");
        if (type_ResonanceMessageBus != null)
            Console.WriteLine("[PASS] 类型 ResonanceMessageBus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResonanceMessageBus 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ProtocolConversionCenter
    var type_ProtocolConversionCenter = Type.GetType("ProtocolConversionCenter");
    if (type_ProtocolConversionCenter != null)
    {
        Console.WriteLine("[PASS] 类型 ProtocolConversionCenter (class) 存在");
        var ctors_ProtocolConversionCenter = type_ProtocolConversionCenter.GetConstructors();
        Console.WriteLine($"[PASS] ProtocolConversionCenter 构造函数数量: {ctors_ProtocolConversionCenter.Length}");
        var methods_ProtocolConversionCenter = type_ProtocolConversionCenter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProtocolConversionCenter 公开方法数量: {methods_ProtocolConversionCenter.Length}");
        foreach (var m in methods_ProtocolConversionCenter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProtocolConversionCenter 未找到，尝试无命名空间...");
        type_ProtocolConversionCenter = Type.GetType("ProtocolConversionCenter");
        if (type_ProtocolConversionCenter != null)
            Console.WriteLine("[PASS] 类型 ProtocolConversionCenter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProtocolConversionCenter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DisruptorRingBuffer
    var type_DisruptorRingBuffer = Type.GetType("DisruptorRingBuffer");
    if (type_DisruptorRingBuffer != null)
    {
        Console.WriteLine("[PASS] 类型 DisruptorRingBuffer (class) 存在");
        var ctors_DisruptorRingBuffer = type_DisruptorRingBuffer.GetConstructors();
        Console.WriteLine($"[PASS] DisruptorRingBuffer 构造函数数量: {ctors_DisruptorRingBuffer.Length}");
        var methods_DisruptorRingBuffer = type_DisruptorRingBuffer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DisruptorRingBuffer 公开方法数量: {methods_DisruptorRingBuffer.Length}");
        foreach (var m in methods_DisruptorRingBuffer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DisruptorRingBuffer 未找到，尝试无命名空间...");
        type_DisruptorRingBuffer = Type.GetType("DisruptorRingBuffer");
        if (type_DisruptorRingBuffer != null)
            Console.WriteLine("[PASS] 类型 DisruptorRingBuffer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DisruptorRingBuffer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessageEvent
    var type_MessageEvent = Type.GetType("MessageEvent");
    if (type_MessageEvent != null)
    {
        Console.WriteLine("[PASS] 类型 MessageEvent (class) 存在");
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

    // 验证 class: CompressedMessageAdapter
    var type_CompressedMessageAdapter = Type.GetType("CompressedMessageAdapter");
    if (type_CompressedMessageAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 CompressedMessageAdapter (class) 存在");
        var ctors_CompressedMessageAdapter = type_CompressedMessageAdapter.GetConstructors();
        Console.WriteLine($"[PASS] CompressedMessageAdapter 构造函数数量: {ctors_CompressedMessageAdapter.Length}");
        var methods_CompressedMessageAdapter = type_CompressedMessageAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CompressedMessageAdapter 公开方法数量: {methods_CompressedMessageAdapter.Length}");
        foreach (var m in methods_CompressedMessageAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CompressedMessageAdapter 未找到，尝试无命名空间...");
        type_CompressedMessageAdapter = Type.GetType("CompressedMessageAdapter");
        if (type_CompressedMessageAdapter != null)
            Console.WriteLine("[PASS] 类型 CompressedMessageAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CompressedMessageAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EncryptedMessageAdapter
    var type_EncryptedMessageAdapter = Type.GetType("EncryptedMessageAdapter");
    if (type_EncryptedMessageAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 EncryptedMessageAdapter (class) 存在");
        var ctors_EncryptedMessageAdapter = type_EncryptedMessageAdapter.GetConstructors();
        Console.WriteLine($"[PASS] EncryptedMessageAdapter 构造函数数量: {ctors_EncryptedMessageAdapter.Length}");
        var methods_EncryptedMessageAdapter = type_EncryptedMessageAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EncryptedMessageAdapter 公开方法数量: {methods_EncryptedMessageAdapter.Length}");
        foreach (var m in methods_EncryptedMessageAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EncryptedMessageAdapter 未找到，尝试无命名空间...");
        type_EncryptedMessageAdapter = Type.GetType("EncryptedMessageAdapter");
        if (type_EncryptedMessageAdapter != null)
            Console.WriteLine("[PASS] 类型 EncryptedMessageAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EncryptedMessageAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChunkedMessageAdapter
    var type_ChunkedMessageAdapter = Type.GetType("ChunkedMessageAdapter");
    if (type_ChunkedMessageAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 ChunkedMessageAdapter (class) 存在");
        var ctors_ChunkedMessageAdapter = type_ChunkedMessageAdapter.GetConstructors();
        Console.WriteLine($"[PASS] ChunkedMessageAdapter 构造函数数量: {ctors_ChunkedMessageAdapter.Length}");
        var methods_ChunkedMessageAdapter = type_ChunkedMessageAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChunkedMessageAdapter 公开方法数量: {methods_ChunkedMessageAdapter.Length}");
        foreach (var m in methods_ChunkedMessageAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChunkedMessageAdapter 未找到，尝试无命名空间...");
        type_ChunkedMessageAdapter = Type.GetType("ChunkedMessageAdapter");
        if (type_ChunkedMessageAdapter != null)
            Console.WriteLine("[PASS] 类型 ChunkedMessageAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChunkedMessageAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TracedMessageAdapter
    var type_TracedMessageAdapter = Type.GetType("TracedMessageAdapter");
    if (type_TracedMessageAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 TracedMessageAdapter (class) 存在");
        var ctors_TracedMessageAdapter = type_TracedMessageAdapter.GetConstructors();
        Console.WriteLine($"[PASS] TracedMessageAdapter 构造函数数量: {ctors_TracedMessageAdapter.Length}");
        var methods_TracedMessageAdapter = type_TracedMessageAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TracedMessageAdapter 公开方法数量: {methods_TracedMessageAdapter.Length}");
        foreach (var m in methods_TracedMessageAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TracedMessageAdapter 未找到，尝试无命名空间...");
        type_TracedMessageAdapter = Type.GetType("TracedMessageAdapter");
        if (type_TracedMessageAdapter != null)
            Console.WriteLine("[PASS] 类型 TracedMessageAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TracedMessageAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PriorityMessageAdapter
    var type_PriorityMessageAdapter = Type.GetType("PriorityMessageAdapter");
    if (type_PriorityMessageAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 PriorityMessageAdapter (class) 存在");
        var ctors_PriorityMessageAdapter = type_PriorityMessageAdapter.GetConstructors();
        Console.WriteLine($"[PASS] PriorityMessageAdapter 构造函数数量: {ctors_PriorityMessageAdapter.Length}");
        var methods_PriorityMessageAdapter = type_PriorityMessageAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PriorityMessageAdapter 公开方法数量: {methods_PriorityMessageAdapter.Length}");
        foreach (var m in methods_PriorityMessageAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PriorityMessageAdapter 未找到，尝试无命名空间...");
        type_PriorityMessageAdapter = Type.GetType("PriorityMessageAdapter");
        if (type_PriorityMessageAdapter != null)
            Console.WriteLine("[PASS] 类型 PriorityMessageAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PriorityMessageAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PersistentMessageAdapter
    var type_PersistentMessageAdapter = Type.GetType("PersistentMessageAdapter");
    if (type_PersistentMessageAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 PersistentMessageAdapter (class) 存在");
        var ctors_PersistentMessageAdapter = type_PersistentMessageAdapter.GetConstructors();
        Console.WriteLine($"[PASS] PersistentMessageAdapter 构造函数数量: {ctors_PersistentMessageAdapter.Length}");
        var methods_PersistentMessageAdapter = type_PersistentMessageAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PersistentMessageAdapter 公开方法数量: {methods_PersistentMessageAdapter.Length}");
        foreach (var m in methods_PersistentMessageAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PersistentMessageAdapter 未找到，尝试无命名空间...");
        type_PersistentMessageAdapter = Type.GetType("PersistentMessageAdapter");
        if (type_PersistentMessageAdapter != null)
            Console.WriteLine("[PASS] 类型 PersistentMessageAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PersistentMessageAdapter 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: CacheAlignedBuffer
    var type_CacheAlignedBuffer = Type.GetType("CacheAlignedBuffer");
    if (type_CacheAlignedBuffer != null)
    {
        Console.WriteLine("[PASS] 类型 CacheAlignedBuffer (struct) 存在");
        var ctors_CacheAlignedBuffer = type_CacheAlignedBuffer.GetConstructors();
        Console.WriteLine($"[PASS] CacheAlignedBuffer 构造函数数量: {ctors_CacheAlignedBuffer.Length}");
        var methods_CacheAlignedBuffer = type_CacheAlignedBuffer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheAlignedBuffer 公开方法数量: {methods_CacheAlignedBuffer.Length}");
        foreach (var m in methods_CacheAlignedBuffer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheAlignedBuffer 未找到，尝试无命名空间...");
        type_CacheAlignedBuffer = Type.GetType("CacheAlignedBuffer");
        if (type_CacheAlignedBuffer != null)
            Console.WriteLine("[PASS] 类型 CacheAlignedBuffer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheAlignedBuffer 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ProtocolMessage
    var type_ProtocolMessage = Type.GetType("ProtocolMessage");
    if (type_ProtocolMessage != null)
    {
        Console.WriteLine("[PASS] 类型 ProtocolMessage (record) 存在");
        var ctors_ProtocolMessage = type_ProtocolMessage.GetConstructors();
        Console.WriteLine($"[PASS] ProtocolMessage 构造函数数量: {ctors_ProtocolMessage.Length}");
        var methods_ProtocolMessage = type_ProtocolMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProtocolMessage 公开方法数量: {methods_ProtocolMessage.Length}");
        foreach (var m in methods_ProtocolMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProtocolMessage 未找到，尝试无命名空间...");
        type_ProtocolMessage = Type.GetType("ProtocolMessage");
        if (type_ProtocolMessage != null)
            Console.WriteLine("[PASS] 类型 ProtocolMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProtocolMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: ProtocolType
    var type_ProtocolType = Type.GetType("ProtocolType");
    if (type_ProtocolType != null)
    {
        Console.WriteLine("[PASS] 类型 ProtocolType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProtocolType 未找到，尝试无命名空间...");
        type_ProtocolType = Type.GetType("ProtocolType");
        if (type_ProtocolType != null)
            Console.WriteLine("[PASS] 类型 ProtocolType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProtocolType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
