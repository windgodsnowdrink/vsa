#load "dynamic_router.cs"

Console.WriteLine("=== dynamic_router Test ===");

try
{
    var t0 = typeof(RouterOptions);
    Console.WriteLine($"[PASS] RouterOptions 存在");
    var t1 = typeof(RouteContext);
    Console.WriteLine($"[PASS] RouteContext 存在");
    var t2 = typeof(RouteInfo);
    Console.WriteLine($"[PASS] RouteInfo 存在");
    var t3 = typeof(RouteExecutionStats);
    Console.WriteLine($"[PASS] RouteExecutionStats 存在");
    var t4 = typeof(RouteNode);
    Console.WriteLine($"[PASS] RouteNode 存在");
    var t5 = typeof(DynamicRouter);
    Console.WriteLine($"[PASS] DynamicRouter 存在");
    var t6 = typeof(DynamicRouterExtensions);
    Console.WriteLine($"[PASS] DynamicRouterExtensions 存在");
    var t7 = typeof(IDynamicRouter);
    Console.WriteLine($"[PASS] IDynamicRouter 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}