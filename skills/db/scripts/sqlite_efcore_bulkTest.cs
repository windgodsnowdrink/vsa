#load "sqlite_efcore_bulk.cs"

Console.WriteLine("=== sqlite_efcore_bulk Test ===");

try
{
    var t0 = typeof(Product);
    Console.WriteLine($"[PASS] Product 存在");
    var t1 = typeof(AppDbContext);
    Console.WriteLine($"[PASS] AppDbContext 存在");
    var t2 = typeof(BulkOperation);
    Console.WriteLine($"[PASS] BulkOperation 存在");
    var t3 = typeof(BulkOperationService);
    Console.WriteLine($"[PASS] BulkOperationService 存在");
    var t4 = typeof(DbContextPoolPolicy);
    Console.WriteLine($"[PASS] DbContextPoolPolicy 存在");
    var t5 = typeof(BulkOperationServiceExtensions);
    Console.WriteLine($"[PASS] BulkOperationServiceExtensions 存在");
    var t6 = typeof(BulkController);
    Console.WriteLine($"[PASS] BulkController 存在");
    var t7 = typeof(BulkOperationType);
    Console.WriteLine($"[PASS] BulkOperationType enum 存在 (IsEnum: {t7.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}