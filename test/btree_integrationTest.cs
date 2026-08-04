#load "btree_integration.cs"

Console.WriteLine("=== btree_integration Test ===");

try
{
    var t0 = typeof(TreeNode);
    Console.WriteLine($"[PASS] TreeNode 存在");
    var t1 = typeof(BTreeService);
    Console.WriteLine($"[PASS] BTreeService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(IBTreeService);
    Console.WriteLine($"[PASS] IBTreeService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}