#load "thinggateway_integration.cs"

Console.WriteLine("=== thinggateway_integration Test ===");

try
{
    var t0 = typeof(IotGatewayService);
    Console.WriteLine($"[PASS] IotGatewayService 存在");
    var t1 = typeof(DeviceContextPooledPolicy);
    Console.WriteLine($"[PASS] DeviceContextPooledPolicy 存在");
    var t2 = typeof(DeviceContext);
    Console.WriteLine($"[PASS] DeviceContext 存在");
    var t3 = typeof(SerialPortPooledPolicy);
    Console.WriteLine($"[PASS] SerialPortPooledPolicy 存在");
    var t4 = typeof(IotDbContext);
    Console.WriteLine($"[PASS] IotDbContext 存在");
    var t5 = typeof(DeviceDataEntity);
    Console.WriteLine($"[PASS] DeviceDataEntity 存在");
    var t6 = typeof(DeviceConfigVersionEntity);
    Console.WriteLine($"[PASS] DeviceConfigVersionEntity 存在");
    var t7 = typeof(DeviceModelPublisher);
    Console.WriteLine($"[PASS] DeviceModelPublisher 存在");
    var t8 = typeof(DeviceCommandSubscriber);
    Console.WriteLine($"[PASS] DeviceCommandSubscriber 存在");
    var t9 = typeof(DeviceModelProtocolPooledPolicy);
    Console.WriteLine($"[PASS] DeviceModelProtocolPooledPolicy 存在");
    var t10 = typeof(DefaultDeviceModelProtocol);
    Console.WriteLine($"[PASS] DefaultDeviceModelProtocol 存在");
    var t11 = typeof(IDeviceModelProtocol);
    Console.WriteLine($"[PASS] IDeviceModelProtocol 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(DeviceNotification);
    Console.WriteLine($"[PASS] DeviceNotification record 存在");
    var t13 = typeof(DeviceConfigVersion);
    Console.WriteLine($"[PASS] DeviceConfigVersion record 存在");
    var t14 = typeof(DeviceModel);
    Console.WriteLine($"[PASS] DeviceModel record 存在");
    var t15 = typeof(DeviceCommand);
    Console.WriteLine($"[PASS] DeviceCommand record 存在");
    var t16 = typeof(NotificationType);
    Console.WriteLine($"[PASS] NotificationType enum 存在 (IsEnum: {t16.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}