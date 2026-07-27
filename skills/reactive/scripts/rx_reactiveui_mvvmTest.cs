#load "rx_reactiveui_mvvm.cs"

Console.WriteLine("=== rx_reactiveui_mvvm Test ===");

try
{
    var t0 = typeof(ReactiveOptions);
    Console.WriteLine($"[PASS] ReactiveOptions 存在");
    var t1 = typeof(ReactiveViewModelBase);
    Console.WriteLine($"[PASS] ReactiveViewModelBase 存在");
    var t2 = typeof(UserProfileViewModel);
    Console.WriteLine($"[PASS] UserProfileViewModel 存在");
    var t3 = typeof(ReactiveService);
    Console.WriteLine($"[PASS] ReactiveService 存在");
    var t4 = typeof(ReactiveServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ReactiveServiceCollectionExtensions 存在");
    var t5 = typeof(ObjectPool);
    Console.WriteLine($"[PASS] ObjectPool 存在");
    var t6 = typeof(DisposableExtensions);
    Console.WriteLine($"[PASS] DisposableExtensions 存在");
    var t7 = typeof(ProgramEntry);
    Console.WriteLine($"[PASS] ProgramEntry 存在");
    var t8 = typeof(IReactiveService);
    Console.WriteLine($"[PASS] IReactiveService 接口存在 (IsInterface: {t8.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}