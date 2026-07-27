#load "wolverine_device.cs"

Console.WriteLine("=== wolverine_device Test ===");

try
{
    // 验证 IDeviceService 接口
    var deviceServiceType = typeof(IDeviceService);
    Console.WriteLine($"[PASS] IDeviceService 类型存在: {deviceServiceType.Name}");

    // 验证 DeviceService 类
    var serviceType = typeof(DeviceService);
    Console.WriteLine($"[PASS] DeviceService 类型存在: {serviceType.Name}");

    // 验证 Device 类
    var deviceType = typeof(Device);
    Console.WriteLine($"[PASS] Device 类型存在: {deviceType.Name}");

    // 验证 DeviceHandler 类
    var handlerType = typeof(DeviceHandler);
    Console.WriteLine($"[PASS] DeviceHandler 类型存在: {handlerType.Name}");

    // 验证 DeviceQueryService 类
    var queryServiceType = typeof(DeviceQueryService);
    Console.WriteLine($"[PASS] DeviceQueryService 类型存在: {queryServiceType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}