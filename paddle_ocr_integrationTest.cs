#load "paddle_ocr_integration.cs"

Console.WriteLine("=== paddle_ocr_integration Test ===");

try
{
    var t0 = typeof(OcrProcessor);
    Console.WriteLine($"[PASS] OcrProcessor 存在");
    var t1 = typeof(OcrResult);
    Console.WriteLine($"[PASS] OcrResult 存在");
    var t2 = typeof(OcrResultPooledPolicy);
    Console.WriteLine($"[PASS] OcrResultPooledPolicy 存在");
    var t3 = typeof(OcrFrame);
    Console.WriteLine($"[PASS] OcrFrame record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}