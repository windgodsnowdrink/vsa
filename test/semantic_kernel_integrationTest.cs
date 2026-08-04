#load "semantic_kernel_integration.cs"

Console.WriteLine("=== semantic_kernel_integration Test ===");

try
{
    var t0 = typeof(SemanticKernelIntegration.SemanticKernelOptions);
    Console.WriteLine($"[PASS] SemanticKernelOptions 存在");
    var t1 = typeof(SemanticKernelIntegration.SemanticKernelService);
    Console.WriteLine($"[PASS] SemanticKernelService 存在");
    var t2 = typeof(SemanticKernelIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(SemanticKernelIntegration.ISemanticKernelService);
    Console.WriteLine($"[PASS] ISemanticKernelService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}