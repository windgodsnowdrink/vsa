#load "resonance_adapter.cs"

Console.WriteLine("=== resonance_adapter Test ===");

try
{
    var t0 = typeof(MultiProtocolAdapter);
    Console.WriteLine($"[PASS] MultiProtocolAdapter 存在");
    var t1 = typeof(ResonanceMessageAdapter);
    Console.WriteLine($"[PASS] ResonanceMessageAdapter 存在");
    var t2 = typeof(TcpProtocolAdapter);
    Console.WriteLine($"[PASS] TcpProtocolAdapter 存在");
    var t3 = typeof(GrpcProtocolAdapter);
    Console.WriteLine($"[PASS] GrpcProtocolAdapter 存在");
    var t4 = typeof(RestProtocolAdapter);
    Console.WriteLine($"[PASS] RestProtocolAdapter 存在");
    var t5 = typeof(MqttProtocolAdapter);
    Console.WriteLine($"[PASS] MqttProtocolAdapter 存在");
    var t6 = typeof(AmqpProtocolAdapter);
    Console.WriteLine($"[PASS] AmqpProtocolAdapter 存在");
    var t7 = typeof(JsonMessageAdapter);
    Console.WriteLine($"[PASS] JsonMessageAdapter 存在");
    var t8 = typeof(ResonanceMessageBus);
    Console.WriteLine($"[PASS] ResonanceMessageBus 存在");
    var t9 = typeof(ProtocolConversionCenter);
    Console.WriteLine($"[PASS] ProtocolConversionCenter 存在");
    var t10 = typeof(for);
    Console.WriteLine($"[PASS] for 存在");
    var t11 = typeof(DisruptorRingBuffer);
    Console.WriteLine($"[PASS] DisruptorRingBuffer 存在");
    var t12 = typeof(MessageEvent);
    Console.WriteLine($"[PASS] MessageEvent 存在");
    var t13 = typeof(MessageEventHandler);
    Console.WriteLine($"[PASS] MessageEventHandler 存在");
    var t14 = typeof(CompressedMessageAdapter);
    Console.WriteLine($"[PASS] CompressedMessageAdapter 存在");
    var t15 = typeof(EncryptedMessageAdapter);
    Console.WriteLine($"[PASS] EncryptedMessageAdapter 存在");
    var t16 = typeof(ChunkedMessageAdapter);
    Console.WriteLine($"[PASS] ChunkedMessageAdapter 存在");
    var t17 = typeof(TracedMessageAdapter);
    Console.WriteLine($"[PASS] TracedMessageAdapter 存在");
    var t18 = typeof(PriorityMessageAdapter);
    Console.WriteLine($"[PASS] PriorityMessageAdapter 存在");
    var t19 = typeof(PersistentMessageAdapter);
    Console.WriteLine($"[PASS] PersistentMessageAdapter 存在");
    var t20 = typeof(ProtocolMessage);
    Console.WriteLine($"[PASS] ProtocolMessage record 存在");
    var t21 = typeof(CacheAlignedBuffer);
    Console.WriteLine($"[PASS] CacheAlignedBuffer struct 存在");
    var t22 = typeof(ProtocolType);
    Console.WriteLine($"[PASS] ProtocolType enum 存在 (IsEnum: {t22.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}