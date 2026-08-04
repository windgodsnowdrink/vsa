#load "webapiclientcore_mesh.cs"

Console.WriteLine("=== webapiclientcore_mesh Test ===");

try
{
    var t0 = typeof(ChannelMeshProxy);
    Console.WriteLine($"[PASS] ChannelMeshProxy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}