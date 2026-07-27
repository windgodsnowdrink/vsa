#load "webhook_integration.cs"

Console.WriteLine("=== webhook_integration Test ===");

try
{
    var t0 = typeof(WebhookOptions);
    Console.WriteLine($"[PASS] WebhookOptions 存在");
    var t1 = typeof(WebhookNotification);
    Console.WriteLine($"[PASS] WebhookNotification 存在");
    var t2 = typeof(WebhookMetrics);
    Console.WriteLine($"[PASS] WebhookMetrics 存在");
    var t3 = typeof(WebhookDispatcher);
    Console.WriteLine($"[PASS] WebhookDispatcher 存在");
    var t4 = typeof(WebhookExtensions);
    Console.WriteLine($"[PASS] WebhookExtensions 存在");
    var t5 = typeof(WebhookBackgroundService);
    Console.WriteLine($"[PASS] WebhookBackgroundService 存在");
    var t6 = typeof(IWebhookDispatcher);
    Console.WriteLine($"[PASS] IWebhookDispatcher 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}