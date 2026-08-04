#load "webapiclientcore_dynamicproxy.cs"

Console.WriteLine("=== webapiclientcore_dynamicproxy Test ===");

try
{
    var t0 = typeof(ThreadLocalSpanInterceptor);
    Console.WriteLine($"[PASS] ThreadLocalSpanInterceptor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}