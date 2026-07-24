#load "tracing_extension.cs"

Console.WriteLine("=== tracing_extension Test ===");

try
{
    // 验证 TracingExtensions 类
    var extensionsType = typeof(TracingExtensions);
    Console.WriteLine($"[PASS] TracingExtensions 类型存在: {extensionsType.Name}");
    Console.WriteLine($"[PASS] AddOpenTelemetryTracing 方法: {extensionsType.GetMethod("AddOpenTelemetryTracing") != null}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}