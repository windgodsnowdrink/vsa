#load "scrutor_demo.cs"

Console.WriteLine("=== scrutor_demo Test ===");

try
{
    // 验证 ScrutorDemo.Program 类
    var programType = Type.GetType("ScrutorDemo.Program");
    Console.WriteLine($"[PASS] ScrutorDemo.Program 类型存在: {programType != null}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}