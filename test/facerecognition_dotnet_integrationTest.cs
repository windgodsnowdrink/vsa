#load "facerecognition_dotnet_integration.cs"

Console.WriteLine("=== facerecognition_dotnet_integration Test ===");

try
{
    var t0 = typeof(FaceRecognitionService);
    Console.WriteLine($"[PASS] FaceRecognitionService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(IFaceRecognitionService);
    Console.WriteLine($"[PASS] IFaceRecognitionService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}