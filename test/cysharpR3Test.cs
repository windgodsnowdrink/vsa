#load "cysharpR3.cs"

Console.WriteLine("=== cysharpR3 Test ===");

try
{
    var t0 = typeof(SignalRDemo.CommandData);
    Console.WriteLine($"[PASS] CommandData 存在");
    var t1 = typeof(SignalRDemo.DataModel);
    Console.WriteLine($"[PASS] DataModel 存在");
    var t2 = typeof(SignalRDemo.CommandQueue);
    Console.WriteLine($"[PASS] CommandQueue 存在");
    var t3 = typeof(SignalRDemo.DataGenerator);
    Console.WriteLine($"[PASS] DataGenerator 存在");
    var t4 = typeof(SignalRDemo.DataConsumer);
    Console.WriteLine($"[PASS] DataConsumer 存在");
    var t5 = typeof(SignalRDemo.CommandSender);
    Console.WriteLine($"[PASS] CommandSender 存在");
    var t6 = typeof(SignalRDemo.CommandResponseBuilder);
    Console.WriteLine($"[PASS] CommandResponseBuilder 存在");
    var t7 = typeof(SignalRDemo.ResponseDTO);
    Console.WriteLine($"[PASS] ResponseDTO 存在");
    var t8 = typeof(SignalRDemo.CommandHandler);
    Console.WriteLine($"[PASS] CommandHandler 存在");
    var t9 = typeof(SignalRDemo.QueryHandler);
    Console.WriteLine($"[PASS] QueryHandler 存在");
    var t10 = typeof(SignalRDemo.DataStore);
    Console.WriteLine($"[PASS] DataStore 存在");
    var t11 = typeof(SignalRDemo.MyLoggerConfig);
    Console.WriteLine($"[PASS] MyLoggerConfig 存在");
    var t12 = typeof(SignalRDemo.ZLoggerOutputExtensions);
    Console.WriteLine($"[PASS] ZLoggerOutputExtensions 存在");
    var t13 = typeof(SignalRDemo.DataParser);
    Console.WriteLine($"[PASS] DataParser 存在");
    var t14 = typeof(SignalRDemo.ByteUtil);
    Console.WriteLine($"[PASS] ByteUtil 存在");
    var t15 = typeof(SignalRDemo.CommandHandlerManager);
    Console.WriteLine($"[PASS] CommandHandlerManager 存在");
    var t16 = typeof(SignalRDemo.EchartDataGenerator);
    Console.WriteLine($"[PASS] EchartDataGenerator 存在");
    var t17 = typeof(SignalRDemo.DataHub);
    Console.WriteLine($"[PASS] DataHub 存在");
    var t18 = typeof(SignalRDemo.MainPage);
    Console.WriteLine($"[PASS] MainPage 存在");
    var t19 = typeof(SignalRDemo.DataStreamingService);
    Console.WriteLine($"[PASS] DataStreamingService 存在");
    var t20 = typeof(SignalRDemo.DataPoint);
    Console.WriteLine($"[PASS] DataPoint 存在");
    var t21 = typeof(SignalRDemo.Period);
    Console.WriteLine($"[PASS] Period 存在");
    var t22 = typeof(SignalRDemo.AddCommand);
    Console.WriteLine($"[PASS] AddCommand 存在");
    var t23 = typeof(SignalRDemo.SubtractCommand);
    Console.WriteLine($"[PASS] SubtractCommand 存在");
    var t24 = typeof(SignalRDemo.CommandReceiver);
    Console.WriteLine($"[PASS] CommandReceiver 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}