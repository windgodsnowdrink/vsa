#load "filemanager_aot.cs"

Console.WriteLine("=== filemanager_aot Test ===");

try
{
    var t0 = typeof(FileManager.AOT.FileManagerOptions);
    Console.WriteLine($"[PASS] FileManagerOptions 存在");
    var t1 = typeof(FileManager.AOT.FileInfoDto);
    Console.WriteLine($"[PASS] FileInfoDto 存在");
    var t2 = typeof(FileManager.AOT.FileManagerCommandResult);
    Console.WriteLine($"[PASS] FileManagerCommandResult 存在");
    var t3 = typeof(FileManager.AOT.FileManagerService);
    Console.WriteLine($"[PASS] FileManagerService 存在");
    var t4 = typeof(FileManager.AOT.FileManagerAotEngine);
    Console.WriteLine($"[PASS] FileManagerAotEngine 存在");
    var t5 = typeof(FileManager.AOT.IFileManagerService);
    Console.WriteLine($"[PASS] IFileManagerService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(FileManager.AOT.FileManagerCommandType);
    Console.WriteLine($"[PASS] FileManagerCommandType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}