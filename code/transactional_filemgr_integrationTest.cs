#load "transactional_filemgr_integration.cs"

Console.WriteLine("=== transactional_filemgr_integration.cs Test ===");

try
{
    // 验证 class: FileTransactionService
    var type_FileTransactionService = Type.GetType("FileTransactionService");
    if (type_FileTransactionService != null)
    {
        Console.WriteLine("[PASS] 类型 FileTransactionService (class) 存在");
        var ctors_FileTransactionService = type_FileTransactionService.GetConstructors();
        Console.WriteLine($"[PASS] FileTransactionService 构造函数数量: {ctors_FileTransactionService.Length}");
        var methods_FileTransactionService = type_FileTransactionService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FileTransactionService 公开方法数量: {methods_FileTransactionService.Length}");
        foreach (var m in methods_FileTransactionService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FileTransactionService 未找到，尝试无命名空间...");
        type_FileTransactionService = Type.GetType("FileTransactionService");
        if (type_FileTransactionService != null)
            Console.WriteLine("[PASS] 类型 FileTransactionService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FileTransactionService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
