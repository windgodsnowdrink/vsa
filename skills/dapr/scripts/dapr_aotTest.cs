#load "dapr_aot.cs"

Console.WriteLine("=== dapr_aot Test ===");

try
{
    var t0 = typeof(Dapr.AOT.DaprOptions);
    Console.WriteLine($"[PASS] DaprOptions 存在");
    var t1 = typeof(Dapr.AOT.where);
    Console.WriteLine($"[PASS] where 存在");
    var t2 = typeof(Dapr.AOT.DaprResult);
    Console.WriteLine($"[PASS] DaprResult 存在");
    var t3 = typeof(Dapr.AOT.DaprStatus);
    Console.WriteLine($"[PASS] DaprStatus 存在");
    var t4 = typeof(Dapr.AOT.DaprService);
    Console.WriteLine($"[PASS] DaprService 存在");
    var t5 = typeof(Dapr.AOT.DaprAotEngine);
    Console.WriteLine($"[PASS] DaprAotEngine 存在");
    var t6 = typeof(Dapr.AOT.DaprClientExtensions);
    Console.WriteLine($"[PASS] DaprClientExtensions 存在");
    var t7 = typeof(Dapr.AOT.IDaprService);
    Console.WriteLine($"[PASS] IDaprService 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}