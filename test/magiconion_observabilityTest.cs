#load "magiconion_observability.cs"

Console.WriteLine("=== magiconion_observability Test ===");

try
{
    var t0 = typeof(ServiceMeshExtensions);
    Console.WriteLine($"[PASS] ServiceMeshExtensions 存在");
    var t1 = typeof(ChaosPolicyBuilder);
    Console.WriteLine($"[PASS] ChaosPolicyBuilder 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}