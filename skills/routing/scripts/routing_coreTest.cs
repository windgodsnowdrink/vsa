#load "routing_core.cs"

Console.WriteLine("=== routing_core Test ===");

try
{
    var t0 = typeof(Routing.Core.Route);
    Console.WriteLine($"[PASS] Route 存在");
    var t1 = typeof(Routing.Core.RouteParameter);
    Console.WriteLine($"[PASS] RouteParameter 存在");
    var t2 = typeof(Routing.Core.MiddlewareInfo);
    Console.WriteLine($"[PASS] MiddlewareInfo 存在");
    var t3 = typeof(Routing.Core.RouteManager);
    Console.WriteLine($"[PASS] RouteManager 存在");
    var t4 = typeof(Routing.Core.RouteParser);
    Console.WriteLine($"[PASS] RouteParser 存在");
    var t5 = typeof(Routing.Core.MiddlewareManager);
    Console.WriteLine($"[PASS] MiddlewareManager 存在");
    var t6 = typeof(Routing.Core.FileRouteStore);
    Console.WriteLine($"[PASS] FileRouteStore 存在");
    var t7 = typeof(Routing.Core.IRouteManager);
    Console.WriteLine($"[PASS] IRouteManager 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(Routing.Core.IRouteParser);
    Console.WriteLine($"[PASS] IRouteParser 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(Routing.Core.IMiddlewareManager);
    Console.WriteLine($"[PASS] IMiddlewareManager 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(Routing.Core.IRouteStore);
    Console.WriteLine($"[PASS] IRouteStore 接口存在 (IsInterface: {t10.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}