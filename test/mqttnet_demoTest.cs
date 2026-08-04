#load "mqttnet_demo.cs"

Console.WriteLine("=== mqttnet_demo Test ===");

try
{
    var t0 = typeof(MqttMessageDbContext);
    Console.WriteLine($"[PASS] MqttMessageDbContext 存在");
    var t1 = typeof(PersistedMessage);
    Console.WriteLine($"[PASS] PersistedMessage 存在");
    var t2 = typeof(EnhancedMemoryOptimizer);
    Console.WriteLine($"[PASS] EnhancedMemoryOptimizer 存在");
    var t3 = typeof(TodoEvent);
    Console.WriteLine($"[PASS] TodoEvent 存在");
    var t4 = typeof(EnhancedMqttEventBusService);
    Console.WriteLine($"[PASS] EnhancedMqttEventBusService 存在");
    var t5 = typeof(MqttPerformanceMonitor);
    Console.WriteLine($"[PASS] MqttPerformanceMonitor 存在");
    var t6 = typeof(MqttClusterManager);
    Console.WriteLine($"[PASS] MqttClusterManager 存在");
    var t7 = typeof(MessageCompressor);
    Console.WriteLine($"[PASS] MessageCompressor 存在");
    var t8 = typeof(MqttConnectionPool);
    Console.WriteLine($"[PASS] MqttConnectionPool 存在");
    var t9 = typeof(MqttConnectionLeakDetector);
    Console.WriteLine($"[PASS] MqttConnectionLeakDetector 存在");
    var t10 = typeof(MqttConnectionPoolAutoScaler);
    Console.WriteLine($"[PASS] MqttConnectionPoolAutoScaler 存在");
    var t11 = typeof(MqttRetryPolicy);
    Console.WriteLine($"[PASS] MqttRetryPolicy 存在");
    var t12 = typeof(MqttTlsOptimizer);
    Console.WriteLine($"[PASS] MqttTlsOptimizer 存在");
    var t13 = typeof(MqttMessageCompressor);
    Console.WriteLine($"[PASS] MqttMessageCompressor 存在");
    var t14 = typeof(MqttHealthCheck);
    Console.WriteLine($"[PASS] MqttHealthCheck 存在");
    var t15 = typeof(MqttConnectionMetrics);
    Console.WriteLine($"[PASS] MqttConnectionMetrics 存在");
    var t16 = typeof(MqttMessageProcessor);
    Console.WriteLine($"[PASS] MqttMessageProcessor 存在");
    var t17 = typeof(MessageRecoveryService);
    Console.WriteLine($"[PASS] MessageRecoveryService 存在");
    var t18 = typeof(TieredMemoryManager);
    Console.WriteLine($"[PASS] TieredMemoryManager 存在");
    var t19 = typeof(AotMessageCompressor);
    Console.WriteLine($"[PASS] AotMessageCompressor 存在");
    var t20 = typeof(MqttDiagnosticObserver);
    Console.WriteLine($"[PASS] MqttDiagnosticObserver 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}