#load "opencv_biometric_processor.cs"

Console.WriteLine("=== opencv_biometric_processor Test ===");

try
{
    var t0 = typeof(BiometricProcessor);
    Console.WriteLine($"[PASS] BiometricProcessor 存在");
    var t1 = typeof(BiometricResult);
    Console.WriteLine($"[PASS] BiometricResult 存在");
    var t2 = typeof(BiometricFrame);
    Console.WriteLine($"[PASS] BiometricFrame record 存在");
    var t3 = typeof(IrisRecognitionResult);
    Console.WriteLine($"[PASS] IrisRecognitionResult record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}