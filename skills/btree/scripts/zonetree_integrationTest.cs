#load "zonetree_integration.cs"

Console.WriteLine("=== zonetree_integration Test ===");

try
{
    var t0 = typeof(TreeNode);
    Console.WriteLine($"[PASS] TreeNode 存在");
    var t1 = typeof(TreeService);
    Console.WriteLine($"[PASS] TreeService 存在");
    var t2 = typeof(TreeServiceExtensions);
    Console.WriteLine($"[PASS] TreeServiceExtensions 存在");
    var t3 = typeof(ITreeService);
    Console.WriteLine($"[PASS] ITreeService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}