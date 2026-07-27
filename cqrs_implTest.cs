#load "cqrs_impl.cs"

Console.WriteLine("=== cqrs_impl Test ===");

try
{
    var t0 = typeof(Get);
    Console.WriteLine($"[PASS] Get 存在");
    var t1 = typeof(Update);
    Console.WriteLine($"[PASS] Update 存在");
    var t2 = typeof(Create);
    Console.WriteLine($"[PASS] Create 存在");
    var t3 = typeof(GetAll);
    Console.WriteLine($"[PASS] GetAll record 存在");
    var t4 = typeof(Delete);
    Console.WriteLine($"[PASS] Delete record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}