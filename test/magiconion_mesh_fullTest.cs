#load "magiconion_mesh_full.cs"

Console.WriteLine("=== magiconion_mesh_full Test ===");

try
{
    var t0 = typeof(ServiceMeshExtensions);
    Console.WriteLine($"[PASS] ServiceMeshExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}