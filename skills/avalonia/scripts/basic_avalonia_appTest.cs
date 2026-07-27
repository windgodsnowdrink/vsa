#load "basic_avalonia_app.cs"

Console.WriteLine("=== basic_avalonia_app Test ===");

try
{
    var t0 = typeof(BasicAvaloniaApp.MainWindow);
    Console.WriteLine($"[PASS] MainWindow 存在");
    var t1 = typeof(BasicAvaloniaApp.App);
    Console.WriteLine($"[PASS] App 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}