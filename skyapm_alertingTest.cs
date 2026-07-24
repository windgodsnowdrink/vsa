#load "skyapm_alerting.cs"

Console.WriteLine("=== skyapm_alerting Test ===");

try
{
    var t0 = typeof(SmartAlertEngine);
    Console.WriteLine($"[PASS] SmartAlertEngine 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}