#load "mqttnet.cs"

Console.WriteLine("=== mqttnet Test ===");

try
{
    var t0 = typeof(MqttAdvancedIntegration.MqttConnectionOptions);
    Console.WriteLine($"[PASS] MqttConnectionOptions 存在");
    var t1 = typeof(MqttAdvancedIntegration.TransformOptions);
    Console.WriteLine($"[PASS] TransformOptions 存在");
    var t2 = typeof(MqttAdvancedIntegration.PersistedMessage);
    Console.WriteLine($"[PASS] PersistedMessage 存在");
    var t3 = typeof(MqttAdvancedIntegration.MqttMessageDbContext);
    Console.WriteLine($"[PASS] MqttMessageDbContext 存在");
    var t4 = typeof(MqttAdvancedIntegration.Device);
    Console.WriteLine($"[PASS] Device 存在");
    var t5 = typeof(MqttAdvancedIntegration.DeviceEvent);
    Console.WriteLine($"[PASS] DeviceEvent 存在");
    var t6 = typeof(MqttAdvancedIntegration.UpdateDeviceStatusCommand);
    Console.WriteLine($"[PASS] UpdateDeviceStatusCommand 存在");
    var t7 = typeof(MqttAdvancedIntegration.GetDeviceStatusQuery);
    Console.WriteLine($"[PASS] GetDeviceStatusQuery 存在");
    var t8 = typeof(MqttAdvancedIntegration.DeviceCommandHandler);
    Console.WriteLine($"[PASS] DeviceCommandHandler 存在");
    var t9 = typeof(MqttAdvancedIntegration.DeviceQueryHandler);
    Console.WriteLine($"[PASS] DeviceQueryHandler 存在");
    var t10 = typeof(MqttAdvancedIntegration.WarmMessageCache);
    Console.WriteLine($"[PASS] WarmMessageCache 存在");
    var t11 = typeof(MqttAdvancedIntegration.ColdStorageProxy);
    Console.WriteLine($"[PASS] ColdStorageProxy 存在");
    var t12 = typeof(MqttAdvancedIntegration.MqttArchiveService);
    Console.WriteLine($"[PASS] MqttArchiveService 存在");
    var t13 = typeof(MqttAdvancedIntegration.MemoryStreamPoolPolicy);
    Console.WriteLine($"[PASS] MemoryStreamPoolPolicy 存在");
    var t14 = typeof(MqttAdvancedIntegration.PoolingManager);
    Console.WriteLine($"[PASS] PoolingManager 存在");
    var t15 = typeof(MqttAdvancedIntegration.AdvancedMemoryOwner);
    Console.WriteLine($"[PASS] AdvancedMemoryOwner 存在");
    var t16 = typeof(MqttAdvancedIntegration.DeviceConnectionStateMachine);
    Console.WriteLine($"[PASS] DeviceConnectionStateMachine 存在");
    var t17 = typeof(MqttAdvancedIntegration.DeviceIntegrationService);
    Console.WriteLine($"[PASS] DeviceIntegrationService 存在");
    var t18 = typeof(MqttAdvancedIntegration.MessageTransformBlock);
    Console.WriteLine($"[PASS] MessageTransformBlock 存在");
    var t19 = typeof(MqttAdvancedIntegration.EventProcessor);
    Console.WriteLine($"[PASS] EventProcessor 存在");
    var t20 = typeof(MqttAdvancedIntegration.CleanupHandler);
    Console.WriteLine($"[PASS] CleanupHandler 存在");
    var t21 = typeof(MqttAdvancedIntegration.MqttNetworkServer);
    Console.WriteLine($"[PASS] MqttNetworkServer 存在");
    var t22 = typeof(MqttAdvancedIntegration.WebSocketNotificationService);
    Console.WriteLine($"[PASS] WebSocketNotificationService 存在");
    var t23 = typeof(MqttAdvancedIntegration.MessageHandlerRouter);
    Console.WriteLine($"[PASS] MessageHandlerRouter 存在");
    var t24 = typeof(MqttAdvancedIntegration.MqttAuthenticationService);
    Console.WriteLine($"[PASS] MqttAuthenticationService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}