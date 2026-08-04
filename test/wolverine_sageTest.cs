#load "wolverine_sage.cs"

Console.WriteLine("=== wolverine_sage Test ===");

try
{
    var t0 = typeof(Bff.Api.Controllers.UserDeviceSavedNotifier);
    Console.WriteLine($"[PASS] UserDeviceSavedNotifier 存在");
    var t1 = typeof(Bff.Api.Controllers.DeviceStatusHandler);
    Console.WriteLine($"[PASS] DeviceStatusHandler 存在");
    var t2 = typeof(Bff.Api.Controllers.UserDeviceController);
    Console.WriteLine($"[PASS] UserDeviceController 存在");
    var t3 = typeof(Bff.Api.Controllers.BindDeviceSaga);
    Console.WriteLine($"[PASS] BindDeviceSaga 存在");
    var t4 = typeof(Bff.Api.Controllers.GetSagaStateHandler);
    Console.WriteLine($"[PASS] GetSagaStateHandler 存在");
    var t5 = typeof(Bff.Api.Controllers.BindRequest);
    Console.WriteLine($"[PASS] BindRequest record 存在");
    var t6 = typeof(Bff.Api.Controllers.StartBindDeviceSaga);
    Console.WriteLine($"[PASS] StartBindDeviceSaga record 存在");
    var t7 = typeof(Bff.Api.Controllers.UserDeviceSavedTimeout);
    Console.WriteLine($"[PASS] UserDeviceSavedTimeout record 存在");
    var t8 = typeof(Bff.Api.Controllers.GetSagaState);
    Console.WriteLine($"[PASS] GetSagaState record 存在");
    var t9 = typeof(Bff.Api.Controllers.BindDeviceSucceeded);
    Console.WriteLine($"[PASS] BindDeviceSucceeded record 存在");
    var t10 = typeof(Bff.Api.Controllers.BindDeviceFailed);
    Console.WriteLine($"[PASS] BindDeviceFailed record 存在");
    var t11 = typeof(Bff.Api.Controllers.CompensateUserDevice);
    Console.WriteLine($"[PASS] CompensateUserDevice record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}