#load "jieba_integration.cs"

Console.WriteLine("=== jieba_integration Test ===");

try
{
    var t0 = typeof(JiebaNetIntegration.JiebaOptions);
    Console.WriteLine($"[PASS] JiebaOptions 存在");
    var t1 = typeof(JiebaNetIntegration.JiebaService);
    Console.WriteLine($"[PASS] JiebaService 存在");
    var t2 = typeof(JiebaNetIntegration.JiebaServiceCollectionExtensions);
    Console.WriteLine($"[PASS] JiebaServiceCollectionExtensions 存在");
    var t3 = typeof(JiebaNetIntegration.JiebaSegmenterPooledObjectPolicy);
    Console.WriteLine($"[PASS] JiebaSegmenterPooledObjectPolicy 存在");
    var t4 = typeof(JiebaNetIntegration.IJiebaService);
    Console.WriteLine($"[PASS] IJiebaService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}