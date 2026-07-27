#load "opencv_extended.cs"

Console.WriteLine("=== opencv_extended Test ===");

try
{
    var t0 = typeof(ExtendedOcrProcessor);
    Console.WriteLine($"[PASS] ExtendedOcrProcessor 存在");
    var t1 = typeof(AnalysisResult);
    Console.WriteLine($"[PASS] AnalysisResult 存在");
    var t2 = typeof(AnalysisResultPooledPolicy);
    Console.WriteLine($"[PASS] AnalysisResultPooledPolicy 存在");
    var t3 = typeof(AnalysisFrame);
    Console.WriteLine($"[PASS] AnalysisFrame record 存在");
    var t4 = typeof(Color);
    Console.WriteLine($"[PASS] Color struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}