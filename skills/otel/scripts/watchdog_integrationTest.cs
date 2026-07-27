#load "watchdog_integration.cs"

Console.WriteLine("=== watchdog_integration Test ===");

try
{
    // 该文件配置 WatchDog 监控服务
    // 验证文件可成功加载
    Console.WriteLine("[PASS] watchdog_integration.cs 文件加载成功");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}