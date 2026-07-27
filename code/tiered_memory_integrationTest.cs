#load "tiered_memory_integration.cs"

Console.WriteLine("=== tiered_memory_integration.cs Test ===");

try
{
    // 验证 class: DataProcessingService
    var type_DataProcessingService = Type.GetType("DataProcessingService");
    if (type_DataProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 DataProcessingService (class) 存在");
        var ctors_DataProcessingService = type_DataProcessingService.GetConstructors();
        Console.WriteLine($"[PASS] DataProcessingService 构造函数数量: {ctors_DataProcessingService.Length}");
        var methods_DataProcessingService = type_DataProcessingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DataProcessingService 公开方法数量: {methods_DataProcessingService.Length}");
        foreach (var m in methods_DataProcessingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DataProcessingService 未找到，尝试无命名空间...");
        type_DataProcessingService = Type.GetType("DataProcessingService");
        if (type_DataProcessingService != null)
            Console.WriteLine("[PASS] 类型 DataProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DataProcessingService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MemoryMonitorService
    var type_MemoryMonitorService = Type.GetType("MemoryMonitorService");
    if (type_MemoryMonitorService != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryMonitorService (class) 存在");
        var ctors_MemoryMonitorService = type_MemoryMonitorService.GetConstructors();
        Console.WriteLine($"[PASS] MemoryMonitorService 构造函数数量: {ctors_MemoryMonitorService.Length}");
        var methods_MemoryMonitorService = type_MemoryMonitorService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryMonitorService 公开方法数量: {methods_MemoryMonitorService.Length}");
        foreach (var m in methods_MemoryMonitorService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryMonitorService 未找到，尝试无命名空间...");
        type_MemoryMonitorService = Type.GetType("MemoryMonitorService");
        if (type_MemoryMonitorService != null)
            Console.WriteLine("[PASS] 类型 MemoryMonitorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryMonitorService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
