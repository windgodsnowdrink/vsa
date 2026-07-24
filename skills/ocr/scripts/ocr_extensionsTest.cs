#load "ocr_extensions.cs"

Console.WriteLine("=== ocr_extensions Test ===");

try
{
    var t0 = typeof(OCROptions);
    Console.WriteLine($"[PASS] OCROptions 存在");
    var t1 = typeof(OCRTextLine);
    Console.WriteLine($"[PASS] OCRTextLine 存在");
    var t2 = typeof(OCRResult);
    Console.WriteLine($"[PASS] OCRResult 存在");
    var t3 = typeof(ImageProcessor);
    Console.WriteLine($"[PASS] ImageProcessor 存在");
    var t4 = typeof(TextRecognizer);
    Console.WriteLine($"[PASS] TextRecognizer 存在");
    var t5 = typeof(OCRService);
    Console.WriteLine($"[PASS] OCRService 存在");
    var t6 = typeof(OCRServiceCollectionExtensions);
    Console.WriteLine($"[PASS] OCRServiceCollectionExtensions 存在");
    var t7 = typeof(IOCRService);
    Console.WriteLine($"[PASS] IOCRService 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(IImageProcessor);
    Console.WriteLine($"[PASS] IImageProcessor 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(ITextRecognizer);
    Console.WriteLine($"[PASS] ITextRecognizer 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(IOCRResult);
    Console.WriteLine($"[PASS] IOCRResult 接口存在 (IsInterface: {t10.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}