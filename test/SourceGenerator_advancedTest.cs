#load "SourceGenerator_advanced.cs"

Console.WriteLine("=== SourceGenerator_advanced Test ===");

try
{
    var t0 = typeof(AdvancedDtoGenerator);
    Console.WriteLine($"[PASS] AdvancedDtoGenerator 存在");
    var t1 = typeof(DtoInfo);
    Console.WriteLine($"[PASS] DtoInfo 存在");
    var t2 = typeof(PropertyInfo);
    Console.WriteLine($"[PASS] PropertyInfo 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}