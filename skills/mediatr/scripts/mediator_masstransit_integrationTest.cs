#load "mediator_masstransit_integration.cs"

Console.WriteLine("=== mediator_masstransit_integration Test ===");

try
{
    var t0 = typeof(App.WeatherUpdatedConsumer);
    Console.WriteLine($"[PASS] WeatherUpdatedConsumer 存在");
    var t1 = typeof(App.WeatherUpdateLogger);
    Console.WriteLine($"[PASS] WeatherUpdateLogger 存在");
    var t2 = typeof(App.WeatherUpdateStorage);
    Console.WriteLine($"[PASS] WeatherUpdateStorage 存在");
    var t3 = typeof(App.Db);
    Console.WriteLine($"[PASS] Db 存在");
    var t4 = typeof(App.WeatherUpdated);
    Console.WriteLine($"[PASS] WeatherUpdated record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}