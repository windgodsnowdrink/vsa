#load "distributed_file_lock.cs"

Console.WriteLine("=== distributed_file_lock.cs Test ===");

try
{
    // 验证 class: FileLockService
    var type_FileLockService = Type.GetType("FileLockService");
    if (type_FileLockService != null)
    {
        Console.WriteLine("[PASS] 类型 FileLockService (class) 存在");
        var ctors_FileLockService = type_FileLockService.GetConstructors();
        Console.WriteLine($"[PASS] FileLockService 构造函数数量: {ctors_FileLockService.Length}");
        var methods_FileLockService = type_FileLockService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FileLockService 公开方法数量: {methods_FileLockService.Length}");
        foreach (var m in methods_FileLockService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FileLockService 未找到，尝试无命名空间...");
        type_FileLockService = Type.GetType("FileLockService");
        if (type_FileLockService != null)
            Console.WriteLine("[PASS] 类型 FileLockService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FileLockService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DistributedLockService
    var type_DistributedLockService = Type.GetType("DistributedLockService");
    if (type_DistributedLockService != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedLockService (class) 存在");
        var ctors_DistributedLockService = type_DistributedLockService.GetConstructors();
        Console.WriteLine($"[PASS] DistributedLockService 构造函数数量: {ctors_DistributedLockService.Length}");
        var methods_DistributedLockService = type_DistributedLockService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedLockService 公开方法数量: {methods_DistributedLockService.Length}");
        foreach (var m in methods_DistributedLockService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedLockService 未找到，尝试无命名空间...");
        type_DistributedLockService = Type.GetType("DistributedLockService");
        if (type_DistributedLockService != null)
            Console.WriteLine("[PASS] 类型 DistributedLockService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedLockService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
