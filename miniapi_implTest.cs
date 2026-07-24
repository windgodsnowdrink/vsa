#load "miniapi_impl.cs"

Console.WriteLine("=== miniapi_impl Test ===");

try
{
    var t0 = typeof(MinimalApiExtensions);
    Console.WriteLine($"[PASS] MinimalApiExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}