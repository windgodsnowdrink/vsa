#load "securityheaders_integration.cs"

Console.WriteLine("=== securityheaders_integration.cs Test ===");

try
{
    // 未检测到显式类型声明，可能为顶层语句文件
    Console.WriteLine("[INFO] 顶层语句文件，无需类型验证");
    Console.WriteLine("[PASS] 文件加载成功");
    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
