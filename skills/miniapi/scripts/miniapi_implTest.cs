#load "miniapi_impl.cs"

Console.WriteLine("=== miniapi_impl Test ===");

try
{
    var t0 = typeof(Todo);
    Console.WriteLine($"[PASS] Todo 存在");
    var t1 = typeof(TodoDto);
    Console.WriteLine($"[PASS] TodoDto 存在");
    var t2 = typeof(TodoService);
    Console.WriteLine($"[PASS] TodoService 存在");
    var t3 = typeof(WeatherForecastService);
    Console.WriteLine($"[PASS] WeatherForecastService 存在");
    var t4 = typeof(WeatherForecast);
    Console.WriteLine($"[PASS] WeatherForecast 存在");
    var t5 = typeof(MemoryHealthCheck);
    Console.WriteLine($"[PASS] MemoryHealthCheck 存在");
    var t6 = typeof(DiskHealthCheck);
    Console.WriteLine($"[PASS] DiskHealthCheck 存在");
    var t7 = typeof(MiniApiExtensions);
    Console.WriteLine($"[PASS] MiniApiExtensions 存在");
    var t8 = typeof(where);
    Console.WriteLine($"[PASS] where 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}