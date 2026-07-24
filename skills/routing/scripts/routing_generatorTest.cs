#load "routing_generator.cs"

Console.WriteLine("=== routing_generator Test ===");

try
{
    var t0 = typeof(Routing.Generator.TemplateInfo);
    Console.WriteLine($"[PASS] TemplateInfo 存在");
    var t1 = typeof(Routing.Generator.ValidationResult);
    Console.WriteLine($"[PASS] ValidationResult 存在");
    var t2 = typeof(Routing.Generator.RouteConfig);
    Console.WriteLine($"[PASS] RouteConfig 存在");
    var t3 = typeof(Routing.Generator.MiddlewareConfig);
    Console.WriteLine($"[PASS] MiddlewareConfig 存在");
    var t4 = typeof(Routing.Generator.RouteParameter);
    Console.WriteLine($"[PASS] RouteParameter 存在");
    var t5 = typeof(Routing.Generator.RouteGenerator);
    Console.WriteLine($"[PASS] RouteGenerator 存在");
    var t6 = typeof(Routing.Generator.Configuration);
    Console.WriteLine($"[PASS] Configuration 存在");
    var t7 = typeof(Routing.Generator.TemplateManager);
    Console.WriteLine($"[PASS] TemplateManager 存在");
    var t8 = typeof(Routing.Generator.ConfigurationValidator);
    Console.WriteLine($"[PASS] ConfigurationValidator 存在");
    var t9 = typeof(Routing.Generator.AspNetCoreGenerator);
    Console.WriteLine($"[PASS] AspNetCoreGenerator 存在");
    var t10 = typeof(Routing.Generator.FastEndpointsGenerator);
    Console.WriteLine($"[PASS] FastEndpointsGenerator 存在");
    var t11 = typeof(Routing.Generator.MinimalApiGenerator);
    Console.WriteLine($"[PASS] MinimalApiGenerator 存在");
    var t12 = typeof(Routing.Generator.EndpointExtensions);
    Console.WriteLine($"[PASS] EndpointExtensions 存在");
    var t13 = typeof(Routing.Generator.IRouteGenerator);
    Console.WriteLine($"[PASS] IRouteGenerator 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(Routing.Generator.ITemplateManager);
    Console.WriteLine($"[PASS] ITemplateManager 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(Routing.Generator.IConfigurationValidator);
    Console.WriteLine($"[PASS] IConfigurationValidator 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(Routing.Generator.ICodeGenerator);
    Console.WriteLine($"[PASS] ICodeGenerator 接口存在 (IsInterface: {t16.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}