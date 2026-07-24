#load "acme_dotnet_integration.cs"

Console.WriteLine("=== acme_dotnet_integration Test ===");

try
{
    var t0 = typeof(AcmeDotNetService);
    Console.WriteLine($"[PASS] AcmeDotNetService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}