#load "version_control_manager.cs"

Console.WriteLine("=== version_control_manager.cs Test ===");

try
{
    // 验证 class: VersionControlManager
    var type_VersionControlManager = Type.GetType("VersionControlManager");
    if (type_VersionControlManager != null)
    {
        Console.WriteLine("[PASS] 类型 VersionControlManager (class) 存在");
        var ctors_VersionControlManager = type_VersionControlManager.GetConstructors();
        Console.WriteLine($"[PASS] VersionControlManager 构造函数数量: {ctors_VersionControlManager.Length}");
        var methods_VersionControlManager = type_VersionControlManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VersionControlManager 公开方法数量: {methods_VersionControlManager.Length}");
        foreach (var m in methods_VersionControlManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VersionControlManager 未找到，尝试无命名空间...");
        type_VersionControlManager = Type.GetType("VersionControlManager");
        if (type_VersionControlManager != null)
            Console.WriteLine("[PASS] 类型 VersionControlManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VersionControlManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SnapshotCompressionEngine
    var type_SnapshotCompressionEngine = Type.GetType("SnapshotCompressionEngine");
    if (type_SnapshotCompressionEngine != null)
    {
        Console.WriteLine("[PASS] 类型 SnapshotCompressionEngine (class) 存在");
        var ctors_SnapshotCompressionEngine = type_SnapshotCompressionEngine.GetConstructors();
        Console.WriteLine($"[PASS] SnapshotCompressionEngine 构造函数数量: {ctors_SnapshotCompressionEngine.Length}");
        var methods_SnapshotCompressionEngine = type_SnapshotCompressionEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SnapshotCompressionEngine 公开方法数量: {methods_SnapshotCompressionEngine.Length}");
        foreach (var m in methods_SnapshotCompressionEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SnapshotCompressionEngine 未找到，尝试无命名空间...");
        type_SnapshotCompressionEngine = Type.GetType("SnapshotCompressionEngine");
        if (type_SnapshotCompressionEngine != null)
            Console.WriteLine("[PASS] 类型 SnapshotCompressionEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SnapshotCompressionEngine 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
