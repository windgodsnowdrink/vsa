#load "httprepl_mesh.cs"

Console.WriteLine("=== httprepl_mesh Test ===");

try
{
    var t0 = typeof(ChannelMeshProcessor);
    Console.WriteLine($"[PASS] ChannelMeshProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}