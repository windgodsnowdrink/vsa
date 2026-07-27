#load "fastendpoints_impl.cs"

Console.WriteLine("=== fastendpoints_impl Test ===");

try
{
    var t0 = typeof(FastEndpointsImpl.TodoDto);
    Console.WriteLine($"[PASS] TodoDto 存在");
    var t1 = typeof(FastEndpointsImpl.FastEndpointsExtensions);
    Console.WriteLine($"[PASS] FastEndpointsExtensions 存在");
    var t2 = typeof(FastEndpointsImpl.ITodoService);
    Console.WriteLine($"[PASS] ITodoService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}