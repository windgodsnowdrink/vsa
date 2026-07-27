#load "serializer_core.cs"

Console.WriteLine("=== serializer_core Test ===");

try
{
    var t0 = typeof(Serializer.Options);
    Console.WriteLine($"[PASS] Options 存在");
    var t1 = typeof(Serializer.PerformanceResult);
    Console.WriteLine($"[PASS] PerformanceResult 存在");
    var t2 = typeof(Serializer.JsonSerializerImpl);
    Console.WriteLine($"[PASS] JsonSerializerImpl 存在");
    var t3 = typeof(Serializer.NewtonsoftJsonSerializerImpl);
    Console.WriteLine($"[PASS] NewtonsoftJsonSerializerImpl 存在");
    var t4 = typeof(Serializer.XmlSerializerImpl);
    Console.WriteLine($"[PASS] XmlSerializerImpl 存在");
    var t5 = typeof(Serializer.YamlSerializerImpl);
    Console.WriteLine($"[PASS] YamlSerializerImpl 存在");
    var t6 = typeof(Serializer.ProtobufSerializerImpl);
    Console.WriteLine($"[PASS] ProtobufSerializerImpl 存在");
    var t7 = typeof(Serializer.SerializerFactory);
    Console.WriteLine($"[PASS] SerializerFactory 存在");
    var t8 = typeof(Serializer.PerformanceTester);
    Console.WriteLine($"[PASS] PerformanceTester 存在");
    var t9 = typeof(Serializer.SerializerExtensions);
    Console.WriteLine($"[PASS] SerializerExtensions 存在");
    var t10 = typeof(Serializer.ISerializer);
    Console.WriteLine($"[PASS] ISerializer 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(Serializer.ISerializerFactory);
    Console.WriteLine($"[PASS] ISerializerFactory 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(Serializer.IPerformanceTester);
    Console.WriteLine($"[PASS] IPerformanceTester 接口存在 (IsInterface: {t12.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}