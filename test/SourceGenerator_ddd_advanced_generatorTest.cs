#load "SourceGenerator_ddd_advanced_generator.cs"

Console.WriteLine("=== SourceGenerator_ddd_advanced_generator Test ===");

try
{
    var t0 = typeof(DomainEventGenerator);
    Console.WriteLine($"[PASS] DomainEventGenerator 存在");
    var t1 = typeof(ValueObjectGenerator);
    Console.WriteLine($"[PASS] ValueObjectGenerator 存在");
    var t2 = typeof(MicroserviceGenerator);
    Console.WriteLine($"[PASS] MicroserviceGenerator 存在");
    var t3 = typeof(DomainEventSyntaxReceiver);
    Console.WriteLine($"[PASS] DomainEventSyntaxReceiver 存在");
    var t4 = typeof(ValueObjectSyntaxReceiver);
    Console.WriteLine($"[PASS] ValueObjectSyntaxReceiver 存在");
    var t5 = typeof(MicroserviceSyntaxReceiver);
    Console.WriteLine($"[PASS] MicroserviceSyntaxReceiver 存在");
    var t6 = typeof(EntityInfo);
    Console.WriteLine($"[PASS] EntityInfo 存在");
    var t7 = typeof(ValueObjectInfo);
    Console.WriteLine($"[PASS] ValueObjectInfo 存在");
    var t8 = typeof(ServiceInfo);
    Console.WriteLine($"[PASS] ServiceInfo 存在");
    var t9 = typeof(Get);
    Console.WriteLine($"[PASS] Get 存在");
    var t10 = typeof(Create);
    Console.WriteLine($"[PASS] Create 存在");
    var t11 = typeof(PipeNetworkGenerator);
    Console.WriteLine($"[PASS] PipeNetworkGenerator 存在");
    var t12 = typeof(ChannelProcessorGenerator);
    Console.WriteLine($"[PASS] ChannelProcessorGenerator 存在");
    var t13 = typeof(ObjectPoolGenerator);
    Console.WriteLine($"[PASS] ObjectPoolGenerator 存在");
    var t14 = typeof(ArrayPooledObjectPolicy);
    Console.WriteLine($"[PASS] ArrayPooledObjectPolicy 存在");
    var t15 = typeof(I);
    Console.WriteLine($"[PASS] I 接口存在 (IsInterface: {t15.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}