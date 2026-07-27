#load "facedetect_torchsharp_integration.cs"

Console.WriteLine("=== facedetect_torchsharp_integration Test ===");

try
{
    var t0 = typeof(FaceDetectionService);
    Console.WriteLine($"[PASS] FaceDetectionService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(IFaceDetectionService);
    Console.WriteLine($"[PASS] IFaceDetectionService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}