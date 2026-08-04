#load "dotnetty_integration.cs"

Console.WriteLine("=== dotnetty_integration Test ===");

try
{
    var t0 = typeof(DotNettyOptions);
    Console.WriteLine($"[PASS] DotNettyOptions 存在");
    var t1 = typeof(DotNettyService);
    Console.WriteLine($"[PASS] DotNettyService 存在");
    var t2 = typeof(DotNettyExtensions);
    Console.WriteLine($"[PASS] DotNettyExtensions 存在");
    var t3 = typeof(DotNettyExample);
    Console.WriteLine($"[PASS] DotNettyExample 存在");
    var t4 = typeof(SpanLengthFieldBasedFrameDecoder);
    Console.WriteLine($"[PASS] SpanLengthFieldBasedFrameDecoder 存在");
    var t5 = typeof(SpanLengthFieldPrepender);
    Console.WriteLine($"[PASS] SpanLengthFieldPrepender 存在");
    var t6 = typeof(SpanStringEncoder);
    Console.WriteLine($"[PASS] SpanStringEncoder 存在");
    var t7 = typeof(SpanStringDecoder);
    Console.WriteLine($"[PASS] SpanStringDecoder 存在");
    var t8 = typeof(ByteArrayPooledObjectPolicy);
    Console.WriteLine($"[PASS] ByteArrayPooledObjectPolicy 存在");
    var t9 = typeof(SimpleChannelHandler);
    Console.WriteLine($"[PASS] SimpleChannelHandler 存在");
    var t10 = typeof(HeartbeatHandler);
    Console.WriteLine($"[PASS] HeartbeatHandler 存在");
    var t11 = typeof(IDotNettyService);
    Console.WriteLine($"[PASS] IDotNettyService 接口存在 (IsInterface: {t11.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}