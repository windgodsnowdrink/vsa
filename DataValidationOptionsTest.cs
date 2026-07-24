#load "DataValidationOptions.cs"

Console.WriteLine("=== DataValidationOptions Test ===");

try
{
    var t0 = typeof(ChoETL.Integration.DataValidationOptions);
    Console.WriteLine($"[PASS] DataValidationOptions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}