#load "ipc_json_serializer.cs"

Console.WriteLine("=== ipc_json_serializer Test ===");

try
{
    var t0 = typeof(IpcJsonSerializer);
    Console.WriteLine($"[PASS] IpcJsonSerializer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}