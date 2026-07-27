#load "watchdog_distributed_tracing.cs"

Console.WriteLine("=== watchdog_distributed_tracing Test ===");

try
{
    // 该文件配置 WatchDog 与 OpenTelemetry 集成
    // 验证文件可成功加载
    Console.WriteLine("[PASS] watchdog_distributed_tracing.cs 文件加载成功");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}