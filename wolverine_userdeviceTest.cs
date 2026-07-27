#load "wolverine_userdevice.cs"

Console.WriteLine("=== wolverine_userdevice Test ===");

try
{
    var t0 = typeof(UserDeviceHandler);
    Console.WriteLine($"[PASS] UserDeviceHandler 存在");
    var t1 = typeof(InMemoryUserDeviceRepository);
    Console.WriteLine($"[PASS] InMemoryUserDeviceRepository 存在");
    var t2 = typeof(UserDeviceCompensationHandler);
    Console.WriteLine($"[PASS] UserDeviceCompensationHandler 存在");
    var t3 = typeof(IUserDeviceRepository);
    Console.WriteLine($"[PASS] IUserDeviceRepository 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}