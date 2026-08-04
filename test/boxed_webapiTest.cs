#load "boxed_webapi.cs"

Console.WriteLine("=== boxed_webapi Test ===");

try
{
    var t0 = typeof(TodoEndpoint);
    Console.WriteLine($"[PASS] TodoEndpoint 存在");
    var t1 = typeof(ApiChannelProcessor);
    Console.WriteLine($"[PASS] ApiChannelProcessor 存在");
    var t2 = typeof(ApiMessage);
    Console.WriteLine($"[PASS] ApiMessage 存在");
    var t3 = typeof(TodoRequest);
    Console.WriteLine($"[PASS] TodoRequest record 存在");
    var t4 = typeof(TodoResponse);
    Console.WriteLine($"[PASS] TodoResponse record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}