#load "craftsman_aot.cs"

Console.WriteLine("=== craftsman_aot Test ===");

try
{
    var t0 = typeof(Craftsman.AOT.CraftsmanOptions);
    Console.WriteLine($"[PASS] CraftsmanOptions 存在");
    var t1 = typeof(Craftsman.AOT.CraftsmanInput);
    Console.WriteLine($"[PASS] CraftsmanInput 存在");
    var t2 = typeof(Craftsman.AOT.CraftsmanResult);
    Console.WriteLine($"[PASS] CraftsmanResult 存在");
    var t3 = typeof(Craftsman.AOT.CraftsmanStatus);
    Console.WriteLine($"[PASS] CraftsmanStatus 存在");
    var t4 = typeof(Craftsman.AOT.CraftsmanService);
    Console.WriteLine($"[PASS] CraftsmanService 存在");
    var t5 = typeof(Craftsman.AOT.CraftsmanAotEngine);
    Console.WriteLine($"[PASS] CraftsmanAotEngine 存在");
    var t6 = typeof(Craftsman.AOT.ICraftsmanService);
    Console.WriteLine($"[PASS] ICraftsmanService 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}