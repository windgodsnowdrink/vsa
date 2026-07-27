#load "linkers_mesh_integration.cs"

Console.WriteLine("=== linkers_mesh_integration Test ===");

try
{
    var t0 = typeof(LinkersMeshOptions);
    Console.WriteLine($"[PASS] LinkersMeshOptions 存在");
    var t1 = typeof(LinkersMeshClient);
    Console.WriteLine($"[PASS] LinkersMeshClient 存在");
    var t2 = typeof(LinkersMeshExtensions);
    Console.WriteLine($"[PASS] LinkersMeshExtensions 存在");
    var t3 = typeof(LinkersMeshBackgroundService);
    Console.WriteLine($"[PASS] LinkersMeshBackgroundService 存在");
    var t4 = typeof(ILinkersMeshClient);
    Console.WriteLine($"[PASS] ILinkersMeshClient 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}