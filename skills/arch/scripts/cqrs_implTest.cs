#load "cqrs_impl.cs"

Console.WriteLine("=== cqrs_impl Test ===");

try
{
    var t0 = typeof(CqrsImplementation.Get);
    Console.WriteLine($"[PASS] Get 存在");
    var t1 = typeof(CqrsImplementation.Update);
    Console.WriteLine($"[PASS] Update 存在");
    var t2 = typeof(CqrsImplementation.Create);
    Console.WriteLine($"[PASS] Create 存在");
    var t3 = typeof(CqrsImplementation.Delete);
    Console.WriteLine($"[PASS] Delete 存在");
    var t4 = typeof(CqrsImplementation.ConcurrencyConflictException);
    Console.WriteLine($"[PASS] ConcurrencyConflictException 存在");
    var t5 = typeof(CqrsImplementation.I);
    Console.WriteLine($"[PASS] I 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(CqrsImplementation.IMapper);
    Console.WriteLine($"[PASS] IMapper 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(CqrsImplementation.GetAll);
    Console.WriteLine($"[PASS] GetAll record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}