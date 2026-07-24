#load "watchdog_alert_rules.cs"

Console.WriteLine("=== watchdog_alert_rules Test ===");

try
{
    // 该文件配置 WatchDog 告警规则
    // 验证文件可成功加载
    Console.WriteLine("[PASS] watchdog_alert_rules.cs 文件加载成功");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}