#load "sqlite_bulk_writer.cs"

Console.WriteLine("=== sqlite_bulk_writer Test ===");

try
{
    var t0 = typeof(SqliteBulkWriter);
    Console.WriteLine($"[PASS] SqliteBulkWriter 存在");
    var t1 = typeof(SqliteBulkWriterExtensions);
    Console.WriteLine($"[PASS] SqliteBulkWriterExtensions 存在");
    var t2 = typeof(SampleData);
    Console.WriteLine($"[PASS] SampleData 存在");
    var t3 = typeof(SqliteController);
    Console.WriteLine($"[PASS] SqliteController 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}