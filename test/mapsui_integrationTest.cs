#load "mapsui_integration.cs"

Console.WriteLine("=== mapsui_integration Test ===");

try
{
    var t0 = typeof(MapFeature);
    Console.WriteLine($"[PASS] MapFeature 存在");
    var t1 = typeof(MapsuiMapService);
    Console.WriteLine($"[PASS] MapsuiMapService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(IMapService);
    Console.WriteLine($"[PASS] IMapService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}