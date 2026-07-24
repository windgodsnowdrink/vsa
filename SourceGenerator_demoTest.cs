#load "SourceGenerator_demo.cs"

Console.WriteLine("=== SourceGenerator_demo Test ===");

try
{
    var t0 = typeof(DtoGenerator);
    Console.WriteLine($"[PASS] DtoGenerator 存在");
    var t1 = typeof(DtoSyntaxReceiver);
    Console.WriteLine($"[PASS] DtoSyntaxReceiver 存在");
    var t2 = typeof(DtoInfo);
    Console.WriteLine($"[PASS] DtoInfo 存在");
    var t3 = typeof(PropertyInfo);
    Console.WriteLine($"[PASS] PropertyInfo 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}