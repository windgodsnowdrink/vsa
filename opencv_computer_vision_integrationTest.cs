#load "opencv_computer_vision_integration.cs"

Console.WriteLine("=== opencv_computer_vision_integration Test ===");

try
{
    var t0 = typeof(ComputerVisionService);
    Console.WriteLine($"[PASS] ComputerVisionService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}