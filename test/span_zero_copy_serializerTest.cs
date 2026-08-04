#load "span_zero_copy_serializer.cs"

Console.WriteLine("=== span_zero_copy_serializer Test ===");

try
{
    var t0 = typeof(SpanZeroCopySerializer);
    Console.WriteLine($"[PASS] SpanZeroCopySerializer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}