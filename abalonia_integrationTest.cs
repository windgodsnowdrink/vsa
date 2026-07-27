#load "abalonia_integration.cs"

Console.WriteLine("=== abalonia_integration Test ===");

try
{
    var t0 = typeof(App);
    Console.WriteLine($"[PASS] App 存在");
    var t1 = typeof(MainViewModel);
    Console.WriteLine($"[PASS] MainViewModel 存在");
    var t2 = typeof(MainWindow);
    Console.WriteLine($"[PASS] MainWindow 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}