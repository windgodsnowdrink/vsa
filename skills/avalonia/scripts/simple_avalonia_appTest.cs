#load "simple_avalonia_app.cs"

Console.WriteLine("=== simple_avalonia_app Test ===");

try
{
    var t0 = typeof(SimpleAvaloniaApp.MainViewModel);
    Console.WriteLine($"[PASS] MainViewModel 存在");
    var t1 = typeof(SimpleAvaloniaApp.MainWindow);
    Console.WriteLine($"[PASS] MainWindow 存在");
    var t2 = typeof(SimpleAvaloniaApp.App);
    Console.WriteLine($"[PASS] App 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}