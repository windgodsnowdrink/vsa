#load "http_aot.cs"

Console.WriteLine("=== http_aot Test ===");

try
{
    var t0 = typeof(HttpSettings);
    Console.WriteLine($"[PASS] HttpSettings 存在");
    var t1 = typeof(HttpResponse);
    Console.WriteLine($"[PASS] HttpResponse 存在");
    var t2 = typeof(HttpService);
    Console.WriteLine($"[PASS] HttpService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}