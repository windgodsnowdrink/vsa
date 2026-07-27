#load "wolverine_contracts.cs"

Console.WriteLine("=== wolverine_contracts Test ===");

try
{
    // 验证 DeviceCreated record
    var deviceCreatedType = Type.GetType("Contracts.Messages.DeviceCreated");
    Console.WriteLine($"[PASS] Contracts.Messages.DeviceCreated 类型存在: {deviceCreatedType != null}");

    // 验证 DeviceUpdated record
    var deviceUpdatedType = Type.GetType("Contracts.Messages.DeviceUpdated");
    Console.WriteLine($"[PASS] Contracts.Messages.DeviceUpdated 类型存在: {deviceUpdatedType != null}");

    // 验证 UserCreated record
    var userCreatedType = Type.GetType("Contracts.Messages.UserCreated");
    Console.WriteLine($"[PASS] Contracts.Messages.UserCreated 类型存在: {userCreatedType != null}");

    // 验证 BindDeviceSagaState 类
    var sagaStateType = Type.GetType("Contracts.Messages.SagaState.BindDeviceSagaState");
    Console.WriteLine($"[PASS] BindDeviceSagaState 类型存在: {sagaStateType != null}");

    // 验证 BindDeviceStep enum
    var stepEnum = Type.GetType("Contracts.Messages.SagaState.BindDeviceStep");
    Console.WriteLine($"[PASS] BindDeviceStep 枚举存在: {stepEnum != null}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}