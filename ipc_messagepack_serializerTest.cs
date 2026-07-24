#load "ipc_messagepack_serializer.cs"

Console.WriteLine("=== ipc_messagepack_serializer Test ===");

try
{
    var t0 = typeof(IpcMessagePackSerializer);
    Console.WriteLine($"[PASS] IpcMessagePackSerializer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}