#load "version_control_manager.cs"

Console.WriteLine("=== version_control_manager Test ===");

try
{
    var t0 = typeof(VersionControlManager);
    Console.WriteLine($"[PASS] VersionControlManager 存在");
    var t1 = typeof(SnapshotCompressionEngine);
    Console.WriteLine($"[PASS] SnapshotCompressionEngine 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}