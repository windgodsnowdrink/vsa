#load "etcd_aot.cs"

Console.WriteLine("=== etcd_aot Test ===");

try
{
    var t0 = typeof(Etcd.AOT.EtcdOptions);
    Console.WriteLine($"[PASS] EtcdOptions 存在");
    var t1 = typeof(Etcd.AOT.EtcdKeyValue);
    Console.WriteLine($"[PASS] EtcdKeyValue 存在");
    var t2 = typeof(Etcd.AOT.EtcdCommandResult);
    Console.WriteLine($"[PASS] EtcdCommandResult 存在");
    var t3 = typeof(Etcd.AOT.EtcdService);
    Console.WriteLine($"[PASS] EtcdService 存在");
    var t4 = typeof(Etcd.AOT.EtcdAotEngine);
    Console.WriteLine($"[PASS] EtcdAotEngine 存在");
    var t5 = typeof(Etcd.AOT.IEtcdService);
    Console.WriteLine($"[PASS] IEtcdService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(Etcd.AOT.EtcdCommandType);
    Console.WriteLine($"[PASS] EtcdCommandType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}