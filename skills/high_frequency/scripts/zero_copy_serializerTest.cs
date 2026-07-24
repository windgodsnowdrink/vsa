#load "zero_copy_serializer.cs"

Console.WriteLine("=== zero_copy_serializer Test ===");

try
{
    var t0 = typeof(ZeroCopySerializer);
    Console.WriteLine($"[PASS] ZeroCopySerializer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}