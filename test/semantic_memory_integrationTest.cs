#load "semantic_memory_integration.cs"

Console.WriteLine("=== semantic_memory_integration Test ===");

try
{
    var t0 = typeof(SemanticMemoryIntegration.SemanticMemoryOptions);
    Console.WriteLine($"[PASS] SemanticMemoryOptions 存在");
    var t1 = typeof(SemanticMemoryIntegration.SemanticMemoryService);
    Console.WriteLine($"[PASS] SemanticMemoryService 存在");
    var t2 = typeof(SemanticMemoryIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(SemanticMemoryIntegration.ISemanticMemoryService);
    Console.WriteLine($"[PASS] ISemanticMemoryService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}