#load "silky_maui_integration.cs"

Console.WriteLine("=== silky_maui_integration Test ===");

try
{
    var t0 = typeof(MauiProgram);
    Console.WriteLine($"[PASS] MauiProgram 存在");
    var t1 = typeof(AppModule);
    Console.WriteLine($"[PASS] AppModule 存在");
    var t2 = typeof(App);
    Console.WriteLine($"[PASS] App 存在");
    var t3 = typeof(MyMicroService);
    Console.WriteLine($"[PASS] MyMicroService 存在");
    var t4 = typeof(MainPage);
    Console.WriteLine($"[PASS] MainPage 存在");
    var t5 = typeof(IMyMicroService);
    Console.WriteLine($"[PASS] IMyMicroService 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}