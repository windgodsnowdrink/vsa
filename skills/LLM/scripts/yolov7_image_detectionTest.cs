#load "yolov7_image_detection.cs"

Console.WriteLine("=== yolov7_image_detection Test ===");

try
{
    var t0 = typeof(DetectionResult);
    Console.WriteLine($"[PASS] DetectionResult 存在");
    var t1 = typeof(YoloV7DetectionService);
    Console.WriteLine($"[PASS] YoloV7DetectionService 存在");
    var t2 = typeof(with);
    Console.WriteLine($"[PASS] with 存在");
    var t3 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t4 = typeof(IObjectDetectionService);
    Console.WriteLine($"[PASS] IObjectDetectionService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}