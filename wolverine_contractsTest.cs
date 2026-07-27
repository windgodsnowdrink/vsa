#load "wolverine_contracts.cs"

Console.WriteLine("=== wolverine_contracts Test ===");

try
{
    var t0 = typeof(Contracts.Messages.BindDeviceSagaState);
    Console.WriteLine($"[PASS] BindDeviceSagaState 存在");
    var t1 = typeof(Contracts.Messages.DeviceCreated);
    Console.WriteLine($"[PASS] DeviceCreated record 存在");
    var t2 = typeof(Contracts.Messages.DeviceUpdated);
    Console.WriteLine($"[PASS] DeviceUpdated record 存在");
    var t3 = typeof(Contracts.Messages.UserCreated);
    Console.WriteLine($"[PASS] UserCreated record 存在");
    var t4 = typeof(Contracts.Messages.UserUpdated);
    Console.WriteLine($"[PASS] UserUpdated record 存在");
    var t5 = typeof(Contracts.Messages.DeviceBoundToUser);
    Console.WriteLine($"[PASS] DeviceBoundToUser record 存在");
    var t6 = typeof(Contracts.Messages.DeviceUnboundFromUser);
    Console.WriteLine($"[PASS] DeviceUnboundFromUser record 存在");
    var t7 = typeof(Contracts.Messages.QueryUserDevices);
    Console.WriteLine($"[PASS] QueryUserDevices record 存在");
    var t8 = typeof(Contracts.Messages.QueryDeviceUsers);
    Console.WriteLine($"[PASS] QueryDeviceUsers record 存在");
    var t9 = typeof(Contracts.Messages.UserDeviceAdded);
    Console.WriteLine($"[PASS] UserDeviceAdded record 存在");
    var t10 = typeof(Contracts.Messages.UserDeviceRemoved);
    Console.WriteLine($"[PASS] UserDeviceRemoved record 存在");
    var t11 = typeof(Contracts.Messages.GetUserDevices);
    Console.WriteLine($"[PASS] GetUserDevices record 存在");
    var t12 = typeof(Contracts.Messages.UserDevicesResponse);
    Console.WriteLine($"[PASS] UserDevicesResponse record 存在");
    var t13 = typeof(Contracts.Messages.DeviceUsersResponse);
    Console.WriteLine($"[PASS] DeviceUsersResponse record 存在");
    var t14 = typeof(Contracts.Messages.BindDeviceCommand);
    Console.WriteLine($"[PASS] BindDeviceCommand record 存在");
    var t15 = typeof(Contracts.Messages.BindDeviceResponse);
    Console.WriteLine($"[PASS] BindDeviceResponse record 存在");
    var t16 = typeof(Contracts.Messages.UnbindDeviceCommand);
    Console.WriteLine($"[PASS] UnbindDeviceCommand record 存在");
    var t17 = typeof(Contracts.Messages.UnbindDeviceResponse);
    Console.WriteLine($"[PASS] UnbindDeviceResponse record 存在");
    var t18 = typeof(Contracts.Messages.UserExistsQuery);
    Console.WriteLine($"[PASS] UserExistsQuery record 存在");
    var t19 = typeof(Contracts.Messages.UserExistsResponse);
    Console.WriteLine($"[PASS] UserExistsResponse record 存在");
    var t20 = typeof(Contracts.Messages.UserDeviceSaved);
    Console.WriteLine($"[PASS] UserDeviceSaved record 存在");
    var t21 = typeof(Contracts.Messages.BindDeviceStep);
    Console.WriteLine($"[PASS] BindDeviceStep enum 存在 (IsEnum: {t21.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}