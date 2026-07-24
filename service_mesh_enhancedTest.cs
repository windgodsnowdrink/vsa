#load "service_mesh_enhanced.cs"

Console.WriteLine("=== service_mesh_enhanced Test ===");

try
{
    var t0 = typeof(ChannelMeshSecurityHandler);
    Console.WriteLine($"[PASS] ChannelMeshSecurityHandler 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}