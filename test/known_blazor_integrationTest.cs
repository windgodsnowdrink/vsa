#load "known_blazor_integration.cs"

Console.WriteLine("=== known_blazor_integration Test ===");

try
{
    var t0 = typeof(KnownBlazorOptions);
    Console.WriteLine($"[PASS] KnownBlazorOptions 存在");
    var t1 = typeof(KnownBlazorService);
    Console.WriteLine($"[PASS] KnownBlazorService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(IKnownBlazorService);
    Console.WriteLine($"[PASS] IKnownBlazorService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}