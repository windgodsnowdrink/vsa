#load "webapiclientcore_protobuf.cs"

Console.WriteLine("=== webapiclientcore_protobuf Test ===");

try
{
    var t0 = typeof(ThreadLocalSpanSerializer);
    Console.WriteLine($"[PASS] ThreadLocalSpanSerializer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}