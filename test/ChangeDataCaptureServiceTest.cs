#load "ChangeDataCaptureService.cs"

Console.WriteLine("=== ChangeDataCaptureService Test ===");

try
{
    var t0 = typeof(ChoETL.Integration.ChangeDataCaptureService);
    Console.WriteLine($"[PASS] ChangeDataCaptureService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}