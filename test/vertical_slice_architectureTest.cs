#load "vertical_slice_architecture.cs"

Console.WriteLine("=== vertical_slice_architecture Test ===");

try
{
    var t0 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t1 = typeof(TodoDbContext);
    Console.WriteLine($"[PASS] TodoDbContext 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}