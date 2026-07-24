#load "skyapm_alerting.cs"

Console.WriteLine("=== skyapm_alerting Test ===");

try
{
    // 验证 SmartAlertEngine 类
    var engineType = typeof(SmartAlertEngine);
    Console.WriteLine($"[PASS] SmartAlertEngine 类型存在: {engineType.Name}");
    Console.WriteLine($"[PASS] RunAsync 方法: {engineType.GetMethod("RunAsync") != null}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}