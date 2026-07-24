#load "miniexcel_production.cs"

Console.WriteLine("=== miniexcel_production.cs Test ===");

try
{
    // 验证 class: ExcelGenerationService
    var type_ExcelGenerationService = Type.GetType("ExcelGenerationService");
    if (type_ExcelGenerationService != null)
    {
        Console.WriteLine("[PASS] 类型 ExcelGenerationService (class) 存在");
        var ctors_ExcelGenerationService = type_ExcelGenerationService.GetConstructors();
        Console.WriteLine($"[PASS] ExcelGenerationService 构造函数数量: {ctors_ExcelGenerationService.Length}");
        var methods_ExcelGenerationService = type_ExcelGenerationService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExcelGenerationService 公开方法数量: {methods_ExcelGenerationService.Length}");
        foreach (var m in methods_ExcelGenerationService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExcelGenerationService 未找到，尝试无命名空间...");
        type_ExcelGenerationService = Type.GetType("ExcelGenerationService");
        if (type_ExcelGenerationService != null)
            Console.WriteLine("[PASS] 类型 ExcelGenerationService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExcelGenerationService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MemoryStreamPooledPolicy
    var type_MemoryStreamPooledPolicy = Type.GetType("MemoryStreamPooledPolicy");
    if (type_MemoryStreamPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryStreamPooledPolicy (class) 存在");
        var ctors_MemoryStreamPooledPolicy = type_MemoryStreamPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MemoryStreamPooledPolicy 构造函数数量: {ctors_MemoryStreamPooledPolicy.Length}");
        var methods_MemoryStreamPooledPolicy = type_MemoryStreamPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryStreamPooledPolicy 公开方法数量: {methods_MemoryStreamPooledPolicy.Length}");
        foreach (var m in methods_MemoryStreamPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryStreamPooledPolicy 未找到，尝试无命名空间...");
        type_MemoryStreamPooledPolicy = Type.GetType("MemoryStreamPooledPolicy");
        if (type_MemoryStreamPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 MemoryStreamPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryStreamPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
