#load "version_metadata_service.cs"

Console.WriteLine("=== version_metadata_service Test ===");

try
{
    var t0 = typeof(VersionMetadataService);
    Console.WriteLine($"[PASS] VersionMetadataService 存在");
    var t1 = typeof(VersionMetadata);
    Console.WriteLine($"[PASS] VersionMetadata 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}