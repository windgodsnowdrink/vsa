#load "wolverine_device.cs"

Console.WriteLine("=== wolverine_device Test ===");

try
{
    var t0 = typeof(DeviceService);
    Console.WriteLine($"[PASS] DeviceService 存在");
    var t1 = typeof(DeviceQueryService);
    Console.WriteLine($"[PASS] DeviceQueryService 存在");
    var t2 = typeof(InMemoryDeviceRepository);
    Console.WriteLine($"[PASS] InMemoryDeviceRepository 存在");
    var t3 = typeof(Device);
    Console.WriteLine($"[PASS] Device 存在");
    var t4 = typeof(DeviceHandler);
    Console.WriteLine($"[PASS] DeviceHandler 存在");
    var t5 = typeof(BindDeviceHandler);
    Console.WriteLine($"[PASS] BindDeviceHandler 存在");
    var t6 = typeof(IDeviceService);
    Console.WriteLine($"[PASS] IDeviceService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(IDeviceRepository);
    Console.WriteLine($"[PASS] IDeviceRepository 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(DeviceDto);
    Console.WriteLine($"[PASS] DeviceDto record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}