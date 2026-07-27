#load "vulkan_integration.cs"

Console.WriteLine("=== vulkan_integration Test ===");

try
{
    var t0 = typeof(VulkanContext);
    Console.WriteLine($"[PASS] VulkanContext 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}