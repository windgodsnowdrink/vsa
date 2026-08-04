#load "spanjson_serializer.cs"

Console.WriteLine("=== spanjson_serializer Test ===");

try
{
    var t0 = typeof(SpanJsonSerializer);
    Console.WriteLine($"[PASS] SpanJsonSerializer 存在");
    var t1 = typeof(ArrayPooledObjectPolicy);
    Console.WriteLine($"[PASS] ArrayPooledObjectPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}