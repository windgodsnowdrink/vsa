#load "DeviceModel.cs"

Console.WriteLine("=== DeviceModel Test ===");

try
{
    var t0 = typeof(IDeviceModelProtocol);
    Console.WriteLine($"[PASS] IDeviceModelProtocol 接口存在 (IsInterface: {t0.IsInterface})");
    var t1 = typeof(DeviceModel);
    Console.WriteLine($"[PASS] DeviceModel record 存在");
    var t2 = typeof(PropertyMetadata);
    Console.WriteLine($"[PASS] PropertyMetadata record 存在");
    var t3 = typeof(DeviceModelType);
    Console.WriteLine($"[PASS] DeviceModelType enum 存在 (IsEnum: {t3.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}