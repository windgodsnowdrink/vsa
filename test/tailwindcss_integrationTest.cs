#load "tailwindcss_integration.cs"

Console.WriteLine("=== tailwindcss_integration Test ===");

try
{
    var t0 = typeof(TailwindOptions);
    Console.WriteLine($"[PASS] TailwindOptions 存在");
    var t1 = typeof(TailwindService);
    Console.WriteLine($"[PASS] TailwindService 存在");
    var t2 = typeof(TailwindExtensions);
    Console.WriteLine($"[PASS] TailwindExtensions 存在");
    var t3 = typeof(ITailwindService);
    Console.WriteLine($"[PASS] ITailwindService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}