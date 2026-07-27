#load "otel_extensions.cs"

Console.WriteLine("=== otel_extensions Test ===");

try
{
    var t0 = typeof(OpenTelemetryOptions);
    Console.WriteLine($"[PASS] OpenTelemetryOptions 存在");
    var t1 = typeof(OpenTelemetryConfiguration);
    Console.WriteLine($"[PASS] OpenTelemetryConfiguration 存在");
    var t2 = typeof(AlertRule);
    Console.WriteLine($"[PASS] AlertRule 存在");
    var t3 = typeof(Alert);
    Console.WriteLine($"[PASS] Alert 存在");
    var t4 = typeof(OpenTelemetryService);
    Console.WriteLine($"[PASS] OpenTelemetryService 存在");
    var t5 = typeof(OpenTelemetryTracingService);
    Console.WriteLine($"[PASS] OpenTelemetryTracingService 存在");
    var t6 = typeof(OpenTelemetryMetricsService);
    Console.WriteLine($"[PASS] OpenTelemetryMetricsService 存在");
    var t7 = typeof(OpenTelemetryLoggingService);
    Console.WriteLine($"[PASS] OpenTelemetryLoggingService 存在");
    var t8 = typeof(OpenTelemetryAlertingService);
    Console.WriteLine($"[PASS] OpenTelemetryAlertingService 存在");
    var t9 = typeof(OpenTelemetryServiceCollectionExtensions);
    Console.WriteLine($"[PASS] OpenTelemetryServiceCollectionExtensions 存在");
    var t10 = typeof(IOpenTelemetryService);
    Console.WriteLine($"[PASS] IOpenTelemetryService 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(IOpenTelemetryTracingService);
    Console.WriteLine($"[PASS] IOpenTelemetryTracingService 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(IOpenTelemetryMetricsService);
    Console.WriteLine($"[PASS] IOpenTelemetryMetricsService 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(IOpenTelemetryLoggingService);
    Console.WriteLine($"[PASS] IOpenTelemetryLoggingService 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(IOpenTelemetryAlertingService);
    Console.WriteLine($"[PASS] IOpenTelemetryAlertingService 接口存在 (IsInterface: {t14.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}