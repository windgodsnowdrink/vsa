#load "lz4_integration.cs"

Console.WriteLine("=== lz4_integration Test ===");

try
{
    var t0 = typeof(LZ4Options);
    Console.WriteLine($"[PASS] LZ4Options 存在");
    var t1 = typeof(LZ4Service);
    Console.WriteLine($"[PASS] LZ4Service 存在");
    var t2 = typeof(LZ4ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] LZ4ServiceCollectionExtensions 存在");
    var t3 = typeof(LZ4Demo);
    Console.WriteLine($"[PASS] LZ4Demo 存在");
    var t4 = typeof(ILZ4Service);
    Console.WriteLine($"[PASS] ILZ4Service 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}