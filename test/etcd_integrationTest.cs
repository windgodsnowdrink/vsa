#load "etcd_integration.cs"

Console.WriteLine("=== etcd_integration Test ===");

try
{
    var t0 = typeof(EtcdIntegration.EtcdOptions);
    Console.WriteLine($"[PASS] EtcdOptions 存在");
    var t1 = typeof(EtcdIntegration.EtcdService);
    Console.WriteLine($"[PASS] EtcdService 存在");
    var t2 = typeof(EtcdIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(EtcdIntegration.IEtcdService);
    Console.WriteLine($"[PASS] IEtcdService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}