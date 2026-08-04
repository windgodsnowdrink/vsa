#load "tracing_extension.cs"

Console.WriteLine("=== tracing_extension Test ===");

try
{
    var t0 = typeof(TracingExtensions);
    Console.WriteLine($"[PASS] TracingExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}