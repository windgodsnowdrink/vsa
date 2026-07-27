#load "SourceGenerator_complete.cs"

Console.WriteLine("=== SourceGenerator_complete Test ===");

try
{
    var t0 = typeof(MultiLanguageAttribute);
    Console.WriteLine($"[PASS] MultiLanguageAttribute 存在");
    var t1 = typeof(CompleteGenerator);
    Console.WriteLine($"[PASS] CompleteGenerator 存在");
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