#load "swashbucklerdiary_integration.cs"

Console.WriteLine("=== swashbucklerdiary_integration Test ===");

try
{
    var t0 = typeof(SwashbucklerDiary.Integration.SwashbucklerDiaryOptions);
    Console.WriteLine($"[PASS] SwashbucklerDiaryOptions 存在");
    var t1 = typeof(SwashbucklerDiary.Integration.SwashbucklerDiaryService);
    Console.WriteLine($"[PASS] SwashbucklerDiaryService 存在");
    var t2 = typeof(SwashbucklerDiary.Integration.SwashbucklerDiaryExtensions);
    Console.WriteLine($"[PASS] SwashbucklerDiaryExtensions 存在");
    var t3 = typeof(SwashbucklerDiary.Integration.ExampleUsage);
    Console.WriteLine($"[PASS] ExampleUsage 存在");
    var t4 = typeof(SwashbucklerDiary.Integration.ISwashbucklerDiaryService);
    Console.WriteLine($"[PASS] ISwashbucklerDiaryService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}