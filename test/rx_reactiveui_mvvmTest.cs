#load "rx_reactiveui_mvvm.cs"

Console.WriteLine("=== rx_reactiveui_mvvm Test ===");

try
{
    var t0 = typeof(ReactiveViewModelBase);
    Console.WriteLine($"[PASS] ReactiveViewModelBase 存在");
    var t1 = typeof(UserProfileViewModel);
    Console.WriteLine($"[PASS] UserProfileViewModel 存在");
    var t2 = typeof(ReactiveExtensions);
    Console.WriteLine($"[PASS] ReactiveExtensions 存在");
    var t3 = typeof(UserProfileView);
    Console.WriteLine($"[PASS] UserProfileView 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}