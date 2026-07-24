#load "zero_copy_messagepack.cs"

Console.WriteLine("=== zero_copy_messagepack Test ===");

try
{
    var t0 = typeof(ZeroCopyMessagePackSerializer);
    Console.WriteLine($"[PASS] ZeroCopyMessagePackSerializer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}