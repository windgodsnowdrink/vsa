#load "silknet_graphics.cs"

Console.WriteLine("=== silknet_graphics Test ===");

try
{
    var t0 = typeof(GraphicsEngine);
    Console.WriteLine($"[PASS] GraphicsEngine 存在");
    var t1 = typeof(RenderProcessor);
    Console.WriteLine($"[PASS] RenderProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}