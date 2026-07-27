#load "jieba_aot.cs"

Console.WriteLine("=== jieba_aot Test ===");

try
{
    var t0 = typeof(JiebaAot.JiebaService);
    Console.WriteLine($"[PASS] JiebaService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}